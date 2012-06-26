using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;

namespace SWAT.Tests.AssertElementIsActive
{
    [TestFixture]
    [Category("Chrome")]
    public class ChromeTests : AssertElementIsActiveTestFixture
    {
        public ChromeTests() : base(BrowserType.Chrome) { }

        [Test]
        [ExpectedException(typeof (ChromeContentScriptIsNotConnectedException))]
        public void ChromeThrowsExceptionWhenPortDisconnectsAsTimeoutIsReachedTest()
        {
            _browser.NavigateBrowser(getTestPage("TestPage.htm"));
            _browser.PressKeys(@"\{TAB\}", 5);
            _browser.PressKeys(@"\{ENTER\}");

            for (int j = 0; j < 5; j++)
            {
                try
                {
                    _browser.AssertElementIsActive(IdentifierType.Id, "makeSureIExist", "input", 1);
                    break;
                }
                catch (AssertionFailedException)
                {
                    _browser.PressKeys(@"\{TAB\}");
                }
            }
        }
    }
}
