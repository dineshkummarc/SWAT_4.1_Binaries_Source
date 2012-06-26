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
using SWAT;
namespace SWAT.Tests.GetLocation
{
    public abstract class GetLocationTestFixture : BrowserTestFixture
    {
        public GetLocationTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region GetLocation

        [Test]
        public void GetLocationTest()
        {
            Assert.That(_browser.GetLocation().Contains("/swat/TestPage.htm"));
        }

        [Test]
        public void GetLocationTestA()
        {
            try
            {
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.google.com");
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.w3schools.com");

                Assert.That(_browser.GetLocation().Contains("w3schools.com"));
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        #endregion
    }
}
