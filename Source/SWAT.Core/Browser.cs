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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using SWAT.Configuration;
using SWAT.Configuration.Normalization;

namespace SWAT
{
    public abstract class Browser : IDocumentInfo 
    {
        #region Constructor

        public Browser(BrowserType type, BrowserProcess process)
        {
            browserType = type;
            ProcessName = process;
        }

        #endregion

        #region Browser Class variables

        internal IntPtr curWindowHandle;
        internal readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        internal readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        protected BrowserProcess processName;
        protected LastBrowserWindowAction lastBrowserWindowAction = LastBrowserWindowAction.InUse;
        protected BrowserType browserType { get; set; }
        private string dialogText = "";


        #endregion

        #region LastBrowserWindowAction

        public enum LastBrowserWindowAction
        {
            Closed,
            Killed,
            KilledExceptTitle,
            InUse,
            Refreshed
        }

        #endregion

        #region JS Dialog

        internal abstract IntPtr GetJSDialogHandle(int timeoutSeconds);

        protected string GetDialogText(IntPtr dialogHandle, IBrowser browser)
        {
            DateTime timeout = DateTime.Now.AddSeconds(1);
            dialogText = "";
            string lastDialogText = dialogText;
            while (lastDialogText == dialogText && DateTime.Now < timeout)
            {
                KeyboardInput input = new KeyboardInput(browser);
                input.Copy(dialogHandle);

                Thread t = new Thread(new ThreadStart(GetClipboardText));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();
            }

            return dialogText;
        }

        [NCover.CoverageExcludeAttribute] //Because the catch statement doesn't always run
        private void GetClipboardText()
        {
            try
            {
                IDataObject clipboardObj = Clipboard.GetDataObject();
                if (clipboardObj.GetDataPresent(DataFormats.Text))
                {
                    dialogText = clipboardObj.GetData(DataFormats.Text) as string;
                    Clipboard.Clear();
                }
            }
            catch { }
        }

        #endregion

        #region RunScript

        public String RunScriptSaveResult(String language, String theScript)
        {
            string result;
            string resultLang;

            if (language == "javascript")
            {
                resultLang = "Javascript";
                result = runJavaScript(theScript);
            }
            else
            {
                resultLang = "Applescript";
                result = runAppleScript(theScript);
            }

            if (String.IsNullOrEmpty(result) || IsRunScriptError(result))
                throw new ArgumentException(String.Format("{0} yielded no results. Error : {1}", resultLang, result));

            return result;
        }

        public void RunScript(String language, String theScript, String expectedResult)
        {
            string result = language == "javascript" ? runJavaScript(theScript) : runAppleScript(theScript);

            if (result != expectedResult)
                throw new AssertionFailedException("Mismatching results. Expected: " + expectedResult + " , - Actual: " + result);
        }

        private static bool IsRunScriptError(string result)
        {
            result = result.ToLower();
            string[] errorMsgs = { "error", "undefined", "unexpected", "can't find", "is not defined", "(null)", "{\n}", "socket disconnected" };
            return errorMsgs.Any(s => result.Contains(s));
        }

        protected abstract string runJavaScript(string theScript);
        protected virtual string runAppleScript(string theScript)
        {
            throw new InvalidOperationException("AppleScript can only be executed on a Mac.");
        }

        #endregion

        #region Screenshots

