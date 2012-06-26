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
using System.Diagnostics;

namespace SWAT.Tests.OpenNavigateCloseBrowser
{
    [TestFixture]
    [Category("Chrome")]
    public class ChromeTests : OpenNavigateCloseBrowserTestFixture
    {
        public ChromeTests()
            : base(BrowserType.Chrome)
        {

        }

        [Test]
        public void SimpleChromeNavigationTest()
        {
            _browser.OpenBrowser();
            _browser.NavigateBrowser("www.yahoo.com");
            _browser.Sleep(60000);
        }

        [Test]
        public void ChromeClosesBrowserAfterJSDialogClosesLastTabTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Id, "btnDialogCloseWindow", "onclick", "input");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
                Assert.IsTrue(Process.GetProcessesByName("chrome").Length == 0, "Chrome did not close when a JSDialog caused the last tab to close.");
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                _browser.OpenBrowser();
                NavigateToSwatTestPage();
            }
        }

        [Test]
        public void ChromeClosesBrowserAfterStimulateElementClosesLastTabTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Id, "btnCloseWindow", "onclick", "input");
                waitForBrowserClose();
                Assert.IsTrue(Process.GetProcessesByName("chrome").Length == 0, "Chrome did not close after a stimulate element caused the window to close.");
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                _browser.OpenBrowser();
                NavigateToSwatTestPage();
            }
        }

        [Test]
        public void ChromeClosesBrowserWhenAllTabsAreClosedTest()
        {
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    _browser.OpenBrowser();
                    _browser.NavigateBrowser("www.google.com");
                }
                for (int i = 0; i < 6; i++) //extra SWAT test page tab
                {
                    _browser.CloseBrowser();
                    if (i < 5)
                        _browser.AttachToWindow("");
                }
                waitForBrowserClose();
                Assert.IsTrue(Process.GetProcessesByName("chrome").Length == 0, "Chrome did not close completely when no tabs are open");
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                _browser.OpenBrowser();
                NavigateToSwatTestPage();
            }
        }

        [Test]
        public void NavigateBrowserWithNetworkFileTest()
        {
            string url =
                string.Format(@"\\{0}\c$\swat\trunk\swat.tests\testpages\TestPage.htm",
                    Environment.MachineName.ToLower());
            try
            {
                _browser.NavigateBrowser(url);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void NavigateBrowserWithLocalFileTest()
        {
            if (_browserType == BrowserType.Safari)
                Assert.Ignore("Test is not relevant to Safari.");
            try
            {
                _browser.NavigateBrowser(@"C:\swat\trunk\swat.tests\testpages\TestPage.htm");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        #region Exception Tests

        [Test]
        [ExpectedException(typeof(ChromeExtensionNotConnectedException), UserMessage = "Failed to connect to the Google Chrome SWAT extension. Please make sure it is installed and enabled.")]
        public void ChromeExtensionNotConnectedExceptionTest()
        {
            throw new ChromeExtensionNotConnectedException();
        }

        [Test]
        [ExpectedException(typeof(ChromeCommandTimedOutException), UserMessage = "The command TestCommandName timed out after 10 seconds.")]
        public void ChromeCommandTimedOutExceptionTest()
        {
            throw new ChromeCommandTimedOutException("TestCommandName", 10);
        }

        [Test]
        [ExpectedException(typeof(NoAvailablePortException), UserMessage = "Failed to find an available port. All ports are either blocked or currently in use.")]
        public void NoAvailablePortExceptionTest()
        {
            throw new NoAvailablePortException();
        }

        #endregion

        #region Private Methods

        private void waitForBrowserClose()
        {
            DateTime timeOut = DateTime.Now.AddSeconds(5);
            while (DateTime.Now < timeOut && Process.GetProcessesByName("chrome").Length > 0) { /*wait for OS to register the process as closed*/ }
        }

        #endregion
    }
}
