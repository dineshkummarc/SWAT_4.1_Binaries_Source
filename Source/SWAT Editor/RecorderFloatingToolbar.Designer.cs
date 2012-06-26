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
  partial class RecorderFloatingToolbar
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.rdAssertNone = new System.Windows.Forms.RadioButton();
      this.rdAssertExists = new System.Windows.Forms.RadioButton();
      this.rdEnabled = new System.Windows.Forms.RadioButton();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.tlbPause = new System.Windows.Forms.ToolStripButton();
      this.tlbStop = new System.Windows.Forms.ToolStripButton();
      this.groupBox1.SuspendLayout();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.rdEnabled);
      this.groupBox1.Controls.Add(this.rdAssertExists);
      this.groupBox1.Controls.Add(this.rdAssertNone);
      this.groupBox1.Location = new System.Drawing.Point(12, 37);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(381, 38);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Assertion Mode";
      // 
      // rdAssertNone
      // 
      this.rdAssertNone.AutoSize = true;
      this.rdAssertNone.Location = new System.Drawing.Point(6, 15);
      this.rdAssertNone.Name = "rdAssertNone";
      this.rdAssertNone.Size = new System.Drawing.Size(51, 17);
      this.rdAssertNone.TabIndex = 0;
      this.rdAssertNone.TabStop = true;
      this.rdAssertNone.Text = "None";
      this.rdAssertNone.UseVisualStyleBackColor = true;
      // 
      // rdAssertExists
      // 
      this.rdAssertExists.AutoSize = true;
      this.rdAssertExists.Location = new System.Drawing.Point(61, 15);
      this.rdAssertExists.Name = "rdAssertExists";
      this.rdAssertExists.Size = new System.Drawing.Size(52, 17);
      this.rdAssertExists.TabIndex = 1;
      this.rdAssertExists.TabStop = true;
      this.rdAssertExists.Text = "Exists";
      this.rdAssertExists.UseVisualStyleBackColor = true;
      // 
      // rdEnabled
      // 
      this.rdEnabled.AutoSize = true;
      this.rdEnabled.Location = new System.Drawing.Point(119, 15);
      this.rdEnabled.Name = "rdEnabled";
      this.rdEnabled.Size = new System.Drawing.Size(64, 17);
      this.rdEnabled.TabIndex = 1;
      this.rdEnabled.TabStop = true;
      this.rdEnabled.Text = "Enabled";
      this.rdEnabled.UseVisualStyleBackColor = true;
      // 
      // toolStrip1
      // 
      this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlbPause,
            this.tlbStop});
      this.toolStrip1.Location = new System.Drawing.Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new System.Drawing.Size(405, 25);
      this.toolStrip1.TabIndex = 2;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // tlbPause
      // 
      this.tlbPause.CheckOnClick = true;
      this.tlbPause.Image = global::SWAT_Editor.Properties.Resources._16_control_pause;
      this.tlbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tlbPause.Name = "tlbPause";
      this.tlbPause.Size = new System.Drawing.Size(61, 22);
      this.tlbPause.Text = "Pause";
      this.tlbPause.ToolTipText = "Pause Recording";
      // 
      // tlbStop
      // 
      this.tlbStop.CheckOnClick = true;
      this.tlbStop.Image = global::SWAT_Editor.Properties.Resources._16_control_stop;
      this.tlbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tlbStop.Name = "tlbStop";
      this.tlbStop.Size = new System.Drawing.Size(53, 22);
      this.tlbStop.Text = "Stop";
      // 
      // RecorderFloatingToolbar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(405, 87);
      this.ControlBox = false;
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.groupBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "RecorderFloatingToolbar";
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "Recorder Settings";
      this.TopMost = true;
      this.Load += new System.EventHandler(this.RecorderFloatingToolbar_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.RadioButton rdAssertNone;
    private System.Windows.Forms.RadioButton rdEnabled;
    private System.Windows.Forms.RadioButton rdAssertExists;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripButton tlbPause;
    private System.Windows.Forms.ToolStripButton tlbStop;

  }
}
