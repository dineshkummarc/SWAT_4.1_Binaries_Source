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
using System.Text;
using NUnit.Framework;
using SWAT.Fitnesse;
using System.Diagnostics;
using System.Threading;
using SWAT.Reflection;

using fit;

namespace SWAT.Tests.Fitnesse
{
    [TestFixture]
    public class FitTests : FitnesseTestFixture
    {
        [SetUp]
        public override void TestSetup()
        {
            TestManager.ResetForNewTest();
            TestManager.InCompareData = false;
        }

        [Test]
        public void TestChromeSWATFixture()
        {
            ChromeSWATFixture testFixture = new ChromeSWATFixture();
        }

        [Test]
        public void TestSafariSWATFixture()
        {
            SafariSWATFixture testFixture = new SafariSWATFixture();
        }

        [Test]
        public void TestGetelementAttributeAndSetAVariable()
        {
            /*
             * This test is importatnt because it saves a value to "variable", later on in another test there will be a Fixture.Recall(variable) will will 
             * replace the variable value for a new one. 
             */
            FireFoxSWATFixture testFixture = new FireFoxSWATFixture();
            SetDebugMode(testFixture, true);
            bool passed = true;

            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>file:///C:/SWAT/trunk/SWAT.Tests/TestPages/TestPage.htm</td></tr><tr><td>GetElementAttribute</td><td>Expression</td><td>id:dd</td><td>innerHTML</td><td>variable</td><td>a</td></tr><tr><td>CloseBrowser</td></tr></table>");

            try
            {
                testFixture.DoTable(testParse);
            }
            catch (Exception)
            {
                passed = false;
            }

            Assert.IsTrue(passed, "Command failed unexpectedly.");
            SetDebugMode(testFixture, false);
        }


        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>SetVariable</td><td>variable</td><td>Testing 123</td></tr></table><table><tr><td>SWATFixture</td></tr><tr><td><>SetElementAttribute</td><td>InnerHtml</td><td>testing</td><td>value</td><td>>>variable<<</td><td>*</td></tr></table>", true)] //SetElementAttribute
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td><>GetElementAttribute</td><td>InnerHtml</td><td>testing</td><td>value</td><td>variable</td><td>*</td></tr></table>", true)] //GetElementAttribute
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>GetDbRecord</td><td>VariableName</td><td>1</td><td>1</td></tr></table>", true)]//GetDbRecord
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>GetDbDate</td><td>VariableName</td><td>1</td></tr></table> ", true)] //GetDbDate
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>GetLocation</td><td>Variable</td></tr></table>", true)] //OpenBrowser,GetLocation, CloseBrowser
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td><>OpenBrowser</td></tr><tr><td>GetWindowTitle</td><td>Variable</td></tr><tr><td>CloseBrowser</td></tr></table>", true)] //OpenBrowser, GetWindowTitle, CloseBrowser
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>RunScriptSaveResult</td><td>ScriptThatReturnsAValue</td><td>VariableName</td></tr></table>", true)] //RunScriptSaveResult
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>GetDbRecordByColumnName</td><td>VariableName</td><td>1</td><td>1</td></tr></table>", true)] //GetDbRecordByColumnName
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>BeginCompareData</td></tr></table> ", true)] //BeginCompareData
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>http://www.google.com</td></tr><tr><td colspan=\"2\">CloseBrowser</td></tr></table>", true)] //OpenBrowser, NaviagetBrowser, CloseBrowser   
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>SetVariable</td><td>variable</td><td>Testing 123</td></tr></table><table><tr><td>SWATFixture</td></tr><tr><td><>SetElementAttribute</td><td>InnerHtml</td><td>testing</td><td>value</td><td>>>variable<<</td><td>*</td></tr></table>", false)] //SetElementAttribute
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>GetElementAttribute</td><td>InnerHtml</td><td>testing</td><td>value</td><td>variable</td><td>*</td></tr></table>", false)] //GetElementAttribute
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>GetDbRecord</td><td>VariableName</td><td>1</td><td>1</td></tr></table>", false)]//GetDbRecord
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td><>GetDbDate</td><td>VariableName</td><td>1</td></tr></table> ", false)] //GetDbDate
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td><>SaveDbDate</td><td>1</td></tr></table> ", false)] //SaveDbDate
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td><>GetSavedDbDate</td><td>VariableName</td><td>1</td></tr></table> ", false)] //GetSavedDbDate
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td><>GetSavedDbDateDay</td><td>VariableName</td><td>1</td></tr></table> ", false)] //GetSavedDbDateDay
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td><>GetSavedDbDateMonth</td><td>VariableName</td><td>1</td></tr></table> ", false)] //GetSavedDbDateMonth
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td><>GetSavedDbDateYear</td><td>VariableName</td><td>1</td></tr></table> ", false)] //GetSavedDbDateYear
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>GetLocation</td><td>Variable</td></tr></table>", false)] //OpenBrowser,GetLocation, CloseBrowser
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>GetWindowTitle</td><td>Variable</td></tr><tr><td>CloseBrowser</td></tr></table>", false)] //OpenBrowser, GetWindowTitle, CloseBrowser
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>RunScriptSaveResult</td><td>ScriptThatReturnsAValue</td><td>VariableName</td></tr></table> ", false)] //RunScriptSaveResult
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>GetDbRecordByColumnName</td><td>VariableName</td><td>1</td><td>1</td></tr></table>", false)] //GetDbRecordByColumnName
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>SetVariable</td><td>VariableName</td><td>Hello World 123</td></tr></table>", false)]//SetVariable
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>BeginCompareData</td></tr></table>", false)] //BeginCompareData
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td><>OpenBrowser</td></tr><tr><td>@@NavigateBrowser</td><td>http://www.google.com</td></tr><tr><td>@@CloseBrowser</td></tr></table>", false)] //OpenBrowser, NaviagetBrowser, CloseBrowser
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>EndCompareData</td></tr></table>", true)] //EndCompareData
        [TestCase("<table><tr><td>SWATFixture</td></tr><tr><td>GetConfigurationItem</td><td>FindElementTimeout</td><td>configItemVar</td></tr></table>", false)] //GetConfigurationItem
        public void TestInternetExplorerSWATFixtureTestTable(String parser, bool value)
        {
            TestManager.InCompareData = value;
            InternetExplorerSWATFixture testFixture = new InternetExplorerSWATFixture();
            fit.Parse testParse = new fit.Parse(parser);
            testFixture.DoTable(testParse);

        }

