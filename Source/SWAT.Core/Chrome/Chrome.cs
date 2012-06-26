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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.IO;
using SWAT.Windows;

namespace SWAT
{
    [NCover.CoverageExclude]
    public class Chrome : Browser, IBrowser, IDisposable
    {
        #region Constructor

        public Chrome() : base(BrowserType.Chrome, BrowserProcess.chrome)
        {
            chromeHttpServer = new ChromeHttpServer(this);
        }

        #endregion

        #region Private Variables

        ChromeHttpServer chromeHttpServer;
        private bool disposed;
        private IntPtr dialogHandle;
        private const string contentClass = "Chrome_RenderWidgetHostHWND";
        private string previousRequest = "";
        private string requestWeAreWaitingToComplete = "";

        #endregion

        #region Properties

        internal string PreviousRequest
        {
            get { return previousRequest; }
            set
            {
                string currentRequest = value;
                SetRequestToWaitFor(currentRequest);
                previousRequest = currentRequest;
            }
        }

        private string RequestWeAreWaitingToComplete
        {
            get { return requestWeAreWaitingToComplete; }
            set { requestWeAreWaitingToComplete = value; }
        }

        internal bool DialogInterrupted { get; set; }

        private static bool ChromeIsRunning
        {
            get { return Process.GetProcessesByName("chrome").Length > 0; }
        }

        private bool WaitIsRequired
        {
            get
            {
                return !string.IsNullOrEmpty(RequestWeAreWaitingToComplete);
            }
        }

        #endregion

        #region IBrowser Members

        public void OpenBrowser()
        {
            ConnectToChrome();
            WaitForBrowser();
            ChromeCommand command = new ChromeCommand("OpenBrowser");
            chromeHttpServer.SendMessage(command);
            chromeHttpServer.HandleResponse(command);
            curWindowHandle = GetChromeWindowHandle("about:swat - Google Chrome");
        }

        public void CloseBrowser()
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("CloseBrowser");
            chromeHttpServer.SendMessage(command);
            chromeHttpServer.HandleResponse(command);

            WaitForTabToClose();
            
            if (FindDialog(DateTime.Now.AddMilliseconds(100)) == IntPtr.Zero)
                HandleSWATTabs();
        }

        private void HandleSWATTabs()
        {
            if (GetNumberOfSWATTabs() == 0)
                KillAllOpenBrowsers();
        }

        private int GetNumberOfSWATTabs()
        {
            WaitForBrowser();
            ChromeCommand command = new ChromeCommand("GetNumberOfSWATTabs");
            chromeHttpServer.SendMessage(command);
            ChromeResponse response = chromeHttpServer.HandleResponse(command);
            return Int32.Parse("" + response.Value);
        }

        public void RefreshBrowser()
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("RefreshBrowser");
            //command.Arguments.Add("timeout", DefaultTimeouts.WaitForDocumentLoadTimeout);
            chromeHttpServer.SendMessage(command);
            ChromeResponse response = chromeHttpServer.HandleResponse(command);

