﻿using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.BusinessEntities.Dialogs;
using ENBManager.Infrastructure.Constants;
using ENBManager.Infrastructure.Exceptions;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Models;
using ENBManager.Modules.Shared.ViewModels.Base;
using ENBManager.Modules.Shared.Views;
using NLog;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class PresetsViewModel : TabItemBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationManager<AppSettings> _configurationManager;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPresetManager _presetManager;

        private GameModule _game;

        #endregion

        #region Public Properties

        public ObservableCollection<Preset> Presets { get; set; }

        #endregion

        #region Helper Properties

        private bool _listPresetView;
        public bool ListPresetView
        {
            get { return _listPresetView; }
            set
            {
                if (!value && !_gridPresetView)
                    value = true;

                _listPresetView = value;
                RaisePropertyChanged();

                _configurationManager.Settings.DefaultPresetView = value;
                _configurationManager.SaveSettings();
            }
        }

        private bool _gridPresetView;
        public bool GridPresetView
        {
            get { return _gridPresetView; }
            set
            {
                if (!value && !_listPresetView)
                    value = true;

                _gridPresetView = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public DelegateCommand<Preset> ActivatePresetCommand { get; }
        public DelegateCommand<Preset> RenamePresetCommand { get; }
        public DelegateCommand<Preset> DeletePresetCommand { get; }
        public DelegateCommand SaveCurrentPresetCommand { get; }
        public DelegateCommand AddPresetCommand { get; }

        #endregion

        #region Constructor

        public PresetsViewModel(
            IConfigurationManager<AppSettings> configurationManager,
            IDialogService dialogService,
            IEventAggregator eventAggregator,
            IPresetManager presetManager)
            : base(eventAggregator)
        {
            _configurationManager = configurationManager;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _presetManager = presetManager;

            ActivatePresetCommand = new DelegateCommand<Preset>(async (x) => await OnActivatePresetCommand(x));
            RenamePresetCommand = new DelegateCommand<Preset>(async (x) => await OnRenamePresetCommand(x));
            DeletePresetCommand = new DelegateCommand<Preset>(async (x) => await OnDeletePresetCommand(x));
            SaveCurrentPresetCommand = new DelegateCommand(async () => await OnSaveCurrentPresetCommand());
            AddPresetCommand = new DelegateCommand(async () => await OnAddPresetCommand());

            _listPresetView = _configurationManager.Settings.DefaultPresetView;
            _gridPresetView = !_listPresetView;

            _logger.Debug($"{nameof(PresetsViewModel)} initialized");
        }

        #endregion

        #region Private Methods

        private async Task OnActivatePresetCommand(Preset preset)
        {
            using (var dialog = new ProgressDialog(true))
            {
                _ = dialog.OpenAsync();

                foreach (var other in Presets.Where(x => x.Name != preset.Name))
                {
                    if (other.IsActive)
                    {
                        await _presetManager.DeactivatePresetAsync(_game, other);
                        other.IsActive = false;
                    }
                }

                if (preset.IsActive)
                    _game.Settings.ActivePreset = preset.Name;
                else
                    _game.Settings.ActivePreset = string.Empty;

                var configManager = new ConfigurationManager<GameSettings>(_game.Settings);
                configManager.SaveSettings();

                if (preset.IsActive)
                {
                    await _presetManager.ActivatePresetAsync(_game, preset);
                    _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish($"{preset.Name} {Strings.PRESET_ACTIVATED}");

                    _logger.Info($"Preset {preset.Name} activated");
                }
                else
                {
                    await _presetManager.DeactivatePresetAsync(_game, preset);
                    _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish(Strings.NO_PRESET_ACTIVE);

                    _logger.Info($"No preset activated");
                } 
            }
        }

        private async Task OnRenamePresetCommand(Preset preset)
        {
            var dialog = new InputDialog(Strings.ENTER_A_NEW_NAME, preset.Name);
            var result = await dialog.OpenAsync();

            if (result)
            {
                try
                {
                    preset.FullPath = _presetManager.RenamePreset(preset, dialog.Value);
                    preset.Name = dialog.Value;

                    _logger.Info("Preset renamed");
                }
                catch (ArgumentNullException ex)
                {
                    _logger.Warn(ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    _logger.Warn(ex);
                }
                catch (ArgumentException ex)
                {
                    _logger.Debug(ex);
                    return;
                }
                catch (IdenticalNameException ex)
                {
                    _logger.Debug(ex.Message);
                    await new MessageDialog(Strings.AN_ITEM_WITH_THIS_NAME_ALREADY_EXISTS).OpenAsync();
                }
            }
        }

        private async Task OnDeletePresetCommand(Preset preset)
        {
            var dialog = new ConfirmDialog(Strings.YOU_ARE_ABOUT_TO_DELETE_THIS_ITEM_ARE_YOU_SURE);
            var result = await dialog.OpenAsync();

            if (result)
            {
                try
                {
                    _presetManager.DeletePreset(preset);
                    _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish(Strings.PRESET_DELETED);
                    _logger.Info("Preset deleted");
                }
                catch (DirectoryNotFoundException ex)
                {
                    _logger.Warn(ex);
                }

                Presets.Remove(preset);
            }
        }

        private async Task OnSaveCurrentPresetCommand()
        {
            // Create preset
            var newPreset = _presetManager.CreateExistingPreset(_game);

            // Validate file count
            if (newPreset.Files.Count() == 0)
            {
                var messageDialog = new MessageDialog(Strings.INVALID_PRESET_NO_FILES);
                await messageDialog.OpenAsync();

                _logger.Debug($"OnSaveCurrentPresetCommand: {Strings.INVALID_PRESET_NO_FILES}");
                return;
            }

            // Prompt name
            var inputDialog = new InputDialog(Strings.ENTER_A_NEW_NAME);
            var result = await inputDialog.OpenAsync();

            if (result)
            {
                using (var dialog = new ProgressDialog(true))
                {
                    try
                    {
                        _ = dialog.OpenAsync();

                        // Save preset
                        newPreset.Name = inputDialog.Value;
                        await _presetManager.SaveCurrentPresetAsync(_game, newPreset);

                        // Reload preset
                        newPreset = await _presetManager.GetPresetAsync(_game, newPreset.Name);
                        Presets.Add(newPreset);

                        _logger.Info($"Preset {newPreset.Name} added");

                        _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish(Strings.PRESET_ADDED);
                    }
                    catch (IdenticalNameException ex)
                    {
                        await new MessageDialog(Strings.AN_ITEM_WITH_THIS_NAME_ALREADY_EXISTS).OpenAsync();
                        _logger.Debug(ex.Message);
                    }
                }
            }
        }

        private async Task OnAddPresetCommand()
        {
            _logger.Debug(nameof(OnAddPresetCommand));

            var dp = new DialogParameters();
            dp.Add("GameModule", _game);
            _dialogService.ShowDialog(nameof(AddPresetDialog), dp, (dr) =>
            {
                if (dr.Result == ButtonResult.OK)
                {
                    Presets = new ObservableCollection<Preset>(_presetManager.GetPresets(Paths.GetPresetsDirectory(_game.Module)));
                    var activePreset = Presets.FirstOrDefault(x => x.Name == _game.Settings.ActivePreset);
                    if (activePreset != null)
                        activePreset.IsActive = true;

                    RaisePropertyChanged(nameof(Presets));
                    _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish(Strings.PRESET_ADDED);
                    _logger.Info($"Preset added");
                }
            });

            await Task.CompletedTask;
        }

        #endregion

        #region TabItemBase Override

        public override string Name => Strings.PRESETS;

        protected override void OnModuleActivated(GameModule game)
        {
            _game = game;
            Presets = new ObservableCollection<Preset>(game.Presets);
            RaisePropertyChanged(nameof(Presets));

            _logger.Debug($"Module {game.Module} activated");
        } 

        #endregion
    }
}
