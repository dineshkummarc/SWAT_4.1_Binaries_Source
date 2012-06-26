using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
 
using SWAT.Reflection;

namespace SWAT.Tests.UnitTests
{
    [TestFixture]
    [Category("IE")]
    public class InternetExplorerUnitTestFixture
    {
        [TestFixtureTearDown]
        public void TestFixtureTeardown()
        {
            ProcessKiller killer = new ProcessKiller("iexplore");
            killer.Kill();
        }

        [Test]
        [ExpectedException(typeof(BrowserExistException))]
        public void isBrowserAccessibleThrowsBrowserExistExceptionTest()
        {
            InternetExplorer ie = new InternetExplorer();
            ie.OpenBrowser();
            ProcessKiller killer = new ProcessKiller("iexplore");
            killer.KillAsyncDelayed();
            ExecutePrivateMethod(ie, "isBrowserAccessible");
        }

        [Test]
        public void isBrowserDocumentHTMLReturnsFalseWhenWindowClosesUnexpectedlyTest()
        {
            InternetExplorer ie = new InternetExplorer();
            ie.OpenBrowser();
            ProcessKiller killer = new ProcessKiller("iexplore");
            killer.KillAsyncDelayed();
            object returnValue = ExecutePrivateMethod(ie, "isBrowserDocumentHTML");
            Assert.IsNotNull(returnValue);
            bool returned = (bool)returnValue;
            Assert.IsFalse(returned);
        }

        [Test]
        public void isBrowserValidReturnsFalseWhenWindowClosesUnexpectedlyTest()
        {
            InternetExplorer ie = new InternetExplorer();
            ie.OpenBrowser();
            ProcessKiller killer = new ProcessKiller("iexplore");
            killer.KillAsyncDelayed();
            object returnValue = ExecutePrivateMethod(ie, "isBrowserValid");
            Assert.IsNotNull(returnValue);
            bool returned = (bool)returnValue;
            Assert.IsFalse(returned);
        }

         
        [TestCase("waitForBrowser")]
        [TestCase("waitForBrowserReadyOnly")]
        public void waitForBrowsersThrowsExceptionWhenWindowClosesUnexpectedlyTest(string methodName)
        {
            InternetExplorer ie = new InternetExplorer();
            ie.OpenBrowser();
            ProcessKiller killer = new ProcessKiller("iexplore");
            killer.KillAsyncDelayed();
            ExecutePrivateMethod(ie, methodName);
        }

        [Test]
        public void SimulateCOMExceptionWhileKillAllOpenBrowsersExceptWindowTitleIsExecutingDoesNotCrashTest()
        {
            InternetExplorer ieBrowser = new InternetExplorer();
            try
            {
                SetUpKillAllOpenBrowsersCOMExceptionSimulation(ieBrowser);
                ieBrowser.KillAllOpenBrowsers("facebook");
            }
            finally
            {
                new ProcessKiller("iexplore").Kill();
            }
        }

        [Test]
        public void SimulateCOMExceptionIsThrownWhileKillAllOpenBrowsersIsExecutingDoesNotCrashTest()
        {
            InternetExplorer ieBrowser = new InternetExplorer();
            try
            {
                SetUpKillAllOpenBrowsersCOMExceptionSimulation(ieBrowser);
                ieBrowser.KillAllOpenBrowsers();
            }
            finally
            {
                new ProcessKiller("iexplore").Kill();
            }
        }

        #region Helper Functions

        private object ExecutePrivateMethod(object objectName, string methodName)
        {
            object returnValue = null;
            DateTime timeout = DateTime.Now.AddSeconds(5);
            while (DateTime.Now < timeout)
            {
                returnValue =  ReflectionHelper.InvokeMethod<object>(objectName, methodName);
                Thread.Sleep(500);
            }
            return returnValue;
        }

        private void SetUpKillAllOpenBrowsersCOMExceptionSimulation(InternetExplorer ieBrowser)
        {
            OpenNEmptyWindows(ieBrowser, 3);
            SetForceCOMException(ieBrowser);
        }

        private void SetForceCOMException(InternetExplorer ieBrowser)
        {
            ReflectionHelper.SetField(ieBrowser, "forceCOMException", true);
        }

        private void OpenNEmptyWindows(InternetExplorer ieBrowser, int numberOfWindows)
        {
            for (int i = 0; i < numberOfWindows; i++)
            {
                ieBrowser.OpenBrowser();
            }
        }

        #endregion
    }
}
