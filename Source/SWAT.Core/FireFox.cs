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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SWAT
{
    public class FireFox : Browser, IBrowser, IDisposable
    {
        #region Class Variables

        private Uri _currentURI;
        private string currentlyAttachedFirefoxWindowGuid;
        private string _lastAttachWindowTitle;
        private const int NULL_PROCESS_ID = -1;
        public int _processId = NULL_PROCESS_ID;
        private bool disposed;
        private string _firefoxVersion;
        private bool _lastWasConfirmDialog;

        #endregion

        #region Constructor

        public FireFox() : base(BrowserType.FireFox, BrowserProcess.firefox)
        {
        }

        #endregion

        #region Miscellaneous Methods

        private void WriteToConsole(string message)
        {
            string formattedMessage = string.Format("({0}, {1}) - {2}", DateTime.Now, curWindowHandle, message);
            Debug.WriteLine(formattedMessage);
        }

        public void OpenBrowser()
        {
            //Start a firefox browser with JSSH

            string path = BrowserPaths.FirefoxRootDirectory;

            if (string.IsNullOrEmpty(path)) //not found in the registry
                throw new BrowserNotInstalledException("Firefox is not installed.");
            if (!File.Exists(path))
                throw new IllegalDirectoryException(string.Format("Firefox was not found in {0}", path));

            WriteToConsole("Opening new Firefox window..");

            Process p = Process.Start(path, "about:config -jssh");
            p.WaitForInputIdle();
            
            Thread.Sleep(1000); // wait for Firefox to actually start...

            if (_firefoxVersion == null)
                _firefoxVersion = FileVersionInfo.GetVersionInfo(path).ProductVersion;

            //writeToConsole("Found firefox window, attempting to establish connection..");

            using (JSSHConnection jssh = GetExtensionConnection(false))
            {
                DateTime timeout = DateTime.Now.AddSeconds(60);
                while (!jssh.ConnectToJSSH())
                {
                    if (DateTime.Now > timeout)
                        throw new BrowserDidNotLoadException("Firefox did not load properly");
                }
            }

            //writeToConsole("Connected, attempting to attach to browser");
            AttachToBrowserWindow("about:config", DefaultTimeouts.AttachToWindowBrowserTimeout, 0);
        }

        public void NavigateBrowser(string url)
        {
            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                jssh.SendMessage(
                    string.Format("browser.loadURI(\"{0}\");print('SwatCommandCompleted Navigate Browser');", url));
                WaitForBrowserReadyState();
                setCurrentURI();
            }
        }

        private void setCurrentURI()
        {
            if (!isJSDialogShowing)
            {
                setTheUri();
            }
        }

        private void setTheUri()
        {
            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                WaitForBrowserReadyState();

                _lastAttachWindowTitle = jssh.SendMessage("print(firefoxWindow.document.title)");

                string temp = jssh.SendMessage("print(browser.currentURI.spec)");

                try
                {
                    Uri newUri = new Uri(temp);

                    //if we didnt fail we have a good Uri
                    _currentURI = newUri;
                }
                catch
                {
                }
            }
        }

        public string GetCurrentLocation()
        {
            setCurrentURI();
            return _currentURI != null ? _currentURI.ToString() : "Current URI is not set";
        }

        public string GetCurrentDocumentTitle()
        {
            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                setCurrentURI();

                StringBuilder msg = new StringBuilder();
                msg.Append("var elem=doc.title;");
                msg.Append("if(elem != null){print(elem);}");
                string docTitle = jssh.SendMessage(msg.ToString());
                return docTitle;
            }
        }

        public string GetCurrentWindowTitle()
        {
            setCurrentURI();
            return _lastAttachWindowTitle;
        }

        public void WaitForBrowserReadyState()
        {
            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("var done = true;");
                msg.Append("for (var i = 0; i < firefoxWindow.frames.length && done; i++)");
                msg.Append("{ if (firefoxWindow.frames[i].document.readyState != 'complete') {");
                msg.Append("done = false; } }");
                msg.Append("done = done && browser.webProgress.busyFlags == 0;");
                msg.Append("print(done);");

                DateTime end = DateTime.Now.AddSeconds(DefaultTimeouts.WaitForBrowserTimeout);
                while (DateTime.Now < end)
                {
                    if (dialogWatcher.FoundDialog)
                        return;
                    using (JSSHConnection jssh = GetExtensionConnection(false))
                    {
                        string dialogOpenResult = jssh.SendMessage("print(document.activeElement.label);", true, false);
                        bool dialogIsOpen = dialogOpenResult.Contains("OK") || dialogOpenResult.Contains("Cancel");
                        if (dialogIsOpen)
                            return;
                    }
                    using (JSSHConnection jssh = GetExtensionConnection(true))
                    {
                        string result = jssh.SendMessage(msg.ToString());
                        string title = jssh.SendMessage("print(firefoxWindow.document.title);");
                        if (result == "true" || title.Contains("Problem loading page") ||
                            title.Contains("(application/pdf Object)") || !IsSuccessfulResult(result))
                            break;
                    }
                    Thread.Sleep(0);
                }
            }
        }


        public void CloseBrowser()
        {
            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                WriteToConsole("Closing browser window...");
                jssh.SendMessage("firefoxWindow.close();print('SwatCommandCompleted Close Browser');");
                WaitForWindowToClose();
            }
        }

        private void WaitForBrowserWindow()
        {
            switch (lastBrowserWindowAction)
            {
                case LastBrowserWindowAction.Closed:
                    WaitForWindowToClose();
                    break;
                case LastBrowserWindowAction.Killed:
                    WaitForProcessesToEnd();
                    break;
                case LastBrowserWindowAction.KilledExceptTitle:
                    WaitForRemainingWindowsToClose();
                    break;
                case LastBrowserWindowAction.Refreshed:
                    WaitForWindowToRefresh();
                    break;
                default:
                    WaitForBrowserReadyState();
                    break;
            }
        }

        private void WaitForWindowToClose()
        {
            lastBrowserWindowAction = LastBrowserWindowAction.Closed;
            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                WriteToConsole("Waiting for browser window to finish closing...");
                DateTime timeout = DateTime.Now.AddSeconds(30);
                try
                {
                    //if the browser closes check and make sure that the processId does not exist anymore or has Exited need this in a try catch because it throws an exception once the process has terminated
                    while (NativeMethods.IsWindow(curWindowHandle) && DateTime.Now < timeout)
                    {
                        if (dialogWatcher.FoundDialog)
                        {
                            return;
                        }
                        Thread.Sleep(0); //! 250               
                    }
                }
                catch
                {
                }

                if (NativeMethods.GetNumberOfWindowsWithSubString("- Mozilla Firefox") == 0)
                {
                    while (Process.GetProcessesByName("firefox").Length > 0)
                    {
                        Thread.Sleep(0); //! 1000
                    }
                }
            }
            lastBrowserWindowAction = LastBrowserWindowAction.InUse;

            if (Process.GetProcessesByName("firefox").Length == 0)
            {
                ClearAttachedWindowVariables();
            }

            Thread.Sleep(500); // needed to prevent firefox from showing popup when reopening
        }

        private void WaitForProcessesToEnd()
        {
            WaitForProcessesToEnd(true);
        }

        private void WaitForProcessesToEnd(bool killingProcess)
        {
            lastBrowserWindowAction = LastBrowserWindowAction.Killed;
            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                WriteToConsole("Waiting for the Firefox process to completely close...");

                DateTime timeout = DateTime.Now.AddSeconds(7.5);
                while ((Process.GetProcessesByName("firefox").Length > 0) && DateTime.Now < timeout)
                {
                    if (dialogWatcher.FoundDialog && !killingProcess)
                        return;
                    Thread.Sleep(1);
                }
            }
            lastBrowserWindowAction = LastBrowserWindowAction.InUse;

            if (Process.GetProcessesByName("firefox").Length == 0)
            {
                ClearAttachedWindowVariables();
            }
            Thread.Sleep(500); // needed to prevent firefox from showing popup when reopening
        }

        public void KillAllOpenBrowsers()
        {
            string procName = BrowserProcess.firefox.ToString();
            Process[] processes = Process.GetProcessesByName(procName);
            WriteToConsole("Killing the Firefox process...");
            foreach (Process p in processes)
                p.Kill();
            WaitForProcessesToEnd();
            ClearAttachedWindowVariables();
        }

        private static string GetKillWindowsCommandString(string windowTitleLowerCase)
        {
            StringBuilder killCommand = new StringBuilder("var openWindows = getWindows(); ");
            killCommand.Append("var found = false;");
            killCommand.Append("for (i = 0; i < openWindows.length; i++) {");
            killCommand.Append("if (openWindows[i].document.title.toLowerCase().match('" + windowTitleLowerCase +
                               "')) {");
            killCommand.Append("found = true; } }");
            killCommand.Append("print(found);");
            killCommand.Append("for(i = 0; i < openWindows.length; i++) {");
            killCommand.Append("if((openWindows[i].document.title.toLowerCase().match('" + windowTitleLowerCase +
                               "')) != '" + windowTitleLowerCase + "') {");
            killCommand.Append("openWindows[i].close(); } }");
            return killCommand.ToString();
        }

        private static string GetWaitForWindowsToCloseCommandString(string windowTitleLowerCase)
        {
            StringBuilder waitCommand = new StringBuilder("var openWindows = getWindows(); ");
            waitCommand.Append("for(i = 0; i < openWindows.length; i++) {");
            waitCommand.Append("if((openWindows[i].document.title.toLowerCase().match('" + windowTitleLowerCase +
                               "')) != '" + windowTitleLowerCase + "') {");
            waitCommand.Append("print('windowExists'); }}");
            return waitCommand.ToString();
        }

        public void KillAllOpenBrowsers(string windowTitle)
        {
            if (Process.GetProcessesByName(BrowserProcess.firefox.ToString()).Length > 0)
            {
                previousLowerCaseWindowTitleToKeepOpen = windowTitle.ToLower();
                string killWindowsCommandString = GetKillWindowsCommandString(previousLowerCaseWindowTitleToKeepOpen);
                string result;
                using (JSSHConnection jssh = GetExtensionConnection(true))
                {
                    result = jssh.SendMessage(killWindowsCommandString);
                }
                if (result.Contains("false"))
                    WaitForProcessesToEnd(false);
                else
                    WaitForRemainingWindowsToClose();
            }
        }

        public void RefreshBrowser()
        {
            WriteToConsole("Refreshing browser window...");
            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                jssh.SendMessage("browser.reload();print('SwatCommandCompleted Refresh Browser');");
                WaitForWindowToRefresh();
            }
        }

        private void WaitForWindowToRefresh()
        {
            WriteToConsole("Waiting for browser window to finish refreshing...");
            lastBrowserWindowAction = LastBrowserWindowAction.Refreshed;
            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                WaitForBrowserReadyState();
                if (dialogWatcher.FoundDialog)
                {
                    return;
                }
            }
            lastBrowserWindowAction = LastBrowserWindowAction.InUse;
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        private static IntPtr GetContentServerHandle(IntPtr parentHwnd)
        {
            StringBuilder className = new StringBuilder(255);

            NativeMethods.GetClassName(parentHwnd, className, className.MaxCapacity); //get the parent's type
            if (string.Equals(className.ToString(), "MozillaContentWindowClass") || parentHwnd == IntPtr.Zero)
                return parentHwnd;
            List<IntPtr> children = GetWindowChildren(parentHwnd); // get the children
            IntPtr child = IntPtr.Zero;
            foreach (IntPtr t in children)
            {
                child = GetContentServerHandle(t);
                NativeMethods.GetClassName(child, className, className.MaxCapacity);
                //get the childs's type                    
                if (string.Equals(className.ToString(), "MozillaContentWindowClass") || t == IntPtr.Zero) break;
            }
            return child == IntPtr.Zero ? parentHwnd : child;
        }

        private static List<IntPtr> GetWindowChildren(IntPtr parentHwnd)
        {
            List<IntPtr> children = new List<IntPtr>();
            IntPtr retrievedChild = NativeMethods.GetWindow(parentHwnd, NativeMethods.GW_CHILD);

            while (!retrievedChild.Equals(IntPtr.Zero))
            {
                children.Add(retrievedChild);
                retrievedChild = NativeMethods.GetWindow(retrievedChild, NativeMethods.GW_HWNDNEXT);
                // get a child's sibling
            }

            return children;
        }

        public void AssertBrowserExists(string windowTitle)
        {
            findBrowser(windowTitle, DefaultTimeouts.AssertBrowserExists, true);
        }

        public void AssertBrowserExists(string windowTitle, double timeOut)
        {
            findBrowser(windowTitle, (int) Math.Ceiling(timeOut/1000), true);
        }

        public void AssertBrowserDoesNotExist(string windowTitle)
        {
            findBrowser(windowTitle, DefaultTimeouts.AssertBrowserExists, false);
        }

        public void AssertBrowserDoesNotExist(string windowTitle, double timeOut)
        {
            findBrowser(windowTitle, (int) Math.Ceiling(timeOut/1000), false);
        }

        private void findBrowser(string windowTitle, int timeOut, bool expectPositiveResult)
        {
            windowTitle = escapeChars(windowTitle).ToLower();

            StringBuilder msg = new StringBuilder("popUpwindow = null; i = 0; ");
            msg.Append("var windows = getWindows();");
            msg.Append("for(i = 0;i < windows.length;i++){");
            msg.Append("var windowTitle = windows[i].document.title.toLowerCase();");
            msg.Append("if(windowTitle.indexOf('" + windowTitle + "') > -1){");
            msg.Append("print('windowExists'); break;}}");

            string message = msg.ToString();
            string result = string.Empty;

            DateTime timeout = DateTime.Now.AddSeconds(timeOut);

            while (DateTime.Now < timeout)
            {
                using (JSSHConnection jssh = GetExtensionConnection(true))
                {
                    if (jssh.ConnectToJSSH())
                    {
                        result = jssh.SendMessage(message);
                    }
                }

                Thread.Sleep(0);

                if (expectPositiveResult && result.Contains("windowExists")) //AssertBrowserExists
                {
                    return;
                }
                if (!expectPositiveResult && !result.Contains("windowExists")) //AssertBrowserDoesNotExist
                {
                    return;
                }
            }

            if (!expectPositiveResult && result.Contains("windowExists")) //AssertBrowserDoesNotExist
            {
                throw new BrowserExistException(string.Format("There is a browser with title \"{0}\" open.",
                                                                windowTitle));
            }
            if (expectPositiveResult) //AssertBrowserExists
            {
                throw new BrowserExistException(string.Format("There is no browser with title \"{0}\" open.",
                                                                windowTitle));
            }
        }

        #endregion

        #region Element Processing Methods

        public void AssertElementIsActive(IdentifierType identType, string identifier, string tagName,
                                          int timeoutSeconds)
        {
            setCurrentURI();

            StringBuilder msg = new StringBuilder();
            msg.Append("var activeElemFound = false;");
            buildElementFindCommands(identType, identifier, tagName, msg);
            msg.Append("var contDoc, actElem; var resultMessage = '';");
            msg.Append("if (elem != null) { resultMessage = 'elem found';");
            msg.Append("contDoc = elem.ownerDocument; actElem = contDoc.activeElement;");
            msg.Append("if (elem == actElem){ activeElemFound = true; } } ");
            msg.Append("resultMessage = resultMessage + activeElemFound;");
            msg.Append("print(resultMessage);");

            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                string result;
                DateTime timeout = DateTime.Now.AddSeconds(timeoutSeconds);

                do
                {
                    result = jssh.SendMessage(msg.ToString());
                } while (!result.Contains("true") && DateTime.Now < timeout);

                if (result.Contains("false") && result.Contains("elem found"))
                {
                    if (WantInformativeExceptions.GetInformativeExceptions)
                        throw new ElementNotActiveException(identifier, identType, tagName);
                    throw new ElementNotActiveException(identifier, identType);
                }
                if (result.Contains("false"))
                {
                    if (WantInformativeExceptions.GetInformativeExceptions)
                        throw new ElementNotFoundException(identifier, identType, tagName);
                    throw new ElementNotFoundException(identifier, identType);
                }
            }
        }

        public void StimulateElement(IdentifierType identType, string identifier, string elementTagName,
                                     string eventType)
        {
            StimulateElement(identType, identifier, elementTagName, eventType, DefaultTimeouts.FindElementTimeout);
        }

        public void StimulateElement(IdentifierType identType, string identifier, string elementTagName,
                                     string eventType, int waitTime)
        {
            setCurrentURI();
            bool foundElement = false;
            identifier = escapeChars(identifier);
            DateTime timeout = DateTime.Now.AddSeconds(waitTime);
            while (DateTime.Now < timeout)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("var elem = null;");

                buildElementFindCommands(identType, identifier, elementTagName, msg);

                if (eventType.Equals("change") || eventType.Equals("blur") || eventType.Equals("select"))
                    msg.Append("if(elem != null){ var event = doc.createEvent(\"HTMLEvents\"); event.initEvent('" +
                               eventType +
                               "',true,true);elem.dispatchEvent(event);print('SwatCommandCompleted Stimulate Element');}else{ print('failed');}");
                else if (eventType.ToLower().Equals("keyup") || eventType.ToLower().Equals("keydown") ||
                         eventType.ToLower().Equals("keypress"))
                    msg.Append(
                        "if(elem != null){ var event = doc.createEvent(\"KeyEvents\"); event.initKeyEvent('" +
                        eventType +
                        "',true, true, null, false, false, false, false, 9, 0);elem.dispatchEvent(event);print('SwatCommandCompleted Stimulate Element');}else{ print('failed');}");
                else if (eventType.ToLower().Equals("focus") && !elementTagName.ToLower().Equals("option"))
                    msg.Append(
                        "if(elem != null){ elem.focus(); print('SwatCommandCompleted Stimulate Element'); }else{ print('SwatCommandCompleted Stimulate Element failed');}");
                else
                    msg.Append(
                        "if(elem != null){var event = doc.createEvent(\"MouseEvents\"); event.initMouseEvent('" +
                        eventType +
                        "', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null); print('SwatCommandCompleted Stimulate Element'); elem.dispatchEvent(event);  }else{ print('failed');}");

                string result;
                using (JSSHConnection jssh = GetExtensionConnection(true))
                {
                    result = jssh.SendMessage(msg.ToString()).Trim();
                }

                if (!IsSuccessfulResult(result))
                    Thread.Sleep(100); //! 1000
                else
                {
                    foundElement = true;
                    break;
                }
            }

            if (!foundElement)
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotFoundException(identifier, identType, elementTagName);
                throw new ElementNotFoundException(identifier, identType);
            }
            if (!_firefoxVersion.StartsWith("3"))
                Thread.Sleep(400);
        }

        private static void buildKeyDownEvent(StringBuilder msg, int keyCode, int charCode)
        {
            msg.Append("var keyDownEvent = doc.createEvent('KeyboardEvent'); ");
            msg.Append("keyDownEvent.initKeyEvent('keydown', true, true, null, false, false, false, false, " + keyCode +
                       ", " + charCode + ");");
            msg.Append("success = elem.dispatchEvent(keyDownEvent); ");
        }

        private static void buildKeyPressEvent(StringBuilder msg, int keyCode, int charCode)
        {
            msg.Append("var keyPressEvent = doc.createEvent('KeyboardEvent'); ");
            msg.Append("keyPressEvent.initKeyEvent('keypress', true, true, null, false, false, false, false, " + keyCode +
                       ", " + charCode + ");");
            msg.Append("success = elem.dispatchEvent(keyPressEvent); ");
        }

        private static void buildKeyUpEvent(StringBuilder msg, int keyCode, int charCode)
        {
            msg.Append("var keyUpEvent = doc.createEvent('KeyboardEvent'); ");
            msg.Append("keyUpEvent.initKeyEvent('keyup', true, true, null, false, false, false, false, " + keyCode +
                       ", " + charCode + ");");
            msg.Append("success = elem.dispatchEvent(keyUpEvent);");
        }

        private static void buildKeyEvents(StringBuilder msg, int keyCode)
        {
            msg.Append("var success = true;");
            buildKeyDownEvent(msg, keyCode, keyCode);
            buildKeyPressEvent(msg, 0, keyCode);
            buildKeyUpEvent(msg, keyCode, keyCode);
            msg.Append("print(success);");
        }

        public void PressKeys(IdentifierType identType, string identifier, string word, string tagName)
        {
            setCurrentURI();

            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                StringBuilder msg = new StringBuilder();
                buildElementFindCommands(identType, identifier, tagName, msg);

                msg.Append(
                    "if (elem) { print('SwatCommandCompleted Press Keys'); } else {print('SwatCommandCompleted Press Keys failed');}");
                bool foundElement = false;
                DateTime timeout = DateTime.Now.AddSeconds(DefaultTimeouts.FindElementTimeout);

                while (DateTime.Now < timeout)
                {
                    string result = jssh.SendMessage(msg.ToString());
                    if (IsSuccessfulResult(result))
                    {
                        foundElement = true;
                        break;
                    }
                }

                if (!foundElement)
                {
                    if (WantInformativeExceptions.GetInformativeExceptions)
                        throw new ElementNotFoundException(identifier, identType, tagName);
                    throw new ElementNotFoundException(identifier, identType);
                }

                msg = new StringBuilder();
                char[] characters = word.ToCharArray();
                foreach (char t in characters)
                {
                    int keyCode = t;
                    buildKeyEvents(msg, keyCode);
                    jssh.SendMessage(msg.ToString());
                    msg = new StringBuilder();
                }
            }
        }

        public void IsMatchMethod(object value, ExpressionToken token, IsMatchResult isMatchResult)
        {
            if (isMatchResult.ReturnValue != null && isMatchResult.ToString() != string.Empty)
                isMatchResult.ReturnValue += " && ";

            //Custom attribute if searching by style
            string tokenAttribute = token.Attribute.Replace("parentElement", "parentNode").Replace("parentWindow",
                                                                                                   "ownerDocument.defaultView");
            string tokenValue = token.Value;

            if (tokenAttribute.Equals("style", StringComparison.OrdinalIgnoreCase))
            {
                if (tokenValue.Contains("#"))
                    tokenValue = HexToDecimal(tokenValue);

                tokenAttribute += ".cssText";
            }

            string parentString = "";
            while (tokenAttribute.StartsWith("parentNode."))
            {
                tokenAttribute = tokenAttribute.Remove(0, "parentNode.".Length);
                parentString += ".parentNode";
            }

            tokenAttribute = AttributeNormalizer.Normalize(tokenAttribute);

            if (tokenValue == "")
                tokenValue = "[]*";

            if (!string.IsNullOrEmpty(tokenValue))
            {
                if (token.MatchType == MatchType.Contains)
                {
                    if (token.ExpectedMatchCount != int.MinValue)
                    {
                        isMatchResult.ReturnValue +=
                            string.Format(
                                " ((elems[a]" + parentString + " != null && elems[a]" + parentString +
                                ".hasAttributes() == true && elems[a]" + parentString +
                                ".getAttribute('{0}') != null && elems[a]" + parentString +
                                ".getAttribute('{0}').toString().match(/{1}/gim) != null && elems[a]" + parentString +
                                ".getAttribute('{0}').toString().match(/{1}/gim).length == {2}) || (elems[a]" +
                                parentString + " != null && elems[a]" + parentString + ".{0} != null && elems[a]" +
                                parentString + ".{0}.toString().match(/{1}/gim) != null && elems[a]" + parentString +
                                ".{0}.toString().match(/{1}/gim).length == {2}))", tokenAttribute, tokenValue,
                                token.ExpectedMatchCount);
                    }
                    else
                    {
                        isMatchResult.ReturnValue +=
                            string.Format(
                                " ((elems[a]" + parentString + " != null && elems[a]" + parentString +
                                ".hasAttributes() == true && elems[a]" + parentString +
                                ".getAttribute('{0}') != null && elems[a]" + parentString +
                                ".getAttribute('{0}').toString().match(/{1}/gim) != null && elems[a]" + parentString +
                                ".getAttribute('{0}').toString().match(/{1}/gim).length >= 0) || (elems[a]" +
                                parentString + " != null && elems[a]" + parentString + ".{0} != null && elems[a]" +
                                parentString + ".{0}.toString().match(/{1}/gim) != null && elems[a]" + parentString +
                                ".{0}.toString().match(/{1}/gim).length >= 0))", tokenAttribute, tokenValue);
                    }
                }
                else
                {
                    isMatchResult.ReturnValue +=
                        string.Format(
                            " ((elems[a]" + parentString + " != null && elems[a]" + parentString +
                            ".hasAttributes() == true && elems[a]" + parentString +
                            ".getAttribute('{0}') != null && elems[a]" + parentString +
                            ".getAttribute('{0}').toString().match(/{1}/gim) != null && elems[a]" + parentString +
                            ".getAttribute('{0}').toString().match(/{1}/gim)[0].length == elems[a]" + parentString +
                            ".getAttribute('{0}').toString().length) || (elems[a]" + parentString +
                            " != null && elems[a]" + parentString + ".{0} != null && elems[a]" + parentString +
                            ".{0}.toString().match(/{1}/gim) != null && elems[a]" + parentString +
                            ".{0}.toString().match(/{1}/gim)[0].length == elems[a]" + parentString +
                            ".{0}.toString().length))", tokenAttribute, tokenValue, token.Value.Length);
                }
            }
            else
            {
                isMatchResult.ReturnValue +=
                    string.Format(
                        " ((elems[a]" + parentString + " != null && elems[a]" + parentString +
                        ".hasAttributes() == true && elems[a]" + parentString +
                        ".getAttribute('{0}') != null && elems[a]" + parentString +
                        ".getAttribute('{0}').toString() != null && elems[a]" + parentString +
                        ".getAttribute('{0}').toString() == '') || (elems[a]" + parentString + " != null && elems[a]" +
                        parentString + ".{0} != null && elems[a]" + parentString + ".{0}.toString() != null && elems[a]" +
                        parentString + ".{0}.toString() == '')) ", tokenAttribute, tokenValue, token.Value.Length);
            }
            isMatchResult.ContinueChecking = true;
        }

        private static string HexToDecimal(string value)
        {
            try
            {
                while (value.Contains("#"))
                {
                    int start = value.IndexOf("#");
                    string hexNum = value.Substring(start, 7);
                    int red = int.Parse(hexNum.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    int green = int.Parse(hexNum.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    int blue = int.Parse(hexNum.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    string decConverted = string.Format("rgb\\({0}, {1}, {2}\\)", red, green, blue);
                    value = value.Replace(hexNum, decConverted);
                }
            }
            catch (Exception e)
            {
                //Input string was not in a correct format exception
                throw new Exception("Error in HexToDecimal: " + e);
            }

            return value;
        }


        // TODO: METHOD IS UNUSED!!!
/*
  private void setElementProperty(IdentifierType identType, string identifier, string elementTagName, AttributeType attrType, string propertyName, string propertyValue)
  {

  setElementProperty(identType, identifier, elementTagName, attrType, propertyName, propertyValue, DefaultTimeouts.FindElementTimeout);
  }
*/

        private void setElementProperty(IdentifierType identType, string identifier, string elementTagName,
                                        AttributeType attrType, string propertyName, string propertyValue, int waitTime)
        {
            propertyValue = propertyValue.Replace("\\", "\\\\");
            // escape backslash character when explicitly typed in field

            setCurrentURI();

            string elementType = getElementProperty(identType, identifier, elementTagName, attrType, "type", waitTime);

            if (elementType != null && elementType.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                if (propertyName.Equals("value", StringComparison.OrdinalIgnoreCase) && !File.Exists(propertyValue))
                    throw new FileNotFoundException(string.Format("Could not find file {0}", propertyValue));
            }

            bool foundElement = false;

            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                StringBuilder msg = new StringBuilder();
                buildElementFindCommands(identType, identifier, elementTagName, msg);

                if (propertyValue.Contains("'")) //escape single quotes in value assignment to avoid javascript error.
                    propertyValue = propertyValue.Replace("'", @"\'");

                if (!propertyValue.Equals("true") && !propertyValue.Equals("false"))
                    propertyValue = propertyValue.Insert(0, "'") + "'";
                else
                    propertyValue = propertyValue.ToLower();

                string temp = propertyValue.Replace("\\", "");
                temp = fixIllegalChars(propertyValue, 127, 65536);

                if (!propertyValue.Equals(temp, StringComparison.OrdinalIgnoreCase) &&
                    propertyName.Equals("value", StringComparison.OrdinalIgnoreCase) &&
                    elementTagName.Equals("select", StringComparison.OrdinalIgnoreCase))
                    setSelectBoxByIndex(ref msg, temp);
                else
                    msg.Append("if(elem != null){print('SwatCommandCompleted Set Element'); elem." + propertyName +
                               " = " + propertyValue.Replace("\r", @"\r").Replace("\n", @"\n") +
                               "}else{ print('SwatCommandCompleted Set Element failed');}");

                DateTime timeout = DateTime.Now.AddSeconds(waitTime);
                while (DateTime.Now < timeout)
                {
                    string result = jssh.SendMessage(msg.ToString());

                    if (IsSuccessfulResult(result))
                    {
                        foundElement = true;
                        break;
                    }
                }

                if (!foundElement)
                {
                    if (WantInformativeExceptions.GetInformativeExceptions)
                        throw new ElementNotFoundException(identifier, identType, elementTagName);
                    throw new ElementNotFoundException(identifier, identType);
                }
            }

            if (!_firefoxVersion.StartsWith("3"))
                Thread.Sleep(400);
        }

        private static void setSelectBoxByIndex(ref StringBuilder msg, string propertyValue)
        {
            msg.Append(
                "var index = -1; if (elem != null) { for (var x = 0; x < elem.options.length; x++){ if (elem.options[x].value.match(new RegExp(" +
                propertyValue + ",'i'))) index = x;}");
            msg.Append(
                "if (index > -1) {elem.selectedIndex = index;} else {print('failed');} } else { print('failed');}");
        }

        private string getElementProperty(IdentifierType identType, string identifier, string elementTagName,
                                          AttributeType attrType, string propertyName, int waitTime)
        {
            setCurrentURI();
            bool foundElement = false;
            string result;
            const string resultPrefix = "##SWATResult##";
            const string emptyAttributeValue = "##EMPTYSWATATTRIBUTE##";

            StringBuilder findElementCommand = new StringBuilder();
            buildElementFindCommands(identType, identifier, elementTagName, findElementCommand);

            findElementCommand.Append("if(elem != null){");
            if (attrType == AttributeType.Custom)
            {
                findElementCommand.Append("var attributeValue = elem.getAttribute('" + propertyName +
                                          "'); if (!attributeValue) attributeValue = ''; print('" + resultPrefix +
                                          "' + attributeValue");
            }
            else
            {
                findElementCommand.Append("print('" + resultPrefix + "' + elem." + propertyName);
                if (propertyName.StartsWith("style"))
                {
                    findElementCommand.Append(".cssText");
                }
            }

            findElementCommand.Append(");}else{print('failed');}");
            DateTime timeout = DateTime.Now.AddSeconds(waitTime);

            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                do
                {
                    result = jssh.SendMessage(findElementCommand.ToString());
                    if (result.Contains(resultPrefix))
                    {
                        foundElement = true;
                        result = result.Replace(resultPrefix, "").Replace(emptyAttributeValue, "");
                        break;
                    }
                } while (DateTime.Now < timeout);
            }
            if (!foundElement)
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotFoundException(identifier, identType, elementTagName);
                throw new ElementNotFoundException(identifier, identType);
            }

            return result;
        }

        /* TODO: UNUSED METHOD!!!
        private void buildFlickerElement(StringBuilder msg)
        {
            //flicker element
            msg.Append("var origColor = '';");
            msg.Append("if(elem != null){origColor = elem.style.backgroundColor;if(origColor == null){origColor = '';} elem.style.backgroundColor = 'yellow';}");
        }
        */

        private static string escapeChars(string ident)
        {
            //ident = fixIllegalChars(ident, 127, 159);
            ident = fixIllegalChars(ident, 127, 65536);
            Regex reg = new Regex("(?<!\\\\)/");
            ident = reg.Replace(ident, "\\/");
            //reg = new System.Text.RegularExpressions.Regex(""); //This was a fix for the bug that firefox had with single quotes.
            //ident = reg.Replace(ident, ".").ToString();
            reg = new Regex("(?<!\\\\)'");
            return reg.Replace(ident, "\\'");
        }

        private static string fixIllegalChars(string identifier, int from, int to)
        {
            char[] chars = identifier.ToCharArray();
            StringBuilder sb = new StringBuilder(identifier);
            for (int x = 0; x < chars.Length; x++)
            {
                if (chars[x] > from && chars[x] < to)
                {
                    sb.Replace(chars[x].ToString(), @"\u" + String.Format("{0:X4}", (int) chars[x]));
                }
            }

            return sb.ToString();
        }

        private static void buildElementFindHelper(string searchStr, StringBuilder msg)
        {
            msg.Append(searchStr);
            msg.Append("var found = false;");
            //make sure the window object is defined correctly
            msg.Append(
                "var window = null; for(var i=0; i < firefoxWindow.frames.length; i++){if(firefoxWindow.frames[i].toString().toLowerCase().indexOf('object window') > -1){window = firefoxWindow.frames[i]; break;}}");
            msg.Append("function recursiveSearch(frames){ for(var i=0; i<frames.length; i++){" +
                       searchStr.Replace("doc.", "frames[i].document.") +
                       " if(elem){found = true; return;} else{ if(frames[i].frames.length>0){recursiveSearch(frames[i].frames);}}}}");
            msg.Append("if(!elem && window.frames.length > 0){ recursiveSearch(window.frames); }");
        }

        private void buildElementFindCommands(IdentifierType identType, string identifier, string elementTagName,
                                              StringBuilder msg)
        {
            msg.Append("var elem = null;");
            identifier = escapeChars(identifier);
            string searchStr;
            switch (identType)
            {
                case IdentifierType.Id:
                    buildElementFindCommands(IdentifierType.Expression, "id=" + identifier, elementTagName, msg);
                    break;

                case IdentifierType.Name:
                    buildElementFindCommands(IdentifierType.Expression, "name=" + identifier, elementTagName, msg);
                    break;

                case IdentifierType.InnerHtml:
                    searchStr =
                        string.Format(
                            "var elems = doc.getElementsByTagName('{0}'); for(var a=0;a < elems.length;a++){{ if(elems[a].innerHTML != null && elems[a].innerHTML.toString().toLowerCase() == '{1}')elem = elems[a];}}",
                            elementTagName, identifier.ToLower());
                    buildElementFindHelper(searchStr, msg);
                    break;

                case IdentifierType.InnerHtmlContains:
                    searchStr =
                        string.Format(
                            "var elems = doc.getElementsByTagName('{0}'); for(var a=0;a < elems.length;a++){{ if(elems[a].innerHTML != null && elems[a].innerHTML.toString().toLowerCase().indexOf('{1}') > -1)elem = elems[a]; }}",
                            elementTagName, identifier.ToLower());
                    buildElementFindHelper(searchStr, msg);
                    break;

                case IdentifierType.Expression:

                    string regularExpStmt;

                    StringBuilder completeRegularExp = new StringBuilder();
                    if (identifier.ToLower().Contains("style:"))
                    {
                        int styleInd = identifier.ToLower().IndexOf("style:");
                        if (styleInd > 0)
                        {
                            identifier = identifier.Remove(styleInd, 6).Insert(styleInd, "style:");
                            IdentifierExpression expPt1 =
                                new IdentifierExpression(
                                    identifier.Substring(0, styleInd - (identifier[styleInd - 1] == ';' ? 1 : 0)),
                                    IsMatchMethod);
                            completeRegularExp.Append("(");
                            completeRegularExp.Append((string) expPt1.IsMatch("", BrowserType.FireFox));
                            completeRegularExp.Append(")&&");
                        }

                        IdentifierExpression expPt2 = new IdentifierExpression(identifier.Substring(styleInd),
                                                                               IsMatchMethod);
                        completeRegularExp.Append("(");
                        completeRegularExp.Append((string) expPt2.IsMatch("", BrowserType.FireFox));
                        completeRegularExp.Append(")");

                        //If there is a colon in the style string, meaning there are
                        //attribute-value pairs instead of seeking to match the entire style string
                        if (identifier.IndexOf(':', styleInd + 7) != -1)
                        {
                            //6 is the length of "style:"
                            completeRegularExp.Append("||(");
                            completeRegularExp.Append(buildStyleString(identifier.Substring(styleInd + 6)));
                            completeRegularExp.Append(")");
                        }

                        regularExpStmt = completeRegularExp.ToString();
                    }
                    else
                    {
                        IdentifierExpression exp = new IdentifierExpression(identifier, IsMatchMethod);
                        regularExpStmt = (string) exp.IsMatch("", BrowserType.FireFox);
                    }

                    searchStr =
                        string.Format(
                            "var elems = doc.getElementsByTagName('{0}'); for(a=0;a < elems.length;a++){{ if({1})elem = elems[a]; }}",
                            elementTagName, regularExpStmt);
                    buildElementFindHelper(searchStr, msg);

                    break;
            }
        }

        private static String buildStyleString(string identifier)
        {
            StringBuilder msg = new StringBuilder();

            identifier = escapeChars(identifier);

            string[] expressions = identifier.Split(';');

            ExpressionToken[] tokens = (from expression in expressions
                                        where !expression.Contains("parentElement")
                                        select new ExpressionToken(expression)).ToArray();

            msg.Append("elems[a]!=null&&elems[a].hasAttributes() == true ");
            const string matcherPart2 =
                "&& doc.defaultView != null && doc.defaultView.getComputedStyle(elems[a], null).getPropertyCSSValue('{0}') != null && doc.defaultView.getComputedStyle(elems[a], null).getPropertyCSSValue('{0}').cssText != null && doc.defaultView.getComputedStyle(elems[a], null).getPropertyCSSValue('{0}').cssText.toLowerCase().match('{1}') != null && doc.defaultView.getComputedStyle(elems[a], null).getPropertyCSSValue('{0}').cssText.toLowerCase().match('{1}').length>=0 ";

            foreach (ExpressionToken t in tokens)
                msg.AppendFormat(matcherPart2, t.Attribute.Trim(), t.Value.Trim());

            return msg.ToString();
        }

        #endregion

        #region Extension Methods

        private JSSHConnection GetExtensionConnection(bool withGuid)
        {
            if (_firefoxVersion[0] == '3')
                return withGuid ? new JSSHConnection(currentlyAttachedFirefoxWindowGuid) : new JSSHConnection();          
            return withGuid ? new ReplConnection(currentlyAttachedFirefoxWindowGuid) : new ReplConnection();
        }

        private bool IsSuccessfulResult(string result)
        {
            return IsConfirmDialogOpen() || isJSDialogShowing ||
                   (((!result.Contains("failed") && !result.Contains("ReferenceError")) && !result.Contains("Exception")) &&
                    !result.Contains("TypeError")) && !result.Contains("SOCKET DISCONNECTED");
        }

        #endregion

        #region JSDialog Handling Methods

        private bool isJSDialogShowing
        {
            get
            {
                if (_currentURI != null)
                {
                    IntPtr dialogHwnd = GetJSDialogHandle();
                    return dialogHwnd != IntPtr.Zero;
                }
                return false;
            }
        }

        private static readonly string[] jsDialogWindowSubstrings =
            {
                "The page at",
                "[JavaScript Application]",
                "Confirm",
                "Are you sure?"
            }; // This contains the Dialog substrings that we'll look for

        private static readonly string[] jsDialogClasses =
            {
                "#32770",
                "MozillaDialogClass"
            }; // This contains the Dialog class names that we'll look for

        internal IntPtr GetJSDialogHandle()
        {
            return GetJSDialogHandle(0);
        }

        internal override IntPtr GetJSDialogHandle(int timeoutSeconds)
        {
            // Ignore timeout length
            IntPtr dialogHwnd = IntPtr.Zero;

            foreach (string className in jsDialogClasses)
            {
                foreach (string windowTitle in jsDialogWindowSubstrings)
                {
                    dialogHwnd = NativeMethods.FindWindowByClassNameAndSubstring(className, windowTitle);

                    if (dialogHwnd != IntPtr.Zero)
                    {
                        return dialogHwnd;
                    }
                }
            }

            return dialogHwnd;
        }

        private static void ClickOkButton(IntPtr okButtonHandle)
        {
            NativeMethods.SendMessage(okButtonHandle, NativeMethods.WM_SETFOCUS, 0x00000000, 0x00000000);
            NativeMethods.SendMessage(okButtonHandle, NativeMethods.WM_KEYDOWN, 0x0000000D, 0x001C0001);
            NativeMethods.SendMessage(okButtonHandle, NativeMethods.WM_CHAR, 0x0000000D, 0x401C0001);
            NativeMethods.SendMessage(okButtonHandle, NativeMethods.WM_KEYUP, 0x0000000D, 0xC01C0001);
        }

        private static void CloseDialog(IntPtr dialogHandle)
        {
            NativeMethods.CloseWindow(dialogHandle);
        }

        private void ClickOkAlternateDialog()
        {
            _lastWasConfirmDialog = true;
            using (JSSHConnection jssh = GetExtensionConnection(false))
            {
                jssh.SendMessage("document.activeElement.click();", false, false);
            }
        }

        private void CloseAlternateDialog()
        {
            using (JSSHConnection jssh = GetExtensionConnection(false))
            {
                jssh.SendMessage("document.activeElement.parentNode.children[4].click();", false, false);
            }
        }

        private void InteractWithJSDialog(JScriptDialogButtonType buttonType, IntPtr dialogHandle, IntPtr buttonHandle)
        {
            if (dialogHandle != curWindowHandle)
                ShowDialog(dialogHandle);

            if (buttonType == JScriptDialogButtonType.Ok)
            {
                if (_firefoxVersion[0] == '3')
                {
                    _lastWasConfirmDialog = false;
                    ClickOkButton(buttonHandle);
                }
                else
                {
                    ClickOkAlternateDialog();
                }
            }
            else
            {
                if (_firefoxVersion[0] == '3')
                    CloseDialog(dialogHandle);
                else
                    CloseAlternateDialog();
            }
        }

        private IntPtr FindDialog(DateTime timeout)
        {
            const string domDialog = "document.activeElement";
            const string dialogButton = "document.activeElement.label";
            IntPtr dialogHandle = IntPtr.Zero;
            while (dialogHandle == IntPtr.Zero && DateTime.Now < timeout)
            {
                dialogHandle = GetJSDialogHandle();
                if (dialogHandle == IntPtr.Zero)
                {
                    bool dialogOpen;
                    using (JSSHConnection jssh = GetExtensionConnection(false))
                    {
                        string xulObject = jssh.SendMessage(domDialog, true, false);
                        string buttonObject = jssh.SendMessage(dialogButton, true, false);
                        dialogOpen = xulObject.Contains("XULElement") &&
                                     (buttonObject.Equals("OK") || buttonObject.Equals("Cancel"));
                    }
                    if (dialogOpen)
                        return curWindowHandle;
                }
                Thread.Sleep(0);
            }
            return dialogHandle;
        }

        private IntPtr FindButton(IntPtr dialogHandle, DateTime timeout)
        {
            IntPtr buttonHandle = IntPtr.Zero;
            if (dialogHandle != IntPtr.Zero)
            {
                while (buttonHandle == IntPtr.Zero && DateTime.Now < timeout)
                {
                    ShowDialog(dialogHandle);
                    buttonHandle = GetButtonHandle(dialogHandle);
                    if (buttonHandle == IntPtr.Zero)
                    {
                        using (JSSHConnection jssh = GetExtensionConnection(false))
                        {
                            string actElem = jssh.SendMessage("print(document.activeElement);", true, false);
                            buttonHandle = (actElem.Contains("XULElement")) ? new IntPtr(-1) : IntPtr.Zero;
                        }
                    }
                }
            }
            return buttonHandle;
        }

        private IntPtr GetButtonHandle(IntPtr dialogHandle)
        {
            return (_firefoxVersion[0] != '3')
                       ? dialogHandle
                       : NativeMethods.GetChildWindowHwnd(dialogHandle, "MozillaWindowClass");
        }

        private static void ShowDialog(IntPtr dialogHandle)
        {
            NativeMethods.SetActiveWindow(dialogHandle);
            NativeMethods.SwitchToThisWindow(dialogHandle, true);
        }

        public void ClickJSDialog(JScriptDialogButtonType buttonType)
        {
            bool dialogClicked = false;

            DateTime timeout = DateTime.Now.AddSeconds(30);
            IntPtr dialogHandle = FindDialog(timeout);
            IntPtr buttonHandle = FindButton(dialogHandle, timeout);

            if (dialogHandle != IntPtr.Zero)
            {
                while (!dialogClicked && DateTime.Now < timeout)
                {
                    InteractWithJSDialog(buttonType, dialogHandle, buttonHandle);
                    Thread.Sleep(5);
                    dialogClicked = !IsJSDialogOpen(dialogHandle);
                }
            }
            _lastWasConfirmDialog = false;

            RecoverFromDialog(buttonType);

            if (!dialogClicked)
                throw new ClickJSDialogException();
        }

        private bool IsConfirmDialogOpen()
        {
            bool res;
            using (JSSHConnection jssh = GetExtensionConnection(false))
            {
                string activeElem = jssh.SendMessage("print(document.activeElement.label);", true, false);
                res = activeElem.Contains("OK") || activeElem.Contains("Cancel");
            }
            return res;
        }

        private bool IsJSDialogOpen(IntPtr dialogHandle)
        {
            if (_lastWasConfirmDialog)
                return false;
            bool res = false;
            if (_firefoxVersion[0] != '3')
                res = IsConfirmDialogOpen();
            res = res ? true : NativeMethods.IsWindowVisible(dialogHandle) && dialogHandle != curWindowHandle;
            return res;
        }

        private void RecoverFromDialog(JScriptDialogButtonType buttonType)
        {
            if (buttonType == JScriptDialogButtonType.Ok)
            {
                WaitForBrowserWindow();
            }
            else
            {
                lastBrowserWindowAction = LastBrowserWindowAction.InUse;
            }
        }

        public void AssertJSDialogContent(string dialogContent)
        {
            AssertJSDialogContent(dialogContent, DefaultTimeouts.FindElementTimeout);
        }

        public void AssertJSDialogContent(string dialogContent, int timeoutSeconds)
        {
            DateTime timeout = DateTime.Now.AddSeconds(timeoutSeconds);
            IntPtr dialogHandle = FindDialog(timeout);
            if (dialogHandle == curWindowHandle)
            {
                AssertConfirmDialog(dialogContent);
                return;
            }
            StringBuilder msg = new StringBuilder("var popUpWindow = null; ");
            msg.Append("var openWindows = getWindows();");
            msg.Append("for(i = 0; i < openWindows.length; i++){ ");
            msg.Append("if((openWindows[i].document.title.indexOf('[JavaScript Application]') > -1) ");
            msg.Append("|| (openWindows[i].document.title.indexOf('[JavaScript Application] ') > -1) ");
            msg.Append("|| (openWindows[i].document.title.indexOf('" + _currentURI.Scheme + "://" + _currentURI.Host +
                       "') > -1) ");
            msg.Append("|| (openWindows[i].document.title.indexOf('" + _currentURI.Scheme + "://" +
                       _currentURI.Authority + "') > -1)");
            foreach (string titleSubstring in jsDialogWindowSubstrings)
            {
                msg.Append("|| (openWindows[i].document.title.substr(0, " + titleSubstring.Length + ") == '" +
                           titleSubstring + "')");
            }
            msg.Append(") { popUpWindow = openWindows[i]; break; }} "); //find the JScript dialog window
            msg.Append("if(popUpWindow != null) print(domDumpFull(popUpWindow.document));"); //return the dialog DOM

            if (dialogHandle == IntPtr.Zero)
                throw new DialogNotFoundException();

            string message = msg.ToString();
            string retrievedDOM;
            bool matched = false;

            timeout = DateTime.Now.AddMilliseconds(250);
            using (JSSHConnection jssh = GetExtensionConnection(false))
            {
                do
                {
                    retrievedDOM = jssh.SendMessage(message);

                    // if the dialog we found contains the content we're looking for,
                    // then we have a match
                    if (retrievedDOM.Contains(dialogContent))
                    {
                        matched = true;
                        break;
                    }
                } while (DateTime.Now < timeout);

                if (!matched)
                    throw new AssertionFailedException(
                        string.Format("The open javascript dialog content is not equal to \"{0}\".", dialogContent));
            }
        }

        private void AssertConfirmDialog(string dialogContents)
        {
            string dialogText;
            using (JSSHConnection jssh = GetExtensionConnection(false))
                dialogText = jssh.SendMessage("document.activeElement.parentNode.parentNode.textContent", true, false);
            if (!dialogText.Contains(dialogContents))
                throw new AssertionFailedException(
                    string.Format("The open javascript dialog content is not equal to \"{0}\".", dialogContents));
        }

        #endregion

        #region Browser Attachment Methods

        private void AttachToBrowserWindow(string windowTitle, int waitTime, int windowIndex)
        {
            windowTitle = escapeChars(windowTitle).ToLower(); // Added to fix apostrophe issue in FireFox.

            /*SWAT  neeeds to have a socket connecting to Firefox. The solution is 2 fold: 
             * Start Firefox with JSSH enabled (the FF shortcut needs to be "...\firefox.exe" -jssh with the Firefox jssh XPI installed)
             * Create a new socket to connect to FF when attaching to a window           
            */
            StringBuilder msg = new StringBuilder();
            msg.Append("indexCount = 0;");
            msg.Append("firefoxWindow = null;");
            msg.Append("i = 0;");
            msg.Append("var windows = getWindows();");
            msg.Append("for(i = 0;i < windows.length;i++){");
            msg.Append("var windowTitle = windows[i].document.title.toLowerCase(); ");
            msg.Append("if(windowTitle.indexOf('" + windowTitle + "') > -1){");
            msg.Append("if(indexCount ==" + windowIndex + "){");
            msg.Append("firefoxWindow = windows[i];");
            msg.Append("break;}");
            msg.Append("else ");
            msg.Append("indexCount++;");
            msg.Append("}");
            msg.Append("}");

            StringBuilder specialChar = new StringBuilder();
            specialChar.Append("var title = firefoxWindow.document.title;");
            specialChar.Append("var a = [];");
            specialChar.Append("for (var i = 0; i < title.length; i++)");
            specialChar.Append("a.push(title.charCodeAt(i));");
            specialChar.Append("print(a);");

            string message = msg.ToString();
            string result = string.Empty;
            string fullWindowTitle = string.Empty;
            bool connected = false;

            DateTime timeOut = DateTime.Now.AddSeconds(waitTime);

            if (_firefoxVersion == null)
            {
                string path = BrowserPaths.FirefoxRootDirectory;
                if (string.IsNullOrEmpty(path)) //not found in the registry
                    throw new BrowserNotInstalledException("Firefox is not installed.");
                if (!File.Exists(path))
                    throw new IllegalDirectoryException(string.Format("Firefox was not found in {0}", path));
                _firefoxVersion = FileVersionInfo.GetVersionInfo(path).ProductVersion;
            }

            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                while (DateTime.Now < timeOut)
                {
                    connected = jssh.ConnectToJSSH();
                    if (connected)
                    {
                        jssh.SendMessage(message);
                        result = jssh.SendMessage("firefoxWindow").Trim();

                        if (!result.ToLower().Contains("chrome"))
                            Thread.Sleep(500);
                        else
                            break;
                    }
                }

                if (result.ToLower().Contains("chrome"))
                {
                    //set browser
                    int ctr = 15;
                    while ((fullWindowTitle == "" || fullWindowTitle == "null" ||
                            fullWindowTitle.Contains("[object ChromeWindow]") ||
                            fullWindowTitle.Contains("SwatCommandCompleted") ||
                            fullWindowTitle.Contains("TypeError: firefoxWindow is null") ||
                            fullWindowTitle.Contains("firefoxWindow is undefined")) && ctr > 0)
                    {
                        fullWindowTitle = jssh.SendMessage(specialChar.ToString());
                        fullWindowTitle = decodeSpecialChars(fullWindowTitle);
                        jssh.SendMessage(
                            "var browser = firefoxWindow.getBrowser();print('SwatCommandCompleted Navigate Browser');");
                        ctr--;
                    }

                    currentlyAttachedFirefoxWindowGuid = Guid.NewGuid().ToString();
                    jssh.SendMessage(string.Format("firefoxWindow.swatGuid = '{0}';print(firefoxWindow.swatGuid);",
                                                   currentlyAttachedFirefoxWindowGuid));

                    Regex windowTitleRegex = new Regex(Regex.Escape(fullWindowTitle).Replace("\\?", "."),
                                                       RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
                    string windowClass = (_firefoxVersion[0] != '3') ? "MozillaWindowClass" : "MozillaUIWindowClass";
                    curWindowHandle = NativeMethods.FindWindowByClassNameAndRegex(windowClass, windowTitleRegex);

                    if (curWindowHandle == IntPtr.Zero)
                        throw new WindowHandleNotFoundException(
                            string.Format("Window handle not found for window title \"{0}\"", fullWindowTitle));

                    NativeMethods.BringWindowToTop(curWindowHandle);
                    NativeMethods.SetForegroundWindow(curWindowHandle);
                }
                else if (!connected && Process.GetProcessesByName(BrowserProcess.firefox.ToString()).Length > 0)
                {
                    throw new FireFoxClientIsNotConnectedException(
                        "Unable to connect to JSSH. Please verify Firefox was opened with JSSH enabled.");
                }
                else
                {
                    string indexCountStr = jssh.SendMessage("print(indexCount);");
                    if (fullWindowTitle.Contains("pdf"))
                        goto end;
                    int finalCount;

                    try
                    {
                        finalCount = !connected ? 0 : Convert.ToInt32(indexCountStr);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error occurred while attaching to Firefox bad indexCountStr: " + indexCountStr);
                    }

                    if (finalCount == 0)
                        throw new WindowNotFoundException(windowTitle);

                    string error = finalCount == 1
                                       ? "There is only one window"
                                       : "There are only " + finalCount + " windows";

                    throw new IndexOutOfRangeException(error + " that has " + windowTitle + " in the title.");
                }
            }
        end:
            _lastAttachWindowTitle = windowTitle;

        if (!_firefoxVersion.StartsWith("3"))
            Thread.Sleep(1500);
        }

        private static string decodeSpecialChars(string charArray)
        {
            StringBuilder res = new StringBuilder(charArray.Length/2);
            string[] charCodes = charArray.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string charCode in charCodes)
            {
                int val;
                bool worked = Int32.TryParse(charCode, out val);
                if (worked)
                    res.Append((char) val);
            }

            return res.ToString().Trim();
        }

        public void AttachBrowserToWindow(string windowTitle)
        {
            AttachBrowserToWindow(windowTitle, 0);
        }

        public void AttachBrowserToWindow(string windowTitle, int windowIndex)
        {
            AttachToBrowserWindow(windowTitle, DefaultTimeouts.AttachToWindowBrowserTimeout, windowIndex);
        }

        #endregion

        #region IBrowser Members

        public void AssertTopWindow(string browserTitle, int index, int timeout)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("var count = -1;");
            msg.Append("var success = false;");
            msg.Append("var windows = getWindows();");
            msg.Append("for (var i = 0; i < windows.length && !success; i++) {");
            msg.Append("if (getWindows()[i].document.title.toLowerCase().indexOf('" + browserTitle.ToLower() +
                       "') > -1) { count++; }");
            msg.Append("if (count == " + index + " && windows[i].swatGuid == '" + currentlyAttachedFirefoxWindowGuid +
                       "') { success = true; break; } }");
            msg.Append("if (success) print('true');");
            msg.Append("else print(count+1);");

            string message = msg.ToString();
            string actualTitle;

            DateTime end = DateTime.Now.AddSeconds(timeout);
            string response;
            do
            {
                using (JSSHConnection jssh = GetExtensionConnection(true))
                    response = jssh.SendMessage(message);

                IntPtr topWindwHnd = NativeMethods.GetForegroundWindow();
                actualTitle = NativeMethods.GetWindowText(topWindwHnd);
                string title = actualTitle.ToLower();

                if (response.Contains("true") && title.Contains(browserTitle.ToLower()) &&
                    title.Contains("mozilla firefox"))
                    return;
            } while (DateTime.Now < end);

            if (index == 0)
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new TopWindowMismatchException(browserTitle, actualTitle);
                throw new TopWindowMismatchException(browserTitle);
            }
            if (!response.Contains("true") && Convert.ToInt32(response) <= index)
                throw new IndexOutOfRangeException("Index: " + index + " is too large, there are only " + response +
                                                   " window(s) with title: " + browserTitle);
            if (WantInformativeExceptions.GetInformativeExceptions)
                throw new TopWindowMismatchException(browserTitle, index, actualTitle);
            throw new TopWindowMismatchException(browserTitle, index);
        }

        public void ElementFireEvent(IdentifierType identType, string identifier, string tagName, string eventName)
        {
            eventName = eventName.ToLower();

            if (eventName.StartsWith("on"))
                eventName = eventName.Remove(0, 2);

            StimulateElement(identType, identifier, tagName, eventName);
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, string tagName,
                                        AttributeType attributeType, string attributeName, string attributeValue,
                                        int timeOut)
        {
            setElementProperty(identType, identifier, tagName, attributeType, attributeName, attributeValue, timeOut);
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, string tagName,
                                        AttributeType attributeType, string attributeName, string attributeValue)
        {
            SetElementAttribute(identType, identifier, tagName, attributeType, attributeName, attributeValue,
                                DefaultTimeouts.FindElementTimeout);
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, string tagName,
                                          AttributeType attributeType, string attributeName)
        {
            return GetElementAttribute(identType, identifier, tagName, attributeType, attributeName,
                                       DefaultTimeouts.FindElementTimeout);
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, string tagName,
                                          AttributeType attributeType, string attributeName, int timeOut)
        {
            return getElementProperty(identType, identifier, tagName, attributeType, attributeName, timeOut);
        }

        private string previousLowerCaseWindowTitleToKeepOpen = "";

        private void WaitForRemainingWindowsToClose()
        {
            lastBrowserWindowAction = LastBrowserWindowAction.KilledExceptTitle;

            string waitForWindowsToCloseCommandString =
                GetWaitForWindowsToCloseCommandString(previousLowerCaseWindowTitleToKeepOpen);
            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                DateTime timeout = DateTime.Now.AddSeconds(5);
                while (DateTime.Now < timeout)
                {
                    string result;
                    using (JSSHConnection jssh = GetExtensionConnection(true))
                        result = jssh.SendMessage(waitForWindowsToCloseCommandString);

                    if (!result.Contains("windowExists"))
                    {
                        break;
                    }

                    if (dialogWatcher.FoundDialog)
                    {
                        return;
                    }
                    Thread.Sleep(0);
                }
            }

            lastBrowserWindowAction = LastBrowserWindowAction.InUse;

            if (!IsBrowserAttached())
            {
                ClearAttachedWindowVariables();
            }
        }

        public override void AssertBrowserIsAttached()
        {
            if (curWindowHandle == IntPtr.Zero)
                throw new NoAttachedWindowException();

            if (!IsBrowserAttached())
            {
                ClearAttachedWindowVariables();
                throw new NoAttachedWindowException();
            }
        }

        private bool IsBrowserAttached()
        {
            string guid;
            using (JSSHConnection jssh = GetExtensionConnection(true))
                guid = jssh.SendMessage("print(firefoxWindow.swatGuid);").Trim();
            //if a frame executes javascript that causes a diffrerent frame to load in the doc, it resets the swatGuid
            if (!guid.Contains(currentlyAttachedFirefoxWindowGuid) && (string.IsNullOrEmpty(guid) ||
                                                                       ((guid.Contains("repl") ||
                                                                         guid.Contains("object") ||
                                                                         guid.Contains("function"))
                                                                        &&
                                                                        GetCurrentWindowTitle() ==
                                                                        _lastAttachWindowTitle)))
            {
                using (JSSHConnection jssh = GetExtensionConnection(true))
                    guid =
                        jssh.SendMessage(string.Format(
                            "firefoxWindow.swatGuid = '{0}'; print(firefoxWindow.swatGuid);",
                            currentlyAttachedFirefoxWindowGuid));
                Console.WriteLine("bad guid: " + guid);
            }
            return (!string.IsNullOrEmpty(guid) && guid.Contains(currentlyAttachedFirefoxWindowGuid)) &&
                   Process.GetProcessesByName(BrowserProcess.firefox.ToString()).Length != 0;
        }

        private void ClearAttachedWindowVariables()
        {
            curWindowHandle = IntPtr.Zero;
            currentlyAttachedFirefoxWindowGuid = "";
        }

        #endregion

        #region RunScript

        protected override string runJavaScript(String theScript)
        {
            WaitForBrowserReadyState();

            StringBuilder msg = new StringBuilder();

            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                //send a message to set browser, document and window objects 
                //AND store current window var to a temp var in order to restore it back after runScript
                jssh.SendMessage("var browser = firefoxWindow.getBrowser(); var document = browser.contentDocument; var window = null;"
                                 +
                                 "for(var i=0; i < firefoxWindow.frames.length; i++){if(firefoxWindow.frames[i].toString().toLowerCase().indexOf('object window') > -1)"
                                 + "{window = firefoxWindow.frames[i]; break;}} new Function(\"return true;\")();");

                msg.Append(theScript);
                string result = string.Empty;
                DateTime timeout = DateTime.Now.AddSeconds(DefaultTimeouts.FindElementTimeout);
                while (DateTime.Now < timeout)
                {
                    result = jssh.SendMessage(msg.ToString());

                    if (String.IsNullOrEmpty(result))
                        Thread.Sleep(100); //! 1000
                    else
                    {
                        break;
                    }
                }

                return result;
            }
        }

        // TODO: delete the following unused method if it isn't necessary
        //private static string matchDelegate(Match match)
        //{
        //    System.Diagnostics.Debug.WriteLine(match.Value);
        //    return match.Value.Replace("].", string.Format("].contentWindow."));
        //}

        #endregion

        #region ScreenShots Helper Methods

        public override IntPtr GetContentHandle()
        {
            return GetContentServerHandle(curWindowHandle);
        }

        public override void SetDocumentAttribute(string theAttributeName, object theAttributeValue)
        {
            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                StringBuilder command = new StringBuilder("if (doc.compatMode != 'BackCompat')");
                command.Append("{ doc.documentElement." + theAttributeName + " = '" + theAttributeValue + "'; }");
                command.Append("else { doc.body." + theAttributeName + " = '" + theAttributeValue + "'; }");
                jssh.SendMessage(command.ToString());
            }
        }

        public override object GetDocumentAttribute(string theAttributeName)
        {
            using (JSSHConnection jssh = GetExtensionConnection(true))
            {
                StringBuilder command = new StringBuilder("var theAttribute = null; if (doc.compatMode != 'BackCompat')");
                command.Append("{ theAttribute = doc.documentElement." + theAttributeName + "; print(theAttribute); }");
                command.Append("else { theAttribute = doc.body." + theAttributeName + "; print(theAttribute); }");
                return jssh.SendMessage(command.ToString());
            }
        }

        #endregion

        #region IDisposable

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

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion
    }
}
