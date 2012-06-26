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

namespace SWAT.Tests.AssertElementExists
{
    public abstract class AssertElementExistsTestFixture : BrowserTestFixture
    {
        public AssertElementExistsTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region AssertElementExists

        [TestCase(IdentifierType.Id, "txtOne")]
        public void AssertElementExistsTest(IdentifierType identType, string identifier)
        {
			_browser.AssertElementExists(identType, identifier);
        }


        [TestCase("id=tdInnerHtml;innerHTML=This is inner html", "td")]
        [TestCase("id=tdInnerHtml;innerHTML=T..s .s .n.e. .tm.", "td")]
        [TestCase("id=tdInnerHtml;innerHTML:html$", "td")]
        [TestCase("id=tdInnerHtml;innerHTML:n{2}", "td")]
        [TestCase("id=btn.*ar", "input")]
        [TestCase("id=tdInnerHtml;innerHTML:.+", "td")]
        [TestCase("id=tdInnerHtml;innerHTML=This is inner h@?tml", "td")]
        [TestCase("id=tdInnerHtml;innerHTML:[a-z ]+", "td")]
        [TestCase("innerHTML:This is inner html;innerHTML:File Input field:;innerHTML:Test list item close tags;innerHTML:Apple;innerHTML:Ann’s, Ann's, Ann`s", "*")]
        public void AssertElementExistsWithExpressionsTest(string expression, string tagName)
        {
            _browser.AssertElementExists(IdentifierType.Expression, expression, tagName);
            _browser.AssertElementExists(IdentifierType.Expression, expression);
        }

        [TestCase(IdentifierType.Expression, "id:IDontExist", 0, "input")]
        [TestCase(IdentifierType.Id, "txtOne", -1, "input")]
        [TestCase(IdentifierType.Id, "txtOn", 6000, "input")]
        public void AssertElementExistsWithTimeoutFailsTest(IdentifierType identType, string identifier, double timeOutLengthMilliseconds, string tagName)
        {
            DateTime startTime = DateTime.MinValue;
            bool correctTime = false;
            bool threwException = false;
            bool checkTimeout = timeOutLengthMilliseconds > 0;

            try
            {
                startTime = DateTime.Now;
                _browser.AssertElementExistsWithTimeout(identType, identifier, timeOutLengthMilliseconds, tagName);
            }
            catch (AssertionFailedException)
            {
                DateTime exceptionTime = DateTime.Now;
                threwException = true;

                if (timeOutLengthMilliseconds > 0)
                {
                    double expectedTimeoutModifier = (_browserType == BrowserType.Safari) ? 4000 : 2000; // since safari is ran on another computer, we need to expect a delay.
                    if (exceptionTime < startTime.AddMilliseconds(timeOutLengthMilliseconds + expectedTimeoutModifier)) //we should get our exception within the timeout value within 2 second.
                        //Increased due to sending 2 messages within the loop
                        correctTime = true;
                }
            }
            catch (ArgumentException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "AssertElementExists passed when it should have failed.");

            if (checkTimeout)
                Assert.IsTrue(correctTime, "AssertElementExists threw the correct exception but not within the timeout value.");
        }

        [Test]
        public void AssertElementExistsImageTypeWithDisabledAttributeTest()
        {
            _browser.AssertElementExistsWithTimeout(IdentifierType.Expression, "id:testImage;disabled=false", 5000, "input");
        }

