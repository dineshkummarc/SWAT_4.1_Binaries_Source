using NUnit.Framework;

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("Chrome")]
    public class ChromeUnitTestFixture
    {
        private WebBrowser _browser;

        [SetUp]
        public void TestSetUp()
        {

        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _browser.KillAllOpenBrowsers();
        }

        [Test]
        [Repeat(100)]
        public void ChromeClosesPortsCorrectlyAfterUseTest()
        {
            _browser = new WebBrowser(BrowserType.Chrome);
            _browser.OpenBrowser();
            _browser.KillAllOpenBrowsers();
        }
    }
}