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
using System.Threading;
using SHDocVw;

namespace SWAT.Tests.AssertBrowserDoesNotExist
{
    [TestFixture, RequiresSTA]
    [Category("IE")]
    public class InternetExplorerTests : AssertBrowserDoesNotExistTestFixture 
    {
        public InternetExplorerTests()
            : base(BrowserType.InternetExplorer)
        {

        }

        [Test]
        public void AssertBrowserDoesNotExistPassesWhenBrowserIsClosedExternally()
        {
            try
            {
                Thread t = new Thread(new ThreadStart(AssertPageIsNotOpen));
                t.SetApartmentState(System.Threading.ApartmentState.STA);
                t.Start();

                Thread.CurrentThread.Join(3000);

                CloseInternetExplorer();

                while (!_done)
                    Thread.Sleep(200);

                bool result = _done && !_browserExistsException;

                TearDownAssertPageIsNotOpen();

                Assert.IsTrue(result);
            }
            finally
            {
                // Clean up  
                this.OpenSwatTestPage();
            }
        }

        private void CloseInternetExplorer()
        {
            ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
            foreach (SHDocVw.InternetExplorer ie in m_IEFoundBrowsers)
            {
                try { ie.Quit(); }
                catch { }
            }
        }

        private bool _done = false;
        private bool _browserExistsException = false;

        private void AssertPageIsNotOpen()
        {
            try
            {
                _browser.AssertBrowserDoesNotExist("SWAT Test Page");
            }
            catch (BrowserExistException)
            {
                _browserExistsException = true;
            }

            _done = true;
        }

        private void TearDownAssertPageIsNotOpen()
        {
            _done = false;
            _browserExistsException = false;
        }        
    }
}
