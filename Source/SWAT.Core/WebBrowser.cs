
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
using System.Linq;
using System.Text.RegularExpressions;
using SWAT.DataAccess;
using System.Threading;
using System.Reflection;

namespace SWAT
{
    public class WebBrowser : IDisposable, IBrowserCommands, IDatabaseCommands
    {
        #region Class Variables

        private const String begKeyPattern = "^(\\\\{)";
        private const String endKeyPattern = "(\\\\})$";
        private bool disposed;

        protected IBrowser _browser;
        protected Database _database;
        protected CodeRunner _codeRunner;
        protected IVariableRetriever _variableRetriver;

        private readonly BrowserType _browserType;
        private bool _forceBrowserPressKeys;
        bool _killProcess;

        private readonly AutoResetEvent _navigateBrowserCompleteEvent = new AutoResetEvent(false);

        private readonly Dictionary<string, DateTime> timers = new Dictionary<string, DateTime>();

        private readonly List<PropertyInfo> UserSettingsProperties = new List<PropertyInfo>();

        private IntPtr currentlyAttachedNonBrowserWindow = IntPtr.Zero;

        #endregion
        

        #region IBrowser Methods

        public WebBrowser(BrowserType browserType)
        {
            _browserType = browserType;
            _browser = BrowserFactory.CreateBrowser(browserType);
            initializeUserSettingsPropertiesList();
        }

        public WebBrowser(BrowserType browserType, IVariableRetriever v)
        {
            _browserType = browserType;
            _browser = BrowserFactory.CreateBrowser(browserType);
            _variableRetriver = v;
            initializeUserSettingsPropertiesList();
        }

        private void initializeUserSettingsPropertiesList()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = new Uri(Path.Combine(path, "SWAT.Core.dll")).LocalPath;            
            Assembly swatAssembly = Assembly.LoadFrom(path);

            Type[] AllTypes = swatAssembly.GetTypes();

            //Finds classes with the custom attribute UserSettingAttribute and adds its
            //PropertyInfo for each of its properties to UserSettingsProperties List
            foreach (Type currentType in AllTypes)
            {
                if (currentType.GetCustomAttributes(typeof(UserSettingAttribute), false).Length > 0)
                {
                    UserSettingsProperties.AddRange(currentType.GetProperties());
                }
            }
        }

        public void KillAllOpenBrowsers()
        {
            _browser.KillAllOpenBrowsers();
        }

        public void KillAllOpenBrowsers(string windowTitleToSpare)
        {
            _browser.KillAllOpenBrowsers(windowTitleToSpare);
        }

        public void OpenBrowser()
        {
            _browser.OpenBrowser();
        }

        public void CloseBrowser()
        {
            _browser.AssertBrowserIsAttached();                       

            _browser.CloseBrowser();
        }

        public void RefreshBrowser()
        {
            _browser.AssertBrowserIsAttached();                   

            _browser.RefreshBrowser();
        }

        public void NavigateBrowser(string url)
        {
            NavigateBrowser(url, DefaultTimeouts.WaitForDocumentLoadTimeout);
        }

        //private Type navigateBrowserExceptionThrown;

        public void NavigateBrowser(string url, int timeout)
        {
            if (timeout < 30)
                throw new ArgumentException(string.Format("A minimum timeout of 30 seconds is required for navigate browser.", timeout));

            _browser.AssertBrowserIsAttached();

            _navigateBrowserCompleteEvent.Reset();
            Thread navigationThread = new Thread(NavigateToPage);
            navigationThread.Start(url);

            if (_navigateBrowserCompleteEvent.WaitOne(new TimeSpan(0, 0, timeout)))
            {
                //HandleNavigateBrowserExceptions();
                return;
            }

            HandleInternetExplorerStoppedResponding();

            navigationThread.Abort();
            throw new NavigationTimeoutException(string.Format("Navigation timeout for url {0} at {1} second(s).", url, timeout));
        }

        private void NavigateToPage(object url)
        {
            //try
            //{
                _browser.NavigateBrowser(url as string);
            //}
            //catch (Exception e)
            //{
            //    navigateBrowserExceptionThrown = e.GetType();
            //}
                        
            _navigateBrowserCompleteEvent.Set();
        }

        //private void HandleNavigateBrowserExceptions()
        //{
        //    try
        //    {
        //        if (navigateBrowserExceptionThrown == typeof(NoAttachedWindowException))
        //        {
        //            throw new NoAttachedWindowException();
        //        }
        //    }
        //    finally
        //    {
        //        navigateBrowserExceptionThrown = null;
        //    }
        //}

        private void HandleInternetExplorerStoppedResponding()
        {
            if (_browserType == BrowserType.InternetExplorer)
            {
                _killProcess = false;

                if (Process.GetProcessesByName("iexplore").Any(iexplore => !iexplore.Responding))
                {
                    _killProcess = true;
                }

                if (_killProcess)
                {
                    foreach (Process iexplore in Process.GetProcessesByName("iexplore"))
                    {
                        iexplore.Kill();
                    }
                }
            }
        }

