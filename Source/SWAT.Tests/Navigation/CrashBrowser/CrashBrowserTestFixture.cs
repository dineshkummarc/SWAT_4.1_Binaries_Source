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
using System.Diagnostics;
using SWAT.Reflection;

namespace SWAT.Tests.CrashBrowser
{
    public abstract class CrashBrowserTestFixture : BrowserTestFixture
    {
        public CrashBrowserTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        [TearDown]
        public override void TestTeardown()
        {
            _browser.KillAllOpenBrowsers();
            this.OpenSwatTestPage();
        }

        #region CrashBrowser

        [Test]
        [ExpectedException(typeof(NavigationTimeoutException))]
        public void NavigateBrowserTimeoutTest()
        {
            int timeout = 30;
            ReflectionHelper.SetField(_browser, "_killProcess", true);

            ReflectionHelper.InvokeMethod(_browser, "NavigateBrowser", getTestPage("PageThatHangs.htm"), timeout);

            ReflectionHelper.SetField(_browser, "_killProcess", false);
        }

        [Test]
        [ExpectedException]
        public void NavigateBrowserThrowsErrorWhenTimeoutLessThanThirtySecondsTest()
        {
            int timeout = 29;
            _browser.NavigateBrowser(getTestPage("PageThatHangs.htm"), timeout);
        }

        [Test]
        public void NavigateBrowserCausesTimeoutWhenInternetExplorerNotRespondingTest()
        {
            bool browserKilled = false;
            try
            {
                _browser.SetWindowPosition(WindowPositionTypes.MAXIMIZE);
                _browser.NavigateBrowser(getTestPage("IECrasherPage.htm"), 45);
            }
            catch (NavigationTimeoutException)
            {
                browserKilled = true;
                try
                {
                    _browser.AssertBrowserDoesNotExist("IE Crasher");
                }
                catch (BrowserExistException)
                {
                    browserKilled = false;
                }
            }

            Assert.IsTrue(browserKilled, "Document load timeout failed to close IE process when it stopped responding.");
        }

        [Test]
        [ExpectedException(typeof(NavigationTimeoutException))]
        public void NavigateBrowserTimesOutInFiveMinutesByDefaultTest()
        {
            ReflectionHelper.SetField(_browser, "_killProcess", true);

            ReflectionHelper.InvokeMethod(_browser, "NavigateBrowser", getTestPage("PageThatHangs.htm"));

            ReflectionHelper.SetField(_browser, "_killProcess", false);
        }

        #endregion
    }
}
