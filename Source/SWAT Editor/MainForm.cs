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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SHDocVw;
using System.Diagnostics;
using fitnesse.fitserver;
using System.IO;
using SWAT;
using System.Collections;
using SWAT_Editor.Controls.Recorder;
using System.Runtime.InteropServices;

using mshtml;
using System.Xml;
using System.Threading;

namespace SWAT_Editor
{

	public partial class MainForm : Form
	{

		#region Class Variables

		Stack SnapShot = new Stack();
		FindReplaceForm frForm;
		DBBuilderForm dbForm;
		private RecentFilesHandler _recentFiles = new RecentFilesHandler();
		ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
		private bool _allowClose;

        static BackgroundWorker bw = new BackgroundWorker();
		//Unused Variables
		//RecorderFloatingToolbar recFloatingToolbar;
		//ReportBugForm rbaForm;

		#endregion Class Variables


		#region Accessors

        public SWAT_Editor.Controls.DDEditor DDEditor
        {
            get { return ddEditor1; }
        }

		public bool AllowClose
		{
			get { return _allowClose; }
			set { _allowClose = value; }
		}

		#endregion Accesors


		#region Form methods

		public MainForm()
		{
			InitializeComponent();
			SetupGlueEvents();
            LoadCollapsePanelSettings();
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			switch (e.CloseReason)
			{
				case CloseReason.UserClosing: commandEditor1.CheckChangesToExit();
					e.Cancel = AllowClose;
					break;
				case CloseReason.WindowsShutDown: break;
				default: break;
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			myTimer.Stop();
		}

		private void MainForm_Activated(object sender, EventArgs e)
		{
			//Enable/Disable the Browser List's buttons. This code is commented out because this controls are currently not visible.
			// cBox_openBrowserWindows.Enabled = commandEditor1.CanSave;
			// btn_attachToWindow.Enabled = commandEditor1.CanSave;
		}

		#endregion Constructor


		#region Public Methods

		public void ChangeResume(bool value)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new MethodInvoker(delegate() { ChangeResume(value); }));
			}
			else
			{
				toolStripResumeButton.Enabled = value;
				resumeToolStripMenuItem.Enabled = value;
				stepFowardToolStripMenuItem.Enabled = value;
			}
		}

		public bool getBreakPointStatus()
		{
			return this.enableBreakPointsToolStripMenuItem.Checked;
		}

		#endregion


		#region Menu Bar Methods

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.CheckChangesToExit();
			Application.Exit();
		}

		#endregion


		#region Helper Methods

		private void SetupGlueEvents()
		{
			swatExplorer.commandEditorLoadLocalFile += new SWAT_Editor.Controls.Explorer.LoadLocalFileHandler(swatExplorer_LoadLocalFile);
            swatExplorer.dBBuilderLoadLocalFile += new SWAT_Editor.Controls.Explorer.LoadLocalFileHandler(dBBuilder_LoadLocalFile);
            
		}
        private void LoadCollapsePanelSettings()
        {
            this.topMainContainer.Panel2Collapsed = !Properties.Settings.Default.ShowExplorerChecked;
            this.mainContainer.Panel2Collapsed = !Properties.Settings.Default.ShowResultsChecked;
        }

		private void Record()
		{
			if (!ieRecorder1.Recording && commandEditor1.CanSave)
			{
				//recFloatingToolbar = new RecorderFloatingToolbar();
				//recFloatingToolbar.Show(this);
				BeginRecordingDialog dialog = new BeginRecordingDialog(ref ieRecorder1, useInternetExplorerToolStripMenuItem.Checked, this.notifyIcon);
				if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				{
					return;
				}
				useFirefoxToolStripMenuItem1.Checked = false;
				useInternetExplorerToolStripMenuItem.Checked = true;
			}
			else if (ieRecorder1.Recording && ieRecorder1.IsPaused())
			{
				ieRecorder1.Resume();
			}
			if (notifyIcon.Visible == false)
			{
				Hide();
				this.notifyIcon.Visible = true;
				this.notifyIcon.BalloonTipText = "SWAT Editor recording";
				notifyIcon.ShowBalloonTip(3);
			}

			commandEditor1.Enabled = false;
			toolStripPlayButton.Enabled = false;
			toolStripPauseButton.Enabled = true;
			toolStripRecordButton.Enabled = false;
			toolStripStopButton.Enabled = true;
			toolStripResumeButton.Enabled = false;
			toolStripAssertionButton.Enabled = true;
            newScriptToolStripMenuItem.Enabled = false;
			resumeToolStripMenuItem.Enabled = false;
			startRecToolStripMenuItem2.Enabled = false;
			stopRecToolStripMenuItem2.Enabled = true;

			if (this.notifyIcon.Visible == true)
			{
				/** needed for notifyicon
				 * */
				this.notifyIcon.BalloonTipText = "SWAT Editor recording";
				notifyIcon.ShowBalloonTip(3);
                assertToolStripNotifyIcon.Enabled = true;
                stopToolStripNotifyIcon.Enabled = true;
                UnpauseToolStripNotifyIcon.Enabled = false;
                pauseToolStripNotifyIcon.Enabled = true;
			}
		}

		private void Play()
		{

			if (commandEditor1.CanStart)
			{
				if (cBox_openBrowserWindows.Text != "")
				{
					int itr = 1;
					while ((commandEditor1.CurrentEditorPage.getEditor().GetLineText(itr) == "" ||//advance to the first non empty line
							 commandEditor1.CurrentEditorPage.getEditor().GetLineText(itr) == "\n") &&
							  itr < commandEditor1.CurrentEditorPage.getEditor().GetLineCount())
					{
						itr++;
					}

					if (!commandEditor1.CurrentEditorPage.getEditor().GetLineText(itr).Contains("|AttachToWindow|"))
						commandEditor1.CurrentEditorPage.getEditor().Document.Text = string.Format("|AttachToWindow|{0}|", cBox_openBrowserWindows.Text) +
																				  commandEditor1.CurrentEditorPage.getEditor().Document.Text;
				}
				commandEditor1.RunTestOnCurrentEditor(getBrowserType());
			}

			toolStripPlayButton.Enabled = false;
			toolStripPauseButton.Enabled = false;
			toolStripRecordButton.Enabled = false;
			toolStripStopButton.Enabled = true;
			toolStripResumeButton.Enabled = false;
			resumeToolStripMenuItem.Enabled = false;
            SWAT.Fitnesse.TableHandler.continueSuspend = true;

			if (this.notifyIcon.Visible == true)
			{
				/** needed for notify icon
				* */
                //playToolStripNotifyIcon.Enabled = false;
				pauseToolStripNotifyIcon.Enabled = false;
                //recordToolStripNotifyIcon.Enabled = false;
				stopToolStripNotifyIcon.Enabled = true;
				assertToolStripNotifyIcon.Enabled = false;
			}
		}

        private void PlayDDE()
        {
            if (ddEditor1.canStart())
            {
                setupDDE();                

                prepareTests();

                #region unused script parser code
                /*string curXML = getToTestCases(xml);
                try
                {
                    while (curXML != null && !curXML.ToLower().Contains("/testcases"))
                    {
                        File.WriteAllText(filePath, test.ReadToEnd());
                        test = ddEditor1.getTestFileContent();

                        curXML = moveToNextTest(xml, curXML);
                        testEnabled = Boolean.Parse(extractValue(curXML, "enabled") != null ? extractValue(curXML, "enabled") : "false");
                        
                        if (testEnabled)
                            testNames.Enqueue(extractValue(curXML, "tcid").Replace("/", "").Replace("\\", ""));

                        curXML = xml.ReadLine();
                        while (curXML != null && !curXML.ToLower().Contains("/testcase"))
                        {
                            if (curXML.ToLower().Contains("variable"))
                            {
                                replaceVariables(filePath, extractValue(curXML, "varName"), extractValue(curXML, "varValue"));
                            }
                            curXML = xml.ReadLine();
                        }

                        if (testEnabled)
                            commandEditor1.AddRecentScriptEditor(this, filePath);
                        curXML = moveToNextTest(xml, curXML);
                    }
                }
                catch (NullReferenceException ex)
                {
                    return;
                }*/
                #endregion

                selectFirstTest();

                startDDE();
            }
        }

        private void Stop()
		{
			if (ieRecorder1.Recording)
			{
				string result = ieRecorder1.Stop();
				commandEditor1.AppendToCurrentScript(result);
				commandEditor1.Enabled = true;
			}
			else if (commandEditor1.CanStop)
			{
				commandEditor1.StopCurrentTest();
			}
			toolStripPlayButton.Enabled = true;
			toolStripPauseButton.Enabled = false;
			toolStripRecordButton.Enabled = true;
			toolStripStopButton.Enabled = false;
			toolStripAssertionButton.Enabled = false;
			SWAT_Editor.Recorder.HTMLEvents.AssertElement = false;
			toolStripAssertionButton.Checked = false;
			toolStripResumeButton.Enabled = false;
            newScriptToolStripMenuItem.Enabled = true;
			resumeToolStripMenuItem.Enabled = false;
			startRecToolStripMenuItem2.Enabled = true;
			stopRecToolStripMenuItem2.Enabled = false;

			//notify icon
			if (assertToolStripNotifyIcon.Text.Equals("Stop Asserting"))
				assertToolStripNotifyIcon.Text = "Assert";

			if (this.notifyIcon.Visible == true)
			{
				/** needed for notify icon
				 * */
                //playToolStripNotifyIcon.Enabled = true;
				pauseToolStripNotifyIcon.Enabled = false;
                //recordToolStripNotifyIcon.Enabled = true;
				assertToolStripNotifyIcon.Enabled = false;
				stopToolStripNotifyIcon.Enabled = false;
				toolStripResumeButton.Enabled = false;
			}
		}

        private void StopDDE()
        {
            bw.CancelAsync();
            if (commandEditor1.CanStop)
            {
                commandEditor1.StopCurrentTest();
            }
        }

		private void Pause()
		{
			string result = ieRecorder1.Pause();
			commandEditor1.AppendToCurrentScript(result,false);
			commandEditor1.Enabled = true;
			toolStripPlayButton.Enabled = false;
			toolStripPauseButton.Enabled = false;
			toolStripRecordButton.Enabled = true;
			toolStripStopButton.Enabled = true;
			toolStripResumeButton.Enabled = false;
			resumeToolStripMenuItem.Enabled = false;

			if (this.notifyIcon.Visible == true)
			{
				/** needed for notify icon
				* */

                stopToolStripNotifyIcon.Enabled = true;
                UnpauseToolStripNotifyIcon.Enabled = true;
                pauseToolStripNotifyIcon.Enabled = false;
                toolStripResumeButton.Enabled = false;
			}
		}

		private void Resume()
		{
			commandEditor1.ResumeCurrentTest();
		}

		private void Step()
		{
			commandEditor1.StepForward();
		}

		void commandEditor1_TabCountChangedEvent(int count)
		{
			if (count == 0 && !ddEditor1.Visible)
			{
				toolStripPlayButton.Enabled = false;
				toolStripPauseButton.Enabled = false;
				toolStripRecordButton.Enabled = false;
				toolStripStopButton.Enabled = false;
				toolStripResumeButton.Enabled = false;
			}
			else if (!ddEditor1.Visible)
			{
				toolStripPlayButton.Enabled = true;
				toolStripPauseButton.Enabled = false;
				toolStripRecordButton.Enabled = true;
				toolStripStopButton.Enabled = false;
			}

            //if FindReplaceForm is open, close it
            if (frForm != null && !frForm.IsDisposed)
                frForm.Close();
		}

        void commandEditor1_RunCompleteEvent()
        {
            if (!ddEditor1.Visible)
            {
                toolStripPlayButton.Enabled = true;
                toolStripPauseButton.Enabled = false;
                toolStripRecordButton.Enabled = true;
                toolStripStopButton.Enabled = false;
                toolStripResumeButton.Enabled = false;
                resumeToolStripMenuItem.Enabled = false;
                dDEditorWizardSelect.Enabled = true;
            }
        }

		//public void LoadBrowserWindows()
		//{
		//    ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
		//    foreach (SHDocVw.InternetExplorer Browser in m_IEFoundBrowsers)
		//    {
		//        string windowTitle = GetProcessName(Browser.HWND);

		//        if(!string.IsNullOrEmpty(windowTitle))
		//            cmbWindowTitle.Items.Add(new WindowItem(windowTitle, Browser.HWND));
		//    }
		//}

		public string GetProcessName(int hwnd)
		{
			foreach (Process proc in Process.GetProcesses())
			{
				if (proc.MainWindowHandle == (IntPtr)hwnd)
					return proc.MainWindowTitle;
			}

			return null;
		}

		public class WindowItem
		{
			private string _windowTitle;
			private int _windowHwnd;

			public WindowItem(string windowTitle, int windowHandle)
			{
				WindowTitle = windowTitle;
				WindowHandle = windowHandle;
			}

			public int WindowHandle
			{
				get { return _windowHwnd; }
				set { _windowHwnd = value; }
			}

			public string WindowTitle
			{
				get { return _windowTitle; }
				set { _windowTitle = value; }
			}

			public override string ToString()
			{
				return WindowTitle;
			}

		}

		private BrowserType getBrowserType()
		{
			BrowserType browserType = commandEditor1.getCurrentPageBrowserType();

			if (SWAT_Editor.Properties.Settings.Default.OverrideTestBrowser == true)
			{
				//there is no browser selected by code. Follow the code selection
                if (useInternetExplorerToolStripMenuItem.Checked)
                    browserType = BrowserType.InternetExplorer;
                else if (useFirefoxToolStripMenuItem1.Checked)
                    browserType = BrowserType.FireFox;
                else if (useChromeToolStripMenuItem.Checked)
                    browserType = BrowserType.Chrome;
                else if (useSafariToolStripMenuItem.Checked)
                    browserType = BrowserType.Safari;
			}
			else
			{
				//Then select the appropiate value in the menu
				if (browserType == BrowserType.InternetExplorer)
				{
                    useInternetExplorerToolStripMenuItem.Checked = true;
                    useFirefoxToolStripMenuItem1.Checked = false;
                    useChromeToolStripMenuItem.Checked = false;
                    useSafariToolStripMenuItem.Checked = false;
                    SWAT_Editor.Properties.Settings.Default.TestBrowserType = "ie";
				}
				else if (browserType == BrowserType.FireFox)
				{
                    useInternetExplorerToolStripMenuItem.Checked = false;
                    useFirefoxToolStripMenuItem1.Checked = true;
                    useChromeToolStripMenuItem.Checked = false;
                    useSafariToolStripMenuItem.Checked = false;
                    SWAT_Editor.Properties.Settings.Default.TestBrowserType = "ff";
				}
                else if (browserType == BrowserType.Chrome)
                {
                    useInternetExplorerToolStripMenuItem.Checked = false;
                    useFirefoxToolStripMenuItem1.Checked = false;
                    useChromeToolStripMenuItem.Checked = true;
                    useSafariToolStripMenuItem.Checked = false;
                    SWAT_Editor.Properties.Settings.Default.TestBrowserType = "chrome";
                }
                else if (browserType == BrowserType.Safari)
                {
                    useInternetExplorerToolStripMenuItem.Checked = false;
                    useFirefoxToolStripMenuItem1.Checked = false;
                    useChromeToolStripMenuItem.Checked = false;
                    useSafariToolStripMenuItem.Checked = true;
                    SWAT_Editor.Properties.Settings.Default.TestBrowserType = "safari";
                }
			}

			return browserType;
		}

		void swatExplorer_LoadLocalFile(string filePath)
		{            
			commandEditor1.AddRecentScriptEditor(this, filePath);
            commandEditorToolStripButton_Click(this, new EventArgs());
		}

        void dBBuilder_LoadLocalFile(string filePath)
        {
            DialogResult response = MessageBox.Show("Would you like to open this file with the Data Access Editor?\n" + 
                "Otherwise it will open in the Text Editor", "Open SQL File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (response == DialogResult.Yes)
            {
                dbBuilderWizard1.loadLocalFile(filePath);
                dBBuilderWizardSelect_Click(this, new EventArgs());
            }
            else
                swatExplorer_LoadLocalFile(filePath);
        }

		#endregion


		#region Delegates

		public delegate void UpdateListDelegate(string command, string result, Color color);
		public delegate void TestRunCompete();

		#endregion


		#region Events
		private void Form1_Load(object sender, EventArgs e)
		{
			notifyIcon.Visible = false;
			commandEditor1.ResultDisplayer = resultCommandList;

			commandEditor1.RunCompleteEvent += new SWAT_Editor.Controls.CommandEditor.RunCompleteDelegate(commandEditor1_RunCompleteEvent);
			commandEditor1.TabCountChangedEvent += new SWAT_Editor.Controls.CommandEditor.TabCountChangedDelegate(commandEditor1_TabCountChangedEvent);

			myTimer.Interval = System.Convert.ToInt32(SWAT_Editor.Properties.Settings.Default.AutosaveFrequency) * 60000;

			if (SWAT_Editor.Properties.Settings.Default.AutosaveEnabled) { myTimer.Start(); }

			if (!Directory.Exists(SWAT_Editor.Properties.Settings.Default.AutosaveDirectory))
				Directory.CreateDirectory(SWAT_Editor.Properties.Settings.Default.AutosaveDirectory);

			string[] directoryFiles = Directory.GetFiles(SWAT_Editor.Properties.Settings.Default.AutosaveDirectory);
			bool autoSaveFilesExist = false;

			foreach (string file in directoryFiles)
			{
				if (file.Contains("SWATAutosave"))
				{
					autoSaveFilesExist = true;
					break;
				}
			}

			if (directoryFiles.Length > 0 && autoSaveFilesExist)
			{
				commandEditor1.CheckForTemporaryFiles(this, directoryFiles);
			}

			if (SWAT_Editor.Properties.Settings.Default.LoadBlankForm)
			{
				commandEditor1.AddNewScriptEditor(this);
				if (commandEditor1.CanStart)
				{
					toolStripPlayButton.Enabled = true;
					toolStripRecordButton.Enabled = true;
				}
			}

            if (!string.IsNullOrEmpty(SWAT.FitnesseSettings.FitnesseRootDirectory))
            {
                swatExplorer.Load(SWAT.FitnesseSettings.FitnesseRootDirectory);
            }
			else
			{
				swatExplorer.Load(SWAT_Editor.Properties.Settings.Default.explorerDirectory);
			}

			fillOpenBrowserWindows();
		}

		private void btnOpenBrowser_Click(object sender, EventArgs e)
		{
			if (!useInternetExplorerToolStripMenuItem.Checked)
			{
				System.Diagnostics.Process fx = System.Diagnostics.Process.Start(@"C:\Program Files\Mozilla Firefox\firefox", "about:config -jssh");
			}
			else
			{
				Process m_Proc = Process.Start("IExplore.exe", "about:Blank");
			}
		}

		private void openWindowsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenWindows openWindowForm = new OpenWindows();
			openWindowForm.ShowDialog(this);
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
            OptionsForm2 optionsForm = new OptionsForm2();
            optionsForm.FormClosed += new FormClosedEventHandler(optionsForm_FormClosed);
            if (!commandEditor1.UseIntellisense) SWAT_Editor.Properties.Settings.Default.UseAutoComplete = false;
            optionsForm.ShowDialog(this);

		}

		private void optionsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			//useCustomSnippets();
            if (SWAT_Editor.Properties.Settings.Default.UseAutoComplete && (!commandEditor1.UseIntellisense))
			{
				commandEditor1.reverseIntellisense();
			}
            else if (!SWAT_Editor.Properties.Settings.Default.UseAutoComplete && (commandEditor1.UseIntellisense))
			{
				commandEditor1.reverseIntellisense();
			}

            if (SWAT_Editor.Properties.Settings.Default.AutosaveEnabled && !myTimer.Enabled)
			{
				myTimer.Start();
			}
            else if (!SWAT_Editor.Properties.Settings.Default.AutosaveEnabled && myTimer.Enabled)
			{
				myTimer.Stop();
			}

            //if (!string.IsNullOrEmpty(SWAT_Editor.Properties.Settings.Default.FitnesseRootDirectory))
            //{
            //    swatExplorer.clear();
            //    swatExplorer.Load(SWAT_Editor.Properties.Settings.Default.FitnesseRootDirectory);
            //}

            if (!string.IsNullOrEmpty(SWAT.FitnesseSettings.FitnesseRootDirectory))
            {
                swatExplorer.clear();
                swatExplorer.Load(SWAT.FitnesseSettings.FitnesseRootDirectory);
            }
		}

		/* SNIPPET DISABLED
		private void snippetButton_Click(object sender, EventArgs e)
		{
		  CreateSnippetForm snipForm = new CreateSnippetForm();
		  snipForm.FormClosed += new FormClosedEventHandler(snipForm_FormClosed);
		  snipForm.ShowDialog(this);      
		}

		private void snipForm_FormClosed(object sender, FormClosedEventArgs e)
		{
		  useCustomSnippets();
		}

		private void customSnippets_Click(object sender, EventArgs e)
		{
		  MenuItem prac = (MenuItem)sender;
		  string docpath = SWAT_Editor.Properties.Settings.Default.CustomSnippetDirectory;
		  DirectoryInfo swatPath = new DirectoryInfo(docpath);

		  StreamReader inputStream = File.OpenText(swatPath + prac.Text + ".txt");
		  txtScript.AppendText(inputStream.ReadToEnd() + Environment.NewLine);


		}

		/// <summary>
		/// Used to search given directory for text files and to display them in a 
		/// context menu. Gives each item the customSnippets_Click event.
		/// </summary>
		/// <param name="dir">DirectoryInfo Class which holds directory to search</param>
		private void loadCustoms(DirectoryInfo dir)
		{

		  FileInfo[] listOfFiles = dir.GetFiles("*.txt");
		  ContextMenu customList = new ContextMenu();
		  MenuItem snippetSub = new MenuItem();
		  MenuItem refreshSub = new MenuItem();
		  snippetSub.Text = "Custom Snippets";
		  refreshSub.Text = "Refresh Snippets";
		  snippetSub.Name = "snipMenu";
		  refreshSub.Click += new EventHandler(refreshSub_Click);
		  customList.MenuItems.Add(snippetSub);
		  customList.MenuItems.Add(refreshSub);
		  customList.MenuItems.AddRange(createCutCopyPaste());
		  txtScript.ContextMenu = customList;

		  if (listOfFiles.Length == 0)
		  {
			 MenuItem none = new MenuItem();
			 none.Text = "No Snippets Found";
			 none.Enabled = false;
			 snippetSub.MenuItems.Add(none);
		  }
		  else
		  {

			 foreach (FileInfo file in listOfFiles)
			 {
				int num = file.Name.Length;
				string shortName = file.Name.Substring(0, num - 4);
				snippetSub.MenuItems.Add(new MenuItem(shortName, new System.EventHandler(this.customSnippets_Click)));

			 }
		  }
		}        
		  private void Menu_Undo(object sender, EventArgs e)
		  {
				txtScript.Undo();
		  }
		private MenuItem[] createCutCopyPaste()
		{
		  MenuItem[] baseCommands = new MenuItem[4];

		  MenuItem copySub = new MenuItem();
		  MenuItem cutSub = new MenuItem();
		  MenuItem pasteSub = new MenuItem();
		  MenuItem sepSub = new MenuItem();


		  sepSub.Text = "-";
		  copySub.Text = "Copy";
		  cutSub.Text = "Cut";
		  pasteSub.Text = "Paste";


		  copySub.Click += new EventHandler(copySub_Click);
		  cutSub.Click += new EventHandler(cutSub_Click);
		  pasteSub.Click += new EventHandler(pasteSub_Click);

		  baseCommands[0] = sepSub;
		  baseCommands[1] = cutSub;
		  baseCommands[2] = copySub;
		  baseCommands[3] = pasteSub;

		  return baseCommands;
		}

        
		void refreshSub_Click(object sender, EventArgs e)
		{
		  useCustomSnippets();
		}

		void pasteSub_Click(object sender, EventArgs e)
		{
		  txtScript.Paste();
		}

		void cutSub_Click(object sender, EventArgs e)
		{
		  txtScript.Cut();
		}

		void copySub_Click(object sender, EventArgs e)
		{
		  txtScript.Copy();
		}
		private void noDirectory()
		{
		  ContextMenu customList = new ContextMenu();
		  MenuItem snippetSub = new MenuItem();
		  snippetSub.Text = "Custom Snippets";
		  snippetSub.Name = "snipMenu";
		  MenuItem refreshSub = new MenuItem();
		  refreshSub.Text = "Refresh Snippets";
		  refreshSub.Enabled = false;
		  customList.MenuItems.Add(snippetSub);
		  customList.MenuItems.Add(refreshSub);
		  customList.MenuItems.AddRange(createCutCopyPaste());
		  txtScript.ContextMenu = customList;

		  MenuItem none = new MenuItem();
		  none.Text = "Set Directory Using Options";
		  none.Enabled = false;
		  snippetSub.MenuItems.Add(none);

		  snippetButton.Enabled = false;

		}
		/*
		/// <summary>
		/// Used to reload the context menu and to determine if it is to display.
		/// </summary>
		private void useCustomSnippets()
		{
		  if (SWAT_Editor.Properties.Settings.Default.CustomSnippetDirectory == "")
		  {
			 noDirectory();
		  }
		  else
		  {
			 DirectoryInfo snipDir = new DirectoryInfo(SWAT_Editor.Properties.Settings.Default.CustomSnippetDirectory);
			 loadCustoms(snipDir);
			 snippetButton.Enabled = true;
		  }
		}*/

		private void useInternetExplorerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			useInternetExplorerToolStripMenuItem.Checked = true;
            useFirefoxToolStripMenuItem1.Checked = false;
            useChromeToolStripMenuItem.Checked = false;
            useSafariToolStripMenuItem.Checked = false;
			SWAT_Editor.Properties.Settings.Default.TestBrowserType = "ie";
		}

		private void useFireFoxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			useInternetExplorerToolStripMenuItem.Checked = false;
            useFirefoxToolStripMenuItem1.Checked = true;
            useChromeToolStripMenuItem.Checked = false;
            useSafariToolStripMenuItem.Checked = false;
			SWAT_Editor.Properties.Settings.Default.TestBrowserType = "ff";
		}

        private void useChromeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useInternetExplorerToolStripMenuItem.Checked = false;
            useFirefoxToolStripMenuItem1.Checked = false;
            useChromeToolStripMenuItem.Checked = true;
            useSafariToolStripMenuItem.Checked = false;
            SWAT_Editor.Properties.Settings.Default.TestBrowserType = "chrome";
        }

        private void useSafariToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useInternetExplorerToolStripMenuItem.Checked = false;
            useFirefoxToolStripMenuItem1.Checked = false;
            useChromeToolStripMenuItem.Checked = false;
            useSafariToolStripMenuItem.Checked = true;
            SWAT_Editor.Properties.Settings.Default.TestBrowserType = "safari";
        }
        
		//Captures Key strokes
		//private void txtScript_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		//{
		//    if (e.KeyCode == Keys.Z && e.Control)
		//    {
		//        if (SnapShot.Count != 0)
		//        {
		//            txtScript.Text = SnapShot.Pop().ToString();
		//            MoveCaret();
		//        }

		//        else if (SnapShot.Count == 0)
		//            txtScript.Text = "";
		//    }
		//    else
		//    {
		//        if (SnapShot.Count > 50)
		//        {
		//            SnapShot.Clear();
		//        }                
		//        if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Insert ||  e.KeyCode == Keys.Delete ||  e.KeyCode == Keys.Enter || e.KeyCode == Keys.X && e.Control || e.KeyCode == Keys.V && e.Control)
		//        {
		//            SnapShot.Push(txtScript.Text);
		//        }                               
		//    }
		//}

		////Moves the caret to the end of the string being modified
		//private void MoveCaret()
		//{
		//    this.txtScript.SelectionStart = txtScript.Text.Length;                        
		//}

		private void newScriptToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.AddNewScriptEditor(this);
			if (commandEditor1.CanStart && !ddEditor1.Visible)
			{
                toolStripPlayButton.Enabled = true;
                toolStripRecordButton.Enabled = true;
                //notify icon
                startRecToolStripMenuItem2.Enabled = true;
                assertToolStripNotifyIcon.Enabled = false;
                stopToolStripNotifyIcon.Enabled = false;
                UnpauseToolStripNotifyIcon.Enabled = true;
                pauseToolStripNotifyIcon.Enabled = false;     
			}

            commandEditor1.CurrentEditorPage.getEditor().Focus();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.AddExistingScriptEditor(this);

			try
			{
				_recentFiles.Add(commandEditor1.CurrentEditorPage.FilePath);
			}
			catch { }
		}

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (commandEditor1.CanSave)
            {
                commandEditor1.SaveScriptEditor();
                try
                {
                    _recentFiles.Add(commandEditor1.CurrentEditorPage.FilePath);
                }
                catch { }
            }
        }
        //Save As
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (commandEditor1.CanSave) { commandEditor1.SaveAs(); }
        }

		//Undo
		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.Undo();
		}

		//Redo
		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.Redo();
		}

		//Cut
		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.Cut();
		}

		//Paste
		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.Paste();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.Copy();

		}

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ddEditor1.Active)
                PlayDDE();
            else
            {
                dDEditorWizardSelect.Enabled = false;
                Play();
            }
        }
		private void stopToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (ddEditor1.Active)
                StopDDE();
            else
			    Stop();		
        }

		private void startToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Record();
		}

		private void stopRecToolStripMenuItem_Click(object sender, EventArgs e)
		{
            if (ddEditor1.Active)
                StopDDE();
            else
			    Stop();		
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ddEditor1.Active)
                StopDDE();
            else
                Stop();
        }

		private void recordActionsToolStripMenuItem_MouseEnter(object sender, EventArgs e)
		{
			//stopRecToolStripMenuItem.Enabled = ieRecorder1.Recording;
			stopRecToolStripMenuItem2.Enabled = ieRecorder1.Recording;
			//startRecToolStripMenuItem.Enabled = !ieRecorder1.Recording && commandEditor1.CanSave; //if commandEditor can save it means a current document is loaded.
			startRecToolStripMenuItem2.Enabled = !ieRecorder1.Recording && commandEditor1.CanSave;
		}

		private void dataAccessEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (dbForm == null || dbForm.IsDisposed)
			{
				dbForm = new DBBuilderForm();
				dbForm.Show(this);
			}
			else
			{
				dbForm.Activate();
				dbForm.WindowState = FormWindowState.Normal;
			}
		}

		private void toolStripPlayButton_Click(object sender, EventArgs e)
		{
            if (ddEditor1.Active)
                PlayDDE();
            else
            {
                dDEditorWizardSelect.Enabled = false;
                Play();
            }
		}

		private void toolStripPauseButton_Click(object sender, EventArgs e)
		{
			//Pause();
		}

		private void toolStripStopButton_Click(object sender, EventArgs e)
		{
            if (ddEditor1.Active)
                StopDDE();
            else
                Stop();	
		}

		private void toolStripRecordButton_Click(object sender, EventArgs e)
		{
			Record();
		}

		private void startToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			Record();
		}

		private void toolStripResumeButton_Click(object sender, EventArgs e)
		{
			Resume();
		}

		private void stepFowardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Step();
		}

		/*        private void toolStripAssertionButton_Click(object sender, EventArgs e)
				  {
						SWAT_Editor.Recorder.HTMLEvents.AssertElement = !SWAT_Editor.Recorder.HTMLEvents.AssertElement;
						if (SWAT_Editor.Recorder.HTMLEvents.AssertElement) toolStripAssertionButton.Checked = true;
						else
							 toolStripAssertionButton.Checked = false;
						if (assertToolStripNotifyIcon.Text.Equals("Assert"))
							 assertToolStripNotifyIcon.Text = "Stop Asserting";
						else
							 assertToolStripNotifyIcon.Text = "Assert";
				  }
		 * */

		//MenuActivated event of the menustrip
		//This happens before the click even and therefore eliminates the flickery
		private void menuStrip1_MenuActivate(object sender, EventArgs e)
		{
            if (ddEditor1.Active)
            {
                setDDEMenuSettings();
            }
            else
            {
                commandEditorMenuSettings();
            }
		}

        private void setDDEMenuSettings()
        {
            closeToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            saveAllToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            copyToolStripMenuItem.Enabled = cutToolStripMenuItem.Enabled =
                 toolStripMenuItem2Comment.Enabled = toolStripMenuItem2UnComment.Enabled =
                 false;
            pasteToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem1.Enabled = false;
            findToolStripMenuItem.Enabled = replaceToolStripMenuItem.Enabled = false;
            //runToolStripMenuItem1.Enabled = !commandEditor1.CanStop;
            stopToolStripMenuItem1.Enabled = commandEditor1.CanStop;
        }

        private void commandEditorMenuSettings()
        {
            closeToolStripMenuItem.Enabled = commandEditor1.CanSave;
            saveToolStripMenuItem.Enabled = commandEditor1.CanSave;
            saveAsToolStripMenuItem.Enabled = commandEditor1.CanSave;
            saveAllToolStripMenuItem.Enabled = commandEditor1.CanSaveAll;
            undoToolStripMenuItem.Enabled = commandEditor1.CanUndo;
            redoToolStripMenuItem.Enabled = commandEditor1.CanRedo;
            copyToolStripMenuItem.Enabled = cutToolStripMenuItem.Enabled =
                 toolStripMenuItem2Comment.Enabled = toolStripMenuItem2UnComment.Enabled =
                 commandEditor1.CanCut;
            pasteToolStripMenuItem.Enabled = commandEditor1.CanPaste;
            //runToolStripMenuItem1.Enabled = commandEditor1.CanStart;
            stopToolStripMenuItem1.Enabled = commandEditor1.CanStop;
            findToolStripMenuItem.Enabled = replaceToolStripMenuItem.Enabled = commandEditor1.CanFind;
        }

		/*
		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			 if (frForm == null || frForm.IsDisposed)
				  frForm = new FindReplaceForm(commandEditor1);

			 frForm.load(this, "Replace");
		}*/

		private void reportABugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//for now, we'll just link it to the sourcefourge page to report bugs
			System.Diagnostics.Process.Start("IExplore.exe", "http://sourceforge.net/tracker/?func=add&group_id=199701&atid=970554");

			//if (rbaForm == null || rbaForm.IsDisposed)
			//{
			//    rbaForm = new ReportBugForm();
			//    rbaForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(rbaForm_FormClosed);
			//}

			//rbaForm.load(this, "Report A Bug");
			//reportABugToolStripMenuItem.Enabled = false;
		}
        private void checkVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string downloadURL = "http://sourceforge.net/projects/ulti-swat/";
            Version latestVersion = getSVNVersionNumber();
            Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (curVersion.CompareTo(latestVersion) < 0)
            {
                string title = "New version detected.";
                string question = "Download the new version?";
                if (DialogResult.Yes ==
                    MessageBox.Show(this, question, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    System.Diagnostics.Process.Start(downloadURL);
                }
            }
            else
            {
                MessageBox.Show("You are running the latest version!");
            }
        }
        private Version getSVNVersionNumber()
        {
            Version newVersion = null;
            string xmlURL = "https://ulti-swat.svn.sourceforge.net/svnroot/ulti-swat/VersionFile/version.xml";
            try
            {
                //create new reader
                XmlTextReader reader = new XmlTextReader(xmlURL);
                //skip to content
                reader.MoveToContent();
                string elementName = "";
                //check that current node is an element <SWAT>
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "SWAT"))
                {
                    while (reader.Read())
                    {
                        //remember element node's name
                        if (reader.NodeType == XmlNodeType.Element)
                            elementName = reader.Name;
                        else
                        {
                            //handle text
                            if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                            {

                                switch (elementName)
                                {
                                    case "version":
                                        newVersion = new Version(reader.Value);
                                        break; 
                                }

                            }
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to check for update");
            }
            return newVersion;
        }

		//when the reportabug window closes, enable the click event again
		void rbaForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
		{
			reportABugToolStripMenuItem.Enabled = true;
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutForm form = new AboutForm();
			form.ShowDialog(this);
		}

		//navigates to the swat main wiki page
		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("IExplore.exe", "http://ulti-swat.wiki.sourceforge.net/");
		}

		//sets the user's preference to use or not the intellisense feature
		private void useIntellisenseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.reverseIntellisense();
		}

		private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Resume();
		}

		private void commentLinesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.CurrentEditorPage.CommentLines();
		}

		private void uncommentLinesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.CurrentEditorPage.UnCommentLines();
		}

		private void startFindReplace(FindReplaceForm.ModeValues startMode)
		{
			if (frForm == null || frForm.IsDisposed)
			{
				frForm = new FindReplaceForm(commandEditor1);
			}
			frForm.load(this, startMode);
		}

		private void quickFindMI_Click(object sender, EventArgs e)
		{
			this.startFindReplace(FindReplaceForm.ModeValues.quickFind);
		}

		private void quickReplaceMI_Click(object sender, EventArgs e)
		{
			this.startFindReplace(FindReplaceForm.ModeValues.quickReplace);
		}

		private void recordToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			if (commandEditor1.TabSet.TabPages.Count == 0 || ieRecorder1.Recording || ddEditor1.Visible)
				startRecToolStripMenuItem2.Enabled = false;
			else
				startRecToolStripMenuItem2.Enabled = true;

            if (ieRecorder1.Recording)
            {
                stopRecToolStripMenuItem2.Enabled = true;
            }
            else
                stopRecToolStripMenuItem2.Enabled = false;
		}

		private void debugDDToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			if ((commandEditor1.TabSet.TabPages.Count == 0 && !ddEditor1.Active) || commandEditor1.CanStop)
				runToolStripMenuItem1.Enabled = false;
			else
				runToolStripMenuItem1.Enabled = true;
		}

		#endregion


		#region minimizeToTray


		private void MainForm_Resize_1(object sender, EventArgs e)
		{
            if (SWAT_Editor.Properties.Settings.Default.MinimizeToTray)
            {
                minimizeToTray();
            }
            else
            {
                minimizeToTaskbar();
            }
		}

		private void recordToolStripNotifyIcon_Click(object sender, EventArgs e)
		{
            this.ResumeRecording();
		}

		private void assertToolStripNotifyIcon_Click(object sender, EventArgs e)
		{
			SWAT_Editor.Recorder.HTMLEvents.AssertElement = !SWAT_Editor.Recorder.HTMLEvents.AssertElement;
			if (SWAT_Editor.Recorder.HTMLEvents.AssertElement) toolStripAssertionButton.Checked = true;
			else
				toolStripAssertionButton.Checked = false;
			if (assertToolStripNotifyIcon.Text.Equals("Assert"))
			{
				assertToolStripNotifyIcon.Text = "Stop Asserting";
				this.notifyIcon.BalloonTipText = "Assert Mode Activated";
				notifyIcon.ShowBalloonTip(3);
			}
			else
				assertToolStripNotifyIcon.Text = "Assert";
		}

		private void playToolStripNotifyIcon_Click(object sender, EventArgs e)
		{
			this.Play();
		}

		private void pauseToolStripNotifyIcon_Click(object sender, EventArgs e)
		{
			this.Pause();
		}

		private void stopToolStripNotifyIcon_Click(object sender, EventArgs e)
		{
			this.Stop();
			openMainForm();
		}

		private void openToolStripNotifyIcon_Click(object sender, EventArgs e)
		{
			openMainForm();
		}

		private void exitToolStripNotifyIcon_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("Are you sure you want to exit the editor?", "SWAT Editor", MessageBoxButtons.OKCancel);
			if (result == DialogResult.OK)
			{ this.notifyIcon.Visible = false; Application.Exit(); }
			else
				return;
		}

		private void notifyIcon_DoubleClick_1(object sender, EventArgs e)
		{
			openMainForm();
		}

		private void minimizeToTray()
		{
			if (FormWindowState.Minimized == WindowState)
			{
				Hide();
				this.notifyIcon.Visible = true;
				if (!commandEditor1.CanStart)
				{
                    assertToolStripNotifyIcon.Enabled = false;
                    stopToolStripNotifyIcon.Enabled = false;
                    UnpauseToolStripNotifyIcon.Enabled = false;
                    pauseToolStripNotifyIcon.Enabled = false;
				}
				else if (ieRecorder1.Recording)
				{ ;}
				else
				{
                    assertToolStripNotifyIcon.Enabled = false;
                    stopToolStripNotifyIcon.Enabled = false;
                    UnpauseToolStripNotifyIcon.Enabled = true;
                    pauseToolStripNotifyIcon.Enabled = false;
				}
			}
		}

        private void minimizeToTaskbar()
        {
            if (FormWindowState.Minimized == WindowState)
            {
                this.ShowInTaskbar = true;
            }
        }

		private void openMainForm()
		{
			this.notifyIcon.Visible = false;
			this.Show();
			WindowState = FormWindowState.Normal;
		}
		#endregion


		#region Tool Stripevents

		private void toolStripMenuItem2Comment_Click(object sender, EventArgs e)
		{
			commandEditor1.CurrentEditorPage.CommentLines();
		}

		private void toolStripMenuItem2UnComment_Click(object sender, EventArgs e)
		{
			commandEditor1.CurrentEditorPage.UnCommentLines();
		}

		private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			commandEditor1.SaveAllTabs();
		}

		private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			_recentFiles.Init();
			recentFilesToolStripMenuItem.DropDownItems.Clear();
			foreach (string item in _recentFiles.GetAll())
			{
				if ((item != "") && (item != null)) recentFilesToolStripMenuItem.DropDownItems.Add(item);
			}
			if (recentFilesToolStripMenuItem.DropDownItems.Count == 0 || ddEditor1.Active) recentFilesToolStripMenuItem.Enabled = false;
			else recentFilesToolStripMenuItem.Enabled = true;
		}

		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			commandEditor1.AddRecentScriptEditor(this, e.ClickedItem.Text);
		}

		private void showExplorerToolStripMenuItem_Click(object sender, EventArgs e)
		{
            Properties.Settings.Default.ShowExplorerChecked = showExplorerToolStripMenuItem1.Checked;
            Properties.Settings.Default.ShowExplorerCheckState = showExplorerToolStripMenuItem1.CheckState;
            this.topMainContainer.Panel2Collapsed = !Properties.Settings.Default.ShowExplorerChecked;
            Properties.Settings.Default.Save();
		}

		#endregion


		#region Misc Events

		private void myTimer_Tick_1(object sender, EventArgs e)
		{
			commandEditor1.SaveAllTabsTemp();
		}

		private void showLowerTabset_Click(object sender, EventArgs e)
		{
            Properties.Settings.Default.ShowResultsChecked = showResultTypeToolStripMenuItem.Checked;
            Properties.Settings.Default.ShowResultsCheckState = showResultTypeToolStripMenuItem.CheckState;
            this.mainContainer.Panel2Collapsed = !Properties.Settings.Default.ShowResultsChecked;
            Properties.Settings.Default.Save();
		}

		#endregion


		#region Open Browsers Operations
		//TODO: make the combobox and button enabled only if there is at least a window open. Problem: disable it when all windows are closed

		private void fillOpenBrowserWindows()
		{
			cBox_openBrowserWindows.Items.Add("");
			foreach (SHDocVw.InternetExplorer Browser in m_IEFoundBrowsers)
			{
				if (Browser.FullName.ToLower().Contains("iexplore.exe"))
					cBox_openBrowserWindows.Items.Add(Browser.LocationName);
			}
		}

		private void btn_attachToWindow_Click(object sender, EventArgs e)
		{
			if (cBox_openBrowserWindows.Text != "" && commandEditor1.CanSave)
				commandEditor1.CurrentEditorPage.getEditor().Document.InsertAtCaret(string.Format("\n|AttachToWindow|{0}|", cBox_openBrowserWindows.Text));
			else
				MessageBox.Show("Please select a window to attach to first.");

			cBox_openBrowserWindows.Text = "";
		}

		private void cBox_openBrowserWindows_MouseDown(object sender, MouseEventArgs e)
		{
			string temp = cBox_openBrowserWindows.Text;
			cBox_openBrowserWindows.Items.Clear();
			fillOpenBrowserWindows();
			cBox_openBrowserWindows.Text = temp;
		}

		#endregion

        #region DDE Helper Methods

        private void prepareTests()
        {
            XmlTextReader xml = ddEditor1.getXMLFileContent();
            TextReader test = ddEditor1.getTestFileContent();
            bool testEnabled = false;
            ddEditor1.AllTestNames = new Queue<string>();

            //Extract information from xml
            xml.WhitespaceHandling = WhitespaceHandling.None;
            try { xml.Read(); }
            catch (XmlException e) { Console.WriteLine("Improper Header"); }
            xml.ReadStartElement("TestCases");
            while (xml.LocalName.Equals("TestCase"))
            {
                File.WriteAllText(ddEditor1.TempPath, test.ReadToEnd());
                test = ddEditor1.getTestFileContent();

                ddEditor1.TestName = ddEditor1.determineTestName(xml);
                testEnabled = ddEditor1.determineTestEnabled(xml);

                if (testEnabled)
                    ddEditor1.AllTestNames.Enqueue(ddEditor1.TestName);
                xml.Read();

                ddEditor1.replaceVariables(xml);

                if (testEnabled)
                    commandEditor1.AddRecentScriptEditor(this, ddEditor1.TempPath);
                try { xml.ReadEndElement(); } //TestCase
                catch (XmlException e) { break; };
            }
            try { xml.ReadEndElement(); } //TestCases
            catch (XmlException e) { };
        }

        private void selectFirstTest()
        {
            if (commandEditor1.TabSet.TabCount != 0)
                commandEditor1.TabSet.SelectTab(ddEditor1.StartIndex);
        }

        private void startDDE()
        {
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync();

            File.Delete(ddEditor1.TempPath);
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            CommandResult displayName = new CommandResult();
            bool testStarted = false;

            BrowserType browser = new BrowserType();
            this.Invoke(new MethodInvoker(delegate() { try { browser = this.getBrowserType(); } catch (NullReferenceException ex) { return; } }));

            while (doDDETestsRemain() && !bw.CancellationPending)
            {
                if (!testStarted)
                {
                    ddEditor1.ResetDataForNextTest();

                    displayNextTest(displayName);

                    testStarted = beginNextTest();
                }
                else if (didTestFinish())
                {
                    updateTestResult(displayName);

                    createHtmlOutput(browser);

                    ddEditor1.incrementPassFailCounter();

                    ddEditor1.concludeTest(displayName);

                    (sender as BackgroundWorker).ReportProgress(0);
                    testStarted = false;
                    Thread.Sleep(500);
                }
                else //Test Running
                    Thread.Sleep(20);
            }

            ddEditor1.createSummaryFile(browser);

            if (bw.CancellationPending)
            {
                displayName.Ignored = true;
                displayName.ModIgn = true;
                displayName.Message = "Stopped";
                this.Invoke(new MethodInvoker(delegate() { commandEditor1.ResultDisplayer.UpdateResult(displayName); }));
            }
        }

        private bool doDDETestsRemain()
        {
            return commandEditor1.TabSet.TabCount != ddEditor1.StartIndex;
        }

        private void displayNextTest(CommandResult displayName)
        {
            displayName.Command = ddEditor1.TestFront + ddEditor1.TestName;
            displayName.Message = "Running...";
            displayName.LineNumber = ++ddEditor1.TestNum;
            this.Invoke(new MethodInvoker(delegate() { commandEditor1.ResultDisplayer.LogResult(displayName); }));
        }

        private bool beginNextTest()
        {
            this.Invoke(new MethodInvoker(Play));
            Thread.Sleep(750);

            return true;
        }

        private void updateTestResult(CommandResult displayName)
        {
            displayName.Success = ddEditor1.TestPassed;
            displayName.Message = displayName.Success ? "Passed" : "Failed on line " + ddEditor1.FailureReason;
            this.Invoke(new MethodInvoker(delegate() { commandEditor1.ResultDisplayer.UpdateResult(displayName); }));
        }

        private void createHtmlOutput(BrowserType browser)
        {
            ddEditor1.ResultHandler = new ResultHandler(ddEditor1.TestName, browser, commandEditor1.CurrentEditorPage.getTestResults());
            ddEditor1.ResultHandler.SaveResultsAsHtml(ddEditor1.ResultPath + ddEditor1.TestName + ".html");
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            commandEditor1.closeCurrentDocument(this, new EventArgs());
            if (commandEditor1.TabSet.TabCount > ddEditor1.StartIndex)
                commandEditor1.TabSet.SelectTab(ddEditor1.StartIndex);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Closes any tests that may not have run do to the tests being stopped.
            while (commandEditor1.TabSet.TabCount != ddEditor1.StartIndex)
            {
                commandEditor1.closeCurrentDocument(this, new EventArgs());
            }

            commandEditorToolStripButton.Enabled = true;
            dBBuilderWizardSelect.Enabled = true;
            toolStripPlayButton.Enabled = true;
            toolStripPauseButton.Enabled = false;
            toolStripRecordButton.Enabled = false;
            toolStripStopButton.Enabled = false;
            toolStripResumeButton.Enabled = false;
            resumeToolStripMenuItem.Enabled = false;
        }

        private bool didTestFinish()
        {
            return !commandEditor1.CurrentEditorPage.isTestRunning;
        }

        private void setupDDE()
        {
            commandEditor1.ResultDisplayer.Clear();
            ddEditor1.ResetTotalTestStatistics();
            commandEditorToolStripButton.Enabled = false;
            dBBuilderWizardSelect.Enabled = false;
            ddEditor1.StartIndex = commandEditor1.TabSet.TabCount;
        }

        #region unused script parser methods
        /*private string getToTestCases(TextReader xml)
        {
            string cur = xml.ReadLine();
            while (cur != null && !cur.ToLower().Contains("testcases"))
                cur = xml.ReadLine();

            return cur;
        }

        private string moveToNextTest(TextReader xml, string curXML)
        {
            string cur = curXML;
            while (cur != null && !cur.ToLower().Contains("tcid") && !cur.ToLower().Contains("[\btestcase\b]?"))
                cur = xml.ReadLine();

            return cur;
        }

        private string extractValue(string curXML, string attribute)
        {
            string attributeValue = null;
            string[] split = System.Text.RegularExpressions.Regex.Split(curXML, attribute + @"[\s]*?=[\s]*?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (split.Length > 1)
            {
                attributeValue = split[split.Length - 1];
                attributeValue = System.Text.RegularExpressions.Regex.Match(attributeValue, "\"" + @"[\w\W]*?" + "\"").ToString();
                attributeValue = attributeValue.TrimEnd(new char[] { '"' });
                attributeValue = attributeValue.TrimStart(new char[] { '"' });
            }
            return attributeValue;
        }*/
        #endregion
        #endregion

        private void dBBuilderWizardSelect_Click(object sender, EventArgs e)
		{
			if (!dbBuilderWizard1.Visible)
			{
				commandEditorToolStripButton.Checked = false;
				commandEditor1.Visible = false;
                dDEditorWizardSelect.Checked = false;                
				ddEditor1.Visible = false;				
				dbBuilderWizard1.Visible = true;
                dBBuilderWizardSelect.Checked = true;
                SwitchForm();			
			}
			else
				dBBuilderWizardSelect.Checked = true;

		}

		private void commandEditorToolStripButton_Click(object sender, EventArgs e)
		{
			if (!commandEditor1.Visible)
			{
				commandEditorToolStripButton.Checked = true;
				dBBuilderWizardSelect.Checked = false;
				dbBuilderWizard1.Visible = false;
                dDEditorWizardSelect.Checked = false;
                ddEditor1.Visible = false;                
				commandEditor1.Visible = true;
				SwitchForm();
			}
		}

        private void dDEditorWizardSelect_Click(object sender, EventArgs e)        
		{
			if (!ddEditor1.Visible)
			{
				commandEditorToolStripButton.Checked = false;
				commandEditor1.Visible = false;
				dbBuilderWizard1.Visible = false;
				dBBuilderWizardSelect.Checked = false;
				dDEditorWizardSelect.Checked = true;
				ddEditor1.Visible = true;
				SwitchForm();
			}
		}

		private void sleepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserInputPopup Sleep = new UserInputPopup();
            Sleep.Text = "Insert Sleep Command";
            Sleep.SetLabel("How many milliseconds would you like SWAT to sleep for?");
            DialogResult result = Sleep.ShowDialog(this);
            string sleepstring;
            if (result == DialogResult.OK)
            {
                sleepstring = Sleep.GetUserInput();
                int sleepTime = 0;
                try
                {
                    sleepTime = (int)new Int32Converter().ConvertFromString(sleepstring);
                }
                catch
                {
                    MessageBox.Show("Invalid Number entered");
                    return;
                }
                this.Pause();
                commandEditor1.AppendToCurrentScript("|Sleep|" + sleepTime + "|" + System.Environment.NewLine, false);
                this.ResumeRecording();
            }
            return;
        }

        private void assertBrowserDoesNotExistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserInputPopup AssertBrowserDNE = new UserInputPopup();
            AssertBrowserDNE.Text = "Insert AssertBrowserDoesNotExist Command";
            AssertBrowserDNE.SetLabel("What is the name of the window that you want to assert doesn't exist");
            DialogResult result = AssertBrowserDNE.ShowDialog(this);
            string windowtitlestring;
            if (result == DialogResult.OK)
            {
                windowtitlestring = AssertBrowserDNE.GetUserInput();
                this.Pause();
                commandEditor1.AppendToCurrentScript("|AssertBrowserDoesNotExist|" + windowtitlestring + "|" + System.Environment.NewLine, false);
                this.ResumeRecording();
            }
            return;
        }

        private void ResumeRecording()
        {
            pauseToolStripNotifyIcon.Enabled = true;
            UnpauseToolStripNotifyIcon.Enabled = false;
            ieRecorder1.Resume();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            commandEditor1.closeCurrentDocument(sender, e);
        }

    
        private void SwitchForm()
        {
            if (ddEditor1.Active)
            {
                switchToDDE();
            }
            else
            {
                switchToCommandEditor();
            }
        }

        private void switchToDDE()
        {
            toolStripRecordButton.Enabled = false;
            toolStripPlayButton.Enabled = true;
            newScriptToolStripMenuItem.Enabled = false;
            openToolStripMenuItem.Enabled = false;
            enableBreakPointsToolStripMenuItem.Enabled = false;
            commandEditor1.ResultDisplayer.Clear();
        }

        private void switchToCommandEditor()
        {
            newScriptToolStripMenuItem.Enabled = true;
            openToolStripMenuItem.Enabled = true;
            enableBreakPointsToolStripMenuItem.Enabled = true;
            if (commandEditor1.CanStart)
            {
                commandEditor1.CurrentEditorPage.refreshResultList();
                toolStripRecordButton.Enabled = true;
                toolStripPlayButton.Enabled = true;
            }
            else
            {
                commandEditor1.ResultDisplayer.Clear();
                toolStripRecordButton.Enabled = false;
                toolStripPlayButton.Enabled = false;
            }
        }
    }
}
