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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using Microsoft.Win32;
using mshtml;
using SHDocVw;


namespace SWAT
{
    public class InternetExplorer : Browser, IBrowser//, IDisposable
    {
        #region Class Variables

        private string _browserVersion = string.Empty;
        private RegistryKey _browserVersionKey = Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Internet Explorer");
        private bool _attachedToModalDialog = false;
        HTMLDocument _doc;
        private ElementEventInfo _eventInfo;
        private List<KeyEventStatus> _keyEvents;
        private int _keyCode = 0;

        // The IWebBrowser2.Name is always "Windows Internet Explorer" for IE windows
        private static string ieBrowserName = "Windows Internet Explorer";

        SHDocVw.InternetExplorer currentIEBrowser;

        //const uint WM_KEYDOWNlParam = 0x001E0001;
        //const uint WM_KEYUPlParam = 0xC01E0001;

        //These lParams get the scan code calculated in before sending the key press
        const uint WM_KEYDOWNlParam = 0x00000001;
        const uint WM_KEYUPlParam = 0xC0000001;

        //public Boolean isParent = false;
        //public Boolean searchedParent = false;

        private enum ParentValues { id, @class, className, innerHTML }

        #endregion


        #region Constructor

        public InternetExplorer() : base(BrowserType.InternetExplorer, BrowserProcess.iexplore) { setIEVersionNumber(); }

        #endregion


        #region Delegates

        delegate void buttonClickDelegate();

        private delegate void fireEventDelegate(IHTMLElement3 elem, string eventName);

        #endregion


        #region Navigation Methods

        [NCover.CoverageExcludeAttribute]
        public void OpenBrowser()
        {
            bool done = false;
            while (!done)
            {
                try
                {
                    currentIEBrowser = (InternetExplorerClass)Activator.CreateInstance(typeof(InternetExplorerClass));
                    currentIEBrowser.Visible = true;
                    curWindowHandle = new IntPtr(currentIEBrowser.HWND);
                    done = true;
                }
                catch { }
            }
            navigateTo("about:Blank");
            setDocument(currentIEBrowser.Document as HTMLDocument);
            bringCurrentBrowserWindowToTop();
        }

        public void CloseBrowser()
        {
            currentIEBrowser.Quit();
            WaitForWindowToClose(curWindowHandle);
        }

        private void WaitForBrowserWindow()
        {
            switch (lastBrowserWindowAction)
            {
                case LastBrowserWindowAction.Closed: WaitForWindowToClose(curWindowHandle);
                    break;
                case LastBrowserWindowAction.Killed: WaitForProcessesToEnd();
                    break;
                default: waitForBrowser();
                    break;
            }
        }

        private void WaitForWindowToClose(IntPtr closingWindowHandle)
        {
            lastBrowserWindowAction = LastBrowserWindowAction.Closed;
            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                bool windowStillOpen = true;
                while (windowStillOpen)
                {
                    if (dialogWatcher.FoundDialog)
                    {
                        return;
                    }

                    windowStillOpen = NativeMethods.IsWindow(closingWindowHandle);

                    if (!windowStillOpen)
                        break;
                }
            }

            lastBrowserWindowAction = LastBrowserWindowAction.InUse;

            if (!isBrowserValid())
            {
                curWindowHandle = IntPtr.Zero;
                _doc = null;
            }
        }

        private void WaitForProcessesToEnd()
        {
            lastBrowserWindowAction = LastBrowserWindowAction.Killed;
            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                int count = 30;
                while ((Process.GetProcessesByName(BrowserProcess.iexplore.ToString()).Length > 0) && (count > 0))
                {
                    if (dialogWatcher.FoundDialog)
                    {
                        return;
                    }
                    System.Threading.Thread.Sleep(250);
                    count--;
                }
            }
            lastBrowserWindowAction = LastBrowserWindowAction.InUse;

