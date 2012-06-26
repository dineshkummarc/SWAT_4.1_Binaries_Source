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
using NUnit.Framework;
 

namespace SWAT.Tests.ClickJSDialog
{
    public abstract class ClickJSDialogTestFixture : BrowserTestFixture
    {
        public ClickJSDialogTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region ClickJSDialog

        [Test]
        public void ClickJSDialogTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
                _browser.AssertElementExists(IdentifierType.Expression, "id:txtOne;value:Ok", "input");

                _browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick");
                _browser.ClickJSDialog(JScriptDialogButtonType.Cancel);
                _browser.AssertElementExists(IdentifierType.Expression, "id:txtOne;value:Cancel", "input");

                _browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
                _browser.AssertElementExists(IdentifierType.Expression, "id:txtOne;value:Ok", "input");

                _browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick");
                _browser.ClickJSDialog(JScriptDialogButtonType.Cancel);
                _browser.AssertElementExists(IdentifierType.Expression, "id:txtOne;value:Cancel", "input");

                _browser.StimulateElement(IdentifierType.Id, "btnNewWindowClose", "onclick");
                _browser.AttachToWindow("Close Window Page");
                _browser.StimulateElement(IdentifierType.Id, "btnCloseWindow", "onclick");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
            }
            finally
            {
                // Clean up
                this.AttachToSwatTestPage();
            }
        }                                                                             

        [Test]
        [ExpectedException(typeof(ClickJSDialogException), UserMessage = "Failed to click JSDialog")]
        public void ClickJSDialogFailedTest()
        {
            // Makes sure ClickJSDialog fails when no dialog is present
            _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
        }

        [Test]
        public void ClickJSDialogThatLaunchesAnotherJSDialogTest()
        {
			_browser.StimulateElement(IdentifierType.Id, "btnTwoAlerts", "onclick", "input");
            _browser.AssertJSDialogContent("Press Ok to launch another alert");
            _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
            _browser.AssertJSDialogContent("Press Ok to close this alert");
            _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
            _browser.AssertElementExists(IdentifierType.Expression, "id=txtOne;value=Two", "input");
        }

        #endregion 

        #region ClickJSDialog and Navigation
 
        [Test]        
        public void JSDialogReAttachWindowTest()
        {                              
            _browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick");
            _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
        }

        [Test]
        public void JSDialogNavigateToNewWindowTest()
        {
            string expected = "Second Window";
            try
            {
                _browser.StimulateElement(IdentifierType.Id, "btnDialogNewPage", "onclick");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);

                _browser.AssertBrowserExists(expected);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void ClickJSDialogWhenNavigateBrowserNewPageLaunchesDialogTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("DialogWhileLoading.htm"), 30);

                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void ClickJSDialogOnBeforeUnloadCloseBrowserOkTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.CloseBrowser();
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
                _browser.AssertBrowserDoesNotExist("Dialog Test");
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void ClickJSDialogOnBeforeUnloadCloseBrowserCancelTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.CloseBrowser();
                _browser.ClickJSDialog(JScriptDialogButtonType.Cancel);
                _browser.AssertBrowserExists("Dialog Test");
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

         
        [TestCase(JScriptDialogButtonType.Ok)]
        [TestCase(JScriptDialogButtonType.Cancel)]
        public void ClickJSDialogOnBeforeUnloadRefreshBrowserTest(JScriptDialogButtonType jsBtnType)
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.RefreshBrowser();
                _browser.ClickJSDialog(jsBtnType);
                _browser.AssertBrowserExists("Dialog Test");  // Should end up on this page regardless of OK or Cancel
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

         
        [TestCase(JScriptDialogButtonType.Ok, "SWAT Test Page")]
        [TestCase(JScriptDialogButtonType.Cancel, "Dialog Test")]
        public void ClickJSDialogOnBeforeUnloadNavigateBrowserTest(JScriptDialogButtonType jsBtnType, string expectedWebPage)
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.ClickJSDialog(jsBtnType);
                _browser.AssertBrowserExists(expectedWebPage);
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

         
        [TestCase(JScriptDialogButtonType.Ok, "Google")]
        [TestCase(JScriptDialogButtonType.Cancel, "Dialog Test")]
        public void ClickJSDialogOnBeforeUnloadStimulateElementTest(JScriptDialogButtonType jsBtnType, string expectedWebPage)
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.StimulateElement(IdentifierType.InnerHtml, "Google", "onclick", "a");
                _browser.ClickJSDialog(jsBtnType);
                _browser.AssertBrowserExists(expectedWebPage);
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void ClickJSDialogOnBeforeUnloadKillAllOpenBrowsersWithWindowTitleTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.KillAllOpenBrowsers("facebook");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
                _browser.AssertBrowserDoesNotExist("Dialog Test");
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        #endregion 
    }
}
