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


/*
 * Created by SharpDevelop.
 * User: jared
 * Date: 9/14/2007
 * Time: 10:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Collections;
using SWAT;


namespace SWAT_Editor.Recorder
{
	//	/// <summary>
	//	/// Description of ScriptBuilder.
	//	/// </summary>
	//	public class ScriptBuilder
	//	{
	//		IGenerator _generator;
	//
	//		public ScriptBuilder(BrowserType browserType, OnAfterGenerateCommandEventHandler commandReceiver)
	//		{
	//			_generator = new SWATWikiGenerator();
	//			_generator.OnAfterWriteCommand+=commandReceiver;
	//			_generator.Initialize(browserType);
	//		}
	//		
	//		public void processClick(HtmlElement element)
	//		{
	//			_generator.ClickElement(element);
	//		}
	//		
	//	}



	public class SWATWikiGenerator : IGenerator
	{
		//public event OnAfterGenerateCommandEventHandler OnAfterWriteCommand;
		//public event EventHandler OnFinishedWritingBatchOfCommands;
		BrowserType _browserType;
		bool _currentlyInSWATFixure = false;
		private bool _initialized = false;
		ArrayList commandList = new ArrayList();
		StringBuilder _sb = new StringBuilder();
		StringBuilder _generatedCode = new StringBuilder();
		String lastCommand = "";
		private Boolean _paused = false;

		public Boolean Paused
		{
			get { return _paused; }
			set { _paused = value; }
		}

		private void writeCommand(string command)
		{
			if (Paused) return;
			//if(this.OnAfterWriteCommand != null)
			//  OnAfterWriteCommand(command);
			//if (command.Equals(lastCommand)) return;
			System.Diagnostics.Debug.WriteLine(command);
			//_generatedCode.AppendLine(command);
			commandList.Add(command);
			lastCommand = command;
		}

		public void clearGeneratedCode()
		{
			_generatedCode = _generatedCode.Remove(0, _generatedCode.Length);
			commandList.Clear();
		}

		public void NavigateBrowser(string url)
		{
			startSWATFixtureIfNotInOne();
			writeCommand(string.Format("|NavigateBrowser|{0}|", url));
		}

		public void AttachBrowser(string browserName, int index)
		{
            startSWATFixtureIfNotInOne();
			writeCommand(string.Format("|AttachToWindow|{0}|{1}|", browserName, index));
		}

		public void CloseBrowser()
		{
			writeCommand("|CloseBrowser|");
		}

		public SWATWikiGenerator()
		{
			_currentlyInSWATFixure = false;
		}


		public void Initialize(BrowserType browserType, bool _openBrowser)
		{
			_browserType = browserType;

			StringBuilder sb = new StringBuilder();

			writeCommand("!|Import|");
			writeCommand("|SWAT|");
			writeCommand("|SWAT.Fitnesse|");

			writeCommand(""); //create a blank line

			switch (_browserType)
			{
				case BrowserType.InternetExplorer:
					writeCommand("!|InternetExplorerSWATFixture|");
					break;

				case BrowserType.Firefox:
					writeCommand("!|FireFoxSWATFixture|");
					break;
			}

			if (_openBrowser) openBrowser();
			_initialized = true;

			////OnFinishedWritingBatchOfCommands(null, new EventArgs());
		}

		//		void IGenerator.NavigateBrowser(string url)
		//		{
		//			throw new NotImplementedException();
		//		}

		private void openBrowser()
		{
			startSWATFixtureIfNotInOne();
			writeCommand("|OpenBrowser|");
		}

		private void startSWATFixtureIfNotInOne()
		{
			if (!_currentlyInSWATFixure)
			{
                
				writeCommand("");
				writeCommand("!|SWATFixture|");
				_currentlyInSWATFixure = true;
			}
		}

		/*public void SubmitElement(HtmlElement element)
		{
			 startSWATFixtureIfNotInOne();

			 _sb.Append("|StimulateElement|Expression|");

			 _sb.Append(getFindExpression(element));

			 _sb.Append("|onsubmit|");

			 if (!string.IsNullOrEmpty(element.tagName))
			 {
				  _sb.AppendFormat("{0}|", element.tagName);
			 }

			 writeCommand(getStringBuilderValueAndReset());
		}

		public void ClickElement(HtmlElement element)
		{
			 //StringBuilder sb = new StringBuilder();
			 startSWATFixtureIfNotInOne();

			 _sb.Append("|StimulateElement|Expression|");

			 _sb.Append(getFindExpression(element));

			 _sb.Append("|onclick|");

			 if (!string.IsNullOrEmpty(element.tagName))
			 {
				  _sb.AppendFormat("{0}|", element.tagName);
			 }

			 writeCommand(getStringBuilderValueAndReset());

			 //			if(!string.IsNullOrEmpty(sb.ToString()))
			 //				writeCommand(sb.ToString());
			 //OnFinishedWritingBatchOfCommands(null, new EventArgs());
		}*/

		public void StimulateElement(HtmlElement element, string eventName)
		{
			//StringBuilder sb = new StringBuilder();
			startSWATFixtureIfNotInOne();

			_sb.Append("|StimulateElement|Expression|");

			_sb.Append(getFindExpression(element));

			_sb.Append(string.Format("|{0}|", eventName));

			if (!string.IsNullOrEmpty(element.TagName))
			{
				_sb.AppendFormat("{0}|", element.TagName);
			}

			//double click occured, remove upto 2 previous click events
			if (eventName.Equals("ondblclick"))
			{
				if (((String)commandList[commandList.Count - 1]).Contains("|onclick|"))
				{
					commandList.RemoveAt(commandList.Count - 1);
				}
				if (((String)commandList[commandList.Count - 1]).Contains("|onclick|"))
				{
					commandList.RemoveAt(commandList.Count - 1);
				}
			}
            if (element.TagName == "TEXTAREA" || element.TagName == "HTML") //Clicks on a textarea or in an html element should not be recorded.
            {
                getStringBuilderValueAndReset();
            }
            else
            {
                writeCommand(getStringBuilderValueAndReset());
            }
          

			//			if(!string.IsNullOrEmpty(sb.ToString()))
			//				writeCommand(sb.ToString());
			//OnFinishedWritingBatchOfCommands(null, new EventArgs());
		}

		public void AssertElement(AssertionType type, String assertion, HtmlElement element)
		{
			startSWATFixtureIfNotInOne();
			//_assertion = "|AssertElementExists|expression|" + assertionBox.Text + "|" + _srcElement.tagName + "|";
			switch (type)
			{
				case AssertionType.ElementExists:
					_sb.Append("|AssertElementExists|Expression|");
					break;
				case AssertionType.ElementDoesNotExist:
					_sb.Append("|AssertElementDoesNotExist|Expression|");
					break;
			}
            //assertion = replaceSpecialCharacters(assertion);
			_sb.Append(assertion + "|" + element.TagName + "|");
			writeCommand(getStringBuilderValueAndReset());
		}

		private string getStringBuilderValueAndReset()
		{
			string returnValue = _sb.ToString();
            returnValue = replaceSpecialCharacters(returnValue);
			_sb.Remove(0, _sb.Length);
			return returnValue;
		}

        private string replaceSpecialCharacters(String value)
        {
            string[] specialCharacterList = {fromASCIItoString(45), fromASCIItoString(150), fromASCIItoString(151)}; 
            
            foreach(string s in specialCharacterList)
            {
                if(value.Contains(s))
                {
                    value = value.Replace(s, ".");
                }
            }

            return value;
        }

        private string fromASCIItoString(int ASCIIvalue)
        {
            string constructedString = System.Convert.ToChar(ASCIIvalue).ToString();
            return constructedString;
        }
        

		protected string getFindExpression(HtmlElement element)
		{

			StringBuilder findExpression = new StringBuilder();

			if (!string.IsNullOrEmpty(element.Id))
			{
				findExpression.AppendFormat("id:{0}", element.Id);
			}
			else if (!string.IsNullOrEmpty(element.Name))
			{
				findExpression.AppendFormat("name:{0}", element.Name);
			}
			else if (!string.IsNullOrEmpty(element.InnerHtml))
			{
				findExpression.AppendFormat("innerHtml:{0}", element.InnerHtml);
				if (!string.IsNullOrEmpty(element.Href))
				{
					findExpression.AppendFormat(";href:{0}", element.Href);
				}
			}
			else if (!string.IsNullOrEmpty(element.Href))
			{
				findExpression.AppendFormat("href:{0}", element.Href);
			}
			else if (!string.IsNullOrEmpty(element.Value))
			{
				findExpression.AppendFormat("value:{0}", element.Value);
			}

			if (!string.IsNullOrEmpty(element.OnClick))
			{
				findExpression.AppendFormat(";onclick:{0}", element.OnClick);
			}

			return findExpression.ToString();

			//String temp = "";
			//if (!string.IsNullOrEmpty(element.id))
			//{
			//    temp += string.Format("id:{0};", element.id);
			//}
			//if (!string.IsNullOrEmpty(element.name))
			//{
			//    temp += string.Format("name:{0};", element.name);
			//}
			//if (string.IsNullOrEmpty(temp))
			//{
			//    if (!string.IsNullOrEmpty(element.innerHtml))
			//    {
			//        temp += string.Format("innerHtml:{0};", element.innerHtml);
			//    }
			//    if (!string.IsNullOrEmpty(element.href))
			//    {
			//        temp += string.Format("href:{0};", element.href);
			//    }
			//    if (!string.IsNullOrEmpty(element.value))
			//    {
			//        temp += string.Format("value:{0};", element.value);
			//    }
			//}
			//return string.IsNullOrEmpty(temp) ? "" : temp.Substring(0,temp.Length-1);
		}

		public void SetElementProperty(HtmlElement element, string propertyName)
		{
			startSWATFixtureIfNotInOne();
            string newValue = string.Empty;
            if (element.TagName == "TEXTAREA")
            {
                newValue = element.InnerHtml;
            }
            else
            {
                newValue = element.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.GetProperty, null, element, new object[] { }).ToString();
            }
			_sb.AppendFormat("|SetElementAttribute|{0}|", !string.IsNullOrEmpty(element.Id) ? "id" : "name");
			_sb.AppendFormat("{0}", !string.IsNullOrEmpty(element.Id) ? element.Id : element.Name);
			_sb.AppendFormat("|{0}|{1}|{2}|", propertyName, newValue, element.TagName);
			writeCommand(getStringBuilderValueAndReset());
		}

		public bool Initialized
		{
			get
			{
				return _initialized;
			}
		}
		private string generateCode()
		{
			foreach (string a in commandList)
			{
				string command = a.Trim();
				_generatedCode.AppendLine(command);
			}
			return _generatedCode.ToString();
		}
		public string GeneratedCode
		{
			get { return generateCode(); }
		}


		public void ClickJSDialog(JScriptDialogButtonType buttonType)
		{

			startSWATFixtureIfNotInOne();
			if (buttonType == JScriptDialogButtonType.Ok)
				writeCommand("|ClickJSDialog|Ok|");
			else
				writeCommand("|ClickJSDialog|Cancel|");
		}


		#region DBBuilder Commands

		public void WriteBeginAssertStatement(StringBuilder asserts, string type, string server, string db, string usr, string pass, string sqlQ, bool includeConnectTime, int connectTime)
		{
			asserts.AppendFormat("!|SWATFixture|\r\n");

			WriteConnectToDatabaseStatement(asserts, type, server, db, usr, pass, includeConnectTime, connectTime);

			String tempSqlQ = sqlQ.Trim();
			String tempDb = db.Trim();

			asserts.AppendFormat("|SetDatabase|{0}|\r\n", tempDb);
			asserts.AppendFormat("|SetQuery|{0}|\r\n", tempSqlQ);
		}

		private void WriteConnectToDatabaseStatement(StringBuilder asserts, string type, string server, string db, string usr, string pass, bool includeConnectTime, int connectTime)
		{
			switch (type)
			{
				case "MSSQL":
					if (includeConnectTime)
						asserts.AppendFormat("|ConnectToMssql|{0}|{1}|{2}|{3}|\r\n", server, usr, pass, connectTime);
					else
						asserts.AppendFormat("|ConnectToMssql|{0}|{1}|{2}|\r\n", server, usr, pass);
					break;
				case "Oracle": asserts.AppendFormat("|ConnectToOracle|{0}|{h1}|{2}|\r\n", server, usr, pass); break;
			}
		}

		public void WriteBeginCompareData(StringBuilder asserts)
		{
			asserts.Append("|BeginCompareData|\r\n|");
		}

		public void WriteColumnNames(StringBuilder asserts, String columnName)
		{
		    asserts.Append(columnName + "|");
		}

		public void WriteDataTableView(StringBuilder asserts, String cellValue)
		{
			asserts.Append(cellValue + "|");
		}

		public void WriteDataCellView(StringBuilder asserts, String modifier, int rowIndex, string columnName, string cellValue)
		{
            if(modifier != null)
			    asserts.Append("|" + modifier + "AssertRecordValuesByColumnName|" + rowIndex + "|" + columnName + "|" + cellValue + "|" + "\r\n");
		    else
                asserts.Append("|AssertRecordValuesByColumnName|" + rowIndex + "|" + columnName + "|" + cellValue + "|" + "\r\n");
        }

		public void WriteColumnSepator(StringBuilder asserts)
		{
			asserts.Append("|");
		}

        // Removed boolean flag and moved "|EndCompareData|\r\n" to TableView section of CreateAssertStatements() -GT
        public void WriteEndStatement(StringBuilder asserts, int rowCount)
        {
            asserts.Append("|AssertRecordCount|" + rowCount + "|");
        }

		public void WriteBeginQueryStatement(StringBuilder asserts, string type, string server, string db, string usr, string pass, bool includeConnectTime, int connectTimeout)
		{
			asserts.Append("!|SWATFixture|\r\n");

			WriteConnectToDatabaseStatement(asserts, type, server, db, usr, pass, includeConnectTime, connectTimeout);

			String tempDb = db.Trim();

			asserts.AppendFormat("|SetDatabase|{0}|\r\n", tempDb);
		}

		public void WriteInsertQuery(StringBuilder asserts, string modifier, string tableName)
		{
			asserts.Append("|" + modifier + "SetQuery|INSERT INTO " + tableName + " (");
		}

		public void WriteDeleteQuery(StringBuilder asserts, string modifier, string tableName)
		{
			asserts.Append("|" + modifier + "SetQuery|DELETE FROM " + tableName + " (");
		}

		public void WriteVisibleColumnNames(StringBuilder asserts, string columnHeader)
		{
			asserts.Append(columnHeader + ",");
		}

		public void WriteEndInsertQueryStatement(StringBuilder asserts)
		{
			asserts.Append(")|\r\n");
		}

		public void WriteEndDeleteQueryStatement(StringBuilder asserts)
		{
			asserts.Append("|\r\n");
		}

		public void WriteImportStatement(StringBuilder sb)
		{
			sb.AppendFormat("!|Import|{0}|SWAT.Fitnesse|{0}{0}", Environment.NewLine);
		}

		public void WriteInternetExplorerSWATFixture(StringBuilder sb)
		{
			sb.AppendFormat("!|InternetExplorerSWATFixture|{0}{0}", Environment.NewLine);
		}

		public void WriteFireFoxSWATFixture(StringBuilder sb)
		{
			sb.AppendFormat("!|FireFoxSWATFixture|{0}{0}", Environment.NewLine);
		}

        public void WriteChromeSWATFixture(StringBuilder sb)
        {
            sb.AppendFormat("!|ChromeSWATFixture|{0}{0}", Environment.NewLine);
        }

        public void WriteSafariSWATFixture(StringBuilder sb)
        {
            sb.AppendFormat("!|SafariSWATFixture|{0}{0}", Environment.NewLine);
        }

		#endregion [DBBuilderReferences]
	}


	//public class SWATObjectGenerator : IGenerator
	//{
	//  private Test _test = new Test();

	//  public Test Test
	//  {
	//    get { return _test; }
	//    set { _test = value; }
	//  }

	//  #region IGenerator Members

	//  public void ClickElement(HtmlElement element)
	//  {
	//    Command newCommand = new Command();
	//    newCommand.Name = "StimulateElement";

	//    newCommand.Parameters.Add(new Parameter("Expression", 0));
	//    newCommand.Parameters.Add(new Parameter(getFindExpression(element), 1));
	//    newCommand.Parameters.Add(new Parameter("onclick", 2));
	//    newCommand.Parameters.Add(new Parameter(element.tagName, 3));

	//    Test.Commands.Add(newCommand);
	//  }

	//  public void SetElementProperty(HtmlElement element, string propertyName)
	//  {
	//    //throw new Exception("The method or operation is not implemented.");
	//  }

	//  public void NavigateBrowser(string URL)
	//  {
	//   // throw new Exception("The method or operation is not implemented.");
	//  }

	//  public event OnAfterGenerateCommandEventHandler OnAfterWriteCommand;

	//  public event EventHandler OnFinishedWritingBatchOfCommands;

	//  public void Initialize(BrowserType browserType)
	//  {
	//    //throw new Exception("The method or operation is not implemented.");
	//  }

	//  public bool Initialized
	//  {
	//    get { throw new Exception("The method or operation is not implemented."); }
	//  }

	//  protected string getFindExpression(HtmlElement element)
	//  {
	//    if (!string.IsNullOrEmpty(element.id))
	//    {
	//      return string.Format("id:{0};", element.id);
	//    }
	//    else if (!string.IsNullOrEmpty(element.name))
	//    {
	//      return string.Format("name:{0};", element.name);
	//    }
	//    else if (!string.IsNullOrEmpty(element.innerHtml))
	//    {
	//      return string.Format("innerHtml:{0};", element.innerHtml);
	//    }

	//    return "";
	//  }

	//  #endregion
	//}
}
