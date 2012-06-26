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
    public class ConfigurationItemTestsTestFixture
    {
        WebBrowser _browser;
        
        public ConfigurationItemTestsTestFixture()
        {
            _browser = new WebBrowser(BrowserType.InternetExplorer);
        }
        
        #region GetConfigurationItem Tests

        [Test]
        public void GetSafariAddress()
        {
            string settingValue;

            settingValue = _browser.GetConfigurationItem("SafariAddress");
            Assert.AreEqual(settingValue, SWAT.SafariSettings.SafariAddress);
        }

        [Test]
        public void GetFindElementTimeout()
        {
            string settingValue;

            settingValue = _browser.GetConfigurationItem("FindElementTimeout");
            Assert.AreEqual(settingValue, SWAT.DefaultTimeouts.FindElementTimeout.ToString());
        }

        [Test]
        public void GetGetInformativeExceptions()
        {
            string settingValue;

            settingValue = _browser.GetConfigurationItem("GetInformativeExceptions");
            Assert.AreEqual(settingValue, SWAT.WantInformativeExceptions.GetInformativeExceptions.ToString());
        }

        [Test]
        public void GetDelayBetweenCommands()
        {
            string settingValue;

            settingValue = _browser.GetConfigurationItem("DelayBetweenCommands");
            Assert.AreEqual(settingValue, SWAT.WantDelayBetweenCommands.DelayBetweenCommands.ToString());
        }

        [Test]
        public void GetCloseBrowsersBeforeTestStart()
        {
            string settingValue;

            settingValue = _browser.GetConfigurationItem("CloseBrowsersBeforeTestStart");
            Assert.AreEqual(settingValue, SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart.ToString());
        }

        [Test]
        public void GetSuspendTestOnFail()
        {
            string settingValue;

            settingValue = _browser.GetConfigurationItem("SuspendTestOnFail");
            Assert.AreEqual(settingValue, SWAT.WantSuspendOnFail.SuspendTestOnFail.ToString());
        }

        [Test]
        public void GetSnapShotFolder()
        {
            string settingValue;

            settingValue = _browser.GetConfigurationItem("SnapShotFolder");
            Assert.AreEqual(settingValue, SWAT.ScreenShotSettings.SnapShotFolder);
        }

        [Test]
        public void GetHighlightElementsAsTestsRun()
        {
            string settingValue;

            settingValue = _browser.GetConfigurationItem("HighlightElementsAsTestsRun");
            Assert.AreEqual(settingValue, SWAT.IESettings.HighlightElementsAsTestsRun.ToString());
        }

        [Test]
        public void GetConfigurationItemWithInvalidCommandName()
        {
            string exceptionMessage = "";

            try
            {
                _browser.GetConfigurationItem("NonExistantCommand");
            }
            catch(Exception e)
            {
                exceptionMessage = e.ToString();
            }
            Assert.That(exceptionMessage.Contains("Invalid setting name: NonExistantCommand"));
        }

        [Test]
        public void CheckForBackSlashInDirectoryPath()
        {
            string originalValue = SWAT.ScreenShotSettings.SnapShotFolder;
            string newValue;

            _browser.SetConfigurationItem("SnapShotFolder", @"C:\SnapShotFolderTest");
            newValue = SWAT.ScreenShotSettings.SnapShotFolder;
            SWAT.ScreenShotSettings.SnapShotFolder = originalValue;

            Assert.AreEqual(String.Format("\\\\{0}\\C$\\SnapShotFolderTest", Environment.MachineName), newValue);

            //get the SnapShotFolder
            string settingValue; 
            settingValue = _browser.GetConfigurationItem("SnapShotFolder");
            Assert.AreEqual(settingValue, SWAT.ScreenShotSettings.SnapShotFolder);
        }

        #endregion 

        #region SetConfigurationItem Tests

        [Test]
        public void SetSafariAddress()
        {
            string originalValue = SWAT.SafariSettings.SafariAddress;
            string newValue;

            _browser.SetConfigurationItem("SafariAddress", "111.222.3.4");
            newValue = SWAT.SafariSettings.SafariAddress;
            SWAT.SafariSettings.SafariAddress = originalValue;
            
            Assert.AreEqual("111.222.3.4" , newValue);
        }

        [Test]
        public void SetFindElementTimeout()
        {
            int originalValue = SWAT.DefaultTimeouts.FindElementTimeout;
            int newValue;

            _browser.SetConfigurationItem("FindElementTimeout", "23");
            newValue = SWAT.DefaultTimeouts.FindElementTimeout;
            SWAT.DefaultTimeouts.FindElementTimeout = originalValue;

            Assert.AreEqual("23", newValue.ToString());
        }

        [Test]
        public void SetGetInformativeExceptions()
        {
            bool originalValue = SWAT.WantInformativeExceptions.GetInformativeExceptions;
            bool newValue;

            _browser.SetConfigurationItem("GetInformativeExceptions", "True");
            newValue = SWAT.WantInformativeExceptions.GetInformativeExceptions;
            SWAT.WantInformativeExceptions.GetInformativeExceptions = originalValue;

            Assert.AreEqual("True", newValue.ToString());
        }

        [Test]
        public void SetDelayBetweenCommands()
        {
            int originalValue = SWAT.WantDelayBetweenCommands.DelayBetweenCommands;
            int newValue;

            _browser.SetConfigurationItem("DelayBetweenCommands", "1");
            newValue = SWAT.WantDelayBetweenCommands.DelayBetweenCommands;
            SWAT.WantDelayBetweenCommands.DelayBetweenCommands = originalValue;

            Assert.AreEqual("1", newValue.ToString());
        }

        [Test]
        public void SetCloseBrowsersBeforeTestStart()
        {
            bool originalValue = SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart;
            bool newValue;

            _browser.SetConfigurationItem("CloseBrowsersBeforeTestStart", "True");
            newValue = SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart;
            SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = originalValue;

            Assert.AreEqual("True", newValue.ToString());
        }

        [Test]
        public void SetSuspendTestOnFail()
        {
            bool originalValue = SWAT.WantSuspendOnFail.SuspendTestOnFail;
            bool newValue;

            _browser.SetConfigurationItem("SuspendTestOnFail", "True");
            newValue = SWAT.WantSuspendOnFail.SuspendTestOnFail;
            SWAT.WantSuspendOnFail.SuspendTestOnFail = originalValue;

            Assert.AreEqual("True", newValue.ToString());
        }   

        [Test]
        public void SetSnapShotFolder()
        {
            string originalValue = SWAT.ScreenShotSettings.SnapShotFolder;
            string newValue;

            _browser.SetConfigurationItem("SnapShotFolder", @"C:\SnapShotFolderTest");
            newValue = SWAT.ScreenShotSettings.SnapShotFolder;
            SWAT.ScreenShotSettings.SnapShotFolder = originalValue;

            Assert.AreEqual(String.Format("\\\\{0}\\C$\\SnapShotFolderTest", Environment.MachineName), newValue);
        }
        
        [Test]
        public void SetHighlightElementsAsTestsRun()
        {
            bool originalValue = SWAT.IESettings.HighlightElementsAsTestsRun;
            bool newValue;

            _browser.SetConfigurationItem("HighlightElementsAsTestsRun", "True");
            newValue = SWAT.IESettings.HighlightElementsAsTestsRun;
            SWAT.IESettings.HighlightElementsAsTestsRun = originalValue;

            Assert.AreEqual("True", newValue.ToString());
        }

        [Test]
        public void SetConfigurationItemWithInvalidCommandName()
        {
            string exceptionMessage = "";

            //Test when invalid config item is used exception is thrown
            try
            {
                _browser.SetConfigurationItem("NonExistantCommand", "1");
            }
            catch (Exception e)
            {
                exceptionMessage = e.ToString();
            }
            Assert.That(exceptionMessage.Contains("Invalid setting name: NonExistantCommand"));
        }

        #endregion

        #region SetConfigurationItem With Invalid Setting Value Tests

        [Test]
        public void SetSafariPortWithInvalidValue()
        {
            string exceptionMessage = "";

            try
            {
                _browser.SetConfigurationItem("SafariPort", "0");
            }
            catch (ConfigurationItemException e)
            {
                exceptionMessage = e.Message;
            }
            Assert.AreEqual(exceptionMessage, "Invalid value for this setting: 0");
        }

        [Test]
        public void SetFirefoxRootDirectoryWithInvalidValue()
        {
            string exceptionMessage = "";
            try
            {
                _browser.SetConfigurationItem("FirefoxRootDirectory", "");
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }
            Assert.AreEqual(exceptionMessage, "Invalid value for this setting: ");
        }
        
        [Test]
        public void SetFindElementTimeoutWithInvalidValue()
        {
            string exceptionMessage = "";

            try
            {
                _browser.SetConfigurationItem("FindElementTimeout", "invalidValue");
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }
            Assert.AreEqual(exceptionMessage, "Invalid value for this setting: invalidValue");
        }

        [Test]
        public void SetGetInformativeExceptionsWithInvalidValue()
        {
           string exceptionMessage = "";

            try
            {
                _browser.SetConfigurationItem("GetInformativeExceptions", "invalidValue");
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }
            Assert.AreEqual(exceptionMessage, "Invalid value for this setting: invalidValue");
        }

        [Test]
        public void SetDelayBetweenCommandsWithInvalidValue()
        {
            string exceptionMessage = "";

            try
            {
                _browser.SetConfigurationItem("DelayBetweenCommands", "-1");
            }
            catch (ConfigurationItemException e)
            {
                exceptionMessage = e.Message;
            }
            Assert.AreEqual(exceptionMessage, "Invalid value for this setting: -1");
        }

        [Test]
        public void SetCloseBrowsersBeforeTestStartWithInvalidValue()
        {
            string exceptionMessage = "";

            try
            {
                _browser.SetConfigurationItem("CloseBrowsersBeforeTestStart", "invalidValue");
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }
            Assert.AreEqual(exceptionMessage, "Invalid value for this setting: invalidValue");
        }    

        [Test]
        public void SetHighlightElementsAsTestsRunWithInvalidValue()
        {
            string exceptionMessage = "";

            try
            {
                _browser.SetConfigurationItem("HighlightElementsAsTestsRun", "invalidValue");
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }
            Assert.AreEqual(exceptionMessage, "Invalid value for this setting: invalidValue");
        }

        #endregion
    }
}