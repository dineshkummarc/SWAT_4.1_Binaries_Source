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
using SWAT.Tests;
using NUnit.Framework;
using SWAT.Reflection;

namespace SWAT.Tests.PressKeys.VisuallyImpairedUsers
{
    public abstract class VisuallyImpairedUserTestFixture : BrowserTestFixture
    {
        public VisuallyImpairedUserTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        [TestFixtureSetUp]
        public override void Setup()
        {
            _browser = new WebBrowser(_browserType);
            iBrowserInstance = ReflectionHelper.GetField<IBrowser>(_browser, "_browser");
            _browser.OpenBrowser();

            _browser.NavigateBrowser(getTestPage("SightLessUserTestPage.htm"));
            
        }

        [SetUp]
        public override void TestSetup()
        {
            NavigateToVisuallyImpairedTestPage();
        }

        protected void NavigateToVisuallyImpairedTestPage()
        {
            _browser.NavigateBrowser(getTestPage("SightLessUserTestPage.htm"));
        }

        /// <summary>
        /// Used for modeling the use of a web browser by the visually impaired.
        /// </summary>
        #region Sightless User Tests

        [Test]
        public void PressKeysSightlessUserSetValueTest()
        {
            TabThroughInternetExplorerComponents();

            _browser.PressKeys("\\{TAB\\}");
            _browser.PressKeys("\\{ENTER\\}");
            _browser.AssertElementExists(IdentifierType.Expression, "id=txtOne;value=Test1", "input");

            _browser.PressKeys("\\{TAB\\}");
            _browser.PressKeys("\\{ENTER\\}");
            _browser.AssertElementExists(IdentifierType.Expression, "id=txtOne;value=", "input");
 
        }

        [Test]
        public void PressKeysSightlessUserCheckBoxTest()
        {
            TabThroughInternetExplorerComponents();

            _browser.PressKeys("\\{TAB\\}", 4);
            _browser.PressKeys("\\{SPACEBAR\\}");
            _browser.AssertElementExists(IdentifierType.Expression, "id=chkOne;checked=true", "input");
        }

        [Test]
        public void PressKeysSightlessUserNewWindowTest()
        {
            try
            {
                TabThroughInternetExplorerComponents();

                _browser.PressKeys("\\{TAB\\}", 5);
                _browser.PressKeys("\\{ENTER\\}");
                _browser.AssertBrowserExists("Second Window");
                _browser.AttachToWindow("Second Window");

                if (_browserType == BrowserType.InternetExplorer)
                    _browser.PressKeys("\\{TAB\\}", 6);

                _browser.PressKeys("\\{TAB\\}", 2);
                _browser.PressKeys("\\{ENTER\\}");
                _browser.AssertBrowserDoesNotExist("Second Window");
            }
            finally
            {
                _browser.AttachToWindow("sightless user");
            }
        }

        [Test]
        public void PressKeysSightlessUserDropDownMenuTest()
        {
            TabThroughInternetExplorerComponents();

            _browser.PressKeys("\\{TAB\\}", 7);
            _browser.PressKeys("o");

            if (_browserType != BrowserType.InternetExplorer)
                _browser.PressKeys("\\{ENTER\\}");

            _browser.AssertElementExists(IdentifierType.Expression, "id=optionLabel;innerhtml=value2", "label");
            _browser.PressKeys("o");

            if (_browserType != BrowserType.InternetExplorer)
                _browser.PressKeys("\\{ENTER\\}");

            _browser.PressKeys("\\{ENTER\\}");
            _browser.AssertElementExists(IdentifierType.Expression, "id=optionLabel;innerhtml=value1", "label");
        }

        [Test]
        public void PressKeysSightlessUserTextFieldTest()
        {
            TabThroughInternetExplorerComponents();

            _browser.PressKeys("\\{TAB\\}", 8);
            _browser.PressKeys("Test");
            _browser.AssertElementExists(IdentifierType.Expression, "id=txtAreaOne;value=Test", "textarea");
            _browser.PressKeys("T", 3);
            _browser.AssertElementExists(IdentifierType.Expression, "id=txtAreaOne;value=TestTTT", "textarea");
            _browser.PressKeys("e", 3);
            _browser.AssertElementExists(IdentifierType.Expression, "id=txtAreaOne;value=TestTTTeee", "textarea");
            _browser.PressKeys("s", 3);
            _browser.AssertElementExists(IdentifierType.Expression, "id=txtAreaOne;value=TestTTTeeesss", "textarea");
            _browser.PressKeys("t", 3);
            _browser.AssertElementExists(IdentifierType.Expression, "id=txtAreaOne;value=TestTTTeeesssttt", "textarea");
        }

        [Test]
        public void PressKeysSightlessUserClickLinkTest()
        {
            TabThroughInternetExplorerComponents();

            _browser.PressKeys("\\{TAB\\}", 9);
            _browser.PressKeys("\\{ENTER\\}");
            _browser.AssertBrowserExists("Google");
        }

        [Test]
        public void PressKeysShiftTabWorksCorrectlyTest()
        {
            TabThroughInternetExplorerComponents();

            string message = "shift tab works";

            _browser.PressKeys("\\{TAB\\}", 3);
            _browser.PressKeys(message);
            _browser.AssertElementExists(IdentifierType.Expression, string.Format("id=txtOne;value={0}", message), "input");
            _browser.PressKeys("\\{SHIFT+TAB\\}");
            _browser.PressKeys("\\{ENTER\\}");
            Assert.AreEqual("", _browser.GetElementAttribute(IdentifierType.Id, "txtOne", "value", "input"));
        }

        [Test] // this test should be removed/modified if support for SHIFT modifiers are suported with keys other than TAB in the future
        [ExpectedException(typeof(ArgumentException), UserMessage = "There is no key with name SHIFT+ENTER in the configuration table.")]
        public void PressKeysShiftEnterThrowsArgumentExceptionTest()
        {
            TabThroughInternetExplorerComponents();
            _browser.PressKeys("\\{TAB\\}");
            _browser.PressKeys("\\{SHIFT+ENTER\\}");
        }

        [Test]
        [ExpectedException(typeof(LockedDesktopEnvironmentException), UserMessage = "The key code sequence cannot be processed in a locked desktop environment.")]
        public void PressKeysSightlessUserKeyPressInLockedDesktopTest()
        {
            //if (_browserType == BrowserType.Chrome)
            //    Assert.Ignore("Broken in Chrome until http://code.google.com/p/chromium/issues/detail?id=55391 is resolved.");

            if (_browserType == BrowserType.Safari)
                Assert.Ignore("Does not apply to Safari.");

            try
            {
                SetForceBrowserPressKeys(true);
                _browser.PressKeys("This should throw an exception");
            }
            finally
            {
                // Clean up
                SetForceBrowserPressKeys(false);
            }
        }

        #endregion
    }
}
