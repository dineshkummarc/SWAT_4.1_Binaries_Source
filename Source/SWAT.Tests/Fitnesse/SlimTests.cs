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


namespace SWAT.Tests.Fitnesse
{
    [TestFixture]
    public class SlimTests : FitnesseTestFixture
    {
        [Test]
        public void TestChromeSlimFixture()
        {
            ChromeSlimFixture testFixture = new ChromeSlimFixture();
        }

        [Test]
        public void TestSafariSlimFixture()
        {
            SafariSlimFixture testFixture = new SafariSlimFixture();
        }

        [Test]
        public void TestPressKeysUsingSlim()
        {
            TestManager.InCompareData = false;
            InternetExplorerSlimFixture testFixture = new InternetExplorerSlimFixture();

            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "OpenBrowser" });
            testTable.Add(new List<string> { "NavigateBrowser", "http://www.google.com/" });
            testTable.Add(new List<string> { "PressKeys", "Name", "q", "sample text", "input" });
            testTable.Add(new List<string> { "CloseBrowser" });
            //testTable.Add(new List<string> { "SetElementAttribute", "Name", "btnG", "value", "hacked", "input" });

            List<object> list = testFixture.DoTable(testTable);

            Assert.IsTrue(TablePasses(list));
        }

        [Test]
        public void TestGetelementAttributeAndSetAVariableUsingSlim()
        {
            /*
             * This test is importatnt because it saves a value to "variable", later on in another test there will be a Fixture.Recall(variable) will will 
             * replace the variable value for a new one. 
             */
            TestManager.InCompareData = false;
            FireFoxSlimFixture testFixture = new FireFoxSlimFixture();
            
            //<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>file:///C:/SWAT/trunk/SWAT.Tests/TestPages/TestPage.htm</td></tr><tr><td>GetElementAttribute</td><td>Expression</td><td>id:dd</td><td>innerHTML</td><td>variable</td><td>a</td></tr><tr><td>CloseBrowser</td> </tr></table>
            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "OpenBrowser" });
            testTable.Add(new List<string> { "NavigateBrowser", "file:///C:/SWAT/trunk/SWAT.Tests/TestPages/TestPage.htm" });
            testTable.Add(new List<string> { "GetElementAttribute", "Expression", "id:dd", "innerHTML", "variable", "a" });
            testTable.Add(new List<string> { "CloseBrowser" });

            List<object> list = testFixture.DoTable(testTable);

            Assert.IsTrue(TablePasses(list));
        }

        [Test]
        public void TestFitnesseWithInverserModifierUsingSlim()
        {
            FireFoxSlimFixture testFixture = new FireFoxSlimFixture();
            
            //<table><tr><td>SWATFixture</td></tr> <tr><td>OpenBrowser</td></tr><tr><td>NavigateBrowser</td><td>file:///C:/SWAT/trunk/SWAT.Tests/TestPages/TestPage.htm</td></tr><tr><td>&lt;&gt;GetElementAttribute</td><td>Expression</td><td>id:IDontExist</td><td>variable</td><td>a</td></tr><tr><td>CloseBrowser</td></tr></table>
            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "OpenBrowser" });
            testTable.Add(new List<string> { "NavigateBrowser", "file:///C:/SWAT/trunk/SWAT.Tests/TestPages/TestPage.htm" });
            testTable.Add(new List<string> { "&lt;&gt;GetElementAttribute", "Expression", "id:IDontExist", "variable", "a" });
            testTable.Add(new List<string> { "CloseBrowser" });

            List<object> list = testFixture.DoTable(testTable);

            Assert.IsTrue(TablePasses(list));
        }

        [Test]
        public void TestFitnesseWithCommandThatDoesNotPassUsingSlim()
        {
            FireFoxSlimFixture testFixture = new FireFoxSlimFixture();

            //<table><tr><td>SWATFixture</td></tr><tr><td>OpenBrowser</td></tr><tr><td>CloseBrowser</td></tr><tr><td>&lt;&gt;GetElementAttribute</td><td>Expression</td><td>id:IdontExist</td><td>value</td><td>variable</td><td>a</td></tr></table>
            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "OpenBrowser" });
            testTable.Add(new List<string> { "CloseBrowser" });
            testTable.Add(new List<string> { "GetElementAttribute", "Expression", "id:IDontExist", "value", "variable", "a" });
            //                              &lt;&gt;
            List<object> list = testFixture.DoTable(testTable);

            Assert.IsFalse(TablePasses(list));
        }

        [Test]
        public void TestFitnesseWithMultipleColumnFakeDataUsingSlim()
        {
            FireFoxSlimFixture testFixture = new FireFoxSlimFixture();

            //<table><tr><td>BeginCompareData</td></tr><tr><td>Column1</td><td>Column2</td></tr><tr><td>Data1</td><td>Data2</td></tr><tr><td>EndCompareData</td></tr></table>
            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "BeginCompareData" });
            testTable.Add(new List<string> { "Column1", "Column2" });
            testTable.Add(new List<string> { "Data1", "Data2" });
            testTable.Add(new List<string> { "EndCompareData" });

            List<object> list = testFixture.DoTable(testTable);

            Assert.IsFalse(TablePasses(list));
        }

        [Test]
        public void TestFitnesseWithMultipleColumnRealDataUsingSlim()
        {
            FireFoxSlimFixture testFixture = new FireFoxSlimFixture();

            //<table><tr><td>BeginCompareData</td></tr><tr><td>Column1</td><td>Column2</td></tr><tr><td>Data1</td><td>Data2</td></tr><tr><td>EndCompareData</td></tr></table>
            List<List<string>> testTable = new List<List<string>>();

            testTable.Add(new List<string> { "@@ConnectToMssql", "devausql01", "dev", "usg" });
            testTable.Add(new List<string> { "@@SetDatabase", "ULTIPRO_CALENDAR" });
            testTable.Add(new List<string> { "@@SetQuery", "select pgrperiodcontrol from paygroup where pgrPayGroup = 'QUBW1'" });
            testTable.Add(new List<string> { "@@GetDbRecordByColumnName", "varPeriodControl", "0", "pgrperiodcontrol" });
            testTable.Add(new List<string> { "SetQuery", "select pgpPayGroup, PgpPeriodControl from PgPayPer where pgpPayGroup = 'QUBW1' AND PgpPeriodControl = '>>varPeriodControl<<'" });
            
            testTable.Add(new List<string> { "BeginCompareData" });
            testTable.Add(new List<string> { "pgpPayGroup", "PgpPeriodControl" });
            testTable.Add(new List<string> { "QUBW1", ">>varPeriodControl<<" });
            testTable.Add(new List<string> { "EndCompareData" });
            testTable.Add(new List<string> { "AssertRecordCount", "1" });

            List<object> list = testFixture.DoTable(testTable);

            Assert.IsTrue(TablePasses(list));
        }

        [Test]
        public void TestSWATSlim()
        {
            SWAT.Fitnesse.SWATSlim testing = new SWATSlim();

            bool value = false;
            if (testing is SWATSlim)
                value = true;

            Assert.IsTrue(value, "Slim Fixtures are not being constructed propertly");
        }

        [Test]
        public void FitnesseVariableRetrieverTestUsingSlim()
        {
            InternetExplorerSlimFixture testFixture = new InternetExplorerSlimFixture();

            //<table><tr><td>SWATFixture</td></tr><tr><td>RunScript</td><td>CSHARP</td><td>namespace SWAT{class test{public static string Main(){swatVars.Save(\"key2\", \"yo son\"); return swatVars.Recall(\"key2\");}}}</td>yo son<td></td></tr></table>
            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "RunScript", "CSHARP", "namespace SWAT{class test{public static string Main(){swatVars.Save(\"key2\", \"yo son\"); return swatVars.Recall(\"key2\");}}}", "yo son" });

            List<object> list = testFixture.DoTable(testTable);

            Assert.IsTrue(TablePasses(list));
        }

        [Test]
        public void InverseModifierFailedAPassingCommandTestUsingSlim()
        {
            InternetExplorerSlimFixture testFixture = new InternetExplorerSlimFixture();

            //<table><tr><td>SWATFixture</td></tr><tr><td><>&lt;&gt;AssertEqualTo</td><td>0</td><td>0</td></tr></table>
            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "<>&lt;&gt;AssertEqualTo", "0", "0" });

            List<object> list = testFixture.DoTable(testTable);

            Assert.IsTrue(TablePasses(list));
        }

       [Test]
        public void TestGetTimerValueFailsWhenTimerDoesNotExistUsingSlim()
        {
            FireFoxSlimFixture testFixture = new FireFoxSlimFixture();
            
            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "GetTimerValue", "var1", "aTimer"});

            List<object> list = testFixture.DoTable(testTable);
            Assert.IsFalse(TablePasses(list));
        }

        [Test]
        public void TestDisplayTimerValueCorrectlyDisplaysResultUsingSlim()
        {
            FireFoxSlimFixture testFixture = new FireFoxSlimFixture();

            string timerName = "timer1";
            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "StartTimer", timerName });
            testTable.Add(new List<string> { "Sleep", "1500" });
            testTable.Add(new List<string> { "DisplayTimerValue", timerName });

            List<object> list = testFixture.DoTable(testTable);

            string result = ((List<string>)list[2])[1].ToString();
            int startingPos = result.LastIndexOf(":")+1;

            int timerValue;
            Assert.IsTrue(int.TryParse(result.Substring(startingPos), out timerValue));
        }

        [Test]
        public void TestDisplayVariableCorrectlyDisplaysResultUsingSlim()
        {
            FireFoxSlimFixture testFixture = new FireFoxSlimFixture();

            string varName = "var1";
            List<List<string>> testTable = new List<List<string>>();
            testTable.Add(new List<string> { "OpenBrowser" });
            testTable.Add(new List<string> { "NavigateBrowser", "file:///C:/SWAT/trunk/SWAT.Tests/TestPages/TestPage.htm" });
            testTable.Add(new List<string> { "GetElementAttribute", "Id", "btnSetVal", "value", varName });
            testTable.Add(new List<string> { "DisplayVariable", varName });
            testTable.Add(new List<string> { "CloseBrowser" });

            List<object> list = testFixture.DoTable(testTable);

            string result = ((List<string>)list[3])[1].ToString();
            int startingPos = result.LastIndexOf(":") + 1;

            string varValue = "Set Value";
            Assert.AreEqual(varValue, result.Substring(startingPos));
        }

        #region Utilities

        private static bool TablePasses(List<object> table)
        {
            foreach (object row in table)
            {
                foreach (string result in (List<string>)row)
                {
                    if (result == null || !result.Equals("") && result.StartsWith("fail"))
                        return false;
                }
            }

            return true;
        }

        #endregion
    }
}
