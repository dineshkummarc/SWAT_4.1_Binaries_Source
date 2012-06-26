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


namespace SWAT_Editor.Controls.Options_Control
{
    partial class OptionsControl
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("SWAT Settings");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Editor Settings");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Browser Settings");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Database Settings");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Fitnesse Settings");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Screenshot Settings");
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.CategoriesTreeView = new System.Windows.Forms.TreeView();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.settingsDataEntityBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(4, 4);
            this.mainSplitContainer.Margin = new System.Windows.Forms.Padding(4);
            this.mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.CategoriesTreeView);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.AutoScroll = true;
            this.mainSplitContainer.Panel2.Controls.Add(this.CancelButton);
            this.mainSplitContainer.Panel2.Controls.Add(this.OkButton);
            this.mainSplitContainer.Size = new System.Drawing.Size(728, 466);
            this.mainSplitContainer.SplitterDistance = 196;
            this.mainSplitContainer.SplitterWidth = 5;
            this.mainSplitContainer.TabIndex = 0;
            // 
            // CategoriesTreeView
            // 
            this.CategoriesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CategoriesTreeView.Location = new System.Drawing.Point(0, 0);
            this.CategoriesTreeView.Margin = new System.Windows.Forms.Padding(4);
            this.CategoriesTreeView.Name = "CategoriesTreeView";
            treeNode1.Name = "SWAT Settings";
            treeNode1.Text = "SWAT Settings";
            treeNode2.Name = "Editor Settings";
            treeNode2.Text = "Editor Settings";
            treeNode3.Name = "Browser Settings";
            treeNode3.Text = "Browser Settings";
            treeNode4.Name = "Database Settings";
            treeNode4.Text = "Database Settings";
            treeNode5.Name = "Fitnesse Settings";
            treeNode5.Text = "Fitnesse Settings";
            treeNode6.Name = "Screenshot Settings";
            treeNode6.Text = "Screenshot Settings";
            this.CategoriesTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6});
            this.CategoriesTreeView.Size = new System.Drawing.Size(196, 466);
            this.CategoriesTreeView.TabIndex = 0;
            this.CategoriesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.CategoryTree_AfterSelect);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(421, 431);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(91, 28);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(313, 431);
            this.OkButton.Margin = new System.Windows.Forms.Padding(4);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(91, 28);
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // settingsDataEntityBindingSource
            // 
            this.settingsDataEntityBindingSource.DataSource = typeof(SWAT_Editor.Controls.Options_Control.SettingsDataEntity);
            // 
            // OptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(736, 474);
            this.Name = "OptionsControl";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Size = new System.Drawing.Size(736, 474);
            this.Load += new System.EventHandler(this.OptionsControl_Load);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        public System.Windows.Forms.BindingSource settingsDataEntityBindingSource;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.TreeView CategoriesTreeView;
    }
}
