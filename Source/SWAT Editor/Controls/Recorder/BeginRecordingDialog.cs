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

namespace SWAT_Editor.Controls.Recorder
{
	public partial class BeginRecordingDialog : Form
	{
		private int _selection = 0;
		private System.Collections.ArrayList browserList;
		private System.Collections.ArrayList indexes;
		private SWAT_Editor.Recorder.IERecorder ieRecorder;
		private NotifyIcon _appNotifier;

		public BeginRecordingDialog(ref SWAT_Editor.Recorder.IERecorder recorder, Boolean isIEBrowserSelected, NotifyIcon appNotifier)
		{

			InitializeComponent();
			ieRecorder = recorder;
			_appNotifier = appNotifier;
			browserList = new System.Collections.ArrayList();
			indexes = new System.Collections.ArrayList();
			populateBrowserCheckedBox();
			labelIEBrowserWarning.Text = "**Swat uses Internet Explorer for all recording.\n  Your browser settings have been updated.**";
			labelIEBrowserWarning.Visible = !isIEBrowserSelected;
		}



		private void populateBrowserCheckedBox()
		{
			ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
			foreach (InternetExplorer Browser in m_IEFoundBrowsers)
			{
				int index = 0;

                try
                {
                    if (Browser.Document != null && Browser.Document is mshtml.HTMLDocument)
                    {
                        string title = ((mshtml.HTMLDocument)Browser.Document).title;
                        if (browserList.Contains(title))
                        {
                            foreach (string a in browserList)
                            {
                                if (a.Equals(title)) index++;
                            }
                        }
                        browserCheckedListBox.Items.Add(((mshtml.HTMLDocument)Browser.Document).title + " - Index: " + index);
                        browserList.Add(((mshtml.HTMLDocument)Browser.Document).title);
                        indexes.Add(index);
                    }
                }
                catch (System.Runtime.InteropServices.COMException) { continue; }
			}
			if (browserCheckedListBox.Items.Count == 0) attachBrowserButton.Enabled = false;
		}

		private void newBrowserButton_CheckedChanged(object sender, EventArgs e)
		{
			if (this.newBrowserButton.Checked)
			{
				_selection = 0;
				okButton.Enabled = true;
			}
		}

		private void attachBrowserButton_CheckedChanged(object sender, EventArgs e)
		{
			if (this.attachBrowserButton.Checked)
			{
				_selection = 1;
				if (browserCheckedListBox.CheckedIndices.Count == 0) okButton.Enabled = false;
			}
		}

		private void browserCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.attachBrowserButton.Checked = true;
			if (browserCheckedListBox.CheckedIndices.Count == 0)
			{
				okButton.Enabled = false;
			}
			else
			{
				okButton.Enabled = true;
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (_selection == 0)
			{
				ieRecorder.Record(this._appNotifier.ContextMenuStrip);
			}
			else
			{
				ieRecorder.Record(this._appNotifier.ContextMenuStrip,browserList, indexes, this.browserCheckedListBox.CheckedIndices);
			}
		}

		private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < browserCheckedListBox.Items.Count; i++)
			{
				browserCheckedListBox.SetItemChecked(i, selectAllCheckBox.Checked);
			}
			attachBrowserButton.Checked = true;
			if (browserCheckedListBox.CheckedIndices.Count == 0) okButton.Enabled = false;
			else okButton.Enabled = true;
		}

		private void BeginRecordingDialog_Load(object sender, EventArgs e)
		{
			selectAllCheckBox.Visible = false;
		}

	}
}
