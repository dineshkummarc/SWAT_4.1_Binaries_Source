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


namespace SWAT_Editor
{
    partial class FindReplaceForm
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
			  System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindReplaceForm));
			  this.findlabel = new System.Windows.Forms.Label();
			  this.findTextBox = new System.Windows.Forms.TextBox();
			  this.findButton = new System.Windows.Forms.Button();
			  this.findReplaceDropDown = new System.Windows.Forms.ComboBox();
			  this.findModifierLabel = new System.Windows.Forms.Label();
			  this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			  this.quickFindBtn = new System.Windows.Forms.ToolStripButton();
			  this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			  this.quickReplaceBtn = new System.Windows.Forms.ToolStripButton();
			  this.replaceTextBox = new System.Windows.Forms.TextBox();
			  this.replaceAllButton = new System.Windows.Forms.Button();
			  this.replaceButton = new System.Windows.Forms.Button();
			  this.replaceGB = new System.Windows.Forms.GroupBox();
			  this.toolStrip1.SuspendLayout();
			  this.replaceGB.SuspendLayout();
			  this.SuspendLayout();
			  // 
			  // findlabel
			  // 
			  this.findlabel.AutoSize = true;
			  this.findlabel.Location = new System.Drawing.Point(6, 34);
			  this.findlabel.Name = "findlabel";
			  this.findlabel.Size = new System.Drawing.Size(56, 13);
			  this.findlabel.TabIndex = 0;
			  this.findlabel.Text = "Find what:";
			  // 
			  // findTextBox
			  // 
			  this.findTextBox.Location = new System.Drawing.Point(9, 50);
			  this.findTextBox.Name = "findTextBox";
			  this.findTextBox.Size = new System.Drawing.Size(273, 20);
			  this.findTextBox.TabIndex = 0;
			  this.findTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyDown);
			  // 
			  // findButton
			  // 
			  this.findButton.Location = new System.Drawing.Point(202, 121);
			  this.findButton.Name = "findButton";
			  this.findButton.Size = new System.Drawing.Size(80, 21);
			  this.findButton.TabIndex = 2;
			  this.findButton.Text = "Find Next";
			  this.findButton.UseVisualStyleBackColor = true;
			  this.findButton.Click += new System.EventHandler(this.findButton_Click);
			  this.findButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyDown);
			  // 
			  // findReplaceDropDown
			  // 
			  this.findReplaceDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			  this.findReplaceDropDown.FormattingEnabled = true;
			  this.findReplaceDropDown.Items.AddRange(new object[] {
            "Current File",
            "All Files"});
			  this.findReplaceDropDown.Location = new System.Drawing.Point(9, 92);
			  this.findReplaceDropDown.Name = "findReplaceDropDown";
			  this.findReplaceDropDown.Size = new System.Drawing.Size(273, 21);
			  this.findReplaceDropDown.TabIndex = 1;
			  // 
			  // findModifierLabel
			  // 
			  this.findModifierLabel.AutoSize = true;
			  this.findModifierLabel.Location = new System.Drawing.Point(6, 76);
			  this.findModifierLabel.Name = "findModifierLabel";
			  this.findModifierLabel.Size = new System.Drawing.Size(45, 13);
			  this.findModifierLabel.TabIndex = 8;
			  this.findModifierLabel.Text = "Look in:";
			  // 
			  // toolStrip1
			  // 
			  this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			  this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quickFindBtn,
            this.toolStripSeparator1,
            this.quickReplaceBtn});
			  this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			  this.toolStrip1.Name = "toolStrip1";
			  this.toolStrip1.Size = new System.Drawing.Size(294, 25);
			  this.toolStrip1.TabIndex = 9;
			  this.toolStrip1.Text = "toolStrip1";
			  // 
			  // quickFindBtn
			  // 
			  this.quickFindBtn.AutoSize = false;
			  this.quickFindBtn.Image = global::SWAT_Editor.Properties.Resources.edit_find;
			  this.quickFindBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.quickFindBtn.Name = "quickFindBtn";
			  this.quickFindBtn.Size = new System.Drawing.Size(140, 22);
			  this.quickFindBtn.Text = "Quick Find";
			  this.quickFindBtn.Click += new System.EventHandler(this.quickFindBtn_Click);
			  // 
			  // toolStripSeparator1
			  // 
			  this.toolStripSeparator1.Name = "toolStripSeparator1";
			  this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			  // 
			  // quickReplaceBtn
			  // 
			  this.quickReplaceBtn.AutoSize = false;
			  this.quickReplaceBtn.Image = ((System.Drawing.Image)(resources.GetObject("quickReplaceBtn.Image")));
			  this.quickReplaceBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.quickReplaceBtn.Name = "quickReplaceBtn";
			  this.quickReplaceBtn.Size = new System.Drawing.Size(130, 22);
			  this.quickReplaceBtn.Text = "Quick Replace";
			  this.quickReplaceBtn.Click += new System.EventHandler(this.quickReplaceBtn_Click);
			  // 
			  // replaceTextBox
			  // 
			  this.replaceTextBox.Location = new System.Drawing.Point(4, 19);
			  this.replaceTextBox.Name = "replaceTextBox";
			  this.replaceTextBox.Size = new System.Drawing.Size(273, 20);
			  this.replaceTextBox.TabIndex = 0;
			  this.replaceTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyDown);
			  // 
			  // replaceAllButton
			  // 
			  this.replaceAllButton.Location = new System.Drawing.Point(196, 45);
			  this.replaceAllButton.Name = "replaceAllButton";
			  this.replaceAllButton.Size = new System.Drawing.Size(81, 21);
			  this.replaceAllButton.TabIndex = 2;
			  this.replaceAllButton.Text = "Replace All";
			  this.replaceAllButton.UseVisualStyleBackColor = true;
			  this.replaceAllButton.Click += new System.EventHandler(this.replaceAllButton_Click);
			  this.replaceAllButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyDown);
			  // 
			  // replaceButton
			  // 
			  this.replaceButton.Location = new System.Drawing.Point(110, 45);
			  this.replaceButton.Name = "replaceButton";
			  this.replaceButton.Size = new System.Drawing.Size(80, 21);
			  this.replaceButton.TabIndex = 1;
			  this.replaceButton.Text = "Replace";
			  this.replaceButton.UseVisualStyleBackColor = true;
			  this.replaceButton.Click += new System.EventHandler(this.replaceButton_Click);
			  this.replaceButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyDown);
			  // 
			  // replaceGB
			  // 
			  this.replaceGB.Controls.Add(this.replaceTextBox);
			  this.replaceGB.Controls.Add(this.replaceButton);
			  this.replaceGB.Controls.Add(this.replaceAllButton);
			  this.replaceGB.Location = new System.Drawing.Point(5, 144);
			  this.replaceGB.Name = "replaceGB";
			  this.replaceGB.Size = new System.Drawing.Size(285, 73);
			  this.replaceGB.TabIndex = 3;
			  this.replaceGB.TabStop = false;
			  this.replaceGB.Text = "Replace With:";
			  // 
			  // FindReplaceForm
			  // 
			  this.AcceptButton = this.findButton;
			  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			  this.ClientSize = new System.Drawing.Size(294, 226);
			  this.Controls.Add(this.replaceGB);
			  this.Controls.Add(this.toolStrip1);
			  this.Controls.Add(this.findModifierLabel);
			  this.Controls.Add(this.findReplaceDropDown);
			  this.Controls.Add(this.findButton);
			  this.Controls.Add(this.findTextBox);
			  this.Controls.Add(this.findlabel);
			  this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			  this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			  this.MaximizeBox = false;
			  this.Name = "FindReplaceForm";
			  this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			  this.Text = "Find and Replace";
			  this.Load += new System.EventHandler(this.FindReplaceForm_Load);
			  this.Activated += new System.EventHandler(this.FindReplaceForm_Activated);
			  this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindReplaceForm_KeyDown);
			  this.toolStrip1.ResumeLayout(false);
			  this.toolStrip1.PerformLayout();
			  this.replaceGB.ResumeLayout(false);
			  this.replaceGB.PerformLayout();
			  this.ResumeLayout(false);
			  this.PerformLayout();

        }

        #endregion

		 private System.Windows.Forms.Label findlabel;
		 private System.Windows.Forms.TextBox findTextBox;
		 private System.Windows.Forms.Button findButton;
        private System.Windows.Forms.ComboBox findReplaceDropDown;
		 private System.Windows.Forms.Label findModifierLabel;
		 private System.Windows.Forms.ToolStrip toolStrip1;
		 private System.Windows.Forms.ToolStripButton quickFindBtn;
		 private System.Windows.Forms.ToolStripButton quickReplaceBtn;
		 private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		 private System.Windows.Forms.TextBox replaceTextBox;
		 private System.Windows.Forms.Button replaceAllButton;
		 private System.Windows.Forms.Button replaceButton;
		 private System.Windows.Forms.GroupBox replaceGB;
    }
}
