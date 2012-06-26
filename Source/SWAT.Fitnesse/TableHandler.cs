using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using SWAT.AbstractionEngine;
using System.Windows.Forms;

namespace SWAT.Fitnesse
{
    public class TableHandler
    {
        #region Class Variables & Properties

        protected static SWAT.AbstractionEngine.InvokeManager _mngr;
        private static int _inCompareDataIndex;
        private static List<string> _compareDatafieldNames;
        public static FitnesseVariableRetriever VarRetriever = new FitnesseVariableRetriever();
        public static string suspendParameters;
        public static bool continueSuspend = true;
        public static bool _finishBlockOnFailure = false;
        public static RowStatus Status { get; set; }
        public static List<string> Messages { get; set; }

        private bool ShouldCloseBrowsers
        {
            get { return SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart; }
        }

        #endregion

        private static void setupSuspend(StringCollection parameters)
        {
            suspendParameters = "|";
            foreach (string param in parameters)
                suspendParameters += param + "|";
        }

        public TableHandler(BrowserType browserType)
        {
            if (_mngr != null)
            {
                if (ShouldCloseBrowsers)
                    _mngr.KillBrowsers();
                _mngr.Dispose(); // we need to free up resources new create the new manager.
            }

            _mngr = new InvokeManager(browserType, new FitnesseVariableRetriever());

            // Close all open browsers 
            if (ShouldCloseBrowsers)
                _mngr.KillBrowsers();

            TestManager.ResetForNewTest();
        }

