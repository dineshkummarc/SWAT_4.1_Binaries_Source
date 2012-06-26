using System.Diagnostics;
using NUnit.Framework;
using System;
using SWAT.DataAccess;

namespace SWAT.Tests.AttachToNonBrowserWindow
{
    public abstract class AttachToNonBrowserWindowTestFixture
    {
        protected WebBrowser _browser;

        public AttachToNonBrowserWindowTestFixture(BrowserType type)
        {
            _browser = new WebBrowser(type);
        }
        
        [Test]
        [ExpectedException(typeof(IndexOutOfBoundsException))]
        public void AttachToNonBrowserWindowFailsWithNegativeIndexTest()
        {
            _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "negative index test", -1, 20);
        }

        [Test]
        public void AttachToNonBrowserWindowPassesTest()
        {
            OpenExcelDoc(1);
            _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "ExcelTest", 50);
            _browser.CloseNonBrowserWindow();
        }

        [Test]
        [ExpectedException(typeof(NonBrowserWindowExistException))]
        public void AttachToNonBrowserWindowFailsTest()
        {
            _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "this title does not exist", 5);
        }

        [Test]
        public void AttachToNonBrowserWindowWithIndexPassesTest()
        {
            OpenExcelDoc(1);
            OpenExcelDoc(2);
            _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "ExcelTest", 1, 50);
            _browser.CloseNonBrowserWindow();
            bool success = false;
            try
            {
                _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "ExcelTest2Substring", 20);
            }
            catch (NonBrowserWindowExistException)
            {
                success = true;
            }
            _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "ExcelTest", 0, 50);
            _browser.CloseNonBrowserWindow();
            if (!success)
                Assert.Fail("AttachToNonBrowserWindow is not properly attaching to the correctly indexed window");
        }

        [Test]
        public void AttachToNonBrowserWindowWithIndexFailsTest()
        {
            OpenExcelDoc(1);
            bool success = false;
            try
            {
                _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "ExcelTest", 1, 20);
            }
            catch (NonBrowserWindowExistException)
            {
                success = true;
            }
            _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "ExcelTest", 20);
            _browser.CloseNonBrowserWindow();

            if (!success)
                Assert.Fail("AttachToNonBrowserWindow with index is handling indexes incorrectly");
        }

        [Test]
        public void CloseNonBrowserWindowPassesTest()
        {
            OpenExcelDoc(1);
            _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "ExcelTest", 50);
            _browser.CloseNonBrowserWindow();
        }

        [Test]
        [ExpectedException(typeof(NoAttachedWindowException))]
        public void CloseNonBrowserWindowFailsTest()
        {
            _browser.CloseNonBrowserWindow();
        }

        [Test]
        [ExpectedException(typeof(NonBrowserWindowExistException))]
        public void AttachToNonBrowserWindowFailsWithBrokenFileTest()
        {
            OpenExcelDoc(3);
            try
            {
                _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "Broken", 5);
            }
            finally
            {
                CleanupBrokenFileExcel();
                CloseAllExcelWindows();
            }
        }
        
#region Helper Methods

        protected void OpenExcelDoc(int number)
        {
            switch (number)
            {
                case 1 : OpenExcelDoc("ExcelTest.xls"); break;
                case 3 : OpenExcelDoc("Broken.xls"); break;
                default : OpenExcelDoc("ExcelTest2Substring.xls"); break;
            }
        }

        //different for each OS
        protected abstract void OpenExcelDoc(string name);
        protected abstract void CloseAllExcelWindows();
        protected abstract void CleanupBrokenFileExcel();
         
#endregion
    }
}
