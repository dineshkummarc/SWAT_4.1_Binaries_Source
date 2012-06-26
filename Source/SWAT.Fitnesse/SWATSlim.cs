using fit;
using System;
using System.Collections.Generic;

namespace SWAT.Fitnesse
{
    public class SWATSlim : Fixture
    {
        private TableHandler Handler;
        private List<object> ResultsTable;
        private List<string> replacements;

        #region Constructor

        public SWATSlim(BrowserType browserType)
        {
            Handler = new TableHandler(browserType);
            Setup();
        }

        public SWATSlim() //blank constructor for calling commands
        {
            Setup();
        }

        private void Setup()
        {
            this.Processor = new fit.Service.Service(new fitSharp.Machine.Engine.Configuration());
        }

        #endregion

        #region Slim output

        public List<object> DoTable(List<List<String>> table)
        {
            ResultsTable = new List<object>();

            //check each row, and call methods appropriately
            foreach (List<string> row in table)
            {
                SlimRow sRow = new SlimRow(row);
                TableHandler.ProcessRow(sRow);

                replacements = FindDifferences(row, sRow.GetRow());

                HandleRowResult(row);
            }

            return ResultsTable;
        }

        #endregion

        #region Utilities

        private void HandleRowResult(List<string> row)
        {
            int count = row.Count;

            if (TableHandler.Status == RowStatus.Skipped)
                MarkRow("", count);
            else if (TableHandler.Status == RowStatus.Ignored)
                MarkRow("ignore", count);
            else
            {
                if (TableHandler.Status == RowStatus.Passed)
                    MarkRow("pass", count);
                else if (TableHandler.Status == RowStatus.Failed)
                    MarkRow("fail", count);
                else if (TableHandler.Status == RowStatus.Varied)
                    MarkRow(TableHandler.Messages, count);
            }

        }

        private void MarkRow(string msg, int length)
        {
            List<string> newRow = new List<string>(length);
            string repText = replacements[0];

            //Assert or Fail the first cell in the row only
            if (msg.Equals("fail"))
                newRow.Add("fail:" + TableHandler.Messages[0]);
            else
                newRow.Add(FormatString(msg, repText));

            for (int i = 1; i < length; i++) //Create stubs for the other cells
            {
                repText = replacements[i];

                newRow.Add(FormatString("", repText));
            }

            ResultsTable.Add(newRow);
        }

        private void MarkRow(List<string> messages, int length)
        {
            List<string> newRow = new List<string>(length);

            for (int i = 0; i < length; i++)
            {
                string msg = messages[i];

                if (msg != null && msg.Equals("pass"))
                    newRow.Add(FormatString(msg, replacements[i]));
                else
                    newRow.Add("fail:" + msg);
            }

            ResultsTable.Add(newRow);
        }

        #endregion

        #region Helper functions

        private string FormatString(string msg, string repText)
        {
            string result = "";

            if (!repText.Equals(""))
            {
                if (!msg.Equals(""))
                    result = msg + ":" + repText;
                else
                    result = "report:" + repText;
            }
            else
                result = msg;

            return result;
        }

        //Finds any variable symbols that were replaced with actual values and returns these changes
        private List<string> FindDifferences(List<string> original, List<string> processed)
        {
            List<string> row = new List<string>();

            for (int i = 0; i < original.Count; i++)
            {
                string text = original[i];
                string text2 = processed[i];
                if (!text.Equals(text2))
                    row.Add(text2);
                else
                    row.Add("");
            }

            return row;
        }

        #endregion
    }
}
