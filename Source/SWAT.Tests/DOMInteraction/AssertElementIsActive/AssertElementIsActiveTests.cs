using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Microsoft.Win32;

namespace SWAT.Tests.AssertElementIsActive
{
    public abstract class AssertElementIsActiveTestFixture : BrowserTestFixture
    {
        public AssertElementIsActiveTestFixture(BrowserType browserType)
            :base(browserType)
        {

        }

        /*[TestFixtureSetUp]
        public override void Setup()
        {
            _browser = new WebBrowser(_browserType);
            iBrowserInstance = Reflector.GetField(_browser, "_browser") as IBrowser;
            
            _browser.OpenBrowser();
        }*/

        [SetUp]
        public override void TestSetup()
        {
            _browser.KillAllOpenBrowsers();
            _browser.OpenBrowser();
            this.NavigateToVisuallyImpairedTestPage();
            if (_browserType == BrowserType.InternetExplorer)
                TabThroughInternetExplorerComponents();
        }

        protected void NavigateToVisuallyImpairedTestPage()
        {
            _browser.NavigateBrowser(getTestPage("SightLessUserTestPage.htm"));
        }

        [Test]
        public void AssertElementIsActiveCorrectlyMatchesActiveElementsTest()
        {
            // correctly find select box
            _browser.StimulateElement(IdentifierType.Id, "focusableSelectBox", "onfocus");
            //_browser.AssertElementIsActive(IdentifierType.Id, "focusableSelectBox");
            _browser.AssertElementIsActive(IdentifierType.Id, "focusableSelectBox", "select");

            // correctly find textbox in nested frame
            _browser.StimulateElement(IdentifierType.Id, "txtBox1", "onfocus");
            _browser.AssertElementIsActive(IdentifierType.Id, "txtBox1", "input");
            _browser.AssertElementIsActive(IdentifierType.Expression, "id=txtBox1;type=text;tabindex=1", "input");

            // correctly find button
            _browser.StimulateElement(IdentifierType.Id, "btnClear", "onfocus");
            _browser.AssertElementIsActive(IdentifierType.Id, "btnClear", "input");
            _browser.AssertElementIsActive(IdentifierType.Expression, "id=btnClear;value=Clear Value", "input");

            // correctly find textbox
            _browser.StimulateElement(IdentifierType.Id, "txtOne", "onfocus");
            _browser.AssertElementIsActive(IdentifierType.Id, "txtOne", "input");
            _browser.AssertElementIsActive(IdentifierType.Expression, "id=txtOne;name=txtName", "input");

            // correctly find checkbox
            _browser.StimulateElement(IdentifierType.Id, "chkOne", "onfocus");
            //_browser.AssertElementIsActive(IdentifierType.Id, "chkOne");
            _browser.AssertElementIsActive(IdentifierType.Id, "chkOne", "input");
        }

        // In Chrome, Firefox, and Safari:
        // 2 tabs to btnClear
        // 3 tabs to txtOne
        // In Internet Explorer:
        // ([Same as above] + 6) for Internet Explorer's browser components

         
        [TestCase(3, "txtOne", "input")]
        [TestCase(2, "btnClear", "input")]
        public void AssertElementIsActiveWorksWhenFocusingOnElementUsingTabTest(int numTabs, string identifier, string tagName)
        {
            _browser.PressKeys(@"\{TAB\}", numTabs);
            _browser.AssertElementIsActive(IdentifierType.Id, identifier, tagName);
        }

