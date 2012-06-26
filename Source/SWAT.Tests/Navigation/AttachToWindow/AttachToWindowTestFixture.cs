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
using NUnit.Framework;
using Microsoft.Win32;

namespace SWAT.Tests.AttachToWindow
{
    public abstract class AttachToWindowTestFixture : BrowserTestFixture
    {
        public AttachToWindowTestFixture(BrowserType browserType)
            : base(browserType)
        {

        }

        int defaultTimeout;

        #region Set Up and Tear Down

        [TestFixtureSetUp]
        public override void Setup()
        {
            if (SafariSettings.SafariAddress != "120.0.0.1")
            {
                SafariSettings.SafariAddress = "120.0.0.1";
                UserConfigHandler.Save();
            }

            _browser = new WebBrowser(_browserType);
            this.OpenSwatTestPage();
            defaultTimeout = SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout;
            SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout = 5;
        }

        [TestFixtureTearDown]
        public override void TearDown()
        {
            _browser.KillAllOpenBrowsers();
            SWAT.DefaultTimeouts.AttachToWindowBrowserTimeout = defaultTimeout;
            SWAT.UserConfigHandler.Save();
        }

        #endregion

        #region AttachToWindow

        [Test]
        public void AttachToWindowTest()
        {
            try
            {
                //Second Window
                _browser.StimulateElement(IdentifierType.Id, "btnNewWindow", "onclick");
                _browser.StimulateElement(IdentifierType.Id, "btnNewWindow", "onclick");

                _browser.AttachToWindow("Second Window", 1);
                _browser.NavigateBrowser("http://www.google.com");
                _browser.AssertElementExists(IdentifierType.Expression, "name:q", "INPUT");
                _browser.CloseBrowser(); //let's close the browser windows
                _browser.AttachToWindow("Second Window");
                _browser.CloseBrowser(); //let's close the browser windows
                _browser.AttachToWindow("SWAT Test Page");

                // The following is not supported on Chrome or FireFox
                if (_browserType == BrowserType.InternetExplorer || _browserType == BrowserType.Safari)
                {
                    _browser.StimulateElement(IdentifierType.Id, "modalButton", "onclick");

                    _browser.AttachToWindow("Modal");
                    _browser.SetElementAttribute(IdentifierType.Id, "Text1", Attributes.VALUE, "Attach Successful");
                    _browser.StimulateElement(IdentifierType.Id, "Button1", "onclick");

                    _browser.AttachToWindow("SWAT Test Page");
                    _browser.StimulateElement(IdentifierType.Id, "longModalTitle", "onclick");

                    _browser.AttachToWindow("Long Modal Dialog");
                    _browser.SetElementAttribute(IdentifierType.Id, "Text1", Attributes.VALUE, "Attach Successful");
                    _browser.StimulateElement(IdentifierType.Id, "Button1", "onclick");

                    _browser.AttachToWindow("SWAT Test Page");
                }
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void AttachToWindowUsingEmptyStringTest()
        {
            try
            {
                _browser.KillAllOpenBrowsers();
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.google.com");
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.apple.com");
                _browser.CloseBrowser();
                _browser.AttachToWindow("");

                string windowTitle = _browser.GetWindowTitle();

                Assert.AreEqual(windowTitle, "Google");
            }
            finally
            {
                // Clean up
                this.OpenSwatTestPage();
            }
        }

        [Test]
        [ExpectedException(typeof(WindowNotFoundException))]
        public void AttachToWindowFailsWhenNoBrowserIsOpenTest()
        {
            try
            {
                _browser.KillAllOpenBrowsers();
                _browser.AttachToWindow("SWAT Test Page");
            }
            finally
            {
                // Clean up
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void AttachToWindowDoesNotHaveTimingIssueWhenLookingForPopupTest()
        {
            _browser.StimulateElement(IdentifierType.Id, "btnNewWindow", "onclick", "input");
            try
            {
                _browser.AttachToWindow("Second Window");
            }
            finally
            {
                // Clean up
                _browser.CloseBrowser();
                this.AttachToSwatTestPage();
            }
        }
        
        [Test]
        public void AttachToWindowCaseInsensitiveTest()
        {
			_browser.AttachToWindow("sWaT TeST pagE");
        }

        [Test]
        public void AttachToWindowSpecialCharactersTest()
        {
            _browser.NavigateBrowser(getTestPage("PageSpecialCharacters.htm"));
            try
            {
                _browser.AttachToWindow("~`!@#$%^&*()_-+={[}]|:;'<,>.?/\"àëÉùÙâÏûâÏûÊÛçîÀË«éïÂÎ»œÇÔêôÈŒ\\");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void OpenBrowserCloseBrowserAttachToWindowTest()
        {
            try
            {
                _browser.NavigateBrowser("www.google.com");

                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.apple.com");

                _browser.OpenBrowser();
                this.NavigateToSwatTestPage();

                _browser.AttachToWindow("Google");
                _browser.CloseBrowser();
                _browser.AttachToWindow("Apple");
                _browser.CloseBrowser();
            }
            finally
            {
                this.AttachToSwatTestPage();
            }
        }

        [Test]
        public void AttachToWindowGetsWindowHandleWithSpanishCharactersTest()
        {
            try
            {
                _browser.NavigateBrowser(getTestPage("SpanishCharactersPage.html"));
                _browser.AttachToWindow("Configur");
                _browser.SetWindowPosition(WindowPositionTypes.MAXIMIZE);
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }
            
    #endregion

        #region AttachToWindow with index

        [Test]
        public void AttachToWindowWithIndexOneWindowTitleTest()
        {
            string test0 = "This Test page is index 0";
            string test1 = "This Test page is index 1";
            string test2 = "This Test page is index 2";

            try
            {
                _browser.KillAllOpenBrowsers();

                // Set up three SWAT Test Page browsers w/ unique element values for txtOne     
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.SetElementAttribute(IdentifierType.Id, "txtOne", "value", test0);
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.SetElementAttribute(IdentifierType.Id, "txtOne", "value", test1);
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.SetElementAttribute(IdentifierType.Id, "txtOne", "value", test2);

                // Attach to each specific browser and assert its unique txtOne value
                _browser.AttachToWindow("SWAT Test Page", 0);
                _browser.AssertElementExists(IdentifierType.Expression, "id=txtOne;value=" + test0);
                _browser.AttachToWindow("SWAT Test Page", 1);
                _browser.AssertElementExists(IdentifierType.Expression, "id=txtOne;value=" + test1);
                _browser.AttachToWindow("SWAT Test Page", 2);
                _browser.AssertElementExists(IdentifierType.Expression, "id=txtOne;value=" + test2);
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void AttachToWindowWithIndexMultipleWindowTitlesTest()
        {
            _browser.KillAllOpenBrowsers();

            //Attaching back to instance (with 2 repetitions of 2 types of windows)
            //Opening in order
            _browser.OpenBrowser();
            try
            {
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.SetElementAttribute(IdentifierType.Id, "btnSetVal", "value", "Testing AttachToWindow With Index");
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestOpenWindowPage.htm"));
                _browser.SetElementAttribute(IdentifierType.Id, "myBtn", "value", "Testing AttachToWindow With Index");
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestOpenWindowPage.htm"));
                _browser.AttachToWindow("SWAT Test Page", 1);
                _browser.AssertElementExists(IdentifierType.Expression, "id=btnSetVal;value=Testing AttachToWindow With Index");
                _browser.AttachToWindow("Second Window", 0);
                _browser.AssertElementExists(IdentifierType.Expression, "id=myBtn;value=Testing AttachToWindow With Index");

                //Opening in mixed order
                _browser.KillAllOpenBrowsers();
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestOpenWindowPage.htm"));
                _browser.SetElementAttribute(IdentifierType.Id, "myBtn", "value", "Testing AttachToWindow With Index");
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
                _browser.SetElementAttribute(IdentifierType.Id, "btnSetVal", "value", "Testing AttachToWindow With Index");
                _browser.OpenBrowser();
                _browser.NavigateBrowser(getTestPage("TestOpenWindowPage.htm"));
                _browser.AttachToWindow("Second Window", 0);
                _browser.AssertElementExists(IdentifierType.Expression, "id=myBtn;value=Testing AttachToWindow With Index");
                _browser.CloseBrowser();
                _browser.AttachToWindow("SWAT Test Page", 1);
                _browser.AssertElementExists(IdentifierType.Expression, "id=btnSetVal;value=Testing AttachToWindow With Index");
                _browser.CloseBrowser();
            }
            finally
            {
                // Clean up
                this.AttachToSwatTestPage();
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void AttachToWindowMultipleWindowsIndexOutOfRangeTest()
        {
            _browser.KillAllOpenBrowsers();

            _browser.OpenBrowser();
            _browser.OpenBrowser();
            _browser.OpenBrowser();
            try 
            { 
                string windowTitle = "";
                switch (_browserType)
                {
                    case BrowserType.InternetExplorer: windowTitle = "Blank";
                        break;
                    case BrowserType.FireFox: windowTitle = "about:config";
                        break;
                    case BrowserType.Safari: 
                    case BrowserType.Chrome:
                        windowTitle = "about:swat";
                        break;
                    default:
                        Assert.Fail("Browser type not implemented in this test");
                        break;
                }
           
                _browser.AttachToWindow(windowTitle, 3); 
            }
            finally
            {
                // Clean up
                _browser.KillAllOpenBrowsers();
                this.OpenSwatTestPage();
            }
        }
        
         
        [TestCase("non existent window title")]
        [TestCase("SWAT Test Page with extra characters")]
        [ExpectedException(typeof(WindowNotFoundException))]
        public void AttachToWindowThrowsWindowNotFoundExceptionTest(string invalidWindowTitle)
        {
            _browser.AttachToWindow(invalidWindowTitle);
        }
                              
         
        [TestCase(1)] // Upper boundary
        [TestCase(-1)] // Lower boundary
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void AttachToWindowWithIndexFailureTest(int invalidIndex)
        {
            _browser.KillAllOpenBrowsers();
            this.OpenSwatTestPage();
            _browser.AttachToWindow("SWAT Test Page", invalidIndex);
        }

        #endregion                     

        #region AttachToWindow PDF

        [Test]
        public void AttachToPDFBrowserWindowTest()
        {
			_browser.StimulateElement(IdentifierType.Expression, "innerHtml:Form W-4", "onclick", "a");
            try
            {
                _browser.AttachToWindow("fw4.pdf");
            }
            finally
            {
                // Clean up
                this.NavigateToSwatTestPage();
            }
        }

        [Test]
        public void AttachToPDFBrowserWindowWithHiddenExtentionsTest()
        {
            if (_browserType == BrowserType.Safari) // windows only test
            {
                Assert.Ignore("Test is irrelevant for Safari.");
            }

            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced");
            int hideFileExt = (int)key.GetValue("HideFileExt");
            key.SetValue("HideFileExt", 1);

            _browser.StimulateElement(IdentifierType.Expression, "innerHtml:Form W-4", "onclick", "a");
            try
            {
                _browser.AttachToWindow("fw4.pdf");
            }
            finally
            {
                // Clean up
                key.SetValue("HideFileExt", hideFileExt);
                this.NavigateToSwatTestPage();
            }
        }
        
        #endregion

        #region NoAttachedWindowException
        [Test]
        [ExpectedException(typeof(NoAttachedWindowException))]        
        public void NavigateBrowserNoAttachWindowExceptionTest()
        {
            try
            {
                _browser.KillAllOpenBrowsers();
                _browser.NavigateBrowser("google.com");
            }
            finally
            {
                // Clean up
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void NoAttachedWindowAfterKillAllOpenBrowsersTest()
        {
            _browser.KillAllOpenBrowsers();

            try
            {
                noAttachedWindowTestHelper();
            }
            finally
            {
                // Clean up
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void NoAttachedWindowAfterCloseBrowserTest()
        {
            _browser.CloseBrowser();

            try
            {
                noAttachedWindowTestHelper();
            }
            finally
            {
                // Clean up
                this.OpenSwatTestPage();
            }
        }

        [Test]
        public void NoAttachedWindowAfterKillAllOpenBrowsersWithWindowTitleTest()
        {
            try
            {
                _browser.OpenBrowser();
                _browser.NavigateBrowser("www.google.com");

                _browser.KillAllOpenBrowsers("SWAT Test Page");

                // Current browser should no longer be attached even though another browser is open

                noAttachedWindowTestHelper();
            }
            finally
            {
                // Clean up
                this.AttachToSwatTestPage();
            }
        }

        // There is currently no support for window.close() because of issues with it across various browsers.
        // FireFox does not allow window.close() by default and window.close() will not trigger Internet Explorer's
        //    waitForBrowserToClose() function.
        [Test]
        public void NoAttachedWindowAfterBrowserClosedThroughEventTest()
        {
            _browser.StimulateElement(IdentifierType.Id, "btnNewWindowClose", "onclick");
            _browser.AttachToWindow("Close Window Page");
            _browser.StimulateElement(IdentifierType.Id, "btnCloseBrowser", "onclick");

            noAttachedWindowTestHelper();

            // Clean up
            this.AttachToSwatTestPage();
        }

        private void noAttachedWindowTestHelper()
        {
            bool exceptionThrown = false;
            try { _browser.CloseBrowser(); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }
            Assert.IsTrue(exceptionThrown, "CloseBrowser failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.RefreshBrowser(); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }
            Assert.IsTrue(exceptionThrown, "RefreshBrowser failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.NavigateBrowser(""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; } 
            Assert.IsTrue(exceptionThrown, "NavigateBrowser failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.RunScript("", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }
            Assert.IsTrue(exceptionThrown, "RunScript failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.RunScript("javascript", "", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }
            Assert.IsTrue(exceptionThrown, "RunScript failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.RunScript("javascript", "", "", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }
            Assert.IsTrue(exceptionThrown, "RunScript failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.RunScriptSaveResult(""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }
            Assert.IsTrue(exceptionThrown, "RunScriptSaveResult failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.RunScriptSaveResult("javascript", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; } 
            Assert.IsTrue(exceptionThrown, "RunScriptSaveResult failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.RunScriptSaveResult("javascript", "", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; } 
            Assert.IsTrue(exceptionThrown, "RunScriptSaveResult failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.SetElementAttribute(IdentifierType.Id, "", "", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }  
            Assert.IsTrue(exceptionThrown, "SetElementAttribute failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.PressKeys(IdentifierType.Id, "", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }     
            Assert.IsTrue(exceptionThrown, "PressKeys failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.StimulateElement(IdentifierType.Id, "", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }  
            Assert.IsTrue(exceptionThrown, "StimulateElement failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.GetElementAttribute(IdentifierType.Id, "", "", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }    
            Assert.IsTrue(exceptionThrown, "GetElementAttribute failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.GetLocation(); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }    
            Assert.IsTrue(exceptionThrown, "GetLocation failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.GetWindowTitle(); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }    
            Assert.IsTrue(exceptionThrown, "SetElementAttribute failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.AssertElementDoesNotExist(IdentifierType.Id, "", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }         
            Assert.IsTrue(exceptionThrown, "AssertElementDoesNotExist failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.AssertElementExists(IdentifierType.Id, "", ""); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }   
            Assert.IsTrue(exceptionThrown, "AssertElementExists failed to throw a NoAttachedWindowException.");
            exceptionThrown = false;

            try { _browser.SetWindowPosition(WindowPositionTypes.MINIMIZE); }
            catch (NoAttachedWindowException) { exceptionThrown = true; }      
            Assert.IsTrue(exceptionThrown, "AssertElementExists failed to throw a NoAttachedWindowException.");
        }
        #endregion
    }
}
