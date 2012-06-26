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
using System.Configuration;
using SWAT.Configuration.Normalization;
using SWAT.Configuration;

namespace SWAT
{
    #region Custom Attributes

    public class NonUICommand : Attribute { }

    #endregion

    #region  Enums

    public enum ApplicationType
    {
        Excel = 0
    }

    public enum BrowserProcess
    {
        //process names for different browsers
        iexplore = 1,
        firefox = 2,
        chrome = 3,
        safari = 4
    }

    public enum WindowPositionTypes
    {
      MAXIMIZE = 0,
      MINIMIZE = 1,
      BRINGTOTOP = 2
    }

    
    public enum IdentifierType
    {
        Name = 0,
        Id = 1,
        InnerHtml = 2,
        InnerHtmlContains = 3,
        Expression = 4

        //Removed to fix unsupported HTML element attributes in Intellisense
        //OuterHtml = 5,
        //OuterHtmlContains = 6
    }

    public enum JScriptDialogButtonType
    {
        Ok = 0,
        Cancel = 1
    }

    public enum AttributeType
    {
        BuiltIn = 0,
        Custom = 1
    }

    public enum KeyCode
    {
        BACKSPACE = 0,
        TAB = 1,    
        ESC = 2,
        ENTER =3,
        SHIFT = 4,
        CONTROL = 5,
        PAUSE = 6,
        SPACEBAR = 7,
        INSERT = 8,
        PAGE_UP = 9,
        PAGE_DOWN = 10,
        END = 11,
        HOME = 12,
        LEFT_ARROW = 13,
        RIGHT_ARROW = 14,
        UP_ARROW = 15,
        DOWN_ARROW = 16,
        DELETE = 17,
        PRNTSCRN = 18
    }

    public enum BrowserName
    {
        InternetExplorer = 0,
        FireFox = 1
    }

    #endregion

    public static class TagName
    {
        public const string ALL_TAGS = "*"; //* represents all elements when calling getElementsByTagName in Mozilla.
        public const string previous = "previous";
        public const string next = "next";
        public const string contents = "contents";
        public const string attributes = "attributes";
        public const string index = "index";
        public const string A = "a";
        public const string ABBR = "abbr";
        public const string ACRONYM = "acronym";
        public const string ADDRESS = "address";
        public const string APPLET = "applet";
        public const string AREA = "area";
        public const string B = "b";
        public const string BASE = "base";
        public const string BASEFONT = "basefont";
        public const string BDO = "bdo";
        public const string BIG = "big";
        public const string BLOCKQUOTE = "blockquote";
        public const string BODY = "body";
        public const string BR = "br";
        public const string BUTTON = "button";
        public const string CAPTION = "caption";
        public const string CENTER = "center";
        public const string CITE = "cite";
        public const string CODE = "code";
        public const string COL = "col";
        public const string COLGROUP = "colgroup";
        public const string DD = "dd";
        public const string DEL = "del";
        public const string DFN = "dfn";
        public const string DIR = "dir";
        public const string DIV = "div";
        public const string DL = "dl";
        public const string DT = "dt";
        public const string EM = "em";
        public const string FIELDSET = "fieldset";
        public const string FONT = "font";
        public const string FORM = "form";
        public const string FRAME = "frame";
        public const string FRAMESET = "frameset";
        public const string H1 = "h1";
        public const string H2 = "h2";
        public const string H3 = "h3";
        public const string H4 = "h4";
        public const string H5 = "h5";
        public const string H6 = "h6";
        public const string HEAD = "head";
        public const string HR = "hr";
        public const string HTML = "html";
        public const string I = "i";
        public const string IFRAME = "iframe";
        public const string IMG = "img";
        public const string INPUT = "input";
        public const string INS = "ins";
        public const string ISINDEX = "isindex";
        public const string KBD = "kbd";
        public const string LABEL = "label";
        public const string LEGEND = "legend";
        public const string LI = "li";
        public const string LINK = "link";
        public const string MAP = "map";
        public const string MENU = "menu";
        public const string META = "meta";
        public const string NOFRAMES = "noframes";
        public const string NOSCRIPT = "noscript";
        public const string OBJECT = "object";
        public const string OL = "ol";
        public const string OPTGROUP = "optgroup";
        public const string OPTION = "option";
        public const string P = "p";
        public const string PARAM = "param";
        public const string PRE = "pre";
        public const string Q = "q";
        public const string S = "s";
        public const string SAMP = "samp";
        public const string SCRIPT = "script";
        public const string SELECT = "select";
        public const string SMALL = "small";
        public const string SPAN = "span";
        public const string STRIKE = "strike";
        public const string STRONG = "strong";
        public const string STYLE = "style";
        public const string SUB = "sub";
        public const string SUP = "sup";
        public const string TABLE = "table";
        public const string TBODY = "tbody";
        public const string TD = "td";
        public const string TEXTAREA = "textarea";
        public const string TFOOT = "tfoot";
        public const string TH = "th";
        public const string THEAD = "thead";
        public const string TITLE = "title";
        public const string TR = "tr";
        public const string TT = "tt";
        public const string U = "u";
        public const string UL = "ul";
        public const string VAR = "var";
    }

