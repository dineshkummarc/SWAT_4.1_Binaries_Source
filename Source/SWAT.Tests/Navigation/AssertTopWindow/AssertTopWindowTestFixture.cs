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
using NUnit.Framework;

namespace SWAT.Tests.AssertTopWindow
{
    public abstract class AssertTopWindowTestFixture : BrowserTestFixture
    {
        public AssertTopWindowTestFixture(BrowserType browserType) 
            : base(browserType) 
        { 
        
        }

        #region AssertTopWindow

        [Test]
        [ExpectedException(typeof(TopWindowMismatchException))]
        public void AssertTopWindowFailsTest()
        {
            _browser.AssertTopWindow("This title makes no sense");
        }

        [Test]
        public void AssertTopWindowInformativeExceptionsTest()
        {
            string fakeTitle = "fake window title 123";
            bool backup = SWAT.WantInformativeExceptions.GetInformativeExceptions;
            SWAT.WantInformativeExceptions.GetInformativeExceptions = true;
            SWAT.UserConfigHandler.Save();

            try
            {
                _browser.AssertTopWindow(fakeTitle);
            }
            catch (TopWindowMismatchException e)
            {
                Assert.IsTrue(e.Message.Contains("SWAT Test Page") && e.Message.Contains(fakeTitle), "Incorrect message from informative exceptions, actual message : " + e.Message);
            }
            finally
            {
                SWAT.WantInformativeExceptions.GetInformativeExceptions = backup;
                SWAT.UserConfigHandler.Save();
            }
        }

        [Test]
        public void AssertTopWindowWithIndexFailsWithCorrectCount()
        {
            bool passed = false;
            int index = 3;

            try
            {
                _browser.AssertTopWindow("SWAT Test Page", index);
            }
            catch (IndexOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains( index + " is too large, there are only 1 window(s)"));
                passed = true;
            }

            Assert.IsTrue(passed);
        }

        [Test]
        public void AssertTopWindowWithIndexTimeout()
        {
            int defaultTimeout = 10;
            DateTime end = DateTime.Now.AddSeconds(defaultTimeout);
            try
            {
                _browser.AssertTopWindow("find this", 2);
            }
            catch
            {
                Assert.IsTrue(DateTime.Now > end && (_browserType == BrowserType.Safari ||  DateTime.Now <= end.AddSeconds(2)));
            }
        }

        [Test]
        public void AssertTopWindowCustomTimeoutTest()
        {
            DateTime end = DateTime.Now.AddSeconds(2);
            try
            {
                _browser.AssertTopWindow("some bogus title", 0, 2);
            }
            catch (TopWindowMismatchException)
            {
                Assert.IsTrue(DateTime.Now >= end && (_browserType == BrowserType.Safari || DateTime.Now <= end.AddSeconds(2)), "AssertTopWindow is not correctly handling timeouts");
            }
        }

        [Test]
        public void AssertTopWindowHandlesAttachToBrowserTest()
        {
            try
            {
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.wikipedia.org");
                _browser.AssertTopWindow("Wikipedia");

                _browser.AttachToWindow("SWAT Test Page");
                _browser.AssertTopWindow("SWAT Test Page");
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                _browser.OpenBrowser();
                NavigateToSwatTestPage();
            }
        }

        [Test]
        public void AssertTopWindowWithIndexPasses()
        {
            try
            {
                _browser.NavigateBrowser("www.google.com");
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.google.com");
                _browser.Sleep(10000);
                _browser.AssertTopWindow("Google", 1); //Zero-index
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                _browser.OpenBrowser();
                NavigateToSwatTestPage();
            }
        }

        [Test]
        [ExpectedException(typeof(TopWindowMismatchException))]
        public void AssertTopWindowWithIndexFails()
        {
            try
            {
                _browser.OpenBrowser();
                NavigateToSwatTestPage();
                _browser.OpenBrowser();
                NavigateToSwatTestPage();

                _browser.AssertTopWindow("swat test page", 1);
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                _browser.OpenBrowser();
                NavigateToSwatTestPage();
            }
        }

        [Test]
        public void AssertTopWindowWithIndexAndInformativeExceptionsFails()
        {
            bool backup = SWAT.WantInformativeExceptions.GetInformativeExceptions;
            bool passed = false;
            SWAT.WantInformativeExceptions.GetInformativeExceptions = true;
            SWAT.UserConfigHandler.Save();

            try
            {
                _browser.OpenBrowser();
                NavigateToSwatTestPage();
                _browser.OpenBrowser();
                NavigateToSwatTestPage();

                _browser.AssertTopWindow("swat test page", 1);
            }
            catch (TopWindowMismatchException e)
            {
                Assert.IsTrue(e.Message.Contains("SWAT Test Page") && e.Message.Contains("swat test page"), "Incorrect message from informative exceptions, actual message : " + e.Message);
                passed = true;
            }
            finally
            {
                SWAT.WantInformativeExceptions.GetInformativeExceptions = backup;
                SWAT.UserConfigHandler.Save();

                _browser.KillAllOpenBrowsers();
                _browser.OpenBrowser();
                NavigateToSwatTestPage();
            }

            Assert.IsTrue(passed);
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void AssertTopWindowWithIndexThrowsIndexOutOfRangeException()
        {
            _browser.AssertTopWindow("i dont care about the title", -1);
        }

        #endregion

    }
}
