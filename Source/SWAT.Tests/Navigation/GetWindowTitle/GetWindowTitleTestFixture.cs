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
namespace SWAT.Tests.GetWindowTitle
{
    public abstract class GetWindowTitleTestFixture : BrowserTestFixture
    {
        public GetWindowTitleTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region GetWindowTitle

        [Test]
        public void GetWindowTitleTest()
        {
            try
            {
                this.AssertWindowTitle("SWAT Test Page");
                _browser.NavigateBrowser("www.google.com");
                this.AssertWindowTitle("Google");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

         
        [TestCase("")]
        [TestCase("Googl")]
        [TestCase("GOOGLE")]
        [TestCase("<script></script>")]
        public void GetWindowTitleFailsTest( string invalidString )
        {
            bool exceptionThrown = false;
            _browser.NavigateBrowser("www.google.com");

            try
            {
                this.AssertWindowTitle(invalidString);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }

            Assert.IsTrue(exceptionThrown, String.Format("GetWindowTitle should have failed on input {0}", invalidString));
        }


        [Test]
        public void GetWindowTitleWorksWithSpanishCharactersTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("SpanishCharactersPage.html"));
                this.AssertWindowTitle("ConfiguraciÓn salteña¡ krúss caffé¿ bahá'í güe Áetna eÑe Éthernet Ígloo Úruguay Über");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        #endregion

    }
}
