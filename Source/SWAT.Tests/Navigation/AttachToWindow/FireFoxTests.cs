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
using System.Net.Sockets;
using System.IO;
using SWAT.Reflection;

namespace SWAT.Tests.AttachToWindow
{
    [TestFixture]
    [Category("FireFox")]
    public class FireFoxTests : AttachToWindowTestFixture
    {
        public FireFoxTests()
            : base(BrowserType.FireFox)
        {

        }

        [Test]
        [ExpectedException(typeof(FireFoxClientIsNotConnectedException), UserMessage = "Unable to connect to JSSH. Please verify Firefox was opened with JSSH enabled.")]
        public void ClientNotConnectedFailsTest()
        {
            IBrowser ff = ReflectionHelper.GetField<IBrowser>(_browser, "_browser");
            string ffVersion = ReflectionHelper.GetField<string>(ff, "_firefoxVersion");
            if (ffVersion[0] != '3')
                Assert.Ignore("Test does not apply to Firefox 4.");
            // Close any open FireFox browsers that are open so we close the JSSH 
            _browser.KillAllOpenBrowsers();

            // Start up FireFox without the JSSH client
            string path = BrowserPaths.FirefoxRootDirectory;

            if (string.IsNullOrEmpty(path)) //not found in the registry
                throw new BrowserNotInstalledException("Firefox is not installed.");
            else if (!File.Exists(path))
                throw new IllegalDirectoryException(string.Format("Firefox was not found in {0}", path));

            System.Diagnostics.Process fx = System.Diagnostics.Process.Start(path, getTestPage("TestPage.htm"));

            try
            {
                _browser.AttachToWindow("SWAT Test Page");
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }
        //Test no longer applies
        /*
        [Test]
        public void AttachToWindowThrowsWindowHandleNotFoundExceptionInFirefoxWhenWindowHandleIsntFoundTest()
        {
            bool threwException = false;
            _browser.NavigateBrowser(getTestPage("PageSpecialCharacters.htm"));
            try
            {
                _browser.AttachToWindow("~`!@#$%^&*()_-+={[}]|:;'<,>.?/\"‡Î…˘Ÿ‚œ˚‚œ˚ €ÁÓ¿À´ÈÔ¬Œªú«‘ÍÙ»å\\");
            }
            catch (WindowHandleNotFoundException)
            {
                threwException = true;
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }

            Assert.IsTrue(threwException);        
        }
        */
        private Socket GetClientConnection()
        {
            return ReflectionHelper.GetProperty<Socket>(_browser, "_client");
        }
    }
}
