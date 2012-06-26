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


using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System;

namespace SWAT.Windows
{
    public class ApiWindow
    {
        public string MainWindowTitle { get; set; }
        public string ClassName { get; set; }
        public int hWnd { get; set; }
    }

    public class WindowsEnumerator
    {
        private List<ApiWindow> children = new List<ApiWindow>();
        private List<ApiWindow> topLevel = new List<ApiWindow>();

        private string topLevelClass = "";
        private string childClass = "";

        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;

        private delegate int EnumCallBackDelegate(int hWnd, int lParam);

        // Top-level windows
        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumCallBackDelegate lpEnumFunc, int lParam);

        // Child windows
        [DllImport("user32.Dll")]
        private static extern int EnumChildWindows(int hWndParent, EnumCallBackDelegate lpEnumFunc, int lParam);

        // Get the window class
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(int hWnd, StringBuilder lpClassName, int nMaxCount);

        // Test if the window is visible--only get visible ones.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(int hWnd);

        // Test if the window's parent--only get the one's without parents
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern int GetParent(int hWnd);

        // Get window text length signature
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(int hWnd, int wMsg, int wParam, int lParam);

        // Get window text signature
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(int hWnd, int wMsg, int wParam, StringBuilder lParam);

        /// <summary>
        /// Get all top-level window information.
        /// </summary>
        /// <returns>List of window information objects</returns>
        public List<ApiWindow> GetTopLevelWindows()
        {
            EnumWindows(EnumWindowsProc, 0x0000);
            return topLevel;
        }

        /// <summary>
        /// Get all top-level window information with the given class name.
        /// </summary>
        /// <returns>List of window information objects</returns>
        public List<ApiWindow> GetTopLevelWindows(string className)
        {
            topLevelClass = className;
            return GetTopLevelWindows();
        }

        /// <summary>
        /// Get all child windows for the specific window handle (hWnd).
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns>List of child windows for parent window</returns>
        public List<ApiWindow> GetChildWindows(int hWnd)
        {
            // Clear the window list
            children = new List<ApiWindow>();

            // Start the enumeration process
            EnumChildWindows(hWnd, EnumChildWindowProc, 0x0000);

            // Return the children list when the process is completed.
            return children;
        }

        /// <summary>
        /// Get all child windows for the specific 
        /// window handle (hWnd) and class name.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns>List of child windows for parent window</returns>
        public List<ApiWindow> GetChildWindows(int hWnd, string className)
        {
            // Set the search
            childClass = className;
            return GetChildWindows(hWnd);
        }

        /// <summary>
        /// Callback function that does the work of enumerating top-level windows.
        /// </summary>
        /// <param name="hWnd">Discovered Window handle</param>
        /// <param name="lParam"></param>
        /// <returns>1=keep going, 0=stop</returns>
        private int EnumWindowsProc(int hWnd, int lParam)
        {
            // Eliminate windows that are not top-level.
            if (GetParent(hWnd) == 0 && IsWindowVisible(hWnd))
            {
                // Get the window title / class name.
                ApiWindow window = GetWindowIdentification(hWnd);

                // Match the class name if searching for a specific window class.
                if (string.IsNullOrEmpty(topLevelClass) || window.ClassName.ToLower() == topLevelClass.ToLower())
                {
                    topLevel.Add(window);
                }
            }

            // To continue enumeration, return True (1), and to stop enumeration 
            // return False (0).
            // When 1 is returned, enumeration continues until there are no 
            // more windows left.

            return 1;
        }

        /// <summary>
        /// Callback function that does the work of enumerating child windows.
        /// </summary>
        /// <param name="hWnd">Discovered Window handle</param>
        /// <param name="lParam"></param>
        /// <returns>1=keep going, 0=stop</returns>
        private int EnumChildWindowProc(int hWnd, int lParam)
        {
            ApiWindow window = GetWindowIdentification(hWnd);

            // Attempt to match the child class, if one was specified, otherwise
            // enumerate all the child windows.
            if (childClass.Length == 0 || window.ClassName.ToLower() == childClass.ToLower())
            {
                children.Add(window);
            }

            return 1;
        }

        // Build the ApiWindow object to hold information about the Window object.
        private ApiWindow GetWindowIdentification(int hWnd)
        {
            ApiWindow window = new ApiWindow();
            StringBuilder title = new StringBuilder();

            // Get the size of the string required to hold the window title.
            int size = NativeMethods.GetWindowTextLength(new IntPtr(hWnd));
            //int size = SendMessage(hWnd, WM_GETTEXTLENGTH, 0, 0);

            // If the return is 0, there is no title.
            if (size > 0)
            {
                title = new StringBuilder(size + 1);
                NativeMethods.GetWindowText(new IntPtr(hWnd), title, title.Capacity);
                //SendMessage(hWnd, WM_GETTEXT, title.Capacity, title);
            }

            // Get the class name for the window.
            StringBuilder classBuilder = new StringBuilder(64);
            GetClassName(hWnd, classBuilder, 64);

            // Set the properties for the ApiWindow object.
            window.ClassName = classBuilder.ToString();
            window.MainWindowTitle = title.ToString();
            window.hWnd = hWnd;

            return window;
        }
    }
}
