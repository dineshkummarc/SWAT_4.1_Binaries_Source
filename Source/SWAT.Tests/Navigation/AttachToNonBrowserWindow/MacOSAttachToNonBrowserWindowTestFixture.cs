using System;
using NUnit.Framework;

namespace SWAT.Tests.AttachToNonBrowserWindow
{
    [TestFixture]
    [Category("Safari")]
    public class MacOSAttachToNonBrowserWindowTestFixture : AttachToNonBrowserWindowTestFixture
    {
        public MacOSAttachToNonBrowserWindowTestFixture() : base(BrowserType.Safari) { }

        protected override void OpenExcelDoc(string name)
        {
            string script = string.Format("with timeout of 10 seconds\rtell application \"Microsoft Excel\" to open \"/Volumes/c$/SWAT/trunk/SWAT.Tests/TestPages/files/{0}\"\rend timeout\rreturn \"done\"", name);
            try
            {
                _browser.RunScript("applescript", script, "done");
            }
            catch (AssertionFailedException) { }
        }

        protected override void CleanupBrokenFileExcel()
        {
            const string script = "tell application \"System Events\"\rtell process \"Microsoft Excel\"\ractivate\rset frontmost to true\rclick button 2 of window 1\rend tell\rend tell";
            try
            {
                _browser.RunScript("applescript", script, "");
            }
            catch (AssertionFailedException) { }
        }
        
        protected override void CloseAllExcelWindows()
        {
            const string script = "tell application \"Microsoft Excel\"\rquit\rend tell";
            try
            {
                _browser.RunScript("applescript", script, "");
            }
            catch (AssertionFailedException) { }
        }
        
        [Test]
        [ExpectedException(typeof(Exception))]
        public void CloseNonBrowserWindowErrorWhenWindowIsClosedTest()
        {
            OpenExcelDoc(1);
            _browser.AttachToNonBrowserWindow(ApplicationType.Excel, "ExcelTest", 50);
            const string script = "tell application \"Microsoft Excel\" to close window 1\rreturn \"done\"";
            _browser.RunScript("applescript", script, "done");
            _browser.CloseNonBrowserWindow();
        }
    }
}
