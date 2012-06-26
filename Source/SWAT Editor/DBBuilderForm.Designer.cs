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
  partial class DBBuilderForm
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
		 this.dbBuilderWizard1 = new SWAT_Editor.Controls.DBBuilder.DBBuilder();
		 this.SuspendLayout();
		 // 
		 // dbBuilderWizard1
		 // 
		 this.dbBuilderWizard1.AutoSize = true;
		 this.dbBuilderWizard1.DataConnection = null;
		 this.dbBuilderWizard1.Dock = System.Windows.Forms.DockStyle.Fill;
		 this.dbBuilderWizard1.Location = new System.Drawing.Point(0, 0);
		 this.dbBuilderWizard1.Name = "dbBuilderWizard1";
		 this.dbBuilderWizard1.ShowConnectionSettings = true;
		 this.dbBuilderWizard1.Size = new System.Drawing.Size(743, 462);
		 this.dbBuilderWizard1.TabIndex = 0;
		 // 
		 // DBBuilderForm
		 // 
		 this.AccessibleRole = System.Windows.Forms.AccessibleRole.Pane;
		 this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		 this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		 this.ClientSize = new System.Drawing.Size(743, 462);
		 this.Controls.Add(this.dbBuilderWizard1);
		 this.MinimumSize = new System.Drawing.Size(751, 496);
		 this.Name = "DBBuilderForm";
		 this.Text = "Data Access Command Builder";
		 this.Resize += new System.EventHandler(this.DBBuilderForm_Resize);
		 this.ResumeLayout(false);
		 this.PerformLayout();

    }

    #endregion

      private SWAT_Editor.Controls.DBBuilder.DBBuilder dbBuilderWizard1;






}
}