        public void ClickJSDialog(JScriptDialogButtonType button)
        {
            _browser.ClickJSDialog(button);
        }

        [NonUICommand]
        public void Sleep(int milliseconds)
        {
            _browser.Sleep(milliseconds);
        }

        [NonUICommand]
        public void AbandonTest()
        {
            return;
        }

        [NonUICommand]
        public void ResumeTest()
        {
            return;
        }

        public void AttachToWindow(string windowTitle)
        {
            _browser.AttachBrowserToWindow(windowTitle);
        }

        public void AttachToWindow(string windowTitle, int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("Index must be greater than or equal to 0");

            _browser.AttachBrowserToWindow(windowTitle, index);
        }

        public void AttachToNonBrowserWindow(ApplicationType appType, string title, int timeout)
        {
            AttachToNonBrowserWindow(appType, title, 0, timeout);
        }

        public void AttachToNonBrowserWindow(ApplicationType appType, string title, int index, int timeout)
        {
            if (index < 0)
                throw new IndexOutOfBoundsException("Negative indexes are not allowed in AttachToNonBrowserWindow");
            if (_browserType == BrowserType.Safari)
                ((Safari)_browser).AttachToNonBrowserWindow(appType, title, index, timeout);
            else
            {
                string progName;
                switch (appType) //in case more applications need to be supported
                {
                    default: progName = "Microsoft Excel"; break;
                }
                DateTime end = DateTime.Now.AddSeconds(timeout);
                bool success = false;
                while (DateTime.Now < end && !success)
                {
                    List<IntPtr> windowHnds = NativeMethods.GetAllOpenWindowsSorted();
                    int found = 0;
                    foreach (IntPtr ptr in from ptr in windowHnds
                                           let windowTitle = NativeMethods.GetWindowText(ptr)
                                           where windowTitle.Contains(progName) && windowTitle.Contains(title)
                                           select ptr)
                    {
                        if (found == index)
                        {
                            NativeMethods.BringWindowToTop(ptr);
                            currentlyAttachedNonBrowserWindow = ptr;
                            success = true;
                            break;
                        }
                        found++;
                    }
                }
                if (!success)
                {
                    if (index == 0)
                        throw new NonBrowserWindowExistException(title);
                    throw new NonBrowserWindowExistException(title, index);
                }
            }
        }

        public void CloseNonBrowserWindow()
        {
            if (_browserType == BrowserType.Safari)
                ((Safari)_browser).CloseNonBrowserWindow();
            else
            {
                if (currentlyAttachedNonBrowserWindow == IntPtr.Zero)
                    throw new NoAttachedWindowException();
                NativeMethods.CloseWindow(currentlyAttachedNonBrowserWindow);
                currentlyAttachedNonBrowserWindow = IntPtr.Zero;
            }
        }

        public void RunScript(string theScript, string expectedResult)
        {
            _browser.AssertBrowserIsAttached();

            string code = theScript.StartsWith("/file:") ? extractCodeFromFile(theScript) : theScript;

            _browser.RunScript("javascript", code, expectedResult);
        }

        public void RunScript(string language, string theScript, string expectedResult)
        {   
            RunScript(language, theScript, expectedResult, "");
        }

        public void RunScript(string language, string theScript, string expectedResult, string assems)
        {
            string code;
            code = theScript.StartsWith("/file:") ? extractCodeFromFile(theScript) : theScript;

            switch (language.ToLower())
            {
                case "javascript":
                    _browser.AssertBrowserIsAttached();
                    _browser.RunScript("javascript", code, expectedResult);
                    break;
                case "applescript":
                    _browser.RunScript("applescript", code, expectedResult);
                    break;
                default:
                    createCodeRunner(language);
                    _codeRunner.RunScript(language, code, expectedResult, this, assems);
                    break;
            }
        }

        public string RunScriptSaveResult(string theScript)
        {
            _browser.AssertBrowserIsAttached();

            string code = theScript.StartsWith("/file:") ? extractCodeFromFile(theScript) : theScript;

            return _browser.RunScriptSaveResult("javascript", code);
        }

        public string RunScriptSaveResult(string language, string theScript)
        {                                              
            return RunScriptSaveResult(language, theScript, "");
        }

        public string RunScriptSaveResult(string language, string theScript, string assems)
        {
            string code;
            code = theScript.StartsWith("/file:") ? extractCodeFromFile(theScript) : theScript;

            switch (language.ToLower())
            {
                case "javascript":
                    _browser.AssertBrowserIsAttached();
                    return _browser.RunScriptSaveResult(language, code);
                case "applescript":
                    return _browser.RunScriptSaveResult(language, code);
                default:
                    createCodeRunner(language);
                    return _codeRunner.RunScriptSaveResult(language, code, this, assems);
            }
        }

