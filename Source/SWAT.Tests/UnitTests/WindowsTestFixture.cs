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
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using SWAT.Windows;

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("Misc")]
    public class WindowsTestFixture
    {
        private WindowsEnumerator enumerator;
        private Process p;

        [TestFixtureSetUp]
        public void SetUpWindowsTestFixture()
        {
            enumerator = new WindowsEnumerator();

            p = new Process();
            p.StartInfo.FileName = "iexplore";
            p.Start();

            DateTime timeout = DateTime.Now.AddSeconds(10);
            while (DateTime.Now < timeout && Process.GetProcessesByName("iexplore").Length == 0)
                Thread.Sleep(1000);

            Thread.Sleep(2000);
        }

        [TestFixtureTearDown]
        public void TearDownWindowsTestFixture()
        {
            ProcessKiller.Kill("iexplore");
        }

        [Test]
        public void GetTopLevelWindowsTest()
        {
            List<ApiWindow> windows = enumerator.GetTopLevelWindows();
            Assert.IsTrue(windows.Count > 0);

            List<ApiWindow> ieWindows = enumerator.GetTopLevelWindows("IEFrame");
            Assert.IsTrue(ieWindows.Count > 0);
        }

        [Test]
        public void GetChildLevelWindowsTest()
        {
            List<ApiWindow> ieWindows = enumerator.GetTopLevelWindows("IEFrame");
            Assert.IsTrue(ieWindows.Count > 0);
            int hWnd = ieWindows[0].hWnd;

            List<ApiWindow> childWindows = enumerator.GetChildWindows(hWnd);
            Assert.IsTrue(childWindows.Count > 0);

            List<ApiWindow> childWindowsWithClass = enumerator.GetChildWindows(hWnd, "Internet Explorer_Server");
            Assert.IsTrue(childWindows.Count > 0);
        }
    }
}
