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
using System.Linq;
using System.Text;
using NUnit.Framework;
 

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("Misc")]
    public class ScreenShotTestFixture
    {
        WebBrowser _browser;

        public ScreenShotTestFixture()
        {
            _browser = new WebBrowser(BrowserType.FireFox);
        }

        [Test]
        public void TestScreenShotSettingsOnOff()
        {
            //Save the value to reset later
            bool reset = SWAT.ScreenShotSettings.SnapShotOption;

            //Turn off screen shot setting
            SWAT.ScreenShotSettings.SnapShotOption = false;
            SWAT.UserConfigHandler.Save();
            Assert.AreEqual(false, SWAT.ScreenShotSettings.SnapShotOption);

            //Turn on screen shot setting
            SWAT.ScreenShotSettings.SnapShotOption = true;
            SWAT.UserConfigHandler.Save();
            Assert.AreEqual(true, SWAT.ScreenShotSettings.SnapShotOption);

            //Reset the value
            SWAT.ScreenShotSettings.SnapShotOption = reset;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestScreenShotSaveFilePath()
        {
            //Save current value and then reset.
            string reset = SWAT.ScreenShotSettings.SnapShotFolder;

            SWAT.ScreenShotSettings.SnapShotFolder = "";
            SWAT.UserConfigHandler.Save();
            Assert.AreEqual(@"\\" + Environment.MachineName + @"\C$\", SWAT.ScreenShotSettings.SnapShotFolder);

            SWAT.ScreenShotSettings.SnapShotFolder = @"C:\";
            SWAT.UserConfigHandler.Save();
            Assert.AreEqual(@"\\" + Environment.MachineName + @"\C$\", SWAT.ScreenShotSettings.SnapShotFolder);

            SWAT.ScreenShotSettings.SnapShotFolder = "denver2";
            SWAT.UserConfigHandler.Save();
            Assert.AreEqual(@"denver2", SWAT.ScreenShotSettings.SnapShotFolder);

            SWAT.ScreenShotSettings.SnapShotFolder = @"C:";
            SWAT.UserConfigHandler.Save();
            Assert.AreEqual(@"\\" + Environment.MachineName + @"\C$", SWAT.ScreenShotSettings.SnapShotFolder);

            SWAT.ScreenShotSettings.SnapShotFolder = @"C:\test\test";
            SWAT.UserConfigHandler.Save();
            Assert.AreEqual(@"\\" + Environment.MachineName + @"\C$\test\test", SWAT.ScreenShotSettings.SnapShotFolder);

            //reset the value
            SWAT.ScreenShotSettings.SnapShotFolder = reset;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestScreenShotAllScreensOption()
        {
            //Save current value and then reset.
            bool reset = SWAT.ScreenShotSettings.ScreenShotAllScreens;

            //Change the file path where images are saved
            SWAT.ScreenShotSettings.ScreenShotAllScreens = true;
            SWAT.UserConfigHandler.Save();
            Assert.IsTrue(SWAT.ScreenShotSettings.ScreenShotAllScreens, "The AllScreen value is not being set properly");

            SWAT.ScreenShotSettings.ScreenShotAllScreens = false;
            SWAT.UserConfigHandler.Save();
            Assert.IsFalse(SWAT.ScreenShotSettings.ScreenShotAllScreens, "The AllScreen value is not being set properly");

            //reset the value
            SWAT.ScreenShotSettings.ScreenShotAllScreens = reset;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestScreenShotBrowserOption()
        {
            //Save current value and then reset.
            bool reset = SWAT.ScreenShotSettings.ScreenShotBrowser;

            //Change the file path where images are saved
            SWAT.ScreenShotSettings.ScreenShotBrowser = true;
            SWAT.UserConfigHandler.Save();
            Assert.IsTrue(SWAT.ScreenShotSettings.ScreenShotBrowser, "The AllScreen value is not being set properly");

            SWAT.ScreenShotSettings.ScreenShotBrowser = false;
            SWAT.UserConfigHandler.Save();
            Assert.IsFalse(SWAT.ScreenShotSettings.ScreenShotBrowser, "The AllScreen value is not being set properly");

            //reset the value
            SWAT.ScreenShotSettings.ScreenShotAllScreens = reset;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestScreenShotCaptureAllScreensFailed()
        {
            SWAT.ErrorSnapShot capture = new SWAT.ErrorSnapShot();
            string returnValue = capture.CaptureAllScreens("C:\test", "Any Command"); // this will fail because is not a folder and return an error message.
            Assert.AreEqual(returnValue, "Illegal characters in path.");
        }

        [Test]
        public void TestScreenShotWorksForAllScreens()
        {
            SWAT.ErrorSnapShot test = new SWAT.ErrorSnapShot();
            string imageSave = test.CaptureAllScreens(@"C:\SWAT\trunk\SWAT.Tests\TestPages\", "Test");
            //Want to make sure that the file was saved.
            Assert.IsTrue(File.Exists(imageSave.Substring(22)), imageSave.Substring(18) + "      No file was created and screen shots is not working for all screens.");
            File.Delete(imageSave.Substring(22));
        }

        [Test]
        public void TestScreenShotWorksForBrowserScreen()
        {
            bool resetScreen = SWAT.ScreenShotSettings.ScreenShotAllScreens;
            bool resetBrowser = SWAT.ScreenShotSettings.ScreenShotBrowser;
            bool resetImageSettings = SWAT.ScreenShotSettings.SnapShotOption;
            SWAT.ScreenShotSettings.SnapShotOption = true;
            SWAT.ScreenShotSettings.ScreenShotAllScreens = false;
            SWAT.ScreenShotSettings.ScreenShotBrowser = true;
            SWAT.UserConfigHandler.Save();
            _browser.OpenBrowser();
            _browser.NavigateBrowser("www.google.com");
            string savePath = _browser.TakeScreenshot("test");
            _browser.CloseBrowser();
            SWAT.ScreenShotSettings.SnapShotOption = resetImageSettings;
            SWAT.ScreenShotSettings.ScreenShotBrowser = resetBrowser;
            SWAT.ScreenShotSettings.ScreenShotAllScreens = resetScreen;
            SWAT.UserConfigHandler.Save();
            Assert.IsTrue(File.Exists(savePath.Substring(22)), savePath.Substring(18) + "      No file was created and screen shots is not working for browser screens.");
            File.Delete(savePath.Substring(22));
        }

         
        [TestCase("ConnectToMssql")]
        [TestCase("ConnectToOracle")]
        [TestCase("SetDatabase")]
        [TestCase("SetQuery")]
        [TestCase("Disconnect")]
        [TestCase("AssertRecordValues")]
        [TestCase("AssertRecordValuesByColumnNameGetDbRecord")]
        [TestCase("GetDbRecordByColumnName")]
        [TestCase("GetDbDate")]
        [TestCase("AssertRecordCount")]
        [TestCase("UpdateTable")]
        [TestCase("InsertIntoTable")]
        [TestCase("DeleteFromTable")]
        [TestCase("Disconnect")]
        [TestCase("AssertLessThan")]
        [TestCase("AssertGreaterThan")]
        [TestCase("AssertLessThanOrEqual")]
        [TestCase("AssertGreaterThanOrEqual")]
        [TestCase("AssertEqualTo")]
        public void TestScreenShotAllScreensIsNotTakenForCommand(String command)
        {
            SWAT.ErrorSnapShot test = new SWAT.ErrorSnapShot();
            string imageSave = test.CaptureAllScreens(@"C:\SWAT\trunk\SWAT.Tests\TestPages\", command);
            //We want to make sure that no file was save.
            Assert.IsTrue(!File.Exists(imageSave.Substring(18)), "File not was created and screen shots is working for non-GUI commands.");
        }

        [Test]
        [ExpectedException(typeof(NoAttachedWindowException))]
        public void TestScreenShotForDocumentFailsWhenNoHandleExists()
        {
            SWAT.ErrorSnapShot test = new SWAT.ErrorSnapShot(_browser as IDocumentInfo, BrowserType.FireFox);

            string imageSave = string.Empty;
            try
            {
                imageSave = test.CaptureBrowser(@"C:\SWAT\trunk\SWAT.Tests\TestPages\", "Test", IntPtr.Zero);
            }
            finally
            {
                if (File.Exists(imageSave))
                    File.Delete(imageSave.Substring(22));
            }
        }
    }
}
