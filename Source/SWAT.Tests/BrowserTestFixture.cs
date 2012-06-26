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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using SWAT.DataAccess;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using SWAT.Reflection;

namespace SWAT.Tests
{
    public abstract class BrowserTestFixture
    {
        protected WebBrowser _browser;
        protected BrowserType _browserType;
        protected IBrowser iBrowserInstance;
        //protected List<double> durations;

        public enum BrowserProcess
        {
            iexplore, firefox, chrome, safari
        }

        [SetUp]
        public virtual void TestSetup()
        {
        }

        [TearDown]
        public virtual void TestTeardown()
        {

        }

        [TestFixtureSetUp]
        public virtual void Setup()
        {
            try
            {
                SafariSettings.SafariAddress = "120.0.0.1";
                SWAT.WantSuspendOnFail.SuspendTestOnFail = false;
                UserConfigHandler.Save();

                _browser = new WebBrowser(_browserType);
                iBrowserInstance = ReflectionHelper.GetField<IBrowser>(_browser, "_browser");

                this.OpenSwatTestPage();
            }
            catch //If set up fails, try again once more..
            {
                _browser.Sleep(5000);
                _browser.KillAllOpenBrowsers();

                _browser = new WebBrowser(_browserType);
                iBrowserInstance = ReflectionHelper.GetField<IBrowser>(_browser, "_browser");

                this.OpenSwatTestPage();
            }
        }

        [TestFixtureTearDown]
        public virtual void TearDown()
        {
            _browser.KillAllOpenBrowsers();
            _browser.Dispose();
            //double sum = 0;
            //foreach (double time in durations)
            //{
            //    sum += time;
            //}

            //double avg = sum / durations.Count;

            //System.Console.WriteLine("Average time to run the SetUp: " + avg);
        }

        public BrowserTestFixture(BrowserType browserType)
        {
            _browserType = browserType;
        }

        #region Helper Methods

        protected void SetForceBrowserPressKeys(bool forcePressKeys)
        {
            ReflectionHelper.SetField(_browser, "_forceBrowserPressKeys", forcePressKeys);
        }

        protected void OpenSwatTestPage()
        {
            _browser.OpenBrowser();
            this.NavigateToSwatTestPage();
        }

        protected void AttachToSwatTestPage()
        {
            _browser.AttachToWindow("SWAT Test Page");
        }

        protected void NavigateToSwatTestPage()
        {
            _browser.NavigateBrowser(getTestPage("TestPage.htm"));
        }

        protected string getTestPage(string pageName)
        {
            // used for computers that aren't respected by name by the Mac
            //if (_browserType == BrowserType.Safari)
            //{
            //    return string.Format("http://{0}/swat/{1}", Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), pageName); // uses the internetwork ip address
            //}
            return string.Format("http://{0}/swat/{1}", Environment.MachineName.ToLower(), pageName); // uses the machine name
            //return System.Configuration.ConfigurationManager.AppSettings["TestPageDirectory"] + pageName; // uses the local directory
        }

        protected string GetBrowserName()
        {
            if (_browserType == BrowserType.FireFox)
                return "firefox";
            else if (_browserType == BrowserType.InternetExplorer)
                return "iexplore";
            else if (_browserType == BrowserType.Chrome)
                return "chrome";
            else if (_browserType == BrowserType.Safari)
                return "safari";
            else
                return "";
        }

        protected IntPtr GetCurrentBrowserWindowHandle()
        {
            Browser browserObj = (Browser)ReflectionHelper.GetField<object>(_browser, "_browser"); 

            return (IntPtr)ReflectionHelper.GetField<object>(browserObj, "curWindowHandle");
        }

        protected string getTestFilePath(string fileName)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["TestFileDirWithoutMachineName"];
    
            return string.Format(@"\\{0}{1}{2}", 
                Environment.MachineName.ToLower(),
                s,
                fileName);
        }

        protected void AssertWindowTitle(string expectedWindowTitle)
        {
            string actualWindowTitle = _browser.GetWindowTitle();
            Assert.AreEqual(expectedWindowTitle, actualWindowTitle, String.Format("Assertion failed: [[{0}]]!= [[{1}]]", expectedWindowTitle, actualWindowTitle));
        }

        public string GetUserConfigFilePath()
        {
            string filePath = Path.Combine(Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().GetName().CodeBase), "SWAT.user.config");

            return filePath.Replace("file:\\", "").Replace('\\', '/');
        }

        protected string CurrentSWATAssemblyPath
        {
            get
            {
                Process editor = new Process();
                string path = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(path);
                path = Uri.UnescapeDataString(uri.Path);
                path = Path.GetDirectoryName(path);
                return path;
            }
        }

        protected void TabThroughInternetExplorerComponents()
        {
			// 6 is the number used on the build machine because
			// it has the Favorites toolbar disabled
            if (_browserType == BrowserType.InternetExplorer)
                _browser.PressKeys("\\{TAB\\}", 3);
        }

        #endregion
    }
}
