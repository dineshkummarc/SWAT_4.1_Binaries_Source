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
using SWAT_Editor.Controls;

namespace SWAT_Editor
{
    public partial class FindReplaceForm : Form
    {
        #region Class variables

        private CommandEditor mainEditor;
        private bool loaded;
        private int findIndex;

		  public enum ModeValues 
		  { 
			  quickFind,
			  quickReplace,
		  }

        #endregion


        #region public properties

        SWAT_Editor.Controls.TextEditor.DocumentTextBox currentDocument
        {
            get
            {
					if (mainEditor.CurrentEditorPage != null)
						return mainEditor.CurrentEditorPage.getEditor().Document;

                return null;
            }
        }

        #endregion


        #region public methods

        public void Show(Form f)
        {
            if (loaded == false)
            {
                base.Show(f);
                loaded = true;
                return;
            }

            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }

        public void load(Form f, ModeValues loadMode)
        {
            if (mainEditor.CanFind)
            {
					 this.initializeMode(loadMode);
                this.Show(f);
            }
        }

		 private void initializeMode(ModeValues loadMode) 
		 {
			 this.replaceGB.Visible = this.replaceTextBox.Visible = this.replaceAllButton.Visible =
				this.replaceButton.Visible = (loadMode == ModeValues.quickReplace);

			 if (loadMode == ModeValues.quickReplace)
			 {
				 this.Size = new System.Drawing.Size(new System.Drawing.Point(300, 250));
			 }
			 else
			 {
				 this.Size = new System.Drawing.Size(new System.Drawing.Point(300, 170));
			 }
		 }

        //find next instance, if any (al)
        public bool find()
        {
            string textToFind = findTextBox.Text;
            int results = -1;
            if (findReplaceDropDown.SelectedIndex == 0)
            {
                findIndex = currentDocument.SelectionStart + currentDocument.SelectedText.Length;

                if (findIndex < (currentDocument.Text.Length))
                    results = currentDocument.Find(textToFind, findIndex, RichTextBoxFinds.None);
                else if (findIndex == (currentDocument.Text.Length))
                    results = currentDocument.Find(textToFind, 0, RichTextBoxFinds.None);
            }
            else
            {
                findIndex = currentDocument.SelectionStart + currentDocument.SelectedText.Length;

                if (findIndex < (currentDocument.Text.Length))
                    results = currentDocument.Find(textToFind, findIndex, RichTextBoxFinds.None);
                else if (findIndex == (currentDocument.Text.Length))
                {
                    if (mainEditor.TabSet.SelectedIndex < (mainEditor.TabSet.TabPages.Count - 1))
                        mainEditor.TabSet.SelectedIndex++;
                    else
                        mainEditor.TabSet.SelectedIndex = 0;
                    if (0 < (currentDocument.Text.Length))
                        results = currentDocument.Find(textToFind, 0, RichTextBoxFinds.None);
                    
                }
            }
            //if the text is not found, display message
            if (results == -1)
                return false;

            return true;
        }

        //replace highlighted instance and find the next, if any (al)
        public bool replace()
        {
            if (currentDocument.SelectedText.ToLower().Equals(findTextBox.Text.ToLower()))
                currentDocument.SelectedText = replaceTextBox.Text;

            return find();
        }

        //replaced all occurrences (al)
        public void replaceAll()
        {
            currentDocument.SelectionStart = 0;
            int count = -1;
            bool replaced;

            do
            {
                replaced = replace();
                count++;
            } while (replaced);

            MessageBox.Show(count + " instances were replaced");
            findTextBox.Focus();
            findTextBox.Select(0, findTextBox.Text.Length);
        }

        public void notFoundMessage()
        {
            MessageBox.Show("Cannot find more instances of \"" + findTextBox.Text + "\"");
            findTextBox.Focus();
            findTextBox.Select(0, findTextBox.Text.Length);

        }

        public FindReplaceForm(CommandEditor ce)
        {
            mainEditor = ce;
            InitializeComponent();
            loaded = false;
        }

        #endregion


        #region Events

        private void FindReplaceForm_Activated(object sender, System.EventArgs e)
        {
            //sets the index from where to start searching
            findIndex = currentDocument.SelectionStart;
        }

        private void FindReplaceForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
                findButton_Click(sender, e);
        }
        
        private void findButton_Click(object sender, EventArgs e)
        {
            if (!find())
                notFoundMessage();
        }

        private void replaceAllButton_Click(object sender, EventArgs e)
        {
            replaceAll();
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            if (!replace())
                notFoundMessage();
        }

        private void FindReplaceForm_Load(object sender, EventArgs e)
        {
            findTextBox.Text = currentDocument.SelectedText;
            findReplaceDropDown.SelectedIndex = 0;
        }

        #endregion        

		 private void quickFindBtn_Click(object sender, EventArgs e)
		 {
			 this.initializeMode(ModeValues.quickFind);
		 }

		 private void quickReplaceBtn_Click(object sender, EventArgs e)
		 {
			 this.initializeMode(ModeValues.quickReplace);
		 }
    }
}
