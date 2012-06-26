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
using System.Diagnostics;

namespace SWAT.Tests.KillAllOpenBrowsers
{
    public abstract class KillAllOpenBrowsersTestFixture : BrowserTestFixture
    {
        public KillAllOpenBrowsersTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }
        
        #region KillAllOpenBrowsers
        
        [Test]
        public void KillAllOpenBrowsersTest()
        {
            _browser.OpenBrowser();
            _browser.NavigateBrowser("www.google.com");
            _browser.OpenBrowser();
            _browser.NavigateBrowser("www.w3schools.com");

            _browser.KillAllOpenBrowsers();

            try
            {
                _browser.AssertBrowserDoesNotExist("google");
                _browser.AssertBrowserDoesNotExist("w3");
                _browser.AssertBrowserDoesNotExist("SWAT Test Page");
            }
            finally
            {   // Clean up
                this.OpenSwatTestPage();
            }              
        }
                                         
         
        [TestCase(false)]
        [TestCase(true)]
        public void CloseBrowsersBeforeTestStartTest(bool closeAll)
        {
            if (!(_browserType == BrowserType.Safari))
            {
                // Save the value to reset it later
                bool reset = SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart;

                // Get name of the process of the browser
                string browserName = GetBrowserName();

                // Boolean used for the assertions
                bool areAllClosed;

                // Open multiple windows to close
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.google.com");
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.w3schools.com");

                // Set the option to close or not when the test starts
                SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = closeAll;
                SWAT.UserConfigHandler.Save();

                // Command extractor's constructor should not close any open browsers
                SWAT_Editor.CommandExtractor extractor = new SWAT_Editor.CommandExtractor(_browserType);
                
                // Check if there are any browsers open
                areAllClosed = (Process.GetProcessesByName(browserName).Length == 0);

                try
                {
                    if (closeAll)
                    {
                        // Assert that all the web browsers have been closed
                        Assert.IsTrue(areAllClosed, "CloseBrowsersBeforeTestStart failed: " +
                                        "closed all windows when the user setting was turned off.");
                    }
                    else
                    {
                        // Assert that all the web browsers have not been closed
                        Assert.IsFalse(areAllClosed, "CloseBrowsersBeforeTestStart failed: " +
                            "did not close all windows.");
                    }
                }
                finally
                {
                    // Clean up
                    _browser = new WebBrowser(_browserType);
                    _browser.KillAllOpenBrowsers();
                    this.OpenSwatTestPage();
                }
                // Reset the value
                SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = reset;
                SWAT.UserConfigHandler.Save();
            }
            else if (_browserType == BrowserType.Safari)
            {
                // Save the value to reset it later
                bool reset = SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart;

                // Boolean used for the assertions
                bool isGoogleClosed = false;
                bool isW3Closed = false;

                // Open multiple windows to close
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.google.com");
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.w3schools.com");

                // Set the option not to close when the test starts
                SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = closeAll;
                SWAT.UserConfigHandler.Save();

                // Command extractor's constructor should not close any open browsers
                SWAT_Editor.CommandExtractor extractor = new SWAT_Editor.CommandExtractor(_browserType);
                
                // Check if there are any browsers open
                try
                {
                    _browser.AssertBrowserExists("Google");
                }
                catch (BrowserExistException)
                {
                    isGoogleClosed = true;
                }
                try
                {
                    _browser.AssertBrowserExists("W3");
                }
                catch (BrowserExistException)
                {
                    isW3Closed = true;
                }

                if (closeAll)
                {
                    // Assert that all the web browsers have been closed
                    Assert.IsTrue(isGoogleClosed && isW3Closed, "CloseBrowsersBeforeTestStart failed: " +
                                "closed all windows when the user setting was turned off.");
                }
                else
                {
                    // Assert that all the web browsers have not been closed
                    Assert.IsFalse(isGoogleClosed || isW3Closed, "CloseBrowsersBeforeTestStart failed: " +
                        "did not close all windows.");
                }
                
                //_browser = new WebBrowser(_browserType);

                // Needed for other tests since we closed open web browsers
                this.OpenSwatTestPage();

                // Reset the value
                SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = reset;
                SWAT.UserConfigHandler.Save();
            }

        }
                       
