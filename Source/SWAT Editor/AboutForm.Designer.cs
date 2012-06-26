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
  partial class AboutForm
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
        this.label1 = new System.Windows.Forms.Label();
        this.pictureBox1 = new System.Windows.Forms.PictureBox();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.companyInfo = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        this.SuspendLayout();
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.BackColor = System.Drawing.Color.Transparent;
        this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label1.Location = new System.Drawing.Point(100, 9);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(215, 37);
        this.label1.TabIndex = 2;
        this.label1.Text = "SWAT Editor";
        // 
        // pictureBox1
        // 
        this.pictureBox1.Image = global::SWAT_Editor.Properties.Resources.SWATlogo;
        this.pictureBox1.Location = new System.Drawing.Point(18, 2);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new System.Drawing.Size(360, 90);
        this.pictureBox1.TabIndex = 3;
        this.pictureBox1.TabStop = false;
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(7, 120);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(101, 13);
        this.label2.TabIndex = 4;
        this.label2.Text = "Loaded Assemblies:";
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(7, 140);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(0, 13);
        this.label3.TabIndex = 5;
        // 
        // companyInfo
        // 
        this.companyInfo.AutoSize = true;
        this.companyInfo.Location = new System.Drawing.Point(7, 100);
        this.companyInfo.Name = "companyInfo";
        this.companyInfo.Size = new System.Drawing.Size(368, 13);
        this.companyInfo.TabIndex = 6;
        this.companyInfo.Text = "SWAT Editor Copyright © 2008 by Ultimate Software, Inc. All rights reserved.";
        // 
        // AboutForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(402, 229);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.pictureBox1);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.companyInfo);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "AboutForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "About";
        this.Load += new System.EventHandler(this.AboutForm_Load);
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    //private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label companyInfo;
  }
}
