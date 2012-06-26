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
using NUnit.Framework;

namespace SWAT.Tests.RunScript
{
    public abstract class RunScriptTests : BrowserTestFixture
    {
        public RunScriptTests(BrowserType browserType)
        : base(browserType)
        {

        }

        [Test]
        public void RunScriptTest()
        {
            _browser.RunScript("document.getElementById('chkOne').checked = true;", "true");
            _browser.RunScript("window.frames.length;", "2");
            _browser.RunScript("new Function(\"var elem = document.getElementById('txtOne'); if(elem){elem.value='RunScript test';} return true;\")();", "true");
        }

         
        [TestCase("new Function(\"return 'hello world';\")();", "hello world")]
        [TestCase("(5==5);", "true")]
        public void RunScriptSaveResultTest( string input, string expectedResult )
        {
            try
            {
                _browser.NavigateBrowser("www.google.com");
                String result = _browser.RunScriptSaveResult(input);
                Assert.AreEqual(expectedResult, result.ToLower());
            }
            finally
            {
                _browser.NavigateBrowser(getTestPage("TestPage.htm"));
            }
        }

         
        [TestCase("!@#$%^&*()_+")]
        [TestCase("")]
        [TestCase("windw.dcument;")]
        [TestCase("SELECT * FROM sys.tables;")]
        public void RunScriptSaveResultFailsTest( string invalidScript )
        {
            bool threwException = false;
            try
            {
                _browser.RunScriptSaveResult(invalidScript);
            }
            catch (ArgumentException e)
            {
                threwException = true;
                Assert.IsTrue(e.Message.Contains("Javascript yielded no results."));
            }
            Assert.IsTrue(threwException, "RunScriptSaveResult threw no exception with incorrect javascript");
        }

         
        [TestCase("", "this should fail")]
        [ExpectedException(typeof(AssertionFailedException))]
        public void RunScriptFailsTest(string invalidScript, string expectedResult )
        {
            _browser.RunScript(invalidScript, expectedResult);          
        }

        [Test]
        public void RunScriptCSharpTest()
        {
            _browser.RunScript("CSHARP", "using SWAT; class test{public static string Main(){return 5==5?\"true\":\"false\";}}", "true");
            _browser.RunScript("csharp", "namespace SWAT{class test{public static string Main(){return browser.RunScriptSaveResult(\"(5==5);\");}}}", "true", "");
            _browser.RunScript("CSHARP", "using System.Windows.Forms; namespace SWAT{class test{public static string Main(){return 5==5?\"true\":\"false\";}}}", "true", "c:\\WINDOWS\\Microsoft.NET\\Framework\\v2.0.50727\\System.Windows.Forms.dll");
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void RunScriptCSharpFailsOnMismatchTest()
        {
            _browser.RunScript("CSHARP", "using System.Windows.Forms; namespace SWAT{class test{public static string Main(){return 5==5?\"true\":\"false\";}}}", "false", "c:\\WINDOWS\\Microsoft.NET\\Framework\\v2.0.50727\\System.Windows.Forms.dll");
        }

        [Test]
        public void RunScriptCSharpWorksWithoutOpenBrowserTest()
        {
            WebBrowser browser = new WebBrowser(_browserType);
            browser.RunScript("CSHARP", "using SWAT; class test{public static string Main(){return 5==5?\"true\":\"false\";}}", "true");           
        }

        [Test]
        [ExpectedException(typeof(LanguageNotImplementedException), UserMessage = "JAVA is not implemented.")]
        public void RunScriptWithInvalidLanguageTest()
        {
            WebBrowser browser = new WebBrowser(_browserType);
            browser.RunScript("JAVA", "using SWAT; class test{public static string Main(){return 5==5?\"true\":\"false\";}}", "true");
        }

        [Test]
        public void RunScriptCSharpVariableSaveAndRecallTest()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("key1", "value");
            WebBrowser browser = new WebBrowser(_browserType, new SWAT_Editor.Controls.EditorVariableRetriever(variables));            
            browser.RunScript("CSHARP", "namespace SWAT{class test{public static string Main(){swatVars.Save(\"key2\", \"yo son\"); return swatVars.Recall(\"key1\");}}}", "value", "");
            browser.RunScript("CSHARP", "namespace SWAT{class test{public static string Main(){return swatVars.Recall(\"key2\");}}}", "yo son", "");
        }

