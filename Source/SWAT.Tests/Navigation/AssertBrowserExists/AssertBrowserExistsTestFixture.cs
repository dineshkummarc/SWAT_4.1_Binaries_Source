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

namespace SWAT.Tests.AssertBrowserExists
{
    public abstract class AssertBrowserExistsTestFixture : BrowserTestFixture
    {
        public AssertBrowserExistsTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region AssertBrowserExists

        [Test]
        public void AssertBrowserExistsPassesWhenWindowExistsTest()
        {
			_browser.AssertBrowserExists("SWAT Test Page");
        }

        [Test]
        public void AssertBrowserExistsFailsWhenWindowDoesntExistTest()
        {
            DefaultTimeouts.AssertBrowserExists = 10;

            bool timeOutCorrect = false;
            bool exceptionThrown = false;
            DateTime startTime = DateTime.Now;

            try
            {
                startTime = DateTime.Now;
                _browser.AssertBrowserExists("SWAT Negative");

            }
            catch (BrowserExistException)
            {
                exceptionThrown = true;
                if (DateTime.Now < startTime.AddSeconds(DefaultTimeouts.AssertBrowserExists + 2)) //we should get our exception within the timeout value
                    timeOutCorrect= true;
            }
            if (exceptionThrown)
                Assert.IsTrue(timeOutCorrect, "AssertBrowserExists threw the correct exception but not within the given timeout.");
            else
                Assert.Fail("AssertBrowserExists did not throw an exception.");
        }

        [Test]
        public void AssertBrowserExistsPassesWhenWindowExistsWithinTimeoutTest()
        {
			_browser.AssertBrowserExists("SWAT Test Page", 5000);
        }

        [Test]
        public void AssertBrowserExistsDoesNotHaveTimingIssueWhenLookingForPopupTest()
        {
            _browser.StimulateElement(IdentifierType.Id, "btnNewWindow", "onclick", "input");
            _browser.AssertBrowserExists("Second Window", 10000);
            _browser.KillAllOpenBrowsers("SWAT Test Page");
        }

        [Test]
        public void AssertBrowserExistsFailsWhenWindowDoesntExistAfterTimeoutTest()
        {
			int timeOut = 5;
            bool timeOutCorrect = false;
            bool exceptionThrown = false;
            DateTime startTime = DateTime.Now;

            try
            {
                startTime = DateTime.Now;
                _browser.AssertBrowserExists("SWAT Negative", timeOut*1000);

            }
            catch (BrowserExistException)
            {
                exceptionThrown = true;
                if (DateTime.Now < startTime.AddSeconds(timeOut + 2)) //we should get our exception within the timeout value
                    timeOutCorrect = true;
            }

            if (exceptionThrown)
            {
                Assert.IsTrue(timeOutCorrect, "AssertBrowserExists threw the correct exception but not within the given timeout.");
            }
            else
            {
                Assert.Fail("AssertBrowserExists failed to throw an exception");
            }
        }

        [Test]
        [ExpectedException(typeof(AssertionFailedException))]
        public void AssertBrowserExistsFailsTimeout()
        {
            _browser.AssertBrowserExists("title 7ha7 doesn't exist", 0);
        }

        #endregion
    }
}
