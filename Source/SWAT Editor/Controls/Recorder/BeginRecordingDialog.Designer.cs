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


namespace SWAT_Editor.Controls.Recorder
{
    partial class BeginRecordingDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			  System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeginRecordingDialog));
			  this.newBrowserButton = new System.Windows.Forms.RadioButton();
			  this.attachBrowserButton = new System.Windows.Forms.RadioButton();
			  this.browserCheckedListBox = new System.Windows.Forms.CheckedListBox();
			  this.okButton = new System.Windows.Forms.Button();
			  this.cancelButton = new System.Windows.Forms.Button();
			  this.selectAllCheckBox = new System.Windows.Forms.CheckBox();
			  this.labelIEBrowserWarning = new System.Windows.Forms.Label();
			  this.SuspendLayout();
			  // 
			  // newBrowserButton
			  // 
			  this.newBrowserButton.AutoSize = true;
			  this.newBrowserButton.Checked = true;
			  this.newBrowserButton.Location = new System.Drawing.Point(12, 10);
			  this.newBrowserButton.Name = "newBrowserButton";
			  this.newBrowserButton.Size = new System.Drawing.Size(88, 17);
			  this.newBrowserButton.TabIndex = 0;
			  this.newBrowserButton.TabStop = true;
			  this.newBrowserButton.Text = "New Browser";
			  this.newBrowserButton.UseVisualStyleBackColor = true;
			  this.newBrowserButton.CheckedChanged += new System.EventHandler(this.newBrowserButton_CheckedChanged);
			  // 
			  // attachBrowserButton
			  // 
			  this.attachBrowserButton.AutoSize = true;
			  this.attachBrowserButton.Location = new System.Drawing.Point(12, 33);
			  this.attachBrowserButton.Name = "attachBrowserButton";
			  this.attachBrowserButton.Size = new System.Drawing.Size(118, 17);
			  this.attachBrowserButton.TabIndex = 1;
			  this.attachBrowserButton.TabStop = true;
			  this.attachBrowserButton.Text = "Attach to a Browser";
			  this.attachBrowserButton.UseVisualStyleBackColor = true;
			  this.attachBrowserButton.CheckedChanged += new System.EventHandler(this.attachBrowserButton_CheckedChanged);
			  // 
			  // browserCheckedListBox
			  // 
			  this.browserCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							  | System.Windows.Forms.AnchorStyles.Right)));
			  this.browserCheckedListBox.CheckOnClick = true;
			  this.browserCheckedListBox.FormattingEnabled = true;
			  this.browserCheckedListBox.Location = new System.Drawing.Point(11, 56);
			  this.browserCheckedListBox.Name = "browserCheckedListBox";
			  this.browserCheckedListBox.Size = new System.Drawing.Size(368, 124);
			  this.browserCheckedListBox.TabIndex = 2;
			  this.browserCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.browserCheckedListBox_SelectedIndexChanged);
			  // 
			  // okButton
			  // 
			  this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			  this.okButton.Location = new System.Drawing.Point(223, 188);
			  this.okButton.Name = "okButton";
			  this.okButton.Size = new System.Drawing.Size(75, 23);
			  this.okButton.TabIndex = 3;
			  this.okButton.Text = "OK";
			  this.okButton.UseVisualStyleBackColor = true;
			  this.okButton.Click += new System.EventHandler(this.okButton_Click);
			  // 
			  // cancelButton
			  // 
			  this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			  this.cancelButton.Location = new System.Drawing.Point(304, 188);
			  this.cancelButton.Name = "cancelButton";
			  this.cancelButton.Size = new System.Drawing.Size(75, 23);
			  this.cancelButton.TabIndex = 4;
			  this.cancelButton.Text = "Cancel";
			  this.cancelButton.UseVisualStyleBackColor = true;
			  // 
			  // selectAllCheckBox
			  // 
			  this.selectAllCheckBox.AutoSize = true;
			  this.selectAllCheckBox.Location = new System.Drawing.Point(169, 33);
			  this.selectAllCheckBox.Name = "selectAllCheckBox";
			  this.selectAllCheckBox.Size = new System.Drawing.Size(76, 17);
			  this.selectAllCheckBox.TabIndex = 5;
			  this.selectAllCheckBox.Text = "Select All?";
			  this.selectAllCheckBox.UseVisualStyleBackColor = true;
			  this.selectAllCheckBox.CheckedChanged += new System.EventHandler(this.selectAllCheckBox_CheckedChanged);
			  // 
			  // labelIEBrowserWarning
			  // 
			  this.labelIEBrowserWarning.AutoSize = true;
			  this.labelIEBrowserWarning.Location = new System.Drawing.Point(12, 6);
			  this.labelIEBrowserWarning.Name = "labelIEBrowserWarning";
			  this.labelIEBrowserWarning.Size = new System.Drawing.Size(0, 13);
			  this.labelIEBrowserWarning.TabIndex = 6;
			  // 
			  // BeginRecordingDialog
			  // 
			  this.AcceptButton = this.okButton;
			  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			  this.CancelButton = this.cancelButton;
			  this.ClientSize = new System.Drawing.Size(389, 219);
			  this.Controls.Add(this.labelIEBrowserWarning);
			  this.Controls.Add(this.selectAllCheckBox);
			  this.Controls.Add(this.cancelButton);
			  this.Controls.Add(this.okButton);
			  this.Controls.Add(this.browserCheckedListBox);
			  this.Controls.Add(this.attachBrowserButton);
			  this.Controls.Add(this.newBrowserButton);
			  this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			  this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			  this.MaximizeBox = false;
			  this.MinimizeBox = false;
			  this.Name = "BeginRecordingDialog";
			  this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			  this.Text = "Initialize Recorder";
			  this.Load += new System.EventHandler(this.BeginRecordingDialog_Load);
			  this.ResumeLayout(false);
			  this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton newBrowserButton;
        private System.Windows.Forms.RadioButton attachBrowserButton;
        private System.Windows.Forms.CheckedListBox browserCheckedListBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox selectAllCheckBox;
        private System.Windows.Forms.Label labelIEBrowserWarning;
    }
}
