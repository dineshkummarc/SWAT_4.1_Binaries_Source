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
  partial class CreateSnippetForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateSnippetForm));
      this.txtFunction = new System.Windows.Forms.TextBox();
      this.openfile = new System.Windows.Forms.OpenFileDialog();
      this.browseButton = new System.Windows.Forms.Button();
      this.saveButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txtFunction
      // 
      this.txtFunction.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.txtFunction.Location = new System.Drawing.Point(0, 44);
      this.txtFunction.Multiline = true;
      this.txtFunction.Name = "txtFunction";
      this.txtFunction.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtFunction.Size = new System.Drawing.Size(566, 274);
      this.txtFunction.TabIndex = 0;
      this.txtFunction.TextChanged += new System.EventHandler(this.txtFunction_TextChanged);
      // 
      // browseButton
      // 
      this.browseButton.Location = new System.Drawing.Point(12, 12);
      this.browseButton.Name = "browseButton";
      this.browseButton.Size = new System.Drawing.Size(75, 26);
      this.browseButton.TabIndex = 1;
      this.browseButton.Text = "Browse";
      this.browseButton.UseVisualStyleBackColor = true;
      this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
      // 
      // saveButton
      // 
      this.saveButton.Enabled = false;
      this.saveButton.Location = new System.Drawing.Point(479, 12);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(75, 26);
      this.saveButton.TabIndex = 4;
      this.saveButton.Text = "Save";
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // CreateSnippetForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(566, 318);
      this.Controls.Add(this.saveButton);
      this.Controls.Add(this.browseButton);
      this.Controls.Add(this.txtFunction);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CreateSnippetForm";
      this.Text = "Create Custom Snippet";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtFunction;
    private System.Windows.Forms.Button browseButton;
    private System.Windows.Forms.OpenFileDialog openfile;
    private System.Windows.Forms.Button saveButton;
    
  }
}
