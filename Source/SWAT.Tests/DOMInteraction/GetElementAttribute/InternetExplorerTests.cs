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

namespace SWAT.Tests.GetElementAttribute
{
    [TestFixture, RequiresSTA]
    [Category("IE")]
    public class InternetExplorerTests : GetElementAttributeTestFixture
    {
        public InternetExplorerTests()
            : base(BrowserType.InternetExplorer)
        {

        }

        [Test]
        public void LengthOfGetElementAttributeReturnValueTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("CharacterTestPage.htm"));
                System.Text.StringBuilder test = new System.Text.StringBuilder();
                test.Append(_browser.GetElementAttribute(SWAT.IdentifierType.Expression, "innerHTML:test", "innerHTML", "HTML"));
                Assert.AreEqual(test.Length, 215576);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        [ExpectedException(typeof(BrowserDocumentNotHtmlException), UserMessage = "This method only works on HTML documents.")]
        public void GetElementAttributeFailsInPDFTest()
        {
            _browser.StimulateElement(IdentifierType.Expression, "innerHtml:Form W-4", "onclick", "a");
            _browser.AttachToWindow("fw4.pdf");
            try
            {
                _browser.GetElementAttribute(IdentifierType.Id, "btnSetVal", "value"); // Should throw BrowserDocumentNotHtmlException 
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }
    }
}
