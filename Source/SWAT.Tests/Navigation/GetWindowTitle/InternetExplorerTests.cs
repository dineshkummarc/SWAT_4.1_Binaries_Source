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

namespace SWAT.Tests.GetWindowTitle
{
    [TestFixture, RequiresSTA]
    [Category("IE")]
    public class InternetExplorerTests : GetWindowTitleTestFixture
    {
        public InternetExplorerTests()
            : base(BrowserType.InternetExplorer)
        {

        }

        [Test]
        public void GetPDFWindowTitleTest()
        {
			_browser.StimulateElement(IdentifierType.Expression, "innerHtml:Form W-4", "onclick", "a");
            _browser.AttachToWindow("fw4.pdf");

            string error = "";
            try
            {
                string windowTitle = _browser.GetWindowTitle();
            }
            catch (BrowserDocumentNotHtmlException e)
            {
                error = e.Message;
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }

            Assert.That(error.Contains("This method only works on HTML documents."));
        }
    }
}