            if (!isBrowserValid())
            {
                curWindowHandle = IntPtr.Zero;
            }
        }

        public void KillAllOpenBrowsers()
        {
            foreach (Process ie in Process.GetProcessesByName("iexplore"))
            {
                try
                {
                    ie.Kill();
                    CoverCOMExceptionHandling();
                }
                catch (Exception) { continue; }
            }

            WaitForProcessesToEnd();
        }

        public void KillAllOpenBrowsers(string windowTitle)
        {
            bool windowExists = false;
            string windowTitleLowerCase = setUpPDFExtentions(windowTitle).ToLower();

            ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
            Dictionary<SHDocVw.InternetExplorer, int> killedBrowsers = new Dictionary<SHDocVw.InternetExplorer, int>(m_IEFoundBrowsers.Count);

            // Need to iterate through ShellWindows multiple times in order to get all the windows.
            // Iterating through ShellWindows once or twice leaves a few windows open.
            // This may be because the open browsers may not have been registered with the ShellWindowsClass
            //  yet, in which it won't be visible to it
            for (int count = 10; count >= 0; count--)
            {
                // Refresh the ShellWindows list
                m_IEFoundBrowsers = new ShellWindowsClass();
                foreach (SHDocVw.InternetExplorer browser in m_IEFoundBrowsers)
                {
                    try
                    {
                        if (browser.Name.Equals(ieBrowserName) && !killedBrowsers.ContainsKey(browser))
                        {
                            // Check if browser is not the one we want to spare
                            if (!browserTitleIsMatch(browser, windowTitleLowerCase))
                            {
                                killedBrowsers.Add(browser, browser.HWND);
                                browser.Quit();
                            }
                            else
                                windowExists = true;

                            CoverCOMExceptionHandling();
                        }
                    }
                    catch (COMException)
                    {
                        // Someone manually closed a window while this was running.
                        continue;
                    }

                }
            }

            // If we have killed all the windows, we need to wait for the process to end
            if (!windowExists)
            {
                WaitForProcessesToEnd();
            }
            else   // Wait for each window we killed to close
            {
                foreach (int killedBrowserHandle in killedBrowsers.Values)
                {
                    WaitForWindowToClose(new IntPtr(killedBrowserHandle));
                }
            }
        }

        public void RefreshBrowser()
        {
            if (!(currentIEBrowser.Document is mshtml.HTMLDocument))
                throw new BrowserDocumentNotHtmlException("This method only works on HTML documents.");

            ((mshtml.HTMLDocument)currentIEBrowser.Document).execCommand("Refresh", true, 0);
            waitForBrowser();
        }

        public void NavigateBrowser(string url)
        {
            navigateTo(url);

            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                while (currentIEBrowser.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE)
                {
                    if (dialogWatcher.FoundDialog)
                    {
                        break;
                    }
                }

                updateDoc();
                curWindowHandle = new IntPtr(currentIEBrowser.HWND);
            }
        }

        public string GetCurrentLocation()
        {
            waitForBrowser();
            return currentIEBrowser.LocationURL;
        }

        public string GetCurrentDocumentTitle()
        {
            if (!(currentIEBrowser.Document is mshtml.HTMLDocument))
                throw new BrowserDocumentNotHtmlException("This method only works on HTML documents.");

            return ((IHTMLDocument2)(currentIEBrowser.Document)).title;
        }

        public string GetCurrentWindowTitle()
        {
            StringBuilder windowTitle = new StringBuilder(NativeMethods.GetWindowTextLength(curWindowHandle) + 1);
            NativeMethods.GetWindowText(curWindowHandle, windowTitle, windowTitle.Capacity);
            return windowTitle.ToString();
        }

        public void AssertBrowserExists(string windowTitle)
        {
            findBrowser(windowTitle, DefaultTimeouts.AssertBrowserExists, true);
        }

        public void AssertBrowserExists(string windowTitle, double timeOut)
        {
            findBrowser(windowTitle, (int)Math.Ceiling(timeOut / 1000), true);
        }

        public void AssertBrowserDoesNotExist(string windowTitle)
        {
            findBrowser(windowTitle, DefaultTimeouts.AssertBrowserExists, false);
        }

        public void AssertBrowserDoesNotExist(string windowTitle, double timeOut)
        {
            findBrowser(windowTitle, (int)Math.Ceiling(timeOut / 1000), false);
        }

        private bool doesWindowExist(string windowTitle)
        {
            string windowTitleLowerCase = setUpPDFExtentions(windowTitle).ToLower();
            ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();

            foreach (SHDocVw.InternetExplorer browser in m_IEFoundBrowsers)
            {
                try
                {
                    if (browser.Name.Equals(ieBrowserName) && browserTitleIsMatch(browser, windowTitleLowerCase))
                        return true;
                }
                catch (COMException)
                {
                    // A window was manually closed while iterating through ShellWindows
                    continue;
                }
            }

            List<IntPtr> openWindowList = NativeMethods.GetAllOpenWindowsUnordered();
            StringBuilder className = new StringBuilder(255);

            // Looking for Modal Dialogs
            foreach (IntPtr intPtr in openWindowList)
            {
                NativeMethods.GetClassName(intPtr, className, className.MaxCapacity);
                if (NativeMethods.GetWindowText(intPtr).Contains(windowTitle) && className.ToString().StartsWith("Internet Explorer"))
                    return true;
            }

            // Window not found
            return false;
        }

        private void findBrowser(string windowTitle, int timeOut, bool expectPositiveResult)
        {
            DateTime startTime = DateTime.Now;
            bool foundBrowser = false;

            while (DateTime.Now < startTime.AddSeconds(timeOut))
            {
                foundBrowser = doesWindowExist(windowTitle);

                if (expectPositiveResult && foundBrowser) //AssertBrowserExists
                    return;
                else if (!expectPositiveResult && !foundBrowser) //AssertBrowserDoesNotExist
                    return;
            }

            if (!expectPositiveResult && foundBrowser) //AssertBrowserDoesNotExist
                throw new BrowserExistException(string.Format("There is a browser with title \"{0}\" already open.", windowTitle));
            else if (expectPositiveResult) //AssertBrowserExists
                throw new BrowserExistException(string.Format("There is no browser with title \"{0}\" open.", windowTitle));
        }

        private void attachToBrowser(int waitTime, string titleContents, int index)
        {
            string windowTitleLowerCase = setUpPDFExtentions(titleContents).ToLower();

            HTMLDocument ourDoc = null;
            IntPtr winHndle = IntPtr.Zero;
            SHDocVw.InternetExplorer oldBrowser = currentIEBrowser;
            currentIEBrowser = null;
            _attachedToModalDialog = false;
            int indexCount = 0;

            DateTime timeout = DateTime.Now.AddSeconds(waitTime);

            while (DateTime.Now < timeout)
            {
                //we must reset indexCount every iteration of this loop
                indexCount = 0;
                ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
                foreach (SHDocVw.InternetExplorer browser in m_IEFoundBrowsers)
                {
                    try
                    {
                        if (browser.Name.Equals(ieBrowserName))
                        {
                            // Check if browser is the one we are looking for
                            if (browserTitleIsMatch(browser, windowTitleLowerCase))
                            {
                                if (indexCount == index)
                                {
                                    currentIEBrowser = browser;
                                    curWindowHandle = new IntPtr(currentIEBrowser.HWND);
                                    if (browser.Type.Contains("PDF"))
                                    {
                                        return; // We are done here
                                    }
                                    break;
                                }
                                else
                                {
                                    indexCount++;
                                }
                            }
                        }
                    }
                    catch (COMException)
                    {
                        // A window was manually closed while iterating through ShellWindows
                        continue;
                    }
                }

                //if currentIEBrowser is null, then we havent found any browser windows...so check dialogs
                if (currentIEBrowser != null)
                    ourDoc = (mshtml.HTMLDocument)currentIEBrowser.Document;
                else
                    attachToModalDialog(titleContents, index, indexCount, ref ourDoc);

                //if the doc isn't null but the browser is, we have found a dialog, reassign the browser to the previous one
                if (ourDoc != null)
                {
                    if (currentIEBrowser == null)
                    {
                        _attachedToModalDialog = true;
                        currentIEBrowser = oldBrowser;
                    }
                    break;
                }
            }

            if (ourDoc == null)
            {
                if (indexCount == 0)
                    throw new WindowNotFoundException(titleContents);

                string message;
                if (indexCount == 1)
                    message = "There is only 1 window";
                else
                    message = "There are only " + indexCount + " windows";

                throw new IndexOutOfRangeException(message + " with " + titleContents + " in the title.");
            }

            setDocument(ourDoc);
            bringCurrentBrowserWindowToTop();
        }

        public void AttachBrowserToWindow(string windowTitle)
        {
            AttachBrowserToWindow(windowTitle, 0);
        }

        public void AttachBrowserToWindow(string windowTitle, int windowIndex)
        {
            attachToBrowser(DefaultTimeouts.AttachToWindowBrowserTimeout, windowTitle, windowIndex);
        }

        private void attachToModalDialog(string title, int index, int indexCount, ref HTMLDocument doc)
        {
            /* WatiN uses a DialogWatcher that logs all dialogs that open,
             * then handles them by calling one of its DialogHandlers, each of them
             * specific to the kind of dialog that opens. AFAIK, it doesn't handle
             * HTML Documents opened as modal dialogs. This is my attempt to do so, by
             * borrowing from what WatiN does.
             */
            IntPtr dialogHwnd = IntPtr.Zero;
            NativeMethods.GetWindowWithSubstring(title, index, indexCount, ref dialogHwnd);

            if (dialogHwnd != IntPtr.Zero)
            {
                curWindowHandle = dialogHwnd;
                StringBuilder windowText = new StringBuilder(NativeMethods.GetWindowTextLength(dialogHwnd) + 1);
                NativeMethods.GetWindowText(dialogHwnd, windowText, windowText.Capacity);

                if (windowText.ToString().EndsWith("-- Webpage Dialog"))
                {
                    // This block of code SHOULD take a window handle and return an IHTMLDocument object to dialogDoc
                    // Then it will assign that object to _doc. It follows an example in the MSDN library and WatiN uses it as well.
                    // If it performs correctly, then we should be attached to the Dialog.
                    dialogHwnd = NativeMethods.GetChildWindowHwnd(dialogHwnd, "Internet Explorer_Server");

                    Int32 result = 0;
                    Int32 dialog;
                    Int32 message;

                    message = NativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");

                    NativeMethods.SendMessageTimeout(dialogHwnd, message, 0, 0, NativeMethods.SMTO_ABORTIFHUNG, 1000, ref result);

                    if (result == 0)
                        return;

                    IHTMLDocument2 dialogDoc = null;
                    System.Guid dialogID = typeof(mshtml.HTMLDocument).GUID;
                    dialog = NativeMethods.ObjectFromLresult(result, ref dialogID, 0, ref dialogDoc);

                    if (dialog != 0)
                        return;

                    doc = (HTMLDocument)dialogDoc;
                }
            }
        }

        private void HandleWaitForBrowserException(Exception e)
        {
            Debug.WriteLine(e.Message);
            Debug.WriteLine(e.StackTrace);
            lastBrowserWindowAction = LastBrowserWindowAction.InUse;
        }

        public override void AssertBrowserIsAttached()
        {
            if ( !_attachedToModalDialog && (curWindowHandle == IntPtr.Zero || !isBrowserValid()) )
            {
                curWindowHandle = IntPtr.Zero;
                throw new NoAttachedWindowException();
            }
        }

        private void waitForBrowserReadyOnly()
        {
            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                try
                {
                    int timeOut = SWAT.DefaultTimeouts.WaitForBrowserTimeout;
                    DateTime startTime = DateTime.Now;
                    while ((currentIEBrowser.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE || !getDocumentReadyState(_doc).Equals("complete", StringComparison.OrdinalIgnoreCase)) && DateTime.Now < startTime.AddSeconds(timeOut))
                    {
                        if (dialogWatcher.FoundDialog)
                        {
                            break;
                        }
                    }
                }
                catch (Exception e) // COMException || InvalidCastException
                {
                    HandleWaitForBrowserException(e);
                }
            }
        }

        private void waitForBrowser()
        {
            using (DialogWatcher dialogWatcher = new DialogWatcher(this))
            {
                waitForBrowserRec(_doc, dialogWatcher);
            }
        }

        private void waitForBrowserRec(HTMLDocument nextDoc, DialogWatcher dialogWatcher)
        {
            try
            {
                int timeOut = DefaultTimeouts.WaitForBrowserTimeout;
                DateTime start = DateTime.Now;
                while (((!_attachedToModalDialog && currentIEBrowser.Busy) || (currentIEBrowser.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE || !getDocumentReadyState(nextDoc).Equals("complete", StringComparison.OrdinalIgnoreCase)) && (currentIEBrowser.ReadyState != tagREADYSTATE.READYSTATE_INTERACTIVE)) && DateTime.Now < start.AddSeconds(timeOut))
                {
                    if (dialogWatcher.FoundDialog) return;
                }
                for (int i = 0; i < nextDoc.frames.length; i++)
                {
                    object index = i;
                    IHTMLDocument2 _docBypassSecurity = CrossFrameIE.GetDocumentFromWindow((IHTMLWindow2)nextDoc.frames.item(ref index));
                    HTMLDocument _docFullAccess = (HTMLDocument)_docBypassSecurity;
                    waitForBrowserRec(_docFullAccess, dialogWatcher);
                }
            }
            catch (Exception e)
            {
                HandleWaitForBrowserException(e);
            }
        }

        public void WaitForBrowserReadyState()
        {
            isBrowserAccessible();
            waitForBrowser();
        }

        public void Sleep(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void AssertElementIsActive(IdentifierType identType, string identifier, string tagName, int timeoutSeconds)
        {
            isBrowserAccessible();

            waitForBrowser();

            IHTMLElement elem = getElement(identType, identifier, tagName, timeoutSeconds);
            toggleElementColor(elem);

            IHTMLElement activeElem = null;

            DateTime timeout = DateTime.Now.AddSeconds(2);
            do
            {
                activeElem = findActiveElement(_doc);
            } while (elem != null && !elem.Equals(activeElem) && DateTime.Now < timeout);

            toggleElementColor(activeElem);

            if (!elem.Equals(activeElem))
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotActiveException(identifier, identType, tagName);
                else
                    throw new ElementNotActiveException(identifier, identType);
            }
        }

        private IHTMLElement findActiveElement(HTMLDocument doc)
        {
            IHTMLElement elem = doc.activeElement; //findElement(doc, identType, identifier, identExp, tagName);
            if (elem == null)
                return null;
            string tName = elem.tagName.ToLower();
            Console.WriteLine("elem id = " + elem.id);
            if (tName.Contains("frame") || tName.Contains("body"))
            {
                object index = 0;
                int frames = doc.frames.length;
                for (int i = 0; i < frames; i++)
                {
                    index = i;
                    IHTMLDocument2 _docBypassSecurity = CrossFrameIE.GetDocumentFromWindow((IHTMLWindow2)doc.frames.item(ref index));
                    HTMLDocument _docFullAccess = (HTMLDocument)_docBypassSecurity;

                    elem = findActiveElement((HTMLDocument)_docFullAccess);
                    tName = elem.tagName.ToLower();
                    if (elem != null && !tName.Contains("frame") && !tName.Contains("body"))
                        break;
                }
            }

            return elem;
        }

        #endregion


        #region Helper methods

        private void bringCurrentBrowserWindowToTop()
        {
            NativeMethods.BringWindowToTop(curWindowHandle);
            NativeMethods.SetForegroundWindow(curWindowHandle);
            NativeMethods.SetActiveWindow(curWindowHandle);

            if (curWindowHandle != NativeMethods.GetForegroundWindow())
            {
                SetWindowPosition(WindowPositionTypes.MINIMIZE);
                SetWindowPosition(WindowPositionTypes.BRINGTOTOP);
            }
        }

        private void setIEVersionNumber()
        {
            _browserVersion = _browserVersionKey.GetValue("version").ToString().Substring(0, 1);
        }

        private void updateDoc()
        {
            if (!currentIEBrowser.LocationURL.ToString().EndsWith(".pdf"))
            {
                HTMLDocument temp = currentIEBrowser.Document as HTMLDocument;
                if (temp != null)
                {
                    _doc = temp;
                }
            }
        }
        private void navigateTo(string url)
        {
            object dummyObject = new object();
            currentIEBrowser.Navigate(url, ref dummyObject, ref dummyObject, ref dummyObject, ref dummyObject);
        }

        private void setDocument(HTMLDocument hDoc)
        {
            _doc = hDoc;
            _doc.focus();
        }

        private string getDocumentReadyState(HTMLDocument doc)
        {
            try
            {
                string readyState = doc.readyState;

                object index = 0;
                int frames = doc.frames.length;
                for (int i = 0; i < frames; i++)
                {
                    index = i;
                    IHTMLWindow2 win = (IHTMLWindow2)doc.frames.item(ref index);
                    readyState = getDocumentReadyState((HTMLDocument)win.document);
                }

                return readyState;
            }
            catch (Exception)
            {
                return doc.readyState;
            }
        }

        private void toggleElementColor(IHTMLElement elem)
        {
            if (SWAT.IESettings.HighlightElementsAsTestsRun == true)
            {
                string orig = string.Empty;
                if (elem != null)
                {
                    if (elem.style.backgroundColor != null)
                        orig = elem.style.backgroundColor.ToString();
                    elem.style.backgroundColor = "yellow";

                    System.Threading.Thread.Sleep(200);

                    elem.style.backgroundColor = orig;
                }
            }

        }

        private string setUpPDFExtentions(string titleContents)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced");
            if ((int)key.GetValue("HideFileExt") == 1 && titleContents.EndsWith(".pdf"))
            {
                return titleContents.Substring(0, titleContents.Length - 4);
            }
            return titleContents;
        }

        private bool isBrowserDocumentHTML()
        {
            try
            {
                if (currentIEBrowser == null || !(currentIEBrowser.Document is mshtml.HTMLDocument))
                    throw new BrowserDocumentNotHtmlException("This method only works on HTML documents.");
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                return false;
            }
            return true;
        }

        private bool isBrowserAccessible()
        {
            if (!isBrowserValid())
                throw new BrowserExistException("Browser window was closed unexpectedly");

            return isBrowserDocumentHTML();
        }

        private bool isBrowserValid()
        {
            try
            {
                if (!_attachedToModalDialog)
                {
                    object testBrowserStillOpen = currentIEBrowser.Document;
                }
            }
            catch // NullReferenceException || COMException || InvalidCastException
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if an InternetExplorer browser's title matches the specified string.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="windowTitle"></param>
        /// <returns> true if the windowTitle parameter matches the brower parameter's window title; otherwise, false. </returns>
        private bool browserTitleIsMatch(SHDocVw.InternetExplorer browser, string windowTitle)
        {
            IHTMLDocument2 doc = browser.Document as IHTMLDocument2;

            return (doc != null && // Check if the html document is even accessible
                doc.title.ToLower().Contains(windowTitle)// Check if the document title matches
                        || browser.LocationName.ToLower().Contains(windowTitle)); // check the location name (used for PDFs and "about:blank")
        }

        /// <summary>
        /// Added this helper function and boolean for getting consistent 
        /// code coverage metrics for COMException case handling
        /// </summary>
        private void CoverCOMExceptionHandling()
        {
            if (forceCOMException)
            {
                forceCOMException = false;
                throw new COMException("Forced this COMException to be thrown.");
            }
        }
        private bool forceCOMException = false;

        #endregion


        #region Element Processing

        private class KeyEventStatus
        {
            public bool keyDownFired;
            public bool keyPressFired;
            public bool keyUpFired;
        }

        public void PressKeys(IdentifierType identType, string identifier, string word, string tagName)
        {
            waitForBrowser();
            IHTMLElement elem = getElement(identType, identifier, tagName);

            _keyEvents = new List<KeyEventStatus>();

            if (!(((IHTMLElement3)elem).isDisabled) && (elem is IHTMLInputElement))
            {
                int length = word.Length;
                int elemMaxLength = ((IHTMLInputElement)elem).maxLength;
                if (elemMaxLength < length)
                    length = elemMaxLength;

                // Fire the key events for each character in the word
                char[] characters = word.ToCharArray();
                for (int charIndex = 0; charIndex < length; charIndex++)
                {
                    _keyCode = (int)characters[charIndex];

                    // onkeydown, onkeypress, onkeyup are all fired when this is called
                    fireKeyEvents((IHTMLElement3)elem);
                }

                // Build a string based on which events were fired successfully
                StringBuilder msg = new StringBuilder();
                for (int charIndex = 0; charIndex < length; charIndex++)
                {
                    string subString = word.Substring(charIndex, 1);
                    if (keyEventsFired(_keyEvents[charIndex]))
                        msg.Append(subString);
                }

                toggleElementColor((IHTMLElement)elem);

                // Set the value of the input element
                string currentElementValue = (string)elem.getAttribute("value", 0);
                if (currentElementValue != null)
                    elem.setAttribute("value", currentElementValue + msg.ToString(), 0);
                else
                    elem.setAttribute("value", msg.ToString(), 0);
            }
            else
                throw new ArgumentException("Can't type text in a non-input or disabled element.");
        }

        private bool keyEventsFired(KeyEventStatus key)
        {
            return key.keyDownFired && key.keyPressFired && key.keyUpFired;
        }

        private IHTMLElement getElement(IdentifierType identType, string identifier, string tagName)
        {
            return getElement(identType, identifier, tagName, DefaultTimeouts.FindElementTimeout);
        }

        private IHTMLElement getElement(IdentifierType identType, string identifier, string tagName, int timeOut)
        {
            IHTMLElement elem = null;
            DateTime startTime = DateTime.Now;

            while (DateTime.Now < startTime.AddSeconds(timeOut))
            {
                try
                {
                    elem = findElements(_doc, identType, identifier, tagName);
                }
                catch (Exception)
                { }

                //if (elem == null)
                //    System.Threading.Thread.Sleep(1000);
                if (elem != null)
                    break;
            }

            if (elem == null)
            {
                if (SWAT.WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotFoundException(identifier, identType, tagName);
                else
                    throw new ElementNotFoundException(identifier, identType);
            }
            return elem;
        }

        //private ExpressionToken GetInnerParentAttribute(ExpressionToken token, ref IHTMLElement element)
        //{
        //    while (token.Attribute.StartsWith("parentElement"))
        //    {
        //        isParent = true;
        //        element = element.parentElement;
        //        token.Attribute = token.Attribute.Remove(0, "parentElement.".Length);
        //    }
        //    return token;
        //}

        private IHTMLElement findElements(HTMLDocument doc, IdentifierType identType, string identifier, IdentifierExpression identExp, string tagName)
        {
            IHTMLElement elem = null;

            elem = findElement(doc, identType, identifier, identExp, tagName);
            if (elem == null)
            {
                object index = 0;
                int frames = doc.frames.length;
                for (int i = 0; i < frames; i++)
                {
                    index = i;
                    IHTMLDocument2 _docBypassSecurity = CrossFrameIE.GetDocumentFromWindow((IHTMLWindow2)doc.frames.item(ref index));
                    HTMLDocument _docFullAccess = (HTMLDocument)_docBypassSecurity;

                    //elem = findElements((HTMLDocument)((HTMLWindow2)doc.frames.item(ref index)).document, identType, identifier, tagName);
                    elem = findElements((HTMLDocument)_docFullAccess, identType, identifier, identExp, tagName);
                    if (elem != null)
                        break;
                }
            }

            return elem;
        }

        //This findElements will build the IdentifierExpression before calling the recursive findElements
        private IHTMLElement findElements(HTMLDocument doc, IdentifierType identType, string identifier, string tagName)
        {
            IdentifierExpression identExp = null;
            switch (identType)
            {
                case IdentifierType.Expression:
                    return findElements(doc, identType, identifier, new IdentifierExpression(identifier, new IsMatchHandler(IsMatchMethod)), tagName);

                //GetElementsByNAame does not work correctly in IE with frames.
                case IdentifierType.Name:
                    identType = IdentifierType.Expression;
                    identifier = "name=" + identifier;
                    return findElements(doc, identType, identifier, new IdentifierExpression(identifier, new IsMatchHandler(IsMatchMethod)), tagName);

                //GetElementByID has problems with name and id attributes under IE.
                case IdentifierType.Id:
                    //Only try getElementByID if no tags are specified otherwise do by Expression.
                    if (tagName != TagName.ALL_TAGS)
                    {
                        identType = IdentifierType.Expression;
                        identifier = "id=" + identifier;
                        return findElements(doc, identType, identifier, new IdentifierExpression(identifier, new IsMatchHandler(IsMatchMethod)), tagName);
                    }
                    break;
            }

            return findElements(doc, identType, identifier, identExp, tagName);
        }

        public IHTMLElement findElement(HTMLDocument doc, IdentifierType identType, string identifier, IdentifierExpression identExp, string tagName)
        {
            IHTMLElement elem = null;
            IHTMLElementCollection elements;
            elements = null;
            switch (identType)
            {
                //If getElementByID does not work then try byExpression which is more thorough.
                case IdentifierType.Id:
                    IHTMLElement elementFound = doc.getElementById(identifier);
                    if ((elementFound != null) && (elementFound.id == identifier))
                        return elementFound;
                    else
                        return null;

                case IdentifierType.InnerHtml:
                    elements = GetElements(tagName, doc);

                    foreach (IHTMLElement el in elements)
                    {
                        if (el.innerHTML != null && el.innerHTML.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                            return el;
                    }
                    break;

                case IdentifierType.InnerHtmlContains:
                    elements = GetElements(tagName, doc);

                    foreach (IHTMLElement el in elements)
                    {
                        if (el.innerHTML != null && el.innerHTML.IndexOf(identifier, 0, StringComparison.OrdinalIgnoreCase) > -1)
                            return el;
                    }
                    break;

                case IdentifierType.Expression:
                    elements = GetElements(tagName, doc);
                    return this.getElementByExp(identExp, elements);
            }

            return elem;
        }

        private IHTMLElementCollection GetElements(string tagName, HTMLDocument doc)
        {
            if (tagName == TagName.ALL_TAGS)
                return doc.all;
            else
                return doc.getElementsByTagName(tagName);
        }

        private IHTMLElement getElementByExp(IdentifierExpression exp, IHTMLElementCollection elements)
        {
            foreach (IHTMLElement el in elements)
            {
                if (((bool)exp.IsMatch(el, BrowserType.InternetExplorer)))
                    return el;
                else
                {
                    //special case for the closing </li> tags
                    string tagCheck = exp._identifierExpression;
                    if (tagCheck.EndsWith("</li>"))
                    {
                        el.innerHTML = el.innerHTML.Replace("\r\n", "</li>");
                        if (((bool)exp.IsMatch(el, BrowserType.InternetExplorer)))
                            return el;
                    }
                }
            }
            return null;
        }

        public void IsMatchMethod(object value, ExpressionToken token, IsMatchResult isMatchResult)
        {
            IHTMLElement elem = (IHTMLElement)value;
            string copyOfAttribute = token.Attribute;

            while (copyOfAttribute.StartsWith("parentElement."))
            {
                if (elem.parentElement == null)
                {
                    isMatchResult.ContinueChecking = false;
                    isMatchResult.ReturnValue = false;
                    return;
                }

                elem = elem.parentElement;
                copyOfAttribute = copyOfAttribute.Remove(0, "parentElement.".Length);
            }

            if (!token.Attribute.Equals(copyOfAttribute)) //if we removed parentElement we should re-normalize.
                copyOfAttribute = AttributeNormalizer.Normalize(copyOfAttribute);

            if (copyOfAttribute.StartsWith("parentWindow"))
            {
                string attr;

                isMatchResult.ContinueChecking = false;
                isMatchResult.ReturnValue = false;

                try
                {
                    attr = copyOfAttribute.Split('.')[1];
                }
                catch (Exception ex)
                {
                    //need to throw expression wrong format exception
                    throw ex;
                }

                if (attr == "location")
                {
                    if (token.IsMatch(((HTMLDocument)elem.document).parentWindow.location.href))
                    {
                        isMatchResult.ContinueChecking = true;
                        isMatchResult.ReturnValue = true;
                    }
                }

                if (attr == "name")
                {
                    if (token.IsMatch(((HTMLDocument)elem.document).parentWindow.name))
                    {
                        isMatchResult.ContinueChecking = true;
                        isMatchResult.ReturnValue = true;
                    }
                }

            }
            else
            {
                if (elem != null && !(elem is IHTMLUnknownElement))
                {
                    string attr;

                    if (copyOfAttribute.Equals("style", StringComparison.OrdinalIgnoreCase))
                    {
                        //the border, padding, and margin attributes if specified inline get applied to all 4 sides:
                        //EG: border-right, border-left, border-top, border-bottom
                        //Assumption: use -right for the global search
                        if (token.Value.ToLower().Contains("border:"))
                            token.Value = token.Value.Replace("border:", "border-right:");
                        if (token.Value.ToLower().Contains("padding:"))
                            token.Value = token.Value.Replace("padding:", "padding-right:");
                        if (token.Value.ToLower().Contains("margin:"))
                            token.Value = token.Value.Replace("margin:", "margin-right:");
                        attr = elem.style.cssText;

                    }
                    else
                    {
                        object elemAttribute = elem.getAttribute(copyOfAttribute, 0);

                        if (!(elem.getAttribute(token.Attribute, 0) is string) && !(elemAttribute is Boolean) && !(elemAttribute is int))
                            if (((IHTMLElement4)elem).getAttributeNode(copyOfAttribute) != null && ((IHTMLElement4)elem).getAttributeNode(copyOfAttribute).nodeValue != null)
                                attr = ((IHTMLElement4)elem).getAttributeNode(copyOfAttribute).nodeValue.ToString();
                            else
                                attr = elemAttribute != null ? elemAttribute.ToString() : string.Empty;
                        else
                            attr = elemAttribute.ToString();
                    }
                    if (token.IsMatch(attr))
                    {
                        isMatchResult.ContinueChecking = true;
                        isMatchResult.ReturnValue = true;
                    }
                    else
                    {
                        if (token.IsPartOfStyle)
                        {
                            string origAttr = token.Attribute;
                            string origValue = token.Value;
                            if (token.Attribute == "style" && token.Value.Contains(":"))
                            {
                                token.Attribute = token.Value.Split(new char[] { ':' })[0];
                                token.Value = token.Value.Split(new char[] { ':' })[1];
                            }
                            if (((IHTMLElement2)elem).currentStyle.getAttribute(token.Attribute, 0) != null)
                                attr = ((IHTMLElement2)elem).currentStyle.getAttribute(token.Attribute, 0).ToString();
                            if (token.IsMatch(attr))
                            {
                                isMatchResult.ContinueChecking = true;
                                isMatchResult.ReturnValue = true;
                            }
                            else
                            {
                                isMatchResult.ContinueChecking = false;
                                isMatchResult.ReturnValue = false;
                            }
                            token.Attribute = origAttr;
                            token.Value = origValue;
                        }
                        else
                        {
                            isMatchResult.ContinueChecking = false;
                            isMatchResult.ReturnValue = false;
                        }
                    }
                }
                else
                {
                    isMatchResult.ContinueChecking = false;
                    isMatchResult.ReturnValue = false;
                }
            }
        }

        #endregion


        #region Event Handling

        private void fireKeyEvents(IHTMLElement3 elem)
        {
            object dummy = null;
            object eventObj = _doc.CreateEventObject(ref dummy);

            ((IHTMLEventObj)eventObj).keyCode = _keyCode;

            KeyEventStatus thisKeyStatus = new KeyEventStatus();
            thisKeyStatus.keyDownFired = elem.FireEvent("onkeydown", ref eventObj);
            thisKeyStatus.keyPressFired = elem.FireEvent("onkeypress", ref eventObj);
            thisKeyStatus.keyUpFired = elem.FireEvent("onkeyup", ref eventObj);
            _keyEvents.Add(thisKeyStatus);
        }

        private void fireTheEvent()
        {
            ElementEventInfo elemEvtInfo = _eventInfo;

            object nl = null;
            object evObj = _doc.CreateEventObject(ref nl);

            if (elemEvtInfo.elem is IHTMLSelectElement && elemEvtInfo.eventName.Equals("onchange", StringComparison.OrdinalIgnoreCase))
            {
                IHTMLDocument4 theDoc = _doc as IHTMLDocument4;
                object eventObj = null;
                eventObj = theDoc.CreateEventObject(ref eventObj);

                try
                {
                    (elemEvtInfo.elem as HTMLSelectElement).FireEvent("onchange", ref eventObj);
                }
                catch (Exception ex) { elemEvtInfo.exception = ex; }

                return;
            }

            string evtNm = elemEvtInfo.eventName;
            if ((evtNm == "onchange") || (evtNm == "onclick") || (evtNm == "ondblclick") || (evtNm == "onmousedown") || (evtNm == "onselect") || (evtNm == "onfocus"))
            {
                try
                {
                    ((IHTMLElement2)elemEvtInfo.elem).focus();
                }
                catch { }
            }

            if (elemEvtInfo.eventName == "onclick")
            {
                try
                {
                    ((IHTMLElement)elemEvtInfo.elem).click();
                }
                catch { }
            }
            else
            {
                try
                {
                    //elemEvtInfo.elem.FireEvent(elemEvtInfo.eventName, ref evObj);
                    object temp = Type.Missing;
                    elemEvtInfo.elem.FireEvent(elemEvtInfo.eventName, ref temp);
                }
                catch (Exception ex)
                {
                    elemEvtInfo.exception = ex;
                }
            }
        }

        private class ElementEventInfo
        {
            public IHTMLElement3 elem;
            public string eventName;
            public Exception exception;
        }

        public void ElementFireEvent(IdentifierType identType, string identifier, string tagName, string eventName)
        {
            isBrowserAccessible();

            waitForBrowser();
            IHTMLElement3 elem = (IHTMLElement3)getElement(identType, identifier, tagName);
            _eventInfo = new ElementEventInfo();

            //if (isParent && eventName.Equals("onclick") && !identifier.Contains(";"))
            //{
            //    searchedParent = true;
            //    isParent = false;
            //    //multiple casting to make sure it will perform the even on the first child found
            //    if (elem is HTMLDivElement)
            //    {
            //        IHTMLElement3 childElem = (IHTMLElement3)(((HTMLDivElement)elem).firstChild);
            //        _eventInfo.elem = childElem;
            //        _eventInfo.eventName = eventName;
            //    }
            //    else if (elem is HTMLTableCellClass)
            //    {
            //        IHTMLElement3 childElem = (IHTMLElement3)(((HTMLTableCell)elem).firstChild);
            //        _eventInfo.elem = childElem;
            //        _eventInfo.eventName = eventName;
            //    }
            //    else if (elem is HTMLAnchorElement)
            //    {
            //        IHTMLElement3 childElem = (IHTMLElement3)(((HTMLAnchorElement)elem).firstChild);
            //        _eventInfo.elem = childElem;
            //        _eventInfo.eventName = eventName;
            //    }
            //}
            //else
            //{
            //    isParent = false;
            //    _eventInfo.elem = elem;
            //    _eventInfo.eventName = eventName;
            //}

            _eventInfo.elem = elem;
            _eventInfo.eventName = eventName;
            toggleElementColor((IHTMLElement)elem);
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(fireTheEvent));
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();

            //When debugging you can increase this time so the thread doesn't stop before you step into fireTheEvent
            //t.Join(20000);
            t.Join(2000);

            if (_eventInfo.exception != null)
            {
                throw new StimulateElementException(identifier, eventName);
            }

            if (t.IsAlive)
                t.Interrupt();

            // Give the operating system time in case our stimulate element caused the window to close
            Sleep(100);
        }

        #endregion


        #region JSDialog Methods

        private static string[] jsDialogWindowTitles = 
            {
                 "Microsoft Internet Explorer",
                 "Windows Internet Explorer",
                 "Message from webpage"
            };

        internal override IntPtr GetJSDialogHandle(int timeoutSeconds)
        {
            IntPtr dialogHwnd = IntPtr.Zero;

            DateTime timeout = DateTime.Now.AddSeconds(timeoutSeconds);
            while (DateTime.Now < timeout)
            {
                foreach (string title in jsDialogWindowTitles)
                {
                    dialogHwnd = NativeMethods.FindWindow("#32770", title);

                    if (dialogHwnd != IntPtr.Zero)
                        return dialogHwnd;
                }
            }

            return dialogHwnd;
        }

        private bool ClickDialogButton(IntPtr buttonHandle)
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

        private bool InteractWithJSDialog(JScriptDialogButtonType buttonType, IntPtr dialogHandle)
        {
            IntPtr buttonHandle = IntPtr.Zero;
            DateTime timeout = DateTime.Now.AddSeconds(10);
            bool clicked = false;
            string buttonText = (buttonType == JScriptDialogButtonType.Ok) ? "OK" : "Cancel";

            while (DateTime.Now < timeout)
            {
                buttonHandle = NativeMethods.FindWindowEx(dialogHandle, IntPtr.Zero, "Button", buttonText);

                if (buttonHandle == IntPtr.Zero)
                {
                    buttonText = (buttonType == JScriptDialogButtonType.Ok) ? "&Leave this Page" : "&Stay on this Page";
                    buttonHandle = NativeMethods.FindWindowEx(dialogHandle, IntPtr.Zero, "DirectUIHWND", "");
                    List<IntPtr> childWindows = NativeMethods.GetWindowChildren(buttonHandle);

                    foreach (IntPtr childHandle in childWindows)
                    {
                        buttonHandle = NativeMethods.FindWindowEx(childHandle, IntPtr.Zero, "Button", buttonText);

                        if (buttonHandle != IntPtr.Zero)
                            break;
                    }
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

        public void ClickJSDialog(JScriptDialogButtonType buttonType)
        {
            bool dialogClicked = false;

            isBrowserDocumentHTML();

            IntPtr dialogHwnd = GetJSDialogHandle(15);

            if (dialogHwnd != IntPtr.Zero)
            {
                dialogClicked = InteractWithJSDialog(buttonType, dialogHwnd);
            }

            RecoverFromDialog(buttonType);

            if (!dialogClicked)
                throw new ClickJSDialogException();
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

        [STAThread]
        public void AssertJSDialogContent(string dialogContent, int timeoutSeconds)
        {
            isBrowserDocumentHTML();

            IntPtr dlgMainHwnd = GetJSDialogHandle(timeoutSeconds);

            if (dlgMainHwnd == IntPtr.Zero)
                throw new DialogNotFoundException();

            string dialogText = GetDialogText(dlgMainHwnd, this);
            DateTime timeout = DateTime.Now.AddSeconds(timeoutSeconds);
            bool found = false;
            while(!found && DateTime.Now < timeout)
            {
                found = Convert.ToInt32(_browserVersion) <= 8
                            ? assertOldJSDialogContent(dlgMainHwnd, dialogContent)
                            : assertNewJSDialogContent(dialogText, dialogContent, dlgMainHwnd);
            }

            if (!found) //does a windows with title = content exists?
                throw new AssertionFailedException(string.Format("The open javascript dialog content is not equal to \"{0}\".", dialogContent));
        }

        private static bool assertOldJSDialogContent(IntPtr dialogHandle, string expectedDialogContent)
        {
            StringBuilder sb;
            bool found = false;
            DateTime timeout = DateTime.Now.AddSeconds(2);
            while (!found && DateTime.Now < timeout)
            {
                List<IntPtr> dialogChildren = NativeMethods.GetWindowChildren(dialogHandle);
                foreach (IntPtr child in dialogChildren)
                { //Since the dialog content is the title of one of the dialog's children,
                    //iterate though all the dialog children and inspect their titles.
                    sb = new StringBuilder(NativeMethods.GetWindowTextLength(child) + 1);
                    NativeMethods.GetWindowText(child, sb, sb.Capacity); //Get the child's title
                    if (sb.ToString().Contains(expectedDialogContent)) // Is it what we're looking for?
                    {
                        found = true;
                        break;
                    }
                }
            }

            return found;
        }

        private bool assertNewJSDialogContent(string actualDialogContent, string expectedDialogContent, IntPtr dialogHandle)
        {
            //called when using IE9 and we have received an OnBeforeUnload dialog
            bool found = false;

            actualDialogContent = actualDialogContent.Replace("\r", "").Replace("\n", "");

            int startIndex = actualDialogContent.IndexOf(':');
            actualDialogContent = actualDialogContent.Substring(startIndex + 1);

            //this part gets to the index of the second to last [ because the last part of actualDialogContent will have "actualMessage"[Leave this page][Stay on this page]
            //we want to get "actualMessage"
            int lastBracketIndex = actualDialogContent.LastIndexOf('[');
            if (lastBracketIndex < 0)
                lastBracketIndex = 0;
            actualDialogContent = actualDialogContent.Substring(0, lastBracketIndex);

            lastBracketIndex = actualDialogContent.LastIndexOf('[');
            if (lastBracketIndex < 0)
                lastBracketIndex = 0;
            actualDialogContent = actualDialogContent.Substring(0, lastBracketIndex);

            if (actualDialogContent.Contains(expectedDialogContent))
                found = true;

            if (!found && actualDialogContent.Length == 0) //onbeforeunload
                found = assertOldJSDialogContent(dialogHandle, expectedDialogContent);

            return found;
        }

        #endregion


        #region Element Setters

        public void SetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, string attributeValue, int timeOut)
        {
            isBrowserAccessible();

            waitForBrowser();
            IHTMLElement elem;

            elem = (IHTMLElement)getElement(identType, identifier, tagName, timeOut);
            toggleElementColor(elem);

            if (elem.outerHTML.Contains("type=file"))
            {
                //the element IS a file input
                SetFileInputPath(identType, identifier, attributeValue, tagName);
            }
            else if (elem.outerHTML.ToString().Contains("type=checkbox") && attributeName.Equals("checked", StringComparison.OrdinalIgnoreCase))
            {
                if (attributeValue.ToLower().Equals("true"))
                    elem.setAttribute(attributeName, " ", 0);
                else if (attributeValue.ToLower().Equals("false"))
                    elem.setAttribute(attributeName, "", 0);
            }
            else
            {
                elem.setAttribute(attributeName, attributeValue, 0);
            }
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, string attributeValue)
        {
            SetElementAttribute(identType, identifier, tagName, attributeType, attributeName, attributeValue, DefaultTimeouts.FindElementTimeout);
        }

        private void SetFileInputPath(IdentifierType identType, string identifier, string filePath, string tagName)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("Could not find file {0}", filePath));
            }

            ElementFireEvent(identType, identifier, tagName, "onclick");
            waitForBrowserReadyOnly();
            //Sleep(2000);
            StringBuilder className = new StringBuilder(255);
            StringBuilder btnName = new StringBuilder(255);

            IntPtr dialogHwnd = IntPtr.Zero;
            List<IntPtr> windowChildren = new List<IntPtr>();

            for (int i = 0; i < 30; i++)
            {
                dialogHwnd = NativeMethods.FindWindow("#32770", "Choose File");

                //Specific window title for IE8
                if (dialogHwnd == IntPtr.Zero)
                    dialogHwnd = NativeMethods.FindWindow("#32770", "Choose File to Upload");

                if (dialogHwnd != IntPtr.Zero)
                {   //Now find the combobox fill in box
                    windowChildren = NativeMethods.GetWindowChildren(dialogHwnd);
                    foreach (IntPtr dlgComboBox in windowChildren)
                    {
                        NativeMethods.GetClassName(dlgComboBox, className, className.MaxCapacity); //get the childs's type                    
                        if (string.Equals(className.ToString(), "ComboBoxEx32"))
                        {
                            IntPtr comboBoxTxt = NativeMethods.GetChildWindowHwnd(dlgComboBox, "Edit"); //find the combobox

                            ////old implementation
                            //for (int tempCtr = 0; tempCtr < filePath.Length; tempCtr++) //send the text
                            //    sendKeyPressToHwnd((uint)filePath[tempCtr], comboBoxTxt);

                            //reimplemented using .net UIAutomation library
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
                            NativeMethods.RECT placement = new NativeMethods.RECT();
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
                    //Sleep(1000);
                }
            }

            if (dialogHwnd == IntPtr.Zero)
            {
                throw new Exception(string.Format("Could not find file input dialog handle"));
            }
        }

        public void sendKeyPressToHwnd(uint keyToPress, IntPtr handle)
        {
            NativeMethods.SendMessage(handle, 0x100, keyToPress, WM_KEYDOWNlParam); //WM_KEYDOWN
            NativeMethods.SendMessage(handle, 0x102, keyToPress, WM_KEYDOWNlParam); //WM_CHAR
            NativeMethods.SendMessage(handle, 0x101, keyToPress, WM_KEYUPlParam); //WM_KEYUP
        }

        #endregion


        #region Element Getters

        public string GetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName)
        {
            return GetElementAttribute(identType, identifier, tagName, attributeType, attributeName, DefaultTimeouts.FindElementTimeout);
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, int timeOut)
        {
            isBrowserAccessible();

            waitForBrowser();
            IHTMLElement elem = null;

            elem = getElement(identType, identifier, tagName, timeOut);

            object result = elem.getAttribute(attributeName, 0);

            if (result != null && result.GetType() != typeof(System.DBNull))
                return result.ToString();

            return "";
        }

        #endregion

        #region RunScript

        protected override string runJavaScript(String theScript)
        {
            isBrowserAccessible();

            waitForBrowser();
            object index = 0;
            IHTMLElement elem = null;

            Random rand = new Random();
            String _swatTemporaryElementId = "swat" + rand.NextDouble().ToString().Replace("0.", "");

            try
            {
                IHTMLWindow2 win = _doc.parentWindow;
                theScript = "var actualResult = null; actualResult = " + theScript +
                     " var e = document.createElement('DIV');e.style.visibility=\"hidden\";e.id = '" +
                     _swatTemporaryElementId + "';e.innerHTML = actualResult; document.body.appendChild(e);";
                win.execScript(theScript, "javascript");
            }
            catch (COMException e) // There was an error in executing the script
            {
                return getError(e.ErrorCode);
            }

            try
            {
                IHTMLWindow2 win = _doc.parentWindow;
                elem = ((HTMLDocument)win.document).getElementById(_swatTemporaryElementId);
            }
            catch { }

            if (elem == null)
            {
                for (int i = 0; i < _doc.frames.length; i++)
                {
                    index = i;
                    IHTMLWindow2 win = (IHTMLWindow2)_doc.frames.item(ref index);

                    try
                    {
                        elem = ((HTMLDocument)win.document).getElementById(_swatTemporaryElementId);
                        if (elem != null) break;
                    }
                    catch { }
                }
            }

            if (elem != null)
            {
                return elem.getAttribute("innerHTML", 0).ToString();
            }
            return null;
        }

        private string getError(int errorCode)
        {
            switch (errorCode)
            {
                case -2147352319 :
                    return "Syntax Error or undefined variable";
                default:
                    return string.Format("Script error has occurred with HRESULT: 0x{0:x}, please refer to MSDN for the meaning of this result.", errorCode);
            }
        }
        
        #endregion

        #region ScreenShots Helper Methods

        public override object GetDocumentAttribute(string theAttributeName)
        {
            IHTMLDocument5 doc5 = (IHTMLDocument5)_doc;
            IHTMLDocument3 doc3 = (IHTMLDocument3)_doc;

            //compatibility mode affects how height is computed
            if ((doc3.documentElement != null) && (!doc5.compatMode.Equals("BackCompat")))
            {
                return doc3.documentElement.getAttribute(theAttributeName, 0);
            }

            return _doc.body.getAttribute(theAttributeName, 0);
        }

        public override void SetDocumentAttribute(string theAttributeName, object theAttributeValue)
        {
            IHTMLDocument5 doc5 = (IHTMLDocument5)_doc;
            IHTMLDocument3 doc3 = (IHTMLDocument3)_doc;

            //compatibility mode affects how height is computed
            if ((doc3.documentElement != null) && (!doc5.compatMode.Equals("BackCompat")))
            {
                doc3.documentElement.setAttribute(theAttributeName, theAttributeValue, 0);
            }
            else
            {
                _doc.body.setAttribute(theAttributeName, theAttributeValue, 0);
            }
        }

        public override IntPtr GetContentHandle()
        {
            IntPtr hwd = GetHwndContainingAShellDocObjectView(NativeMethods.FindWindowByClassNameAndSubstring("IEFrame", GetCurrentWindowTitle()));
            return GetHwndForInternetExplorerServer(hwd);
        }

        private static IntPtr GetHwndContainingAShellDocObjectView(IntPtr browserHWND)
        {
            // If window is a HtmlDialog then return.
            var hwnd = browserHWND;
            if (NativeMethods.CompareClassNames(hwnd, "Internet Explorer_TridentDlgFrame"))
            {
                return hwnd;
            }

            // In IE6 and previous, the handle points to a WorkerW window that is a 
            // sibling of the "Document" window (Shell DocObject View) and we can go find
            // it. 
            // In IE7, the handle now points at an intermediate layer at a sibling of the
            // TabWindowClass window which is the parent of the "Document" window. Loop
            // through these siblings to find that TabWindowClass and then drop down to
            // its children.
            hwnd = NativeMethods.GetWindow(hwnd, NativeMethods.GetWindowFlags.GW_CHILD);

            if (!NativeMethods.CompareClassNames(hwnd, "WorkerW")) // IE 7 or 8
            {
                while (hwnd != IntPtr.Zero)
                {
                    if (NativeMethods.CompareClassNames(hwnd, "TabWindowClass"))
                    {
                        break;
                    }
                    // In IE8, TabWindowClass now belongs as a child class of "Frame Tab"
                    // so step down into Frame Tab and continue start searching for 
                    // TabWindowClass there.
                    if (NativeMethods.CompareClassNames(hwnd, "Frame Tab")) // IE 8
                    {
                        //step one deeper for IE 8
                        hwnd = NativeMethods.GetWindow(hwnd, NativeMethods.GetWindowFlags.GW_CHILD);
                    }
                    else
                    {
                        hwnd = NativeMethods.GetWindow(hwnd, NativeMethods.GetWindowFlags.GW_HWNDNEXT);
                    }
                }
                hwnd = NativeMethods.GetWindow(hwnd, NativeMethods.GetWindowFlags.GW_CHILD);
            }
            return hwnd;
        }

        private static IntPtr GetHwndForInternetExplorerServer(IntPtr hwnd)
        {
            //Get Browser "Document" Handle
            while (hwnd != IntPtr.Zero)
            {
                if (NativeMethods.CompareClassNames(hwnd, "Shell DocObject View") ||
                    NativeMethods.CompareClassNames(hwnd, "Internet Explorer_TridentDlgFrame"))
                {
                    hwnd = NativeMethods.FindWindowEx(hwnd, IntPtr.Zero, "Internet Explorer_Server", IntPtr.Zero);
                    break;
                }
                hwnd = NativeMethods.GetWindow(hwnd, NativeMethods.GetWindowFlags.GW_HWNDNEXT);
            }
            return hwnd;
        }

        #endregion

        #region Window Methods

        public void AssertTopWindow(string browserTitle, int index, int timeout)
        {
            string actualTitle = "";

            int count = -1;
            DateTime end = DateTime.Now.AddSeconds(timeout);
            
            do
            {
                IntPtr topWindowHnd = NativeMethods.GetForegroundWindow();
                actualTitle = NativeMethods.GetWindowText(topWindowHnd);
                string title = actualTitle.ToLower();

                if (!title.Contains(browserTitle.ToLower()) || !title.Contains("internet explorer"))
                    continue;
                else if (index == 0 && title.Contains(browserTitle.ToLower()) && title.Contains("internet explorer"))
                    return;

                List<IntPtr> windowHnds = NativeMethods.GetAllOpenWindowsSorted();
                count = -1;
                foreach (IntPtr ptr in windowHnds)
                {
                    string windowTitle = NativeMethods.GetWindowText(ptr).ToLower();
                    if (windowTitle.Contains(browserTitle.ToLower()) && windowTitle.Contains("internet explorer") && NativeMethods.GetClassName(ptr).Equals("IEFrame"))
                    {
                        count++;

                        if (count == index)
                        {
                            if (ptr == topWindowHnd)
                                return;
                            else
                                break;
                        }
                    }
                }
            } while (DateTime.Now < end);

            
            if (index == 0)
            {
                if (SWAT.WantInformativeExceptions.GetInformativeExceptions)
                    throw new TopWindowMismatchException(browserTitle, actualTitle);
                else
                    throw new TopWindowMismatchException(browserTitle);
            }
            else
            {
                if (count < index)
                    throw new IndexOutOfRangeException("Index: " + index + " is too large, there are only " + (count+1) + " window(s) with title " + browserTitle);
                else if (SWAT.WantInformativeExceptions.GetInformativeExceptions)
                    throw new TopWindowMismatchException(browserTitle, index, actualTitle);
                else
                    throw new TopWindowMismatchException(browserTitle, index);
            }
        }

        #endregion

    }
}
