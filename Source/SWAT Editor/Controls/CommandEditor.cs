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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SWAT;
using SWAT.Utilities;
using SWAT.Fitnesse;
using SWAT_Editor.Controls;
using System.Text.RegularExpressions;


namespace SWAT_Editor.Controls
{

	/// <summary>
	/// Represents the commandEditor where the SWAT commands are input into the system.
	/// </summary>
    public partial class CommandEditor : UserControl
    {

        #region Class variables

        private ICommandEditorResultsDisplayer _resultDisplayer;
        internal OpenFileDialog _fileDialog = new OpenFileDialog();
        internal SaveFileDialog _saveFileDialog = new SaveFileDialog();
        internal SWAT.DynamicHelp.SwatHelp _help;
        private System.Windows.Forms.WebBrowser _webBrowser;
        private bool _textWasChangedOverall;
        private MainForm mainForm;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of the command editor control.
        /// </summary>
        public CommandEditor()
        {
            InitializeComponent();
            this.tabSet.ControlAdded += new ControlEventHandler(tabSet_ControlAdded);
            this.tabSet.ControlRemoved += new ControlEventHandler(tabSet_ControlRemoved);
            //On initialization there is no document opened.
        }

        #endregion

        #region Event Handlers

        public delegate void ItemSelected(TextEditor.TextEditor.SelectedModeValues selectedMode, int lineNumber);
        public delegate void TabCountChangedDelegate(int count);
        public event TabCountChangedDelegate TabCountChangedEvent;

        /// <summary>
        /// Determines the behavior right before a tab is removed from the tab control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tabSet_ControlRemoved(object sender, ControlEventArgs e)
        {
            TabCountChangedEvent(this.tabSet.TabPages.Count - 1);
        }

        /// <summary>
        /// Determines the behavior right before a tab is added to the tab control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tabSet_ControlAdded(object sender, ControlEventArgs e)
        {
            TabCountChangedEvent(this.tabSet.TabPages.Count);
        }

        /// <summary>
        /// Handles the close button and the close tab context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabClose_Click(object sender, EventArgs e)
        {
            this.closeCurrentDocument(sender, e);
        }

        /// <summary>
        /// Handles the closing of the current document considering if changes have been made to the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void closeCurrentDocument(object sender, EventArgs e)
        {
            if (this.CurrentEditorPage.getEditor().TextEditorIsChanged) //check to see if changes have been made
            {
                String currentTab = this.tabSet.TabPages[this.tabSet.SelectedIndex].Text.TrimEnd();

                switch (MessageBox.Show("Would you like to save your changes to " + currentTab + " before closing?", "SWAT Editor", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes: SaveScriptEditor();
                                           removeTab(sender, e);
                                           if (this.TabSet.TabCount > 1)
                                           {
                                               this.tabSet.SelectTab(this.tabSet.TabCount - 1);
                                           }
                                           break;
                    case DialogResult.No: removeTab(sender, e);
                                          if (this.TabSet.TabCount > 1)
                                          {
                                               this.tabSet.SelectTab(this.tabSet.TabCount - 1);
                                          }
                                              break;
                    case DialogResult.Cancel: break;
                }
            }
            else
            {
                removeTab(sender, e);
                if (this.TabSet.TabCount > 1)
                {
                    this.tabSet.SelectTab(this.tabSet.TabCount - 1);
                }
            }
        }

        /// <summary>
        /// Removed the tab from the tabControl and displays the open new file label if there are no documents opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeTab(object sender, EventArgs e)
        {
            if (!this.CurrentEditorPage.TempFilePath.Equals("")) { File.Delete(this.CurrentEditorPage.TempFilePath); }

            this.tabSet.TabPages.Remove(this.tabSet.SelectedTab);

            if (this.tabSet.TabCount == 0)
            {
                lblOpenNewFile.Visible = true;
            }
        }

        /// <summary>
        /// When we move thru the tab documents, we want to be able to display the results 
        /// corresponding to the current document being displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mainForm.DDEditor.Active)
            {
                //If there is current document, try to refresh its result values.
                if (this.CurrentEditorPage != null)
                {
                    this.CurrentEditorPage.refreshResultList();
                }
                else
                {
                    //Otherwise we want to clear the result panel.
                    ResultDisplayer.Clear();
                }
            }
        }

		  private void tabSet_KeyUp(object sender, KeyEventArgs e)
		  {
			 if (e.Control == true && e.KeyValue == (int)Keys.Tab)
			 {
				 int indexToSelect = this.tabSet.SelectedIndex + 1;
				 if (indexToSelect >= this.tabSet.TabPages.Count)
				 {
					 indexToSelect = 0;
				 }
				 this.tabSet.SelectTab(indexToSelect);
			 }
		  }

        #endregion

        #region Add Script Editor

        /// <summary>
        /// Adds a new document for a given fileName.
        /// </summary>
        /// <param name="fileName"></param>
        public void AddNewScriptEditor(string fileName)
        {            
            this.tabSet.TabPages.Add(new CommandEditorPage(fileName, this));
            this.tabSet.SelectTab(this.tabSet.TabCount - 1);

            if (tabSet.TabCount != 0)
            {
                lblOpenNewFile.Visible = false;
                this.tabSet.SelectedTab = this.tabSet.TabPages[this.tabSet.TabCount - 1];
            }

            this.CurrentEditorPage.getEditor().TextEditorIsChanged = false;
        }