    public static class Attributes
    {
        public const string VALUE = "value";
        public const string INNER_HTML = "innerHTML";
        public const string ID = "id";
        public const string NAME = "name";
        public const string TYPE = "type";
        public const string HREF = "href";
        public const string STYLE = "style";
        public const string ONCLICK = "onclick";
        public const string ACTION = "action";
        public const string ALT = "alt";
        public const string ALIGN = "align";
        public const string CLASS = "class";
        public const string CHECKED = "checked";
        public const string DISABLE = "disable";
        public const string FRAME = "frame";
        public const string LABEL = "label";
        public const string METHOD = "method";
        public const string SELECTEDINDEX = "selectedIndex";
        public const string TITLE = "title";
        public const string SPAN = "span";
        public const string SRC = "src";
        public const string DIR = "dir";
        public const string LANG = "lang";
        public const string LANGUAGE = "language";
        public const string ONABORT = "onabort";
        public const string ONBLUR = "onblur";
        public const string ONCHANGE = "onchange";
        public const string ONDBCLICK = "ondbclick";
        public const string ONERROR = "onerror";
        public const string ONFOCUS = "onfocus";
        public const string ONKEYDOWN = "onkeydown";
        public const string ONKEYPRESS = "onkeypress";
        public const string ONKEYUP = "onkeyup";
        public const string ONLOAD = "onload";
        public const string ONMOUSEDOWN = "onmousedown";
        public const string ONMOUSEMOVE = "onmousemove";
        public const string ONMOUSEOUT = "onmouseout";
        public const string ONMOUSEOVER = "onmouseover";
        public const string ONMOUSEUP = "onmouseup";
        public const string ONRESET = "onreset";
        public const string ONRESIZE = "onresize";
        public const string ONSELECT = "onselect";
        public const string ONSUBMIT = "onsubmit";
        public const string ONUNLOAD = "onunload";

    }

    public static class Events
    {
        private static string[] _names = {"onabort", "onblur", "onchange", "onclick", "ondblclick", "onerror", "onfocus", 
                                      "onkeydown","onkeypress","onkeyup","onload","onmousedown","onmousemove","onmouseout",
                                      "onmouseover","onmouseup","onreset","onresize","onselect","onsubmit","onunload"};

        public static string[] Names
        {
            get { return _names; }
        }
    }

    public static class IdentifierTypeWithoutExpression
    {
        private static string[] _identifiers = {"checked", "contentEditable", "disabled", "id", "indeterminate",
                                                "innerHtml", "name", "readOnly", "size", "start", "type", "href",
                                                "value", "style", "onclick", "class", "parentElement", "title", "alt"};

