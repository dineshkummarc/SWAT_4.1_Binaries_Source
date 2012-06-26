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
  partial class OpenWindows
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenWindows));
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.btnCopyToClipBoard = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.listBox1);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(463, 242);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Open Browser Windows";
      // 
      // listBox1
      // 
      this.listBox1.FormattingEnabled = true;
      this.listBox1.Location = new System.Drawing.Point(6, 19);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new System.Drawing.Size(451, 212);
      this.listBox1.TabIndex = 0;
      // 
      // btnCopyToClipBoard
      // 
      this.btnCopyToClipBoard.Location = new System.Drawing.Point(367, 264);
      this.btnCopyToClipBoard.Name = "btnCopyToClipBoard";
      this.btnCopyToClipBoard.Size = new System.Drawing.Size(102, 23);
      this.btnCopyToClipBoard.TabIndex = 1;
      this.btnCopyToClipBoard.Text = "Copy to clipboard";
      this.btnCopyToClipBoard.UseVisualStyleBackColor = true;
      this.btnCopyToClipBoard.Click += new System.EventHandler(this.btnCopyToClipBoard_Click);
      // 
      // OpenWindows
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(487, 299);
      this.Controls.Add(this.btnCopyToClipBoard);
      this.Controls.Add(this.groupBox1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "OpenWindows";
      this.Text = "Open Windows";
      this.Load += new System.EventHandler(this.OpenWindows_Load);
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.Button btnCopyToClipBoard;
  }
}
