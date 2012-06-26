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
using SWAT.AbstractionEngine;
using SWAT;
using System.Collections.Specialized;
using fitnesse.fitserver;
using fit;
using SWAT.Fitnesse;
using SWAT_Editor.Controls;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace SWAT_Editor
{
    public class CommandExtractor : IDisposable
    {
        #region Internal Fields
        private static int _inCompareDataIndex;
        private static List<string> _compareDatafieldNames;
        private InvokeManager _invokeManager;
        private Dictionary<string, string> _variables = new Dictionary<string, string>();
        public delegate void CommandProcessedHandler(CommandResult result);
        public event CommandProcessedHandler CommandProcessed;
        CommandExtractorStringCollection _usedCommands = new CommandExtractorStringCollection(typeof(SWAT.WebBrowser));
        private int lineNumber;
        public static bool _finishBlockOnFailure = false;
        #endregion


        #region Internal Classes
        protected class WikiVariable
        {
            private string _name;
            private string _value;

            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public WikiVariable(string wikiText)
            {
                //!define myVar (dude)
                string def = wikiText.Replace("!define ", "");
                int firstSpaceIndex = def.IndexOf(" ");
                Name = def.Substring(0, firstSpaceIndex);
                Value = def.Substring(firstSpaceIndex + 1, def.Length - (firstSpaceIndex + 1)).TrimStart('(').TrimEnd(')').TrimStart('{').TrimEnd('}');
            }
        }

        public class WikiInclude
        {
            private string _path;

            public string FilePath
            {
                get { return _path; }
                set { _path = value; }
            }

            public WikiInclude(string wikiCommand)
            {
                string def = wikiCommand.Replace("!include ", "");
                string[] macroPath = def.Split(' ');
                def = macroPath[macroPath.Length - 1];
                string[] sections = def.Split('.');

                foreach (string section in sections)
                {
                    FilePath += section + @"\";
                }
            }
        }

        #endregion


        #region Constructor

        public CommandExtractor(BrowserType browserType)
        {
            if (_invokeManager != null)
                _invokeManager.Dispose();

            _invokeManager = new InvokeManager(browserType, new EditorVariableRetriever(_variables));

            // Close any existing browsers before running test if the option is enabled
            if (SWAT.WantCloseBrowsersBeforeTestStart.CloseBrowsersBeforeTestStart)
                _invokeManager.KillBrowsers();
        }

        #endregion


        #region Public Methods

        public string replaceVar(string section)
        {
            Regex regEx = new Regex(@"((\${[^}]+}))|(>>.+?<<)");
            MatchCollection matches;

            if ((matches = regEx.Matches(section)).Count > 0)
            {
                foreach (Match match in matches)
                {
                    string var = match.Groups[0].Value.Replace("${", "").Replace("}", "").Replace(">>", "").Replace("<<", "");

                    if (_variables.ContainsKey(var))
                        section = section.Replace(match.Groups[0].Value, _variables[var]);
                    else
                        throw new SWATVariableDoesNotExistException(var);
                }
            }
            return section;
        }

        public void CheckForBreakPointOnLine(List<BreakPoint> breakPoints, int lineNumber, System.Threading.Thread activeThread, CommandEditorPage currentPage)
        {
            for (int i = 0; i < breakPoints.Count; i++)
            {
                if (breakPoints[i].BPLineNumber == lineNumber && breakPoints[i].BPActive)
                {
                    currentPage.EnableCurrentBreakPoint(lineNumber);
                    //currentPage.getEditor().EnableSelectedLine(lineNumber);
                    breakPoints[i].PauseForBreakPoint(activeThread);
                    currentPage.DisableCurrentBreakPoint(lineNumber, "normal");
                    return;
                }
            }
            if (breakPoints[breakPoints.Count - 1].TempBP)
            {
                breakPoints.RemoveAt(breakPoints.Count - 1);
                currentPage.EnableCurrentBreakPoint(lineNumber);
                //currentPage.getEditor().EnableSelectedLine(lineNumber);
                breakPoints[0].PauseForBreakPoint(activeThread);
                currentPage.DisableCurrentBreakPoint(lineNumber, "temp");
                return;
            }
        }

        //Command-line mode.
        public List<CommandResult> ProcessWikiCommands(string fileName, string[] wikiCommands, System.Threading.Thread activeThread)
        {
            List<CommandResult> listOfResults = new List<CommandResult>();
            lineNumber = 0;
            ProcessWikiCommands(wikiCommands, new List<BreakPoint>(), activeThread, null, true, listOfResults, fileName);
            return listOfResults;
        }

        //DDE mode
        public List<CommandResult> ProcessWikiCommands(string fileName, string[] wikiCommands, System.Threading.Thread activeThread, CommandEditorPage currentPage)
        {
            List<CommandResult> listOfResults = new List<CommandResult>();
            lineNumber = 0;
            ProcessWikiCommands(wikiCommands, new List<BreakPoint>(), activeThread, currentPage, false, listOfResults, fileName);
            return listOfResults;
        }

        public void ProcessWikiCommands(string[] wikiCommands, List<BreakPoint> breakPoints, System.Threading.Thread activeThread, CommandEditorPage currentPage)
        {
            lineNumber = 0;
            ProcessWikiCommands(wikiCommands, breakPoints, activeThread, currentPage, false, new List<CommandResult>(), string.Empty);
        }

        public void ProcessWikiCommands(string[] wikiCommands, List<BreakPoint> breakPoints, System.Threading.Thread activeThread, CommandEditorPage currentPage, bool commandLineMode, List<CommandResult> listOfResults, string fileName)
        {
            bool inComment = false;
            string cleanedCommand = "";

            foreach (string wikiCommand in wikiCommands)
            {
                lineNumber++;
                Command mngrCommand = new Command(wikiCommand.TrimStart('|'));
                cleanedCommand = cleanCommand(wikiCommand);
                if (breakPoints.Count > 0)
                    CheckForBreakPointOnLine(breakPoints, lineNumber, activeThread, currentPage);

                CommandResult returnResult = new CommandResult();
                returnResult.FileName = fileName;
                returnResult.LineNumber = lineNumber;
                returnResult.FullCommand = mngrCommand.FullCommand;
                listOfResults.Add(returnResult);

                if (cleanedCommand.Equals(String.Empty))
                {
                    if (_finishBlockOnFailure)
                    {
                        TestManager.FinishBlockOnFailure = true;
                        _finishBlockOnFailure = false;
                    }
                    TestManager.ResetForNewTable(); 
                }


                if (!cleanedCommand.StartsWith("#") && !inComment && !cleanedCommand.StartsWith("{{{") && !cleanedCommand.StartsWith("}}}") && !isIgnoredCommand(cleanedCommand) && !TestManager.InCompareData)
                {

                    if (cleanedCommand.StartsWith("!"))
                    {
                        if (cleanedCommand.StartsWith("!define"))
                        {
                            try
                            {
                                cleanedCommand = replaceVar(cleanedCommand);
                            }
                            catch (SWATVariableDoesNotExistException e)
                            {
                                returnResult.Success = false;
                                mngrCommand.Passed = false;
                                returnResult.Message = e.Message;
                                returnResult.Command = "Could not be defined";
                                TestManager.LogCommand(mngrCommand);
                                goto Exit;
                            }

                            WikiVariable var = new WikiVariable(cleanedCommand);

                            if (!_variables.ContainsKey(var.Name))
                                _variables.Add(var.Name, var.Value);
                            else
                                _variables[var.Name] = var.Value;

                            returnResult.Success = true;
                            returnResult.Command = string.Format("Defined {0} as {1}", var.Name, var.Value);
                        }
                        if (cleanedCommand.StartsWith("!include"))
                        {
                            WikiInclude include = new WikiInclude(cleanedCommand);

                            try
                            {
                                string newFileName = SWAT.FitnesseSettings.FitnesseRootDirectory.TrimEnd('\\') + "\\" + include.FilePath + "content.txt";
                                string[] lines = File.ReadAllLines(newFileName);

                                //Need an intermediate slot in the results.
                                returnResult.Command = "INCLUDE";
                                returnResult.FullCommand = cleanedCommand;
                                returnResult.Success = true;
                                returnResult.Ignored = false;

                                ProcessWikiCommands(lines, breakPoints, activeThread, currentPage, commandLineMode, returnResult.Children, newFileName);
                            }
                            catch (DirectoryNotFoundException)
                            {
                                returnResult.Success = false;
                                returnResult.Ignored = false;
                                returnResult.Message = string.Format("Could not load macro at {0}", SWAT.FitnesseSettings.FitnesseRootDirectory.TrimEnd('\\') + "\\" + include.FilePath);
                                returnResult.Command = "INCLUDE";
                            }
                        }
                    }
                    else
                    {

                        //string[] sections = cleanedCommand.TrimStart('|').TrimEnd('|').Split('|');
                        // Remove leading pipes
                        cleanedCommand = cleanedCommand.TrimStart('|');
                        // Remove last pipe. Can't use TrimEnd('|') in case we are passing empty parameters
                        string[] sections = parseSections(cleanedCommand.Substring(0, cleanedCommand.LastIndexOf('|')));

                        StringCollection parameters = new StringCollection();

                        if (sections.Length < 1)
                        {
                            returnResult.Success = false;
                            returnResult.Message = "Command is not formated correctly.";
                            returnResult.Command = cleanedCommand;
                            goto Exit;

                        }
                        string command = sections[0];
                        returnResult.Command = command;

                        for (int i = 1; i < sections.Length; i++)
                        {
                            string replace = "";

                            try
                            {
                                replace = replaceVar(sections[i]);
                            }
                            catch (SWATVariableDoesNotExistException e)
                            {
                                parameters.Add(sections[i]);
                                var paramEntryex = new CommandResult.ParameterEntry { OriginalParam = sections[i], ReplacedParam = sections[i] };
                                returnResult.Parameters.Add(paramEntryex);
                                returnResult.Success = false;
                                mngrCommand.Passed = false;
                                returnResult.Message = e.Message;
                                TestManager.LogCommand(mngrCommand);
                                goto Exit;
                            }

                            parameters.Add(replace);

                            var paramEntry = new CommandResult.ParameterEntry { OriginalParam = sections[i], ReplacedParam = replace };
                            returnResult.Parameters.Add(paramEntry);
                        }

                        if (TestManager.ShouldExecute(mngrCommand))
                        {
                            try
                            {
                                string varKey = "";

                                if (command == "GetElementAttribute")
                                {
                                    varKey = parameters[3];
                                    parameters.RemoveAt(3);
                                }

                                if (command == "GetConfigurationItem")
                                {
                                    varKey = parameters[1];
                                    parameters.RemoveAt(1);
                                }

                                if (command.Contains("GetDbRecord") || command.Contains("GetDbDate") || command.Contains("GetLocation")
                                    || command.Contains("GetWindowTitle") || command.Contains("RunScriptSaveResult") || command.Contains("SetVariable")
                                    || command.StartsWith("GetSavedDbDate") || command == "GetTimerValue")
                                {
                                    varKey = parameters[0];
                                    parameters.RemoveAt(0);
                                }

                                if(command == "BeginCompareData")
                                {
                                    TestManager.InCompareData = true;
                                    _inCompareDataIndex = 0;
                                    TestManager.InCompareDataIsCritical = false;
                                }

                                InvokeResult result = _invokeManager.Invoke(command, parameters);

                                if (mngrCommand.IsInverse)
                                {
                                    returnResult.Success = !result.Success;
                                    mngrCommand.Passed = !result.Success;
                                }
                                else
                                {
                                    returnResult.Success = result.Success;
                                    mngrCommand.Passed = result.Success;
                                }

                                if (!mngrCommand.Passed && mngrCommand.FinishBlockOnFailure)
                                {
                                    _finishBlockOnFailure = true;
                                }

                                returnResult.Cond = wikiCommand.StartsWith("|?");

                                //Notify the user on success.
                                if (result.Success)
                                {
                                    //If original command failed, now passes with inverse modifier
                                    if (mngrCommand.IsInverse)
                                        returnResult.Message = "Inverse modifier failed passing command";
                                    else
                                        returnResult.Message = "Success";


                                    //C# Operator == is overloaded, so Equals() method is called when used. That is why it can be used to compare Strings.
                                    if ((command == "GetElementAttribute" || command.Contains("GetDbRecord") || command == "GetDbDate" || command == "GetLocation" || command == "GetConfigurationItem")
                                        || command == "GetWindowTitle" || command == "RunScriptSaveResult" || command == "SetVariable" || command.StartsWith("GetSavedDbDate") || command == "GetTimerValue")
                                    {
                                        if (_variables.ContainsKey(varKey.ToString()))
                                        {
                                            _variables.Remove(varKey);
                                        }

                                        _variables.Add(varKey, result.ReturnValue);
                                    }

                                    if (command == "DisplayVariable" || command == "DisplayTimerValue")
                                    {
                                        returnResult.Parameters[0].ReplacedParam = result.ReturnValue;
                                    }
                                }

                                if (!result.Success)
                                {
                                    if (mngrCommand.IsInverse)
                                        returnResult.Message = "Success - Inverse modifier passed failing command. Original error message: " + result.FailureMessage;
                                    else
                                        returnResult.Message = result.FailureMessage;
                                }

                            }
                            catch (Exception e)
                            {
                                returnResult.Success = false;
                                returnResult.Message = e.Message;
                                if (mngrCommand.Passed && mngrCommand.FinishBlockOnFailure)
                                {
                                    _finishBlockOnFailure = true;
                                }
                            }
                        }
                        else
                        {
                            returnResult.Ignored = true;
                            returnResult.ModIgn = true;
                        }

                        TestManager.LogCommand(mngrCommand);
                    }

                }
                else if (TestManager.InCompareData)
                {
                    cleanedCommand = cleanCommand(wikiCommand);
                    cleanedCommand = cleanedCommand.TrimStart('|');
                    string[] sections = parseSections(cleanedCommand.Substring(0, cleanedCommand.LastIndexOf('|')));

                    // 1. if endCompareData
                    if (wikiCommand.Contains("EndCompareData"))
                    {
                        returnResult.Command = "EndCompareData";
                        returnResult.Success = true;
                        mngrCommand.Passed = true;
                        returnResult.Message = "Success";
                        TestManager.LogCommand(mngrCommand);
                        TestManager.InCompareData = false;
                    }
                    else
                    {
                        //StringCollection rowParameters = new StringCollection();
                        for (int i = 0; i < sections.Length; i++)
                        {
                            string replace = "";
                            replace = replaceVar(sections[i]);
                            //rowParameters.Add(replace);
                            var paramEntry = new CommandResult.ParameterEntry { OriginalParam = sections[i], ReplacedParam = replace };
                            returnResult.Parameters.Add(paramEntry);
                        }

                        // 2. if _inCompareDataIndex = 0 we are reading the columns
                        if (_inCompareDataIndex == 0)
                        {
                            _compareDatafieldNames = new List<string>();

                            for (int colIndex = 0; colIndex < returnResult.Parameters.Count; colIndex++)
                            {
                                _compareDatafieldNames.Add(returnResult.Parameters[colIndex].ReplacedParam);
                            }
                        }
                        // 3. otherwise we are now looking at actual row of data
                        else
                        {
                            for (int colIndex = 0; colIndex < _compareDatafieldNames.Count; colIndex++)
                            {
                                StringCollection parameters = new StringCollection();
                                int dataRowIndex = _inCompareDataIndex - 1;
                                parameters.Add(dataRowIndex.ToString());
                                parameters.Add(_compareDatafieldNames[colIndex]);
                                parameters.Add(returnResult.Parameters[colIndex].ReplacedParam);

                                InvokeResult result = _invokeManager.Invoke("AssertRecordValuesByColumnName", parameters);

                                if (!result.Success)
                                    returnResult.Message = result.FailureMessage;

                                returnResult.CompareDataResults.Add(result.Success.ToString());
                            }
                        }

                        _inCompareDataIndex++;
                    }
                }
                else
                {
                    returnResult.Ignored = true;
                    returnResult.ModIgn = false;

                    if (wikiCommand.StartsWith("{{{"))
                        inComment = true;
                    if (wikiCommand.EndsWith("}}}"))
                        inComment = false;
                }
            Exit:
                if (!commandLineMode)
                    CommandProcessed(returnResult);
            }
        }

        #endregion



        #region Helper Methods


        public string cleanCommand(string text)
        {
            text = text.Trim();
            Regex atSyms = new Regex(@"^\|(@{1,3})");
            Regex inverseSyms = new Regex(@"^\|<>");
            Regex ifSyms = new Regex(@"^\|(\?{1,2})");
            Regex ifNotSyms = new Regex(@"^\|!");
            Regex percentSym = new Regex(@"^\|%");
            string s = text;


            s = atSyms.Replace(text, "|");
            s = ifSyms.Replace(s, "|");
            s = ifNotSyms.Replace(s, "|");
            s = inverseSyms.Replace(s, "|");
            s = percentSym.Replace(s, "|");
            return s;
        }

        private bool isIgnoredCommand(string command)
        {
            return !_usedCommands.ItemStartsWith(command);
        }

        private string[] parseSections(string command)
        {
            int beginEscapeIndex = command.IndexOf("!-", 0);
            int endEscapeIndex;

            if (beginEscapeIndex == -1 || command.IndexOf("-!", beginEscapeIndex + 2) == -1)
            {
                return command.Split('|');
            }
            else
            {
                List<string> list = new List<string>();
                int baseIndex = 0;
                int pipeIndex = 0;
                string data = string.Empty;

                while (beginEscapeIndex != -1 || pipeIndex != -1)
                {
                    beginEscapeIndex = command.IndexOf("!-", baseIndex);
                    pipeIndex = command.IndexOf('|', baseIndex);

                    if (beginEscapeIndex == -1 && pipeIndex == -1)
                    {
                        list.Add(string.Concat(data, command.Substring(baseIndex, (command.Length - baseIndex))));
                    }
                    else if (beginEscapeIndex == -1 || (pipeIndex != -1 && pipeIndex < beginEscapeIndex))
                    {
                        list.Add(string.Concat(data, command.Substring(baseIndex, pipeIndex - baseIndex)));
                        data = string.Empty;
                        baseIndex = pipeIndex + 1;
                    }
                    else
                    {
                        endEscapeIndex = command.IndexOf("-!", beginEscapeIndex + 2);

                        if (endEscapeIndex == -1)
                        {
                            if (pipeIndex == -1)
                            {
                                list.Add(string.Concat(data, command.Substring(baseIndex, (command.Length - baseIndex))));
                                beginEscapeIndex = -1;
                            }
                            else
                            {
                                list.Add(string.Concat(data, command.Substring(baseIndex, pipeIndex - baseIndex)));
                                data = string.Empty;
                                baseIndex = pipeIndex + 1;
                            }
                        }
                        else
                        {
                            while (true)
                            {
                                if (endEscapeIndex + 3 < command.Length && command.Substring(endEscapeIndex + 2, 2).Equals("-!"))
                                {
                                    endEscapeIndex += 2;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            data = string.Concat(data, string.Concat(command.Substring(baseIndex, beginEscapeIndex - baseIndex),
                                command.Substring(beginEscapeIndex + 2, endEscapeIndex - (beginEscapeIndex + 2))));
                            baseIndex = endEscapeIndex + 2;
                        }
                    }
                }

                return list.ToArray();
            }
        }

        #endregion


        private class CommandExtractorStringCollection : StringCollection
        {

            public CommandExtractorStringCollection(Type webBrowserObjectType)
            {
                this.AddRange(new string[] { @"!define .+ \(.+\)", @"!define .+ \(.*\)", "!define .+ {.+}", "!include .+" });

                foreach (MethodInfo method in webBrowserObjectType.GetMethods())
                {
                    if (method.IsPublic)
                    {
                        StringBuilder pipes = new StringBuilder();
                        for (int i = 0; i < method.GetParameters().Length; i++)
                        {
                            pipes.Append(@".*\|");
                        }

                        this.Add(@"\|" + method.Name + @"\|" + pipes.ToString());
                    }

                }

            }

            public bool ItemStartsWith(string text)
            {
                foreach (string key in this)
                {
                    if (Regex.IsMatch(text, key))
                        return true;
                }

                return false;
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            _invokeManager.Dispose();
        }

        #endregion
    }

    public class CommandResult
    {
        #region Constructors

        public CommandResult() { }

        public CommandResult(CommandResult result)
        {
            Cond = result.Cond;
            Command = result.Command;
            Children = result.Children;
            FileName = result.FileName;
            FullCommand = result.FullCommand;
            Ignored = result.Ignored;
            LineNumber = result.LineNumber;
            Message = result.Message;
            ModIgn = result.ModIgn;
            Parameters = result.Parameters;
            CompareDataResults = result.CompareDataResults;
            Success = result.Success;
        }

        #endregion
        #region class variables
        private bool _success;
        private string _message;
        private string _command;
        private List<ParameterEntry> _parameters = new List<ParameterEntry>();
        private List<string> _compareDataResults = new List<string>();
        private List<CommandResult> _children = new List<CommandResult>();
        private string _fullCommand;
        private string _fileName;
        private int _lineNumber;
        private bool _ignored;
        private bool _modIgn;
        private bool _cond;
        #endregion

        #region Public Methods

        public bool Cond
        {
            get { return _cond; }
            set { _cond = value; }
        }

        public bool ModIgn
        {
            get { return _modIgn; }
            set { _modIgn = value; }
        }

        public bool Ignored
        {
            get { return _ignored; }
            set { _ignored = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public string FullCommand
        {
            get { return _fullCommand; }
            set { _fullCommand = value; }
        }

        public List<ParameterEntry> Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public List<string> CompareDataResults
        {
            get { return _compareDataResults; }
            set { _compareDataResults = value; }
        }

        public List<CommandResult> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        #endregion

        public class ParameterEntry
        {
            public string OriginalParam { get; set; }
            public string ReplacedParam { get; set; }
        }
    }

}
