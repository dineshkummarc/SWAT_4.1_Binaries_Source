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

namespace SWAT.Tests.RunScript
{
    [TestFixture]
    [Category("Safari")]
    public class SafariTests : RunScriptTests
    {
        public SafariTests()
            : base(BrowserType.Safari)
        {

        }

        [Test]
        public void RunAppleScriptTest()
        {
            try
            {
                string appleScript = "tell application \"Safari\"\ropen location \"www.google.com\"\rtell application \"System Events\" to tell process \"Safari\" to set value of attribute \"AXMain\" of window 1 to true\rdelay 3\rreturn name of document 1\rdelay 5\rend tell";
                _browser.RunScript("applescript", appleScript, "Google");
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                OpenSwatTestPage();
            }
        }

        [Test]
        public void RunAppleScriptSaveResultTest()
        {
            try
            {
                string appleScript = "tell application \"Safari\"\ropen location \"www.google.com\"\rtell application \"System Events\" to tell process \"Safari\" to set value of attribute \"AXMain\" of window 1 to true\rdelay 3\rreturn name of document 1\rend tell";
                string result = _browser.RunScriptSaveResult("applescript", appleScript);
                Assert.AreEqual(result, "Google");
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                OpenSwatTestPage();
            }
        }

        [Test]
        public void RunAppleScriptLoadFromFileTest()
        {
            try
            {
                string path = getTestFilePath("appleScriptTest.txt");
                string fileArg = string.Format("/file:{0}", path);
                string result;

                result = _browser.RunScriptSaveResult("AppleScript", fileArg);
                Assert.AreEqual("Google", result);
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                OpenSwatTestPage();
            }
        }

        [Test]
        public void RunAppleScriptSaveResultFailsTest()
        {
            bool threwException = false;
            try
            {
                string appleScript = "tell application \"Safari\"\ropen location \"www.google.com\"\rtell application \"System Events\" to tell process \"Safari\" to set value of attribute \"AXMain\" of window 1 to true\rdelay 3\rend tell";
                string result = _browser.RunScriptSaveResult("applescript", appleScript);
            }
            catch (ArgumentException e)
            {
                threwException = true;
                Assert.IsTrue(e.Message.Contains("Applescript yielded no results"));
            }
            finally
            {
                _browser.KillAllOpenBrowsers();
                OpenSwatTestPage();
            }
            Assert.IsTrue(threwException, "RunScriptSaveResult failed to throw exception with incorrect AppleScript");
        }

        [Test]
        public void RunScriptApplescriptReturnsErrorMessagesTest()
        {
            bool exceptionThrown = false;
            string failScript = "tell application not in quotes to close window 1";
            try
            {
                _browser.RunScript("applescript", failScript, "i dont expect this to work");
            }
            catch (AssertionFailedException e)
            {
                exceptionThrown = true;
                Assert.IsTrue(e.Message.ToLower().Contains("expected expression"), "RunScript is not returning Applescript errors");
            }
            Assert.IsTrue(exceptionThrown, "RunScript did not throw the correct exception");
        }
    }
}
