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
using SWAT;
using System.IO;
using System.Reflection;
using System.Xml;

namespace SWAT.Tests.Configuration
{
    [TestFixture]
    [Category("Misc")]
    public class UserSettingsTestFixture
    {
        [Test]
        public void TestSafariSettings()
        {
            string tmp1 = SWAT.SafariSettings.SafariAddress;
            int tmp2 = SWAT.SafariSettings.SafariPort;
            int tmp3 = SWAT.SafariSettings.MacResponseTimeout;

            try
            {
                SWAT.SafariSettings.SafariAddress = "testAddress";
                SWAT.SafariSettings.SafariPort = 1;
                SWAT.SafariSettings.MacResponseTimeout = 1;
                SWAT.UserConfigHandler.Save();

                Assert.AreEqual(SWAT.SafariSettings.SafariAddress, "testAddress");
                Assert.AreEqual(SWAT.SafariSettings.SafariPort, 1);
                Assert.AreEqual(SWAT.SafariSettings.MacResponseTimeout, 1);

                SWAT.SafariSettings.SafariPort = -1;
                SWAT.SafariSettings.MacResponseTimeout = -1;
                SWAT.UserConfigHandler.Save();

                Assert.AreEqual(SWAT.SafariSettings.SafariAddress, "testAddress");
                Assert.AreEqual(SWAT.SafariSettings.SafariPort, 1);
                Assert.AreEqual(SWAT.SafariSettings.MacResponseTimeout, 1);
            }
            finally
            {
                SWAT.SafariSettings.SafariAddress = tmp1;
                SWAT.SafariSettings.SafariPort = tmp2;
                SWAT.SafariSettings.MacResponseTimeout = tmp3;
                SWAT.UserConfigHandler.Save();
            }
        }

