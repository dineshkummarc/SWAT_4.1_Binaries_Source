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
using Microsoft.Win32;

namespace SWAT.Tests.SetWindowPosition
{
    public abstract class SetWindowPositionTestFixture : BrowserTestFixture
    {
        public SetWindowPositionTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        #region SetWindowPosition Fails 

         
        [TestCase(WindowPositionTypes.BRINGTOTOP)]
        [TestCase(WindowPositionTypes.MAXIMIZE)]
        [TestCase(WindowPositionTypes.MINIMIZE)]
        [ExpectedException(typeof(NoAttachedWindowException))]
        public void SetWindowPositonFailsWithNoWindowTest(WindowPositionTypes positionType)
        {
            _browser.KillAllOpenBrowsers();
            try
            {
                _browser.SetWindowPosition(positionType);
            }
            finally
            {
                // Clean up
                this.OpenSwatTestPage();
            }
        }


        #endregion

		#region SetWindowPosition BringToTop

        [Test]
        public void SetWindowPositonBringToTopTest()
        {
            //visual test to see if BRINGTOTOP brings the window to the top. 
            _browser.SetWindowPosition(WindowPositionTypes.BRINGTOTOP);
            _browser.Sleep(1000);
        }

		#endregion

        #region SetWindowPosition Maximize


        #endregion

        #region SetWindowPosition Minimize


        #endregion

        [Test]
        public void TestSetWindowPositonMinimizeBringToTopMaximize()
        {
            _browser.SetWindowPosition(WindowPositionTypes.MINIMIZE);
            _browser.SetWindowPosition(WindowPositionTypes.BRINGTOTOP);
            _browser.SetWindowPosition(WindowPositionTypes.MAXIMIZE);
        }
    }
}
