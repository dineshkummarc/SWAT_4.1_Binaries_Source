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

namespace SWAT.Tests.StimulateElement
{
    [TestFixture, RequiresSTA]
    [Category("IE")]
    public class InternetExplorerTests : StimulateElementTestFixture
    {
        public InternetExplorerTests()
            : base(BrowserType.InternetExplorer)
        {

        }

        #region StimulateElement fails with FormEvents

        [Test]
        public void UnsupportedFormEventsErrorMessageTest()
        {
            string[] events = { "onload", "onunload", "onerror", "onresize" };
            string id = "id=myform";
            try
            {
                foreach (string ev in events)
                {
                    try
                    {
                        _browser.StimulateElement(IdentifierType.Expression, id, ev, "form");
                    }
                    catch (StimulateElementException ex)
                    {
                        Assert.AreEqual(ex.Message, string.Format("Could not fire {0} on element with {1}", ev, id));
                    }
                }
            }
            finally
            {
                // Clean up
                // NOTE: this clean up is not supposed to be necessary and is currently a work around for a bug
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        #endregion

          #region StimulateElement fails with PDF

        [Test]
        [ExpectedException(typeof(BrowserDocumentNotHtmlException))]
        public void TestStimulateElementFailsInPDF()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Expression, "innerHtml:Form W-4", "onclick", "a");
                _browser.AttachToWindow("fw4.pdf");
                _browser.StimulateElement(IdentifierType.Id, "btnSetVal", "onclick");
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        #endregion 

        [Test]
        public void StimulateElementOnChangeFramesetFramePageTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("OnChangeFrame.htm"));                                      
                _browser.SetElementAttribute(IdentifierType.Name, "selConnectASP", "value", "1", "select");
                _browser.StimulateElement(IdentifierType.Name, "selConnectASP", "onchange", "select");
                _browser.AssertElementExists(IdentifierType.Expression, "id:selConnectASPResult;innerHtml:1", "span");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }
    }
}
