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
using System.Collections.Generic;
using System.Text;

namespace SWAT_Editor.Controls.Options_Control
{
    public sealed class SettingsDataEntity
    {
        #region Properties
                
        public string FitnesseRootDirectory
        {
            get
            {
                return SWAT.FitnesseSettings.FitnesseRootDirectory;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    SWAT.FitnesseSettings.FitnesseRootDirectory = value;
            }
        }

        public string ConnectionTimeout
        {
            get
            {
                return SWAT_Editor.Properties.Settings.Default.ConnectionTimeout.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                    SWAT_Editor.Properties.Settings.Default.ConnectionTimeout = int.Parse(value);
            }
        }


        public bool LoadBlankForm
        {
            get
            {
                return SWAT_Editor.Properties.Settings.Default.LoadBlankForm;
            }
            set
            {
                SWAT_Editor.Properties.Settings.Default.LoadBlankForm = value;
            }
        }

        public bool OverrideTestBrowser
        {
            get
            {
                return SWAT_Editor.Properties.Settings.Default.OverrideTestBrowser;
            }
            set
            {
                SWAT_Editor.Properties.Settings.Default.OverrideTestBrowser = value;
            }
        }

        public bool AutosaveEnabled
        {
            get
            {
                return SWAT_Editor.Properties.Settings.Default.AutosaveEnabled;
            }
            set
            {
                SWAT_Editor.Properties.Settings.Default.AutosaveEnabled = value;
            }
        }

        public string AutosaveFrequency
        {
            get
            {
                return SWAT_Editor.Properties.Settings.Default.AutosaveFrequency.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                    SWAT_Editor.Properties.Settings.Default.AutosaveFrequency = int.Parse(value);
            }
        }

        public bool CtrlRightClickShowRecMenu
        {
            get
            {
                return SWAT_Editor.Properties.Settings.Default.showIEAppMenu;
            }
            set
            {
                SWAT_Editor.Properties.Settings.Default.showIEAppMenu = value;
            }
        }

        public string AttachToWindowTimeout
        {
            get
            {
                return SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout = int.Parse(value);
            }
        }

        public string WaitForBrowserTimeout
        {
            get
            {
                return SWAT.DefaultTimeouts.WaitForBrowserTimeout.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                SWAT.DefaultTimeouts.WaitForBrowserTimeout = int.Parse(value);
            }
        }

        public string AssertBrowserExistsTimeout
        {
            get
            {
                return SWAT.DefaultTimeouts.AssertBrowserExists.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                    SWAT.DefaultTimeouts.AssertBrowserExists = int.Parse(value);
            }
        }

        public bool GetInformativeExceptions
        {
            get
            {
                return SWAT.WantInformativeExceptions.GetInformativeExceptions;
            }
            set
            {
                SWAT.WantInformativeExceptions.GetInformativeExceptions = value;
            }
        }

        public string DelayBetweenCommands
        {
            get
            {
                return SWAT.WantDelayBetweenCommands.DelayBetweenCommands.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) >= 0)
                    SWAT.WantDelayBetweenCommands.DelayBetweenCommands = int.Parse(value);
            }
        }

        public bool TakeSnapshots
        {
            get
            {
                return SWAT.ScreenShotSettings.SnapShotOption;
            }
            set
            {
                SWAT.ScreenShotSettings.SnapShotOption = value;
            }
        }

        public string ImageFileDirectory
        {
            get
            {
                return SWAT.ScreenShotSettings.SnapShotFolder;
            }
            set
            {
                if(!string.IsNullOrEmpty(value))
                    SWAT.ScreenShotSettings.SnapShotFolder = value;
            }
        }

        public bool WindowOnlyScreenshot
        {
            get
            {
                return SWAT.ScreenShotSettings.ScreenShotBrowser;
            }
            set
            {
                SWAT.ScreenShotSettings.ScreenShotBrowser = value;
            }
        }

        public bool AllScreensSnapshot
        {
            get
            {
                return SWAT.ScreenShotSettings.ScreenShotAllScreens;
            }
            set
            {
                SWAT.ScreenShotSettings.ScreenShotAllScreens = value;
            }
        }

        public string SafariPort
        {
            get
            {
                return SWAT.SafariSettings.SafariPort.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                SWAT.SafariSettings.SafariPort = int.Parse(value);
            }
        }

        public string SafariAddress
        {
            get
            {
                return SWAT.SafariSettings.SafariAddress;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                SWAT.SafariSettings.SafariAddress = value;
            }
        }

        public string FindElement
        {
            get
            {
                return SWAT.DefaultTimeouts.FindElementTimeout.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                    SWAT.DefaultTimeouts.FindElementTimeout = int.Parse(value);
            }
        }

        public string WaitForDocumentLoadTimeOut
        {
            get
            {
                return SWAT.DefaultTimeouts.WaitForDocumentLoadTimeout.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                    SWAT.DefaultTimeouts.WaitForDocumentLoadTimeout = int.Parse(value);
            }
        }

        public string DoesElementExistTimeOut
        {
            get
            {
                return SWAT.DefaultTimeouts.DoesElementExistTimeout.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                SWAT.DefaultTimeouts.DoesElementExistTimeout = int.Parse(value);
            }
        }

        public string DoesElementNotExistTimeOut
        {
            get
            {
                return SWAT.DefaultTimeouts.DoesElementNotExistTimeout.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                    SWAT.DefaultTimeouts.DoesElementNotExistTimeout = int.Parse(value);
            }
        }

        public string DoesElementNotExistLookTimeout
        {
            get
            {
                return SWAT.DefaultTimeouts.DoesElementNotExistLookTimeout.ToString();
            }
            set
            {
                if (checkStringIsInt(ref value) > 0)
                SWAT.DefaultTimeouts.DoesElementNotExistLookTimeout = int.Parse(value);
            }
        }

        public string AutosaveFrequencyDirectory 
        {
            get
            {
                return SWAT_Editor.Properties.Settings.Default.AutosaveDirectory;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                SWAT_Editor.Properties.Settings.Default.AutosaveDirectory = value;
            }
        }

        public bool UseAutoComplete
        {
            get
            {
                return SWAT_Editor.Properties.Settings.Default.UseAutoComplete;
            }
            set
            {
                SWAT_Editor.Properties.Settings.Default.UseAutoComplete = value;
            }
        }

        public bool MinimizeToTray
        {
            get
            {
                return SWAT_Editor.Properties.Settings.Default.MinimizeToTray;
            }
            set
            {
                SWAT_Editor.Properties.Settings.Default.MinimizeToTray = value;
            }
        }

        public string FirefoxRootDirectory
        {
            get
            {
                return SWAT.BrowserPaths.FirefoxRootDirectory;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    SWAT.BrowserPaths.FirefoxRootDirectory = value;
            }
        }
        public bool CloseBrowsersBeforeTestStart
        {
            get
            {
                return SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart;
            }
            set
            {
                SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = value;
            }
        }

        public bool SuspendTestOnFail
        {
            get
            {
                return SWAT.WantSuspendOnFail.SuspendTestOnFail;
            }
            set
            {
                SWAT.WantSuspendOnFail.SuspendTestOnFail = value;
            }
        }

		public bool IEAutoHighlight
		{
			get
			{
				return SWAT.IESettings.HighlightElementsAsTestsRun;
			}
			set
			{
				SWAT.IESettings.HighlightElementsAsTestsRun = value;
			}
		}

        #endregion

        private int checkStringIsInt(ref string value)
        {
            int result;
            if (int.TryParse(value, out result) && ((value == "0" && result == 0) || (result > 0)))
                return result;
            else
                return -1;
        }
    }
}
