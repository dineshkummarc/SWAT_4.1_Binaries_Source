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


namespace SWAT_Editor.Controls.Explorer
{
  partial class Explorer
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Explorer));
        this.treeView = new System.Windows.Forms.TreeView();
        this.imageList = new System.Windows.Forms.ImageList(this.components);
        this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
        this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        this.addDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.addAFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.contextmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.contextMenuFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.addAFileForFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.removeAFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.contextMenuDirectories = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        this.FileExplorertoolTip = new System.Windows.Forms.ToolTip(this.components);
        this.toolStripContainer1.ContentPanel.SuspendLayout();
        this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
        this.toolStripContainer1.SuspendLayout();
        this.menuStrip1.SuspendLayout();
        this.contextmenu.SuspendLayout();
        this.contextMenuFiles.SuspendLayout();
        this.contextMenuDirectories.SuspendLayout();
        this.SuspendLayout();
        // 
        // treeView
        // 
        this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.treeView.ImageIndex = 0;
        this.treeView.ImageList = this.imageList;
        this.treeView.Location = new System.Drawing.Point(0, 0);
        this.treeView.Margin = new System.Windows.Forms.Padding(0);
        this.treeView.Name = "treeView";
        this.treeView.SelectedImageIndex = 0;
        this.treeView.Size = new System.Drawing.Size(249, 267);
        this.treeView.TabIndex = 0;
        this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
        this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeExpand);
        this.treeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeCollapse);
        this.treeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeview_MouseUp);
        this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeview_MouseDown);
        // 
        // imageList
        // 
        this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
        this.imageList.TransparentColor = System.Drawing.Color.Transparent;
        this.imageList.Images.SetKeyName(0, "text-x-generic.png");
        this.imageList.Images.SetKeyName(1, "folder.png");
        // 
        // toolStripContainer1
        // 
        this.toolStripContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        // 
        // toolStripContainer1.ContentPanel
        // 
        this.toolStripContainer1.ContentPanel.Controls.Add(this.treeView);
        this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(249, 267);
        this.toolStripContainer1.Location = new System.Drawing.Point(1, 0);
        this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(0);
        this.toolStripContainer1.Name = "toolStripContainer1";
        this.toolStripContainer1.Size = new System.Drawing.Size(249, 291);
        this.toolStripContainer1.TabIndex = 2;
        this.toolStripContainer1.Text = "toolStripContainer1";
        // 
        // toolStripContainer1.TopToolStripPanel
        // 
        this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
        // 
        // menuStrip1
        // 
        this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
        this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
        this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDirectoryToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.addAFileToolStripMenuItem});
        this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        this.menuStrip1.Name = "menuStrip1";
        this.menuStrip1.ShowItemToolTips = true;
        this.menuStrip1.Size = new System.Drawing.Size(249, 24);
        this.menuStrip1.TabIndex = 0;
        this.menuStrip1.Text = "menuStrip1";
        // 
        // addDirectoryToolStripMenuItem
        // 
        this.addDirectoryToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this.addDirectoryToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addDirectoryToolStripMenuItem.Image")));
        this.addDirectoryToolStripMenuItem.Name = "addDirectoryToolStripMenuItem";
        this.addDirectoryToolStripMenuItem.Size = new System.Drawing.Size(28, 20);
        this.addDirectoryToolStripMenuItem.ToolTipText = "Add folder to explorer...";
        this.addDirectoryToolStripMenuItem.Click += new System.EventHandler(this.addDirectory_Click);
        // 
        // refreshToolStripMenuItem
        // 
        this.refreshToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this.refreshToolStripMenuItem.Image = global::SWAT_Editor.Properties.Resources.view_refresh;
        this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
        this.refreshToolStripMenuItem.Size = new System.Drawing.Size(28, 20);
        this.refreshToolStripMenuItem.ToolTipText = "Refresh directories";
        this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
        // 
        // addAFileToolStripMenuItem
        // 
        this.addAFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addAFileToolStripMenuItem.Image")));
        this.addAFileToolStripMenuItem.Name = "addAFileToolStripMenuItem";
        this.addAFileToolStripMenuItem.Size = new System.Drawing.Size(28, 20);
        this.addAFileToolStripMenuItem.ToolTipText = "Add a file...";
        this.addAFileToolStripMenuItem.Click += new System.EventHandler(this.addAFileToolStripMenuItem_Click);
        // 
        // contextmenu
        // 
        this.contextmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem});
        this.contextmenu.Name = "contextmenu";
        this.contextmenu.Size = new System.Drawing.Size(125, 48);
        // 
        // addToolStripMenuItem
        // 
        this.addToolStripMenuItem.Name = "addToolStripMenuItem";
        this.addToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
        this.addToolStripMenuItem.Text = "Add...";
        // 
        // removeToolStripMenuItem
        // 
        this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
        this.removeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
        this.removeToolStripMenuItem.Text = "Remove";
        this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeContextMenu_Click);
        // 
        // contextMenuFiles
        // 
        this.contextMenuFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAFileForFilesMenuItem,
            this.removeAFileMenuItem});
        this.contextMenuFiles.Name = "contextMenuFiles";
        this.contextMenuFiles.Size = new System.Drawing.Size(143, 48);
        // 
        // addAFileForFilesMenuItem
        // 
        this.addAFileForFilesMenuItem.Name = "addAFileForFilesMenuItem";
        this.addAFileForFilesMenuItem.Size = new System.Drawing.Size(142, 22);
        this.addAFileForFilesMenuItem.Text = "Add a file...";
        this.addAFileForFilesMenuItem.Click += new System.EventHandler(this.addToFileParentDirMenuItem_Click);
        // 
        // removeAFileMenuItem
        // 
        this.removeAFileMenuItem.Name = "removeAFileMenuItem";
        this.removeAFileMenuItem.Size = new System.Drawing.Size(142, 22);
        this.removeAFileMenuItem.Text = "Remove";
        this.removeAFileMenuItem.Click += new System.EventHandler(this.removeContextMenu_Click);
        // 
        // contextMenuDirectories
        // 
        this.contextMenuDirectories.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
        this.contextMenuDirectories.Name = "contextMenuDirectories";
        this.contextMenuDirectories.Size = new System.Drawing.Size(143, 26);
        // 
        // toolStripMenuItem1
        // 
        this.toolStripMenuItem1.Name = "toolStripMenuItem1";
        this.toolStripMenuItem1.Size = new System.Drawing.Size(142, 22);
        this.toolStripMenuItem1.Text = "Add a file...";
        this.toolStripMenuItem1.Click += new System.EventHandler(this.addToDirectoryMenuItem_Click);
        // 
        // Explorer
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.toolStripContainer1);
        this.Margin = new System.Windows.Forms.Padding(0);
        this.Name = "Explorer";
        this.Size = new System.Drawing.Size(250, 292);
        this.toolStripContainer1.ContentPanel.ResumeLayout(false);
        this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
        this.toolStripContainer1.TopToolStripPanel.PerformLayout();
        this.toolStripContainer1.ResumeLayout(false);
        this.toolStripContainer1.PerformLayout();
        this.menuStrip1.ResumeLayout(false);
        this.menuStrip1.PerformLayout();
        this.contextmenu.ResumeLayout(false);
        this.contextMenuFiles.ResumeLayout(false);
        this.contextMenuDirectories.ResumeLayout(false);
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView treeView;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.ToolStripContainer toolStripContainer1;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem addDirectoryToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
      private System.Windows.Forms.ContextMenuStrip contextmenu;
      private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
      private System.Windows.Forms.ContextMenuStrip contextMenuFiles;
      private System.Windows.Forms.ContextMenuStrip contextMenuDirectories;
      private System.Windows.Forms.ToolStripMenuItem addAFileForFilesMenuItem;
      private System.Windows.Forms.ToolStripMenuItem removeAFileMenuItem;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem addAFileToolStripMenuItem;
      private System.Windows.Forms.ToolTip FileExplorertoolTip;
  }
}
