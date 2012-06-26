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
namespace SWAT.Tests.AssertJSDialogContent
{
    public abstract class AssertJSDialogContentTestFixture : BrowserTestFixture
    {
        public AssertJSDialogContentTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        [Test]
        public void AssertJSDialogContentTest()
        {
            //With Content
            _browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick", "input");
            _browser.AssertJSDialogContent("Please press Ok or Cancel");
            _browser.ClickJSDialog(JScriptDialogButtonType.Cancel);

            //Blank
            _browser.StimulateElement(IdentifierType.Id, "btnBlankAlert", "onclick", "input");
            _browser.AssertJSDialogContent("");
            _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
        }

        [Test]
        public void AssertJSDialogContentWithTimeoutTest()
        {
            //With Content
            _browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick", "input");
            _browser.AssertJSDialogContent("Please press Ok or Cancel", 15000);
            _browser.ClickJSDialog(JScriptDialogButtonType.Cancel);
        }

        [Test]
        public void AssertJSDialogContentOnDelayedDialogTest()
        {
            _browser.StimulateElement(IdentifierType.Id, "btnSlowAlert", "onclick", "input");
            _browser.AssertJSDialogContent("Please press Ok or Cancel", 20000);
            _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
        }

        [Test]
        public void AssertJSDialogWithZeroTimeoutFailsTest()
        {
            bool passed = false;

            try
            {
                _browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick", "input");
                _browser.AssertJSDialogContent("Please press Ok or Cancel", 0);
                _browser.ClickJSDialog(JScriptDialogButtonType.Cancel);
            }
            catch (AssertionFailedException)
            {
                passed = true;
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                OpenSwatTestPage();
            }

            Assert.IsTrue(passed);
        }

        [Test]
        public void AssertJSDialogContentNoMatchFailsTest()
        {
            string invalidDialogContent = "This dialog content is completely and utterly invalid.";
			_browser.StimulateElement(IdentifierType.Id, "btnAlert", "onclick", "input");
            bool passed = false;
            try
            {
                _browser.AssertJSDialogContent(invalidDialogContent);
            }
            catch (Exception e)
            {
                if (e.Message.Contains(string.Format("The open javascript dialog content is not equal to \"{0}\"", invalidDialogContent)))
                    passed = true;
            }

            if (passed)
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);

            Assert.IsTrue(passed);
        }

        [Test]
        public void AssertJSDialogContentNoDialogFailsTest()
        {
            bool passed = false;
            try
            {
                _browser.AssertJSDialogContent("This parameter is irrelevant");
            }
            catch (Exception e)
            {
                if (e.Message.Contains("There is no javascript dialog open"))
                    passed = true;
            }

            if (!passed)
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);

            Assert.IsTrue(passed);
        }

        #region OnBeforeUnload

        [Test]
        public void AssertJSDialogContentOnBeforeUnloadCloseBrowserTest()
        {
            if (_browserType == BrowserType.FireFox)
                Assert.Ignore("Firefox does not support custom OnBeforeUnload JS Dialog messages.");

            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.CloseBrowser();
                _browser.AssertJSDialogContent("haha");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void AssertJSDialogContentOnBeforeUnloadRefreshBrowserTest()
        {
            if (_browserType == BrowserType.FireFox)
                Assert.Ignore("Firefox does not support custom OnBeforeUnload JS Dialog messages.");

            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.RefreshBrowser();
                _browser.AssertJSDialogContent("haha");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void AssertJSDialogContentOnBeforeUnloadNavigateBrowserTest()
        {
            if (_browserType == BrowserType.FireFox)
                Assert.Ignore("Firefox does not support custom OnBeforeUnload JS Dialog messages.");

            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.AssertJSDialogContent("haha");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void AssertJSDialogContentOnBeforeUnloadStimulateElementTest()
        {
            if (_browserType == BrowserType.FireFox)
                Assert.Ignore("Firefox does not support custom OnBeforeUnload JS Dialog messages.");

            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.StimulateElement(IdentifierType.InnerHtml, "Google", "onclick", "a");
                _browser.AssertJSDialogContent("haha");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void AssertJSDialogContentOnBeforeUnloadKillAllOpenBrowsersWithWindowTitleTest()
        {
            if (_browserType == BrowserType.FireFox)
                Assert.Ignore("Firefox does not support custom OnBeforeUnload JS Dialog messages.");

            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.KillAllOpenBrowsers("facebook");
                _browser.AssertJSDialogContent("haha");
                _browser.ClickJSDialog(JScriptDialogButtonType.Cancel);
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
