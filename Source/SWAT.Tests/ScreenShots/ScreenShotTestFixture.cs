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
using System.IO;
using System.Reflection;
using NUnit.Framework;
 
using SWAT.Reflection;

namespace SWAT.Tests.ScreenShots
{
    
    public abstract class ScreenShotTestFixture : BrowserTestFixture
    {
        public ScreenShotTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        protected Browser GetBrowserObject()
        {
            return (Browser) ReflectionHelper.GetField<object>(_browser, "_browser");
        }

        protected IntPtr GetContentHandle(Browser obj)
        {
            return ReflectionHelper.InvokeMethod<IntPtr>(obj, "GetContentHandle");
        }

        [Test]
        public void TestScreenShotWorksForBrowserStandardDoctypePage()
        {
            if (_browserType == BrowserType.Safari)
               Assert.Ignore("This test is irrelevant for Safari");

            string screenshot = String.Empty;

            try
            {
                ErrorSnapShot test = new ErrorSnapShot(GetBrowserObject(), _browserType);
                screenshot = test.CaptureBrowser(@"C:\SWAT\trunk\SWAT.Tests\TestPages\", "FakeCommand", GetContentHandle(GetBrowserObject()));
                Assert.IsTrue(File.Exists(screenshot.Substring(22)), "Browser screenshot was not created.");
            }
            finally
            {
                // Clean up
                if(!String.IsNullOrEmpty(screenshot))
                    File.Delete(screenshot.Substring(22));
            }
        }

        [Test]
        public void TestScreenShotWorksForBrowserInOlderDoctypePage()
        {
            if (_browserType == BrowserType.Safari)
                Assert.Ignore("This test is irrelevant for Safari");

            string screenshot = String.Empty;

            try
            {
                _browser.NavigateBrowser(getTestPage("OlderDoctypePage.htm"));

                SWAT.ErrorSnapShot test = new SWAT.ErrorSnapShot(GetBrowserObject() as IDocumentInfo, _browserType);
                screenshot = test.CaptureBrowser(@"C:\SWAT\trunk\SWAT.Tests\TestPages\", "FakeCommand", this.GetContentHandle(GetBrowserObject()));

                Assert.IsTrue(File.Exists(screenshot.Substring(22)), "Browser screenshot was not created.");
            }
            finally
            {
                // Clean up
                if (!String.IsNullOrEmpty(screenshot))
                    File.Delete(screenshot.Substring(22));
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void TestScreenShotCaptureBrowserAllScreensFailedTest()
        {
            if (_browserType == BrowserType.Safari)
                Assert.Ignore("This test is irrelevant for Safari");

            this.NavigateToSwatTestPage();
            SWAT.ErrorSnapShot capture = new SWAT.ErrorSnapShot(GetBrowserObject() as IDocumentInfo, _browserType);
            string returnValue = capture.CaptureBrowser("C:\test", "Any Command", this.GetContentHandle(GetBrowserObject())); // this will fail because is not a folder and return an error message.

            Assert.AreEqual("Illegal characters in path.", returnValue);

            string filePath = @"C:\SWAT\trunk\SWAT.Tests\TestPages\";
            string command = "ConnectToMssql";
            string expected = string.Format("ScreenShot was not taken because \"{0}\" does not require an interface.", command);

            returnValue = capture.CaptureBrowser(filePath, command, this.GetContentHandle(GetBrowserObject()));

            Assert.AreEqual(expected, returnValue);  
        }

         
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void TestTakeScreenShot(bool screens, bool browser)
        {
			_browser.NavigateBrowser("www.en.wikipedia.com");

            bool tmp1 = SWAT.ScreenShotSettings.ScreenShotAllScreens;
            bool tmp2 = SWAT.ScreenShotSettings.ScreenShotBrowser;
            string tmp3 = SWAT.ScreenShotSettings.SnapShotFolder;
            bool tmp4 = SWAT.ScreenShotSettings.SnapShotOption;

            SWAT.ScreenShotSettings.ScreenShotAllScreens = screens;
            SWAT.ScreenShotSettings.ScreenShotBrowser = browser;
            SWAT.ScreenShotSettings.SnapShotFolder = @"C:\SWAT\trunk\SWAT.Tests\TestPages\";
            SWAT.ScreenShotSettings.SnapShotOption = true;
            SWAT.UserConfigHandler.Save();

            try
            {
                string screenshot = _browser.TakeScreenshot("RandomPrefix");
                string filePath = "";
                if (_browserType == BrowserType.Safari)
                {
                    string[] pathSeperated = screenshot.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    filePath = string.Format("\\\\{0}\\SWAT Screenshots\\{1}", SWAT.SafariSettings.SafariAddress, pathSeperated[pathSeperated.Length - 1]);
                }
                else
                    filePath = screenshot.Substring(22);


                Assert.IsTrue(File.Exists(filePath), "Browser screenshot was not created.");
                File.Delete(filePath);
            }
            finally
            {
                SWAT.ScreenShotSettings.ScreenShotAllScreens = tmp1;
                SWAT.ScreenShotSettings.ScreenShotBrowser = tmp2;
                SWAT.ScreenShotSettings.SnapShotFolder = tmp3;
                SWAT.ScreenShotSettings.SnapShotOption = tmp4;
                SWAT.UserConfigHandler.Save();
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void TestTakeScreenShotFails()
        {
            bool tmp1 = SWAT.ScreenShotSettings.ScreenShotAllScreens;
            bool tmp2 = SWAT.ScreenShotSettings.ScreenShotBrowser;
            string tmp3 = SWAT.ScreenShotSettings.SnapShotFolder;
            bool tmp4 = SWAT.ScreenShotSettings.SnapShotOption;

            SWAT.ScreenShotSettings.ScreenShotAllScreens = false;
            SWAT.ScreenShotSettings.ScreenShotBrowser = true;
            SWAT.ScreenShotSettings.SnapShotFolder = @"C:\SWAT\trunk\SWAT.Tests\TestPages\";
            SWAT.ScreenShotSettings.SnapShotOption = true;
            SWAT.UserConfigHandler.Save();

            try
            {
                _browser.KillAllOpenBrowsers();

                string screenshot = _browser.TakeScreenshot("RandomPrefix");

                string filePath = "";
                if (_browserType == BrowserType.Safari)
                {
                    string[] pathSeperated = screenshot.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    filePath = string.Format("\\\\{0}\\SWAT Screenshots\\RandomPrefix", SWAT.SafariSettings.SafariAddress);
                }
                else
                    filePath = screenshot.Substring(22);    

                bool fileExists = File.Exists(filePath);
                if (fileExists)
                {
                    File.Delete(filePath);
                }
                Assert.IsFalse(fileExists, "Browser screenshot was created.");
            }
            finally
            {
                SWAT.ScreenShotSettings.ScreenShotAllScreens = tmp1;
                SWAT.ScreenShotSettings.ScreenShotBrowser = tmp2;
                SWAT.ScreenShotSettings.SnapShotFolder = tmp3;
                SWAT.ScreenShotSettings.SnapShotOption = tmp4;
                SWAT.UserConfigHandler.Save();
                this.OpenSwatTestPage();
            }
        }
        
        [Test]
        public void TestScreenShotUpdatesSnapShotFolderWithMachineName()
        {
            if (_browserType == BrowserType.Safari)
            {
                Assert.Ignore("This test is irrelevant for Safari since it doesn't use the same SnapShotFolder property.");
            }

            string screenShotMessage = " ScreenShot saved in : ";
			string assemblyPath = String.Format("\\\\{0}\\{1}\\", Environment.MachineName, CurrentSWATAssemblyPath.Replace(@":", @"$"));
			string configName = "SWAT.user.config";
            string configPath = String.Format("{0}{1}", assemblyPath, configName);
            string backupConfigName = "Orig_SWAT.user.config";
			string backupConfigPath = String.Format("{0}{1}", assemblyPath, backupConfigName);
            string testConfigPath = getTestFilePath(configName);

            File.Move(configPath, configPath.Replace(configName, backupConfigName));
            File.Copy(testConfigPath, configPath);

            SWAT.ScreenShotSettings.ScreenShotAllScreens = false;
            SWAT.ScreenShotSettings.ScreenShotBrowser = true;
            SWAT.ScreenShotSettings.SnapShotOption = true;
            SWAT.UserConfigHandler.Save();

            _browser.NavigateBrowser("www.en.wikipedia.com");

            try
            {
                string screenshot = _browser.TakeScreenshot("RandomPrefix");
                string filePath = screenshot.Substring(screenShotMessage.Length);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }   
                             
                Assert.IsFalse(SWAT.ScreenShotSettings.SnapShotFolder.Contains(@":"));               
            }
            finally
            {
                File.Delete(configPath);
                File.Move(backupConfigPath, configPath);
				File.Delete(backupConfigPath);
      
                this.NavigateToSwatTestPage();              
            }
        }

        [Test]
        public void TestNonUiCommandList()
        {
            ErrorSnapShot screen = new ErrorSnapShot();
            HashSet<string> nonUiCommands = ReflectionHelper.GetField<HashSet<string>>(screen, "NonUiCommands");

            if (nonUiCommands == null)
                throw new NullReferenceException(
                    "NonUiCommands is null which means it is missing from the ErrorSnapshot class.");

            Assert.IsTrue(nonUiCommands.Count >= 44, "nonUiCommands does not contain all the excluded non ui commands");

            Type t = typeof (DataAccess.MSSql);
            MethodInfo[] methods = t.GetMethods();

            foreach (MethodInfo methodInfo in methods)
                Assert.IsTrue(nonUiCommands.Contains(methodInfo.Name.ToLower()), string.Format("NonUiCommands is missing method {0}", methodInfo.Name));
        }

        [Test]
        public void TestAlternativeScreenShotMethod()
        {

            _browser.NavigateBrowser(getTestPage("BlackScreenShot.html"));

            bool tmp1 = SWAT.ScreenShotSettings.ScreenShotAllScreens;
            bool tmp2 = SWAT.ScreenShotSettings.ScreenShotBrowser;
            string tmp3 = SWAT.ScreenShotSettings.SnapShotFolder;
            bool tmp4 = SWAT.ScreenShotSettings.SnapShotOption;

            SWAT.ScreenShotSettings.ScreenShotAllScreens = false;
            SWAT.ScreenShotSettings.ScreenShotBrowser = true;
            SWAT.ScreenShotSettings.SnapShotFolder = @"C:\SWAT\trunk\SWAT.Tests\TestPages\";
            SWAT.ScreenShotSettings.SnapShotOption = true;
            SWAT.UserConfigHandler.Save();

            try
            {
                string screenshot = _browser.TakeScreenshot("RandomPrefix");
                string filePath = "";
                if (_browserType == BrowserType.Safari)
                {
                    string[] pathSeperated = screenshot.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    filePath = string.Format("\\\\{0}\\SWAT Screenshots\\{1}", SWAT.SafariSettings.SafariAddress, pathSeperated[pathSeperated.Length - 1]);
                }
                else
                    filePath = screenshot.Substring(22);


                Assert.IsTrue(File.Exists(filePath), "Browser screenshot was not created.");
                File.Delete(filePath);
            }
            finally
            {
                SWAT.ScreenShotSettings.ScreenShotAllScreens = tmp1;
                SWAT.ScreenShotSettings.ScreenShotBrowser = tmp2;
                SWAT.ScreenShotSettings.SnapShotFolder = tmp3;
                SWAT.ScreenShotSettings.SnapShotOption = tmp4;
                SWAT.UserConfigHandler.Save();
                this.NavigateToSwatTestPage();
            }
        }
    }
}