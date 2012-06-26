namespace SWAT_Editor.Controls
{
    partial class ScreenshotSettings
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
            System.Windows.Forms.Label takeSnapshotsLabel;
            System.Windows.Forms.Label windowOnlyScreenshotLabel;
            System.Windows.Forms.Label allScreensSnapshotLabel;
            System.Windows.Forms.Label imageFileDirectoryLabel;
            this.ScreenShotGroupBox = new System.Windows.Forms.GroupBox();
            this.takeSnapshotsCheckBox = new System.Windows.Forms.CheckBox();
            this.settingsDataEntityBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ScreenshotDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.ScreenshotDirectoryButton = new System.Windows.Forms.Button();
            this.windowOnlyScreenshotRadioButton = new System.Windows.Forms.RadioButton();
            this.allScreensSnapshotRadioButton = new System.Windows.Forms.RadioButton();
            this.lblScreenShotMessage = new System.Windows.Forms.Label();
            this.imageFileDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            takeSnapshotsLabel = new System.Windows.Forms.Label();
            windowOnlyScreenshotLabel = new System.Windows.Forms.Label();
            allScreensSnapshotLabel = new System.Windows.Forms.Label();
            imageFileDirectoryLabel = new System.Windows.Forms.Label();
            this.ScreenShotGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).BeginInit();
            this.ScreenshotDetailsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // takeSnapshotsLabel
            // 
            takeSnapshotsLabel.AutoSize = true;
            takeSnapshotsLabel.Location = new System.Drawing.Point(8, 27);
            takeSnapshotsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            takeSnapshotsLabel.Name = "takeSnapshotsLabel";
            takeSnapshotsLabel.Size = new System.Drawing.Size(127, 17);
            takeSnapshotsLabel.TabIndex = 4;
            takeSnapshotsLabel.Text = "Take Screenshots:";
            // 
            // windowOnlyScreenshotLabel
            // 
            windowOnlyScreenshotLabel.AutoSize = true;
            windowOnlyScreenshotLabel.Location = new System.Drawing.Point(19, 63);
            windowOnlyScreenshotLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            windowOnlyScreenshotLabel.Name = "windowOnlyScreenshotLabel";
            windowOnlyScreenshotLabel.Size = new System.Drawing.Size(170, 17);
            windowOnlyScreenshotLabel.TabIndex = 20;
            windowOnlyScreenshotLabel.Text = "Window Only Screenshot:";
            // 
            // allScreensSnapshotLabel
            // 
            allScreensSnapshotLabel.AutoSize = true;
            allScreensSnapshotLabel.Location = new System.Drawing.Point(19, 34);
            allScreensSnapshotLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            allScreensSnapshotLabel.Name = "allScreensSnapshotLabel";
            allScreensSnapshotLabel.Size = new System.Drawing.Size(159, 17);
            allScreensSnapshotLabel.TabIndex = 19;
            allScreensSnapshotLabel.Text = "All Screens Screenshot:";
            // 
            // imageFileDirectoryLabel
            // 
            imageFileDirectoryLabel.AutoSize = true;
            imageFileDirectoryLabel.Location = new System.Drawing.Point(19, 95);
            imageFileDirectoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            imageFileDirectoryLabel.Name = "imageFileDirectoryLabel";
            imageFileDirectoryLabel.Size = new System.Drawing.Size(137, 17);
            imageFileDirectoryLabel.TabIndex = 2;
            imageFileDirectoryLabel.Text = "Image File Directory:";
            // 
            // ScreenShotGroupBox
            // 
            this.ScreenShotGroupBox.Controls.Add(takeSnapshotsLabel);
            this.ScreenShotGroupBox.Controls.Add(this.takeSnapshotsCheckBox);
            this.ScreenShotGroupBox.Controls.Add(this.ScreenshotDetailsGroupBox);
            this.ScreenShotGroupBox.Location = new System.Drawing.Point(4, 4);
            this.ScreenShotGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.ScreenShotGroupBox.Name = "ScreenShotGroupBox";
            this.ScreenShotGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.ScreenShotGroupBox.Size = new System.Drawing.Size(512, 404);
            this.ScreenShotGroupBox.TabIndex = 6;
            this.ScreenShotGroupBox.TabStop = false;
            this.ScreenShotGroupBox.Text = "Screenshot Settings";
            // 
            // takeSnapshotsCheckBox
            // 
            this.takeSnapshotsCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "TakeSnapshots", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.takeSnapshotsCheckBox.Location = new System.Drawing.Point(154, 21);
            this.takeSnapshotsCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.takeSnapshotsCheckBox.Name = "takeSnapshotsCheckBox";
            this.takeSnapshotsCheckBox.Size = new System.Drawing.Size(19, 30);
            this.takeSnapshotsCheckBox.TabIndex = 5;
            this.takeSnapshotsCheckBox.UseVisualStyleBackColor = true;
            this.takeSnapshotsCheckBox.CheckedChanged += new System.EventHandler(this.takeScreenShot_CheckedChanged);
            // 
            // settingsDataEntityBindingSource
            // 
            this.settingsDataEntityBindingSource.DataSource = typeof(SWAT_Editor.Controls.Options_Control.SettingsDataEntity);
            // 
            // ScreenshotDetailsGroupBox
            // 
            this.ScreenshotDetailsGroupBox.Controls.Add(this.ScreenshotDirectoryButton);
            this.ScreenshotDetailsGroupBox.Controls.Add(windowOnlyScreenshotLabel);
            this.ScreenshotDetailsGroupBox.Controls.Add(this.windowOnlyScreenshotRadioButton);
            this.ScreenshotDetailsGroupBox.Controls.Add(allScreensSnapshotLabel);
            this.ScreenshotDetailsGroupBox.Controls.Add(this.allScreensSnapshotRadioButton);
            this.ScreenshotDetailsGroupBox.Controls.Add(this.lblScreenShotMessage);
            this.ScreenshotDetailsGroupBox.Controls.Add(imageFileDirectoryLabel);
            this.ScreenshotDetailsGroupBox.Controls.Add(this.imageFileDirectoryTextBox);
            this.ScreenshotDetailsGroupBox.Location = new System.Drawing.Point(8, 65);
            this.ScreenshotDetailsGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.ScreenshotDetailsGroupBox.Name = "ScreenshotDetailsGroupBox";
            this.ScreenshotDetailsGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.ScreenshotDetailsGroupBox.Size = new System.Drawing.Size(496, 162);
            this.ScreenshotDetailsGroupBox.TabIndex = 8;
            this.ScreenshotDetailsGroupBox.TabStop = false;
            this.ScreenshotDetailsGroupBox.Text = "Take Screenshot Of";
            // 
            // ScreenshotDirectoryButton
            // 
            this.ScreenshotDirectoryButton.Location = new System.Drawing.Point(388, 111);
            this.ScreenshotDirectoryButton.Margin = new System.Windows.Forms.Padding(4);
            this.ScreenshotDirectoryButton.Name = "ScreenshotDirectoryButton";
            this.ScreenshotDirectoryButton.Size = new System.Drawing.Size(100, 30);
            this.ScreenshotDirectoryButton.TabIndex = 22;
            this.ScreenshotDirectoryButton.Text = "Browse";
            this.ScreenshotDirectoryButton.UseVisualStyleBackColor = true;
            this.ScreenshotDirectoryButton.Click += new System.EventHandler(this.ScreenshotBrowseButton_Click);
            // 
            // windowOnlyScreenshotRadioButton
            // 
            this.windowOnlyScreenshotRadioButton.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.settingsDataEntityBindingSource, "WindowOnlyScreenshot", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.windowOnlyScreenshotRadioButton.Location = new System.Drawing.Point(200, 52);
            this.windowOnlyScreenshotRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.windowOnlyScreenshotRadioButton.Name = "windowOnlyScreenshotRadioButton";
            this.windowOnlyScreenshotRadioButton.Size = new System.Drawing.Size(25, 33);
            this.windowOnlyScreenshotRadioButton.TabIndex = 21;
            this.windowOnlyScreenshotRadioButton.TabStop = true;
            this.windowOnlyScreenshotRadioButton.UseVisualStyleBackColor = true;
            // 
            // allScreensSnapshotRadioButton
            // 
            this.allScreensSnapshotRadioButton.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.settingsDataEntityBindingSource, "AllScreensSnapshot", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.allScreensSnapshotRadioButton.Location = new System.Drawing.Point(200, 27);
            this.allScreensSnapshotRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.allScreensSnapshotRadioButton.Name = "allScreensSnapshotRadioButton";
            this.allScreensSnapshotRadioButton.Size = new System.Drawing.Size(25, 30);
            this.allScreensSnapshotRadioButton.TabIndex = 20;
            this.allScreensSnapshotRadioButton.TabStop = true;
            this.allScreensSnapshotRadioButton.UseVisualStyleBackColor = true;
            this.allScreensSnapshotRadioButton.CheckedChanged += new System.EventHandler(this.AllScreens_CheckedChanged);
            // 
            // lblScreenShotMessage
            // 
            this.lblScreenShotMessage.ForeColor = System.Drawing.Color.Red;
            this.lblScreenShotMessage.Location = new System.Drawing.Point(245, 34);
            this.lblScreenShotMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblScreenShotMessage.Name = "lblScreenShotMessage";
            this.lblScreenShotMessage.Size = new System.Drawing.Size(185, 54);
            this.lblScreenShotMessage.TabIndex = 19;
            this.lblScreenShotMessage.Text = "Vista users: If Aero feature is enabled, choose Browser for screen shots.";
            this.lblScreenShotMessage.Visible = false;
            // 
            // imageFileDirectoryTextBox
            // 
            this.imageFileDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.imageFileDirectoryTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "ImageFileDirectory", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.imageFileDirectoryTextBox.Location = new System.Drawing.Point(23, 114);
            this.imageFileDirectoryTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.imageFileDirectoryTextBox.Name = "imageFileDirectoryTextBox";
            this.imageFileDirectoryTextBox.Size = new System.Drawing.Size(356, 22);
            this.imageFileDirectoryTextBox.TabIndex = 3;
            // 
            // ScreenshotSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ScreenShotGroupBox);
            this.Name = "ScreenshotSettings";
            this.Size = new System.Drawing.Size(523, 416);
            this.Load += new System.EventHandler(this.ScreenshotSettings_Load);
            this.ScreenShotGroupBox.ResumeLayout(false);
            this.ScreenShotGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).EndInit();
            this.ScreenshotDetailsGroupBox.ResumeLayout(false);
            this.ScreenshotDetailsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ScreenShotGroupBox;
        private System.Windows.Forms.CheckBox takeSnapshotsCheckBox;
        private System.Windows.Forms.GroupBox ScreenshotDetailsGroupBox;
        private System.Windows.Forms.Button ScreenshotDirectoryButton;
        private System.Windows.Forms.RadioButton windowOnlyScreenshotRadioButton;
        private System.Windows.Forms.RadioButton allScreensSnapshotRadioButton;
        private System.Windows.Forms.Label lblScreenShotMessage;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.BindingSource settingsDataEntityBindingSource;
        private System.Windows.Forms.TextBox imageFileDirectoryTextBox;
    }
}
