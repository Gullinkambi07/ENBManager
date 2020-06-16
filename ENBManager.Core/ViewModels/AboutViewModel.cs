﻿using ENBManager.Infrastructure.Constants;
using ENBManager.Localization.Strings;
using NLog;
using Prism.Services.Dialogs;
using System;
using System.Reflection;

namespace ENBManager.Core.ViewModels
{
    public class AboutViewModel : IDialogAware
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Properties

        public string Name => ApplicationName.NAME;
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        #endregion

        #region IDialogAware Implementation

        public string Title => Strings.ABOUT;

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            _logger.Info("Closed");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _logger.Info("Opened");
        } 

        #endregion
    }
}