        public virtual string TakeScreenshot(string filePrefix)
        {
            string image = "";

            if (SWAT.ScreenShotSettings.SnapShotOption == true)
            {
                //Handles scenario where SnapShotFolder is set using "DriveLetter:\" instead of "\\MacineName\DriveLetter$\".
                //This occurs if user's SWAT.user.config file originated from an older version of SWAT where the SnapShotFolder
                //setting was not checking and updating the format of the folder's path.
                if (SWAT.ScreenShotSettings.SnapShotFolder.Contains(@":"))
                {
                    //Uses the logic from the SnapShotFolder's set method to correctly format the SnapShotFolder path
                    SWAT.ScreenShotSettings.SnapShotFolder = SWAT.ScreenShotSettings.SnapShotFolder;
                    SWAT.UserConfigHandler.Save();
                }
                if (SWAT.ScreenShotSettings.ScreenShotAllScreens == true)
                {
                    SWAT.ErrorSnapShot capture = new SWAT.ErrorSnapShot();
                    image = capture.CaptureAllScreens(SWAT.ScreenShotSettings.SnapShotFolder, filePrefix);
                }
                if (SWAT.ScreenShotSettings.ScreenShotBrowser == true)
                {
                    try
                    {
                        SWAT.ErrorSnapShot capture = new SWAT.ErrorSnapShot(this as IDocumentInfo, browserType);
                        image = capture.CaptureBrowser(SWAT.ScreenShotSettings.SnapShotFolder, filePrefix, GetContentHandle());
                    }
                    catch
                    {
                        image = "Unable to take picture since no browser and/or document was present.";
                    }
                }
            }
            return image;
        }

        public abstract IntPtr GetContentHandle();

        public abstract void SetDocumentAttribute(string theAttributeName, object theAttributeValue);

        public abstract object GetDocumentAttribute(string theAttributeName);

        #endregion

        #region Public helper methods

        public uint getKeyValue(string key)
        {
            KeyMappingSection configSection;
            //0 is default of enum when not set. Safari does not set process name
            if (browserType == BrowserType.Safari)
                configSection = ConfigurationSections.GetMacKeyMappingSection();
            else
                configSection = ConfigurationSections.GetWindowsKeyMappingSection();

            foreach (KeyboardCode keyCode in configSection.KeyCodes)
            {
                if (string.Equals(keyCode.codeName.ToString(), key, StringComparison.OrdinalIgnoreCase))
                    return keyCode.codeValue;
            }

            return 0;
        }

        public string getAsciiValue(char character)
        {
            AsciiMappingSection configSection = ConfigurationSections.GetAsciiMappingSection();

            foreach (AsciiCode asciiCode in configSection.AsciiCodes)
            {
                if (char.Equals(asciiCode.character, character))
                    return asciiCode.codeValue;
            }

            return "";
        }


        public abstract void AssertBrowserIsAttached();

        #endregion


        #region Browser properties

        public static ExpressionToken parentToken;
        public static Boolean parentFound = false;

        public BrowserProcess ProcessName
        {
            get { return this.processName; }
            set { this.processName = value; }
        }

        public IntPtr GetCurrentWindowHandle()
        {
            return curWindowHandle;
        }

        public void SetCurrentWindowHandle(string winTitle)
        {
            curWindowHandle = IntPtr.Zero;
            DateTime startTime = DateTime.Now;

            string regex = Regex.Escape(winTitle).Replace("\\?", ".");

            do
            {
                curWindowHandle = NativeMethods.FindWindowByRegex(new Regex(regex));
                //NativeMethods.GetWindowWithSubstring(winTitle, 0, 0, ref curWindowHandle);
            }
            while (curWindowHandle == IntPtr.Zero && (startTime.AddSeconds(10) < DateTime.Now));

            if (curWindowHandle == IntPtr.Zero)
                throw new Exception(string.Format("FATAL ERROR: Unable to aquire window handle for window title {0}.", winTitle));
        }

        #endregion


        #region User Available Methods

