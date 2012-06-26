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
using System.IO;

namespace SWAT.Tests.SetElementAttribute
{
    public abstract class SetElementAttributeTestFixture : BrowserTestFixture
    {
        public SetElementAttributeTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region SetElementAttribute

        //Missing most test cases for generic SetElementAttribute

        [Test]
        public void SetTextboxByIdTest()
        {
            try
            {
                _browser.SetElementAttribute(IdentifierType.Expression, "id:txtOne", "value", "Testing111");
                Assert.AreEqual("Testing111", _browser.GetElementAttribute(IdentifierType.Id, "txtOne", "value"));

                _browser.SetElementAttribute(IdentifierType.Expression, "id:txtOne", "value", "");
                Assert.AreEqual("", _browser.GetElementAttribute(IdentifierType.Id, "txtOne", "value"));
            }
            finally
            {
                // Clean up : resets all the elements
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SetElementUsingIdWithWrongTagNameTest()
        {
            bool firstException = false;
            bool secondException = false;
            try
            {
                _browser.SetElementAttribute(IdentifierType.Expression, "id:grvEmployeePays_firstSelect_0", "value", "Employee number");
                _browser.SetElementAttribute(IdentifierType.Id, "grvEmployeePays_firstSelect_0", "value", "Employee number");
                _browser.SetElementAttribute(IdentifierType.Expression, "id:grvEmployeePays_firstSelect_0", "value", "Employee number", "select");
                _browser.SetElementAttribute(IdentifierType.Id, "grvEmployeePays_firstSelect_0", "value", "Employee number", "select");

                try
                {
                    _browser.SetElementAttribute(IdentifierType.Expression, "id:grvEmployeePays_firstSelect_0", "value", "Employee number", "option");
                }
                catch (ElementNotFoundException)
                {
                    firstException = true;
                }

                try
                {
                    _browser.SetElementAttribute(IdentifierType.Id, "grvEmployeePays_firstSelect_0", "value", "Employee number", "option");

                }
                catch (ElementNotFoundException)
                {
                    secondException = true;
                }

                Assert.IsTrue(firstException && secondException, string.Format("expecting True and True, but got {0} and {1}", firstException, secondException));
            }
            finally
            {
                // Clean up : resets all the elements
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SetElementAttributeOfElementInFrameTest()
        {
            _browser.SetElementAttribute(IdentifierType.Id, "txtBox1", "value", "test", "input");
            Assert.AreEqual("test", _browser.GetElementAttribute(IdentifierType.Id, "txtBox1", "value", "input"));
        }

        [Test]
        public void SetElementAttributeSingleQuotesTest()
        {
            _browser.SetElementAttribute(IdentifierType.Id, "txtBox1", "value", "t''est", "input");
            Assert.AreEqual("t''est", _browser.GetElementAttribute(IdentifierType.Id, "txtBox1", "value", "input"));
        }

        [Test]
        public void SetElementAttributeWithBooleanTest()
        {
            _browser.SetElementAttribute(IdentifierType.Id, "firstname", "readonly", "true", "input");
            Assert.AreEqual("true", _browser.GetElementAttribute(IdentifierType.Id, "firstname", "readonly", "input").ToLower());
        }

        #endregion

        #region SetElementAttribute with input elements

        [Test]
        public void SetCheckboxTest()
        {
            try
            {
                _browser.SetElementAttribute(IdentifierType.Id, "chkOne", AttributeType.BuiltIn, "checked", "true");
                _browser.AssertElementExists(IdentifierType.Expression, "id:chkOne;checked:true");
                _browser.SetElementAttribute(IdentifierType.Expression, "id:chkOne", AttributeType.Custom, "checked", "true", "input");
                _browser.AssertElementExists(IdentifierType.Expression, "id:chkOne;checked:true");

                _browser.SetElementAttribute(IdentifierType.Id, "chkOne", AttributeType.BuiltIn, "checked", "false", "input");
                _browser.AssertElementExists(IdentifierType.Expression, "id:chkOne;checked:false");
                _browser.SetElementAttribute(IdentifierType.Expression, "id:chkOne", AttributeType.Custom, "checked", "false", "input");
                _browser.AssertElementExists(IdentifierType.Expression, "id:chkOne;checked:false");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SetSelectBoxValueTest()
        {
            try
            {
                _browser.SetElementAttribute(IdentifierType.Id, "cmbOne", Attributes.VALUE, "value2");
                Assert.AreEqual("value2", _browser.GetElementAttribute(IdentifierType.Id, "cmbOne", Attributes.VALUE));
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SetTextareaValueTest()
        {
            try
            {
                _browser.SetElementAttribute(IdentifierType.Expression, "id:txtAreaOne", "value", "Testing111");
                string temp = _browser.GetElementAttribute(IdentifierType.Id, "txtAreaOne", "value");
                Assert.AreEqual("Testing111", temp);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SetSelectBoxWithValueEqualToTrueOrFalseTest()
        {
            try
            {
                _browser.SetElementAttribute(IdentifierType.Id, "ctl00_Content_AddHideBehavior", "value", "False", "select");
                string result = _browser.GetElementAttribute(IdentifierType.Id, "ctl00_Content_AddHideBehavior", "value", "select");
                Assert.AreEqual(result, "False", "Select box option value should be False");
                _browser.SetElementAttribute(IdentifierType.Id, "ctl00_Content_AddHideBehavior", "value", "True", "select");
                result = _browser.GetElementAttribute(IdentifierType.Id, "ctl00_Content_AddHideBehavior", "value", "select");
                Assert.AreEqual(result, "True", "Select box option value should be True");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SetFileInputTest()
        {
            if (_browserType == BrowserType.Safari)
                Assert.Ignore("SetFileInputTestOnMac is the Safari equivalent of this test.");

            try
            {
                string filePath = @"C:\SWAT\trunk\SWAT.Tests\TestPages\TestPage.htm";
                _browser.SetElementAttribute(IdentifierType.Expression, "id:fileInput", "value", filePath);
                string temp = _browser.GetElementAttribute(IdentifierType.Id, "fileInput", "value");

                filePath = getFileNameFromFilePath(filePath);
                temp = getFileNameFromFilePath(temp);

                Assert.AreEqual(filePath, temp);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void SetFileInputNegativeTest()
        {
            string filePath = @"C:\SWAT\trunk\SWAT.Tests\TestPages\DoesNotExist.htm";
            _browser.SetElementAttribute(IdentifierType.Expression, "id:fileInput", "value", filePath);
        }

        [Test]
        public void SetTextareaValueWithSingleQuoteTest()
        {
            try
            {
                _browser.SetElementAttribute(IdentifierType.Expression, "id:txtAreaOne", "value", "'Testing111'");
                string temp = _browser.GetElementAttribute(IdentifierType.Id, "txtAreaOne", "value");
                Assert.AreEqual("'Testing111'", temp);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SetTextareaValueWithNewLineCharacterTest()
        {
            try
            {
                _browser.SetElementAttribute(IdentifierType.Expression, "id:txtAreaOne", "value", String.Format("Testing{0}111", System.Environment.NewLine));
                string temp = _browser.GetElementAttribute(IdentifierType.Id, "txtAreaOne", "value");
				Assert.That(temp.Contains("\n"));
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void SetSelectedOptionTest()
        {
            try
            {
                _browser.SetElementAttribute(IdentifierType.Id, "optionTestId3", AttributeType.BuiltIn, "selected", "true", "option");
                string temp = _browser.GetElementAttribute(IdentifierType.Expression, "class=optionTestClass", "selectedIndex");
                Assert.AreEqual(temp, "2");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void CascadingDropdownTest()
        {
            try
            {
                _browser.SetElementAttribute(IdentifierType.Id, "ctl00_Content_AddHideBehavior", "value", "True", "select");
                _browser.StimulateElement(IdentifierType.Id, "ctl00_Content_AddHideBehavior", "onchange", "select");
                _browser.SetElementAttribute(IdentifierType.Id, "employeeStatus", "value", "Full", "select");
                _browser.StimulateElement(IdentifierType.Id, "employeeStatus", "onchange", "select");
                string hours = _browser.GetElementAttribute(IdentifierType.Id, "txtHours", "value", "input");

                Assert.AreEqual("40", hours);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }           
        }

        #endregion

        #region SetElementAttribute with special characters

        [Test]
        public void SetElementAttributeSelectBoxValueWithInternationalCharactersTest()
        {

            try
            {
                _browser.AssertElementExists(IdentifierType.Expression, "id:GridView1;value:option", "select");
                _browser.SetElementAttribute(IdentifierType.Expression, "id:GridView1", "value", "Numéro d’employé", "select");
                _browser.AssertElementExists(IdentifierType.Expression, "id:GridView1;selectedIndex=1", "select");
                _browser.AssertElementExists(IdentifierType.Expression, "index=1;value:Num.ro d.employ.", "option");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        #endregion 

        private string getFileNameFromFilePath(string filePath)
        {
            if (filePath.IndexOf(@"\") != -1)
                return filePath.Substring(filePath.LastIndexOf(@"\") + 1);
            return filePath;
        }
    }
}