        [Test]
        public void TestFireFoxSWATFixtureTestTable()
        {
            object[] fixtureValues = {
                "<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>file:///C:/SWAT/trunk/SWAT.Tests/TestPages/TestPage.htm</td></tr><tr><td>GetElementAttribute</td><td>Expression</td><td>id:dd</td><td>innerHTML</td><td>variable</td><td>a</td></tr><tr><td>CloseBrowser</td> </tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>SetElementAttribute</td><td>Id</td><td>txtOne</td><td>value</td><td>>>variable<<</td><td>*</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>GetElementAttribute</td><td>InnerHtml</td><td>testing</td><td>value</td><td>variable</td><td>*</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td>GetDbRecord</td><td>VariableName</td><td>1</td><td>1</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td>GetDbDate</td><td>VariableName</td><td>1</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td>GetLocation</td><td>Variable</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>OpenBrowser</td></tr><tr><td>GetWindowTitle</td><td>Variable</td></tr><tr><td>CloseBrowser</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td>RunScriptSaveResult</td><td>ScriptThatReturnsAValue</td><td>VariableName</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td>GetDbRecordByColumnName</td><td>VariableName</td><td>1</td><td>1</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td>BeginCompareData</td></tr></table> ", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>http://www.google.com</td></tr><tr><td colspan=\"2\">CloseBrowser</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td>SetVariable</td><td>variable</td><td>testing</td></tr><tr><td><>SetElementAttribute</td><td>Id</td><td>txtOne</td><td>value</td><td>>>variable<<</td><td>*</td></tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td>GetElementAttribute</td><td>InnerHtml</td><td>testing</td><td>value</td><td>variable</td><td>*</td></tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td>GetDbRecord</td><td>VariableName</td><td>1</td><td>1</td></tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>GetDbDate</td><td>VariableName</td><td>1</td></tr></table> ", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>SaveDbDate</td><td>1</td></tr></table> ", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>GetSavedDbDate</td><td>VariableName</td><td>1</td></tr></table> ", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>GetSavedDbDateDay</td><td>VariableName</td><td>1</td></tr></table> ", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>GetSavedDbDateMonth</td><td>VariableName</td><td>1</td></tr></table> ", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>GetSavedDbDateYear</td><td>VariableName</td><td>1</td></tr></table> ", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td>GetLocation</td><td>Variable</td></tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>GetWindowTitle</td><td>Variable</td></tr><tr><td>CloseBrowser</td></tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td>RunScriptSaveResult</td><td>ScriptThatReturnsAValue</td><td>VariableName</td></tr></table> ", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td>GetDbRecordByColumnName</td><td>VariableName</td><td>1</td><td>1</td></tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td>SetVariable</td><td>VariableName</td><td>Hello World 123</td></tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td>&lt;&gt;BeginCompareData</td></tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td><>OpenBrowser</td></tr><tr><td>@@NavigateBrowser</td><td>http://www.google.com</td></tr><tr><td>@@CloseBrowser</td></tr></table>", false,
                "<table><tr><td>SWATFixture</td></tr><tr><td>EndCompareData</td></tr></table>", true,
                "<table><tr><td>SWATFixture</td></tr><tr><td>GetConfigurationItem</td><td>FindElementTimeout</td><td>configItemVar</td></tr></table>", false
            };
            FireFoxSWATFixture testFixture = new FireFoxSWATFixture();
            for (int i = 0; i < fixtureValues.Length; i += 2)
            {
                TestManager.InCompareData = (bool) fixtureValues[i + 1];
                fit.Parse testParse = new fit.Parse((string)fixtureValues[i]);
                testFixture.DoTable(testParse);
                TestManager.ResetForNewTest();
            }
        }