        public static void ProcessRow(TableRow row)
        {
            Status = RowStatus.Unprocessed;
            Messages = null;
            Command currentCommand = new Command(row.GetCellAt(0).Trim());
            string varKey = "";

            if (TestManager.ShouldExecute(currentCommand))
            {
                if (!TestManager.InCompareData)
                {
                    StringCollection parameters = row.GetParameters();

                    switch (currentCommand.Name)
                    {
                        case "GetElementAttribute":
                            {
                                varKey = parameters[3];
                                parameters.RemoveAt(3);
                                break;
                            }
                        case "GetDbRecord":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetDbDate":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetSavedDbDate":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetSavedDbDateDay":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetSavedDbDateMonth":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetSavedDbDateYear":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetLocation":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetWindowTitle":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "RunScriptSaveResult":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetDbRecordByColumnName":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "SetVariable":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetTimerValue":
                            {
                                varKey = parameters[0];
                                parameters.RemoveAt(0);
                                break;
                            }
                        case "GetConfigurationItem":
                            {
                                varKey = parameters[1];
                                parameters.RemoveAt(1);
                                break;
                            }
                        case "BeginCompareData":
                            {
                                TestManager.InCompareData = true;
                                currentCommand.IsCritical = false;
                                _inCompareDataIndex = 0; //Line was missing in original SWATFixture!
                                TestManager.InCompareDataIsCritical = false;

                                if (currentCommand.ShouldReport)
                                {
                                    Status = RowStatus.Passed;
                                }

                                return;
                            }

                        default:
                            break;
                    }

                    setupSuspend(parameters);
                    InvokeResult result = _mngr.Invoke(currentCommand.Name, parameters);

                    if (currentCommand.IsInverse)
                        currentCommand.Passed = !result.Success;
                    else currentCommand.Passed = result.Success;

                    if (!currentCommand.Passed && currentCommand.FinishBlockOnFailure)
                    {
                        _finishBlockOnFailure = true;
                    }

                    if (result.Success)
                    {
                        if (currentCommand.Name.Equals("GetElementAttribute") || currentCommand.Name.Equals("GetDbRecord") ||
                            currentCommand.Name.Contains("GetDbDate") || currentCommand.Name.Equals("GetLocation") ||
                            currentCommand.Name.Contains("GetSavedDbDate") || currentCommand.Name.Contains("GetSavedDbDateDay") || currentCommand.Name.Contains("GetSavedDbDateMonth") || currentCommand.Name.Contains("GetSavedDbDateYear") ||
                            currentCommand.Name.Equals("GetWindowTitle") || currentCommand.Name.Equals("RunScriptSaveResult") ||
                                     currentCommand.Name.Equals("GetDbRecordByColumnName") || currentCommand.Name.Equals("SetVariable") || currentCommand.Name.Equals("GetTimerValue") || currentCommand.Name.Equals("GetConfigurationItem"))
                            VarRetriever.Save(varKey, result.ReturnValue);
                        if (currentCommand.ShouldReport)
                            if (currentCommand.IsInverse)
                            {
                                Status = RowStatus.Failed;
                                Messages = new List<string> { "Inverse modifier failed passing command" };
                            }
                            else
                            {
                                if (currentCommand.Name.Equals("DisplayTimerValue") || currentCommand.Name.Equals("DisplayVariable"))
                                {
                                    row.SetCellAt(1, result.ReturnValue);
                                }
                                Status = RowStatus.Passed;
                            }
                    }
                    else
                    {
                        if (currentCommand.ShouldReport)
                            if (currentCommand.IsInverse)
                                Status = RowStatus.Passed;
                            else
                            {
                                Status = RowStatus.Failed;
                                Messages = new List<string> { result.FailureMessage };
                            }
                    }
                }
                else
                {
                    if (currentCommand.Name.Equals("EndCompareData", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentCommand.ShouldReport)
                        {
                            Status = RowStatus.Passed;
                        }

                        TestManager.InCompareData = false;
                    }
                    else if (!currentCommand.Name.Equals("EndCompareData", StringComparison.OrdinalIgnoreCase))
                    {
                        StringCollection rowParameters = row.GetParameters(true);

                        if (_inCompareDataIndex == 0) //we are in the row that defines column names
                        {
                            _compareDatafieldNames = new List<string>();

                            for (int colIndex = 0; colIndex < rowParameters.Count; colIndex++)
                            {
                                _compareDatafieldNames.Add(rowParameters[colIndex]);
                            }

                            currentCommand.Passed = true;
                            Status = RowStatus.Skipped;
                        }
                        else //we are now looking at actual row of data
                        {
                            Status = RowStatus.Varied;
                            Messages = new List<string>();
                            for (int colIndex = 0; colIndex < _compareDatafieldNames.Count; colIndex++)
                            {
                                StringCollection parameters = new StringCollection();
                                int dataRowIndex = _inCompareDataIndex - 1;
                                parameters.Add(dataRowIndex.ToString());
                                parameters.Add(_compareDatafieldNames[colIndex]);
                                parameters.Add(row.GetCellAt(colIndex)); //row.Parts.At(colIndex).Body);

                                InvokeResult result = _mngr.Invoke("AssertRecordValuesByColumnName", parameters);
                                currentCommand.Passed = result.Success;

                                if (currentCommand.Passed)
                                {
                                    if (currentCommand.ShouldReport)
                                        Messages.Add("pass");
                                }
                                else
                                {
                                    if (currentCommand.ShouldReport)
                                    {
                                        Messages.Add(result.FailureMessage);

                                        if (TestManager.InCompareDataIsCritical)
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    _inCompareDataIndex++;
                }

            }
            else
                Status = RowStatus.Ignored;

            TestManager.LogCommand(currentCommand);
        }
    }

    static public class TestManager
    {
        static private bool _ignoreRemaingTableRows = false;
        static private bool _ignoreRemainingTestRows = false;
        static private bool _ignoreNextCommand = false;
        static private bool _abandonTest = false;
        static private Command _previousCommand;
        public static bool _InCompareDataIsCritical = false;
        private static bool _finishBlockOnFailure = false;

        static public bool FinishBlockOnFailure
        {
            get { return _finishBlockOnFailure; }
            set { _finishBlockOnFailure = value; }
        }

        static public bool AbandonTest
        {
            get { return _abandonTest; }
            set { _abandonTest = value; }
        }

        static public Command PreviousCommand
        {
            get { return _previousCommand; }
            set { _previousCommand = value; }
        }

        static public bool IgnoreRemainingTableRows
        {
            get { return _ignoreRemaingTableRows; }
            set { _ignoreRemaingTableRows = value; }
        }

        static public bool IgnoreRemainingTestRows
        {
            get { return _ignoreRemainingTestRows; }
            set { _ignoreRemainingTestRows = value; }
        }

        static public bool SetAbandon(Command currentCommand)
        {
            //before evaluating, check for modifiers that will ignore this command
            if (ShouldNotIgnore() || currentCommand.MustExecuteCommand)
            {
                if (currentCommand.Name.Trim('|').Equals("AbandonTest"))
                {
                    TestManager.AbandonTest = true;
                    return true;
                }
                else if (currentCommand.Name.Trim('|').Equals("ResumeTest"))
                {
                    TestManager.AbandonTest = false;
                    return true;
                }

            }
            //if command is ignored or not Abandon or Resume
            return false;
        }

        static public bool ShouldExecute(Command currentCommand)
        {
            //if command was abandontest, resumetest and not ignored then do not continue evaluating command
            if (SetAbandon(currentCommand))
            {
                return true;
            }
            return ((ShouldNotIgnore() || currentCommand.MustExecuteCommand) && (!_abandonTest));
        }

        static private bool ShouldNotIgnore()
        {
            return (!IgnoreRemainingTableRows && !IgnoreRemainingTestRows && !_ignoreNextCommand && !_finishBlockOnFailure);
        }

        static public void ResetForNewTest()
        {
            _ignoreRemaingTableRows = false;
            _ignoreRemainingTestRows = false;
            _finishBlockOnFailure = false;
            _ignoreNextCommand = false;
            _previousCommand = null;
            _abandonTest = false;
            SWAT.Fitnesse.TableHandler.continueSuspend = true;
        }

        static public void ResetForNewTable()
        {
            _ignoreRemaingTableRows = false;
            _ignoreNextCommand = false;
        }

        private static bool _inCommandData;

        public static bool InCompareData
        {
            get { return _inCommandData; }
            set { _inCommandData = value; }
        }

        public static bool InCompareDataIsCritical
        {
            get { return _InCompareDataIsCritical; }
            set { _InCompareDataIsCritical = value; }
        }

        private static void SuspendTest(Command command)
        {
            DialogResult _result;
            _result = MessageBox.Show("Command Failed: |" + command.Name + TableHandler.suspendParameters + "\nContinue with the test?", "Suspend Test", MessageBoxButtons.YesNo);

            if (_result.Equals(DialogResult.No))
            {
                SWAT.Fitnesse.TestManager.IgnoreRemainingTestRows = true;
                TableHandler.continueSuspend = false;
            }
            else if (_result.Equals(DialogResult.Yes) && SWAT.Fitnesse.TestManager.IgnoreRemainingTestRows)
                SWAT.Fitnesse.TestManager.IgnoreRemainingTestRows = false;
        }

        static public void LogCommand(Command currentCommand)
        {
            _ignoreNextCommand = false; //reset
            
            if (!currentCommand.Passed)
            {
                if (currentCommand.IfStatementType == IfStatementType.Single)
                    _ignoreNextCommand = true;

                if (currentCommand.IfStatementType == IfStatementType.Block)
                    _ignoreRemaingTableRows = true;

                if (currentCommand.IsCritical)
                    _ignoreRemainingTestRows = true;

                if (currentCommand.IsInverse)
                    currentCommand.Passed = true;

                if (SWAT.WantSuspendOnFail.SuspendTestOnFail && TableHandler.continueSuspend && currentCommand.IfStatementType == IfStatementType.None)
                    SuspendTest(currentCommand);
            }
            else
            {
                if (currentCommand.IfStatementType == IfStatementType.IfNotSingle)
                    _ignoreNextCommand = true;

                if (currentCommand.IfStatementType == IfStatementType.IfNotBlock)
                    _ignoreRemaingTableRows = true;

                if (currentCommand.IsInverse)
                    currentCommand.Passed = false;
            }

            PreviousCommand = currentCommand;
        }
    }

    public enum RowStatus
    {
        Unprocessed,
        Skipped,
        Ignored,
        Passed,
        Failed,
        Varied
    }

    public enum IfStatementType
    {
        Single = 0,
        Block = 1,
        None = 2,
        IfNotSingle = 3,
        IfNotBlock = 4
    }

    public class Command
    {
        #region Private fields

        bool _isCritical = true;
        bool _isInverse = false;
        IfStatementType _ifStatementType = IfStatementType.None;
        bool _passed = true;
        bool _finishBlockOnFailure = false;
        string _command = string.Empty;
        string _fullCommand = string.Empty;
        //bool _executed; //UNUSED Variable

        #endregion

        public Command(string command)
        {
            Initialize(command);
        }

        private void Initialize(string command)
        {
            _command = command;
            setFullCommand(command);
            SetIsCritical(command);
            SetIfStatement(command);
            SetInverseCommand(command);
        }


        #region Private Methods

        private void setFullCommand(string command)
        {
            var indexOfEndPipe = command.IndexOf('|');
            var indexOfDecl = command.IndexOf("!|");

            if (indexOfDecl == -1 && indexOfEndPipe != -1)
                FullCommand = command.Substring(0, indexOfEndPipe);
            else
                FullCommand = command;
        }

        private void SetIsCritical(string command)
        {
            IsCritical = true; //by default we are critical. @@ is also critical.

            Regex regEx = new Regex(@"^(?<ampersands>@{1,3})(\w|[<][>]\w)");
            Group ampersands = regEx.Match(command).Groups["ampersands"];

            if (regEx.IsMatch(command))
            {
                if (ampersands.Length == 1) //@
                    IsCritical = false;

                if (ampersands.Length == 3) //@@@
                    IsCritical = false;
            }
            else if (command.StartsWith("?")) //?
                IsCritical = false;

            if(command.StartsWith("%")) //#
            {
                IsCritical = false;
                _finishBlockOnFailure = true;
            }
        }

        private void SetIfStatement(string command)
        {//?? not working properly!!!
            IfStatementType = IfStatementType.None;
            //^(?<question>\?{1,2})(\w|[<][>]\w|(?<exclamation>!\w)|(?<exclamation>!)[<][>]\w)
            Regex regEx = new Regex(@"^(?<question>\?{1,2})(\w|[<][>]\w|((?<exclamation>!)(\w|[<][>]\w)))");
            Group questionMarks = regEx.Match(command).Groups["question"];
            Group exclamationSigns = regEx.Match(command).Groups["exclamation"];

            if (regEx.IsMatch(command))
            {
                if (questionMarks.Length == 1)
                {
                    if (exclamationSigns.Length == 1)
                        IfStatementType = IfStatementType.IfNotSingle;
                    else
                        IfStatementType = IfStatementType.Single;
                }
                if (questionMarks.Length == 2)
                {
                    if (exclamationSigns.Length == 1)
                        IfStatementType = IfStatementType.IfNotBlock;
                    else
                        IfStatementType = IfStatementType.Block;
                }
            }
        }

        private void SetInverseCommand(string command)
        {
            //command beginning <> @<> @@<> @@@<> ?<> ?!<> ??<> ??!<> followed by alphanumeric character
            Regex regEx = new Regex(@"^((<>)|(@{1,3}<>)|(\?<>)|(\?!<>)|(\?\?<>)|(\?\?!<>))\w");

            if (regEx.IsMatch(command))
                IsInverse = true;
        }

        public string Name
        {
            get
            {
                string str = _command.Replace("@", "");
                str = str.Replace("?", "");
                str = str.Replace("<>", "");
                str = str.Replace("%", "");
                return str.Replace("!", "");
            }

        }

        public bool MustExecuteCommand
        {
            get
            {
                return _command.StartsWith("@@") || _command.CompareTo("EndCompareData") == 0;
            }
        }

        #endregion

        public bool IsCritical
        {
            get { return _isCritical; }
            set { _isCritical = value; }
        }

        public bool FinishBlockOnFailure
        {
            get { return _finishBlockOnFailure; }
            set { _finishBlockOnFailure = value; }
        }

        public bool IsInverse
        {
            get { return _isInverse; }
            set { _isInverse = value; }
        }
        public bool Passed
        {
            get { return _passed; }
            set { _passed = value; }
        }

        public string FullCommand
        {
            get { return _fullCommand; }
            set { _fullCommand = value; }
        }

        public IfStatementType IfStatementType
        {
            get { return _ifStatementType; }
            set { _ifStatementType = value; }
        }

        public bool ShouldReport
        {
            get
            {
                return (!_command.StartsWith("?"));
            }
        }
    }
}