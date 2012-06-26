using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SWAT.Fitnesse;
using SWAT_Editor;

namespace SWAT.Console
{
    public class CommandLineHandler
    {
        private readonly string testName;
        private readonly List<string> commands;
        private BrowserType browserType = BrowserType.Null;
        private Thread currentTestThread;
        private ResultHandler resultHandler;

        public CommandLineHandler(string inputFileName)
        {
            commands = ReadLinesToList(inputFileName);
            testName = inputFileName;
            DetermineBrowserType();
        }

        #region Command Invokation

        public void Start()
        {
            ParameterizedThreadStart start = RunScript;

            currentTestThread = new Thread(start);
            currentTestThread.SetApartmentState(ApartmentState.STA);
            currentTestThread.Start(commands);
            currentTestThread.Join();
        }

        public void Save(string outputFile)
        {
            resultHandler.SaveResultsAsHtml(outputFile);
        }

        private void RunScript(object obj)
        {
            var lines = (List<string>)obj;

            using (var extractor = new CommandExtractor(browserType))
            {
                TestManager.ResetForNewTest();
                List<CommandResult> commandResults = extractor.ProcessWikiCommands(testName,lines.ToArray(), null);
                resultHandler = new ResultHandler(testName, browserType, commandResults);
            }
        }

        #endregion

        #region Determine Command Information

        private static List<string> ReadLinesToList(string inputFileName)
        {
            var listOfStrings = new List<string>();
            var fileReader = new StreamReader(inputFileName);

            string line;
            while ((line = fileReader.ReadLine()) != null)
                listOfStrings.Add(line);

            fileReader.Close();
            return listOfStrings;
        }

        private static BrowserType MatchBrowserType(string line)
        {
            if (line.StartsWith("!|FireFoxSWATFixture|") || line.StartsWith("!|FirefoxSWATFixture|"))
                return BrowserType.FireFox;

            /*if (line.StartsWith("!|ChromeSWATFixture|"))
                return BrowserType.Chrome;*/
            /*if (line.StartsWith("!|SafariSWATFixture|"))
                return BrowserType.Safari;*/

            return BrowserType.Null;
        }

        private void DetermineBrowserType(string [] input)
        {
            foreach (string command in input)
            {
                if (browserType != BrowserType.Null)
                    return;

                if (browserType == BrowserType.Null && 
                   (browserType = MatchBrowserType(command)) != BrowserType.Null)
                    return;

                if (command.StartsWith("!include"))
                {
                    var include = new SWAT_Editor.CommandExtractor.WikiInclude(command);
                    try
                    {
                        string[] lines = File.ReadAllLines
                            (SWAT.FitnesseSettings.FitnesseRootDirectory.TrimEnd('\\')
                            + "\\" + include.FilePath + "content.txt");

                        DetermineBrowserType(lines);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        //This will be reproduced in CommandExtractor. Do nothing here.
                    }
                }
            }
        }

        private void DetermineBrowserType()
        {
            DetermineBrowserType(commands.ToArray());

            //Couldn't find a BrowserType
            if (browserType == BrowserType.Null)
                browserType = BrowserType.InternetExplorer;
        }
        #endregion

        public BrowserType GetBrowserType()
        {
            return browserType;
        }

        public List<CommandResult> GetCommandResults()
        {
            return resultHandler.GetCommandResults();
        }
    }
}