        [Test]
        [ExpectedException(typeof(AssertionFailedException), UserMessage = "Element with Id 'txtOne' is not active.")]
        public void AssertActiveElementFailsWhenFocusIsOnAnotherElementTest()
        {
            _browser.StimulateElement(IdentifierType.Id, "txtBox1", "onfocus");
            _browser.AssertElementIsActive(IdentifierType.Id, "txtOne", "input");
        }

         
        [TestCase("fakeIdentifer", "badTagName")]
        [TestCase("chkOne", "input")]
        public void AssertElementIsActiveReturnsInformativeExceptionTest(string identifier, string tagName)
        {
            bool original = SWAT.WantInformativeExceptions.GetInformativeExceptions;
            string errorMessage = "";
            try
            {
                SWAT.WantInformativeExceptions.GetInformativeExceptions = true;
                SWAT.UserConfigHandler.Save();
                _browser.AssertElementIsActive(IdentifierType.Id, identifier, tagName);
            }
            catch (AssertionFailedException e)
            {
                errorMessage = e.Message;
            }
            finally
            {
                SWAT.WantInformativeExceptions.GetInformativeExceptions = original;
                SWAT.UserConfigHandler.Save();
            }

            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(errorMessage.Contains("tag"), string.Format("Informative exception returns incorrect message: {0}", errorMessage));
        }

        [Test]
        public void AssertElementIsActiveWorksWhenTabbingInAndOutOfSubframesTest()
        {
            int tabCount;
            int shiftTabCount;
            if (_browserType == BrowserType.FireFox || _browserType == BrowserType.InternetExplorer)
            {
                tabCount = 11;
                shiftTabCount = 2;
            }
            else
            {
                tabCount = 10;
                shiftTabCount = 1;
            }
            _browser.PressKeys(@"\{TAB\}", tabCount);
            _browser.AssertElementIsActive(IdentifierType.Id, "focusableTextBox", "input");
            _browser.PressKeys(@"\{SHIFT+TAB\}", shiftTabCount);
            _browser.AssertElementIsActive(IdentifierType.Expression, "href:http://www.google.com", "a");
        }

        [Test]
        public void AssertElementIsActiveWorksWithNestedFramesTabbingTest()
        {
            int tabCount;
            int shiftTabCount;
            if (_browserType == BrowserType.FireFox)
            {
                tabCount = 19;
                shiftTabCount = 2;
            }
            else if (_browserType == BrowserType.InternetExplorer)
            {
                tabCount = 17;
                shiftTabCount = 1;
            }
            else
            {
                tabCount = 16;
                shiftTabCount = 1;
            }
            _browser.PressKeys(@"\{TAB\}", tabCount);
            _browser.AssertElementIsActive(IdentifierType.Expression, "type=text;id=txtBox1;onkeyup:processOnKeyUp()", "input");
            _browser.PressKeys(@"\{SHIFT+TAB\}", shiftTabCount);
            _browser.AssertElementIsActive(IdentifierType.Expression, "type=button;id=focusableButton;value=Click Me", "input");
            if (_browserType == BrowserType.FireFox)
                shiftTabCount = 8;
            else if (_browserType == BrowserType.InternetExplorer)
                shiftTabCount = 7;
            else
                shiftTabCount = 6;
            _browser.PressKeys(@"\{SHIFT+TAB\}", shiftTabCount);
            _browser.AssertElementIsActive(IdentifierType.Expression, "href:http://www.google.com", "a");
        }

        [Test]
        public void AssertElementIsActiveUsesFindElementTimeoutTest()
        {
            bool threwAssertionFailedException = false;
            DateTime end = DateTime.Now.AddSeconds(DefaultTimeouts.FindElementTimeout);
            try
            {
                _browser.AssertElementIsActive(IdentifierType.Id, "im an identifier", "sometag");
            }
            catch (AssertionFailedException) { threwAssertionFailedException = true; }
            Assert.IsTrue(threwAssertionFailedException);
            Assert.IsTrue(DateTime.Now >= end);
        }

        [Test]
        public void AssertElementIsActiveObeysOptionalTimeoutTest()
        {
            bool threwAssertionFailedException = false;
            int expectedTimeout = 2;

            DateTime start = DateTime.Now;
            DateTime end = start.AddSeconds(2);

            try
            {
                _browser.AssertElementIsActive(IdentifierType.Id, "im an identifier", "sometag", expectedTimeout);
            }
            catch (AssertionFailedException) { threwAssertionFailedException = true; }

            Assert.IsTrue(threwAssertionFailedException);
            Assert.IsTrue(DateTime.Now >= end);
            Assert.IsTrue(DateTime.Now < start.AddSeconds(DefaultTimeouts.FindElementTimeout));
        }
    }
}
