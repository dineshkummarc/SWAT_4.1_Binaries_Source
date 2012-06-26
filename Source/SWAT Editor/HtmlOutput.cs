using SWAT.Utilities;
using SWAT_Editor;
using System.Text;
using System.Linq;
using System;
using SWAT;
using System.Collections;

namespace SWAT_Editor
{
    public class HtmlOutput
    {
        private bool tableBreak = false;
        private readonly HtmlUtil htmlGenerator = new HtmlUtil();
        private bool inCompareData = false;
        private bool summaryFile;
        private string hrefLocation;


        public HtmlOutput CreateHeader(string testName, BrowserType browserType, int totalRight, int totalWrong, int totalIgnored)
        {
            htmlGenerator.GenerateHeaderWithTitle(testName)
                .StartBody()
                .StartCenter()
                .GenerateTextWithSize(testName + " Results", 10)
                .GenerateBreakline()
                .GenerateTextWithSize("Browser: " + browserType, 4)
                .GenerateBreakline().GenerateBreakline()
                .GenerateTableHeaderWithWidth("30%")
                .GenerateTableColumnWithTextColorWithBGColor("" + totalRight + " Right.", Colors.ResultTextColor, "green")
                .GenerateTableColumnWithTextColorWithBGColor("" + totalWrong + " Wrong.", Colors.ResultTextColor, "red")
                .GenerateTableColumnWithTextColorWithBGColor("" + totalIgnored + " Ignored.", Colors.ResultTextColor, "gray")
                .GenerateTableFooter()
                .GenerateBreakline();

            SetUpTable();
            return this;
        }

        public void CreateEntry(CommandResult result, string outputFileName)
        {
            if (outputFileName.Contains("TestSummary"))
            {
                summaryFile = true;
                hrefLocation = outputFileName.Remove(outputFileName.IndexOf("TestSummary"));
            }


            if (result.Command == null && !inCompareData)
            {
                //Filter out !| declares.
                if (result.FullCommand != null && result.FullCommand.StartsWith("!|"))
                    return;

                CreateCommentEntry(result);
                return;
            }

            CreateCommandEntry(result);
        }

        public HtmlOutput CreateCommandEntry(CommandResult result)
        {
            return CreateCommandEntry(result, 0);
        }

        public HtmlOutput CreateCommandEntry(CommandResult result, int currentShift)
        {
            //Redirect children calls that are actually comments.
            if (result.Command == null && !inCompareData)
            {
                CreateCommentEntry(result, currentShift);
                return this;
            }

            //Filter out !| declares from possible children calls.
            if (result.FullCommand != null && result.FullCommand.StartsWith("!|"))
                return this;

            if (currentShift == 0 && tableBreak)
            {
                SetUpTable();
                tableBreak = false;
            }

            if (result.Command == "BeginCompareData")
            {
                inCompareData = true;
            }

            else if (result.Command == "EndCompareData")
            {
                inCompareData = false;
                FinalizeCompareDataTable();
            }

            else if (inCompareData)
            {
                //create the column titles for the database table
                if (result.CompareDataResults.Count == 0)
                    CreateCompareDataColumnEntry(result, currentShift);

                //handle the row results for Compare Data
                else
                    CreateCompareDataResultEntry(result, currentShift);

                return this;
            }

            if (currentShift > 0)
            {
                tableBreak = true;
                BreakTable(currentShift);
            }

            htmlGenerator.GenerateTableRowHeader();

            for (int i = 0; i < currentShift; i++)
                htmlGenerator.GenerateTableColumnWithWidth(string.Empty, "5%");

            string data = "";
            if (summaryFile)
            {
                data = "<a href=\"file:///" + hrefLocation + result.FullCommand + ".html\">" + result.FullCommand + "</a>";
            }
            else
            {
                data = result.FullCommand;
            }
            htmlGenerator
                    .GenerateTableColumnWithTextColorWithBGColor("" + result.LineNumber, Colors.GeneralTextColor, Colors.TableBgColor)
                    .GenerateTableColumnWithTextColorWithBGColor(data, Colors.GeneralTextColor, Colors.TableBgColor)
                    .GenerateTableColumnHeaderWithWidthWithBgColor("20%", Colors.TableBgColor)
                    .GenerateTableHeaderWithWidth("100%");

            result.Parameters.ForEach(param => BuildParameter(param));

            htmlGenerator.GenerateTableColumnFooter()
                .GenerateTableFooter();

            BuildCommandResult(result);

            htmlGenerator.GenerateTableRowFooter();

            //Unbreak table
            if (currentShift > 0)
                htmlGenerator.GenerateTableFooter();

            //Create command results for child commands.
            result.Children.ForEach(child => CreateCommandEntry(child, currentShift + 1));

            return this;
        }

