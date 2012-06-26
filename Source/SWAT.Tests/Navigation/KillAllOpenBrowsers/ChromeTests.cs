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
 

namespace SWAT.Tests.KillAllOpenBrowsers
{
    [TestFixture]
    [Category("Chrome")]
    public class ChromeTests : KillAllOpenBrowsersTestFixture
    {
        public ChromeTests()
            : base(BrowserType.Chrome)
        {

        }

        [TestCase(15)]
        public void OpenKillBrowsersTest(int iterations)
        {
            try
            {
                for (int i = 0; i < iterations; i++)
                {
                    _browser.KillAllOpenBrowsers();
                    _browser.OpenBrowser();
                }
            }
            finally
            {
                this.OpenSwatTestPage();
            }
        }

        #region KillAllOpenBrowsers except specified window

         
        [TestCase("Google", new string[] { "about:swat", "SWAT Test Page", "fw.pdf" })]
        [TestCase("about:swat", new string[] { "Google", "SWAT Test Page", "fw.pdf" })]
        [TestCase("fw4.pdf", new string[] { "about:swat", "SWAT Test Page", "Google" })]
        [TestCase("SWAT Test Page", new string[] { "about:swat", "Google", "fw.pdf" })]
        public void KillAllOpenBrowsersExceptWindowTitleTest(string sparedBrowser, string[] killedBrowsers)
        {
            try
            {
                int numBrowsers = 3;
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
                // Make sure we haven't closed ANY Google windows by attaching to all of them
                for (int i = 0; i < numBrowsers; i++)
                {
                    _browser.AttachToWindow(sparedBrowser, i);
                }
                foreach (string killedBrowser in killedBrowsers)
                {
                    _browser.AssertBrowserDoesNotExist(killedBrowser);
                }
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        #endregion
    }
}