        [Test]
        public void TestFitnesseWithInverserModifier()
        {
            FireFoxSWATFixture testFixture = new FireFoxSWATFixture();
            SetDebugMode(testFixture, true);
            bool passed = true;

            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr> <tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>file:///C:/SWAT/trunk/SWAT.Tests/TestPages/TestPage.htm</td></tr><tr><td>&lt;&gt;GetElementAttribute</td><td>Expression</td><td>id:IDontExist</td><td>variable</td><td>a</td></tr><tr><td>CloseBrowser</td></tr></table>");

            try
            {
                testFixture.DoTable(testParse);
            }
            catch (Exception)
            {
                passed = false;
            }

            Assert.IsTrue(passed, "Command failed unexpectedly.");
            SetDebugMode(testFixture, false);
        }

        [Test]
        public void TestFitnesseWithCommandThatDoesNotPass()
        {
            FireFoxSWATFixture testFixture = new FireFoxSWATFixture();
            SetDebugMode(testFixture, true);
            bool passed = false;

            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>CloseBrowser</td></tr><tr><td>GetElementAttribute</td><td>Expression</td><td>id:IdontExist</td><td>value</td><td>variable</td><td>a</td></tr></table>");
            
            try
            {
                testFixture.DoTable(testParse);
            }
            catch (SWAT.AssertionFailedException)
            {
                passed = true;
            }

            Assert.IsTrue(passed, "Command did not throw an exception as expected.");
            SetDebugMode(testFixture, false);
        }

        [Test]
        public void TestFitnesseWithCommandThatDoesNotPassAndAbandonTest()
        {
            InternetExplorerSWATFixture testFixture = new InternetExplorerSWATFixture();
            bool passed = true;

            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>www.google.com</td></tr><tr><td>@@RunScript</td><td>(5==5);</td><td>false</td></tr><tr><td>@@AbandonTest</td></tr><tr><td>@@SetElementAttribute</td><td>Name</td><td>q</td><td>value</td><td>test</td></tr><tr><td>@@ResumeTest</td></tr></table>");
            fit.Parse testResultParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>@@AttachToWindow</td><td>Google</td></tr><tr><td>@@AssertElementDoesNotExist</td><td>Expression</td><td>name=q;value=test</td><td>input</td></tr><tr><td>@@CloseBrowser</td></tr></table>");
            
            testFixture.DoTable(testParse);

            SetDebugMode(testFixture, true);
            try
            {
                testFixture.DoTable(testResultParse);
            }
            catch (SWAT.AssertionFailedException)
            {
                passed = false;
            }

            Assert.IsTrue(passed);
            SetDebugMode(testFixture, false);
        }