        [Test]
        public void KillAllOpenBrowsersWithJSDialogsOpenTest()
        {   
            try
            {
            _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
            
            _browser.OpenBrowser();
            _browser.NavigateBrowser(getTestPage("TestPage.htm"));
            _browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick");

            _browser.KillAllOpenBrowsers();

            
                _browser.AssertBrowserDoesNotExist("TestPage.htm");
                _browser.AssertBrowserDoesNotExist("OnBeforeUnload.htm");
            }
            finally
            {
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void KillAllOpenBrowsersPassesWithNoWindowOpenTest()
        {
            try
            {
                _browser.KillAllOpenBrowsers(); // SWAT Test Page should already be open
                _browser.KillAllOpenBrowsers();
            }
            finally
            {
                this.OpenSwatTestPage();
            }
        }

        #endregion
        
        #region KillAllOpenBrowsers except specified window


        [TestCase("Google", new string[] { "about:config", "SWAT Test Page", "fw.pdf" })]
        [TestCase("about:config", new string[] { "Google", "SWAT Test Page", "fw.pdf" })]
        [TestCase("fw4.pdf", new string[] { "about:config", "SWAT Test Page", "Google" })]
        [TestCase("SWAT Test Page", new string[] { "about:config", "Google", "fw.pdf" })]
        public void KillAllOpenBrowsersExceptWindowTitleTest(string sparedBrowser, string[] killedBrowsers)
        {
            sparedBrowser = GeneralizeName(sparedBrowser);

            try
            {
                int numBrowsers = 2;
                for (int i = 0; i < numBrowsers; i++)
                {
                    _browser.OpenBrowser();
                    _browser.NavigateBrowser("www.google.com");
                    _browser.OpenBrowser();
                    _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                    _browser.StimulateElement(IdentifierType.Expression, "innerHtml:Form W-4", "onclick", "a");
                    _browser.OpenBrowser();
                    _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                    _browser.OpenBrowser();
                }

                _browser.KillAllOpenBrowsers(sparedBrowser);

                _browser.AssertBrowserExists(sparedBrowser);
                // Make sure we haven't closed ANY sparedBrowser windows by attaching to all of them
                for (int i = 0; i < numBrowsers; i++)
                {
                    _browser.AttachToWindow(sparedBrowser, i);
                }
                foreach (string killedBrowser in killedBrowsers)
                {
                    _browser.AssertBrowserDoesNotExist(GeneralizeName(killedBrowser));
                }
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        private string GeneralizeName(string blankPage)
        {
            if (blankPage.Equals("about:config"))
            {
                if (_browserType == BrowserType.InternetExplorer)
                    return "about:Blank";
                if (_browserType == BrowserType.Chrome || _browserType == BrowserType.Safari)
                    return "about:swat";
            }

            return blankPage;
        }

        [Test]
        public void KillAllOpenBrowsersExceptWindowTitleThatDoesNotExistTest()
        {
            int numBrowsers = 3;
            string[] windowTitles = { "Google", "W3Schools", "fw4" };
            string[] urls = { "www.google.com", getTestPage("files/fw4.pdf"), getTestPage("TestPage.htm") };

            // Set up
            for (int i = 0; i < numBrowsers; i++)
            {
                foreach (string url in urls)
	            {
		            _browser.OpenBrowser();
                    _browser.NavigateBrowser(url);
	            }
            }

            // Kill all of the browsers
            _browser.KillAllOpenBrowsers("Non Existent Window Title");

            // Assert that all the browsers have been closed
            try
            {
                foreach(string windowTitle in windowTitles)
                {
                    _browser.AssertBrowserDoesNotExist(windowTitle);
                }
            }
            finally // Clean up
            {
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void KillAllOpenBrowsersDoesNotClearWindowHandleWhenAttachedWindowRemainsOpenTest()
        {
            if (_browserType == BrowserType.Safari) // this should remain since Mac's don't have IntPtr's
            {
                Assert.Ignore("This test is irrelevent for Safari.");
            }

            try
            {
                _browser.OpenBrowser();
                _browser.NavigateBrowser("http://www.google.com");
                _browser.KillAllOpenBrowsers("Google");

                Assert.AreNotEqual(IntPtr.Zero, GetCurrentBrowserWindowHandle());
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void KillAllOpenBrowersClearsWindowHandleWhenAttachedWindowClosesTest()
        {
            if (_browserType == BrowserType.Safari) // this should remain since Mac's don't have IntPtr's
            {
                Assert.Ignore("This test is irrelevent for Safari.");
            }

            try
            {
                _browser.OpenBrowser();
                _browser.NavigateBrowser("http://www.google.com");
                _browser.KillAllOpenBrowsers("SWAT Test Page");

                Assert.AreEqual(IntPtr.Zero, GetCurrentBrowserWindowHandle());
            }
            finally
            {
				AttachToSwatTestPage();
            }
        }

        [Test]
        public void KillAllOpenBrowsersExceptWindowTitleOnBeforeUnloadWillOnlyClearWindowHandleAfterClickingOkWhenItIsTheWindowToCloseTest()
        {
            if (_browserType == BrowserType.Safari) // this should remain since Mac's don't have IntPtr's
            {
                Assert.Ignore("This test is irrelevent for Safari.");
            }

            _browser.OpenBrowser();
            _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));

            try
            {
                _browser.KillAllOpenBrowsers("SWAT Test Page");
                try
                {
                    Assert.AreNotEqual(IntPtr.Zero, GetCurrentBrowserWindowHandle());
                }
                finally
                {
                    _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
                }

                Assert.AreEqual(IntPtr.Zero, GetCurrentBrowserWindowHandle());
            }
            finally
            {
				AttachToSwatTestPage();
            }
        }
        
        #endregion 
    }
}
