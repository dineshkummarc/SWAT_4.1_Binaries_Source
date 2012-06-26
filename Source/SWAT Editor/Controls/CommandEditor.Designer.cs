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


namespace SWAT_Editor.Controls
{
  partial class CommandEditor
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
		 this.components = new System.ComponentModel.Container();
		 this.cmsTab = new System.Windows.Forms.ContextMenuStrip(this.components);
		 this.closeTabMI = new System.Windows.Forms.ToolStripMenuItem();
		 this.lblOpenNewFile = new System.Windows.Forms.Label();
		 this.tabSet = new SWAT_Editor.Controls.TabControlExtension();
		 this.cmsTab.SuspendLayout();
		 this.SuspendLayout();
		 // 
		 // cmsTab
		 // 
		 this.cmsTab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeTabMI});
		 this.cmsTab.Name = "cmsTab";
		 this.cmsTab.Size = new System.Drawing.Size(124, 26);
		 // 
		 // closeTabMI
		 // 
		 this.closeTabMI.Name = "closeTabMI";
		 this.closeTabMI.Size = new System.Drawing.Size(123, 22);
		 this.closeTabMI.Text = "Close...";
		 this.closeTabMI.Click += new System.EventHandler(this.tabClose_Click);
		 // 
		 // lblOpenNewFile
		 // 
		 this.lblOpenNewFile.Anchor = System.Windows.Forms.AnchorStyles.None;
		 this.lblOpenNewFile.AutoSize = true;
		 this.lblOpenNewFile.Location = new System.Drawing.Point(217, 203);
		 this.lblOpenNewFile.Name = "lblOpenNewFile";
		 this.lblOpenNewFile.Size = new System.Drawing.Size(124, 13);
		 this.lblOpenNewFile.TabIndex = 2;
		 this.lblOpenNewFile.Text = "To begin open a new file";
		 // 
		 // tabSet
		 // 
		 this.tabSet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						 | System.Windows.Forms.AnchorStyles.Left)
						 | System.Windows.Forms.AnchorStyles.Right)));
		 this.tabSet.ContextMenuStrip = this.cmsTab;
		 this.tabSet.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
		 this.tabSet.Location = new System.Drawing.Point(3, 3);
		 this.tabSet.Name = "tabSet";
		 this.tabSet.SelectedIndex = 0;
		 this.tabSet.Size = new System.Drawing.Size(545, 453);
		 this.tabSet.TabIndex = 0;
		 this.tabSet.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tabSet_KeyUp);
		 this.tabSet.SelectedIndexChanged += new System.EventHandler(this.tabSet_SelectedIndexChanged);
		 // 
		 // CommandEditor
		 // 
		 this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		 this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		 this.Controls.Add(this.lblOpenNewFile);
		 this.Controls.Add(this.tabSet);
		 this.Name = "CommandEditor";
		 this.Size = new System.Drawing.Size(548, 458);
		 this.cmsTab.ResumeLayout(false);
		 this.ResumeLayout(false);
		 this.PerformLayout();

    }

    #endregion

      private TabControlExtension tabSet;
      private System.Windows.Forms.Label lblOpenNewFile;
      private bool useIntellisense = true;
		private System.Windows.Forms.ContextMenuStrip cmsTab;
		private System.Windows.Forms.ToolStripMenuItem closeTabMI; //indicates if the user prepare to use intellisense or not


  }
}