        [Test]
        public void TestFitnesseWithMultipleColumnData()
        {
            FireFoxSWATFixture testFixture = new FireFoxSWATFixture();
            SetDebugMode(testFixture, true);
            bool passed = false;

            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>BeginCompareData</td></tr><tr><td>Column1</td><td>Column2</td></tr><tr><td>Data1</td><td>Data2</td></tr><tr><td>EndCompareData</td></tr></table>");

            try
            {
                testFixture.DoTable(testParse);
            }
            catch (SWAT.AssertionFailedException)
            {
                passed = true;
            }

            Assert.IsTrue(passed, "Command did not throw an exception as expected.");
            SetDebugMode(testFixture, false);
        }

        [Test]
        public void TestSWATFixture()
        {
            SWAT.Fitnesse.SWATFixture testing = new SWATFixture();

            bool value = false;
            if (testing is SWATFixture)
                value = true;

            Assert.IsTrue(value, "SWAT Fixtures are not being constructed propertly");
        }

        [Test]
        public void FitnesseVariableRetrieverTest()
        {
            InternetExplorerSWATFixture testFixture = new InternetExplorerSWATFixture();
            SetDebugMode(testFixture, true);
            bool passed = true;

            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>RunScript</td><td>CSHARP</td><td>namespace SWAT{class test{public static string Main(){swatVars.Save(\"key2\", \"yo son\"); return swatVars.Recall(\"key2\");}}}</td><td>yo son</td></tr></table>");

            try
            {
                testFixture.DoTable(testParse);
            }
            catch (Exception)
            {
                passed = false;
            }

            Assert.IsTrue(passed, "Command failed unexpectedly.");
            SetDebugMode(testFixture, false);
        }

        [Test]
        [ExpectedException(typeof(SWATVariableDoesNotExistException))]
        public void FitnesseVariableThatDoesNotExistRetrieverTest()
        {
            InternetExplorerSWATFixture testFixture = new InternetExplorerSWATFixture();

            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>http://localhost/swat/TestPage.htm</td></tr><tr><td>SetElementAttribute</td><td>Id</td><td>txtOne</td><td>value</td><td>&gt;&gt;doesNotExit&lt;&lt;</td><td>input</td></tr></table>");

            try
            {
                testFixture.DoTable(testParse);
            }
            finally
            {
                ProcessKiller.Kill("iexplore");
            }
        }

        [Test]
        public void InverseModifierFailedAPassingCommandTest()
        {
            InternetExplorerSWATFixture testFixture = new InternetExplorerSWATFixture();
            SetDebugMode(testFixture, true);
            bool passed = true;

            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td><>&lt;&gt;AssertEqualTo</td><td>0</td><td>0</td></tr></table>");

            try
            {
                testFixture.DoTable(testParse);
            }
            catch (Exception)
            {
                passed = false;
            }

            Assert.IsTrue(passed, "Command failed unexpectedly.");
            SetDebugMode(testFixture, false);
        }

        [Test]
        public void CloseBrowsersBeforeTestStartTest()
        {
            // Save the value to reset it later
            bool reset = SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart;
            string ieProcessName = "iexplore";
            string ffProcessName = "firefox";
            bool areAllIEClosed;
            bool areAllFFClosed;
            WebBrowser _IEBrowser = new WebBrowser(BrowserType.InternetExplorer);
            WebBrowser _FFBrowser = new WebBrowser(BrowserType.FireFox);
            SWAT.Fitnesse.SWATFixture testing;

            // Open multiple windows to close
            _IEBrowser.OpenBrowser();
            _IEBrowser.NavigateBrowser("www.google.com");
            _IEBrowser.OpenBrowser();
            _IEBrowser.NavigateBrowser("www.w3schools.com");

            // Set the user setting to false
            SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = false;
            SWAT.UserConfigHandler.Save();

            // SWATFixture's constructor should not kill all open IE browsers
            testing = new InternetExplorerSWATFixture();

            // Assert that all the IE browsers have not closed
            areAllIEClosed = (Process.GetProcessesByName("iexplore").Length == 0);
            Assert.IsFalse(areAllIEClosed, "CloseBrowsersBeforeTestStart failed: " +
                           "closed all windows when the user setting was turned off.");

            // Open multiple windows to close again
            _IEBrowser.OpenBrowser();
            _IEBrowser.NavigateBrowser("www.google.com");
            _IEBrowser.OpenBrowser();
            _IEBrowser.NavigateBrowser("www.w3schools.com");

            // Set the user setting to true
            SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = true;
            SWAT.UserConfigHandler.Save();

            // SWATFixture's constructor should kill all open IE browsers
            testing = new InternetExplorerSWATFixture();

            // Assert that all the IE windows have been closed
            areAllIEClosed = (Process.GetProcessesByName(ieProcessName).Length == 0);
            Assert.IsTrue(areAllIEClosed, "CloseBrowsersBeforeTestStart failed: " +
                          "did not close all windows.");

            // Open multiple windows to close again
            _IEBrowser.OpenBrowser();
            _IEBrowser.NavigateBrowser("www.google.com");
            _IEBrowser.OpenBrowser();
            _IEBrowser.NavigateBrowser("www.w3schools.com");

            // Open multiple windows to close again
            _FFBrowser.OpenBrowser();
            _FFBrowser.NavigateBrowser("www.google.com");
            _FFBrowser.OpenBrowser();
            _FFBrowser.NavigateBrowser("www.w3schools.com");

            // SWATFixture's constructor should kill all open IE and FF browsers
            testing = new FireFoxSWATFixture();

            // Assert that all the IE windows have been closed
            areAllIEClosed = (Process.GetProcessesByName(ieProcessName).Length == 0);
            Assert.IsTrue(areAllIEClosed, "CloseBrowsersBeforeTestStart failed: " +
                          "did not close all windows.");

            // Assert that all the FF windows have been closed
            areAllFFClosed = (Process.GetProcessesByName(ffProcessName).Length == 0);
            Assert.IsTrue(areAllFFClosed, "CloseBrowsersBeforeTestStart failed: " +
                          "did not close all windows.");

            // Reset the user config setting
            SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart = reset;
            SWAT.UserConfigHandler.Save();

        }
        
        private static bool found;

        [Test]
        public void TestContinueSuspendedTest()
        {
            // Save the value to reset it later
            SWAT.WantSuspendOnFail.SuspendTestOnFail = true;
            SWAT.UserConfigHandler.Save();
            found = false;

            InternetExplorerSWATFixture testFixture = new InternetExplorerSWATFixture();
            Thread thread = new Thread(new ThreadStart(continueSuspendTest));
            
            //Begin searching for Suspend Test dialog
            thread.Start();
            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>www.google.com</td></tr><tr><td>AssertElementExists</td><td>Id</td><td>DoesNotExist</td></tr><tr><td>CloseBrowser</td></tr></table>");
            testFixture.DoTable(testParse);
            System.Threading.Thread.Sleep(500);
            thread.Abort();

            Assert.IsTrue(found);
            Assert.IsFalse(findBrowser());
            SWAT.WantSuspendOnFail.SuspendTestOnFail = false;
            SWAT.UserConfigHandler.Save();
        }

        [Test]
        public void TestEndSuspendedTest()
        {
            // Save the value to reset it later
            SWAT.WantSuspendOnFail.SuspendTestOnFail = true;
            SWAT.UserConfigHandler.Save();
            found = false;

            InternetExplorerSWATFixture testFixture = new InternetExplorerSWATFixture();
            Thread thread = new Thread(new ThreadStart(endSuspendTest));

            //Begin searching for Suspend Test dialog
            thread.Start();
            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>www.google.com</td></tr><tr><td>AssertElementExists</td><td>Id</td><td>DoesNotExist</td></tr><tr><td>CloseBrowser</td></tr></table>");
            testFixture.DoTable(testParse);
            System.Console.Write("Made it past the DoTable issue");
            System.Threading.Thread.Sleep(500);
            thread.Abort();

            Assert.IsTrue(found);
            Assert.IsTrue(findBrowser());
            SWAT.WantSuspendOnFail.SuspendTestOnFail = false;
            SWAT.UserConfigHandler.Save();
        }

        #region Suspend Test Helpers

        private static void continueSuspendTest()
        {
            List<IntPtr> list;
            while (true)
            {
                list = NativeMethods.GetAllOpenWindowsSorted();
                foreach (IntPtr hwnd in list)
                {
                    if (NativeMethods.GetWindowText(hwnd).Equals("Suspend Test"))
                    {
                        found = true;
                        NativeMethods.SetForegroundWindow(hwnd);
                        System.Threading.Thread.Sleep(250);
                        System.Windows.Forms.SendKeys.SendWait("{Enter}");
                    }
                }
            }
        }

        private static void endSuspendTest()
        {
            List<IntPtr> list;
            while (true)
            {
                list = NativeMethods.GetAllOpenWindowsSorted();
                foreach (IntPtr hwnd in list)
                {
                    if (NativeMethods.GetWindowText(hwnd).Equals("Suspend Test"))
                    {
                        found = true;
                        NativeMethods.SetForegroundWindow(hwnd);
                        System.Threading.Thread.Sleep(250);
                        System.Windows.Forms.SendKeys.SendWait("{Tab}");
                        System.Threading.Thread.Sleep(500);
                        System.Windows.Forms.SendKeys.SendWait("{Enter}");
                    }
                }
            }
        }

        private bool findBrowser()
        {
            List<IntPtr> list = NativeMethods.GetAllOpenWindowsSorted();
            foreach (IntPtr hwnd in list)
            {
                if (NativeMethods.GetWindowText(hwnd).Contains("Google"))
                {
                    NativeMethods.CloseWindow(hwnd);
                    return true;
                }
            }

            return false;
        }

        #endregion

        [Test]
        public void GetTimerValueWithTimerThatDoesNotExistsTest()
        {
            FireFoxSWATFixture testFixture = new FireFoxSWATFixture();
            SetDebugMode(testFixture, true);
            bool passed = true;

            string timerName = "timer1";
            string variable1 = "var1";
            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>GetTimerValue</td><td>" + variable1 + "</td><td>" + timerName + "</td></tr></table>");
            
            try
            {
                testFixture.DoTable(testParse);
                passed = false;
            }
            catch (AssertionFailedException)
            {
                passed = true;
            }

            Assert.IsTrue(passed, "Command failed unexpectedly.");
            SetDebugMode(testFixture, false);
        }

        [Test]
        public void TestDisplayTimerValueCorrectlyDisplaysResultUsingFitnesse()
        {
            FireFoxSWATFixture testFixture = new FireFoxSWATFixture();

            string timerName = "timer1";
            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>StartTimer</td><td>" + timerName + "</td></tr><tr><td>Sleep</td><td>1500</td></tr><tr><td>DisplayTimerValue</td><td>" + timerName + "</td></tr></table>");

            testFixture.DoTable(testParse);

            int timerValue;
            string result = parseString(testParse.Parts.At(3).Parts.At(1).ToString());
            System.Console.Write(int.TryParse(result, out timerValue));
            Assert.IsTrue(int.TryParse(result, out timerValue));
        }

        [Test]
        public void TestDisplayVariableCorrectlyDisplaysResultUsingFitnesse()
        {
            FireFoxSWATFixture testFixture = new FireFoxSWATFixture();

            string varName = "var1";
            fit.Parse testParse = new fit.Parse("<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>file:///C:/SWAT/trunk/SWAT.Tests/TestPages/TestPage.htm</td></tr><tr><td>GetElementAttribute</td><td>Id</td><td>btnSetVal</td><td>value</td><td>" + varName + "</td></tr><tr><td>DisplayVariable</td><td>" + varName + "</td></tr><tr><td>CloseBrowser</td></tr></table>");

            testFixture.DoTable(testParse);
            string expectedVarValue = "Set Value";

            string result = parseString(testParse.Parts.At(4).Parts.At(1).ToString());

            Assert.AreEqual(expectedVarValue, result);
        }

#region Utilities
        protected void SetDebugMode(SWATFixture fixture, bool mode)
        {
            ReflectionHelper.SetField(fixture, "InDebugMode", mode);
        }

        //Parse method for removing unwanted HTML codes
        private string parseString(string original)
        {
            string result = original;

            if (result.StartsWith("<td>"))
                result = result.Remove(0, 4);
            if (result.EndsWith("</td>"))
                result = result.Remove(result.Length - 5);

            return result;
        } 

#endregion
    }
}
