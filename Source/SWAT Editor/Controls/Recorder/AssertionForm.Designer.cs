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


namespace SWAT_Editor.Recorder
{
    partial class AssertionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssertionForm));
            this.okButton = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.cancelButton = new System.Windows.Forms.Button();
            this.assertionBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.attribValueBox = new System.Windows.Forms.RichTextBox();
            this.addAssertBut = new System.Windows.Forms.Button();
            this.paramListBox = new System.Windows.Forms.ListBox();
            this.showEmpty = new System.Windows.Forms.CheckBox();
            this.assertExistButton = new System.Windows.Forms.RadioButton();
            this.assertNotExistButton = new System.Windows.Forms.RadioButton();
            this.tagLabel = new System.Windows.Forms.Label();
            this.tagTextLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(25, 349);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(130, 349);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // assertionBox
            // 
            this.assertionBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.assertionBox.Location = new System.Drawing.Point(130, 68);
            this.assertionBox.Name = "assertionBox";
            this.assertionBox.Size = new System.Drawing.Size(347, 112);
            this.assertionBox.TabIndex = 3;
            this.assertionBox.Text = "";
            this.assertionBox.TextChanged += new System.EventHandler(this.assertionBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Attributes";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(254, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Assertion";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(234, 183);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Attribute Value";
            // 
            // attribValueBox
            // 
            this.attribValueBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.attribValueBox.Location = new System.Drawing.Point(131, 206);
            this.attribValueBox.Name = "attribValueBox";
            this.attribValueBox.Size = new System.Drawing.Size(346, 60);
            this.attribValueBox.TabIndex = 7;
            this.attribValueBox.Text = "";
            this.attribValueBox.TextChanged += new System.EventHandler(this.attribValueBox_TextChanged);
            // 
            // addAssertBut
            // 
            this.addAssertBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addAssertBut.Location = new System.Drawing.Point(238, 305);
            this.addAssertBut.Name = "addAssertBut";
            this.addAssertBut.Size = new System.Drawing.Size(115, 23);
            this.addAssertBut.TabIndex = 8;
            this.addAssertBut.Text = "Add to Assertion";
            this.addAssertBut.UseVisualStyleBackColor = true;
            this.addAssertBut.Click += new System.EventHandler(this.addAssertBut_Click);
            // 
            // paramListBox
            // 
            this.paramListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.paramListBox.FormattingEnabled = true;
            this.paramListBox.HorizontalScrollbar = true;
            this.paramListBox.Location = new System.Drawing.Point(3, 68);
            this.paramListBox.Name = "paramListBox";
            this.paramListBox.Size = new System.Drawing.Size(121, 225);
            this.paramListBox.Sorted = true;
            this.paramListBox.TabIndex = 9;
            this.paramListBox.SelectedIndexChanged += new System.EventHandler(this.paramListBox_SelectedIndexChanged);
            // 
            // showEmpty
            // 
            this.showEmpty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.showEmpty.AutoSize = true;
            this.showEmpty.Location = new System.Drawing.Point(3, 300);
            this.showEmpty.Name = "showEmpty";
            this.showEmpty.Size = new System.Drawing.Size(66, 17);
            this.showEmpty.TabIndex = 11;
            this.showEmpty.Text = "Show all";
            this.showEmpty.UseVisualStyleBackColor = true;
            this.showEmpty.CheckedChanged += new System.EventHandler(this.showEmpty_CheckedChanged);
            // 
            // assertExistButton
            // 
            this.assertExistButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.assertExistButton.AutoSize = true;
            this.assertExistButton.Checked = true;
            this.assertExistButton.Location = new System.Drawing.Point(144, 272);
            this.assertExistButton.Name = "assertExistButton";
            this.assertExistButton.Size = new System.Drawing.Size(119, 17);
            this.assertExistButton.TabIndex = 12;
            this.assertExistButton.TabStop = true;
            this.assertExistButton.Text = "AssertElementExists";
            this.assertExistButton.UseVisualStyleBackColor = true;
            this.assertExistButton.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // assertNotExistButton
            // 
            this.assertNotExistButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.assertNotExistButton.AutoSize = true;
            this.assertNotExistButton.Location = new System.Drawing.Point(321, 272);
            this.assertNotExistButton.Name = "assertNotExistButton";
            this.assertNotExistButton.Size = new System.Drawing.Size(156, 17);
            this.assertNotExistButton.TabIndex = 13;
            this.assertNotExistButton.Text = "AssertElementDoesNotExist";
            this.assertNotExistButton.UseVisualStyleBackColor = true;
            // 
            // tagLabel
            // 
            this.tagLabel.AutoSize = true;
            this.tagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tagLabel.Location = new System.Drawing.Point(50, 9);
            this.tagLabel.Name = "tagLabel";
            this.tagLabel.Size = new System.Drawing.Size(160, 20);
            this.tagLabel.TabIndex = 14;
            this.tagLabel.Text = "Current Tag Name:";
            // 
            // tagTextLabel
            // 
            this.tagTextLabel.AutoSize = true;
            this.tagTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tagTextLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.tagTextLabel.Location = new System.Drawing.Point(216, 9);
            this.tagTextLabel.Name = "tagTextLabel";
            this.tagTextLabel.Size = new System.Drawing.Size(0, 20);
            this.tagTextLabel.TabIndex = 16;
            // 
            // AssertionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 380);
            this.Controls.Add(this.tagTextLabel);
            this.Controls.Add(this.tagLabel);
            this.Controls.Add(this.assertNotExistButton);
            this.Controls.Add(this.assertExistButton);
            this.Controls.Add(this.showEmpty);
            this.Controls.Add(this.paramListBox);
            this.Controls.Add(this.addAssertBut);
            this.Controls.Add(this.attribValueBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.assertionBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AssertionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Elements to Assert";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AssertionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RichTextBox assertionBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox attribValueBox;
        private System.Windows.Forms.Button addAssertBut;
        private System.Windows.Forms.ListBox paramListBox;
        private System.Windows.Forms.CheckBox showEmpty;
        private System.Windows.Forms.RadioButton assertExistButton;
        private System.Windows.Forms.RadioButton assertNotExistButton;
        private System.Windows.Forms.Label tagLabel;
        private System.Windows.Forms.Label tagTextLabel;
    }
}
