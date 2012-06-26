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

namespace SWAT_Editor.Controls
{
	/// <summary>
	/// Represents the control to display the results of a test.
	/// </summary>
	public partial class CommandList : UserControl, ICommandEditorResultsDisplayer
	{

		# region Constructor

		/// <summary>
		/// Creates a new instance of the commandList control.
		/// </summary>
		public CommandList()
		{
			InitializeComponent();
			InitializeCompletedCommandList();
		}

		# endregion

        # region Class Methods

        /// <summary>
		/// Initializes the listView control to the default properties.
		/// </summary>
		public void InitializeCompletedCommandList()
		{
			this.lstCompletedCommands.View = View.Details;
			this.lstCompletedCommands.HideSelection = false;

			int lineWidth = 40;
			int commandWidth = ((int)(lstCompletedCommands.Width / 2));
			int resultWidth = (this.lstCompletedCommands.Width - commandWidth) - 4;
			this.lstCompletedCommands.Columns.Add("command", "Line", lineWidth);
			this.lstCompletedCommands.Columns.Add("command", "Command", commandWidth);
			this.lstCompletedCommands.Columns.Add("command", "Result", resultWidth);
		}

		/// <summary>
		/// Adds a new item to the list, given the command and the result
		/// </summary>
		/// <param name="lineNumber">Line number where the item will be inserted.</param>
		/// <param name="command">Command representing the item to be inserted.</param>
		/// <param name="result">Result of the executed command of this item.</param>
		/// <param name="color">Color of the new line item to be inserted.</param>
		public void AddNewListItem(int lineNumber, string command, string result, Color color)
		{
			ListViewItem newItem = new ListViewItem();
			newItem.Text = lineNumber.ToString();

			ListViewItem.ListViewSubItem itemCommandName = new ListViewItem.ListViewSubItem();
			ListViewItem.ListViewSubItem itemResult = new ListViewItem.ListViewSubItem();

			itemCommandName.Text = command;
			itemResult.Text = result;

			newItem.ForeColor = color;
			newItem.SubItems.Add(itemCommandName);
			newItem.SubItems.Add(itemResult);

			newItem.UseItemStyleForSubItems = true;

			this.lstCompletedCommands.Items.Add(newItem);
			this.lstCompletedCommands.EnsureVisible(this.lstCompletedCommands.Items.Count - 1);
		}

		/// <summary>
		/// Selects the item choosen by the user applying the specified selection mode.
		/// </summary>
		/// <param name="selectedMode">Mode in which the item has been selected.</param>
		private void selectItemBy(TextEditor.TextEditor.SelectedModeValues selectedMode)
		{
			if (this.lstCompletedCommands.SelectedItems.Count > 0)
			{
				int result;
				if (int.TryParse(this.lstCompletedCommands.SelectedItems[0].Text, out result))
				{
					ItemSelect(selectedMode, result);
				}
			}
		}

        public string GetCommand()
        {
            return this.lstCompletedCommands.SelectedItems[0].SubItems[1].Text;
        }

		# endregion

		# region EventHandlers

		/// <summary>
		///We want to insert the break points when we double click, so it does not interferes with the
		///behavior of any context menu added to the ListView control. This will be useful for other
		///functionalities.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstCompletedCommands_DoubleClick(object sender, EventArgs e)
		{
			this.selectItemBy(TextEditor.TextEditor.SelectedModeValues.ToInsertBreakPoint);
		}

		/// <summary>
		/// Go to the current selected line corresponding to the current command. Notice that we select 
		/// the line as well, but using a different  mode.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void goToLineMI_Click(object sender, EventArgs e)
		{
			this.selectItemBy(TextEditor.TextEditor.SelectedModeValues.ToGoToLine);
		}

		/// <summary>
		/// Controls the copyToClipboard functionality.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmsMainCopyMI_Click(object sender, EventArgs e)
		{
			const int tabAscii = 9;
			char tabChar = (char)tabAscii;	//Used to concatenate the item text values for each row.

			System.Text.StringBuilder CopiedItems;
			CopiedItems = new System.Text.StringBuilder();

			//Make sure the clipboard is empty before we attach anything to it.
			Clipboard.Clear();

			//Loop thru all the items (rows) selected in the list view control.
			foreach (ListViewItem item in this.lstCompletedCommands.SelectedItems)
			{
				//Concatenate each subItems's text.
				foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
				{
					//We copy the text of the item, even if it is empty since we want to respect the tabular format.
					CopiedItems.Append(subItem.Text);

					// Do not include the tab after the last subItem.
					if (item.SubItems.IndexOf(subItem) != item.SubItems.Count - 1)
					{
						CopiedItems.Append(tabChar);
					}
				}
				CopiedItems.AppendLine();
			}

			//Finally, results go to the clipboard. Now it is up to the user.
			Clipboard.SetText(CopiedItems.ToString().Trim());
		}

		/// <summary>
		/// Handles the opening event of the context menu of the results pane.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmsMain_Opening(object sender, CancelEventArgs e)
		{
			//The ability to copy depends in the existance of items on the listView.
			this.cmsMainCopyMI.Enabled = this.lstCompletedCommands.SelectedItems.Count > 0;
		}

		# endregion

		//Implementing interface members.
		#region ICommandEditorResultsDisplayer Members

		/// <summary>
		/// Logs the result of the test to the ListView control.
		/// </summary>
		/// <param name="result"></param>
		public void LogResult(CommandResult result)
		{
            if (result.Command.StartsWith("Test:"))
                AddNewListItem(result.LineNumber, result.Command, result.Message, Color.Blue);
			else if (!(result.Ignored) && !(result.Cond))
				AddNewListItem(result.LineNumber, result.Command, result.Message, result.Success ? Color.Green : Color.Red);
			else if (result.Cond)
				AddNewListItem(result.LineNumber, result.Command, result.Message, Color.DarkGray);
			else if (result.ModIgn)
				AddNewListItem(result.LineNumber, result.Command, result.Message, Color.Gray);
		}

        /// <summary>
        /// Removes the last entry  in the listView and replaces it with the new result
        /// </summary>
        /// <param name="result"></param>
        public void UpdateResult(CommandResult result)
        {
            lstCompletedCommands.Items.RemoveAt(lstCompletedCommands.Items.Count - 1);
            if (result.ModIgn)
				AddNewListItem(result.LineNumber, result.Command, result.Message, Color.Gray);
            else
                AddNewListItem(result.LineNumber, result.Command, result.Message, result.Success ? Color.Green : Color.Red);
        }

		/// <summary>
		/// Clears the items in the listView.
		/// </summary>
		public void Clear()
		{
			lstCompletedCommands.Items.Clear();
		}

		public event CommandEditor.ItemSelected ItemSelect;

		#endregion

	}
}
