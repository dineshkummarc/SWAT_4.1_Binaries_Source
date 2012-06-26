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
using Microsoft.Win32;
using System.IO;

namespace SWAT.Tests.OpenNavigateCloseBrowser
{
    public abstract class OpenNavigateCloseBrowserTestFixture : BrowserTestFixture
    {
        public OpenNavigateCloseBrowserTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region OpenAndCloseBrowser
        

        [Test]
        public void OpenBrowserCloseBrowserOpenBrowserTest()
        {
            try
            {
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.google.com");
                _browser.CloseBrowser();

                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.google.com");
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.AttachToWindow("Google");
                _browser.CloseBrowser();
                _browser.AttachToWindow("SWAT Test Page");
                _browser.CloseBrowser();

                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.google.com");
                _browser.CloseBrowser();
            }
            finally
            {
                //Clean up
                this.OpenSwatTestPage();
            }
        }

        /// <summary>
        /// Stress test to make sure opening multiple browsers won't cause SWAT to crash
        /// </summary>
        [Test]
        public void OpenBrowserMultipleTimesTest()
        {
            try
            {
                int count = 30;
                for (int i = 0; i < count; i++)
                {
                    _browser.OpenBrowser();
                }
            }
            finally
            {
                // Clean up
                this.OpenSwatTestPage();
            }
        }

        #endregion


        #region Navigate Browser

         
        [TestCase("www.google.com")]
        [TestCase("google.com")]
        public void NavigateBrowserWithUrlMissingPrefixTest(string url)
        {
            try
            {
                _browser.NavigateBrowser(url);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        

        //[Test]
        //public void NavigateBrowsersTestWithHistory()
        //{
        //    _browser.OpenBrowser();
        //    _browser.NavigateBrowser(@"http://en.wikipedia.org/wiki/Main_Page");
        //    _browser.AssertBrowserExists("Wikipedia");
        //    _browser.NavigateBrowser(@"http://www.google.com/");
        //    _browser.AssertBrowserExists("Google");
        //    _browser.NavigateBrowser(@"http://www.abstrusegoose.com");
        //    _browser.AssertBrowserExists("Abstruse Goose");
        //    _browser.NavigateBrowser(@"http://java.sun.com/javase/6/docs/api/");
        //    _browser.AssertBrowserExists("Java Platform");
        //    _browser.NavigateBrowser(@"http://www.bing.com");
        //    _browser.AssertBrowserExists("Bing");

        //    _browser.RunScript("new Function('window.history.back();return true;')();", "true");
        //    _browser.AssertBrowserExists("Java Platform");

        //    _browser.RunScript("new Function('window.history.back();return true;')();", "true");
        //    _browser.AssertBrowserExists("Abstruse Goose");

        //    _browser.RunScript("new Function('window.history.back();return true;')();", "true");
        //    _browser.AssertBrowserExists("Google");

        //    _browser.RunScript("new Function('window.history.back();return true;')();", "true");
        //    _browser.AssertBrowserExists("Wikipedia");


        //    //Ending tabs: SWAT, Abstruse Goose, Java, Bing
        //}

        #endregion
    }
}