        /// <summary>
        /// Adds a new document from scratch.
        /// </summary>
        /// <param name="mainWindow"></param>
        public void AddNewScriptEditor(MainForm mainWindow)
        {
            mainForm = mainWindow;
            AddNewScriptEditor("");
        }

        /// <summary>
        /// Adds a new document from an existing file opened from memory.
        /// </summary>
        /// <param name="mainWindow"></param>
        public void AddExistingScriptEditor(MainForm mainWindow)
        {
            mainForm = mainWindow;
            _fileDialog.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            DialogResult result = _fileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                AddNewScriptEditor(_fileDialog.FileName);
            }

            if (tabSet.TabCount != 0)
            {
                lblOpenNewFile.Visible = false;
            }
        }

        //Adds a new document from the recent document's list.
        public void AddRecentScriptEditor(MainForm mainWindow, string filePath)
        {
            try
            {
                mainForm = mainWindow;
                AddNewScriptEditor(filePath);
            }
            catch { }
        }

        #endregion

        #region Saving related

        /// <summary>
        /// Checks changes before exiting and prompts the user so save current changes, if any.
        /// </summary>
        public void CheckChangesToExit()
        {
            if (tabSet.TabPages.Count >= 1)
            {
                int tabCount = 0;

                do
                {
                    this.tabSet.SelectTab(tabCount);
                    TextWasChangedOverall = this.CurrentEditorPage.getEditor().TextEditorIsChanged;
                    tabCount++;
                } while (this.CurrentEditorPage.getEditor().TextEditorIsChanged == false && tabCount < tabSet.TabPages.Count);


                if (TextWasChangedOverall == true)
                {
                    DialogResult result = MessageBox.Show("Would you like to save your changes before exiting?",
                                                                      "SWAT Editor", MessageBoxButtons.YesNoCancel);
                    switch (result)
                    {
                        case DialogResult.Yes: CheckToSaveScript();
                            break;
                        case DialogResult.No: for (int i = 0; i < tabSet.TabPages.Count; i++)
                            {
                                CommandEditorPage page = (CommandEditorPage)this.tabSet.TabPages[i];
                                if (!page.TempFilePath.Equals("")) { File.Delete(page.TempFilePath); }
                            }
                            Application.Exit();
                            break;
                        case DialogResult.Cancel: mainForm.AllowClose = true;
                            break;
                    }
                }
                else { Application.Exit(); }
            }
        }