        [Test]
        public void TestDefaultTimeouts()
        {            
            int tmp1 = SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout;
            int tmp2 = SWAT.DefaultTimeouts.DoesElementExistTimeout;
            int tmp3 = SWAT.DefaultTimeouts.DoesElementNotExistLookTimeout;
            int tmp4 = SWAT.DefaultTimeouts.DoesElementNotExistTimeout;
            int tmp5 = SWAT.DefaultTimeouts.FindElementTimeout;
            int tmp6 = SWAT.DefaultTimeouts.WaitForBrowserTimeout;
            int tmp7 = SWAT.DefaultTimeouts.WaitForDocumentLoadTimeout;
            int tmp8 = SWAT.DefaultTimeouts.AssertBrowserExists;


            SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout = 1;
            SWAT.DefaultTimeouts.DoesElementExistTimeout = 1;
            SWAT.DefaultTimeouts.DoesElementNotExistLookTimeout = 1;
            SWAT.DefaultTimeouts.DoesElementNotExistTimeout = 1;
            SWAT.DefaultTimeouts.FindElementTimeout = 1;
            SWAT.DefaultTimeouts.WaitForBrowserTimeout = 1;
            SWAT.DefaultTimeouts.WaitForDocumentLoadTimeout = 1;
            SWAT.DefaultTimeouts.AssertBrowserExists = 1;
            SWAT.UserConfigHandler.Save();

            Assert.AreEqual(SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.DoesElementExistTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.DoesElementNotExistLookTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.DoesElementNotExistTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.FindElementTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.WaitForBrowserTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.WaitForDocumentLoadTimeout, 30);
            Assert.AreEqual(SWAT.DefaultTimeouts.AssertBrowserExists, 1);

            SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout = -1;
            SWAT.DefaultTimeouts.DoesElementExistTimeout = -1;
            SWAT.DefaultTimeouts.DoesElementNotExistLookTimeout = -1;
            SWAT.DefaultTimeouts.DoesElementNotExistTimeout = -1;
            SWAT.DefaultTimeouts.FindElementTimeout = -1;
            SWAT.DefaultTimeouts.WaitForBrowserTimeout = -1;
            SWAT.DefaultTimeouts.WaitForDocumentLoadTimeout = -35;
            SWAT.DefaultTimeouts.AssertBrowserExists = -1;
            SWAT.UserConfigHandler.Save();

            Assert.AreEqual(SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.DoesElementExistTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.DoesElementNotExistLookTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.DoesElementNotExistTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.FindElementTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.WaitForBrowserTimeout, 1);
            Assert.AreEqual(SWAT.DefaultTimeouts.WaitForDocumentLoadTimeout, 30);
            Assert.AreEqual(SWAT.DefaultTimeouts.AssertBrowserExists, 1);

            SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout = tmp1;
            SWAT.DefaultTimeouts.DoesElementExistTimeout = tmp2;
            SWAT.DefaultTimeouts.DoesElementNotExistLookTimeout = tmp3;
            SWAT.DefaultTimeouts.DoesElementNotExistTimeout = tmp4;
            SWAT.DefaultTimeouts.FindElementTimeout = tmp5;
            SWAT.DefaultTimeouts.WaitForBrowserTimeout = tmp6;
            SWAT.DefaultTimeouts.WaitForDocumentLoadTimeout = tmp7;
            SWAT.DefaultTimeouts.AssertBrowserExists = tmp8;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestWantInformativeExceptions()
        {
            bool tmp1 = SWAT.WantInformativeExceptions.GetInformativeExceptions;

            SWAT.WantInformativeExceptions.GetInformativeExceptions = true;
            SWAT.UserConfigHandler.Save();

            Assert.IsTrue(SWAT.WantInformativeExceptions.GetInformativeExceptions);

            SWAT.WantInformativeExceptions.GetInformativeExceptions = false;
            SWAT.UserConfigHandler.Save();

            Assert.IsFalse(SWAT.WantInformativeExceptions.GetInformativeExceptions);

            SWAT.WantInformativeExceptions.GetInformativeExceptions = tmp1;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestIESettingHighlighElement()
        {
            bool userDefault = SWAT.IESettings.HighlightElementsAsTestsRun;

            SWAT.IESettings.HighlightElementsAsTestsRun = true;
            SWAT.UserConfigHandler.Save();                      
            Assert.IsTrue(SWAT.IESettings.HighlightElementsAsTestsRun);

            SWAT.IESettings.HighlightElementsAsTestsRun = false;
            SWAT.UserConfigHandler.Save();
            Assert.IsFalse(SWAT.IESettings.HighlightElementsAsTestsRun);

            SWAT.IESettings.HighlightElementsAsTestsRun = true;
            SWAT.UserConfigHandler.Save();
            Assert.IsTrue(SWAT.IESettings.HighlightElementsAsTestsRun);

            SWAT.IESettings.HighlightElementsAsTestsRun = false;
            SWAT.UserConfigHandler.Save();
            Assert.IsFalse(SWAT.IESettings.HighlightElementsAsTestsRun);

            SWAT.IESettings.HighlightElementsAsTestsRun = true;
            SWAT.UserConfigHandler.Save();
            Assert.IsTrue(SWAT.IESettings.HighlightElementsAsTestsRun);

            //restore to user default
            SWAT.IESettings.HighlightElementsAsTestsRun = userDefault;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestIEHighlightSetPropertyFails()
        {
            bool userDefault = SWAT.IESettings.HighlightElementsAsTestsRun;
            string input = "man";
            string exception = "";

            SWAT.IESettings.HighlightElementsAsTestsRun= true;
            SWAT.UserConfigHandler.Save();
            Assert.IsTrue(SWAT.IESettings.HighlightElementsAsTestsRun);

            
            WebBrowser wb = new WebBrowser(BrowserType.InternetExplorer);

            try
            {                
                wb.SetConfigurationItem("HighlightElementsAsTestsRun", input);                
            }
            catch (ConfigurationItemException e)
            {
                exception = e.Message;
            }
            Assert.AreEqual(exception, String.Format("Invalid value for this setting: {0}", input)); 
                      
            SWAT.IESettings.HighlightElementsAsTestsRun = userDefault;
            SWAT.UserConfigHandler.Save();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestIEHighlightElementsAsTestsRun(bool passedIn)
        {
            bool userDefault = SWAT.IESettings.HighlightElementsAsTestsRun;

            SWAT.IESettings.HighlightElementsAsTestsRun = passedIn;
            SWAT.UserConfigHandler.Save();

            WebBrowser wb = new WebBrowser(BrowserType.InternetExplorer);

            wb.OpenBrowser();
            wb.NavigateBrowser(String.Format("http://{0}/swat//{1}", Environment.MachineName, "TestPage.htm"));
            wb.SetElementAttribute(IdentifierType.Id, "txtOne", "value", "highlightTest");
            wb.StimulateElement(IdentifierType.Id, "btnClear", "onclick");
            wb.AssertElementExists(IdentifierType.Expression, "id:txtOne;value=", "input");
            wb.StimulateElement(IdentifierType.Id, "btnSetVal", "onclick");
            wb.AssertElementExists(IdentifierType.Expression, "id:txtOne;value=Test1", "input");
            wb.CloseBrowser();

            SWAT.IESettings.HighlightElementsAsTestsRun = userDefault;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestWantDelayBewteenCommands()
        {
            int tmp1 = SWAT.WantDelayBetweenCommands.DelayBetweenCommands;

            SWAT.WantDelayBetweenCommands.DelayBetweenCommands = 1;
            SWAT.UserConfigHandler.Save();

            Assert.AreEqual(SWAT.WantDelayBetweenCommands.DelayBetweenCommands, 1);

            SWAT.WantDelayBetweenCommands.DelayBetweenCommands = -1;
            SWAT.UserConfigHandler.Save();

            Assert.AreEqual(SWAT.WantDelayBetweenCommands.DelayBetweenCommands, 1);

            SWAT.WantDelayBetweenCommands.DelayBetweenCommands = 10;
            SWAT.UserConfigHandler.Save();

            Assert.AreEqual(SWAT.WantDelayBetweenCommands.DelayBetweenCommands, 10);

            SWAT.WantDelayBetweenCommands.DelayBetweenCommands = tmp1;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestScreenShotSettings()
        {
            bool tmp1 = SWAT.ScreenShotSettings.ScreenShotAllScreens;
            bool tmp2 = SWAT.ScreenShotSettings.ScreenShotBrowser;
            string tmp3 = SWAT.ScreenShotSettings.SnapShotFolder;
            bool tmp4 = SWAT.ScreenShotSettings.SnapShotOption;

            SWAT.ScreenShotSettings.ScreenShotAllScreens = true;
            SWAT.ScreenShotSettings.ScreenShotBrowser = true;
            SWAT.ScreenShotSettings.SnapShotFolder = "directory";
            SWAT.ScreenShotSettings.SnapShotOption = true;
            SWAT.UserConfigHandler.Save();

            Assert.IsTrue(SWAT.ScreenShotSettings.ScreenShotAllScreens);
            Assert.IsTrue(SWAT.ScreenShotSettings.ScreenShotBrowser);
            Assert.IsTrue(SWAT.ScreenShotSettings.SnapShotOption);
            Assert.AreEqual(SWAT.ScreenShotSettings.SnapShotFolder, "directory");

            SWAT.ScreenShotSettings.ScreenShotAllScreens = false;
            SWAT.ScreenShotSettings.ScreenShotBrowser = false;            
            SWAT.ScreenShotSettings.SnapShotOption = false;
            SWAT.ScreenShotSettings.SnapShotFolder = @"C:\directory";
            SWAT.UserConfigHandler.Save();

            Assert.IsFalse(SWAT.ScreenShotSettings.ScreenShotAllScreens);
            Assert.IsFalse(SWAT.ScreenShotSettings.ScreenShotBrowser);
            Assert.IsFalse(SWAT.ScreenShotSettings.SnapShotOption);
            Assert.AreEqual(SWAT.ScreenShotSettings.SnapShotFolder, String.Format("\\\\{0}\\C$\\directory", Environment.MachineName));

            SWAT.ScreenShotSettings.ScreenShotAllScreens = tmp1;
            SWAT.ScreenShotSettings.ScreenShotBrowser = tmp2;
            SWAT.ScreenShotSettings.SnapShotFolder = tmp3;
            SWAT.ScreenShotSettings.SnapShotOption = tmp4;
            SWAT.UserConfigHandler.Save();
        }

        [TestCase(@"C:\Program Files\Mozilla Firefox\firefox.exe", "")]
        [TestCase(@"C:\Program Files\Mozilla Firefox\firefox.exe", @"C:\Program Files\Mozilla Firefox")]
        [TestCase(@"C:\Program Files\Mozilla Firefox\firefox.exe", @"C:\Program Files\Mozilla Firefox\")]
        [TestCase(@"C:\Program Files\Mozilla Firefox\firefox.exe", @"C:\Program Files\Mozilla Firefox\firefox.exe")]
        [TestCase(@"C:\Program Files\Firefox Test Folder\firefox.exe", @"C:\Program Files\Firefox Test Folder")]
        public void TestFirefoxExePaths( string expectedPath, string inputPath)
        {
            string configPath = SWAT.BrowserPaths.FirefoxRootDirectory;

            SWAT.BrowserPaths.FirefoxRootDirectory = inputPath;
            SWAT.UserConfigHandler.Save();

            try
            {
                Assert.AreEqual(expectedPath, SWAT.BrowserPaths.FirefoxRootDirectory);
            }
            finally
            {
                SWAT.BrowserPaths.FirefoxRootDirectory = configPath;
                SWAT.UserConfigHandler.Save();
            }
        }

        [Test]
        public void TestFitnesseRootDirectorySetting()
        {
            string dir = @"C:\SWAT\";
            string backup = SWAT.FitnesseSettings.FitnesseRootDirectory;

            try
            {
                SWAT.FitnesseSettings.FitnesseRootDirectory = dir;
                SWAT.UserConfigHandler.Save();

                Assert.AreEqual(dir, SWAT.FitnesseSettings.FitnesseRootDirectory);
            }
            finally
            {
                SWAT.FitnesseSettings.FitnesseRootDirectory = backup;
            }
        }

        [Test]
        public void TestDefaultValues()
        {
            string filePath = getUserConfigFilePath();
        
            File.Move(filePath, filePath + "1");
            try
            {
                Assert.AreEqual(SWAT.SafariSettings.SafariAddress, "120.0.0.1");
                Assert.AreEqual(SWAT.SafariSettings.SafariPort, 9997);
                Assert.AreEqual(SWAT.SafariSettings.MacResponseTimeout, 60);
                Assert.AreEqual(SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout, 50);
                Assert.AreEqual(SWAT.DefaultTimeouts.DoesElementExistTimeout, 15);
                Assert.AreEqual(SWAT.DefaultTimeouts.DoesElementNotExistLookTimeout, 5);
                Assert.AreEqual(SWAT.DefaultTimeouts.DoesElementNotExistTimeout, 15);
                Assert.AreEqual(SWAT.DefaultTimeouts.FindElementTimeout, 15);
                Assert.AreEqual(SWAT.DefaultTimeouts.WaitForBrowserTimeout, 300);
                Assert.AreEqual(SWAT.DefaultTimeouts.WaitForDocumentLoadTimeout, 300);
                Assert.AreEqual(SWAT.DefaultTimeouts.AssertBrowserExists, 10);
                Assert.IsFalse(SWAT.WantInformativeExceptions.GetInformativeExceptions);
                Assert.IsFalse(SWAT.ScreenShotSettings.ScreenShotAllScreens);
                Assert.IsFalse(SWAT.ScreenShotSettings.ScreenShotBrowser);
                Assert.AreEqual(SWAT.ScreenShotSettings.SnapShotFolder, @"\\" + Environment.MachineName + @"\C$\");
                Assert.IsFalse(SWAT.ScreenShotSettings.SnapShotOption);
                Assert.AreEqual(@"C:\Program Files\Mozilla Firefox\firefox.exe", SWAT.BrowserPaths.FirefoxRootDirectory);
                Assert.AreEqual(0, SWAT.WantDelayBetweenCommands.DelayBetweenCommands);
                Assert.IsFalse(SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart);
                Assert.IsFalse(SWAT.WantSuspendOnFail.SuspendTestOnFail);
                Assert.AreEqual(string.Empty, SWAT.FitnesseSettings.FitnesseRootDirectory);
            }
            finally
            {
                File.Move(filePath + "1", filePath);
            }
        }

        [Test]
        public void TestWantCloseBrowsersBeforeTestStart()
        {
            bool tmp1 = SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart;
            try
            {
                SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = true;
                SWAT.UserConfigHandler.Save();

                Assert.IsTrue(SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart);

                SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = false;
                SWAT.UserConfigHandler.Save();

                Assert.IsFalse(SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart);
            }
            finally
            {
                SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = tmp1;
                SWAT.UserConfigHandler.Save();
            }
        }

        [Test]
        public void TestSuspendOnFail()
        {
            bool tmp1 = SWAT.WantSuspendOnFail.SuspendTestOnFail;
            try
            {
                SWAT.WantSuspendOnFail.SuspendTestOnFail = true;
                SWAT.UserConfigHandler.Save();

                Assert.IsTrue(SWAT.WantSuspendOnFail.SuspendTestOnFail);

                SWAT.WantSuspendOnFail.SuspendTestOnFail = false;
                SWAT.UserConfigHandler.Save();

                Assert.IsFalse(SWAT.WantSuspendOnFail.SuspendTestOnFail);
            }
            finally
            {
                SWAT.WantSuspendOnFail.SuspendTestOnFail = tmp1;
                SWAT.UserConfigHandler.Save();
            }
        }

        [Test]
        public void TestMissingUserConfig()
        {
            string filePath = getUserConfigFilePath();        

            File.Move(filePath, filePath + "1");

            SWAT.ScreenShotSettings.ScreenShotAllScreens = true;

            try
            {
                SWAT.UserConfigHandler.Save();
            }
            catch (UserConfigFileDoesNotExistException exc)
            { Assert.AreEqual(exc.Message, "Unable to save settings SWAT.user.config file is missing"); }

            File.Move(filePath + "1", filePath);
            
        }

        [Test]
        public void TestInsertionOfMissingSettingsOnUserConfig()
        {
            XmlDocument config = new XmlDocument();
            string configFilePath = getUserConfigFilePath();
            string backupFilePath = configFilePath + "backup";
            string key = "SafariPort";
            int value = 9997;

            bool isSettingPresent;

            File.Copy(configFilePath, backupFilePath, true);

            removeOldSetting( ref config, configFilePath, key, value );

            isSettingPresent = checkIsSettingPresent(ref config, configFilePath, key, value);

            if (!isSettingPresent)
            {

                //Modify Swat Setting
                SWAT.SafariSettings.SafariPort = value;
                SWAT.UserConfigHandler.Save();

                isSettingPresent = checkIsSettingPresent(ref config, configFilePath, key, value);

                File.Copy(backupFilePath, configFilePath, true);
                File.Delete(backupFilePath);

                Assert.IsTrue(isSettingPresent, "New setting was not correctly inserted on config file.");
            }
            else
            {
                Assert.Fail("Test implementation is defective. Old setting is still present on config file.");
            }

        }

        private void removeOldSetting(ref XmlDocument config, string configFilePath, string key, int value )
        {

            loadXmlDocFromConfig(ref config, configFilePath);

            XmlNodeList nodeList = config.SelectNodes("//add");

            XmlNode node = getNodeFromConfig(ref config, key);

            if (node != null)
            {
                //remove the settings node
                XmlNode appSettings = config.SelectSingleNode("appSettings");
                appSettings.RemoveChild(node);
            }

            config.PreserveWhitespace = true;
            XmlTextWriter wrtr = new XmlTextWriter(configFilePath, Encoding.UTF8);
            config.Save(wrtr);
            wrtr.Close();
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        private bool checkIsSettingPresent(ref XmlDocument config, string configFilePath, string key, int value)
        {
            loadXmlDocFromConfig( ref config, configFilePath );

            XmlNodeList nodeList = config.SelectNodes("//add");

            XmlNode node = getNodeFromConfig(ref config, key);

            if (node != null)
            {
                if( node.Attributes[1].Value.Equals( value.ToString() ) )
                    return true;

            }
            
            return false;
        }

        private void loadXmlDocFromConfig(ref XmlDocument config, string configFilePath )
        {
            if (System.IO.File.Exists(configFilePath))
            {
                config.Load(configFilePath);
            }
            else
            {
                throw new UserConfigFileDoesNotExistException("Unable to save settings SWAT.user.config file is missing");
            }
        }

        private XmlNode getNodeFromConfig(ref XmlDocument config, string key)
        {
            XmlNodeList nodeList = config.SelectNodes("//add");

            foreach (XmlNode node in nodeList)
            {
                if (node.Attributes[0].Value.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return node;
                }
            }

            return null;
        }

        private string getUserConfigFilePath()
        {
            string filePath = Path.Combine(Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().GetName().CodeBase), "SWAT.user.config");

            return filePath.Replace("file:\\", "").Replace('\\', '/');
        }

        [Test]
        public void TestChangingSettingValueBeforeSaving()
        {
            int origValue = SWAT.WantDelayBetweenCommands.DelayBetweenCommands;

            SWAT.WantDelayBetweenCommands.DelayBetweenCommands = 99;
            SWAT.WantDelayBetweenCommands.DelayBetweenCommands = 88;
            SWAT.UserConfigHandler.Save();

            Assert.AreEqual(SWAT.WantDelayBetweenCommands.DelayBetweenCommands, 88);

            SWAT.WantDelayBetweenCommands.DelayBetweenCommands = origValue;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestLasSettingSuccesfulProperty()
        {
            int origValue = SWAT.DefaultTimeouts.WaitForBrowserTimeout;

            SWAT.DefaultTimeouts.WaitForBrowserTimeout = 1;
            Assert.IsTrue(UserConfigHandler.LastSettingSuccessful);

            SWAT.DefaultTimeouts.WaitForBrowserTimeout = 0;
            Assert.IsFalse(UserConfigHandler.LastSettingSuccessful);

            SWAT.DefaultTimeouts.WaitForBrowserTimeout = origValue;
            SWAT.UserConfigHandler.Save();
        }
    }
}
