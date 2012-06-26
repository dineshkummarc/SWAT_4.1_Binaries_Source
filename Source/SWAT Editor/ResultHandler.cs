using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SWAT;

namespace SWAT_Editor
{
    public class ResultHandler
    {
        private BrowserType browserType;
        private string currentFileName = string.Empty;
        private string testName;
        private List<CommandResult> commandResults = new List<CommandResult>();
        private int totalIgnored;
        private int totalRight;
        private int totalWrong;

        public int TotalIgnored
        {
            get { return totalIgnored; }
        }

        public int TotalRight
        {
            get { return totalRight; }
        }

        public int TotalWrong
        {
            get { return totalWrong; }
        }

        public ResultHandler(string testName, BrowserType browserType, List<CommandResult> testResults)
        {
            this.testName = testName;
            this.browserType = browserType;
            commandResults = testResults;
            countResults(commandResults);
        }

        private void countResults(List<CommandResult> testResults)
        {
            bool inCompareData = false;

            foreach (var result in testResults)
            {
                if (result.Command == "BeginCompareData")
                    inCompareData = true;

                else if (result.Command == "EndCompareData")
                    inCompareData = false;

                else if (inCompareData)
                {
                    countCompareDataResults(result);
                    continue;
                }


                if (result.Command == null)
                    continue;
                if (!result.FullCommand.Contains("?")) //command with command modifiers will not count either as success or ignored
                {
                    if (result.Success)
                    {
                        totalRight++;
                    }
                    else if (result.Ignored || result.Cond)
                    {
                        if (!string.IsNullOrEmpty(result.Command))
                            totalIgnored++;
                    }
                    else if (!result.Success)
                        totalWrong++;
                }

                if (result.Children.Count > 0)
                    countResults(result.Children);
            }
        }

        private void countCompareDataResults(CommandResult result)
        {
            if (result.CompareDataResults.Count == 0)
                return;

            else
            {
                foreach (var cellResult in result.CompareDataResults)
                {
                    if (cellResult == "True")
                        totalRight++;
                    else
                        totalWrong++;
                }
            }
        }

        #region Output Test Results

        public void SaveResultsAsHtml(string outputFileName)
        {
            var fileWriter = new StreamWriter(outputFileName);
            var htmlWriter = new HtmlOutput();
            htmlWriter.CreateHeader(testName, browserType, totalRight, totalWrong, totalIgnored);

            foreach (var result in commandResults)
            {
                    htmlWriter.CreateEntry(result, outputFileName);
            }

            htmlWriter.CreateFooter();
            fileWriter.Write(htmlWriter.ToString());
            fileWriter.Close();
        }

        #endregion

        public List<CommandResult> GetCommandResults()
        {
            return commandResults;
        }

    }
}
