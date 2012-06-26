using System;
using NUnit.Framework;
using SWAT.Windows;
using SWAT.Reflection;

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("Misc")]
    public class KeyboardInputTestFixture
    {
        WebBrowser browser;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            
        }

        private IntPtr GetTestWindowHandle(string windowTitle)
        {
            WindowsEnumerator enumerator = new WindowsEnumerator();

            foreach (ApiWindow window in enumerator.GetTopLevelWindows())
            {
                if (window.MainWindowTitle.Contains(windowTitle))
                {
                    return new IntPtr(window.hWnd);
                }
            }

            return IntPtr.Zero;
        }

        [Test]
        public void KeyboardCopyWorksTest()
        {
            browser = new WebBrowser(BrowserType.FireFox);

            IBrowser browserObj = (IBrowser) ReflectionHelper.GetField<object>(browser, "_browser");
            KeyboardInput input = new KeyboardInput(browserObj);

            browser.OpenBrowser();
            browser.NavigateBrowser("http://www.google.com");

            input.Copy(GetTestWindowHandle("Google"));

            browser.KillAllOpenBrowsers();
        }

        [Test]
        [ExpectedException(typeof(PressKeysFailureException), UserMessage = "PressKeys failed to type any characters.")]
        public void TestPressKeysInSixtyFourBit()
        {
            browser = new WebBrowser(BrowserType.InternetExplorer);

            IBrowser browserObj = (IBrowser) ReflectionHelper.GetField<object>(browser, "_browser");
            KeyboardInput input = new KeyboardInput(browserObj);

            browser.OpenBrowser();
            browser.NavigateBrowser("http://www.google.com");

            try
            {
                ReflectionHelper.SetField(input, "forceSixtyFourBit", true);
                input.ProcessKey(NativeMethods.VkKeyScan('a'));
                input.SendInputString(browserObj.GetCurrentWindowTitle());
            }
            finally
            {
                // Clean up
                ReflectionHelper.SetField(input, "forceSixtyFourBit", false);
                browser.KillAllOpenBrowsers();
            }
        }
    }
}
