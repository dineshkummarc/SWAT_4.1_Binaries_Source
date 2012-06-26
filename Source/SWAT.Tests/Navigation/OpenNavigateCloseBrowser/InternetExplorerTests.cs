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


using System.CodeDom.Compiler;
using NUnit.Framework;
using System.Diagnostics;

namespace SWAT.Tests.OpenNavigateCloseBrowser
{
    [TestFixture, RequiresSTA]
    [Category("IE")]
    public class InternetExplorerTests : OpenNavigateCloseBrowserTestFixture
    {
        public InternetExplorerTests()
            : base(BrowserType.InternetExplorer)
        {
            
        }

        [Test]
        public void BringIEWindowToTopTest()
        {
            Process proc = new Process();

            proc.StartInfo.FileName = "notepad.exe";
            proc.Start();

            _browser.OpenBrowser();
            _browser.NavigateBrowser("www.google.com");

            _browser.AttachToWindow("SWAT Test Page");
            _browser.Sleep(5000);

            proc.Kill();
        }
    }
}