        public HtmlOutput CreateCompareDataColumnEntry(CommandResult result, int currentShift)
        {
            //end the current table
            htmlGenerator.GenerateTableFooter();

            //start a new table for the database table
            htmlGenerator.GenerateTableHeaderWithWidth("80%");

            //shift the table over if needed
            for (int i = 0; i < currentShift; i++)
                htmlGenerator.GenerateTableColumnWithWidth(string.Empty, "5%");

            //create the column titles
            for (int colIndex = 0; colIndex < result.Parameters.Count; colIndex++)
            {
                htmlGenerator.GenerateTableColumnHeaderWithWidthWithBgColorWithAlignWithFontColor("0%", Colors.CompareDataColumn, "center", Colors.GeneralTextColor)
                    .GenerateDivWithStyle(result.Parameters[colIndex].ReplacedParam)
                    .GenerateTableColumnFooter();
            }

            return this;
        }

        public HtmlOutput CreateCompareDataResultEntry(CommandResult result, int currentShift)
        {
            string resultColor;
            string textForCell;

            //create a new row
            htmlGenerator.GenerateTableRowHeader();

            //shift over if needed
            for (int i = 0; i < currentShift; i++)
                htmlGenerator.GenerateTableColumnWithWidth(string.Empty, "5%");

            //enter the results in the row cells
            for (int colIndex = 0; colIndex < result.Parameters.Count; colIndex++)
            {
                if (result.CompareDataResults[colIndex] == "True")
                {
                    resultColor = Colors.SuccessBgColor;
                    textForCell = result.Parameters[colIndex].ReplacedParam;
                }
                else
                {
                    resultColor = Colors.FailureBgColor;
                    textForCell = result.Message;
                }

                htmlGenerator.GenerateTableColumnHeaderWithWidthWithBgColorWithAlignWithFontColor("0%", resultColor, "center", Colors.ResultTextColor)
                    .GenerateDivWithStyle(textForCell)
                    .GenerateTableColumnFooter();
            }

            //end the row
            htmlGenerator.GenerateTableRowFooter();
            return this;
        }

        public HtmlOutput FinalizeCompareDataTable()
        {
            //end the database table
            htmlGenerator.GenerateTableFooter();
            //start a new table
            SetUpTable();
            return this;
        }

        private void BuildParameter(CommandResult.ParameterEntry param)
        {
            string output = param.ReplacedParam;

            htmlGenerator.GenerateTableColumnHeaderWithWidthWithBgColor("0%", Colors.TableBgColor)
                .GenerateDivWithStyle(output)
                .GenerateTableColumnFooter();
        }

