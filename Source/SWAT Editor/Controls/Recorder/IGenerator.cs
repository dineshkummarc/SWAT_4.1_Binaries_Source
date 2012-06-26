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
using System.Text;
using SWAT;

namespace SWAT_Editor.Recorder
{

    public enum BrowserType
    {
        InternetExplorer = 1,
        Firefox = 2
    }

    public enum AssertionType
    {
        ElementExists = 1,
        ElementDoesNotExist = 2
    }

    public delegate void OnAfterGenerateCommandEventHandler(string newCommand);


    public interface IGenerator
    {
        //void ClickElement(HtmlElement element);
        void AssertElement(AssertionType assertionType, String assertion, HtmlElement element);
        void StimulateElement(HtmlElement element, string eventName);
        void AttachBrowser(string browserName, int index);
        //void SubmitElement(HtmlElement element);
        void SetElementProperty(HtmlElement element, string propertyName);
        void NavigateBrowser(string URL);         
        //event OnAfterGenerateCommandEventHandler OnAfterWriteCommand;
        //event EventHandler OnFinishedWritingBatchOfCommands;
        void Initialize(BrowserType browserType, bool openBrowser);
        bool Initialized { get;}
        string GeneratedCode { get;}

        void ClickJSDialog(JScriptDialogButtonType buttonType);
    }

}
