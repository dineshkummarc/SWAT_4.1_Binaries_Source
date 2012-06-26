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
namespace SWAT
{   

    public interface IVariableRetriever
    {
        void Save(string key, string value);
        string Recall(string key);
    }

    public abstract class CodeRunner
    {
        protected IVariableRetriever variables; 

        public CodeRunner(IVariableRetriever v)
        {
            variables = v;
        }

        public void SetVariables(IVariableRetriever v)
        {
            variables = v;
        }

        public void RunScript(string language, string theScript, string expectedResult, WebBrowser browser, string assems)
        {            
            string result = RunCode(theScript, browser, assems);
            if (result != expectedResult) throw new IndexOutOfRangeException("Mismatching results. Expected: " + expectedResult + " , - Actual: " + result);
        }

        public string RunScriptSaveResult(string language, string theScript, WebBrowser browser, string assems)
        {           
            string result = RunCode(theScript, browser, assems);

            if (String.IsNullOrEmpty(result))
                throw new ArgumentException("Code yielded no results.");
            return result;            
        }

        protected abstract string RunCode(string source, WebBrowser browser, string assems);
    }
}