        private void BuildCommandResult(CommandResult result)
        {
            if (!result.FullCommand.Contains("?"))
            {
                if (result.Success)
                {
                    htmlGenerator.GenerateTableColumnWithTextColorWithBGColor("Success", Colors.ResultTextColor, Colors.SuccessBgColor)
                        .GenerateTableColumnWithTextColorWithBGColor(string.Empty, Colors.GeneralTextColor, Colors.TableBgColor)
                        .GenerateTableColumnWithTextColorWithBGColor(
                            (result.Message == null || result.Message.Equals("Success") || result.FullCommand.Contains("<>")) ? string.Empty : result.Message,
                            Colors.GeneralTextColor, Colors.TableBgColor);
                }
                else if (result.Ignored || result.Cond)
                {
                    htmlGenerator.GenerateTableColumnWithTextColorWithBGColor("Ignored", Colors.ResultTextColor,
                                                                              Colors.IgnoredBgColor)
                        .GenerateTableColumnWithTextColorWithBGColor(string.Empty, Colors.GeneralTextColor,
                                                                     Colors.TableBgColor)
                        .GenerateTableColumnWithTextColorWithBGColor(string.Empty, Colors.GeneralTextColor,
                                                                     Colors.TableBgColor);
                }
                else if (!result.Success)
                {
                    htmlGenerator.GenerateTableColumnWithTextColorWithBGColor("Failure", Colors.ResultTextColor,
                                                                              Colors.FailureBgColor)
                        .GenerateTableColumnWithTextColorWithBGColor(string.Empty, Colors.GeneralTextColor,
                                                                     Colors.TableBgColor)
                        .GenerateTableColumnWithTextColorWithBGColor(result.Message, Colors.GeneralTextColor,
                                                                     Colors.TableBgColor);
                }
            }
            else
            {
                htmlGenerator.GenerateTableColumnWithTextColorWithBGColor("", Colors.ResultTextColor, Colors.TableBgColor)
                        .GenerateTableColumnWithTextColorWithBGColor(string.Empty, Colors.GeneralTextColor, Colors.TableBgColor)
                        .GenerateTableColumnWithTextColorWithBGColor(
                        (result.Message == null || result.Message.Equals("Success") || (result.FullCommand.Contains("<>") && !result.Message.Equals("Success"))) ? string.Empty : result.Message,
                            Colors.GeneralTextColor, Colors.TableBgColor);
            }
        }

        public HtmlOutput CreateCommentEntry(CommandResult result)
        {
            return CreateCommentEntry(result, 0);
        }

        public HtmlOutput CreateCommentEntry(CommandResult result, int currentShift)
        {
            //Filter out !| declares from possible children calls.
            if (result.FullCommand != null && result.FullCommand.StartsWith("!|"))
                return this;

            if (result.FullCommand != string.Empty)
            {
                htmlGenerator.GenerateTableFooter();

                if (currentShift == 0)
                    htmlGenerator.GenerateBreakline();

                htmlGenerator.GenerateTableHeaderWithWidth("80%");

                if (currentShift > 0)
                {
                    for (int i = 0; i < currentShift; i++)
                        htmlGenerator.GenerateTableColumnWithWidth(string.Empty, "5%");
                }

                htmlGenerator
                    .GenerateTableColumn(result.FullCommand)
                    .GenerateTableFooter();

                SetUpTable();
            }

            return this;
        }

        private void SetUpTable()
        {
            htmlGenerator.GenerateTableHeaderWithWidth("80%")
                .GenerateTableColumnWithWidth(string.Empty, "5%")
                .GenerateTableColumnWithWidth(string.Empty, "25%")
                .GenerateTableColumnWithWidth(string.Empty, "20%")
                .GenerateTableColumnWithWidth(string.Empty, "8%")
                .GenerateTableColumnWithWidth("", "2%")
                .GenerateTableColumnWithWidth(string.Empty, "15%");
        }

        private void BreakTable(int currentShift)
        {
            htmlGenerator.GenerateTableFooter();
            htmlGenerator.GenerateTableHeaderWithWidth("80%");

            for (int i = 0; i < currentShift; i++)
                htmlGenerator.GenerateTableColumnWithWidth(string.Empty, "5%");

            htmlGenerator.GenerateTableColumnWithWidth(string.Empty, "5%")
                .GenerateTableColumnWithWidth(string.Empty, "25%")
                .GenerateTableColumnWithWidth(string.Empty, "20%")
                .GenerateTableColumnWithWidth(string.Empty, "8%")
                .GenerateTableColumnWithWidth("", "2%")
                .GenerateTableColumnWithWidth(string.Empty, "15%");
        }

        public HtmlOutput CreateFooter()
        {
            htmlGenerator.GenerateTableFooter()
                .EndCenter()
                .EndBody()
                .GenerateFooter();

            return this;
        }

        public new string ToString()
        {
            return htmlGenerator.ToString();
        }

        private static class Colors
        {
            public const string TableBgColor = "#FFF4D6";
            public const string CompareDataColumn = "#F2F2F2";
            public const string ResultTextColor = "white";
            public const string GeneralTextColor = "black";
            public const string SuccessBgColor = "green";
            public const string FailureBgColor = "red";
            public const string IgnoredBgColor = "gray";
        }
    }
}