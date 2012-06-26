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
using NUnit.Framework;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace SWAT.Tests.OpenNavigateCloseBrowser
{
    [TestFixture]
    [Category("FireFox")]
    public class FireFoxTests : OpenNavigateCloseBrowserTestFixture
    {
        public FireFoxTests()
            : base(BrowserType.FireFox)
        {

        }

        [Test]
        [ExpectedException(typeof(IllegalDirectoryException))]
        public void FirefoxOpenBrowserExceptionTest()
        {
            string configPath = SWAT.BrowserPaths.FirefoxRootDirectory;

            SWAT.BrowserPaths.FirefoxRootDirectory = @"C:\Documents and Settings\Default User";
            SWAT.UserConfigHandler.Save();

            try
            {
                _browser.OpenBrowser();
            }
            finally
            {
                SWAT.BrowserPaths.FirefoxRootDirectory = configPath;
                SWAT.UserConfigHandler.Save();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        [ExpectedException(typeof(BrowserNotInstalledException))]
        public void FirefoxNotInstalledExceptionThrownTest()
        {
            _browser.KillAllOpenBrowsers();

            // move SWAT.user.config temporarily            
            string configFilePath = GetUserConfigFilePath();
            File.Move(configFilePath, configFilePath + "Temp");
                                          
            // retrieve the registry key fore firefox
            RegistryKey key = Registry.LocalMachine;
            key = key.OpenSubKey(@"Software\Mozilla\Mozilla Firefox", true);

            string originalPath = string.Empty;

            if (key != null)
            {
                

                // backup the firefox path from the registry and delete it
                string[] arr = key.GetSubKeyNames();
                string versionFolder = arr[arr.Length - 1];
                key = key.OpenSubKey(versionFolder + @"\Main", true);
                originalPath = (string)key.GetValue("PathToExe");
                key.DeleteValue("PathToExe");

                try // test that the proper exception is thrown
                {
                    _browser.OpenBrowser();
                }
                finally
                {
                    // restore the firefox path to the registry
                    key.SetValue("PathToExe", originalPath);

                    // move SWAT.user.config back to its original location
                    File.Move(configFilePath + "Temp", configFilePath);

                    this.OpenSwatTestPage();
                }
            }
        }           

        private void killFirefox()
        {
            DateTime timer = DateTime.Now.AddSeconds(5);

            while (Process.GetProcessesByName("firefox").Length == 0)
                if (DateTime.Now > timer)
                    return;

            Thread.Sleep(100);
            Process.GetProcessesByName("firefox")[0].Kill();
        }     

        [Test]
        [ExpectedException(typeof(BrowserDidNotLoadException))]
        public void FirefoxOpenBrowserTimeoutTest()
        {
            _browser.KillAllOpenBrowsers();

            Thread thread = new Thread(new ThreadStart(killFirefox));
            thread.Start();

            try
            {
                _browser.OpenBrowser();

                thread.Join();
            }
            finally
            {
                // Clean up
                this.OpenSwatTestPage();
            }
        }
    }
}
