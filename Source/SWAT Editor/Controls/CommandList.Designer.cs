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
  partial class CommandList
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
		 this.lstCompletedCommands = new System.Windows.Forms.ListView();
		 this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
		 this.cmsMainCopyMI = new System.Windows.Forms.ToolStripMenuItem();
		 this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		 this.goToLineMI = new System.Windows.Forms.ToolStripMenuItem();
		 this.insertBreakpointMI = new System.Windows.Forms.ToolStripMenuItem();
		 this.cmsMain.SuspendLayout();
		 this.SuspendLayout();
		 // 
		 // lstCompletedCommands
		 // 
		 this.lstCompletedCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						 | System.Windows.Forms.AnchorStyles.Left)
						 | System.Windows.Forms.AnchorStyles.Right)));
		 this.lstCompletedCommands.ContextMenuStrip = this.cmsMain;
		 this.lstCompletedCommands.FullRowSelect = true;
		 this.lstCompletedCommands.LabelEdit = true;
		 this.lstCompletedCommands.Location = new System.Drawing.Point(0, 3);
		 this.lstCompletedCommands.Name = "lstCompletedCommands";
		 this.lstCompletedCommands.Size = new System.Drawing.Size(466, 247);
		 this.lstCompletedCommands.TabIndex = 1;
		 this.lstCompletedCommands.UseCompatibleStateImageBehavior = false;
		 this.lstCompletedCommands.DoubleClick += new System.EventHandler(this.lstCompletedCommands_DoubleClick);
		 // 
		 // cmsMain
		 // 
		 this.cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsMainCopyMI,
            this.toolStripSeparator1,
            this.goToLineMI,
            this.insertBreakpointMI});
		 this.cmsMain.Name = "cmsMain";
		 this.cmsMain.Size = new System.Drawing.Size(250, 98);
		 this.cmsMain.Opening += new System.ComponentModel.CancelEventHandler(this.cmsMain_Opening);
		 // 
		 // cmsMainCopyMI
		 // 
		 this.cmsMainCopyMI.Name = "cmsMainCopyMI";
		 this.cmsMainCopyMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
		 this.cmsMainCopyMI.Size = new System.Drawing.Size(204, 22);
		 this.cmsMainCopyMI.Text = "&Copy";
		 this.cmsMainCopyMI.Click += new System.EventHandler(this.cmsMainCopyMI_Click);
		 // 
		 // toolStripSeparator1
		 // 
		 this.toolStripSeparator1.Name = "toolStripSeparator1";
		 this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
		 // 
		 // goToLineMI
		 // 
		 this.goToLineMI.Name = "goToLineMI";
		 this.goToLineMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
		 this.goToLineMI.Size = new System.Drawing.Size(204, 22);
		 this.goToLineMI.Text = "&Go To Line";
		 this.goToLineMI.Click += new System.EventHandler(this.goToLineMI_Click);
		 // 
		 // insertBreakpointMI
		 // 
		 this.insertBreakpointMI.Name = "insertBreakpointMI";
		 this.insertBreakpointMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
		 this.insertBreakpointMI.Size = new System.Drawing.Size(249, 22);
		 this.insertBreakpointMI.Text = "Insert/Remove &Breakpoint";
		 this.insertBreakpointMI.Click += new System.EventHandler(this.lstCompletedCommands_DoubleClick);
		 // 
		 // CommandList
		 // 
		 this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		 this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		 this.Controls.Add(this.lstCompletedCommands);
		 this.Name = "CommandList";
		 this.Size = new System.Drawing.Size(469, 253);
		 this.cmsMain.ResumeLayout(false);
		 this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView lstCompletedCommands;
	  private System.Windows.Forms.ContextMenuStrip cmsMain;
	  private System.Windows.Forms.ToolStripMenuItem cmsMainCopyMI;
	  private System.Windows.Forms.ToolStripMenuItem goToLineMI;
	  private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	  private System.Windows.Forms.ToolStripMenuItem insertBreakpointMI;
  }
}
