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

namespace SWAT.Tests.AssertBrowserExists
{
    [TestFixture, RequiresSTA]
    [Category("IE")]
    public class InternetExplorerTests : AssertBrowserExistsTestFixture
    {
        public InternetExplorerTests()
            : base(BrowserType.InternetExplorer)
        {
        }

        [Test]
        public void AssertBrowserExistsModalDialogTest()
        {
            _browser.StimulateElement(IdentifierType.Id, "modalButton", "onclick");
            _browser.AttachToWindow("Modal");
            _browser.AssertBrowserExists("Modal Dialog Window");
            _browser.StimulateElement(IdentifierType.Id, "Button1", "onclick");

            _browser.AttachToWindow("SWAT Test Page");
            _browser.StimulateElement(IdentifierType.Id, "longModalTitle", "onclick");

            _browser.AssertBrowserExists("Long Modal");
            _browser.AttachToWindow("Long Modal");
            _browser.StimulateElement(IdentifierType.Id, "Button1", "onclick");

            _browser.AttachToWindow("SWAT Test Page");
        }

        [Test]
        public void AssertBrowserExistsModalDialogElementsTest()
        {
            _browser.StimulateElement(IdentifierType.Id, "modalButton", "onclick");
            _browser.AttachToWindow("Modal");
            _browser.SetElementAttribute(IdentifierType.Id, "Text1", Attributes.VALUE, "Successfully Attached to Modal Dialog");
            string value = _browser.GetElementAttribute(IdentifierType.Id, "Text1", "value", "input"); 
            Assert.AreEqual(value, "Successfully Attached to Modal Dialog");
            _browser.StimulateElement(IdentifierType.Id, "Button1", "onclick");

            _browser.AttachToWindow("SWAT Test Page");
            _browser.StimulateElement(IdentifierType.Id, "longModalTitle", "onclick");

            _browser.AttachToWindow("Long Modal");
            _browser.SetElementAttribute(IdentifierType.Id, "Text1", Attributes.VALUE, "Successfully Attached to Modal Dialog 2");
            value = _browser.GetElementAttribute(IdentifierType.Id, "Text1", "value", "input");
            Assert.AreEqual(value, "Successfully Attached to Modal Dialog 2");
            _browser.StimulateElement(IdentifierType.Id, "Button1", "onclick");

            _browser.AttachToWindow("SWAT Test Page");
        }

        [Test]
        public void AssertBrowserExistsModalDialogPressKeysTest()
        {
            _browser.StimulateElement(IdentifierType.Id, "modalButton", "onclick");

            _browser.AttachToWindow("Modal");
            _browser.PressKeys(IdentifierType.Id, "Text1", "Attach Successful", "input");
            string value = _browser.GetElementAttribute(IdentifierType.Id, "Text1", "value", "input");

            for (int i = 0; i < value.Length; i++)
            {
                _browser.PressKeys(IdentifierType.Id, "Text1", "\\{BACKSPACE\\}");
            }

            _browser.PressKeys(IdentifierType.Id, "Text1", "123Testing", "input");
            _browser.StimulateElement(IdentifierType.Id, "Button1", "onclick");

            _browser.AttachToWindow("SWAT Test Page");
            _browser.StimulateElement(IdentifierType.Id, "longModalTitle", "onclick");

            _browser.AttachToWindow("Long Modal");
            _browser.SetElementAttribute(IdentifierType.Id, "Text1", Attributes.VALUE, value);

            value = _browser.GetElementAttribute(IdentifierType.Id, "Text1", "value", "input");

            for (int i = 0; i < value.Length; i++)
            {
                _browser.PressKeys(IdentifierType.Id, "Text1", "\\{BACKSPACE\\}");
            }

            _browser.PressKeys(IdentifierType.Id, "Text1", "12343465767", "input");
            _browser.StimulateElement(IdentifierType.Id, "Button1", "onclick");

            _browser.AttachToWindow("SWAT Test Page");
        }
    }
}
