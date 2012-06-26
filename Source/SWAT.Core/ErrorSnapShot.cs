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
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SWAT
{
	public class ErrorSnapShot
    {
        #region Private Instance Members

	    private static readonly HashSet<string> NonUiCommands = ExcludedMethodsSet();
        private IDocumentInfo _docInfo;
        private BrowserType _browserType;

        #endregion

        #region Constructors

        public ErrorSnapShot()
        {

        }

        public ErrorSnapShot(IDocumentInfo docInfo, BrowserType browserType)
        {
            _docInfo = docInfo;
            _browserType = browserType;
        }

        #endregion

        #region Methods To Get ScreenShots

        private void CaptureScreenToFile(string filename, ImageFormat format) 
        {
            Bitmap img = CreateImageFromDesktop(User32.GetShellWindow());
            img.Save(filename,format);
        }

        private void CaptureScreenToFile(string filename, ImageFormat format, IntPtr contentHandle)
        {
            Bitmap img = CreateImageFromDocument(contentHandle);

            if (isBlack(img))
                img = CreateImageFromDesktopCrop(contentHandle);

            img.Save(filename, format);
        }

        //private int ParseNumber(string attribute)
        //{
        //    Regex numeric = new Regex("[0-9]");

        //    for (int i = 0; i < attribute.Length; i++)
        //    {
        //        string sub = attribute.Substring(i, 1);
        //        if (!numeric.IsMatch(sub))
        //            return int.Parse(attribute.Substring(0, i));
        //    }

        //    return int.Parse(attribute);
        //}

        private Bitmap CreateImageFromDocument(IntPtr contentHandle)
        {
            _docInfo.SetDocumentAttribute("scroll", "yes");

            //Get Browser Window Height
            var browserHeight = int.Parse(_docInfo.GetDocumentAttribute("scrollHeight").ToString());
            var browserWidth = int.Parse(_docInfo.GetDocumentAttribute("scrollWidth").ToString());

            //Get Screen Height
            var screenHeight = int.Parse(_docInfo.GetDocumentAttribute("clientHeight").ToString());
            var screenWidth = int.Parse(_docInfo.GetDocumentAttribute("clientWidth").ToString());

            //Get bitmap to hold screen fragment.
            Bitmap bm = new Bitmap(screenWidth, screenHeight, PixelFormat.Format48bppRgb);

            //Create a target bitmap to draw into.
            Bitmap bm2 = new Bitmap(browserWidth, browserHeight, PixelFormat.Format16bppRgb555);

            Graphics g2 = Graphics.FromImage(bm2);

            var myPage = 0;

            //Get Screen Height (for bottom up screen drawing)
            while ((myPage * screenHeight) < browserHeight)
            {
                _docInfo.SetDocumentAttribute("scrollTop", (screenHeight - 5) * myPage);
                myPage++;
            }
            //Rollback the page count by one
            myPage--;

            var myPageWidth = 0;

            while ((myPageWidth * screenWidth) < browserWidth)
            {
                _docInfo.SetDocumentAttribute("scrollLeft", (screenWidth - 5) * myPageWidth);
                var brwLeft = int.Parse(_docInfo.GetDocumentAttribute("scrollLeft").ToString());

                for (var i = myPage; i >= 0; --i)
                {
                    //Shoot visible window
                    var g = Graphics.FromImage(bm);
                    var hdc = g.GetHdc();

                    _docInfo.SetDocumentAttribute("scrollTop", (screenHeight - 5) * i);
                    var brwTop = int.Parse(_docInfo.GetDocumentAttribute("scrollTop").ToString());

                    User32.PrintWindow(contentHandle, hdc, PrintWindowFlags.PW_ALL);

                    // Original code
                    g.ReleaseHdc(hdc);
                    g.Flush();
                    g.Dispose();

                    var hBitmap = bm.GetHbitmap();
                    System.Drawing.Image screenfrag = System.Drawing.Image.FromHbitmap(hBitmap);
                    GDI32.DeleteObject(hBitmap);

                    if (_browserType == BrowserType.Chrome && i == myPage)
                    {
                        brwTop -= ((myPage + 1) * screenHeight - browserHeight - (i * 5));
                    }

                    g2.DrawImage(screenfrag, brwLeft, brwTop);
                }
                ++myPageWidth;
            }

            Bitmap img = new Bitmap(browserWidth, browserHeight, PixelFormat.Format16bppRgb555);
            Graphics gFinal = Graphics.FromImage(img);
            gFinal.DrawImage(bm2, 0, 0, browserWidth, browserHeight);

            //Clean Up.
            g2.Dispose();
            gFinal.Dispose();
            bm.Dispose();
            bm2.Dispose();

            return img;
        }

        private Boolean isBlack(Bitmap image)
        {
            Random rand = new Random();
            int blackCount = 0;

            for(int i = 0; i < 10; i++)
            {
                if (image.GetPixel( (int)(image.Width * rand.NextDouble()),
                    (int)(image.Height * rand.NextDouble())).ToArgb() == Color.Black.ToArgb())
                    blackCount++;
            }

            if (blackCount == 10)
                return true;
            return false;
        }

        private Image CreateImageFromDesktopForCrop(IntPtr handle)
        {
            // get the hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);

            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            //GDI32.DeleteObject(hBitmap);

            return img;
        }

        private Bitmap CreateImageFromDesktopCrop(IntPtr contentHandle)
        {
            _docInfo.SetDocumentAttribute("scroll", "yes");

            //Get Browser Window Height
            var browserHeight = int.Parse(_docInfo.GetDocumentAttribute("scrollHeight").ToString());
            var browserWidth = int.Parse(_docInfo.GetDocumentAttribute("scrollWidth").ToString());

            //Get Screen Height
            var screenHeight = int.Parse(_docInfo.GetDocumentAttribute("clientHeight").ToString());
            var screenWidth = int.Parse(_docInfo.GetDocumentAttribute("clientWidth").ToString());

            //Create a target bitmap to draw into.
            Bitmap bm2 = new Bitmap(browserWidth, browserHeight, PixelFormat.Format16bppRgb555);

            Graphics g2 = Graphics.FromImage(bm2);

            var myPage = 0;

            //Get Screen Height (for bottom up screen drawing)
            while ((myPage * screenHeight) < browserHeight)
            {
                _docInfo.SetDocumentAttribute("scrollTop", (screenHeight - 5) * myPage);
                myPage++;
            }
            //Rollback the page count by one
            myPage--;

            var myPageWidth = 0;

            while ((myPageWidth * screenWidth) < browserWidth)
            {
                _docInfo.SetDocumentAttribute("scrollLeft", (screenWidth - 5) * myPageWidth);
                var brwLeft = int.Parse(_docInfo.GetDocumentAttribute("scrollLeft").ToString());

                for (var i = myPage; i >= 0; --i)
                {
                    //Get Window Location
                    User32.RECT srcRect = new User32.RECT();
                    User32.GetWindowRect(contentHandle, ref srcRect);

                    //Shoot visible window
                    _docInfo.SetDocumentAttribute("scrollTop", (screenHeight - 5) * i);
                    var brwTop = int.Parse(_docInfo.GetDocumentAttribute("scrollTop").ToString());

                    User32.SetForegroundWindow(contentHandle);
                    System.Threading.Thread.Sleep(200);
                    Bitmap desktop = CreateImageFromDesktopForCrop(User32.GetShellWindow()) as Bitmap;
                    Bitmap bm = desktop.Clone(new Rectangle(srcRect.Left, srcRect.Top, screenWidth, screenHeight), PixelFormat.Format48bppRgb);

                    var hBitmap = bm.GetHbitmap();
                    System.Drawing.Image screenfrag = System.Drawing.Image.FromHbitmap(hBitmap);
                    GDI32.DeleteObject(hBitmap);

                    if (_browserType == BrowserType.Chrome && i == myPage)
                    {
                        brwTop -= ((myPage + 1) * screenHeight - browserHeight - (i * 5));
                    }

                    g2.DrawImage(screenfrag, brwLeft, brwTop);
                    bm.Dispose();
                }
                ++myPageWidth;
            }

            Bitmap img = new Bitmap(browserWidth, browserHeight, PixelFormat.Format16bppRgb555);
            Graphics gFinal = Graphics.FromImage(img);
            gFinal.DrawImage(bm2, 0, 0, browserWidth, browserHeight);

            //Clean Up.
            g2.Dispose();
            gFinal.Dispose();
            bm2.Dispose();

            return img;
        }

        private Bitmap CreateImageFromDesktop(IntPtr desktopHandle)
        {
            // Obtain device context and dimensions
            IntPtr hdcSrc = User32.GetDC(desktopHandle);
            Rectangle desktop = Rectangle.Round((Graphics.FromHdc(hdcSrc)).VisibleClipBounds);

            // Create image from device contexts and dimensions
            IntPtr hdcMem = GDI32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, desktop.Width, desktop.Height);
            IntPtr hOld = GDI32.SelectObject(hdcMem, hBitmap);
            GDI32.BitBlt(hdcMem, 0, 0, desktop.Width, desktop.Height, hdcSrc, 0, 0, CopyPixelOperation.SourceCopy);
            GDI32.SelectObject(hdcMem, hOld);
            Bitmap img = Bitmap.FromHbitmap(hBitmap);

            // Free device contexts and system resources
            GDI32.DeleteDC(hdcMem);
            GDI32.DeleteObject(hBitmap);
            User32.ReleaseDC(desktopHandle, hdcSrc);

            return img;
        }

        public string CaptureAllScreens(string filePath, string command)
        {
            if (NonUiCommands.Contains(command.ToLower()))
                return string.Format("ScreenShot was not taken because \"{0}\" does not require an interface.", command);

            DateTime timeNow = DateTime.Now;
            string fileName = string.Format("{0} {1} Date {2}_{3}_{4} Time {5}_{6}_{7}.jpeg", filePath,command,timeNow.Month,timeNow.Day,timeNow.Year,timeNow.Hour,timeNow.Minute,timeNow.Second);
            try
            {
                CaptureScreenToFile(fileName, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                if(ex.Message.Equals("A generic error occurred in GDI+."))
                    return "Unable to save screenshot in " + filePath + ex.Message; 
                return ex.Message;
            }
            return "ScreenShot saved in : " + fileName;
        }

        public string CaptureBrowser(string filePath, string command, IntPtr contentHandle)
        {
            if (NonUiCommands.Contains(command.ToLower()))
                return string.Format("ScreenShot was not taken because \"{0}\" does not require an interface.", command);

            if (contentHandle == IntPtr.Zero)
                throw new NoAttachedWindowException();
            
            DateTime timeNow = DateTime.Now;
            string fileName = string.Format("{0} {1} Date {2}_{3}_{4} Time {5}_{6}_{7}.jpeg", filePath, command, timeNow.Month, timeNow.Day, timeNow.Year, timeNow.Hour, timeNow.Minute, timeNow.Second);
            try
            {
                CaptureScreenToFile(fileName, ImageFormat.Jpeg, contentHandle);
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("A generic error occurred in GDI+."))
                    return "Unable to save screenshot in " + filePath + ex.Message;
                return ex.Message;
            }
            return " ScreenShot saved in : " + fileName;
        }

        #endregion

        #region GDI32  API Helper Methods

        private class GDI32
        {
            public const int SRCCOPY = 0x00CC0020;

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr handle, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr handleObjectSource, int nXSrc, int nYSrc, CopyPixelOperation dwRop);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr handleDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr handleDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr handleObject);

            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr handleObject);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateBitmap(IntPtr hDC, int nWidth, int nHeight);

        }

        #endregion 

        #region User32 API Helper Methods
        
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }
            
            [DllImport("user32.dll")]
            internal static extern IntPtr GetWindowDC(IntPtr handle); // Returns the device context of this handle

            [DllImport("user32.dll")]
            internal static extern IntPtr ReleaseDC(IntPtr handle, IntPtr handleDC); // Releases the device context

            [DllImport("user32.dll")]
            internal static extern IntPtr GetShellWindow(); // Returns the handle for all screens
            
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName); // Returns the handle of a window

            [DllImport("User32.dll")]
            internal static extern IntPtr GetDC(IntPtr hwnd);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
            
            [DllImport("User32.dll")]
            internal static extern int PrintWindow(IntPtr iHwnd, IntPtr hdcBlt, uint iFlags);

            [DllImport("User32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetForegroundWindow(IntPtr hWnd);
        }

        class PrintWindowFlags
        {
            public const uint PW_ALL = 0x00000000;
            public const uint PW_CLIENTONLY = 0x00000001;
        }

        private static HashSet<string> ExcludedMethodsSet()
        {
            HashSet<string> excludedMethods = new HashSet<string>();

            AddExcludedMethods(excludedMethods, typeof(DataAccess.MSSql));
            AddExcludedMethods(excludedMethods, typeof(DataAccess.Oracle));

            Type webBrowser = typeof (WebBrowser);
            MethodInfo[] methodInfos = webBrowser.GetMethods();

            foreach (MethodInfo methodInfo in methodInfos)
            {
                object[] attributes = methodInfo.GetCustomAttributes(typeof (NonUICommand), true);
                if (attributes.Length > 0)
                    excludedMethods.Add(methodInfo.Name.ToLower());
            }

            return excludedMethods;
        }

        private static void AddExcludedMethods(HashSet<string> excluded, Type t)
        {
            MethodInfo[] methods = t.GetMethods();

            foreach (MethodInfo methodInfo in methods)
                excluded.Add(methodInfo.Name.ToLower());
        }

        #endregion
    }
}