        /// <summary>
        /// Saves the changes to the documents depending on if there are changes in the files.
        /// </summary>
        public void CheckToSaveScript()
        {
            if (TextWasChangedOverall)//in if statement, check if tab has been modified
            {
                SaveDialog saveDialog = new SaveDialog();

                if (tabSet.TabPages.Count == 1)
                {
                    SaveScriptEditor();
                }

                else if (tabSet.TabPages.Count > 1)
                {
                    saveDialog.ShowDialog();

                    switch (saveDialog.dialogResult)
                    {
                        case "Save": saveDialog.Hide();
                            SaveScriptEditor();
                            break;
                        case "Save All": saveDialog.Hide();
                            SaveAllTabs();
                            Application.Exit();
                            break;
                        case "Cancel": saveDialog.Hide();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Saves file to given file path.
        /// </summary>
        public void SaveAs()
        {
            CommandEditorPage page = (CommandEditorPage)this.tabSet.SelectedTab;

            _saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            DialogResult result = _saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                page.FilePath = _saveFileDialog.FileName;
                page.Save();
                this.CurrentEditorPage.getEditor().TextEditorIsChanged = false;
            }

        }

        /// <summary>
        /// Saves to a file the current selected document.
        /// </summary>
        public void SaveScriptEditor()
        {
            CommandEditorPage page = (CommandEditorPage)this.tabSet.SelectedTab;
            if (!string.IsNullOrEmpty(page.FilePath))
            {
                page.Save();
                this.CurrentEditorPage.getEditor().TextEditorIsChanged = false;
            }
            else
            {
                _saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                DialogResult result = _saveFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    page.FilePath = _saveFileDialog.FileName;
                    page.Save();
                    this.CurrentEditorPage.getEditor().TextEditorIsChanged = false;
                }
            }
        }

        /// <summary>
        /// Saves all opened tabs documents. (Batch save)
        /// </summary>
        public void SaveAllTabs()
        {
            int currentTab = this.tabSet.TabPages.IndexOf(this.tabSet.SelectedTab);

            for (int i = 0; i < tabSet.TabPages.Count; i++)
            {
                this.tabSet.SelectTab(i);

                if (this.CurrentEditorPage.getEditor().TextEditorIsChanged)
                {
                    SaveScriptEditor();
                }
            }

            this.tabSet.SelectTab(currentTab);
        }

        /// <summary>
        /// Saves all tab documents to a temporary folder.
        /// </summary>
        public void SaveAllTabsTemp()
        {
            string directory = SWAT_Editor.Properties.Settings.Default.AutosaveDirectory;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            for (int i = 0; i < tabSet.TabPages.Count; i++)
            {
                CommandEditorPage page = (CommandEditorPage)this.tabSet.TabPages[i];

                if (page.getEditor().TextEditorTempIsSaved && page.getEditor().TextEditorIsChanged)
                {
                    page.SaveTempFile(page.TempFilePath);
                }
                else if (!page.getEditor().TextEditorTempIsSaved && page.getEditor().TextEditorIsChanged)
                {

                    string tabName = page.Text.Replace(".txt", "");
                    page.TempFilePath = directory + "\\SWATAutosave" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Date.ToLongDateString() + tabName + "'" + (i + 1) + ".txt";
                    page.SaveTempFile(page.TempFilePath);
                    page.getEditor().TextEditorTempIsSaved = true;
                }

            }
        }

        /// <summary>
        /// Checks and retrieves the temporary files from the temporary folder, depending on the user's desicion.
        /// </summary>
        /// <param name="directoryFiles">File names located in the temporary directory.</param>
        public void CheckForTemporaryFiles(MainForm mainWindow, string[] directoryFiles)
        {
            switch (MessageBox.Show("You have unsaved changes stored in temporary files. Would you like to retrieve them?\n\t\t(Clicking 'No' will delete them permanently).", "SWAT Editor", MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes: for (int i = 0; i < directoryFiles.Length; i++)
                    {
                        if (directoryFiles[i].Contains("SWATAutosave"))
                        {
                            AddRecentScriptEditor(mainWindow, directoryFiles[i]);
                            File.Delete(directoryFiles[i]);
                        }
                    }
                    break;
                case DialogResult.No: for (int i = 0; i < directoryFiles.Length; i++)
                    {
                        if (directoryFiles[i].Contains("SWATAutosave"))
                            File.Delete(directoryFiles[i]);
                    }
                    break;
                case DialogResult.Cancel: break;

            }
        }

        #endregion

        #region Editor Events

        //Undo Method (al)
        public void Undo()
        {
            if (CurrentEditorPage != null)
                CurrentEditorPage.Undo();
        }

        //Redo Method (al)
        public void Redo()
        {
            if (CurrentEditorPage != null)
                CurrentEditorPage.Redo();
        }

        //Cut Method (al)
        public void Cut()
        {
            if (CurrentEditorPage != null)
                CurrentEditorPage.Cut();
        }

        //Paste Method (al)
        public void Paste()
        {
            if (CurrentEditorPage != null)
            {
                CurrentEditorPage.Paste();
                this.CurrentEditorPage.getEditor().TextEditorIsChanged = true;
            }

        }

        //Copy Method (al)
        public void Copy()
        {
            if (CurrentEditorPage != null)
                CurrentEditorPage.Copy();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the main form containing the editor.
        /// </summary>
        public MainForm GetMainForm
        {
            get { return this.mainForm; }
        }

        /// <summary>
        /// Gets or Sets the current CommandEditor result displayer.
        /// </summary>
        public ICommandEditorResultsDisplayer ResultDisplayer
        {
            get { return _resultDisplayer; }
            set
            {
                _resultDisplayer = value;

                if (_resultDisplayer != null)
                    _resultDisplayer.ItemSelect += new ItemSelected(_resultDisplayer_ItemSelect);
            }
        }

        /// <summary>
        /// Gets or Sets a value indicated if the text has been changed at all.
        /// </summary>
        public bool TextWasChangedOverall
        {
            get { return _textWasChangedOverall; }
            set { _textWasChangedOverall = value; }
        }

        /// <summary>
        /// ReadOnly property: Gets a value indicating if intellisense is being used.
        /// </summary>
        public bool UseIntellisense
        {
            get { return this.useIntellisense; }
        }

        /// <summary>
        /// Gets or Sets the dynamicHelp.
        /// </summary>
        public SWAT.DynamicHelp.SwatHelp Help
        {
            get { return _help; }
            set { _help = value; }
        }

        /// <summary>
        /// ReadOnly property: Gets the index of the current tab selected.
        /// </summary>
        public int CurrentTabIndex
        {
            get { return this.tabSet.TabPages.IndexOf(this.tabSet.SelectedTab); }
        }

        /// <summary>
        /// ReadOnly property: Gets the tabControl holding all document tabs.
        /// </summary>
        public TabControlExtension TabSet
        {
            get { return this.tabSet; }
        }

        /// <summary>
        /// Gets or Sets the current webBrowser control.
        /// </summary>
        public System.Windows.Forms.WebBrowser WebBrowser
        {
            get { return _webBrowser; }
            set { _webBrowser = value; }
        }

        /// <summary>
        /// ReadOnly property: Gets the current editor page being selected.
        /// </summary>
        public CommandEditorPage CurrentEditorPage
        {
            get 
            {
                CommandEditorPage cmp = null;
                this.Invoke(new MethodInvoker(delegate() {cmp = (CommandEditorPage)this.tabSet.SelectedTab; }));
                return cmp;
            }
        }

        //CanCut property (al)
        public bool CanCut
        {
            get
            {
                if (CurrentEditorPage == null)
                    return false;
                return CurrentEditorPage.CanCut();
            }
        }

        //CanUndo property (al)
        public bool CanUndo
        {
            get
            {
                if (CurrentEditorPage == null)
                    return false;

                return CurrentEditorPage.CanUndo();
            }
        }

        //CanRedo property (al)
        public bool CanRedo
        {
            get
            {
                if (CurrentEditorPage == null)
                    return false;

                return CurrentEditorPage.CanRedo();
            }
        }

        //CanPaste property (al)
        public bool CanPaste
        {
            get
            {
                //enables the "paste" only if clipboard contains string elements
                if (CurrentEditorPage != null && Clipboard.ContainsData((DataFormats.Text).ToString()))
                    return true;

                return false;
            }
        }

        //CanFind property (al)
        public bool CanFind
        {
            get
            {
                if (CurrentEditorPage == null)
                    return false;

                return true;
            }
        }

        //Determines if a save call can be issued.
        public bool CanSave
        {
            get
            {
                return (this.tabSet.TabPages.Count > 0);
            }
        }

        //Determines if a save all call can be issued.
        public bool CanSaveAll
        {
            get
            {
                return (this.CanSave && this.tabSet.TabPages.Count > 1);
            }
        }

        //Determines if a stop call can be issued.
        public bool CanStop
        {
            get { return (CurrentEditorPage != null && CurrentEditorPage.isTestRunning); }
        }

        //Determines if a start call can be issued.
        public bool CanStart
        {
            get { return (CurrentEditorPage != null && !CurrentEditorPage.isTestRunning); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reverse the use of the intellisese and updates all open tabs
        /// </summary>
        public void reverseIntellisense()
        {
            if (useIntellisense == true)
                useIntellisense = false;
            else
                useIntellisense = true;

            for (int i = 0; i < this.tabSet.TabCount; i++)
            {
                ((CommandEditorPage)this.tabSet.TabPages[i]).setIntellisense(useIntellisense);
            }
        }

        /// <summary>
        /// Runs the test for the document of the tab selected.
        /// </summary>
        /// <param name="browserType"></param>
        public void RunTestOnCurrentEditor(BrowserType browserType)
        {
            CommandEditorPage page = (CommandEditorPage)this.tabSet.SelectedTab;
            page.Run(browserType);
            page.RunCompleteEvent += new CommandEditorPage.RunCompleteDelegate(page_RunCompleteEvent);
        }

        public delegate void RunCompleteDelegate();
        public event RunCompleteDelegate RunCompleteEvent;

        /// <summary>
        /// Runs the complete event that indicates the termination of a test.
        /// </summary>
        void page_RunCompleteEvent()
        {
            RunCompleteEvent();
        }

        /// <summary>
        /// Stops the current test process.
        /// </summary>
        public void StopCurrentTest()
        {
            CurrentEditorPage.Stop();
        }

        /// <summary>
        /// Resumes a previously stopped test process.
        /// </summary>
        public void ResumeCurrentTest()
        {
            CurrentEditorPage.ResumeCurrentThread();
        }

        /// <summary>
        /// Steps forward to the next breakpoint or the end of the test.
        /// </summary>
        public void StepForward()
        {
            CurrentEditorPage.StepFowardCurrentThread();
        }

        /// <summary>
        /// Appends the specified string to the current script, in a new line.
        /// </summary>
        /// <param name="stringToAppend"></param>
        /// <param name="newLine"></param>
        public void AppendToCurrentScript(string stringToAppend, bool newLine)
        {

            if (newLine)
                CurrentEditorPage.EditorText += System.Environment.NewLine + stringToAppend;
            else
                CurrentEditorPage.EditorText += stringToAppend;
        }

        /// <summary>
        /// Appends the specified string to the current script.
        /// </summary>
        /// <param name="stringToAppend"></param>
        public void AppendToCurrentScript(string stringToAppend)
        {
            AppendToCurrentScript(stringToAppend, true);
        }

        /// <summary>
        /// Gets the current browser type in which this test is being run.
        /// </summary>
        /// <returns></returns>
        public BrowserType getCurrentPageBrowserType()
        {
            return CurrentEditorPage._BrowserType;
        }

        #endregion

        #region Misc methods

        /// <summary>
        /// Controls the selection behavior on the text editor, when a selection has been made in the results pane.
        /// </summary>
        /// <param name="selectedMode"></param>
        /// <param name="lineNumber"></param>
        void _resultDisplayer_ItemSelect(TextEditor.TextEditor.SelectedModeValues selectedMode, int lineNumber)
        {
            if (!this.GetMainForm.DDEditor.Active)
                CurrentEditorPage.SelectLine(selectedMode, lineNumber);
            else
            {
                try
                {
                    System.Diagnostics.Process.Start(generateFilePath());
                }
                catch (Win32Exception e) { }
            }
        }

        private string generateFilePath()
        {
           return this.GetMainForm.DDEditor.ResultPath + ResultDisplayer.GetCommand().Substring(GetMainForm.DDEditor.TestFront.Length) + ".html";
        }

        #endregion

    }

	/// <summary>
	/// Represents a document tab.
	/// </summary>
	public class CommandEditorPage : TabPageExtension
	{

		#region class variables

		SWAT.Auto_Complete.SwatAuto_Complete si;
		ListBox _listBox = new ListBox();
		RichTextBox _toolTip = new RichTextBox();

		private string _fileName = string.Empty;
		private string _tempFileName = string.Empty;
		public System.Threading.Thread _currentTestThread;
		public BrowserType _currentTestBrowserType = BrowserType.InternetExplorer;
		private CommandEditor _parent;
		public List<BreakPoint> breakPoints = new List<BreakPoint>();
		SWAT_Editor.Controls.TextEditor.TextEditor _textEditor;
		static int _tabCount = 1;
		private bool _isTestRunning;
		private List<CommandResult> testResults;

		#endregion

		/// <summary>
		/// Creates a new instance of the CommandEditorPage.
		/// </summary>
		/// <param name="fileName">File name of the document being opened.</param>
		/// <param name="parent">Parent that will hold rhw new tab document.</param>
		public CommandEditorPage(string fileName, CommandEditor parent) : base()
		{
			//Initialize test result list.
			this.testResults = new List<CommandResult>();

			//Text editor initialization.
			_textEditor = new SWAT_Editor.Controls.TextEditor.TextEditor(this);
			_textEditor.Size = this.Size - new Size(30, 20);
			_textEditor.BorderStyle = BorderStyle.Fixed3D;
			_textEditor.Font = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)0));
			_textEditor.KeyUp += new KeyEventHandler(editor_KeyUp);
			_textEditor.MouseClick += new MouseEventHandler(editor_MouseClick);
			_textEditor.DisplayBreakpointPanel = false;
			if (parent.Help == null)
				parent.Help = new SWAT.DynamicHelp.SwatHelp(parent.WebBrowser);
            
            _textEditor.Document.WordWrap = false;
            _textEditor.Document.RightMargin = 90000000;
			
            si = new SWAT.Auto_Complete.SwatAuto_Complete(_textEditor.Document, _listBox, _toolTip);

			_parent = parent;
			_textEditor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			this.Controls.Add(_textEditor);
			this.Controls.Add(_listBox);
			this.Controls.Add(_toolTip);

			loadFile(fileName);

			//when loading a new tab, read the and set the current setting of the use of the intellisense
			setIntellisense(parent.UseIntellisense);
		}

		#region Public Methods

		#region Text Edit

		/// <summary>
		/// Comments the text of the selected lines
		/// </summary>
		public void CommentLines()
		{
			int startLineNo = _textEditor.Document.GetLineFromCharIndex(_textEditor.Document.SelectionStart);
			int endLineNo = _textEditor.Document.GetLineFromCharIndex(_textEditor.Document.SelectionStart
				 + _textEditor.Document.SelectionLength - 1);

			//text to modified separate from the text box to avoid flickering
			int selectionStart = _textEditor.Document.GetFirstCharIndexFromLine(startLineNo);
			int selectionLenght = (_textEditor.Document.GetFirstCharIndexFromLine(endLineNo + 1) - 1)
				 - (_textEditor.Document.GetFirstCharIndexFromLine(startLineNo));

			//if the last line is selected, change the selectionLengh
			if (selectionLenght < 0)
				selectionLenght = _textEditor.Document.TextLength - selectionStart;

			_textEditor.Document.Select(selectionStart, selectionLenght);
			string textToBeCommented = _textEditor.Document.SelectedText;
			int lineCounter = 0;

			//if someone selects an empty line, exit method
			if (textToBeCommented.Length == 0)
				return;

			//comment the first line if it is not empty
			if (!textToBeCommented[0].Equals('\n'))
			{
				textToBeCommented = textToBeCommented.Insert(0, "#");
				lineCounter++;
			}

			//comment subsequent lines if they're not empty
			for (int i = 0; i < textToBeCommented.Length - 1; i++)
			{
				if (textToBeCommented[i].Equals('\n') && !(textToBeCommented[i + 1].Equals('\n')))
				{
					textToBeCommented = textToBeCommented.Insert(++i, "#");
					lineCounter++;
				}
			}

			//put modified text back to the textbox
			_textEditor.Document.SelectedText = textToBeCommented;

			//hightlight the selected lines
			_textEditor.Document.Select(selectionStart, selectionLenght + lineCounter);

		}

		/// <summary>
		/// Uncomments the text of the selected lines
		/// </summary>
		public void UnCommentLines()
		{
			int startLineNo = _textEditor.Document.GetLineFromCharIndex(_textEditor.Document.SelectionStart);
			int endLineNo = _textEditor.Document.GetLineFromCharIndex(_textEditor.Document.SelectionStart
				 + _textEditor.Document.SelectionLength - 1);

			//text to modified separate from the text box to avoid flickering
			int selectionStart = _textEditor.Document.GetFirstCharIndexFromLine(startLineNo);
			int selectionLenght = (_textEditor.Document.GetFirstCharIndexFromLine(endLineNo + 1) - 1)
				 - (_textEditor.Document.GetFirstCharIndexFromLine(startLineNo));

			//if the last line is selected, change the selectionLengh
			if (selectionLenght < 0)
				selectionLenght = _textEditor.Document.TextLength - selectionStart;

			_textEditor.Document.SelectionStart = selectionStart;
			_textEditor.Document.SelectionLength = selectionLenght;
			string textToBeCommented = _textEditor.Document.SelectedText;
			int lineCounter = 0;

			//if someone selects an empty line, exit method
			if (textToBeCommented.Length == 0)
				return;

			//uncomment the first line if it is commented
			if (textToBeCommented[0].Equals('#'))
			{
				textToBeCommented = textToBeCommented.Remove(0, 1);
				lineCounter++;
			}

			//uncomment subsequent lines if they're commented
			for (int i = 0; i < textToBeCommented.Length; i++)
			{
				if (textToBeCommented[i].Equals('\n') && (textToBeCommented[i + 1].Equals('#')))
				{
					textToBeCommented = textToBeCommented.Remove(++i, 1);
					lineCounter++;
				}
			}

			//put modified text back to the textbox
			_textEditor.Document.SelectedText = textToBeCommented;

			//hightlight the selected lines
			_textEditor.Document.SelectionStart = selectionStart;
			_textEditor.Document.SelectionLength = selectionLenght - lineCounter;

		}

		/// <summary>
		/// Determines if a cut call can be issued.
		/// </summary>
		/// <returns></returns>
		public bool CanCut()
		{
			return _textEditor.Document.SelectedText.Length > 0;
		}

		/// <summary>
		/// Performs the cut functionality.
		/// </summary>
		public void Cut()
		{
			_textEditor.Document.Cut();
		}

		/// <summary>
		/// Determines if an undo call can be issued.
		/// </summary>
		/// <returns></returns>
		public bool CanUndo()
		{
			return _textEditor.Document.CanUndo;
		}

		/// <summary>
		/// Performs the undo functionality.
		/// </summary>
		public void Undo()
		{
			_textEditor.Document.Undo();
		}

		/// <summary>
		/// Determines if a redo call can be issue.
		/// </summary>
		/// <returns></returns>
		public bool CanRedo()
		{
			return _textEditor.Document.CanRedo;
		}

		/// <summary>
		/// Performs the redo functionality.
		/// </summary>
		public void Redo()
		{
			if (CanRedo())
				_textEditor.Document.Redo();
		}

		/// <summary>
		/// Performs the paste funcitonality.
		/// </summary>
		public void Paste()
		{
			_textEditor.Document.Paste();

		}

		/// <summary>
		/// Performs the copy functionality.
		/// </summary>
		public void Copy()
		{
			_textEditor.Document.Copy();
		}

        public List<CommandResult> getTestResults()
        {
            return testResults;
        }

		#endregion

		#region Debug

		/// <summary>
		/// Enables the breakpoint for the given line.
		/// </summary>
		/// <param name="line">Line index where to insert the breakpoint at.</param>
		public void EnableCurrentBreakPoint(int line)
		{
			MainForm mainWindow = _parent.GetMainForm;
			mainWindow.ChangeResume(true);
			_textEditor.EnableSelectedLine(line);
		}

		/// <summary>
		/// Disables the breakpoint for the given line.
		/// </summary>
		/// <param name="line">Line index where breakpoint should be disabled.</param>
		/// <param name="breakPointType">Type of break point</param>
		public void DisableCurrentBreakPoint(int line, string breakPointType)
		{
			MainForm mainWindow = _parent.GetMainForm;
			mainWindow.ChangeResume(false);
			_textEditor.DisableSelectedLine(line, breakPointType);
		}

		/// <summary>
		/// Selects the given line number for the specified selection mode.
		/// </summary>
		/// <param name="selectedMode">Mode in which the line has been selected.</param>
		/// <param name="lineNumber">Line number to be selected.</param>
		public void SelectLine(TextEditor.TextEditor.SelectedModeValues selectedMode, int lineNumber)
		{
			_textEditor.SelectLine(selectedMode, lineNumber);
		}

		/// <summary>
		/// Enables all the breakpoints in the current document.
		/// </summary>
		public void EnableAllBreakPoints()
		{
			for (int i = 0; i < breakPoints.Count; i++)
				breakPoints[i].BPActive = true;
		}

		/// <summary>
		/// Disables all the breakpoints in the current document.
		/// </summary>
		public void DisableAllBreakPoints()
		{
			for (int i = 0; i < breakPoints.Count; i++)
				breakPoints[i].BPActive = false;
		}

		/// <summary>
		/// Cleans up all the current breakpoints in the current document.
		/// </summary>
		public void CleanUpBreakPoints()
		{
			this.getEditor().Invalidate(false);
			if (breakPoints.Count > 0)
			{
				if (breakPoints[breakPoints.Count - 1].TempBP)
					breakPoints.RemoveAt(breakPoints.Count - 1);
				for (int i = 0; i < breakPoints.Count; i++)
				{
					_textEditor.DisableSelectedLine(breakPoints[i].BPLineNumber);
				}
			}
			//_textEditor.CleanUp();
			this.getEditor().Invalidate();
		}

		#endregion

		#region Save methods

		/// <summary>
		/// Saves out the current document to a text file.
		/// </summary>
		public void Save()
		{
			if (!string.IsNullOrEmpty(_fileName))
			{
				using (StreamWriter sw = File.CreateText(_fileName))
				{
					sw.Write(this.EditorText);
					this.Text = Path.GetFileName(_fileName);
				}
			}
			if (!this.TempFilePath.Equals(""))
			{
				File.Delete(this.TempFilePath);
				this.TempFilePath = this.TempFilePath.Replace(Path.GetFileName(this.TempFilePath), Path.GetFileNameWithoutExtension(_fileName) + "'" + this.getCommandEditor().CurrentTabIndex + ".txt");
			}
		}

		/// <summary>
		/// Saves the current document to a temporary file.
		/// </summary>
		/// <param name="fileName"></param>
		public void SaveTempFile(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				using (StreamWriter sw = File.CreateText(fileName))
				{
					sw.Write(this.EditorText);
					this.getEditor().TextEditorTempIsSaved = true;
				}
			}
		}

		#endregion

		#region Execution Methods

		/// <summary>
		/// Runs the test given a browser type.
		/// </summary>
		/// <param name="browserType"></param>
		public void Run(BrowserType browserType)
		{
            MainForm mainWindow = _parent.GetMainForm;
		    this.testResults.Clear();

			if (mainWindow.getBreakPointStatus())
				EnableAllBreakPoints();
			else
				DisableAllBreakPoints();
			System.Threading.ParameterizedThreadStart start = new System.Threading.ParameterizedThreadStart(runScript);

			_currentTestThread = new System.Threading.Thread(start);

			_currentTestThread.SetApartmentState(System.Threading.ApartmentState.STA);
			_currentTestBrowserType = browserType;
			_currentTestThread.Start(this._textEditor.Document.Lines);
		}

		/// <summary>
		/// Stops the current test thread.
		/// </summary>
		public void Stop()
		{
			try
			{
				CleanUpBreakPoints();
				if (_currentTestThread.IsAlive)
				{
					_currentTestThread.Abort();
					endTest();
				}
			}
			catch
			{
				_currentTestThread.Resume();
				_currentTestThread.Abort();
				endTest();
			}

			this._textEditor.Refresh();
		}

		/// <summary>
		/// Resumes the current test thread.
		/// </summary>
		public void ResumeCurrentThread()
		{
			_currentTestThread.Resume();
		}

		/// <summary>
		/// Steps forward to the next breakpoint or to the end of the test.
		/// </summary>
		public void StepFowardCurrentThread()
		{
			BreakPoint node = new BreakPoint(true, 0, true);
			BreakPoints.Add(node);
			_currentTestThread.Resume();
		}

		#endregion

		#endregion

		#region Delegates

		public event RunCompleteDelegate RunCompleteEvent;

		public delegate void RunCompleteDelegate();

		public delegate void testStatusDelegate();

		public delegate void processResultDelegate(CommandResult result);

		#endregion

		#region Helper Methods

		/// <summary>
		/// Process the new result of the test by logging it in the result displayer.
		/// </summary>
		/// <param name="result"></param>
		private void processResult(CommandResult result)
		{
			//Make sure we record what the results of the current testing was, so it can be retrieved later.
            this.testResults.Add(result);

            if (!_parent.GetMainForm.DDEditor.Active)
                this.displayResult(result);
            else
            {
                if (didCommandFail(result))
                {
                    _parent.GetMainForm.DDEditor.TestPassed = false;
                    _parent.GetMainForm.DDEditor.FailureReason = result.LineNumber + ": " + result.Message;
                }
            }
		}

        /// <summary>
        /// Checks if the command failed
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool didCommandFail(CommandResult result)
        {
            return (!result.Success && !result.Cond) && !result.Ignored && _parent.GetMainForm.DDEditor.FailureReason == null;
        }

		/// <summary>
		/// Displays the latest result of the test by logging it in the result displayer.
		/// </summary>
		/// <param name="result"></param>
		public void displayResult(CommandResult result)
		{
			if (result != null)
			{
				_parent.ResultDisplayer.LogResult(result);
			}
		}

		/// <summary>
		/// Refreshes the latest results from the current test by displaying each result line.
		/// </summary>
		public void refreshResultList()
		{
			//First, make sure we are cleaning the list before we refresh.
			_parent.ResultDisplayer.Clear();

			//Now we can log each command from scratch.
            if (!_parent.GetMainForm.DDEditor.Active)
            {
                foreach (CommandResult eachResult in this.testResults)
                {
                    this.displayResult(eachResult);
                }
            }
		}

		/// <summary>
		/// Marks the start of a test.
		/// </summary>
		private void startTest()
		{
            if (!_parent.GetMainForm.DDEditor.Active)
			    _parent.ResultDisplayer.Clear();
			this._textEditor.Enabled = false;
			this.isTestRunning = true;
		}

		/// <summary>
		/// Marks the end of the test.
		/// </summary>
		private void endTest()
		{
			this._textEditor.Enabled = true;
			this.isTestRunning = false;
			RunCompleteEvent();
		}

		/// <summary>
		/// Runs the specified script lines.
		/// </summary>
		/// <param name="lines"></param>
		private void runScript(object lines)
		{
			string[] editorLines = (string[])lines;

			BrowserType browserType = _currentTestBrowserType;

            // CloseBrowsersBeforeTestStart depends on the following 'using' command
			using (CommandExtractor extractor = new CommandExtractor(browserType))
			{
				extractor.CommandProcessed += new CommandExtractor.CommandProcessedHandler(extractor_CommandProcessed);
				this.Invoke(new testStatusDelegate(startTest));
                TestManager.ResetForNewTest();
                if (_parent.GetMainForm.DDEditor.Active)
                     testResults = extractor.ProcessWikiCommands(this.FilePath, editorLines, _currentTestThread, this);
                else
                    extractor.ProcessWikiCommands(editorLines, breakPoints, _currentTestThread, this);
				this.Invoke(new testStatusDelegate(endTest));
			}
		}

		/// <summary>
		/// Handles the CommandProcessed event of the extractor command.
		/// </summary>
		/// <param name="result"></param>
		void extractor_CommandProcessed(CommandResult result)
		{
            if (result.Command != null)
			    this.Invoke(new processResultDelegate(processResult), result);
		}

		/// <summary>
		/// Loads the file located at the given path.
		/// </summary>
		/// <param name="fileName"></param>
		private void loadFile(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
			{
				_fileName = fileName;

				if (fileName.Contains("SWATAutosave"))
					this.Text = this.Text = "Untitled " + _tabCount;
				else
					this.Text = Path.GetFileName(fileName);


				using (StreamReader sr = File.OpenText(fileName))
				{
					EditorText = sr.ReadToEnd();
				}
			}
			else
			{
				this.Text = "Untitled " + _tabCount;
			}
			_tabCount++;
		}

		/// <summary>
		/// Handles the key uo event of the editor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void editor_KeyUp(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = false;
			e.Handled = false;

			if (e.KeyData == Keys.Enter)
			{
				_parent.Help.displayHelp();
			}

			String currentLine = ControlsUtils.getCurrentLine(_textEditor.Document);
			StringTokenizer stk = new StringTokenizer(currentLine, "|");

			try
			{
				if (currentLine.Length == 0)
				{
					_parent.Help.displayHelp();
				}
				else if (StringUtil.occurrences(currentLine, "|") >= 2 && stk.CountTokens() >= 1)
				{
					_parent.Help.displayHelpForLine(currentLine);
				}
			}
			catch
			{
				//thrown when currentLine is null and occurrences is called, nothing to do.
			}
		}

		/// <summary>
		/// Updates help for the current line the user clicked on.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void editor_MouseClick(object sender, MouseEventArgs e)
		{
			String currentLine = ControlsUtils.getCurrentLine(_textEditor.Document);
			StringTokenizer stk = new StringTokenizer(currentLine, "|");

			try
			{
				if (StringUtil.occurrences(currentLine, "|") >= 2 && stk.CountTokens() >= 1)
				{
					_parent.Help.displayHelpForLine(currentLine);
				}
				else
				{
					_parent.Help.displayHelp();
				}

			}
			catch
			//catch (ArgumentException ex) //UNUSED Variable
			{
				//thrown when currentLine is null and occurrences is called
				_parent.Help.displayHelp();
			}

		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the Command Editor holding the text editor.
		/// </summary>
		/// <returns></returns>
		public CommandEditor getCommandEditor()
		{
			return _parent;
		}

		/// <summary>
		/// Turns On/Off the use of intellisense.
		/// </summary>
		/// <param name="useInt"></param>
		public void setIntellisense(bool useInt)
		{
			si.setIntellisense(useInt);
		}

		/// <summary>
		/// Gets the current text Editor.
		/// </summary>
		/// <returns></returns>
		public SWAT_Editor.Controls.TextEditor.TextEditor getEditor()
		{
			return _textEditor;
		}

		/// <summary>
		/// Gets or Sets a value indicating if the current test is running.
		/// </summary>
		public bool isTestRunning
		{
			get { return _isTestRunning; }
			set { _isTestRunning = value; }
		}

		/// <summary>
		/// Gets or Sets the current breakpoints for the document.
		/// </summary>
		public List<BreakPoint> BreakPoints
		{
			get { return breakPoints; }
			set { breakPoints = value; }
		}

		/// <summary>
		/// Gets or Sets the text of the current document.
		/// </summary>
		public string EditorText
		{
			get { return _textEditor.Document.Text; }
			set { _textEditor.Document.Text = value; }
		}

		/// <summary>
		/// Gets or Sets the current file path to where the document is being saved to.
		/// </summary>
		public string FilePath
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		/// <summary>
		/// Gets or sets the current temporary directoy.
		/// </summary>
		public string TempFilePath
		{
			get { return _tempFileName; }
			set { _tempFileName = value; }
		}

		/// <summary>
		/// ReadOnly property: Gets the browser type in which the current test is being run.
		/// </summary>
		public BrowserType _BrowserType
		{
			get
			{
				if (_textEditor.Document.Text.Contains("!|InternetExplorerSWATFixture|"))
				{
					return BrowserType.InternetExplorer;
				}
                else if (_textEditor.Document.Text.Contains("!|FireFoxSWATFixture|"))
                {
                    return BrowserType.FireFox;
                }
                else if (_textEditor.Document.Text.Contains("!|ChromeSWATFixture|"))
                {
                    return BrowserType.Chrome;
                }
                else if (_textEditor.Document.Text.Contains("!|SafariSWATFixture|"))
                {
                    return BrowserType.Safari;
                }
                else
                {
                    string menuBrowser = SWAT_Editor.Properties.Settings.Default.TestBrowserType;

                    if (menuBrowser.Equals("ie"))
                    {
                        return BrowserType.InternetExplorer;
                    }
                    else if (menuBrowser.Equals("ff"))
                    {
                        return BrowserType.FireFox;
                    }
                    else if (menuBrowser.Equals("chrome"))
                    {
                        return BrowserType.Chrome;
                    }
                    else if (menuBrowser.Equals("safari"))
                    {
                        return BrowserType.Safari;
                    }
                }

                return BrowserType.InternetExplorer;
                
			}
		}

		#endregion
	}

	#region BreakPoint Class

	public class BreakPoint
	{
		private bool _bpActive;
		private int _bpLineNumber;
		private bool _tempBP;

		public BreakPoint()
		{
			BPActive = true;
			BPLineNumber = 0;
			TempBP = false;
		}

		public BreakPoint(bool value, int line)
		{
			BPActive = value;
			BPLineNumber = line;
			TempBP = false;
		}

		public BreakPoint(bool value, int line, bool temp)
		{
			BPActive = value;
			BPLineNumber = line;
			TempBP = temp;
		}

		public int BPLineNumber
		{
			get { return _bpLineNumber; }
			set { _bpLineNumber = value; }
		}

		public bool BPActive
		{
			get { return _bpActive; }
			set { _bpActive = value; }
		}

		public bool TempBP
		{
			get { return _tempBP; }
			set { _tempBP = value; }
		}

		public void PauseForBreakPoint(System.Threading.Thread activeThread)
		{
			activeThread.Suspend();
		}
	}

	#endregion

	#region Interfaces

	/// <summary>
	/// Determines the behavior for all result displayers.
	/// </summary>
	public interface ICommandEditorResultsDisplayer
	{
		void LogResult(CommandResult result);
        void UpdateResult(CommandResult result);
        string GetCommand();
		void Clear();
		event SWAT_Editor.Controls.CommandEditor.ItemSelected ItemSelect;
	}

	#endregion
}