        public virtual void SetWindowPosition(WindowPositionTypes windowPositionType)
        {
            DateTime timeout = DateTime.Now.AddSeconds(2);

            switch (windowPositionType)
            {
                case WindowPositionTypes.MAXIMIZE:
                    NativeMethods.ShowWindow(GetCurrentWindowHandle(), NativeMethods.SW_MAXIMIZE);
                    while (!NativeMethods.IsZoomed(GetCurrentWindowHandle()) && DateTime.Now < timeout)
                    {

                    }
                    break;
                case WindowPositionTypes.MINIMIZE:
                    NativeMethods.ShowWindow(GetCurrentWindowHandle(), NativeMethods.SW_MINIMIZE);
                    while (!NativeMethods.IsIconic(GetCurrentWindowHandle()) && DateTime.Now < timeout)
                    {

                    }
                    break;
                case WindowPositionTypes.BRINGTOTOP:
                    if (!NativeMethods.IsZoomed(GetCurrentWindowHandle()))
                        NativeMethods.ShowWindow(GetCurrentWindowHandle(), NativeMethods.SW_RESTORE);
                    NativeMethods.SetWindowPos(GetCurrentWindowHandle(), HWND_TOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
                    NativeMethods.SetWindowPos(GetCurrentWindowHandle(), HWND_NOTOPMOST, 0, 0, 0, 0, 0x0003);
                    break;
                //Constant Long SW_HIDE = 0
                //Constant Long SW_NORMAL = 1
                //Constant Long SW_SHOWMINIMIZED = 2
                //Constant Long SW_SHOWMAXIMIZED = 3
                //Constant Long SW_SHOWNOACTIVATE = 4
                //Constant Long SW_SHOW = 5
                //Constant Long SW_MINIMIZE = 6
                //Constant Long SW_SHOWMINNOACTIVE = 7
                //Constant Long SW_SHOWNA = 8
                //Constant Long SW_RESTORE = 9
                //Constant Long SW_SHOWDEFAULT = 10
            }
        }

        #endregion


        #region Helper classes

        public delegate void IsMatchHandler(object value, ExpressionToken expressions, IsMatchResult isMatchResult);

        public class IsMatchResult
        {
            private bool _continueChecking;
            private object _value;

            public bool ContinueChecking
            {
                get { return _continueChecking; }
                set { _continueChecking = value; }
            }

            public object ReturnValue
            {
                get { return _value; }
                set { _value = value; }
            }

        }

        public enum MatchType
        {
            Contains = 1,
            Equals = 2
        }

        public static class AttributeNormalizer
        {

            public static string Normalize(string attribute)
            {
                NormalizationSection configSection = ConfigurationSections.GetNormalization();
                foreach (NormalizationAttribute normalizationAttribute in configSection.NormalizationAttributes)
                {
                    if (string.Equals(normalizationAttribute.Attribute.ToString(), attribute, StringComparison.OrdinalIgnoreCase))
                    {
                        attribute = normalizationAttribute.NormalizedAttribute.ToString();
                    }
                }
                return attribute;
            }
        }

        public class IdentifierExpression : System.Collections.CollectionBase
        {
            public string _identifierExpression;
            public IsMatchHandler _isMatchMethod;

            public IdentifierExpression(string identifierExpression, IsMatchHandler isMatchMethod)
            {
                _identifierExpression = identifierExpression;
                createExpression(_identifierExpression);
                _isMatchMethod = isMatchMethod;
            }

            private void createExpression(string identifierExpression)
            {
                string newExpression = string.Empty; // used since string expression is an iterator 
                int flag = 0;  // flag to skip expressions that have an @ symbol in them    	
                if (identifierExpression.Contains("\\;"))
                {
                    identifierExpression = identifierExpression.Replace("\\;", "\\@");
                    flag = 1;
                }

                string[] expressions = identifierExpression.Split(';');
                string parentElementExpression = "";

                foreach (string expression in expressions)
                {
                    if (!expression.Contains("parentElement"))
                    {
                        if (expression.Contains("\\@") && flag == 1)
                        {
                            newExpression = expression.Replace("\\@", ";");
                            this.InnerList.Add(new ExpressionToken(newExpression));
                        }
                        else
                            this.InnerList.Add(new ExpressionToken(expression));
                    }
                    else
                        parentElementExpression = expression;
                }

                if (!parentElementExpression.Equals(""))
                    this.InnerList.Add(new ExpressionToken(parentElementExpression));
            }

            public object IsMatch(object value, BrowserType browserType)
            {
                IsMatchResult result = new IsMatchResult();
                bool inStyle = false;
                foreach (ExpressionToken token in this.InnerList)
                {
                    if (token.Attribute == "style")
                        inStyle = true;
                    token.IsPartOfStyle = inStyle;
                    _isMatchMethod(value, token, result);
                    if (!result.ContinueChecking)
                        break;
                }

                return result.ReturnValue;
            }

        }

        public class ExpressionToken
        {
            private string _token;
            private string _attribute;
            private string _value;
            private bool _isPartOfStyle = false;
            private int _matchCount = int.MinValue; //int minVal means any number of matches
            protected System.Text.RegularExpressions.Regex _regEx;
            MatchType _matchType = MatchType.Contains;

            public ExpressionToken(string token)
            {

                int firstColonLocation = token.IndexOf(':');
                int firstEqualsLocation = token.IndexOf('=');
                _matchType = getMatchType(firstColonLocation, firstEqualsLocation);

                Token = token;

                if (_matchType == Browser.MatchType.Contains)
                {
                    Attribute = Token.Substring(0, firstColonLocation);
                    Value = Token.Substring(firstColonLocation + 1, (Token.Length - firstColonLocation) - 1);
                }
                else
                {
                    Attribute = Token.Substring(0, firstEqualsLocation);
                    Value = Token.Substring(firstEqualsLocation + 1, (Token.Length - firstEqualsLocation) - 1);
                }

                if (Attribute.Contains("#"))
                {
                    int locationOfPound = Attribute.IndexOf('#');
                    _matchCount = int.Parse(Attribute.Substring(locationOfPound + 1, Attribute.Length - (locationOfPound + 1)));
                    Attribute = Attribute.Substring(0, locationOfPound);
                }
            }

            private MatchType getMatchType(int firstColonLocation, int firstEqualsLocation)
            {
                if (firstColonLocation > -1)
                {
                    if (firstEqualsLocation > -1)
                    {
                        //compare them
                        if (firstColonLocation < firstEqualsLocation)
                            return Browser.MatchType.Contains;
                        else
                            return Browser.MatchType.Equals;
                    }
                    else
                    {
                        return Browser.MatchType.Contains;
                    }
                }
                else
                {
                    if (firstEqualsLocation > -1)
                        return Browser.MatchType.Equals;
                    else
                        throw new ArgumentException("No identifier match type specified.");

                }
            }

            public bool IsPartOfStyle
            {
                get { return _isPartOfStyle; }
                set { _isPartOfStyle = value; }
            }

            public string Token
            {
                get { return _token; }
                private set { _token = value; }
            }

            public int ExpectedMatchCount
            {
                get { return _matchCount; }
            }

            public MatchType MatchType
            {
                get { return _matchType; }
            }

            public string Attribute
            {
                get
                {
                    //normalize attribute on get so that parentElement attributes get normalized
                    //because we remove the parentElement text eventually
                    return _attribute;
                }
                set { _attribute = AttributeNormalizer.Normalize(value); }
            }

            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public bool IsMatch(string value)
            {
                if (value == null)
                    return false;

                bool isMatch = false;
                if (_regEx == null || _isPartOfStyle)
                {
                    _regEx = new System.Text.RegularExpressions.Regex(Value, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.ECMAScript | System.Text.RegularExpressions.RegexOptions.Compiled);
                }

                if (MatchType == Browser.MatchType.Contains)
                {
                    if (ExpectedMatchCount != int.MinValue)
                        isMatch = (_regEx.Matches(value).Count == ExpectedMatchCount);
                    else
                        isMatch = _regEx.IsMatch(value);
                }
                else
                    isMatch = _regEx.IsMatch(value) && (_regEx.Match(value).Length == value.Length);


                return isMatch;
            }
        }

        #endregion
    }
}
