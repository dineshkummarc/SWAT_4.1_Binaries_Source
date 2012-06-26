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


namespace SWAT_Editor.Controls.DBBuilder.ColumnEditor
{
    partial class ColumnEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnEditor));
            this.columnEditorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButtonColumnssplitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.leftSplitContainer = new System.Windows.Forms.SplitContainer();
            this.hiddenLabel = new System.Windows.Forms.Label();
            this.selectedColumns = new System.Windows.Forms.ListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.selectOne = new System.Windows.Forms.Button();
            this.deselectAll = new System.Windows.Forms.Button();
            this.selectAll = new System.Windows.Forms.Button();
            this.deselectOne = new System.Windows.Forms.Button();
            this.rightSplitContainer = new System.Windows.Forms.SplitContainer();
            this.hiddenColumns = new System.Windows.Forms.ListBox();
            this.selectedLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.ButtonColumnssplitContainer.Panel1.SuspendLayout();
            this.ButtonColumnssplitContainer.Panel2.SuspendLayout();
            this.ButtonColumnssplitContainer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.leftSplitContainer.Panel1.SuspendLayout();
            this.leftSplitContainer.Panel2.SuspendLayout();
            this.leftSplitContainer.SuspendLayout();
            this.panel3.SuspendLayout();
            this.rightSplitContainer.Panel1.SuspendLayout();
            this.rightSplitContainer.Panel2.SuspendLayout();
            this.rightSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(568, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(40, 40);
            this.btnCancel.TabIndex = 39;
            this.columnEditorToolTip.SetToolTip(this.btnCancel, "Cancel");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.Location = new System.Drawing.Point(522, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(40, 40);
            this.btnOk.TabIndex = 38;
            this.columnEditorToolTip.SetToolTip(this.btnOk, "Accept");
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ButtonColumnssplitContainer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(620, 500);
            this.panel1.TabIndex = 0;
            // 
            // ButtonColumnssplitContainer
            // 
            this.ButtonColumnssplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonColumnssplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.ButtonColumnssplitContainer.Location = new System.Drawing.Point(0, 0);
            this.ButtonColumnssplitContainer.Name = "ButtonColumnssplitContainer";
            this.ButtonColumnssplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ButtonColumnssplitContainer.Panel1
            // 
            this.ButtonColumnssplitContainer.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // ButtonColumnssplitContainer.Panel2
            // 
            this.ButtonColumnssplitContainer.Panel2.Controls.Add(this.btnOk);
            this.ButtonColumnssplitContainer.Panel2.Controls.Add(this.btnCancel);
            this.ButtonColumnssplitContainer.Size = new System.Drawing.Size(620, 500);
            this.ButtonColumnssplitContainer.SplitterDistance = 442;
            this.ButtonColumnssplitContainer.TabIndex = 0;
            this.ButtonColumnssplitContainer.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.rightSplitContainer, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(620, 442);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.leftSplitContainer);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(279, 436);
            this.panel2.TabIndex = 36;
            // 
            // leftSplitContainer
            // 
            this.leftSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftSplitContainer.IsSplitterFixed = true;
            this.leftSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.leftSplitContainer.Name = "leftSplitContainer";
            this.leftSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // leftSplitContainer.Panel1
            // 
            this.leftSplitContainer.Panel1.Controls.Add(this.selectedLabel);
            // 
            // leftSplitContainer.Panel2
            // 
            this.leftSplitContainer.Panel2.Controls.Add(this.hiddenColumns);
            this.leftSplitContainer.Size = new System.Drawing.Size(279, 436);
            this.leftSplitContainer.SplitterDistance = 25;
            this.leftSplitContainer.TabIndex = 0;
            this.leftSplitContainer.TabStop = false;
            // 
            // hiddenLabel
            // 
            this.hiddenLabel.AutoSize = true;
            this.hiddenLabel.Location = new System.Drawing.Point(3, 6);
            this.hiddenLabel.Name = "hiddenLabel";
            this.hiddenLabel.Size = new System.Drawing.Size(93, 13);
            this.hiddenLabel.TabIndex = 38;
            this.hiddenLabel.Text = "Included Columns";
            // 
            // hiddenColumns
            // 
            this.hiddenColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hiddenColumns.FormattingEnabled = true;
            this.hiddenColumns.Location = new System.Drawing.Point(0, 0);
            this.hiddenColumns.Name = "hiddenColumns";
            this.hiddenColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.hiddenColumns.Size = new System.Drawing.Size(279, 407);
            this.hiddenColumns.Sorted = true;
            this.hiddenColumns.TabIndex = 39;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.selectOne);
            this.panel3.Controls.Add(this.deselectAll);
            this.panel3.Controls.Add(this.selectAll);
            this.panel3.Controls.Add(this.deselectOne);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(288, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(44, 436);
            this.panel3.TabIndex = 37;
            // 
            // deselectOne
            // 
            this.deselectOne.Image = ((System.Drawing.Image)(resources.GetObject("selectOne.Image")));
            this.deselectOne.Location = new System.Drawing.Point(6, 231);
            this.deselectOne.Name = "deselectOne";
            this.deselectOne.Size = new System.Drawing.Size(32, 32);
            this.deselectOne.TabIndex = 33;
            this.deselectOne.UseVisualStyleBackColor = true;
            this.deselectOne.Click += new System.EventHandler(this.deselectOne_Click);
            // 
            // selectAll
            // 
            this.selectAll.Image = ((System.Drawing.Image)(resources.GetObject("deselectAll.Image")));
            this.selectAll.Location = new System.Drawing.Point(6, 111);
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(32, 32);
            this.selectAll.TabIndex = 35;
            this.selectAll.UseVisualStyleBackColor = true;
            this.selectAll.Click += new System.EventHandler(this.selectAll_Click);
            // 
            // deselectAll
            // 
            this.deselectAll.Image = ((System.Drawing.Image)(resources.GetObject("selectAll.Image")));
            this.deselectAll.Location = new System.Drawing.Point(6, 293);
            this.deselectAll.Name = "deselectAll";
            this.deselectAll.Size = new System.Drawing.Size(32, 32);
            this.deselectAll.TabIndex = 32;
            this.deselectAll.UseVisualStyleBackColor = true;
            this.deselectAll.Click += new System.EventHandler(this.deselectAll_Click);
            // 
            // selectOne
            // 
            this.selectOne.Image = ((System.Drawing.Image)(resources.GetObject("deselectOne.Image")));
            this.selectOne.Location = new System.Drawing.Point(6, 170);
            this.selectOne.Name = "selectOne";
            this.selectOne.Size = new System.Drawing.Size(32, 32);
            this.selectOne.TabIndex = 34;
            this.selectOne.UseVisualStyleBackColor = true;
            this.selectOne.Click += new System.EventHandler(this.selectOne_Click);
            // 
            // rightSplitContainer
            // 
            this.rightSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightSplitContainer.IsSplitterFixed = true;
            this.rightSplitContainer.Location = new System.Drawing.Point(338, 3);
            this.rightSplitContainer.Name = "rightSplitContainer";
            this.rightSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // rightSplitContainer.Panel1
            // 
            this.rightSplitContainer.Panel1.Controls.Add(this.hiddenLabel);
            // 
            // rightSplitContainer.Panel2
            // 
            this.rightSplitContainer.Panel2.Controls.Add(this.selectedColumns);
            this.rightSplitContainer.Size = new System.Drawing.Size(279, 436);
            this.rightSplitContainer.SplitterDistance = 25;
            this.rightSplitContainer.TabIndex = 38;
            this.rightSplitContainer.TabStop = false;
            // 
            // selectedColumns
            //
            this.selectedColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedColumns.FormattingEnabled = true;
            this.selectedColumns.Location = new System.Drawing.Point(0, 0);
            this.selectedColumns.Name = "selectedColumns";
            this.selectedColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.selectedColumns.Size = new System.Drawing.Size(279, 407);
            this.selectedColumns.Sorted = true;
            this.selectedColumns.TabIndex = 39;
            // 
            // selectedLabel
            // 
            this.selectedLabel.AutoSize = true;
            this.selectedLabel.Location = new System.Drawing.Point(3, 6);
            this.selectedLabel.Name = "selectedLabel";
            this.selectedLabel.Size = new System.Drawing.Size(91, 13);
            this.selectedLabel.TabIndex = 40;
            this.selectedLabel.Text = "Available Columns";
            // 
            // ColumnEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 500);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColumnEditor";
            this.Text = "Add/Remove Columns";
            this.panel1.ResumeLayout(false);
            this.ButtonColumnssplitContainer.Panel1.ResumeLayout(false);
            this.ButtonColumnssplitContainer.Panel2.ResumeLayout(false);
            this.ButtonColumnssplitContainer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.leftSplitContainer.Panel1.ResumeLayout(false);
            this.leftSplitContainer.Panel1.PerformLayout();
            this.leftSplitContainer.Panel2.ResumeLayout(false);
            this.leftSplitContainer.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.rightSplitContainer.Panel1.ResumeLayout(false);
            this.rightSplitContainer.Panel1.PerformLayout();
            this.rightSplitContainer.Panel2.ResumeLayout(false);
            this.rightSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip columnEditorToolTip;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer ButtonColumnssplitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button deselectAll;
        private System.Windows.Forms.Button deselectOne;
        private System.Windows.Forms.Button selectOne;
        private System.Windows.Forms.Button selectAll;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.SplitContainer rightSplitContainer;
        private System.Windows.Forms.SplitContainer leftSplitContainer;
        private System.Windows.Forms.Label hiddenLabel;
        private System.Windows.Forms.ListBox hiddenColumns;
        private System.Windows.Forms.ListBox selectedColumns;
        private System.Windows.Forms.Label selectedLabel;
    }
}