        public static string[] Identifiers
        {
            get { return _identifiers; }
        }
    }

    public static class ConfigurationSections
    {
        public static NormalizationSection GetNormalization()
        {
            return (NormalizationSection)System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(FilePath, ConfigurationUserLevel.None).GetSection("NormalizationSection");
        }

        private static ExeConfigurationFileMap FilePath
        {
            get
            {

                string path = string.Concat(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", ""), ".config");
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = path;

                return map;
            }
        }

        public static KeyMappingSection GetWindowsKeyMappingSection()
        {
            return GetKeyMappingSection("WindowsKeyMappingSection");
        }

        public static KeyMappingSection GetMacKeyMappingSection()
        {
            return GetKeyMappingSection("MacKeyMappingSection");
        }

        private static KeyMappingSection GetKeyMappingSection(string section)
        {
            return (KeyMappingSection)System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(FilePath, ConfigurationUserLevel.None).GetSection(section);
        }


        public static AsciiMappingSection GetAsciiMappingSection()
        {
            return (AsciiMappingSection)System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(FilePath, ConfigurationUserLevel.None).GetSection("AsciiMappingSection");
        }
    }    

    public interface IBrowser
    {
        #region Non user accesible methods

        uint getKeyValue(string key);
        string getAsciiValue(char character);
        void SetCurrentWindowHandle(string windowTitle);
        IntPtr GetCurrentWindowHandle();
        void WaitForBrowserReadyState();

        #endregion


        #region Browser Related  methods

        void OpenBrowser();
        
        void CloseBrowser();
        void RefreshBrowser();
        void KillAllOpenBrowsers();
        void KillAllOpenBrowsers(string windowTitle); 
        
        void NavigateBrowser(string url);
        string GetCurrentLocation();
        string GetCurrentDocumentTitle();
        string GetCurrentWindowTitle();

        void AssertBrowserDoesNotExist(string windowTitle);
        void AssertBrowserDoesNotExist(string windowTitle, double timeOut);

        void AssertBrowserExists(string windowTitle);
        void AssertBrowserExists(string windowTitle, double timeOut);

        void AttachBrowserToWindow(string windowTitle);
        void AttachBrowserToWindow(string windowTitle, int windowIndex);
        void AssertBrowserIsAttached();
        void AssertTopWindow(string browserTitle, int index, int timeout);

        void SetWindowPosition(WindowPositionTypes windowPositionType);
        
        void Sleep(int milliseconds);
                       
        #endregion


        #region Script methods

        void RunScript(String language, String theScript, String expectedResult);
        String RunScriptSaveResult(String language, String theScript);

        #endregion


        #region Dialog methods
        
        void ClickJSDialog(JScriptDialogButtonType buttonType);

        void AssertJSDialogContent(string dialogContent);
        void AssertJSDialogContent(string dialogContent, int timeout);

        #endregion


        #region PressKeys methods

        void PressKeys(IdentifierType identType, string identifier, string word, string tagName);

        #endregion


        #region Elements Methods        
        
        void ElementFireEvent(IdentifierType identType, string identifier, string tagName, string eventName);
        void AssertElementIsActive(IdentifierType identType, string identifier, string tagName, int timeoutSeconds);
        
        #region Setters

        void SetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, string attributeValue);
        void SetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, string attributeValue, int timeOut);
        
        #endregion

        #region Getters

