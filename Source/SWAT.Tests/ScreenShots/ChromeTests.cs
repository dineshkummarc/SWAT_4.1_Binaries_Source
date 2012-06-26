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

namespace SWAT.Tests.ScreenShots
{
    [TestFixture]
    [Category("Chrome")]
    public class ChromeTests : ScreenShotTestFixture
    {
        public ChromeTests()
            : base(BrowserType.Chrome)
        {

        }

        #region Document Attribute Tests

        // The ScrollTop element is an issue in the Chrome browser itself (it can't be set)
        [Test]
        public void GetAndSetScrollTopTest()
        {
            int expected = 400;
            _browser.OpenBrowser();
            _browser.NavigateBrowser(getTestPage("TestPage.htm"));
            Browser browser = GetBrowserObject();
            browser.SetDocumentAttribute("scrollTop", expected);
            int actual = Int32.Parse(browser.GetDocumentAttribute("scrollTop").ToString());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAndSetDocumentAttributeFailTest()
        {
            _browser.OpenBrowser();
            _browser.NavigateBrowser(getTestPage("TestPage.htm"));

            string attributeName = "FakeElement";
            int attributeValue = 400;
            string expectedErrorMessage = string.Format("Unable to find element with {0} '{1}'", "Name", attributeName);
            string actualErrorMessage = "";
            Browser browser = GetBrowserObject();

            try
            {
                browser.SetDocumentAttribute(attributeName, attributeValue);
            }
            catch (ElementNotFoundException ex)
            {
                actualErrorMessage = ex.Message;
            }

            Assert.AreEqual(expectedErrorMessage, actualErrorMessage);
            actualErrorMessage = "";

            try
            {
                var actual = browser.GetDocumentAttribute(attributeName);
            }
            catch (ElementNotFoundException ex)
            {
                actualErrorMessage = ex.Message;
            }

            Assert.AreEqual(expectedErrorMessage, actualErrorMessage);

        }

        #endregion

        
    }
}