        [Test]
        public void RunScriptCSharpDefinesTest()
        {
            _browser.RunScript("csharp", "/* This should not mess up /* #define assdf * #define asdf */ #define blah #define blah2 #define blah3 using SWAT; class Test { public static string Main() { string retstring = \"\"; #if blah #if (blah2 && (blah3)) retstring +=\"Hello\"; #endif #endif #if blah4 retstring = \"broke\"; #elif !blah4 retstring += \" Wonderful\"; #endif #if (!blah3 || blah4) return \"broke\"; #endif return retstring;} } ", "Hello Wonderful");
            _browser.RunScript("csharp", "#define blah #define blah2 #define blah3 namespace SWAT{ class Test { public static string Main() { string retstring = \"\"; #if blah #if (blah2 && blah3) retstring +=\"Hello\"; #endif #endif #if blah4 retstring = \"broke\"; #elif !blah4 retstring += \" Wonderful\"; #endif #if (!blah3 || blah4) return \"broke\"; #endif return retstring;} }}", "Hello Wonderful");

            bool fail = false;
            try
            {
                _browser.RunScript("csharp", "#defineblah #define blah2 namespace SWAT{ class Test { public static string Main() { return \"Hello Wonderful\"; } } }", "Hello Wonderful");
            }
            catch (RunScriptCompilerException)
            {
                fail = true;
            }

            Assert.IsTrue(fail, "Broken define not handled");

            fail = false;
            try
            {
                _browser.RunScript("csharp", "#define blah #define blah2 #define blah3 namespace SWAT{ class Test { public static string Main() { string retstring = \"\"; #if blah #if (blah2 && blah3) retstring +=\"Hello\"; #endif #endif #if blah4 retstring = \"broke\"; #elif !blah4 retstring += \" Wonderful\"; #endif #if (!blah3 || blah4) return \"broke\"; #endifreturnretstring;} }}", "Hello Wonderful");
            }
            catch (RunScriptCompilerException)
            {
                fail = true;
            }

            Assert.IsTrue(fail, "Broken preprocessor not handled");
        }

        [Test]
        public void RunScriptSaveResultCSharpTest()
        {
            string result = _browser.RunScriptSaveResult("CSHARP", "using SWAT; class test{public static string Main(){return 5==5?\"true\":\"false\";}}");
            Assert.AreEqual("true", result);

            result = _browser.RunScriptSaveResult("csharp", "namespace SWAT{class test{public static string Main(){return browser.RunScriptSaveResult(\"(5==5);\");}}}", "");
            Assert.AreEqual("true", result);

            result = _browser.RunScriptSaveResult("CSHARP", "using System.Windows.Forms; namespace SWAT{class test{public static string Main(){return 5==5?\"true\":\"false\";}}}", "c:\\WINDOWS\\Microsoft.NET\\Framework\\v2.0.50727\\System.Windows.Forms.dll");
            Assert.AreEqual("true", result);
        }

        [Test]
        public void RunScriptSaveResultCSharpVariableSaveAndRecallTest()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("key1", "value");
            WebBrowser browser = new WebBrowser(_browserType, new SWAT_Editor.Controls.EditorVariableRetriever(variables));            

            string result = browser.RunScriptSaveResult("CSHARP", "using SWAT; class test{public static string Main(){swatVars.Save(\"url\", \"www.google.com\"); return \"hello wonderful\";}}", "");
            Assert.AreEqual("hello wonderful", result);

            result = browser.RunScriptSaveResult("CSHARP", "namespace SWAT{class test{public static string Main(){return swatVars.Recall(\"url\");}}}", "");
            Assert.AreEqual("www.google.com", result);
        }

        [Test]
        [ExpectedException(typeof(RunScriptRuntimeException))]
        public void RunScriptRunTimeErrorTest()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("key1", "value");
            WebBrowser browser = new WebBrowser(_browserType, new SWAT_Editor.Controls.EditorVariableRetriever(variables));