        private void createCodeRunner(string language)
        {
            switch (language.ToLower())
            {
                case "csharp":
                    _codeRunner = CodeRunnerFactory.CreateRunner(RunnerType.CSharp, _variableRetriver);
                    break;
                default:
                    throw new LanguageNotImplementedException(language + " is not implemented.");
            }
        }

        private static string extractCodeFromFile(string filename)
        {
            filename = filename.Remove(0, 6);
            string extractedCode = String.Empty;
            using (StreamReader reader = new StreamReader(filename))
            {
                try
                {
                    do
                        extractedCode += reader.ReadLine() + Environment.NewLine + " ";
                    while (reader.Peek() != -1);
                }
                finally
                {
                    reader.Close();
                }
            }
                
            return extractedCode;
        }

        [NonUICommand]
        public string DisplayVariable(string varName)
        {
            return _variableRetriver.Recall(varName);
        }

        #endregion


        #region Setters

        public void SetElementAttribute(IdentifierType identType, string identifier, string attributeName, string attributeValue)
        {
            SetElementAttribute(identType, identifier, AttributeType.BuiltIn, attributeName, attributeValue, TagName.ALL_TAGS);
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, string attributeName, string attibuteValue, string tagName)
        {
            SetElementAttribute(identType, identifier, AttributeType.BuiltIn, attributeName, attibuteValue, tagName);
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, AttributeType attributeType, string attributeName, string attributeValue)
        {
            SetElementAttribute(identType, identifier, attributeType, attributeName, attributeValue, TagName.ALL_TAGS);
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, AttributeType attributeType, string attributeName, string attributeValue, string tagName)
        {
            _browser.AssertBrowserIsAttached();                  

            attributeName = Browser.AttributeNormalizer.Normalize(attributeName);
            _browser.SetElementAttribute(identType, identifier, tagName, attributeType, attributeName, attributeValue);
        }

        public void SetWindowPosition(WindowPositionTypes windowPositionType)
        {
            _browser.AssertBrowserIsAttached();
            _browser.SetWindowPosition(windowPositionType);
        }

        public void PressKeys(string word)
        {
            PressKeys(word, true, "");
        }
        
        public void PressKeys(string word, bool checkAttached, string windowTitle)
        {
            PressKeys(word, 1, checkAttached, windowTitle);
        }

        public void PressKeys(string word, int repeatTimes)
        {
            PressKeys(word, repeatTimes, true, "");
        }

        public void PressKeys(string word, int repeatTimes, bool checkAttached, string windowTitle)
        {
            for (int i = 0; i < repeatTimes; i++)
            {
                PressKeys(IdentifierType.Name, "", word, "", false, checkAttached, windowTitle);
            }
        }

        public void PressKeys(IdentifierType identType, string identifier, string word)
        {
            PressKeys(identType, identifier, word, TagName.ALL_TAGS);
        }

        public void PressKeys(IdentifierType identType, string identifier, string word, string tagName)
        {
            PressKeys(identType, identifier, word, tagName, true);
        }

        private void PressKeys(IdentifierType identType, string identifier, string word, string tagName, bool focusOnSpecificElement)
        {
            PressKeys(identType, identifier, word, tagName, focusOnSpecificElement, true, "");
        }

        private void PressKeys(IdentifierType identType, string identifier, string word, string tagName, bool focusOnSpecificElement, bool checkAttached, string windowTitle)
        {
            if (checkAttached)
                _browser.AssertBrowserIsAttached();

            _browser.Sleep(200);

            if (focusOnSpecificElement)
            {
                //Let each browser find the element in its own way, then stimulate it by selecting the element.
                _browser.ElementFireEvent(identType, identifier, tagName, "onfocus");
            }
            else if (checkAttached)
            {
                _browser.WaitForBrowserReadyState();
            }

            if (_browserType == BrowserType.Safari)
            {
                _browser.PressKeys(identType, identifier, word, tagName);
                return;
            }

            bool isUnlockedDesktop = NativeMethods.IsDesktopUnlocked();
            KeyboardInput keyboard = new KeyboardInput(_browser);

            Regex begPattern = new Regex(begKeyPattern);
            if (begPattern.IsMatch(word))
            {
                Regex endPattern = new Regex(endKeyPattern);
                if (!endPattern.IsMatch(word))
                    throw new ArgumentException("The key code sequence has not been completed. Please add a \\} at the end of the keyword.");

                if (!isUnlockedDesktop || _forceBrowserPressKeys)
                    throw new LockedDesktopEnvironmentException();

                string keyValue = word.Substring(2, word.Length - 4);

                if (keyValue.ToUpper().StartsWith("ALT"))
                {
                    keyboard.ProcessAltKeyCombination(keyValue);
                }
                else
                {
                    if (keyValue.ToUpper().Equals("SHIFT+TAB"))
                    {
                        keyboard.ProcessShiftMappedKey(GetMappedKey("TAB"));
                    }
                    else
                    {
                        keyboard.ProcessKey(GetMappedKey(keyValue));
                    }
                }
                keyboard.SendInputString(windowTitle == "" ? _browser.GetCurrentWindowTitle() : windowTitle);
            }
            else if (((_browserType == BrowserType.FireFox) || !isUnlockedDesktop || _forceBrowserPressKeys) && focusOnSpecificElement)
            {
                _browser.PressKeys(identType, identifier, word, tagName);
            }
            else if (!focusOnSpecificElement && (!isUnlockedDesktop || _forceBrowserPressKeys))
            {
                throw new LockedDesktopEnvironmentException();
            }
            else
            {
                foreach (char t in word)
                {
                    string asciiValue = _browser.getAsciiValue(t);
                    if (!asciiValue.Equals(""))
                        keyboard.ProcessInternationalKey(asciiValue);
                    else
                        keyboard.ProcessKey(NativeMethods.VkKeyScan(t));
                }

                keyboard.SendInputString(windowTitle == "" ? _browser.GetCurrentWindowTitle() : windowTitle);
                _browser.Sleep((13 * word.Length) + 500); // Put in for longer strings to finish firing events in the browser
            }
        }

