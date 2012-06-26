using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SWAT_Editor;
using SWAT.Console;
using NUnit.Framework;

namespace SWAT.Tests.CommandLine
{
    [TestFixture]
    [Category("Misc")]
    public class CommandLineTestFixture
    {
        private string TestPage
        {
            get { return string.Format("http://{0}/swat/{1}", Environment.MachineName.ToLower(), "TestPage.htm"); }
        }

        private string SWATCommandLinePath
        {
            get
            {
                Process editor = new Process();
                string path = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(path);
                path = Uri.UnescapeDataString(uri.Path);
                path = Path.GetDirectoryName(path);
                return string.Format("{0}\\SWAT.Console.exe", path);
            }
        }

        #region Program entry point
        [Test]
        public void CommandLineEntryPointTest()
        {
            const string testFile = "testFile.in";
            const string outputFile = "testOut.out";

            var fileWriter = new StreamWriter(testFile);
            fileWriter.Close();
            try
            {
                var cmdline = new Process();

                cmdline.StartInfo.FileName = SWATCommandLinePath;
                cmdline.StartInfo.Arguments = string.Format("{0} {1}", testFile, outputFile);

                cmdline.Start();
                cmdline.WaitForExit();

                Assert.IsTrue(File.Exists(outputFile));
            }
            finally
            {
                File.Delete(testFile);
                File.Delete(outputFile);
            }
        }

        [Test]
        public void CommandLineEntryPointFileNotFoundHandledTest()
        {
            var cmdline = new Process();
            cmdline.StartInfo.FileName = SWATCommandLinePath;
            cmdline.StartInfo.Arguments = "testFile.in testOut.out";
            cmdline.Start();
            cmdline.WaitForExit();
        }

        [Test]
        public void CommandLineEntryPointIncorrectArgumentsHandledTest()
        {
            var cmdline = new Process();
            cmdline.StartInfo.FileName = SWATCommandLinePath;
            cmdline.Start();
            cmdline.WaitForExit();
        }
        #endregion

        #region File input/output

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void CommandLineHandlerReadNonexistentFileTest()
        {
            const string testFile = "testFile.in";
            var testObject = new CommandLineHandler(testFile);
        }

