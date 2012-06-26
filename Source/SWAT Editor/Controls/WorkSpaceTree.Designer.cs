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
  partial class WorkSpaceTree
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkSpaceTree));
      this.workSpaceTreeView = new System.Windows.Forms.TreeView();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.SuspendLayout();
      // 
      // workSpaceTreeView
      // 
      this.workSpaceTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.workSpaceTreeView.ImageIndex = 0;
      this.workSpaceTreeView.ImageList = this.imageList1;
      this.workSpaceTreeView.Location = new System.Drawing.Point(0, 0);
      this.workSpaceTreeView.Name = "workSpaceTreeView";
      this.workSpaceTreeView.SelectedImageIndex = 0;
      this.workSpaceTreeView.Size = new System.Drawing.Size(262, 307);
      this.workSpaceTreeView.TabIndex = 0;
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "doc-plain-orange.png");
      this.imageList1.Images.SetKeyName(1, "folder-closed-blue.png");
      // 
      // WorkSpaceTree
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.workSpaceTreeView);
      this.Name = "WorkSpaceTree";
      this.Size = new System.Drawing.Size(265, 310);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView workSpaceTreeView;
    private System.Windows.Forms.ImageList imageList1;
  }
}