        private uint GetMappedKey(string keyValue)
        {
            uint mappedKey = _browser.getKeyValue(keyValue.ToUpper());
            if (mappedKey == 0)
                throw new ArgumentException(string.Format("There is no key with name {0} in the configuration table.", keyValue));
            return mappedKey;
        }

        [NonUICommand]
        public string SetVariable(string value)
        {
            return value;
        }

        #endregion


        #region Events

        public void StimulateElement(IdentifierType identType, string identifier, string eventName)
        {
            StimulateElement(identType, identifier, eventName, TagName.ALL_TAGS);
        }

        public void StimulateElement(IdentifierType identType, string identifier, string eventName, string tagName)
        {
            _browser.AssertBrowserIsAttached();                 

            Boolean correctName = Events.Names.Any(theEvent => eventName.ToLower().Equals(theEvent));
            if (correctName)
            {
                _browser.ElementFireEvent(identType, identifier, tagName, eventName.ToLower());
            }
            else
            {
                throw new InvalidEventException(eventName);
            }
        }

        #endregion


        #region Getters

        public string GetElementAttribute(IdentifierType identType, string identifier, string attributeName)
        {
            return GetElementAttribute(identType, identifier, AttributeType.BuiltIn, attributeName, TagName.ALL_TAGS);
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, string attributeName, string tagName)
        {
            return GetElementAttribute(identType, identifier, AttributeType.BuiltIn, attributeName, tagName);
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, AttributeType attributeType, string attributeName)
        {
            return GetElementAttribute(identType, identifier, attributeType, attributeName, TagName.ALL_TAGS);
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, AttributeType attributeType, string attributeName, string tagName)
        {
            _browser.AssertBrowserIsAttached();                     

            attributeName = Browser.AttributeNormalizer.Normalize(attributeName);
            return _browser.GetElementAttribute(identType, identifier, tagName, attributeType, attributeName);
        }

        public string GetLocation()
        {
            _browser.AssertBrowserIsAttached();
                

            return _browser.GetCurrentLocation();
        }

        public string GetWindowTitle()
        {
            _browser.AssertBrowserIsAttached();
                

            return _browser.GetCurrentDocumentTitle();
        }

        #endregion


        #region Assertions

        public void AssertTopWindow(string browserTitle)
        {
            AssertTopWindow(browserTitle, 0);
        }

        public void AssertTopWindow(string browserTitle, int index)
        {
            AssertTopWindow(browserTitle, index, SWAT.DefaultTimeouts.AssertBrowserExists);
        }

        public void AssertTopWindow(string browserTitle, int index, int timeout)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("Negative indexes are not allowed in AssertTopWindow");

            _browser.WaitForBrowserReadyState();