        string GetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName);        
        string GetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, int timeOut);

        #endregion

        #endregion        

        #region Screenshots

        string TakeScreenshot(string filePrefix);
        //IntPtr GetContentHandle();
        //object GetDocumentAttribute(string theAttributeName);
        //void SetDocumentAttribute(string theAttributeName, object theAttributeValue);

        #endregion
    }

    public interface IKeyboard
    {
        bool SendInputString(string windowTitle);
        void ProcessKey(uint keyToPressCode);
        void ProcessAltKeyCombination(string keyValue);
        void ProcessInternationalKey(string asciiCode);
    }

    public interface IBrowserCommands
    {
        string GetElementAttribute(IdentifierType identType, string identifier, AttributeType attributeType, string attributeName, string tagName);
        string GetElementAttribute(IdentifierType identType, string identifier, AttributeType attributeType, string attributeName);
        void OpenBrowser();
        void CloseBrowser();
        void RefreshBrowser();
        void NavigateBrowser(string url);
        void NavigateBrowser(string url, int timeout);
        void ClickJSDialog(JScriptDialogButtonType button);
        void Sleep(int milliseconds);
        void AttachToWindow(string windowTitle);
        void AttachToWindow(string windowTitle, int index);
        void AttachToNonBrowserWindow(ApplicationType appType, string title, int timeout);
        void AttachToNonBrowserWindow(ApplicationType appType, string title, int index, int timeout);
        void CloseNonBrowserWindow();
        void RunScript(string theScript, string expectedResult);
        void RunScript(string language, string theScript, string expectedResult);
        void RunScript(string language, string theScript, string expectedResult, string assems);
        string RunScriptSaveResult(string theScript);
        string RunScriptSaveResult(string language, string theScript);
        string RunScriptSaveResult(string language, string theScript, string assems);
        string GetLocation();
        string GetWindowTitle();
        void AssertElementExists(IdentifierType identType, string identifier, string tagName);
        void AssertElementExistsWithTimeout(IdentifierType identType, string identifier, double timeOutLengthMilliseconds);
        void AssertElementExistsWithTimeout(IdentifierType identType, string identifier, double timeOutLengthMilliseconds, string tagName);
        void AssertElementDoesNotExist(IdentifierType identType, string identifier);
        void AssertElementDoesNotExist(IdentifierType identType, string identifier, string tagName);
        void AssertBrowserType(string browserName);
        void AssertJSDialogContent(string dialogContent);
        void AssertBrowserDoesNotExist(string windowTitle);
        void AssertBrowserDoesNotExist(string windowTitle, int timeOut);
        void AssertBrowserExists(string windowTitle);
        void AssertBrowserExists(string windowTitle, int timeOut);
        void AssertElementIsActive(IdentifierType identType, string identifier, string tagName);
        void AssertElementIsActive(IdentifierType identType, string identifier, string tagName, int timeoutSeconds);
        string TakeScreenshot(string filePrefix);
    }

    public interface IDatabaseCommands
    {
        void ConnectToMssql(string serverName, string userName, string password);
        void ConnectToMssql(string serverName, string userName, string password, int connectionTimeout);
        void ConnectToOracle(string serverName, string userName, string password);
        void SetDatabase(string newDb);
        void Disconnect();
        void SetQuery(string SQL);
        void SetQuery(string SQL, int timeout);
        void AssertRecordCount(int expectedNumber);
        string GetDbDate();
        string GetDbDate(int format);
        void SaveDbDate();
        void SaveDbDate(int format);
        string GetSavedDbDate(string part);
        string GetSavedDbDateMonth();
        string GetSavedDbDateDay();
        string GetSavedDbDateYear();
        string GetDbRecord(int row, int col);
        string GetDbRecord();
        string GetDbRecordByColumnName(string name);
        string GetDbRecordByColumnName(int row, string name);
        void AssertRecordValues(string values);
        void AssertRecordValues(int row, int col, string values);
        void AssertRecordValuesByColumnName(string colName, string values);
        void AssertRecordValuesByColumnName(int row, string colName, string values);
        void AssertDBRecordExistsWithTimeout(String sql, int timeout);
        void UpdateTable(string SQL);
        void InsertIntoTable(string SQL);
        void DeleteFromTable(string SQL);
        void BackupTable(string tablename);
        void BackupTable(string tablename, string filter);
        void RestoreTable(string tableName);
        void RestoreTable(string tableName, string filter);
        void RestoreAllTables();
    }

    
}

