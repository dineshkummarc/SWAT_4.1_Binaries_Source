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

namespace SWAT.Tests.AssertElementExists
{
    [TestFixture]
    [Category("Chrome")]
    public class ChromeTests : AssertElementExistsTestFixture
    {
        public ChromeTests()
            : base(BrowserType.Chrome)
        {

        }

        //@TODO Un-ignore when this jQuery bug has been fixed
        [Test]
        [Ignore]
        public void AssertElementWithParenthesisTest()
        {
            string word = "aaa(b";
            string result = @"aaa\(b";

            _browser.SetElementAttribute(IdentifierType.Id, "spanPressKey", "innerHtml", word, "span");
            _browser.AssertElementExists(IdentifierType.Expression, "innerHtml:" + result, "span");
        }
    }
}