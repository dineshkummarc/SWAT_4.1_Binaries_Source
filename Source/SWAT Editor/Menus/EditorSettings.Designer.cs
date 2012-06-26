namespace SWAT_Editor.Controls
{
    partial class EditorSettings
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
            System.Windows.Forms.Label useAutoCompleteLabel;
            System.Windows.Forms.Label loadBlankFormLabel;
            System.Windows.Forms.Label ShowRecMenuLabel;
            System.Windows.Forms.Label autosaveEnabledLabel;
            System.Windows.Forms.Label autosaveFrequencyLabel;
            this.EditorGroupBox = new System.Windows.Forms.GroupBox();
            this.useAutoCompleteCheckBox = new System.Windows.Forms.CheckBox();
            this.minToTrayChkBox = new System.Windows.Forms.CheckBox();
            this.minimizeToTrayOptLabel = new System.Windows.Forms.Label();
            this.loadBlankFormCheckBox = new System.Windows.Forms.CheckBox();
            this.ctrlRightClickShowRecMenuCheckBox = new System.Windows.Forms.CheckBox();
            this.AutosaveGroupBox = new System.Windows.Forms.GroupBox();
            this.autosaveDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.AutosaveDirectoryBrowseButton = new System.Windows.Forms.Button();
            this.lblDirectory = new System.Windows.Forms.Label();
            this.autosaveEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.autosaveFrequencyTextBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.settingsDataEntityBindingSource = new System.Windows.Forms.BindingSource(this.components);
            useAutoCompleteLabel = new System.Windows.Forms.Label();
            loadBlankFormLabel = new System.Windows.Forms.Label();
            ShowRecMenuLabel = new System.Windows.Forms.Label();
            autosaveEnabledLabel = new System.Windows.Forms.Label();
            autosaveFrequencyLabel = new System.Windows.Forms.Label();
            this.EditorGroupBox.SuspendLayout();
            this.AutosaveGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // useAutoCompleteLabel
            // 
            useAutoCompleteLabel.AutoSize = true;
            useAutoCompleteLabel.Location = new System.Drawing.Point(14, 91);
            useAutoCompleteLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            useAutoCompleteLabel.Name = "useAutoCompleteLabel";
            useAutoCompleteLabel.Size = new System.Drawing.Size(132, 17);
            useAutoCompleteLabel.TabIndex = 18;
            useAutoCompleteLabel.Text = "Use Auto-complete:";
            // 
            // loadBlankFormLabel
            // 
            loadBlankFormLabel.AutoSize = true;
            loadBlankFormLabel.Location = new System.Drawing.Point(14, 30);
            loadBlankFormLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            loadBlankFormLabel.Name = "loadBlankFormLabel";
            loadBlankFormLabel.Size = new System.Drawing.Size(177, 17);
            loadBlankFormLabel.TabIndex = 17;
            loadBlankFormLabel.Text = "Open Blank Test On Load:";
            // 
            // ShowRecMenuLabel
            // 
            ShowRecMenuLabel.AutoSize = true;
            ShowRecMenuLabel.Location = new System.Drawing.Point(14, 61);
            ShowRecMenuLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            ShowRecMenuLabel.Name = "ShowRecMenuLabel";
            ShowRecMenuLabel.Size = new System.Drawing.Size(325, 17);
            ShowRecMenuLabel.TabIndex = 15;
            ShowRecMenuLabel.Text = "Ctrl Right Click Shows the Recorder Context Menu:";
            // 
            // autosaveEnabledLabel
            // 
            autosaveEnabledLabel.AutoSize = true;
            autosaveEnabledLabel.Location = new System.Drawing.Point(8, 32);
            autosaveEnabledLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            autosaveEnabledLabel.Name = "autosaveEnabledLabel";
            autosaveEnabledLabel.Size = new System.Drawing.Size(127, 17);
            autosaveEnabledLabel.TabIndex = 0;
            autosaveEnabledLabel.Text = "Autosave Enabled:";
            // 
            // autosaveFrequencyLabel
            // 
            autosaveFrequencyLabel.AutoSize = true;
            autosaveFrequencyLabel.Location = new System.Drawing.Point(8, 64);
            autosaveFrequencyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            autosaveFrequencyLabel.Name = "autosaveFrequencyLabel";
            autosaveFrequencyLabel.Size = new System.Drawing.Size(205, 17);
            autosaveFrequencyLabel.TabIndex = 2;
            autosaveFrequencyLabel.Text = "Autosave Frequency (minutes):";
            // 
            // EditorGroupBox
            // 
            this.EditorGroupBox.Controls.Add(this.useAutoCompleteCheckBox);
            this.EditorGroupBox.Controls.Add(this.minToTrayChkBox);
            this.EditorGroupBox.Controls.Add(this.minimizeToTrayOptLabel);
            this.EditorGroupBox.Controls.Add(useAutoCompleteLabel);
            this.EditorGroupBox.Controls.Add(loadBlankFormLabel);
            this.EditorGroupBox.Controls.Add(this.loadBlankFormCheckBox);
            this.EditorGroupBox.Controls.Add(this.ctrlRightClickShowRecMenuCheckBox);
            this.EditorGroupBox.Controls.Add(ShowRecMenuLabel);
            this.EditorGroupBox.Controls.Add(this.AutosaveGroupBox);
            this.EditorGroupBox.Location = new System.Drawing.Point(4, 4);
            this.EditorGroupBox.Margin = new System.Windows.Forms.Padding(0);
            this.EditorGroupBox.Name = "EditorGroupBox";
            this.EditorGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.EditorGroupBox.Size = new System.Drawing.Size(512, 404);
            this.EditorGroupBox.TabIndex = 1;
            this.EditorGroupBox.TabStop = false;
            this.EditorGroupBox.Text = "Editor Settings";
            // 
            // useAutoCompleteCheckBox
            // 
            this.useAutoCompleteCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "UseAutoComplete", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.useAutoCompleteCheckBox.Location = new System.Drawing.Point(356, 90);
            this.useAutoCompleteCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.useAutoCompleteCheckBox.Name = "useAutoCompleteCheckBox";
            this.useAutoCompleteCheckBox.Size = new System.Drawing.Size(18, 18);
            this.useAutoCompleteCheckBox.TabIndex = 20;
            this.useAutoCompleteCheckBox.UseVisualStyleBackColor = true;
            // 
            // minToTrayChkBox
            // 
            this.minToTrayChkBox.AutoSize = true;
            this.minToTrayChkBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "MinimizeToTray", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.minToTrayChkBox.Location = new System.Drawing.Point(356, 123);
            this.minToTrayChkBox.Margin = new System.Windows.Forms.Padding(4);
            this.minToTrayChkBox.Name = "minToTrayChkBox";
            this.minToTrayChkBox.Size = new System.Drawing.Size(18, 17);
            this.minToTrayChkBox.TabIndex = 22;
            this.minToTrayChkBox.UseVisualStyleBackColor = true;
            // 
            // minimizeToTrayOptLabel
            // 
            this.minimizeToTrayOptLabel.AutoSize = true;
            this.minimizeToTrayOptLabel.Location = new System.Drawing.Point(14, 122);
            this.minimizeToTrayOptLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.minimizeToTrayOptLabel.Name = "minimizeToTrayOptLabel";
            this.minimizeToTrayOptLabel.Size = new System.Drawing.Size(110, 17);
            this.minimizeToTrayOptLabel.TabIndex = 21;
            this.minimizeToTrayOptLabel.Text = "Minimize to tray:";
            // 
            // loadBlankFormCheckBox
            // 
            this.loadBlankFormCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "LoadBlankForm", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.loadBlankFormCheckBox.Location = new System.Drawing.Point(356, 30);
            this.loadBlankFormCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.loadBlankFormCheckBox.Name = "loadBlankFormCheckBox";
            this.loadBlankFormCheckBox.Size = new System.Drawing.Size(18, 21);
            this.loadBlankFormCheckBox.TabIndex = 19;
            this.loadBlankFormCheckBox.UseVisualStyleBackColor = true;
            // 
            // ctrlRightClickShowRecMenuCheckBox
            // 
            this.ctrlRightClickShowRecMenuCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "CtrlRightClickShowRecMenu", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.ctrlRightClickShowRecMenuCheckBox.Location = new System.Drawing.Point(356, 61);
            this.ctrlRightClickShowRecMenuCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.ctrlRightClickShowRecMenuCheckBox.Name = "ctrlRightClickShowRecMenuCheckBox";
            this.ctrlRightClickShowRecMenuCheckBox.Size = new System.Drawing.Size(18, 19);
            this.ctrlRightClickShowRecMenuCheckBox.TabIndex = 16;
            this.ctrlRightClickShowRecMenuCheckBox.UseVisualStyleBackColor = true;
            // 
            // AutosaveGroupBox
            // 
            this.AutosaveGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AutosaveGroupBox.Controls.Add(this.autosaveDirectoryTextBox);
            this.AutosaveGroupBox.Controls.Add(this.AutosaveDirectoryBrowseButton);
            this.AutosaveGroupBox.Controls.Add(this.lblDirectory);
            this.AutosaveGroupBox.Controls.Add(autosaveEnabledLabel);
            this.AutosaveGroupBox.Controls.Add(this.autosaveEnabledCheckBox);
            this.AutosaveGroupBox.Controls.Add(autosaveFrequencyLabel);
            this.AutosaveGroupBox.Controls.Add(this.autosaveFrequencyTextBox);
            this.AutosaveGroupBox.Location = new System.Drawing.Point(9, 157);
            this.AutosaveGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.AutosaveGroupBox.Name = "AutosaveGroupBox";
            this.AutosaveGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.AutosaveGroupBox.Size = new System.Drawing.Size(492, 153);
            this.AutosaveGroupBox.TabIndex = 12;
            this.AutosaveGroupBox.TabStop = false;
            this.AutosaveGroupBox.Text = "Autosave Settings";
            // 
            // autosaveDirectoryTextBox
            // 
            this.autosaveDirectoryTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "AutosaveFrequencyDirectory", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.autosaveDirectoryTextBox.Location = new System.Drawing.Point(12, 121);
            this.autosaveDirectoryTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.autosaveDirectoryTextBox.Name = "autosaveDirectoryTextBox";
            this.autosaveDirectoryTextBox.Size = new System.Drawing.Size(363, 22);
            this.autosaveDirectoryTextBox.TabIndex = 7;
            // 
            // AutosaveDirectoryBrowseButton
            // 
            this.AutosaveDirectoryBrowseButton.Location = new System.Drawing.Point(384, 116);
            this.AutosaveDirectoryBrowseButton.Margin = new System.Windows.Forms.Padding(4);
            this.AutosaveDirectoryBrowseButton.Name = "AutosaveDirectoryBrowseButton";
            this.AutosaveDirectoryBrowseButton.Size = new System.Drawing.Size(100, 30);
            this.AutosaveDirectoryBrowseButton.TabIndex = 6;
            this.AutosaveDirectoryBrowseButton.Text = "Browse";
            this.AutosaveDirectoryBrowseButton.UseVisualStyleBackColor = true;
            this.AutosaveDirectoryBrowseButton.Click += new System.EventHandler(this.AutosaveBrowseButton_Click);
            // 
            // lblDirectory
            // 
            this.lblDirectory.AutoSize = true;
            this.lblDirectory.Location = new System.Drawing.Point(8, 101);
            this.lblDirectory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDirectory.Name = "lblDirectory";
            this.lblDirectory.Size = new System.Drawing.Size(69, 17);
            this.lblDirectory.TabIndex = 5;
            this.lblDirectory.Text = "Directory:";
            // 
            // autosaveEnabledCheckBox
            // 
            this.autosaveEnabledCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "AutosaveEnabled", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.autosaveEnabledCheckBox.Location = new System.Drawing.Point(347, 19);
            this.autosaveEnabledCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.autosaveEnabledCheckBox.Name = "autosaveEnabledCheckBox";
            this.autosaveEnabledCheckBox.Size = new System.Drawing.Size(18, 30);
            this.autosaveEnabledCheckBox.TabIndex = 1;
            this.autosaveEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // autosaveFrequencyTextBox
            // 
            this.autosaveFrequencyTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "AutosaveFrequency", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.autosaveFrequencyTextBox.Location = new System.Drawing.Point(347, 61);
            this.autosaveFrequencyTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.autosaveFrequencyTextBox.Name = "autosaveFrequencyTextBox";
            this.autosaveFrequencyTextBox.Size = new System.Drawing.Size(49, 22);
            this.autosaveFrequencyTextBox.TabIndex = 3;
            this.autosaveFrequencyTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAutosaveFrequency_KeyPress);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.HelpRequest += new System.EventHandler(this.folderBrowserDialog1_HelpRequest);
            // 
            // settingsDataEntityBindingSource
            // 
            this.settingsDataEntityBindingSource.DataSource = typeof(SWAT_Editor.Controls.Options_Control.SettingsDataEntity);
            // 
            // EditorSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.EditorGroupBox);
            this.Name = "EditorSettings";
            this.Size = new System.Drawing.Size(523, 416);
            this.EditorGroupBox.ResumeLayout(false);
            this.EditorGroupBox.PerformLayout();
            this.AutosaveGroupBox.ResumeLayout(false);
            this.AutosaveGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox EditorGroupBox;
        private System.Windows.Forms.CheckBox minToTrayChkBox;
        private System.Windows.Forms.Label minimizeToTrayOptLabel;
        private System.Windows.Forms.CheckBox useAutoCompleteCheckBox;
        private System.Windows.Forms.CheckBox loadBlankFormCheckBox;
        private System.Windows.Forms.CheckBox ctrlRightClickShowRecMenuCheckBox;
        private System.Windows.Forms.GroupBox AutosaveGroupBox;
        private System.Windows.Forms.TextBox autosaveDirectoryTextBox;
        private System.Windows.Forms.Button AutosaveDirectoryBrowseButton;
        private System.Windows.Forms.Label lblDirectory;
        private System.Windows.Forms.CheckBox autosaveEnabledCheckBox;
        private System.Windows.Forms.TextBox autosaveFrequencyTextBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.BindingSource settingsDataEntityBindingSource;
    }
}
