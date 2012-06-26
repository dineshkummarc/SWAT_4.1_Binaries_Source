using System.Diagnostics;
using NUnit.Framework;

namespace SWAT.Tests.AttachToNonBrowserWindow
{
    [TestFixture]
    [Category("Misc")]
    public class WindowsAttachToNonBrowserWindowTestFixture : AttachToNonBrowserWindowTestFixture
    {
        public WindowsAttachToNonBrowserWindowTestFixture() : base(BrowserType.InternetExplorer) { }

        protected override void OpenExcelDoc(string name)
        {
            Process p = new Process();
            p.StartInfo.FileName = "excel.exe";
            p.StartInfo.Arguments = string.Format(@"C:\SWAT\trunk\SWAT.Tests\TestPages\files\{0}", name);
            p.Start();
        }

        protected override void CleanupBrokenFileExcel()
        {
            //do nothing
        }
        
        protected override void CloseAllExcelWindows()
        {
            ProcessKiller.Kill("excel");
        }
    }
}
