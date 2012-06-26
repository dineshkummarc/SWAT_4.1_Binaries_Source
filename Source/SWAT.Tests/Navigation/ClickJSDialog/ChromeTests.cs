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

using NUnit.Framework;
 

namespace SWAT.Tests.ClickJSDialog
{
    [TestFixture]
    [Category("Chrome")]
    public class ChromeTests : ClickJSDialogTestFixture
    {
        public ChromeTests()
            : base(BrowserType.Chrome)
        {

        }

        [TestCase(JScriptDialogButtonType.Cancel, "Dialog Test")]
        public void ClickJSDialogOnBeforeUnloadRecoverFromCommunicationLossTest(JScriptDialogButtonType jsBtnType, string expectedWebPage)
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.ClickJSDialog(jsBtnType);
                _browser.NavigateBrowser(getTestPage("OnBeforeUnload.htm"));
                //_browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.ClickJSDialog(jsBtnType);
                _browser.AssertBrowserExists(expectedWebPage);
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                OpenSwatTestPage();
            }
        }
    }
}