            switch (response.StatusCode)
            {
                case StatusCode.NAVIGATEFAIL:
                    throw new NavigationTimeoutException(response.Value.ToString());
            }
        }

        public void KillAllOpenBrowsers()
        {
            if (!chromeHttpServer.Disposed)
                chromeHttpServer.Dispose();

            foreach (Process chrome in Process.GetProcessesByName("chrome"))
            {
                try
                {
                    chrome.Kill();
                }
                catch { continue; }
            }

            DateTime timeout = DateTime.Now.AddSeconds(15);

            while (DateTime.Now < timeout && ChromeIsRunning)
                Thread.Sleep(1000);
            Thread.Sleep(500);

            curWindowHandle = IntPtr.Zero;
            PreviousRequest = "KilledAll";
        }

        public void KillAllOpenBrowsers(string windowTitle)
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("KillAllOpenBrowsers");

            command.Arguments.Add("windowTitle", windowTitle);

            chromeHttpServer.SendMessage(command);
            chromeHttpServer.HandleResponse(command);

            WaitForTabToClose();
        }

        private void RecoverFromDialog(JScriptDialogButtonType buttonType)
        {
            if (buttonType == JScriptDialogButtonType.Ok)
                WaitForBrowserWindow();
            else
                lastBrowserWindowAction = LastBrowserWindowAction.InUse;
        }

        private void WaitForBrowserWindow()
        {
            switch (lastBrowserWindowAction)
            {
                case LastBrowserWindowAction.Closed: WaitForTabToClose();
                    break;
            }
        }

        private void WaitForTabToClose()
        {
            lastBrowserWindowAction = LastBrowserWindowAction.Closed;

            if (DialogInterrupted)
                return;

            lastBrowserWindowAction = LastBrowserWindowAction.InUse;

            if (GetAttachedTabId() == -1)
                curWindowHandle = IntPtr.Zero;
        }

        public void NavigateBrowser(string url)
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("NavigateBrowser");
            command.Arguments.Add("url", Utilities.FormatUrl(url));
            chromeHttpServer.SendMessage(command);
            ChromeResponse response = chromeHttpServer.HandleResponse(command);

            switch (response.StatusCode)
            {
                case StatusCode.NAVIGATEFAIL:
                    throw new NavigationTimeoutException(response.Value.ToString());
            }
        }

        public string GetCurrentLocation()
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("GetLocation");
            chromeHttpServer.SendMessage(command);
            ChromeResponse response = chromeHttpServer.HandleResponse(command);

            if (response.StatusCode == StatusCode.UNDEFINEDURL)
                throw new ChromeException("The current document has an undefined URL.");

            return response.Value.ToString();

        }

        public string GetCurrentDocumentTitle()
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("GetWindowTitle");
            chromeHttpServer.SendMessage(command);
            ChromeResponse response = chromeHttpServer.HandleResponse(command);

            switch (response.StatusCode)
            {
                case StatusCode.UNDEFINEDTITLE:
                    throw new ChromeException("The current document has an undefined title.");
            }
            return response.Value.ToString();
        }

        public string GetCurrentWindowTitle()
        {
            WaitForBrowser();

            StringBuilder windowTitle = new StringBuilder(NativeMethods.GetWindowTextLength(curWindowHandle) + 1);
            NativeMethods.GetWindowText(curWindowHandle, windowTitle, windowTitle.Capacity);
            return windowTitle.ToString();
        }

        public void AssertBrowserDoesNotExist(string windowTitle)
        {
            FindBrowser(windowTitle, DefaultTimeouts.AssertBrowserExists, false);
        }

        public void AssertBrowserDoesNotExist(string windowTitle, double timeout)
        {
            FindBrowser(windowTitle, (int)Math.Ceiling(timeout / 1000), false);
        }

        public void AssertBrowserExists(string windowTitle)
        {
            FindBrowser(windowTitle, DefaultTimeouts.AssertBrowserExists, true);
        }

        public void AssertBrowserExists(string windowTitle, double timeout)
        {
            FindBrowser(windowTitle, (int)Math.Ceiling(timeout / 1000), true);
        }

        private void FindBrowser(string windowTitle, double timeout, bool expectPositiveResponse)
        {
            if (!expectPositiveResponse && !ChromeIsRunning)
                return;

            WaitForBrowser();
            ChromeResponse response = LookForBrowser(windowTitle, 0, timeout, expectPositiveResponse);

            switch (response.StatusCode)
            {
                case StatusCode.BROWSEREXISTS:
                    throw new BrowserExistException(response.Value.ToString());
            }
        }

        private ChromeResponse LookForBrowser(string windowTitle, int windowIndex, double timeout, bool expectPositiveResponse)
        {
            ConnectToChrome();

            ChromeCommand command = new ChromeCommand("FindBrowser");
            command.Arguments.Add("windowTitle", windowTitle);
            command.Arguments.Add("windowIndex", windowIndex);
            command.Arguments.Add("expectPositiveResult", expectPositiveResponse);

            return chromeHttpServer.SendMessageWithTimeout(command, System.Convert.ToInt32(timeout));
        }

        public void AttachBrowserToWindow(string windowTitle)
        {
            AttachBrowserToWindow(windowTitle, 0);
        }

        public void AttachBrowserToWindow(string windowTitle, int windowIndex)
        {
            AttachBrowserToWindow(windowTitle, windowIndex, DefaultTimeouts.AttachToWindowBrowserTimeout);
        }

        public void AttachBrowserToWindow(string windowTitle, int windowIndex, int timeout)
        {
            WaitForBrowser();
            LookForBrowser(windowTitle, windowIndex, Convert.ToDouble(timeout), true);

            // suffix is added to the window title for windows
            string suffix = " - Google Chrome";

            ChromeCommand command = new ChromeCommand("AttachToWindow");
            command.Arguments.Add("windowTitle", windowTitle);
            command.Arguments.Add("windowIndex", windowIndex);

            chromeHttpServer.SendMessage(command);
            ChromeResponse response = chromeHttpServer.HandleResponse(command);

            switch (response.StatusCode)
            {
                case StatusCode.SUCCESS:
                    curWindowHandle = GetChromeWindowHandle(response.Value + suffix);
                    break;

                case StatusCode.NOSUCHWINDOW:
                    throw new WindowNotFoundException(response.Value.ToString());

                case StatusCode.WINDOWINDEXOUTOFBOUNDS:
                    string indexLimit = response.Value.ToString();
                    string message;
                    if (indexLimit.Equals("1"))
                        message = "There is only one window";
                    else
                        message = "There are only " + indexLimit + " windows";

                    throw new IndexOutOfRangeException(message + " that has " + windowTitle + " in the title.");

                case StatusCode.PORTDISCONNECTED:
                    throw new ChromeContentScriptIsNotConnectedException();
            }

        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public void AssertTopWindow(string browserTitle, int index, int timeout)
        {
            ChromeCommand command = new ChromeCommand("AssertTopWindow");
            command.Arguments.Add("title", browserTitle.ToLower());
            command.Arguments.Add("index", index);

            ChromeResponse response = chromeHttpServer.SendMessageWithTimeout(command, timeout);

            IntPtr topWindwHnd = NativeMethods.GetForegroundWindow();
            string actualTitle = NativeMethods.GetWindowText(topWindwHnd);
            string title = actualTitle.ToLower();

            if (response.StatusCode != StatusCode.SUCCESS || !title.Contains(browserTitle.ToLower()) || !title.Contains("google chrome"))
            {
                if (index == 0)
                {
                    if (WantInformativeExceptions.GetInformativeExceptions)
                        throw new TopWindowMismatchException(browserTitle, actualTitle);
                    else
                        throw new TopWindowMismatchException(browserTitle);
                }
                else
                {
                    if (response.StatusCode == StatusCode.WINDOWINDEXOUTOFBOUNDS)
                        throw new IndexOutOfRangeException("Index: " + index + " is too large, there are only " + response.Value + " window(s) with title " + browserTitle);
                    else if (WantInformativeExceptions.GetInformativeExceptions)
                        throw new TopWindowMismatchException(browserTitle, index, actualTitle);
                    else
                        throw new TopWindowMismatchException(browserTitle, index);
                }
            }
        }

        #region JS Handling Methods

        private static readonly string[] jsDialogWindowSubstrings = 
            {
                 "Javascript",
                 "The page at",
                 "Alert",
                 "http://",
                 "Confirm Navigation"
            };

        internal override IntPtr GetJSDialogHandle(int timeoutSeconds)
        {
            // Ignore timeout length

            // Find all top level chrome windows
            WindowsEnumerator enumerator = new WindowsEnumerator();
            List<ApiWindow> windows = enumerator.GetTopLevelWindows("Chrome_WidgetWin_0");

            foreach (string windowTitle in jsDialogWindowSubstrings)
            {
                //NativeMethods.GetWindowWithSubstring(windowTitle, 0, 0, ref dialogHwnd);

                // Loop through all of the chrome windows
                foreach (ApiWindow window in windows)
                {
                    // if the title matches
                    if (window.MainWindowTitle.Contains(windowTitle))
                    {
                        IntPtr dialogHwnd = new IntPtr(window.hWnd);

                        // check if the window has a Button within it
                        IntPtr buttonHandle = NativeMethods.GetChildWindowHwnd(dialogHwnd, "Button");

                        // if there is a Button then it is a JavaScript Dialog and not a regular window
                        if (buttonHandle != IntPtr.Zero)
                            return dialogHwnd;

                        // its a JS Dialog but the buttons dont have handles, the sleep will give time to the slow JS Dialogs to load
                        Sleep(1200);
                        return dialogHwnd;
                    }
                }
            }

            return IntPtr.Zero;
        }

        private IntPtr GetJSDialogHandle()
        {
            return GetJSDialogHandle(0); // ignore the timeout
        }

        private static bool ClickDialogButton(IntPtr buttonHandle)
        {
            AutomationElement buttonAe = AutomationElement.FromHandle(buttonHandle);
            object clickButtonAe;
            if (buttonAe.TryGetCurrentPattern(InvokePattern.Pattern, out clickButtonAe))
            {
                try
                {
                    ((InvokePattern)clickButtonAe).Invoke();
                    return true;
                }
                catch { } // ElementNotEnabledException || InvalidOperationException
            }

            return false;
        }

        private static void ClickOkButton(IntPtr dialogHandle)
        {
            NativeMethods.SendMessage(dialogHandle, NativeMethods.WM_SETFOCUS, 0x00000000, 0x00000000);
            NativeMethods.SendMessage(dialogHandle, NativeMethods.WM_KEYDOWN, 0x0000000D, 0x001C0001);
            NativeMethods.SendMessage(dialogHandle, NativeMethods.WM_CHAR, 0x0000000D, 0x401C0001);
            NativeMethods.SendMessage(dialogHandle, NativeMethods.WM_KEYUP, 0x0000000D, 0xC01C0001);
        }

        private static void CloseDialogWindow(IntPtr dialogHandle)
        {
            NativeMethods.CloseWindow(dialogHandle);
        }

        private static bool InteractWithJSDialog(JScriptDialogButtonType buttonType, IntPtr dialogHandle, DateTime timeout)
        {
            bool clicked = false;
            string buttonText = (buttonType == JScriptDialogButtonType.Ok) ? "OK" : "Cancel";

            while (DateTime.Now < timeout)
            {
                IntPtr buttonHandle = NativeMethods.FindWindowEx(dialogHandle, IntPtr.Zero, "Button", buttonText);

                if (buttonHandle == IntPtr.Zero)
                {
                    buttonText = (buttonType == JScriptDialogButtonType.Ok) ? "Leave this Page" : "Stay on this Page";
                    buttonHandle = NativeMethods.FindWindowEx(dialogHandle, IntPtr.Zero, "Button", buttonText);
                }

                // this logic is for new version of Chrome where JS Dialogs buttons dont have a handle
                if (buttonHandle == IntPtr.Zero)
                {
                    buttonText = (buttonType == JScriptDialogButtonType.Ok) ? "OK" : "Cancel";

                    if (buttonText == "OK")
                    {
                        ClickOkButton(dialogHandle);
                        return true;
                    }

                    CloseDialogWindow(dialogHandle);
                    return true;
                }

                if (!clicked && buttonHandle != IntPtr.Zero)
                {
                    clicked = ClickDialogButton(buttonHandle);
                }
                else if (clicked && buttonHandle == IntPtr.Zero)
                {
                    return true;
                }
            }

            return false;
        }

        private IntPtr FindDialog(DateTime timeout)
        {
            IntPtr dialogHandle = IntPtr.Zero;
            while (dialogHandle == IntPtr.Zero && DateTime.Now < timeout)
            {
                dialogHandle = GetJSDialogHandle();
                Thread.Sleep(0);
            }
            return dialogHandle;
        }

        private void WaitForDialogToClose(IntPtr dialogHandle)
        {
            while (GetJSDialogHandle().Equals(dialogHandle))
            {
                Thread.Sleep(0);
            }
        }

        public void ClickJSDialog(JScriptDialogButtonType buttonType)
        {
            bool dialogClicked = false;

            DateTime timeout = DateTime.Now.AddSeconds(15);
            IntPtr dialogHandle = FindDialog(timeout);

            if (dialogHandle != IntPtr.Zero)
            {
                dialogClicked = InteractWithJSDialog(buttonType, dialogHandle, timeout);
            }

            if (DialogInterrupted)
            {
                chromeHttpServer.HandleResponse(new ChromeCommand("ClickJSDialog"));
            }

            RecoverFromDialog(buttonType);

            if (!dialogClicked)
            {
                throw new ClickJSDialogException();
            }

            PreviousRequest = "ClickJSDialog";
            WaitForDialogToClose(dialogHandle);
            if (FindDialog(DateTime.Now.AddMilliseconds(100)) == IntPtr.Zero)
                HandleSWATTabs();
        }

        public void AssertJSDialogContent(string dialogContent)
        {
            AssertJSDialogContent(dialogContent, DefaultTimeouts.FindElementTimeout);
        }

        [STAThread]
        public void AssertJSDialogContent(string dialogContent, int timeoutSeconds)
        {
            dialogHandle = IntPtr.Zero;
            DateTime timeout = DateTime.Now.AddSeconds(timeoutSeconds);
            dialogHandle = FindDialog(timeout);

            PreviousRequest = "AssertJSDialogContent";

            if (dialogHandle == IntPtr.Zero)
                throw new DialogNotFoundException();

            string dialogText = GetDialogText(dialogHandle, this);

            if (dialogText == null || !dialogText.Contains(dialogContent))
                throw new AssertionFailedException(string.Format("The open javascript dialog content is not equal to \"{0}\". It is \"{1}\"", dialogContent, dialogText));
        }

        #endregion

        public void PressKeys(IdentifierType identType, string identifier, string word, string tagName)
        {
            WaitForBrowser();

            string keyCodes = "";

            char[] characters = word.ToCharArray();
            for (int charIndex = 0; charIndex < characters.Length; charIndex++)
                keyCodes += ((int)characters[charIndex]) + "-";

            if (keyCodes.Length > 0)
            {
                ChromeCommand command = new ChromeCommand("PressKeys");
                command.Arguments.Add("keyCodes", keyCodes);
                command.Arguments.Add("identifierType", identType.ToString());
                command.Arguments.Add("identifier", identifier);
                command.Arguments.Add("tagName", tagName);
                command.Arguments.Add("attributeName", "value");
                command.Arguments.Add("attributeValue", word);

                chromeHttpServer.SendMessage(command);
                ChromeResponse response = chromeHttpServer.HandleResponse(command);

                switch (response.StatusCode)
                {
                    case StatusCode.SUCCESS:
                        break;
                    case StatusCode.ELEMENTDOESNOTEXIST:
                        if (WantInformativeExceptions.GetInformativeExceptions)
                            throw new ElementNotFoundException(identifier, identType, tagName);
                        else
                            throw new ElementNotFoundException(identifier, identType);
                }
            }

            //If we do enter or space it's the same as firing an onclick event on that element so give it time to load or do its action
            if (word.Contains("ENTER") || word.Contains("SPACE"))
            {
                Thread.Sleep(200);
                if (FindDialog(DateTime.Now.AddMilliseconds(100)) == IntPtr.Zero)
                    HandleSWATTabs();
            }
        }

        public void ElementFireEvent(IdentifierType identType, string identifier, string tagName, string eventName)
        {
            WaitForBrowser(true);

            ChromeCommand command = new ChromeCommand("StimulateElement");
            command.Arguments.Add("identifierType", identType.ToString());
            command.Arguments.Add("identifier", identifier);
            command.Arguments.Add("tagName", tagName);

            if (eventName.StartsWith("on"))
                eventName = eventName.Remove(0, 2);

            command.Arguments.Add("eventName", eventName);

            ChromeResponse response = chromeHttpServer.SendMessageWithTimeout(command, DefaultTimeouts.FindElementTimeout);

            switch (response.StatusCode)
            {
                case StatusCode.SUCCESS:
                    break;
                case StatusCode.ELEMENTDOESNOTEXIST:
                    if (SWAT.WantInformativeExceptions.GetInformativeExceptions)
                        throw new ElementNotFoundException(identifier, identType, tagName);
                    else
                        throw new ElementNotFoundException(identifier, identType);
                case StatusCode.PORTDISCONNECTED:
                    throw new ChromeContentScriptIsNotConnectedException();
            }

            // Give the operating system time in case our stimulate element caused the window to close
            Sleep(100);

            if (FindDialog(DateTime.Now.AddMilliseconds(100)) == IntPtr.Zero)
                HandleSWATTabs();
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, string attributeValue)
        {
            SetElementAttribute(identType, identifier, tagName, attributeType, attributeName, attributeValue, DefaultTimeouts.FindElementTimeout);
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, string attributeValue, int timeout)
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("SetElementAttribute");
            command.Arguments.Add("identifierType", identType.ToString());
            command.Arguments.Add("identifier", identifier);
            command.Arguments.Add("attributeName", attributeName);
            command.Arguments.Add("attributeValue", attributeValue);
            command.Arguments.Add("tagName", tagName);
            ChromeResponse response = chromeHttpServer.SendMessageWithTimeout(command, timeout);

            switch (response.StatusCode)
            {
                case StatusCode.ELEMENTDOESNOTEXIST:
                    if (SWAT.WantInformativeExceptions.GetInformativeExceptions)
                        throw new ElementNotFoundException(identifier, identType, tagName);
                    else
                        throw new ElementNotFoundException(identifier, identType);

                case StatusCode.ATTRIBUTEERROR:
                    throw new AttributeErrorException(string.Format("The attribute name '{0}' was not found.", attributeName));

                case StatusCode.PORTDISCONNECTED:
                    throw new ChromeContentScriptIsNotConnectedException();
            }

            // Special Case: input type is file
            if (response.Value.Equals("file"))
            {
                SetFileInputPath(identType, identifier, attributeValue, tagName);
            }
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName)
        {
            return GetElementAttribute(identType, identifier, tagName, attributeType, attributeName, DefaultTimeouts.FindElementTimeout);
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, int timeout)
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("GetElementAttribute");
            command.Arguments.Add("identifierType", identType.ToString());
            command.Arguments.Add("identifier", identifier);
            command.Arguments.Add("attributeName", attributeName);
            command.Arguments.Add("tagName", tagName);
            ChromeResponse response = chromeHttpServer.SendMessageWithTimeout(command, timeout);

            switch (response.StatusCode)
            {
                case StatusCode.ELEMENTDOESNOTEXIST:
                    if (WantInformativeExceptions.GetInformativeExceptions)
                        throw new ElementNotFoundException(identifier, identType, tagName);
                    throw new ElementNotFoundException(identifier, identType);

                case StatusCode.ATTRIBUTEERROR:
                    throw new AttributeErrorException(string.Format("The attribute name '{0}' was not found.", attributeName));

                case StatusCode.PORTDISCONNECTED:
                    throw new ChromeContentScriptIsNotConnectedException();
            }

            return response.Value.ToString();
        }

        public override IntPtr GetContentHandle()
        {
            return curWindowHandle == IntPtr.Zero ? IntPtr.Zero : GetChromeContentHandle(contentClass, curWindowHandle);
        }

        public override object GetDocumentAttribute(string theAttributeName)
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("GetDocumentAttribute");
            command.Arguments.Add("theAttributeName", theAttributeName);

            chromeHttpServer.SendMessage(command);
            ChromeResponse response = chromeHttpServer.HandleResponse(command);

            switch (response.StatusCode)
            {
                case StatusCode.ELEMENTDOESNOTEXIST:
                    throw new ElementNotFoundException(theAttributeName, IdentifierType.Name);
            }

            return response.Value;
        }

        public override void SetDocumentAttribute(string theAttributeName, object theAttributeValue)
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("SetDocumentAttribute");
            command.Arguments.Add("theAttributeName", theAttributeName);
            command.Arguments.Add("theAttributeValue", theAttributeValue);

            chromeHttpServer.SendMessage(command);
            ChromeResponse response = chromeHttpServer.HandleResponse(command);

            switch (response.StatusCode)
            {
                case StatusCode.ELEMENTDOESNOTEXIST:
                    throw new ElementNotFoundException(theAttributeName, IdentifierType.Name);
            }
        }

        public void WaitForBrowserReadyState()
        {
            WaitForBrowser(true);
        }

        #endregion

        #region Helper Methods

        private void ConnectToChrome()
        {
            if (!chromeHttpServer.Disposed && !ChromeIsRunning)
                chromeHttpServer.Dispose();

            if (chromeHttpServer.Disposed)
                chromeHttpServer = new ChromeHttpServer(this);
        }

        private static IntPtr GetChromeWindowHandle(string lpWindowName)
        {
            IntPtr result = IntPtr.Zero;
            DateTime timeout = DateTime.Now.AddSeconds(5);
            do
            {
                NativeMethods.GetWindowWithSubstring(lpWindowName, 0, 0, ref result);
            } while (result == IntPtr.Zero && DateTime.Now < timeout);

            return result;
        }

        private static IntPtr GetChromeContentHandle(string lpClassName, IntPtr parentHndl)
        {
            IntPtr result;
            DateTime timeout = DateTime.Now.AddSeconds(5);
            do
            {
                result = NativeMethods.GetChildWindowHwnd(parentHndl, lpClassName);
            } while (result == IntPtr.Zero && DateTime.Now < timeout);

            return result;
        }

        private void SetFileInputPath(IdentifierType identType, string identifier, string filePath, string tagName)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("Could not find file {0}", filePath));
            }

            ElementFireEvent(identType, identifier, tagName, "onfocus");
            KeyboardInput keyboard = new KeyboardInput(this);
            keyboard.ProcessKey(this.getKeyValue("ENTER"));
            keyboard.SendInputString(this.GetCurrentWindowTitle());

            StringBuilder className = new StringBuilder(255);
            StringBuilder btnName = new StringBuilder(255);
            Sleep(2000);

            IntPtr dialogHwnd = IntPtr.Zero;

            for (int i = 0; i < 30; i++)
            {
                NativeMethods.GetWindowWithSubstring("Open", 0, 0, ref dialogHwnd);

                if (dialogHwnd != IntPtr.Zero)
                {   //Now find the combobox fill in box
                    List<IntPtr> windowChildren = NativeMethods.GetWindowChildren(dialogHwnd);
                    foreach (IntPtr dlgComboBox in windowChildren)
                    {
                        NativeMethods.GetClassName(dlgComboBox, className, className.MaxCapacity); //get the childs's type                    
                        if (string.Equals(className.ToString(), "ComboBoxEx32"))
                        {
                            IntPtr comboBoxTxt = NativeMethods.GetChildWindowHwnd(dlgComboBox, "Edit"); //find the combobox

                            AutomationElement editBox = AutomationElement.FromHandle(comboBoxTxt);
                            ValuePattern valuePattern = (ValuePattern)editBox.GetCurrentPattern(ValuePattern.Pattern);
                            valuePattern.SetValue(filePath);

                            break;
                        }
                    }
                    //Click on the OK Button
                    foreach (IntPtr okBtn in windowChildren)
                    {
                        NativeMethods.GetWindowText(okBtn, btnName, btnName.Capacity);
                        if (string.Equals(btnName.ToString(), "&Open")) // we found the OK Button
                        {
                            NativeMethods.RECT placement;
                            NativeMethods.GetClientRect(okBtn, out placement);
                            uint lParam = (uint)((placement.Left + 1 * 0x010000) + placement.Top + 1); //Find it's coordinates and generate the parameter

                            //click on it
                            NativeMethods.SendMessage(okBtn, 0x201, 0x000, lParam); //WM_LBUTTONDOWN
                            NativeMethods.SendMessage(okBtn, 0x202, 0x000, lParam); //WM_LBUTTONUP
                            break;
                        }
                    }
                    break;
                }
                else
                {
                    Sleep(100);
                }
            }

            if (dialogHwnd == IntPtr.Zero)
            {
                throw new Exception(string.Format("Could not find file input dialog handle"));
            }
            Sleep(100); //Time for filepath to be displayed
        }

        private enum RequestAction
        {
            Wait,
            DontWait,
            Ignore
        }

        private static RequestAction RequiredAction(string request)
        {
            switch (request)
            {
                case "WaitForBrowserReadyState":
                case "ClickJSDialog":
                case "NavigateBrowser":
                case "RefreshBrowser":
                case "RunJavaScript":
                case "StimulateElement":
                    return RequestAction.Wait;
                case "FindBrowser":
                case "KillAllOpenBrowsers":
                case "AssertBrowserIsAttached":
                    return RequestAction.Ignore;
                case "KillAll":
                    return RequestAction.DontWait;
                default:
                    return RequestAction.DontWait;
            }
        }

        private void SetRequestToWaitFor(string request)
        {
            RequestAction action = RequiredAction(request);
            if (action == RequestAction.Wait)
            {
                RequestWeAreWaitingToComplete = request;
            }
            else if (action == RequestAction.DontWait)
            {
                RequestWeAreWaitingToComplete = "";
            }
        }

        private void WaitForBrowser()
        {
            WaitForBrowser(false);
        }

        private void WaitForBrowser(bool forceWait)
        {
            if (forceWait)
                PreviousRequest = "WaitForBrowserReadyState";

            if (WaitIsRequired)
            {
                DateTime timeout = DateTime.Now.AddSeconds(DefaultTimeouts.WaitForBrowserTimeout);
                WaitForBrowser(timeout);
            }
        }

        private void WaitForBrowser(DateTime timeout)
        {
            if (DateTime.Now < timeout)
            {
                ChromeCommand command = new ChromeCommand("CheckTabStatus");
                chromeHttpServer.SendMessage(command);
                ChromeResponse response = chromeHttpServer.HandleResponse(command);

                switch (response.StatusCode)
                {
                    case StatusCode.SUCCESS:
                        break;
                    case StatusCode.LOADING:
                        Thread.Sleep(100);
                        WaitForBrowser(timeout);
                        break;
                }
            }
            return;
        }

        private int GetAttachedTabId()
        {
            ChromeResponse response = IsBrowserAttached();

            if (response.StatusCode == StatusCode.SUCCESS)
            {
                return int.Parse(response.Value.ToString());
            }

            return -1;
        }

        private ChromeResponse IsBrowserAttached()
        {
            ChromeCommand command = new ChromeCommand("AssertBrowserIsAttached");
            chromeHttpServer.SendMessage(command);
            return chromeHttpServer.HandleResponse(command);
        }

        #endregion

        protected override string runJavaScript(string theScript)
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("RunJavaScript");

            theScript = theScript.Replace("\"", "&quot;");

            command.Arguments.Add("theScript", theScript);

            chromeHttpServer.SendMessage(command);
            ChromeResponse response = chromeHttpServer.HandleResponse(command);

            if (response.Value.ToString() == "undefined")
                response.Value = "";

            return response.Value.ToString();
        }

        public override void AssertBrowserIsAttached()
        {
            if (curWindowHandle == IntPtr.Zero)
                throw new NoAttachedWindowException();

            ChromeResponse response = IsBrowserAttached();

            if (response.StatusCode == StatusCode.NOATTACHEDWINDOW)
            {
                curWindowHandle = IntPtr.Zero;
                throw new NoAttachedWindowException();
            }
        }

        public void AssertElementIsActive(IdentifierType identType, string identifier, string tagName, int timeoutSeconds)
        {
            WaitForBrowser();

            ChromeCommand command = new ChromeCommand("AssertElementIsActive");
            command.Arguments.Add("identifierType", identType.ToString());
            command.Arguments.Add("identifier", identifier);
            command.Arguments.Add("tagName", tagName);
            ChromeResponse response = chromeHttpServer.SendMessageWithTimeout(command, timeoutSeconds);

            if (response.StatusCode == StatusCode.ELEMENTDOESNOTEXIST)
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotFoundException(identifier, identType, tagName);

                throw new ElementNotFoundException(identifier, identType);
            }

            if (response.StatusCode == StatusCode.ELEMENTNOTACTIVE)
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotActiveException(identifier, identType, tagName);

                throw new ElementNotActiveException(identifier, identType);
            }

            if (response.StatusCode == StatusCode.PORTDISCONNECTED)
                throw new ChromeContentScriptIsNotConnectedException();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.

                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                chromeHttpServer.Dispose();
                chromeHttpServer = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion
    }
}
