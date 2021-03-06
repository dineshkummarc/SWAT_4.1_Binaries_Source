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
    [TestFixture]
    [Category("Safari")]
    public class SafariTests : SetElementAttributeTestFixture
    {
        public SafariTests()
            : base(BrowserType.Safari)
        {

        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void SetFileInputNegativeMacTest()
        {
            string filePath = @"Macintosh HD\System\Library\Fonts\DoesNotExist.dfont";
            _browser.SetElementAttribute(IdentifierType.Expression, "id:fileInput", "value", filePath);

        }


        [Test]
        public void SetFileInputTestOnMac()
        {
            string filePath = @"Macintosh HD\System\Library\Fonts\Courier.dfont";
            _browser.SetElementAttribute(IdentifierType.Expression, "id:fileInput", "value", filePath);
            string temp = _browser.GetElementAttribute(IdentifierType.Id, "fileInput", "value");

            filePath = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
            Assert.AreEqual(filePath, temp);
        }
    }
}
