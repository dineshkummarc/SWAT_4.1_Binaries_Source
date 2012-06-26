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

namespace SWAT.Tests.PressKeys
{
    [TestFixture, RequiresSTA, Timeout(250000)]
    [Category("IE")]
    public class InternetExplorerTests : PressKeysTestFixture
    {
        public InternetExplorerTests()
            : base(BrowserType.InternetExplorer)
        {

        }

        [Test]
        public void PressKeysFailsOnInvalidElementTest()
        {
            string exceptionMessage = "";
            SetForceBrowserPressKeys(true);
            try
            {
                _browser.PressKeys(IdentifierType.Id, "matchCountList", "this should fail");
            }
            catch (ArgumentException e)
            {
                exceptionMessage = e.Message;
            }
            finally
            {
                SetForceBrowserPressKeys(false);
            }

            Assert.AreEqual("Can't type text in a non-input or disabled element.", exceptionMessage);
        }

        //[Test]
        //public void PressKeysTextTooLongTest()
        //{
        //    Reflector.SetField(_browser, "_forceBrowserPressKeys", true);
        //    _browser.PressKeys(IdentifierType.Id, "txtPressKey", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
        //    "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +

        //    "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
        //        "input");
        //    string result = _browser.GetElementAttribute(IdentifierType.Id, "txtPressKey", "value", "input");
        //    Assert.AreEqual("abcdefghijklmnopqrstuvwxyz", result, "PressKeys is not working correctly for lowercase alphabet characters.");
        //}
    }
}
