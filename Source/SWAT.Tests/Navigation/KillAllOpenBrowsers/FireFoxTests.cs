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
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
 

namespace SWAT.Tests.KillAllOpenBrowsers
{
    [TestFixture, Timeout(100000)]
    [Category("FireFox")]
    public class FireFoxTests : KillAllOpenBrowsersTestFixture
    {
        public FireFoxTests()
            : base(BrowserType.FireFox)
        {

        }

        [Test]
        public void KillAllOpenedBrowsersJSSHDisabledTest()
        {
            try
            {
                // Close all JSSH enabled browsers first
                _browser.KillAllOpenBrowsers();

                // Open FireFox without the JSSH enabled
                string path = BrowserPaths.FirefoxRootDirectory;
                string browserName = GetBrowserName();

                if (string.IsNullOrEmpty(path)) //not found in the registry
                    throw new BrowserNotInstalledException("Firefox is not installed.");
                else if (!File.Exists(path))
                    throw new IllegalDirectoryException(string.Format("Firefox was not found in {0}", path));

                System.Diagnostics.Process fx = System.Diagnostics.Process.Start(path, "about:config");

                bool threwException = false;
                try
                {
                    // The browser should throw an exception at this point
                    _browser.KillAllOpenBrowsers();
                }
                catch (Exception)
                {
                    threwException = true;
                }

                Assert.IsFalse(threwException);

                // The window we just opened should have been closed by KillAllOpenBrowsers
                bool windowClosed = (Process.GetProcessesByName(browserName).Length == 0);

                Assert.IsTrue(windowClosed, "The JSSH disabled browser failed to be closed.");

            }
            finally
            {   // Clean up
                this.OpenSwatTestPage();
            }
        }
    }
}