            _browser.AssertTopWindow(browserTitle, index, timeout);
        }

        private void AssertDoesElementExist(IdentifierType identType, string identifier, bool expectedResult, string tagName, int timeOut)
        {
            _browser.AssertBrowserIsAttached();

            if (timeOut > 0)
            {
                try
                {
                    _browser.GetElementAttribute(identType, identifier, tagName, AttributeType.BuiltIn, "tagName", timeOut);

                    if (!expectedResult)
                    {
                        if (WantInformativeExceptions.GetInformativeExceptions)
                            throw new AssertionFailedException(string.Format("Element with {0} {1} and tag {2} was found.", Enum.GetName(typeof(IdentifierType), identType), identifier, tagName));
                        throw new AssertionFailedException(string.Format("Element with {0} {1} was found.", Enum.GetName(typeof(IdentifierType), identType), identifier));
                    }

                    //return true;
                }
                catch (ElementNotFoundException)
                {
                    if (expectedResult)
                    {
                        if (WantInformativeExceptions.GetInformativeExceptions)
                            throw new AssertionFailedException(string.Format("Element with {0} {1} and tag {2} was not found.", Enum.GetName(typeof(IdentifierType), identType), identifier, tagName));
                        throw new AssertionFailedException(string.Format("Element with {0} {1} was not found.", Enum.GetName(typeof(IdentifierType), identType), identifier));
                    }
                }
            }
            else
                //When the time is 0, throw exception
                throw new ArgumentException(string.Format("The timeout must be larger than 0ms."));
        }

        public void AssertElementExists(IdentifierType identType, string identifier)
        {
            AssertDoesElementExist(identType, identifier, true, TagName.ALL_TAGS, DefaultTimeouts.DoesElementExistTimeout);
        }

        public void AssertElementExists(IdentifierType identType, string identifier, string tagName)
        {
            AssertDoesElementExist(identType, identifier, true, tagName, DefaultTimeouts.DoesElementExistTimeout);
        }

        public void AssertElementExistsWithTimeout(IdentifierType identType, string identifier, double timeOutLengthMilliseconds)
        {
            AssertDoesElementExist(identType, identifier, true, TagName.ALL_TAGS, (int)Math.Ceiling(timeOutLengthMilliseconds / 1000));
        }

        public void AssertElementExistsWithTimeout(IdentifierType identType, string identifier, double timeOutLengthMilliseconds, string tagName)
        {
            // Pass the rounded up timeout in milliseconds in seconds!!      
            AssertDoesElementExist(identType, identifier, true, tagName, (int)Math.Ceiling(timeOutLengthMilliseconds / 1000));
        }

        public void AssertElementDoesNotExist(IdentifierType identType, string identifier)
        {
            AssertElementDoesNotExist(identType, identifier, TagName.ALL_TAGS);
        }

        public void AssertElementDoesNotExist(IdentifierType identType, string identifier, string tagName)
        {
            _browser.AssertBrowserIsAttached();                 

            int assertElementExistsTimeOut = DefaultTimeouts.DoesElementNotExistTimeout;
            DateTime endTime = DateTime.Now.AddSeconds(assertElementExistsTimeOut);

            do
            {
                AssertDoesElementExist(identType, identifier, false, tagName, assertElementExistsTimeOut);
            }
            while (DateTime.Now < endTime);

        }

        public void AssertBrowserType(string browserName)
        {
#if MACOSX
        if(!browserName.Equals("Safari", StringComparison.OrdinalIgnoreCase))
                throw new IncorrectBrowserException("Browser is not Safari");
#else
            browserName = browserName.ToLower();
            switch (browserName)
            {
                case "internetexplorer":
                    if (!(_browser is InternetExplorer))
                        throw new IncorrectBrowserException("Browser is not InternetExplorer");
                    break;
                case "firefox":
                    if (!(_browser is FireFox))
                        throw new IncorrectBrowserException("Browser is not FireFox");
                    break;
                case "chrome":
                    if (!(_browser is Chrome))
                        throw new IncorrectBrowserException("Browser is not Chrome");
                    break;
                case "safari":
                    if (!(_browser is Safari))
                        throw new IncorrectBrowserException("Browser is not Safari");
                    break;
                default: throw new ArgumentException(browserName + " is not a valid browser type.");
            }
#endif
        }

        public void AssertJSDialogContent(string dialogContent)
        {
            _browser.AssertJSDialogContent(dialogContent);
        }

        public void AssertJSDialogContent(string dialogContent, double timeOutMilliseconds)
        {
            if (timeOutMilliseconds > 0)
                _browser.AssertJSDialogContent(dialogContent, (int)Math.Ceiling(timeOutMilliseconds / 1000));
            else
                throw new AssertionFailedException("Timeout must be greater than 0 seconds");
        }

        public void AssertBrowserDoesNotExist(string windowTitle)
        {
            _browser.AssertBrowserDoesNotExist(windowTitle);
        }

        public void AssertBrowserDoesNotExist(string windowTitle, int timeOut)
        {
            if (timeOut > 0)
                _browser.AssertBrowserDoesNotExist(windowTitle, timeOut);
            else
                throw new AssertionFailedException("Timeout must be greater than 0 seconds");
        }

        public void AssertBrowserExists(string windowTitle)
        {
            _browser.AssertBrowserExists(windowTitle);
        }

        public void AssertBrowserExists(string windowTitle, int timeOut)
        {
            if (timeOut > 0)
                _browser.AssertBrowserExists(windowTitle, timeOut);
            else
                throw new AssertionFailedException("Timeout must be greater than 0 seconds");

        }

        [NonUICommand]
        public void AssertSWATVersionNumber(string versionNum)
        {
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(2);

            if (currentVersion.CompareTo(versionNum) < 0 ||
                 currentVersion.CompareTo(versionNum) > 0)
            {
                throw new InvalidDataException("Current version: " + currentVersion);
            }
        }

        public void AssertElementIsActive(IdentifierType identType, string identifier, string tagName)
        {
            AssertElementIsActive(identType, identifier, tagName, DefaultTimeouts.FindElementTimeout);
        }

        public void AssertElementIsActive(IdentifierType identType, string identifier, string tagName, int timeoutSeconds)
        {
            _browser.AssertBrowserIsAttached();
            try
            {
                _browser.AssertElementIsActive(identType, identifier, tagName, timeoutSeconds);
            }
            catch (ElementNotActiveException e)
            {
                throw new AssertionFailedException(e.Message);
            }
            catch (ElementNotFoundException e)
            {
                throw new AssertionFailedException(e.Message);
            }
        }

        #endregion


        #region Comparison Methods

        [NonUICommand]
        public void AssertLessThan(String value1, String value2)
        {
            value1 = value1.ToLower();
            value2 = value2.ToLower();
            int v1;
            int v2;
            if ((int.TryParse(value1, out v1)) && (int.TryParse(value2, out v2)))
            {
                if (v1.CompareTo(v2) >= 0)
                    throw new ComparisonFailedException(value1 + " is not less than " + value2);
            }
            else
            {
                if (value1.CompareTo(value2) >= 0)
                    throw new ComparisonFailedException(value1 + " is not less than " + value2);
            }
        }

        [NonUICommand]
        public void AssertGreaterThan(String value1, String value2)
        {
            value1 = value1.ToLower();
            value2 = value2.ToLower();
            int v1;
            int v2;
            if ((int.TryParse(value1, out v1)) && (int.TryParse(value2, out v2)))
            {
                if (v1.CompareTo(v2) <= 0)
                    throw new ComparisonFailedException(value1 + " is not greater than " + value2);
            }
            else
            {
                if (value1.CompareTo(value2) <= 0)
                    throw new ComparisonFailedException(value1 + " is not greater than " + value2);
            }
        }

        [NonUICommand]
        public void AssertLessThanOrEqual(String value1, String value2)
        {
            value1 = value1.ToLower();
            value2 = value2.ToLower();
            int v1;
            int v2;
            if ((int.TryParse(value1, out v1)) && (int.TryParse(value2, out v2)))
            {
                if (v1.CompareTo(v2) > 0)
                    throw new ComparisonFailedException(value1 + " is not less than or equal to " + value2);
            }
            else
            {
                if (value1.CompareTo(value2) > 0)
                    throw new ComparisonFailedException(value1 + " is not less than or equal to " + value2);
            }
        }

        [NonUICommand]
        public void AssertGreaterThanOrEqual(String value1, String value2)
        {
            value1 = value1.ToLower();
            value2 = value2.ToLower();
            int v1;
            int v2;
            if ((int.TryParse(value1, out v1)) && (int.TryParse(value2, out v2)))
            {
                if (v1.CompareTo(v2) < 0)
                    throw new ComparisonFailedException(value1 + " is not greater than or equal to " + value2);
            }
            else
            {
                if (value1.CompareTo(value2) < 0)
                    throw new ComparisonFailedException(value1 + " is not greater than or equal to " + value2);
            }
        }

        [NonUICommand]
        public void AssertEqualTo(String value1, String value2)
        {
            value1 = value1.ToLower();
            value2 = value2.ToLower();
            int v1;
            int v2;
            if ((int.TryParse(value1, out v1)) && (int.TryParse(value2, out v2)))
            {
                if (v1.CompareTo(v2) != 0)
                    throw new ComparisonFailedException(value1 + " is not equal to " + value2);
            }
            else
            {
                if (value1.CompareTo(value2) != 0)
                    throw new ComparisonFailedException(value1 + " is not equal to " + value2);
            }
        }

        #endregion


        #region Database Methods

        [NonUICommand]
        public void ConnectToMssql(string serverName, string userName, string password)
        {
            ConnectToMssql(serverName, userName, password, 15);
        }
		
		[NonUICommand]
        public void ConnectToMssql(string serverName, string userName, string password, int connectionTimeout)
        {
            _database = new MSSql();
            _database.Connect(serverName, userName, password, connectionTimeout);
        }

		[NonUICommand]
        public void ConnectToOracle(string serverName, string userName, string password)
        {
            _database = new Oracle();
            _database.Connect(serverName, userName, password);
        }

		[NonUICommand]
        public void SetDatabase(string newDb)
        {
            _database.SetDatabase(newDb);
        }

		[NonUICommand]
        public void Disconnect()
        {
            _database.Disconnect();
        }

        private static bool IsQueryInFile(string query)
        {
            return query.StartsWith("/file:");
        }

        private static string GetQueryString(string filePath)
        {
            filePath = filePath.Remove(0, 6);
            if (File.Exists(filePath))
            {
                TextReader tr = new StreamReader(filePath);
                return tr.ReadToEnd();
            }

            throw new FileNotFoundException();
        }

		[NonUICommand]
        public void SetQuery(string SQL)
        {
            if (IsQueryInFile(SQL))
            {
                string[] split = SplitGos(SQL);
                foreach (string str in split)
                {
                    if(string.IsNullOrEmpty(str.Trim()))
                        continue;

                    _database.SetQuery(str);
                }
            }
            else
                _database.SetQuery(SQL);
        }
		
		[NonUICommand]
        public void SetQuery(string SQL, int timeout)
        {
            if (IsQueryInFile(SQL))
            {
                string[] split = SplitGos(SQL);
                foreach (string str in split)
                {
                    if (string.IsNullOrEmpty(str.Trim()))
                        continue;

                    _database.SetQuery(str, timeout);
                }
            }
            else
                _database.SetQuery(SQL, timeout);
        }

        private static string[] SplitGos(string SQL)
        {
            string go = @"(^|\s)GO(\s|$)";
            string[] split = Regex.Split(GetQueryString(SQL), go, RegexOptions.IgnoreCase);
            return split;
        }

		[NonUICommand]
        public void AssertRecordCount(int expectedNumber)
        {
            _database.AssertRecordCount(expectedNumber);
        }

		[NonUICommand]
        public string GetDbDate()
        {
            return GetDbDate(101);
        }

		[NonUICommand]
        public string GetDbDate(int format)
        {
            return GetDbDate(format, false);
        }

		[NonUICommand]
        public string GetDbDate(int format, bool removeZero)
        {
            return _database.GetDbDate(format, removeZero);
        }

		[NonUICommand]
        public void SaveDbDate()
        {
            SaveDbDate(101);
        }

		[NonUICommand]
        public void SaveDbDate(int format)
        {
            SaveDbDate(format, false);
        }

		[NonUICommand]
        public void SaveDbDate(int format, bool removeZero)
        {
            _database.SaveDbDate(format, removeZero);

        }

		[NonUICommand]
        public string GetSavedDbDateMonth()
        {
            return _database.GetSavedDbDateMonth();
        }

		[NonUICommand]
        public string GetSavedDbDateDay()
        {
            return _database.GetSavedDbDateDay();
        }

		[NonUICommand]
        public string GetSavedDbDateYear()
        {
            return _database.GetSavedDbDateYear();
        }

		[NonUICommand]
        public string GetSavedDbDate(string part)
        {
            return _database.GetSavedDbDate(part);
        }

		[NonUICommand]
        public string GetDbRecord(int row, int col)
        {
            return _database.GetRecord(row, col);
        }

		[NonUICommand]
        public string GetDbRecord()
        {
            return GetDbRecord(0, 0);
        }

		[NonUICommand]
        public string GetDbRecordByColumnName(string name)
        {
            return GetDbRecordByColumnName(0, name);
        }

		[NonUICommand]
        public string GetDbRecordByColumnName(int row, string name)
        {
            return _database.GetRecord(row, name);
        }

		[NonUICommand]
        public void AssertRecordValues(string values)
        { AssertRecordValues(0, 0, values); }

		[NonUICommand]
        public void AssertRecordValues(int row, int col, string values)
        {
            _database.AssertRecordValues(row, col, values);
        }

		[NonUICommand]
        public void AssertRecordValuesByColumnName(string colName, string values)
        { AssertRecordValuesByColumnName(0, colName, values); }

		[NonUICommand]
        public void AssertRecordValuesByColumnName(int row, string colName, string values)
        { _database.AssertRecordValues(row, colName, values); }

		[NonUICommand]
        public void AssertDBRecordExistsWithTimeout(String sql, int timeout)
        { _database.AssertDBRecordExistsWithTimeout(sql, timeout); }

		[NonUICommand]
        public void UpdateTable(string SQL)
        {
            _database.UpdateTable(SQL);
        }

		[NonUICommand]
        public void InsertIntoTable(string SQL)
        {
            _database.InsertIntoTable(SQL);
        }

		[NonUICommand]
        public void DeleteFromTable(string SQL)
        {
            _database.DeleteFromTable(SQL);
        }

		[NonUICommand]
        public void BackupTable(string tablename)
        {
            _database.BackupTable(tablename);
        }

		[NonUICommand]
        public void BackupTable(string tablename, string filter)
        {
            _database.BackupTable(tablename, filter);
        }

		[NonUICommand]
        public void RestoreTable(string tableName)
        {
            _database.RestoreTable(tableName);
        }

		[NonUICommand]
        public void RestoreTable(string tableName, string filter)
        {
            _database.RestoreTable(tableName, filter);
        }

		[NonUICommand]
        public void RestoreAllTables()
        {
            _database.RestoreAllTables();
        }

		[NonUICommand]
        public void BeginCompareData()
        {
            //used when running tests through the console because of the loop foreach(wikicommand in wikicommands)
            //begin/endcomparedata would not get called if these were not incldued in WebBrowser
            return;
        }

		[NonUICommand]
        public void EndCompareData()
        {
            //used when running tests through the console because of the loop foreach(wikicommand in wikicommands)
            //begin/endcomparedata would not get called if these were not incldued in WebBrowser
            return;
        }
        #endregion


        #region Configuration Methods

        [NonUICommand]
        public void SetConfigurationItem(string configName, string value) 
        {
            bool formatExceptionThrown = false;

            foreach (PropertyInfo currentProperty in UserSettingsProperties)
            {
                if (String.Equals(currentProperty.Name, configName, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        currentProperty.SetValue(null, Convert.ChangeType(value, currentProperty.PropertyType), null);
                    }
                    catch(FormatException)
                    {
                        formatExceptionThrown = true;
                    }

                    if (!UserConfigHandler.LastSettingSuccessful || formatExceptionThrown)
                    {
                        throw new ConfigurationItemException(String.Format("Invalid value for this setting: {0}", value));
                    }
                    UserConfigHandler.Save();
                    return;
                }
            }
            throw new ConfigurationItemException(String.Format("Invalid setting name: {0}", configName));
        }

        [NonUICommand]
        public string GetConfigurationItem(string configName)
        {            
            foreach (PropertyInfo currentProperty in UserSettingsProperties)
            {
                if (String.Equals(currentProperty.Name, configName, StringComparison.OrdinalIgnoreCase))
                {
                    return currentProperty.GetGetMethod().Invoke(null, null).ToString();
                }
            }
            throw new ConfigurationItemException(String.Format("Invalid setting name: {0}", configName));
        }

        #endregion

        #region ScreenShots

        public string TakeScreenshot(string filePrefix)
        {
            return _browser.TakeScreenshot(filePrefix);
        }

        #region ScreenShots Helper Methods

        //public IntPtr GetContentHandle()
        //{
        //    return _browser.GetContentHandle();
        //}

        //public void SetDocumentAttribute(string theAttributeName, object theAttributeValue)
        //{
        //    _browser.SetDocumentAttribute(theAttributeName, theAttributeValue);
        //}

        //public object GetDocumentAttribute(string theAttributeName)
        //{
        //    return _browser.GetDocumentAttribute(theAttributeName);
        //}

        #endregion

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
                    

                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                if(_database != null)
                    _database.Dispose();
                if (_browser is FireFox)
                    ((FireFox)_browser).Dispose();
                if (_browser is Safari)
                    ((Safari)_browser).Dispose();

                // Note disposing has been done.
                disposed = true;
            }

        }

        #endregion
        
        #region PSR Commands

        [NonUICommand]
        public void StartTimer(string timerName)
        {
            DateTime newTimer = DateTime.Now;
            timers.Add(timerName, newTimer);
        }

        [NonUICommand]
        public void CheckTimer(string timerName, string op, int targetTime)
        {
            if (!(op.Equals("LessThan") || op.Equals("LessThanOrEqualTo") || op.Equals("EqualTo") || op.Equals("GreaterThan") || op.Equals("GreaterThanOrEqualTo")))
                throw new ArgumentException("Invalid operator");
            AssertTimerExists(timerName);
            if (targetTime < 0)
                throw new ArgumentException("Target time must be a positive value");
            TimeSpan elapsed = GetTimeElapsed(timerName);
            int millis = elapsed.Milliseconds;
            millis += elapsed.Seconds * 1000;
            millis += elapsed.Minutes * 60 * 1000;
            string error = "Timer " + timerName + " equals " + millis;
            if (op.Equals("LessThan") && !(millis < targetTime))
                throw new AssertionFailedException(error);
            if (op.Equals("LessThanOrEqualTo") && !(millis <= targetTime))
                throw new AssertionFailedException(error);
            if (op.Equals("EqualTo") && !(millis >= targetTime - 5 && millis <= targetTime + 5))
                throw new AssertionFailedException(error);
            if (op.Equals("GreaterThan") && !(millis > targetTime))
                throw new AssertionFailedException(error);
            if (op.Equals("GreaterThanOrEqualTo") && !(millis >= targetTime))
                throw new AssertionFailedException(error);
        }

        [NonUICommand]
        public void ResetTimer(string timerName)
        {
            if (!timers.ContainsKey(timerName))
                throw new TimerDoesNotExistException("Timer does not exist");
            timers[timerName] = DateTime.Now;
        }

        [NonUICommand]
        public int GetTimerValue(string timerName)
        {
            AssertTimerExists(timerName);
            return (int)GetTimeElapsed(timerName).TotalMilliseconds;
        }

        [NonUICommand]
        public int DisplayTimerValue(string timerName)
        {
            return GetTimerValue(timerName);            
        }

        #endregion

        #region Helper Functions

        private TimeSpan GetTimeElapsed(string timerName)
        {
            DateTime timer = timers[timerName];
            DateTime now = DateTime.Now;
            return now.Subtract(timer);
        }

        private void AssertTimerExists(string timerName)
        {
            if (!timers.ContainsKey(timerName))
                throw new TimerDoesNotExistException(timerName + " does not exist");
        }
                
        #endregion

    }
}
