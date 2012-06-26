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
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("FireFox")]
    public class JSSHConnectionTestFixture
    {
        #region SetUp & TearDown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            KillFirefox();
        }

        [SetUp]
        public void TestSetUp()
        {
            Process.Start(firefoxPath, firefoxArguments);
            Assert.IsTrue(ConnectToJSSH());
        }

        [TearDown]
        public void TestTearDown()
        {
            connection.Dispose();
            KillFirefox();
        }

        #endregion

        #region Tests

        [Test]
        public void SpecialCharactersReturnedFromBrowserTitleJSSHTest()
        {
            const string actualTitle = "ConfiguraciÓn salteña¡ krúss caffé¿ bahá'í güe Áetna eÑe Éthernet Ígloo Úruguay Über";
            WebBrowser browser = new WebBrowser(BrowserType.FireFox);
            browser.OpenBrowser();
            string page = string.Format("http://{0}/swat/{1}", Environment.MachineName.ToLower(), "SpanishCharactersPage.html");
            browser.NavigateBrowser(page);
            try
            {
                string title = browser.GetWindowTitle();
                Assert.AreEqual(title, actualTitle);
            }
            finally
            {
                browser.CloseBrowser();
            }
        }

        [Test]
        public void SpecialCharactersReturnedFromAssertionJSSHTest()
        {
            const string elemValue = "Óúña¡í güeeÑe ÉtÜ";
            WebBrowser browser = new WebBrowser(BrowserType.FireFox);
            browser.OpenBrowser();
            string page = string.Format("http://{0}/swat/{1}", Environment.MachineName.ToLower(), "ElementsWithSpecialCharactersPage.htm");
            browser.NavigateBrowser(page);
            try
            {
                browser.AssertElementExists(IdentifierType.Expression, "id=Unicode Character Test;value=" + elemValue, "input");
            }
            finally
            {
                browser.CloseBrowser();
            }
        }

        [Test]
        [Ignore]
        public void TestJSSHConnectionConstructorCorrectlyCopiesSessionVariablesWhenPassedValidGuid()
        {
            string guid = Guid.NewGuid().ToString();
            Assert.IsTrue(SetUpJSSHVariables());
            Assert.AreEqual(guid, AddGuidToWindow(guid));
            using (JSSHConnection jssh = new JSSHConnection(guid))
            {
                Assert.IsTrue(AreSessionVariablesDefined());
            }
        }

        [Test]
        public void TestConnectedPropertyReturnsTrueWhenFirefoxIsOpen()
        {
            try
            {
                bool initialConnection = connection.ConnectToJSSH();
                bool connected = connection.Connected;
                Assert.IsTrue(connected);
            }
            finally
            {
                KillFirefox();
            }
        }

        [Test]
        public void TestConnectedPropertyReturnsFalseWhenFirefoxIsNotOpen()
        {
            connection.ConnectToJSSH();
            KillFirefox();
            Assert.IsFalse(connection.Connected);
        }

        [Test]
        public void TestConnectedPropertyReturnsFalseWhenSocketIsNull()
        {
            JSSHConnection nullConnection = new JSSHConnection();
            nullConnection.Dispose();
            Assert.IsFalse(nullConnection.Connected);
        }

        [Test]
        public void TestFreshConnectToJSSHReturnsFalseWhenFirefoxIsNotOpen()
        {
            connection.Dispose();
            KillFirefox();
            Assert.IsFalse(connection.ConnectToJSSH());
        }

        [Test]
        public void TestSendMessageReturnsErrorMessageWhenNotConnectedToJSSH()
        {
            string errorMessage = "SOCKET DISCONNECTED.";
            KillFirefox();
            Assert.AreEqual(errorMessage, SendMessageOnce());
        }

        #endregion

        #region Methods

        private bool ConnectToJSSH()
        {
            connection = new JSSHConnection();
            
            DateTime timeout = DateTime.Now.AddSeconds(10);
            do
            {
                if (connection.ConnectToJSSH())
                {
                    return true;
                }
                Thread.Sleep(0);
            } while (DateTime.Now < timeout);

            return false;
        }

        private void KillFirefox()
        {
            ProcessKiller.Kill(firefoxProcess);
        }

        private bool SetUpJSSHVariables()
        {
            StringBuilder javascriptBuilder = new StringBuilder();
            javascriptBuilder.Append("var windows = getWindows();");
            javascriptBuilder.Append("var window = windows[0];");
            javascriptBuilder.Append("var browser = window.getBrowser();");
            javascriptBuilder.Append("var doc = browser.contentDocument;");
            javascriptBuilder.Append("print(doc);");
            string message = javascriptBuilder.ToString();

            string result = "";
            DateTime timeout = DateTime.Now.AddSeconds(10);

            do
            {
                result = connection.SendMessage(message);
                if (IsDocumentObject(result))
                {
                    return true;
                }
            }
            while (DateTime.Now < timeout);

            return false;
        }

        private bool IsDocumentObject(string result)
        {
            return result.Contains("[object XPCNativeWrapper [object HTMLDocument]]");
        }

        private string AddGuidToWindow(string guid)
        {
            string result = connection.SendMessage("window.guid = '" + guid + "';");

            if (IsDocumentObject(result))
            {
                System.Threading.Thread.Sleep(2000);
                result = connection.SendMessage("window.guid = '" + guid + "';");
            }

            return result;
        }

        private bool AreSessionVariablesDefined()
        {
            StringBuilder javascriptBuilder = new StringBuilder();
            string window = connection.SendMessage("print(window);");
            string browser = connection.SendMessage("print(browser);");
            string doc = connection.SendMessage("print(doc);");
            string result = string.Format("{0}{1}{2}", window, browser, doc);
            return result.Equals("[object ChromeWindow][object XULElement][object XPCNativeWrapper [object HTMLDocument]]");
        }

        private string SendMessageOnce()
        {
            return connection.SendMessage("print(doc);");
        }

        #endregion

        #region Test Variables

        private JSSHConnection connection;
        private string firefoxPath = BrowserPaths.FirefoxRootDirectory;
        private const string firefoxArguments = "http://localhost/swat/testpage.htm -jssh";
        private const string firefoxProcess = "firefox";

        #endregion
    }
}
