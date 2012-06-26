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
    public abstract class GetElementAttributeTestFixture : BrowserTestFixture
    {
        public GetElementAttributeTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region GetElementAttribute

        [Test]
        public void GetElementAttributeTest()
        {
            string attribute;
            attribute = _browser.GetElementAttribute(IdentifierType.Id, "btnSetVal", "value");

            if (attribute.ToUpper() != "SET VALUE")
                Assert.Fail("Get Element Attribute fails when using 'Id' Identifier Type");

            attribute = "";
            attribute = _browser.GetElementAttribute(IdentifierType.Expression, "id:btnSetVal", "value");

            if (attribute.ToUpper() != "SET VALUE")
                Assert.Fail("Get Element Attribute fails when using Id in the 'Expression' Identifier Type");

            attribute = "";
            attribute = _browser.GetElementAttribute(IdentifierType.Expression, "value:Set Value", AttributeType.BuiltIn, "value", "INPUT");

            if (attribute.ToUpper() != "SET VALUE")
                Assert.Fail("Get Element Attribute fails when using value in the 'Expression' Identifier Type");

            attribute = _browser.GetElementAttribute(IdentifierType.Expression, "value:Set Value", AttributeType.BuiltIn, "value");

            if (attribute.ToUpper() != "SET VALUE")
                Assert.Fail("Get Element Attribute fails when using value in the 'Expression' Identifier Type");

            if (_browserType == BrowserType.FireFox)
            {
                attribute = _browser.GetElementAttribute(IdentifierType.Expression, "style:border-top-style:solid", AttributeType.BuiltIn, "style", "p");
                if (attribute.ToUpper() != "BACKGROUND-COLOR: RED; BOTTOM: 10PX;")
                    Assert.Fail(attribute+" Get Element Attribute fails when using non-inline style in the 'Expression' Identifier Type");
            }
        }

        #endregion

        #region GetElementAttribute with InnerHtml

        [Test]
        public void GetElementAttributeInnerHtmlTest()
        {
            Assert.AreEqual("This is inner html", _browser.GetElementAttribute(IdentifierType.Id, "tdInnerHtml", "innerHTML"));
        }

        [Test]
        public void GetElementAttributeInnerHtmlWhichContainsErrorExceptionAndFailedTextTest()
        {
            Assert.AreEqual("Error Exception Failed", _browser.GetElementAttribute(IdentifierType.Id, "textAreaErrorExcptFailed", "innerHTML"));
        }

        #endregion

        #region GetElementAttribute with A tag

        [Test]
        public void GetElementAttributeATagWithHref()
        {
            Assert.AreEqual("linkID", _browser.GetElementAttribute(IdentifierType.Expression, "href=http://www.apple.com/", "id", "a"));
            Assert.AreEqual("linkID", _browser.GetElementAttribute(IdentifierType.Expression, "href:apple", "id", "a"));
            Assert.AreEqual("http://www.google.com/", _browser.GetElementAttribute(IdentifierType.InnerHtml, "Google", "href", "a"));
        }

        [Test]
        public void GetElementAttributeATagWithId()
        {
            Assert.AreEqual("http://www.apple.com/", _browser.GetElementAttribute(IdentifierType.Id, "linkID", "href", "a"));
            Assert.AreEqual("http://www.apple.com/", _browser.GetElementAttribute(IdentifierType.Expression, "id=linkID", "href", "a"));
            Assert.AreEqual("http://www.apple.com/", _browser.GetElementAttribute(IdentifierType.Expression, "id:linkI", "href", "a"));
        }

        [Test]
        public void GetElementAttributeATagWithName()
        {
            Assert.AreEqual("linkID", _browser.GetElementAttribute(IdentifierType.Name, "linkName", "id", "a"));
            Assert.AreEqual("linkID", _browser.GetElementAttribute(IdentifierType.Expression, "id=linkID", "id", "a"));
            Assert.AreEqual("linkID", _browser.GetElementAttribute(IdentifierType.Expression, "id:linkID", "id", "a"));
        }

        [Test]
        public void GetElementAttributeATagWithInnerHtml()
        {
            Assert.AreEqual("linkID", _browser.GetElementAttribute(IdentifierType.InnerHtml, "Apple", "id", "a"));
            Assert.AreEqual("linkID", _browser.GetElementAttribute(IdentifierType.Expression, "innerHtml=Apple", "id", "a"));
            Assert.AreEqual("linkID", _browser.GetElementAttribute(IdentifierType.Expression, "innerHtml:Appl", "id", "a"));
        }

        [Test]
        public void GetElementAttributeATagWithInnerHtmlContains()
        {
            Assert.AreEqual("linkName", _browser.GetElementAttribute(IdentifierType.InnerHtmlContains, "Appl", "name", "a"));
        }

        #endregion

        #region GetElementAttribute with Special Characters

        [Test]
        public void GetElementAttributeSpecialCharacters1Test()
        {
            string word = "~!@#$%^&*()_+=-`";
            string result = @"~!@#$%^&amp;*()_+=-`";

            _browser.SetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", word, "span");
            string var = _browser.GetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", "span");
            Assert.AreEqual(result, var);
        }

        [Test]
        public void GetElementAttributeSpecialCharacters2Test()
        {
            string word = "<>?:\"{}|";
            string result = "&lt;&gt;?:\"{}|";

            _browser.SetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", word, "span");
            string var = _browser.GetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", "span");
            Assert.AreEqual(result, var);
        }

        [Test]
        public void GetElementAttributeSpecialCharacters3Test()
        {
            string word = @",./;[]'\";
            string result = @",./;[]'\";

            _browser.SetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", word, "span");
            string var = _browser.GetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", "span");
            Assert.AreEqual(result, var);
        }

        #endregion

        
        #region GetElementAttribute with Custom Attributes

        [Test]
        public void GetElementAttributeWorksWithCustomAttributeTest()
        {            
            string attribute = _browser.GetElementAttribute(IdentifierType.Id, "customAttributeTextBox", AttributeType.Custom, "testCustomAttrib", "input");
            Assert.AreEqual("pass", attribute);
        }

        [Test]
        public void GetElementAttributeReturnsEmptyStringOnCustomAttributeThatDoesNotExistTest()
        {
            string result = _browser.GetElementAttribute(IdentifierType.Id, "customAttributeTextBox", AttributeType.Custom, "I do not exist", "input");
            Assert.AreEqual(result, "");
        }

        #endregion
    }
}
