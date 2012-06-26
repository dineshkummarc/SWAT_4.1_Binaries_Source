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
using System.Threading;
using SHDocVw;

namespace SWAT.Tests.AssertBrowserDoesNotExist
{
    public abstract class AssertBrowserDoesNotExistTestFixture : BrowserTestFixture
    {
        public AssertBrowserDoesNotExistTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region AssertBrowserDoesNotExist

        [Test]
        public void AssertBrowserDoesNotExistPassesWhenWindowDoesntExistTest()
        {
			_browser.AssertBrowserDoesNotExist("title 7ha7 doesn't exist");
        }

        [Test]
        public void AssertBrowserDoesNotExistPassesWhenThereIsNoBrowserOpenTest()
        {
            _browser.KillAllOpenBrowsers();
            try
            {
                _browser.AssertBrowserDoesNotExist("title 7tha7 does not exist");
            }
            finally
            {
                // Clean up
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void AssertBrowserDoesNotExistPassesWhenWindowDoesntExistTestWithTimeout()
        {
			_browser.AssertBrowserDoesNotExist("title 7tha7 doesn't exist", 5);
        }

        [Test]
        public void AssertBrowserDoesNotExistFailsWhenWindowFoundTest()
        {                                 
            DefaultTimeouts.AssertBrowserExists = 10;

            bool timeOutCorrect = false;
            DateTime startTime = DateTime.Now;

            try
            {
                startTime = DateTime.Now;
                _browser.AssertBrowserDoesNotExist("SWAT Test Page");        
            }
            catch (BrowserExistException)
            {
                if (DateTime.Now < startTime.AddSeconds(DefaultTimeouts.AssertBrowserExists + 2)) //we should get our exception within the timeout value
                    timeOutCorrect = true;
            }

            Assert.IsTrue(timeOutCorrect, "AssertBrowserDoesNotExist threw the correct exception but not within the given timeout.");
        }

        [Test]
        public void AssertBrowserDoesNotExistFailsWhenWindowFoundTestWithTimeout()
        {
            int timeOut = 5;
            bool timeOutCorrect = false;
            DateTime startTime = DateTime.Now;

            try
            {
                startTime = DateTime.Now;
                _browser.AssertBrowserDoesNotExist("SWAT Test Page", timeOut);

            }
            catch (BrowserExistException)
            {
                if (DateTime.Now < startTime.AddSeconds(timeOut + 2)) //we should get our exception within the timeout value
                    timeOutCorrect = true;
            }

            Assert.IsTrue(timeOutCorrect, "AssertBrowserDoesNotExist threw the correct exception but not within the given timeout.");
        }

        [Test]
        [ExpectedException(typeof(AssertionFailedException))]
        public void AssertBrowserDoesNotExistFailsTimeout()
        {
            _browser.AssertBrowserDoesNotExist("title 7ha7 doesn't exist", 0);
        }

        #endregion
    }
}