        [Test]
        public void CommandLineHandlerReadFileAndExecuteTest()
        {
            const string testFile = "testFile.in";
            var fileWriter = new StreamWriter(testFile);
            fileWriter.WriteLine("|OpenBrowser|");
            fileWriter.WriteLine("|CloseBrowser|");
            fileWriter.Close();

            try
            {
                var testHandler = new CommandLineHandler(testFile);
                testHandler.Start();

                List<CommandResult> results = testHandler.GetCommandResults();
                Assert.AreEqual(results.Count, 2);
                Assert.AreEqual(results[0].Command, "OpenBrowser");
                Assert.AreEqual(results[1].Command, "CloseBrowser");
                Assert.IsTrue(results[0].Success);
                Assert.IsTrue(results[1].Success);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        [Test]
        [Ignore]
        public void CommandLineHandlerWriteFileTest()
        {
            const string inputFile = "testFile.in";
            const string outputFile = "testFile.out";
            const string includeRoot = "C:\\TestSwatMacros";
            const string includeDir = "C:\\TestSwatMacros\\SetBrowser";
            const string includeFile = "C:\\TestSwatMacros\\SetBrowser\\content.txt";

            //Create Include file.
            Directory.CreateDirectory(includeDir);
            var includeWriter = new StreamWriter(includeFile);
            includeWriter.WriteLine("!|FireFoxSWATFixture|");
            includeWriter.WriteLine("This is a nested comment.");
            includeWriter.WriteLine("|OpenBrowser|");
            includeWriter.Close();

            var fileWriter = new StreamWriter(inputFile);
            fileWriter.WriteLine("!include TestSwatMacros.SetBrowser.DoesNotExist");
            fileWriter.WriteLine("!include TestSwatMacros.SetBrowser");
            fileWriter.WriteLine("!|InternetExplorerSWATFixture|");
            fileWriter.WriteLine("|OpenBrowser|");
            fileWriter.WriteLine("|NavigateBrowser|www.google.com|");
            fileWriter.WriteLine("|GetWindowTitle|currentTitle|");
            fileWriter.WriteLine("|AssertEqualTo|Google|>>currentTitle<<|");
            fileWriter.WriteLine("We expect this to fail.");
            fileWriter.WriteLine("|AssertElementExists|Id|fail)(*&^%$#@!|");
            fileWriter.WriteLine("|CloseBrowser|");
            fileWriter.WriteLine("|@@KillAllOpenBrowsers|");
            fileWriter.WriteLine("|NavigateBrowser|www.google.com|");
            fileWriter.WriteLine("|SetVariable|myVar|allswatcommands|");
            fileWriter.WriteLine("|SetElementAttribute|name|q|value|>>myVar<<|input|");
            fileWriter.WriteLine("|StimulateElement|name|btnG|onclick|input|");
            fileWriter.WriteLine("!include SwatMacros.UltiproEverest.CurrentBrowserType");
            fileWriter.WriteLine("|OpenBrowser|");
            fileWriter.Close();

            try
            {
                var testHandler = new CommandLineHandler(inputFile);
                testHandler.Start();
                testHandler.Save(outputFile);

                string[] expectedOutput =
                    {
                        "<html><head><title>testFile.in Results</title></head>",
                        "<body>",
                        "<center>",
                        "<font size=10>testFile.in Results</font>",
                        "<br>",
                        "<font size=4>Browser: FireFox</font>",
                        "<br>",
                        "<br>",
                        "<table width=30%>",
                        "<td bgcolor=green><font color=white>7 Right.</td>",
                        "<td bgcolor=red><font color=white>3 Wrong.</td>",
                        "<td bgcolor=gray><font color=white>6 Ignored.</td>",
                        "</table>",
                        "<br>",
                        "<table width=80%>",
                        "<td width=5%></td>",
                        "<td width=25%></td>",
                        "<td width=20%></td>",
                        "<td width=8%></td>",
                        "<td width=2%></td>",
                        "<td width=15%></td>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>1</td>",
                        "<td bgcolor=#FFF4D6><font color=black>!include TestSwatMacros.SetBrowser.DoesNotExist</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=red><font color=white>Failure</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black>Could not load macro at C:\\TestSwatMacros\\SetBrowser\\DoesNotExist\\</td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>2</td>",
                        "<td bgcolor=#FFF4D6><font color=black>!include TestSwatMacros.SetBrowser</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=green><font color=white>Success</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "</table>",
                        "<table width=80%>",
                        "<td width=5%></td>",
                        "<td>This is a nested comment.</td>",
                        "</table>",
                        "<table width=80%>",
                        "<td width=5%></td>",
                        "<td width=25%></td>",
                        "<td width=20%></td>",
                        "<td width=8%></td>",
                        "<td width=2%></td>",
                        "<td width=15%></td>",
                        "</table>",
                        "<table width=80%>",
                        "<td width=5%></td>",
                        "<td width=5%></td>",
                        "<td width=25%></td>",
                        "<td width=20%></td>",
                        "<td width=8%></td>",
                        "<td width=2%></td>",
                        "<td width=15%></td>",
                        "<tr>",
                        "<td width=5%></td>",
                        "<td bgcolor=#FFF4D6><font color=black>3</td>",
                        "<td bgcolor=#FFF4D6><font color=black>OpenBrowser</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=green><font color=white>Success</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "</table>",
                        "<table width=80%>",
                        "<td width=5%></td>",
                        "<td width=25%></td>",
                        "<td width=20%></td>",
                        "<td width=8%></td>",
                        "<td width=2%></td>",
                        "<td width=15%></td>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>4</td>",
                        "<td bgcolor=#FFF4D6><font color=black>OpenBrowser</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=green><font color=white>Success</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>5</td>",
                        "<td bgcolor=#FFF4D6><font color=black>NavigateBrowser</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">www.google.com</div>",
                        "</td>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=green><font color=white>Success</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>6</td>",
                        "<td bgcolor=#FFF4D6><font color=black>GetWindowTitle</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">currentTitle</div>",
                        "</td>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=green><font color=white>Success</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>7</td>",
                        "<td bgcolor=#FFF4D6><font color=black>AssertEqualTo</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">Google</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">Google</div>",
                        "</td>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=green><font color=white>Success</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "</table>",
                        "<br>",
                        "<table width=80%>",
                        "<td>We expect this to fail.</td>",
                        "</table>",
                        "<table width=80%>",
                        "<td width=5%></td>",
                        "<td width=25%></td>",
                        "<td width=20%></td>",
                        "<td width=8%></td>",
                        "<td width=2%></td>",
                        "<td width=15%></td>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>9</td>",
                        "<td bgcolor=#FFF4D6><font color=black>AssertElementExists|Id|fail)(*&^%$#@!|</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">Id</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">fail)(*&^%$#@!</div>",
                        "</td>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=red><font color=white>Failure</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black>Element with Id fail)(*&^%$#@! was not found.</td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>10</td>",
                        "<td bgcolor=#FFF4D6><font color=black>CloseBrowser</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=gray><font color=white>Ignored</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>11</td>",
                        "<td bgcolor=#FFF4D6><font color=black>@@KillAllOpenBrowsers</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=green><font color=white>Success</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>12</td>",
                        "<td bgcolor=#FFF4D6><font color=black>NavigateBrowser</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">www.google.com</div>",
                        "</td>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=gray><font color=white>Ignored</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>13</td>",
                        "<td bgcolor=#FFF4D6><font color=black>SetVariable</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">myVar</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">allswatcommands</div>",
                        "</td>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=gray><font color=white>Ignored</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>14</td>",
                        "<td bgcolor=#FFF4D6><font color=black>SetElementAttribute</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">name</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">q</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">value</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">>>myVar<<</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">input</div>",
                        "</td>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=gray><font color=white>Ignored</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>15</td>",
                        "<td bgcolor=#FFF4D6><font color=black>StimulateElement</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">name</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">btnG</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">onclick</div>",
                        "</td>",
                        "<td width=0% bgcolor=#FFF4D6>",
                        "<div style=\"border: solid 0 #060; border-left-width:2px; padding-left:0.5ex\">input</div>",
                        "</td>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=gray><font color=white>Ignored</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>16</td>",
                        "<td bgcolor=#FFF4D6><font color=black>!include SwatMacros.UltiproEverest.CurrentBrowserType</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=red><font color=white>Failure</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black>Could not load macro at C:\\SwatMacros\\UltiproEverest\\CurrentBrowserType\\</td>",
                        "</tr>",
                        "<tr>",
                        "<td bgcolor=#FFF4D6><font color=black>17</td>",
                        "<td bgcolor=#FFF4D6><font color=black>OpenBrowser</td>",
                        "<td width=20% bgcolor=#FFF4D6>",
                        "<table width=100%>",
                        "</td>",
                        "</table>",
                        "<td bgcolor=gray><font color=white>Ignored</td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "<td bgcolor=#FFF4D6><font color=black></td>",
                        "</tr>",
                        "</table>",
                        "</center>",
                        "</body>",
                        "</html>"
                    };

                var fileReader = new StreamReader(outputFile);
                string line;
                var actualOutput = new List<string>();
                while ((line = fileReader.ReadLine()) != null)
                    actualOutput.Add(line);

                fileReader.Close();

                for (int i = 0; i < actualOutput.Count; i++)
                    Assert.AreEqual(expectedOutput[i], actualOutput[i]);
            }
            finally
            {
                File.Delete(inputFile);
                File.Delete(outputFile);
                File.Delete(includeFile);
                Directory.Delete(includeDir);
                Directory.Delete(includeRoot);
            }
        }

        #endregion

        #region Browser selection

        [Test]
        public void CommandLineHandlerCorrectlyDeterminesFirefoxTest()
        {
            const string testFile = "testFile.in";
            var fileWriter = new StreamWriter(testFile);
            fileWriter.WriteLine("!|FireFoxSWATFixture|");
            fileWriter.Close();

            try
            {
                var testHandler = new CommandLineHandler(testFile);
                testHandler.Start();

                Assert.AreEqual(testHandler.GetBrowserType(), BrowserType.FireFox);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        [Test]
        public void CommandLineHandlerDefaultsToIeTest()
        {
            const string testFile = "testFile.in";
            var fileWriter = new StreamWriter(testFile);
            fileWriter.Close();

            try
            {
                var testHandler = new CommandLineHandler(testFile);
                testHandler.Start();

                Assert.AreEqual(testHandler.GetBrowserType(), BrowserType.InternetExplorer);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        #endregion

        #region Manipulating the Browser

        [Test]
        public void CommandLineHandlerBrowserManipulationTest()
        {
            const string testFile = "testFile.in";
            var fileWriter = new StreamWriter(testFile);
            fileWriter.WriteLine("|OpenBrowser|");
            fileWriter.WriteLine("|NavigateBrowser|" + TestPage + "|");
            fileWriter.WriteLine("|OpenBrowser|");
            fileWriter.WriteLine("|AttachToWindow|SWAT Test Page|");
            fileWriter.WriteLine("|RefreshBrowser|");
            fileWriter.WriteLine("|CloseBrowser|");
            fileWriter.WriteLine("|KillAllOpenBrowsers|");
            fileWriter.Close();

            try
            {
                var testHandler = new CommandLineHandler(testFile);
                testHandler.Start();

                List<CommandResult> results = testHandler.GetCommandResults();
                Assert.AreEqual(results.Count, 7);
                Assert.AreEqual(results[0].Command, "OpenBrowser");
                Assert.AreEqual(results[1].Command, "NavigateBrowser");
                Assert.AreEqual(results[2].Command, "OpenBrowser");
                Assert.AreEqual(results[3].Command, "AttachToWindow");
                Assert.AreEqual(results[4].Command, "RefreshBrowser");
                Assert.AreEqual(results[5].Command, "CloseBrowser");
                Assert.AreEqual(results[6].Command, "KillAllOpenBrowsers");

                foreach (CommandResult result in results)
                    Assert.IsTrue(result.Success);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        #endregion


        #region Test metrics

        [Test]
        public void CommandLineHandlerReportsIgnoredAndFailedTestsTest()
        {
            const string testFile = "testFile.in";
            var fileWriter = new StreamWriter(testFile);
            fileWriter.WriteLine("|OpenBrowser|");
            fileWriter.WriteLine("|NavigateBrowser|" + TestPage + "|");
            fileWriter.WriteLine("|AssertElementExists|Id|fail$!!!@)(*&^%$#@|");
            fileWriter.WriteLine("|RefreshBrowser|");
            fileWriter.WriteLine("|@@CloseBrowser|");
            fileWriter.Close();

            try
            {
                var testHandler = new CommandLineHandler(testFile);
                testHandler.Start();

                List<CommandResult> results = testHandler.GetCommandResults();
                Assert.AreEqual(results.Count, 5);
                Assert.AreEqual(results[0].Command, "OpenBrowser");
                Assert.AreEqual(results[1].Command, "NavigateBrowser");
                Assert.AreEqual(results[2].Command, "AssertElementExists");
                Assert.AreEqual(results[3].Command, "RefreshBrowser");
                Assert.AreEqual(results[4].Command, "CloseBrowser");

                Assert.IsTrue(results[0].Success);
                Assert.IsTrue(results[1].Success);
                Assert.IsFalse(results[2].Success);
                Assert.IsTrue(results[3].Ignored);
                Assert.IsTrue(results[4].Success);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        #endregion

        #region Command modifiers

        [Test]
        public void CommandLineHandlerCommentsTest()
        {
            const string testFile = "testFile.in";
            var fileWriter = new StreamWriter(testFile);
            fileWriter.WriteLine("|OpenBrowser|");
            fileWriter.WriteLine("#|NavigateBrowser|" + TestPage + "|");
            fileWriter.WriteLine("|RefreshBrowser|");
            fileWriter.WriteLine("|CloseBrowser|");
            fileWriter.WriteLine("{|OpenBroser|");
            fileWriter.WriteLine("|CloseBrowser|}");
            fileWriter.Close();

            try
            {
                var testHandler = new CommandLineHandler(testFile);
                testHandler.Start();

                List<CommandResult> results = testHandler.GetCommandResults();
                Assert.AreEqual(results.Count, 6);
                Assert.AreEqual(results[0].Command, "OpenBrowser");
                Assert.AreEqual(results[2].Command, "RefreshBrowser");
                Assert.AreEqual(results[3].Command, "CloseBrowser");

                Assert.IsTrue(results[0].Success);
                Assert.IsFalse(results[1].Success);
                Assert.IsTrue(results[2].Success);
                Assert.IsTrue(results[3].Success);
                Assert.IsFalse(results[4].Success);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        [Test]
        public void CommandLineFinishBlockOnFailureModifierTest()
        {
            const string testFile = "testFile.in";
            StreamWriter fileWriter = new StreamWriter(testFile);
            fileWriter.WriteLine("|OpenBrowser|");
            fileWriter.WriteLine("|NavigateBrowser|"+TestPage+"|");
            fileWriter.WriteLine("|%StimulateElement|Id|btnNwWindow|onclick|input|");
            fileWriter.WriteLine("|StimulateElement|Id|btnNewWindow|onclick|input|");
            fileWriter.WriteLine("|AttachToWindow|Second Window|");
            fileWriter.WriteLine("|CloseBrowser|");
            fileWriter.WriteLine("");
            fileWriter.WriteLine("|StimulateElement|Id|btnDialogNewPage|onclick|input|");
            fileWriter.WriteLine("|AssertJSDialogContent|Please press Ok to open new page|");
            fileWriter.WriteLine("|ClickJSDialog|CANCEL|");
            fileWriter.WriteLine("");
            fileWriter.WriteLine("|@@KillAllOpenBrowsers|");
            fileWriter.Close();
            try
            {
                CommandLineHandler testHandler = new CommandLineHandler(testFile);
                testHandler.Start();
                List<CommandResult> results = testHandler.GetCommandResults();
                Assert.AreEqual(results.Count, 12);
                Assert.AreEqual(results[0].Command, "OpenBrowser");
                Assert.AreEqual(results[1].Command, "NavigateBrowser");
                Assert.AreEqual(results[2].Command, "StimulateElement");
                Assert.AreEqual(results[3].Command, "StimulateElement");
                Assert.AreEqual(results[4].Command, "AttachToWindow");
                Assert.AreEqual(results[5].Command, "CloseBrowser");
                Assert.AreEqual(results[6].Command, null);
                Assert.AreEqual(results[7].Command, "StimulateElement");
                Assert.AreEqual(results[8].Command, "AssertJSDialogContent");
                Assert.AreEqual(results[9].Command, "ClickJSDialog");
                Assert.AreEqual(results[10].Command, null);
                Assert.AreEqual(results[11].Command, "KillAllOpenBrowsers");

                //Block 1
                Assert.IsTrue(results[0].Success);
                Assert.IsTrue(results[1].Success);
                Assert.IsFalse(results[2].Success);
                Assert.IsTrue(results[3].Success);
                Assert.IsTrue(results[4].Success);
                Assert.IsTrue(results[5].Success);
                //Block 2
                Assert.IsFalse(results[7].Success);
                Assert.IsFalse(results[8].Success);
                Assert.IsFalse(results[9].Success);
                //Block3
                Assert.IsTrue(results[11].Success);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        [Test]
        public void CommandLineHandlerModifiersTest()
        {
            const string testFile = "testFile.in";
            var fileWriter = new StreamWriter(testFile);
            fileWriter.WriteLine("|OpenBrowser|");
            fileWriter.WriteLine("|NavigateBrowser|" + TestPage + "|");
            fileWriter.WriteLine("|@AssertElementExists|Id|fail$!!!@)(*&^%$#@|");
            fileWriter.WriteLine("|RefreshBrowser|");
            fileWriter.WriteLine("|AssertElementExists|Id|fail$!!!@)(*&^%$#@|");
            fileWriter.WriteLine("|@@RefreshBrowser|");
            fileWriter.WriteLine("|AssertElementExists|Id|fail$!!!@)(*&^%$#@|");
            fileWriter.WriteLine("|@@@RefreshBrowser|");
            fileWriter.WriteLine("|@@<>RefreshBrowser|");
            fileWriter.WriteLine("|@@CloseBrowser|");
            fileWriter.Close();

            try
            {
                var testHandler = new CommandLineHandler(testFile);
                testHandler.Start();

                List<CommandResult> results = testHandler.GetCommandResults();
                Assert.AreEqual(results.Count, 10);
                Assert.AreEqual(results[0].Command, "OpenBrowser");
                Assert.AreEqual(results[1].Command, "NavigateBrowser");
                Assert.AreEqual(results[2].Command, "AssertElementExists");
                Assert.AreEqual(results[3].Command, "RefreshBrowser");
                Assert.AreEqual(results[4].Command, "AssertElementExists");
                Assert.AreEqual(results[5].Command, "RefreshBrowser");
                Assert.AreEqual(results[6].Command, "AssertElementExists");
                Assert.AreEqual(results[7].Command, "RefreshBrowser");
                Assert.AreEqual(results[8].Command, "RefreshBrowser");
                Assert.AreEqual(results[9].Command, "CloseBrowser");

                Assert.IsTrue(results[0].Success);
                Assert.IsTrue(results[1].Success);
                Assert.IsFalse(results[2].Success);
                Assert.IsTrue(results[3].Success);
                Assert.IsFalse(results[4].Success);
                Assert.IsTrue(results[5].Success);
                Assert.IsTrue(results[6].Ignored);
                Assert.IsTrue(results[7].Success);
                Assert.IsFalse(results[8].Success);
                Assert.IsTrue(results[9].Success);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        #endregion

        #region Using Variables

        [Test]
        public void SWATVariablesWorkInCommandLineTest()
        {
            const string testFile = "testVar.txt";
            StreamWriter file = new StreamWriter(testFile);
            file.WriteLine("!|Import|");
            file.WriteLine("|SWAT.Fitnesse|");
            file.WriteLine("!|InternetExplorerSWATFixture|");
            file.WriteLine("|OpenBrowser|");
            file.WriteLine("|NavigateBrowser|www.google.com|");
            file.WriteLine("|GetElementAttribute|name|btnG|value|buttonName|");
            file.WriteLine("|SetElementAttribute|name|q|value|>>buttonName<<|");
            file.WriteLine("|AssertElementExists|Expression|name:q;value:Search|");
            file.WriteLine("|@@CloseBrowser|");
            file.Close();
            try
            {
                CommandLineHandler handler = new CommandLineHandler(testFile);
                handler.Start();
                List<CommandResult> results = handler.GetCommandResults();
                Assert.IsTrue(results[7].Success, results[7].Message);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        [Test]
        public void FitnesseVariableWorksInCommandLineTest()
        {
            const string testFile = "testFitVar.txt";
            StreamWriter file = new StreamWriter(testFile);
            file.WriteLine("!|Import|");
            file.WriteLine("|SWAT.Fitnesse|");
            file.WriteLine("!define testVar {QUBW1}");
            file.WriteLine("!|InternetExplorerSWATFixture|");
            file.WriteLine("!|SWAT|");
            file.WriteLine("|OpenBrowser|");
            file.WriteLine("|NavigateBrowser|www.google.com|");
            file.WriteLine("|SetElementAttribute|name|q|value|${testVar}|");
            file.WriteLine("|AssertElementExists|Expression|name:q;value:QUBW1|");
            file.WriteLine("|@@CloseBrowser|");
            file.Close();
            try
            {
                CommandLineHandler handler = new CommandLineHandler(testFile);
                handler.Start();
                List<CommandResult> results = handler.GetCommandResults();
                Assert.IsTrue(results[7].Success, results[7].Message);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        [Test]
        public void FakeSWATVariableCausesFailureInCommandLineTest()
        {
            const string testFile = "testFakeSWATVar.txt";
            StreamWriter file = new StreamWriter(testFile);
            file.WriteLine("!|Import|");
            file.WriteLine("|SWAT.Fitnesse|");
            file.WriteLine("!|InternetExplorerSWATFixture|");
            file.WriteLine("|OpenBrowser|");
            file.WriteLine("|NavigateBrowser|www.google.com|");
            file.WriteLine("|GetElementAttribute|name|btnG|value|buttonName|");
            file.WriteLine("|SetElementAttribute|name|q|value|>>buttonNam<<|");
            file.WriteLine("|AssertElementExists|Expression|name:q;value:Search|");
            file.WriteLine("|@@CloseBrowser|");
            file.Close();
            try
            {
                CommandLineHandler handler = new CommandLineHandler(testFile);
                handler.Start();
                List<CommandResult> results = handler.GetCommandResults();
                Assert.IsFalse(results[7].Success, results[7].Message);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        [Test]
        public void FakeFitnesseVariableCausesFailureInCommandLineTest()
        {
            const string testFile = "testFakeFitVar.txt";
            StreamWriter file = new StreamWriter(testFile);
            file.WriteLine("!|Import|");
            file.WriteLine("|SWAT.Fitnesse|");
            file.WriteLine("!define testVar {QUBW1}");
            file.WriteLine("!|InternetExplorerSWATFixture|");
            file.WriteLine("!|SWAT|");
            file.WriteLine("|OpenBrowser|");
            file.WriteLine("|NavigateBrowser|www.google.com|");
            file.WriteLine("|SetElementAttribute|name|q|value|${testVa}|");
            file.WriteLine("|AssertElementExists|Expression|name:q;value:Search|");
            file.WriteLine("|@@CloseBrowser|");
            file.Close();
            try
            {
                CommandLineHandler handler = new CommandLineHandler(testFile);
                handler.Start();
                List<CommandResult> results = handler.GetCommandResults();
                Assert.IsFalse(results[7].Success, results[7].Message);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        [Test]
        public void SettingFitnesseVariableWithFakeVariableCausesFailureInCommandLineTest()
        {
            const string testFile = "testFakeFitVar.txt";
            StreamWriter file = new StreamWriter(testFile);
            file.WriteLine("!|Import|");
            file.WriteLine("|SWAT.Fitnesse|");
            file.WriteLine("!define testVar {QUBW1}");
            file.WriteLine("!define testVar2 {${testVa}}");
            file.WriteLine("!|InternetExplorerSWATFixture|");
            file.WriteLine("!|SWAT|");
            file.WriteLine("|OpenBrowser|");
            file.Close();
            try
            {
                CommandLineHandler handler = new CommandLineHandler(testFile);
                handler.Start();
                List<CommandResult> results = handler.GetCommandResults();
                Assert.IsFalse(results[3].Success, results[3].Message);
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        #endregion

        [Test]
        public void GetTimerValueWorksCorrectlyInCommandLineTest()
        {
            const string testFile = "testTimer.txt";
            StreamWriter file = new StreamWriter(testFile);
            file.WriteLine("!|Import|");
            file.WriteLine("|SWAT.Fitnesse|");
            file.WriteLine("!|InternetExplorerSWATFixture|");
            file.WriteLine("!|SWAT|");
            file.WriteLine("|OpenBrowser|");
            file.WriteLine("|NavigateBrowser|www.google.com|");
            file.WriteLine("!|SWAT|");
            file.WriteLine("|StartTimer|timer1|");
            file.WriteLine("|Sleep|4230|");
            file.WriteLine("|DisplayTimerValue|timer1|");
            file.WriteLine("|GetTimerValue|var1|timer1|");
            file.WriteLine("|SetElementAttribute|name|q|value|>>var1<<|input|");
            file.WriteLine("!|SWAT|");
            file.WriteLine("|@@CloseBrowser|");
            file.Close();
            try
            {
                CommandLineHandler handler = new CommandLineHandler(testFile);
                handler.Start();
                List<CommandResult> results = handler.GetCommandResults();
                Assert.IsTrue(results[13].Success, results[13].Message);
            }
            finally
            {
                File.Delete(testFile);
            }
        }
    }
}