            string result = browser.RunScriptSaveResult("CSHARP", "using SWAT; class test{public static string Main(){string s = \"hello\"; int i =int.Parse(s);return s;}}", "");
        }

        [Test]
        [ExpectedException(typeof(RunScriptCompilerException), UserMessage = "Main method not present")]
        public void RunScriptCompilerErrorMissingMainTest()
        {            
            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("key1", "value");
            WebBrowser browser = new WebBrowser(_browserType, new SWAT_Editor.Controls.EditorVariableRetriever(variables));

            string result = browser.RunScriptSaveResult("CSHARP", "using SWAT; class test{public static string Foo(){return \"s\";}}", "");
        }

        [Test]
        [ExpectedException(typeof(RunScriptCompilerException))]
        public void RunScriptCompilerErrorTest()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("key1", "value");
            WebBrowser browser = new WebBrowser(_browserType, new SWAT_Editor.Controls.EditorVariableRetriever(variables));

            string result = browser.RunScriptSaveResult("CSHARP", "using SWAT; class test{dkfjdkfj}}", "");
        }

        [Test]
        public void RunScriptLoadFromFileTest()
        {
            string path = getTestFilePath("testC#script.cs");
            string fileArg = string.Format("/file:{0}", path);
            string result;
                
            result = _browser.RunScriptSaveResult("CSHARP", fileArg);
            Assert.AreEqual("true", result);

            result = _browser.RunScriptSaveResult("CSHARP", fileArg, "");
            Assert.AreEqual("true", result);

            _browser.RunScript("csharp", fileArg, "true");
            _browser.RunScript("csharp", fileArg, "true", "");

            path = getTestFilePath("testJavaScript.js");
            fileArg = string.Format("/file:{0}", path);

            result = _browser.RunScriptSaveResult("javascript", fileArg);
            Assert.AreEqual("hello world", result);

            _browser.RunScript("javascript", fileArg, "hello world");

            result = _browser.RunScriptSaveResult(fileArg);
            Assert.AreEqual("hello world", result);

            _browser.RunScript(fileArg, "hello world");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RunAppleScriptOnWindowsTest()
        {
            if (_browserType == BrowserType.Safari)
                Assert.Ignore("Test meant for Windows only");

            string appleScript = "tell application \"Safari\"\ropen location \"www.google.com\"\rtell application \"System Events\" to tell process \"Safari\" to set value of attribute \"AXMain\" of window 1 to true\rdelay 3\rreturn name of document 1\rend tell";
            _browser.RunScript("applescript", appleScript, "Google");
        }
        
        [Test]
        public void RunScriptJavascriptReturnsSyntaxErrorMessageTest()
        {
            string script = "there is no way that this will compile";
            bool exceptionThrown = false;
            try
            {
                _browser.RunScript("javascript", script, "some fake expected");
            }
            catch (AssertionFailedException e)
            {
                exceptionThrown = true;
                string msg = e.Message.ToLower();
                string assertFailMsg = "Actual message : " + e.Message;
                switch (_browserType)
                {
                case BrowserType.FireFox:
                case BrowserType.InternetExplorer : Assert.IsTrue(msg.Contains("syntax"), assertFailMsg); break;
                case BrowserType.Chrome : Assert.IsTrue(msg.Contains("unexpected identifier"), assertFailMsg); break;
                case BrowserType.Safari : Assert.IsTrue(msg.Contains("parse error"), assertFailMsg); break;
                default : Assert.Fail(e.Message); break;
                }
            }
            Assert.IsTrue(exceptionThrown, "RunScript did not throw an exception with an invalid script");
        }

        [Test]
        public void RunScriptJavascriptReturnedUndefinedErrorMessageTest()
        {
            string script = "a += 5;";
            bool exceptionThrown = false;
            try
            {
                _browser.RunScript("javascript", script, "some other fake expected");
            }
            catch (AssertionFailedException e)
            {
                exceptionThrown = true;
                string msg = e.Message.ToLower();
                string assertFailMsg = "Actual message : " + e.Message;
                switch (_browserType)
                {
                case BrowserType.FireFox :
                case BrowserType.InternetExplorer : 
                case BrowserType.Chrome : Assert.IsTrue(msg.Contains("is not defined") || msg.Contains("undefined"), assertFailMsg); break;
                case BrowserType.Safari : Assert.IsTrue(msg.Contains("can't find variable"), assertFailMsg); break;
                default : Assert.Fail(e.Message); break;
                }
            }
            Assert.IsTrue(exceptionThrown, "RunScript did not throw an exception with an invalid script");
        }
    }
}
