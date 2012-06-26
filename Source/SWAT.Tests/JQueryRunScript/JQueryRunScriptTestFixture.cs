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
using System.Net;

namespace SWAT.Tests.JQueryRunScript
{
    public abstract class JQueryRunScriptTestFixture : BrowserTestFixture
    {
        public JQueryRunScriptTestFixture(BrowserType b)
            : base(b)
        {
        }

         
        [TestCase("new Function(\"window.top.frames[0].$('input#fname').val('Hello World!'); return window.top.frames[0].$('input#fname').val();\")();", "Hello World!")]
        [TestCase("new Function(\"window.top.frames[1].$('input#lname').val('Hello World!'); return window.top.frames[1].$('input#lname').val();\")();", "Hello World!")]
        public void SendJQueryFunctionsToLeftAndRightFrameFromMainWithoutJQueryTest(string jQueryCommand, string expectedValue)
        {
            _browser.NavigateBrowser(getTestPage("mainNoJQueryPlain.html"));

            try
            {
                _browser.RunScript("javascript", jQueryCommand, expectedValue);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SendJQueryFunctionToMainWithoutJQueryTest()
        {
            string jQuerycommand = "new Function(\"try { window.top.$('input#fname').val('Hello World!');} catch (e) { return 'true'; } return 'false';\")();";

            _browser.NavigateBrowser(getTestPage("mainNoJQueryPlain.html"));
            try
            {
                _browser.RunScript("javascript", jQuerycommand, "true");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SendJQueryFunctionToMainWithJQueryWithEqualChildrenWillNotSetChildElementsTest()
        {
            string jQueryCommand = "new Function(\"window.top.$('input').val('Hello World!'); return 'true';\")();";

            _browser.NavigateBrowser(getTestPage("mainJQueryEqualChildren.html"));
            try
            {
                _browser.RunScript("javascript", jQueryCommand, "true");
                Assert.AreEqual("", _browser.GetElementAttribute(IdentifierType.Id, "fname", "value", "input"));
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }
    }
}