﻿using ENBManager.Infrastructure.BusinessEntities.Dialogs.Base;
using System;

namespace ENBManager.Infrastructure.BusinessEntities.Dialogs
{
    public class ProgressDialog : BaseDialog, IDisposable
    {
        #region Private Members

        private int _progress;

        #endregion

        #region Public Properties

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public bool IsIndeterminate { get; set; }

        #endregion

        #region Constructor

        public ProgressDialog(bool isIndeterminate, string message = "")
            : base (message)
        {
            IsIndeterminate = isIndeterminate;

            if (isIndeterminate)
                Progress = 0;
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
