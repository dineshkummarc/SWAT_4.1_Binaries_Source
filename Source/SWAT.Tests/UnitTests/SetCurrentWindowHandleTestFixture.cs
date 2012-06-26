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
using System.Linq;
using NUnit.Framework;
using System.Text;
using SWAT;
using System.Diagnostics;

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("IE")]
    public class SetCurrentWindowHandleTestFixture
    {
        Browser _browser;

        [Test]
        public void SetCurrentWindowHandleExceptionTest()
        {
            _browser = new InternetExplorer();
            string fakeWindowTitle = "Window does not exist";

            Assert.AreEqual(_browser.GetCurrentWindowHandle(), IntPtr.Zero);
            try
            {
                _browser.SetCurrentWindowHandle(fakeWindowTitle);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, string.Format("FATAL ERROR: Unable to aquire window handle for window title {0}.", fakeWindowTitle));
            }
        }

        [Test]
        public void SetCurrentWindowHandleTest()
        {
            _browser = new InternetExplorer();
            Process proc = new Process();

            proc.StartInfo.FileName = "notepad.exe";
            proc.Start();

            proc.WaitForInputIdle(4000);

            IntPtr windowHandle = proc.MainWindowHandle;
            string windowName = proc.MainWindowTitle;

            if (windowHandle.ToInt32() == 0)
                Assert.Fail("Notepad process failed to start within allotted time.");

            _browser.SetCurrentWindowHandle(windowName);

            Assert.AreEqual(_browser.GetCurrentWindowHandle(), windowHandle);

            proc.Kill();
        }
    }
}
