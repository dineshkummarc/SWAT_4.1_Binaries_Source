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
using System.Linq;
using NUnit.Framework;
using System.Text;
using SWAT;
using System.IO;

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("Misc")]
    public class AssertSWATVersionTestFixture
    {
        WebBrowser _browser;

        [Test]
        public void AssertSWATVersionEqual()
        {
            _browser = new WebBrowser(BrowserType.Null);
            string currVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2);

            _browser.AssertSWATVersionNumber(currVersion);
        }

        [Test]
        [ExpectedException(typeof(InvalidDataException))]
        public void AssertSWATVersionNotEqual()
        {
            _browser = new WebBrowser(BrowserType.Null);
            string currVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            _browser.AssertSWATVersionNumber("0.1");
        }
    }
}