        [Test]
        public void AssertElementPageWithinFrame()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("FramesetTestPage.htm"));
                _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id:logoutButton;class:loginName;innerHTML:Support, Workbrain");
                _browser.AssertElementExists(IdentifierType.Id, "btnId");
                _browser.AssertElementExists(IdentifierType.Name, "btnName");
                _browser.AssertElementExists(IdentifierType.InnerHtml, "Support, Workbrain");
                _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "Workbrain");
            }
            finally
            {
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
            }
         }

        [Test]
        public void AssertElementsWithinNestedFramesAndIFrames()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("NestedFramesTestPage.html"));
                _browser.AssertElementExists(IdentifierType.Id, "anotherInnerDiv", "div");
                _browser.AssertElementExists(IdentifierType.Id, "anotherInnerDiv");
                _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "Lorem ipsum dolor sit amet.", "p");
                _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "Lorem ipsum dolor sit amet.");
                _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id=form1", "input");
                _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id=form1");
                _browser.AssertElementExists(IdentifierType.Name, "innerPar", "p");
                _browser.AssertElementExists(IdentifierType.Name, "innerPar");
                _browser.AssertElementExists(IdentifierType.InnerHtml, "Inner paragraph", "p");
                _browser.AssertElementExists(IdentifierType.InnerHtml, "Inner paragraph");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }                            
        }

        [Test]
        public void SetTextboxByNameAndIdTest()
        {
            //Should work.
            _browser.SetElementAttribute(IdentifierType.Id, "txtOne", "value", "writing");
            Assert.AreEqual("writing", _browser.GetElementAttribute(IdentifierType.Id, "txtOne", "value"));

            //Should not find it, because the Name!= txtOne
            _browser.AssertElementDoesNotExist(IdentifierType.Name, "txtOne");

            //Should not find it, because the Id!= txtName
            _browser.AssertElementDoesNotExist(IdentifierType.Id, "txtName");

            //Should not find it, because the Name!= txtOne
            _browser.AssertElementDoesNotExist(IdentifierType.Name, "txtOne", "input");

            //Should not find it, because the Id!= txtName
            _browser.AssertElementDoesNotExist(IdentifierType.Id, "txtName", "input");
        }


        [Test]
        public void AssertElementExistsUsingUppercaseProperties()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "ID:chkOne;TYPE=checkbox;VALUE=check1;CHECKED=False", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "ID:btnNewWindow;VALUE:Window", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "ID:linkID;CLASS=myClass;NAME=linkName;HREF:apple;INNERHTML=Apple;STYLE:green", "a");
        }

        [Test]
        public void AssertCheckboxExistsWithDefaultValueTest()
        {
            _browser.AssertElementExistsWithTimeout(IdentifierType.Expression, "id:chkDefaultValue;value:on", 5000, "input");

            string val = _browser.GetElementAttribute(IdentifierType.Id, "chkDefaultValue", "value");
            Assert.AreEqual("on", val);
        }

        #endregion

        #region AssertElementExists with Expressions
        
        [Test]
        public void FindElementByExpressionWithRegexTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "id=btnAlert", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id=bt.*ert", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "id:tn.+er", "input");
        }
        
        //These tests reveal a bug in the IE logic.. Both of these tests are supposed to pass
        //[Test]
        //public void FindElementByIdAndNoTagNameFailsTest()
        //{
        //    _browser.AssertElementExists(IdentifierType.Id, "btn.*ert");
        //}

        [Test]
        public void FindElementByIdWithRegexTest()
        {
            _browser.AssertElementExists(IdentifierType.Id, "btnAlert", "input");
            _browser.AssertElementExists(IdentifierType.Id, "bt.*ert", "input");
            _browser.AssertElementExists(IdentifierType.Id, "btn.+ert", "input");
        }

        [Test]
        [ExpectedException(typeof(AssertionFailedException))]
        public void FindElementByExpressionFailsWhenAttributeLengthDoesNotMatchTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "id=tnAler", "input");
        }
        
        [Test]
        public void ExpressionWithInnerHtmlRegexTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml=Apple.*", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml=DIV .+ Case", "div");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml=DIV.*", "div");
        }

        [Test]
        public void ExpressionWithNameRegexTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "name=li.+Name", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "name=.*linkName", "a");
        }

        [Test]
        public void ExpressionWithInnerHtmlRegexWithNoTagsTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml=Apple.*");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml=DIV .+ Case");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml=DIV.*");
        }

        [Test]
        public void ExpressionWithNameRegexWithNoTagsTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "name=li.+Name");
            _browser.AssertElementExists(IdentifierType.Expression, "name=.*linkName");
        }

        //[Test]
        //public void ExpressionWithEventMatchWorksTest()
        //{
        //    _browser.AssertElementExists(IdentifierType.Expression, "onclick:doSomeJavascript", "input");
        //}

        [Test]
        public void LiteralMatchingTest()
        {
            bool elementDoesNotExist = false;
            _browser.AssertElementExists(IdentifierType.Expression, "id=tdInnerHtml;innerHTML=This is inner html", "td");

            try
            {
                _browser.AssertElementExists(IdentifierType.Expression, "id=tdInnerHtml;innerHTML=This is in1ner html", "td");
            }
            catch (AssertionFailedException)
            {
                elementDoesNotExist = true;
            }

            if (!elementDoesNotExist)
                Assert.Fail("Equals sign is passing even though element does not exist.");
        }

        [TestCase("3")]
        [TestCase("5")]
        [ExpectedException(typeof(AssertionFailedException))]
        public void MatchCountFailsTest(string count)
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("SightLessUserTestPage.htm"));
                _browser.AssertElementExists(IdentifierType.Expression, string.Format("innerHTML#{0}:<tr>", count), "table");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void MatchCountTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("SightLessUserTestPage.htm"));
                _browser.AssertElementExists(IdentifierType.Expression, "innerHTML#4:<tr>", "table");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }
        
        [Test]
        public void TestMatchCountOpenLITag()
        {
			_browser.AssertElementExists(IdentifierType.Expression, "id:matchCountList;innerHtml#3:<li>", "ul");
        }
        
        [Test]
        public void TestMatchCountCloseLITag()
        {
                _browser.AssertElementExists(IdentifierType.Expression, "id:matchCountList;innerHtml#3:</li>", "ul");
        }

        [Test]
        public void MatchAttributeWithEmptyStringTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.AssertElementExists(IdentifierType.Expression, "innerHtml:This;innerHtml:is;innerHtml:inner;innerHtml:html;innerHtml:", "td");
            }
            finally
            {
                this.NavigateToSwatTestPage();
            }
        }
        
        #endregion

        #region AssertElementExists with wrong tag name

         
        [TestCase(IdentifierType.Id, "btnSetVal", "a")]
        [TestCase(IdentifierType.Expression, "id=btnSetVal", "a")]
        [TestCase(IdentifierType.Id, "grvEmployeePays_firstSelect_0", "option")]
        [TestCase(IdentifierType.Expression, "id:grvEmployeePays_firstSelect_0", "option")]
        [ExpectedException(typeof(AssertionFailedException))]
        public void FindElementByIdWithWrongTagNameTest(IdentifierType identType, string identifier, string tagName)
        {
            _browser.AssertElementExists(identType, identifier, tagName); 
        }

        #endregion

        #region AssertElementExists by Style

        [Test]
        public void AssertElementByStyleTest()
        {
			_browser.AssertElementExists(IdentifierType.Expression, "style:width: 491px", "td");

            if (_browserType == BrowserType.Safari)
            {
                _browser.AssertElementExists(IdentifierType.Expression, "style:height:100px;color:(255, 0, 255)", "p");
                _browser.AssertElementExists(IdentifierType.Expression, "class:noninlinestyletestclass;style:height:100px;border-bottom-style: solid;border-top-style: solid", "p");
            }
            else if (_browserType == BrowserType.Chrome)
            {
                _browser.AssertElementExists(IdentifierType.Expression, "style:height:100px", "p");
                _browser.AssertElementExists(IdentifierType.Expression, "class:noninlinestyletestclass;style:height:100px", "p");
            }
            else
            {
                _browser.AssertElementExists(IdentifierType.Expression, "style:height:100px;top:10px", "p");
                _browser.AssertElementExists(IdentifierType.Expression, "class:noninlinestyletestclass;style:height:100px;top:10px", "p");
            }
        }

        #endregion

        #region AssertElementExists with parent element

        [Test]
        public void AssertParentElementTest()
        {
            //Id
            _browser.AssertElementExists(IdentifierType.Expression, "id:Val;parentElement.id:td[1-9]", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id:td1;id:chkOne", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id:td1;id:chkOne;value:check1", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id:td1;id:chkOne;value:check1");

            //Name
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.name:testDivName;id:testSpan", "span");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.name:testDivName;id:testSpan");

            //InnerHTML...
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML:Stop;parentElement.id:myId", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id:myId;innerHTML:Stop", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id:myId;innerHTML:Stop");

            //Class & ClassName
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML:Stop;parentElement.class:list", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class:list;innerHTML:Stop", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class:spn;value:testing", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.className:spn;value:testing", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "value:testing;parentElement.class:spn", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "value:testing;parentElement.className:spn", "input");
            _browser.AssertElementExists(IdentifierType.Expression, "value:testing;parentElement.className:spn");
        }

        [Test]
        public void AssertNestedParentElementTest()
        {
            //Testing ParentElement.ParentElement
            _browser.AssertElementExists(IdentifierType.Expression, "id:dd;parentElement.parentElement.parentElement.class:newclass", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.parentElement.parentElement.class:newclass;id:dd", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.parentElement.parentElement.class:newclass;id=dd");
        }

        [Test]
        public void AssertParentElementNegativeTest()
        {
            bool exceptionThrown = false;
            try { _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id:cmbOne;value:value265656", "option"); }
            catch (AssertionFailedException) { exceptionThrown = true; }

            Assert.IsTrue(exceptionThrown, "AssertElementExists with tag name failed to throw an exception.");
            exceptionThrown = false;

            try { _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id:cmbOne;value:value265656"); }
            catch (AssertionFailedException) { exceptionThrown = true; }

            Assert.IsTrue(exceptionThrown, "AssertElementExists with tag name failed to throw an exception.");
        }

        [Test]
        public void ParentElementCustomAttributesTest()
        {
            //Testing custom attributes. Different combinations.
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML:Stop;parentElement.direction:left", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML:Stop;parentElement.code:160", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML:Stop;parentElement.hightlight:false", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML:Stop;parentElement.currentcount:3", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "class:stepPause;parentElement.direction:left", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "class:stepPause;parentElement.code:160", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "class:stepPause;parentElement.hightlight:false", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "class:stepPause;parentElement.currentcount:3", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "id:dd;parentElement.direction:left", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "id:dd;parentElement.code:160", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "id:dd;parentElement.hightlight:false", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "id:dd;parentElement.currentcount:3", "a");
        }

        #endregion

        #region AssertElementExists with A tag

        [Test]
        public void AssertElementExistsATagWithHref()
        {
			_browser.AssertElementExists(IdentifierType.Expression, "href=http://www.apple.com/", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "href:apple", "a");
        }

        [Test]
        public void AssertElementExistsATagWithId()
        {
			_browser.AssertElementExists(IdentifierType.Id, "linkID", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "id=linkID", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "id:linkID", "a");
        }

        [Test]
        public void AssertElementExistsATagWithName()
        {
			_browser.AssertElementExists(IdentifierType.Name, "linkName", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "name=linkName", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "name:linkName", "a");
        }

        [Test]
        public void AssertElementExistsATagWithInnerHtml()
        {
			_browser.AssertElementExists(IdentifierType.InnerHtml, "Apple", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml=Apple", "a");

        }

        [Test]
        public void AssertElementExistsATagInnerHtmlContains()
        {
			_browser.AssertElementExists(IdentifierType.InnerHtmlContains, "Apple", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml:Apple", "a");
        }

        [Test]
        public void AssertElementExistsATagWithStyleTest()
        {
			_browser.AssertElementExists(IdentifierType.Expression, "style:color: green", "a");
        }

        [Test]
        public void AssertElementExistsATagWithClassTest()
        {
			_browser.AssertElementExists(IdentifierType.Expression, "class=myClass", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "class:myCla", "a");
        }

        [Test]
        public void AssertElementExistsATagWithParentElmentTest()
        {
			_browser.AssertElementExists(IdentifierType.Expression, "parentElement.class=newclass", "a");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class:newc", "a");
        }

        #endregion

        #region AssertElementExists with DIV tag

        [Test]
        public void AssertElementExistsDivTagWithInnerHtmlTest()
        {
			_browser.AssertElementExists(IdentifierType.InnerHtml, "DIV Test Case", "div");
            _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "DIV", "div");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml=DIV Test Case", "div");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml:DIV", "div");
        }

        [Test]
        public void AssertElementExistsDivTagWithIdTest()
        {
            _browser.AssertElementExists(IdentifierType.Id, "divTestCaseId", "div");
            _browser.AssertElementExists(IdentifierType.Expression, "id=divTestCaseId", "div");
            _browser.AssertElementExists(IdentifierType.Expression, "id:TestCaseId", "div");
        }

        [Test]
        public void AssertElementExistsDivTagWithNameTest()
        {
			_browser.AssertElementExists(IdentifierType.Name, "divTestCaseName", "div");
            _browser.AssertElementExists(IdentifierType.Expression, "name=divTestCaseName", "div");
            _browser.AssertElementExists(IdentifierType.Expression, "name:TestCaseName", "div");
        }

        [Test]
        public void AssertElementExistsDivWithStyleTest()
        {
			_browser.AssertElementExists(IdentifierType.Expression, "style:background-color", "div");
        }

        [Test]
        public void AssertElementExistsDivTagWithClassTest()
        {
			_browser.AssertElementExists(IdentifierType.Expression, "class=divTestCaseClass", "div");
            _browser.AssertElementExists(IdentifierType.Expression, "class:TestCaseClass", "div");
        }

        [Test]
        public void AssertElementExistsDivTagWithParentElmentTest()
        {
			_browser.AssertElementExists(IdentifierType.Expression, "parentElement.class=newclass;id=divTestCaseId", "div");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class:newc;id=divTestCaseId", "div");
        }


        #endregion

        #region AssertElementExists with SPAN tag

        [Test]
        public void AssertElementExistsSpanTagWithInnerHtmlTest()
        {
			_browser.AssertElementExists(IdentifierType.InnerHtml, "SPAN Test Case", "span");
            _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "SPAN", "span");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml=SPAN Test Case", "span");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml:SPAN", "span");
        }

        [Test]
        public void AssertElementExistsSpanTagWithIdTest()
        {
            _browser.AssertElementExists(IdentifierType.Id, "spanTestCaseId", "span");
            _browser.AssertElementExists(IdentifierType.Expression, "id=spanTestCaseId", "span");
            _browser.AssertElementExists(IdentifierType.Expression, "id:TestCaseId", "span");
        }

        [Test]
        public void AssertElementExistsSpanTagWithNameTest()
        {
            _browser.AssertElementExists(IdentifierType.Name, "spanTestCaseName", "span");
            _browser.AssertElementExists(IdentifierType.Expression, "name=spanTestCaseName", "span");
            _browser.AssertElementExists(IdentifierType.Expression, "name:TestCaseName", "span");
        }

        [Test]
        public void AssertElementExistsSpanWithStyleTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "style:background-color", "span");
        }

        [Test]
        public void AssertElementExistsSpanTagWithClassTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "class=spanTestCaseClass", "span");
            _browser.AssertElementExists(IdentifierType.Expression, "class:TestCaseClass", "span");
        }

        [Test]
        public void AssertElementExistsSpanTagWithParentElmentTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class=newclass;id=spanTestCaseId", "span");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class:newc;id=spanTestCaseId", "span");
        }

        #endregion

        #region AssertElementExists with OPTION tag

        [Test]
        public void AssertElementExistsOptionTagWithValueTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "value=optionTestValue1", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "value=optionTestValue2", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "value=optionTestValue3", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "value:TestValue1", "option");
        }

        [Test]
        public void AssertElementExistsOptionTagWithIdTest()
        {
            _browser.AssertElementExists(IdentifierType.Id, "optionTestId1", "option");
            _browser.AssertElementExists(IdentifierType.Id, "optionTestId2", "option");
            _browser.AssertElementExists(IdentifierType.Id, "optionTestId3", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "id=optionTestId1", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "id:TestId", "option");
        }

        [Test]
        public void AssertElementExistsOptionTagWithNameTest()
        {
            _browser.AssertElementExists(IdentifierType.Name, "optionTestName1", "option");
            _browser.AssertElementExists(IdentifierType.Name, "optionTestName2", "option");
            _browser.AssertElementExists(IdentifierType.Name, "optionTestName3", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "name=optionTestName1", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "name:optionTestName", "option");
        }

        [Test]
        public void AssertElementExistsOptionTagWithInnerHtmlTest()
        {
            _browser.AssertElementExists(IdentifierType.InnerHtml, "OPTION Test Case 1", "option");
            _browser.AssertElementExists(IdentifierType.InnerHtml, "OPTION Test Case 2", "option");
            _browser.AssertElementExists(IdentifierType.InnerHtml, "OPTION Test Case 3", "option");
            _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "Test Case 1", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "innerhtml=OPTION Test Case 2", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "innerhtml:Case 2", "option");
        }

        [Test]
        public void AssertElementExistsOptionTagWithMatchCountTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML#3:<option", "select");
        }

        [Test]
        public void AssertElementExistsOptionTagWithParentElementTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class=optionTestClass;id=optionTestId1", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class:onTestClass;id=optionTestId2", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class=optionTestClass;id:TestId1", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.class:optionTestC;id:ptionTes", "option");
        }

        [Test]
        public void AssertElementExistsOptionTagWithStyleTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "style:background-color: Fuchsia", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "style:background-color: Blue", "option");
            _browser.AssertElementExists(IdentifierType.Expression, "style:background-color:", "option");
        }

        #endregion

        #region AssertElementExists with TABLE tag

        [Test]
        public void AssertElementExistsTableTagWithInnerHtmlTest()
        {
            _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "This is inner html", "table");
            _browser.AssertElementExists(IdentifierType.InnerHtmlContains, ">This", "table");
            _browser.AssertElementExists(IdentifierType.Expression, "innerhtml:This is inner html", "table");
            _browser.AssertElementExists(IdentifierType.Expression, "innerhtml:>This", "table");
        }

        [Test]
        public void AssertElementExistsTableTagWithIdTest()
        {
            _browser.AssertElementExists(IdentifierType.Id, "tableTestId", "table");
            _browser.AssertElementExists(IdentifierType.Expression, "id=tableTestId", "table");
            _browser.AssertElementExists(IdentifierType.Expression, "id:bleTestI", "table");
        }

        [Test]
        public void AssertElementExistsTableTagWithClassTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "class=tableTestClass", "table");
            _browser.AssertElementExists(IdentifierType.Expression, "class:bleTestC", "table");
        }

        [Test]
        public void AssertElementExistsTableTagWithStyleTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "style:Silver", "table");
            _browser.AssertElementExists(IdentifierType.Expression, "style:ground-color:", "table");
        }

        [Test]
        public void AssertElementExistsTableTagWithMatchCountTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML#2:<tr", "table");
        }

        #endregion

        #region AssertElementExists with TR tag

        [Test]
        public void AssertElementExistsTrTagWithInnerHtmlTest()
        {
            _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "<td>Hello", "tr");
            _browser.AssertElementExists(IdentifierType.Expression, "innerhtml:<td>Hellos Wonderful", "tr");
        }

        [Test]
        public void AssertElementExistsTrTagWithIdTest()
        {
            _browser.AssertElementExists(IdentifierType.Id, "trTestId1", "tr");
            _browser.AssertElementExists(IdentifierType.Id, "trTestId2", "tr");
            _browser.AssertElementExists(IdentifierType.Expression, "id=trTestId1", "tr");
            _browser.AssertElementExists(IdentifierType.Expression, "id:rTestI", "tr");
        }

        [Test]
        public void AssertElementExistsTrTagWithClassTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "class=trTestClass1", "tr");
            _browser.AssertElementExists(IdentifierType.Expression, "class=trTestClass2", "tr");
            _browser.AssertElementExists(IdentifierType.Expression, "class:rTestC", "tr");
        }

        [Test]
        public void AssertElementExistsTrTagWithStyleTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "style:style: groove", "tr");
            _browser.AssertElementExists(IdentifierType.Expression, "style:style: double", "tr");
        }

        [Test]
        public void AssertElementExistsTrTagWithMatchCountTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML#2:<td", "tr");
        }

        [Test]
        public void AssertElementExistsTrTagWithParentElementTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.parentElement.id=tableTestId;id:trTestId1", "tr");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.parentElement.id:ableTestI;id=trTestId2", "tr");
        }

        #endregion

        #region AssertElementExists with TD tag

        [Test]
        public void AssertElementExistsTdTagWithInnerHtmlTest()
        {
            _browser.AssertElementExists(IdentifierType.InnerHtml, "Hello Wonderful", "td");
            _browser.AssertElementExists(IdentifierType.InnerHtmlContains, "Hello Wonderful", "td");
            _browser.AssertElementExists(IdentifierType.Expression, "innerhtml:Hello Wonderful", "td");
            _browser.AssertElementExists(IdentifierType.Expression, "innerhtml:Hello", "td");
        }

        [Test]
        public void AssertElementExistsTdTagWithIdTest()
        {
            _browser.AssertElementExists(IdentifierType.Id, "tdTestId1", "td");
            _browser.AssertElementExists(IdentifierType.Id, "tdTestId3", "td");
            _browser.AssertElementExists(IdentifierType.Expression, "id=tdTestId1", "td");
            _browser.AssertElementExists(IdentifierType.Expression, "id:dTestI", "td");
        }

        [Test]
        public void AssertElementExistsTdTagWithClassTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "class=tdTestClass1", "td");
            _browser.AssertElementExists(IdentifierType.Expression, "class=tdTestClass3", "td");
            _browser.AssertElementExists(IdentifierType.Expression, "class:dTestC", "td");
        }

        [Test]
        public void AssertElementExistsTdTagWithStyleTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "style:style: solid", "td");
            _browser.AssertElementExists(IdentifierType.Expression, "style:style: outset", "td");
        }

        [Test]
        public void AssertElementExistsTdTagWithMatchCountTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML#2:<option", "td");
        }

        [Test]
        public void AssertElementExistsTdTagWithParentElementTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id=trTestId1;id=tdTestId1", "td");
            _browser.AssertElementExists(IdentifierType.Expression, "parentElement.id:TestId2;id=tdTestId3", "td");
        }

        [Test]
        public void AssertElementExistsTdTagWithEventTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "onclick:Goodbye Wonderful", "td");
            _browser.AssertElementExists(IdentifierType.Expression, "onfocus:No more options", "td");
        }

        #endregion

        #region AssertElementExists with special characters

        [Test]
        public void AssertWithSlahesTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "href:http:\\/\\/www.google.com", "A");
        }

        [Test]
        public void AssertSpecialCharacters1Test()
        {
            string word = "~!@#$%^&*()_+=-`";
            string result = @"~!@#\$%\^&amp.\*\(\)_\+=-`";

            _browser.SetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", word, "span");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml:" + result, "span");
        }

        [Test]
        public void AssertSpecialCharacters2Test()
        {
            string word = "<>?:\"{}|";
            string result = "&lt.&gt.\\?:\"{}\\|";

            _browser.SetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", word, "span");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml:" + result, "span");
        }

        [Test]
        public void AssertSpecialCharacters3Test()
        {
            string word = @",./;[]'\";
            string result = @",./\;\[]'\\";

            _browser.SetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", word, "span");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml:" + result, "span");
        }

        [Test]
        public void AssertSpecialCharacters4Test()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("ElementsWithSpecialCharactersPage.htm"));

                string result = "#\\;&,\\.\\+\\*~':\"!\\^\\$\\[]\\(\\)=>\\|/";

                _browser.AssertElementExists(IdentifierType.Expression, "value:" + result, "input");
            }
            finally
            {
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void AssertApostrophesTest()
        {
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML:Ann`s", "A");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML:Ann.s", "A"); //For now we are putting a period in the case of a ’
            _browser.AssertElementExists(IdentifierType.Expression, "innerHTML:Ann's", "A");
        }
        
        #endregion

        #region AssertElementDoesNotExist

        [Test]
        [ExpectedException(typeof(AssertionFailedException))]
        public void AssertElementDoesNotExistThrowsAssertionFailedExceptionWhenLookingForElementThatDoesExistTest()
        {
            _browser.AssertElementDoesNotExist(IdentifierType.Id, "txtOne", "input");
        }

        #endregion
    }
}
