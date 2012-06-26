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

namespace SWAT.Tests.AssertBrowserType
{
    [TestFixture]
    [Category("FireFox")]
    public class FireFoxTests : AssertBrowserTypeTestFixture
    {
        public FireFoxTests()
            : base(BrowserType.FireFox)
        {

        }

        #region AssertBrowserType

        [Test]
        public void AssertBrowserTypeTest()
        {
                _browser.AssertBrowserType("firefox");
        }

        [Test]
        [ExpectedException(typeof(IncorrectBrowserException), UserMessage = "Browser is not Safari")]
        public void AssertBrowserTypeFailsWhenNotFirefoxTest()
        {
                _browser.AssertBrowserType("safari");
        }

        #endregion
    }
}
