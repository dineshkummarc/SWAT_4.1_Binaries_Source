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
    public abstract class StimulateElementTestFixture : BrowserTestFixture
    {
        public StimulateElementTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }
                              
        //#region StimulateElement OnBlur

        //[Test]
        //public void IEBlurBugTest()
        //{
        //    if (_browserType == BrowserType.InternetExplorer)
        //    {
        //        //Ask users to try putting onfocus before changing the element
        //        _browser.StimulateElement(IdentifierType.Id, "onBlurTestBox", "onfocus", "input");

        //        _browser.SetElementAttribute(IdentifierType.Id, "onBlurTestBox", "value", "10", "input");
        //        _browser.StimulateElement(IdentifierType.Id, "onBlurTestBox", "onblur", "input");
        //        Assert.AreEqual("$10", _browser.GetElementAttribute(IdentifierType.Id, "onBlurTestBox", "value"));
        //    }
        //}

        //#endregion

        #region StimulateElement OnClick

        [Test]
        public void StimulateElementWaitsForFramesTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("MultiFrameSlowLoad.html"));
                DateTime end = DateTime.Now.AddSeconds(4);
                _browser.StimulateElement(IdentifierType.Expression, "target=targetFrame", "onclick", "a");
                _browser.AssertElementExistsWithTimeout(IdentifierType.Expression, "id=im a button;value=i am slow;type=button", 1000,"input");
                Assert.IsTrue(DateTime.Now >= end, "Browser is not waiting for all frames to finish loading");
            }
            finally
            {
                NavigateToSwatTestPage();
            }
        }

        [Test]
        public void ClickButtonThatExecutesLongAspCodeTest()
        {
            _browser.NavigateBrowser(getTestPage("SlowAspPage.aspx"));
            _browser.StimulateElement(IdentifierType.Id, "slowNavButton", "onclick");
            _browser.AssertElementExists(IdentifierType.Id, "tableTestId");
        }

        [Test]
        public void WaitForPageLoadOnStimulateElementClickHrefTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Expression, "innerHtml:Google", "onclick", "a");
                // Assert that the link navigated to google.com
                _browser.AssertBrowserExists("google");

                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.StimulateElement(IdentifierType.Expression, "innerHtml:Google", "onclick", "a");
                _browser.AssertBrowserExists("google");

                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.StimulateElement(IdentifierType.Id, "lnkGoogle", "onclick", "a");
                _browser.AssertBrowserExists("google");

                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.StimulateElement(IdentifierType.InnerHtml, "Google", "onclick", "a");
                _browser.AssertBrowserExists("google");

                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.StimulateElement(IdentifierType.InnerHtmlContains, "Google", "onclick", "a");
                _browser.AssertBrowserExists("google");

                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.StimulateElement(IdentifierType.Name, "nmGoogle", "onclick", "a");
                _browser.AssertBrowserExists("google");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void ClickButtonTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Id, "btnSetVal", "onclick");
                Assert.AreEqual("Test1", _browser.GetElementAttribute(IdentifierType.Id, "txtOne", "value"));

                _browser.StimulateElement(IdentifierType.Id, "btnClear", "onclick");
                Assert.AreEqual("", _browser.GetElementAttribute(IdentifierType.Id, "txtOne", Attributes.VALUE));
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }
        
        [Test]
        public void ClickCheckboxTest()
        {
            try
            {
                _browser.AssertElementExists(IdentifierType.Expression, "id:chkOne;checked:false");
                _browser.StimulateElement(IdentifierType.Id, "chkOne", "onclick");
                _browser.AssertElementExists(IdentifierType.Expression, "id:chkOne;checked:true");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void ClickCheckboxWithTagTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Id, "chkOne", "onclick", "INPUT");
                Assert.AreEqual("true", _browser.GetElementAttribute(IdentifierType.Id, "chkOne", "checked").ToLower());

                _browser.StimulateElement(IdentifierType.Id, "chkOne", "onclick", "INPUT");
                Assert.AreEqual("false", _browser.GetElementAttribute(IdentifierType.Id, "chkOne", "checked").ToLower());
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void StimulateElementOnClickOnDivTagTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Expression, "id:divTestCaseId", "onclick", "div");
                _browser.AssertElementExists(IdentifierType.Expression, "id:divTestCaseId;innerHtml:OnClick", "div");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void StimulateElementOnClickOnSpanTagTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Expression, "id:spanTestCaseId", "onclick", "span");
                _browser.AssertElementExists(IdentifierType.Expression, "id:spanTestCaseId;innerHtml:OnClick", "span");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void StimulateElementOnClickOnOptionTagTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Expression, "id:optionTestId1", "onclick", "option");
                _browser.AssertElementExists(IdentifierType.Expression, "id:optionTestId1;innerHtml:OnClick1", "option");

                _browser.StimulateElement(IdentifierType.Expression, "id:optionTestId2", "onclick", "option");
                _browser.AssertElementExists(IdentifierType.Expression, "id:optionTestId2;innerHtml:OnClick2", "option");

                _browser.StimulateElement(IdentifierType.Expression, "id:optionTestId3", "onclick", "option");
                _browser.AssertElementExists(IdentifierType.Expression, "id:optionTestId3;innerHtml:OnClick3", "option");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }
                                                              
        [Test]
        public void StimulateElementOnClickTdTagTest()
        {
            try
            {
                _browser.StimulateElement(IdentifierType.Id, "tdTestId1", "onclick", "td");
                _browser.AssertElementExists(IdentifierType.Expression, "id:tdTestId1;innerHtml:Goodbye Wonderful", "td");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void StimulateElementOnClickParentElementTest()
        {
            try
            {

                //Id
                _browser.StimulateElement(IdentifierType.Expression, "id:Val;parentElement.id:td[1-9]", "onclick", "input");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.id:td1;id:Val", "onclick", "input");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.id:td1;id:Val", "onclick");

                //Name
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.name:td1Name;id:btnSetVal", "onclick", "input");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.name:td1Name;id:btnSetVal", "onclick");

                //Class & ClassName
                _browser.StimulateElement(IdentifierType.Expression, "innerHTML:Stop;parentElement.class:list", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.class:list;innerHTML:Stop", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.class:list;innerHTML:Stop", "onclick");

                //InnerHTML
                _browser.StimulateElement(IdentifierType.Expression, "innerHTML:Stop;parentElement.id:myId", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.id:myId;innerHTML:Stop", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.id:myId;innerHTML:Stop", "onclick");

                //Other testing...
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.id:cmbOne;value:value2", "onclick", "option");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.id:cmbOne;value:value2", "onclick");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void StimulateElementOnClickNestedParentElementTest()
        {
            try
            {
                //Testing ParentElement.ParentElement
                _browser.StimulateElement(IdentifierType.Expression, "id:dd;parentElement.parentElement.parentElement.class:newclass", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.parentElement.parentElement.class:newclass;id:dd", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.parentElement.parentElement.class:newclass;id:dd", "onclick");

            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void StimulateElementOnClickParentElementNegativeTest()
        {
            try
            {

                _browser.StimulateElement(IdentifierType.Expression, "parentElement.id:cmbOne;value:value265656", "onclick", "option");
                _browser.StimulateElement(IdentifierType.Expression, "parentElement.id:cmbOne;value:value265656", "onclick");

            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void StimulateElementOnClickParentElementCustomAttributesTest()
        {
            try
            {

                //Testing custom attributes. Different combinations.
                _browser.StimulateElement(IdentifierType.Expression, "innerHTML:Stop;parentElement.direction:left", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "innerHTML:Stop;parentElement.code:160", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "innerHTML:Stop;parentElement.hightlight:false", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "innerHTML:Stop;parentElement.currentcount:3", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "class:stepPause;parentElement.direction:left", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "class:stepPause;parentElement.code:160", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "class:stepPause;parentElement.hightlight:false", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "class:stepPause;parentElement.currentcount:3", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "id:dd;parentElement.direction:left", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "id:dd;parentElement.code:160", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "id:dd;parentElement.hightlight:false", "onclick", "a");
                _browser.StimulateElement(IdentifierType.Expression, "id:dd;parentElement.currentcount:3", "onclick", "a");

            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        #endregion

        #region StimulateElement OnMouseOver

        [Test]
        public void MouseOverTest()
        {
            try
            {

                _browser.StimulateElement(IdentifierType.Id, "dvMouseOver", "onmouseover");
                string temp = _browser.GetElementAttribute(IdentifierType.Id, "dvMouseOver", Attributes.INNER_HTML);

            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        #endregion

        #region StimulateElement OnChange

        [Test]
        public void TestStimulateElementOnChange()
        {
            try
            {

                _browser.StimulateElement(IdentifierType.Id, "cmbOne", "onchange", "select");
                Assert.AreEqual("value1", _browser.GetElementAttribute(IdentifierType.Id, "optionLabel", "innerHtml"), "StimulateElement OnChange isn't working properly.");

            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void StimulateElementOnChangeTriggersJSDialogTest()
        {
            try
            {

                _browser.NavigateBrowser(getTestPage("OnChangeTestPage.htm"));
                _browser.SetElementAttribute(IdentifierType.Name, "selJSDialog", "value", "1", "select");
                _browser.StimulateElement(IdentifierType.Name, "selJSDialog", "onchange", "select");
                _browser.ClickJSDialog(JScriptDialogButtonType.Ok);

            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        #endregion

        #region StimulateElement OnFocus

        [Test]
        public void StimulateElementOnFocusOnOptionTagTest()
        {
            try
            {

                _browser.StimulateElement(IdentifierType.Expression, "id:optionTestId1", "onfocus", "option");
                _browser.AssertElementExists(IdentifierType.Expression, "id:optionTestId1;innerHtml:OnFocus1", "option");

                _browser.StimulateElement(IdentifierType.Expression, "id:optionTestId2", "onfocus", "option");
                _browser.AssertElementExists(IdentifierType.Expression, "id:optionTestId2;innerHtml:OnFocus2", "option");

                _browser.StimulateElement(IdentifierType.Expression, "id:optionTestId3", "onfocus", "option");
                _browser.AssertElementExists(IdentifierType.Expression, "id:optionTestId3;innerHtml:OnFocus3", "option");

            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        #endregion 

        #region

        [Test]
        public void StimulateElementOnKeyEventTest()
        {
            //test onKeyUp
            _browser.StimulateElement(IdentifierType.Id, "txtBox1", "onkeyup", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTxtBox1;innerHtml=keyup", "div");

            //test onKeyDown
            _browser.StimulateElement(IdentifierType.Id, "txtBox2", "onkeydown", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTxtBox2;innerHtml=keydown", "div");

            //testOnKeyPress
            _browser.StimulateElement(IdentifierType.Id, "txtBox3", "onkeypress", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTxtBox3;innerHtml=keypress", "div");
        }

        [Test]
        public void StimulateElementOnUpperCaseKeyEventTest()
        {
            //test onKeyUp
            _browser.StimulateElement(IdentifierType.Id, "txtBox1", "ONKEYUP", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTxtBox1;innerHtml=keyup", "div");

            //test onKeyDown
            _browser.StimulateElement(IdentifierType.Id, "txtBox2", "OnKeyDown", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTxtBox2;innerHtml=keydown", "div");

            //testOnKeyPress
            _browser.StimulateElement(IdentifierType.Id, "txtBox3", "onKEYPress", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTxtBox3;innerHtml=keypress", "div");
        }

        #endregion

        #region StimulateElement throws Exception...

        [Test]
        [ExpectedException(typeof(InvalidEventException))]
        public void StimulateElementThrowsInvalidEventExceptionTest()
        {
            _browser.StimulateElement(IdentifierType.Id, "btnSetVal", "nonEvent", "input");
        }

        #endregion
    }
}
