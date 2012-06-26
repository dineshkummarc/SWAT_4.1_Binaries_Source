/********************************************************************************
    This file is part of Simple Web Automation Toolkit, 
    Copyright (C) 2007 by Ultimate Software, Inc. All rights reserved.

    Simple Web Automation Toolkit is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License version 3 as published by
    the Free Software Foundation; 

    Simple Web Automation Toolkit is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

 */

/********************************************************************************/


using System;
using System.ComponentModel;
using System.Threading;

namespace SWAT
{
    public class DialogWatcher : IDisposable
    {
        #region Constructor

        public DialogWatcher(Browser browser)
        {
            this.browser = browser;
            FoundDialog = false;
            InitializeDialogWatcher();
            Start();
        }

        #endregion

        #region Private Variables

        private readonly Browser browser;
        private BackgroundWorker backgroundWorker;

        #endregion

        #region Properties

        public bool FoundDialog { get; set; }

        #endregion

        #region Public Methods

        public void Start()
        {
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        public void Stop()
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }
        }

        #endregion

        #region Helper Methods

        private void InitializeDialogWatcher()
        {
            backgroundWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            backgroundWorker.DoWork += dialogWatcher_DoWork;
            backgroundWorker.ProgressChanged += dialogWatcher_ProgressChanged;
        }

        #endregion

        #region Event Handlers

        void dialogWatcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FoundDialog = Convert.ToBoolean(e.ProgressPercentage);
        }

        void dialogWatcher_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                backgroundWorker.ReportProgress(browser.GetJSDialogHandle(1) != IntPtr.Zero ? 1 : 0);
                Thread.Sleep(25);
            }
        }

        #endregion

        #region IDisposable Members

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.

                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                Stop();

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion
    }
}
