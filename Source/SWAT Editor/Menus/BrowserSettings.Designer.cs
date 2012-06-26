namespace SWAT_Editor.Controls
{
    partial class BrowserSettings
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
            System.Windows.Forms.Label safariPortLabel;
            System.Windows.Forms.Label safariAddressLabel;
            this.BrowserGroupBox = new System.Windows.Forms.GroupBox();
            this.IESettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.IEEnableHighlightingCheckBox = new System.Windows.Forms.CheckBox();
            this.settingsDataEntityBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.IEEnableAutohighlighting = new System.Windows.Forms.Label();
            this.FirefoxSettingsBox = new System.Windows.Forms.GroupBox();
            this.FirefoxDirectoryBrowseButton = new System.Windows.Forms.Button();
            this.FirefoxDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.FirefoxPathLabel = new System.Windows.Forms.Label();
            this.SafariSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.safariPortTextBox = new System.Windows.Forms.TextBox();
            this.safariAddressTextBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            safariPortLabel = new System.Windows.Forms.Label();
            safariAddressLabel = new System.Windows.Forms.Label();
            this.BrowserGroupBox.SuspendLayout();
            this.IESettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).BeginInit();
            this.FirefoxSettingsBox.SuspendLayout();
            this.SafariSettingsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // safariPortLabel
            // 
            safariPortLabel.AutoSize = true;
            safariPortLabel.Location = new System.Drawing.Point(7, 57);
            safariPortLabel.Name = "safariPortLabel";
            safariPortLabel.Size = new System.Drawing.Size(59, 13);
            safariPortLabel.TabIndex = 2;
            safariPortLabel.Text = "Safari Port:";
            // 
            // safariAddressLabel
            // 
            safariAddressLabel.AutoSize = true;
            safariAddressLabel.Location = new System.Drawing.Point(7, 27);
            safariAddressLabel.Name = "safariAddressLabel";
            safariAddressLabel.Size = new System.Drawing.Size(78, 13);
            safariAddressLabel.TabIndex = 0;
            safariAddressLabel.Text = "Safari Address:";
            // 
            // BrowserGroupBox
            // 
            this.BrowserGroupBox.Controls.Add(this.IESettingsGroupBox);
            this.BrowserGroupBox.Controls.Add(this.FirefoxSettingsBox);
            this.BrowserGroupBox.Controls.Add(this.SafariSettingsGroupBox);
            this.BrowserGroupBox.Location = new System.Drawing.Point(3, 3);
            this.BrowserGroupBox.Name = "BrowserGroupBox";
            this.BrowserGroupBox.Size = new System.Drawing.Size(384, 328);
            this.BrowserGroupBox.TabIndex = 15;
            this.BrowserGroupBox.TabStop = false;
            this.BrowserGroupBox.Text = "Browser Settings";
            // 
            // IESettingsGroupBox
            // 
            this.IESettingsGroupBox.Controls.Add(this.IEEnableHighlightingCheckBox);
            this.IESettingsGroupBox.Controls.Add(this.IEEnableAutohighlighting);
            this.IESettingsGroupBox.Location = new System.Drawing.Point(6, 23);
            this.IESettingsGroupBox.Name = "IESettingsGroupBox";
            this.IESettingsGroupBox.Size = new System.Drawing.Size(369, 54);
            this.IESettingsGroupBox.TabIndex = 6;
            this.IESettingsGroupBox.TabStop = false;
            this.IESettingsGroupBox.Text = "Internet Explorer Settings";
            // 
            // IEEnableHighlightingCheckBox
            // 
            this.IEEnableHighlightingCheckBox.AutoSize = true;
            this.IEEnableHighlightingCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "IEAutoHighlight", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.IEEnableHighlightingCheckBox.Location = new System.Drawing.Point(165, 24);
            this.IEEnableHighlightingCheckBox.Name = "IEEnableHighlightingCheckBox";
            this.IEEnableHighlightingCheckBox.Size = new System.Drawing.Size(11, 11);
            this.IEEnableHighlightingCheckBox.TabIndex = 1;
            this.IEEnableHighlightingCheckBox.UseVisualStyleBackColor = true;
            // 
            // settingsDataEntityBindingSource
            // 
            this.settingsDataEntityBindingSource.DataSource = typeof(SWAT_Editor.Controls.Options_Control.SettingsDataEntity);
            // 
            // IEEnableAutohighlighting
            // 
            this.IEEnableAutohighlighting.AutoSize = true;
            this.IEEnableAutohighlighting.Location = new System.Drawing.Point(6, 24);
            this.IEEnableAutohighlighting.Name = "IEEnableAutohighlighting";
            this.IEEnableAutohighlighting.Size = new System.Drawing.Size(121, 13);
            this.IEEnableAutohighlighting.TabIndex = 0;
            this.IEEnableAutohighlighting.Text = "Enable Autohighlighting:";
            // 
            // FirefoxSettingsBox
            // 
            this.FirefoxSettingsBox.Controls.Add(this.FirefoxDirectoryBrowseButton);
            this.FirefoxSettingsBox.Controls.Add(this.FirefoxDirectoryTextBox);
            this.FirefoxSettingsBox.Controls.Add(this.FirefoxPathLabel);
            this.FirefoxSettingsBox.Location = new System.Drawing.Point(6, 84);
            this.FirefoxSettingsBox.Name = "FirefoxSettingsBox";
            this.FirefoxSettingsBox.Size = new System.Drawing.Size(369, 64);
            this.FirefoxSettingsBox.TabIndex = 14;
            this.FirefoxSettingsBox.TabStop = false;
            this.FirefoxSettingsBox.Text = "Firefox Settings";
            // 
            // FirefoxDirectoryBrowseButton
            // 
            this.FirefoxDirectoryBrowseButton.Location = new System.Drawing.Point(288, 34);
            this.FirefoxDirectoryBrowseButton.Name = "FirefoxDirectoryBrowseButton";
            this.FirefoxDirectoryBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.FirefoxDirectoryBrowseButton.TabIndex = 2;
            this.FirefoxDirectoryBrowseButton.Text = "Browse";
            this.FirefoxDirectoryBrowseButton.UseVisualStyleBackColor = true;
            this.FirefoxDirectoryBrowseButton.Click += new System.EventHandler(this.FirefoxDirectoryBrowseButton_Click);
            // 
            // FirefoxDirectoryTextBox
            // 
            this.FirefoxDirectoryTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "FirefoxRootDirectory", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.FirefoxDirectoryTextBox.Location = new System.Drawing.Point(9, 36);
            this.FirefoxDirectoryTextBox.Name = "FirefoxDirectoryTextBox";
            this.FirefoxDirectoryTextBox.Size = new System.Drawing.Size(273, 20);
            this.FirefoxDirectoryTextBox.TabIndex = 1;
            // 
            // FirefoxPathLabel
            // 
            this.FirefoxPathLabel.AutoSize = true;
            this.FirefoxPathLabel.Location = new System.Drawing.Point(6, 16);
            this.FirefoxPathLabel.Name = "FirefoxPathLabel";
            this.FirefoxPathLabel.Size = new System.Drawing.Size(112, 13);
            this.FirefoxPathLabel.TabIndex = 0;
            this.FirefoxPathLabel.Text = "Firefox Root Directory:";
            // 
            // SafariSettingsGroupBox
            // 
            this.SafariSettingsGroupBox.Controls.Add(safariPortLabel);
            this.SafariSettingsGroupBox.Controls.Add(this.safariPortTextBox);
            this.SafariSettingsGroupBox.Controls.Add(safariAddressLabel);
            this.SafariSettingsGroupBox.Controls.Add(this.safariAddressTextBox);
            this.SafariSettingsGroupBox.Location = new System.Drawing.Point(6, 154);
            this.SafariSettingsGroupBox.Name = "SafariSettingsGroupBox";
            this.SafariSettingsGroupBox.Size = new System.Drawing.Size(369, 79);
            this.SafariSettingsGroupBox.TabIndex = 0;
            this.SafariSettingsGroupBox.TabStop = false;
            this.SafariSettingsGroupBox.Text = "Safari Settings";
            // 
            // safariPortTextBox
            // 
            this.safariPortTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "SafariPort", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.safariPortTextBox.Location = new System.Drawing.Point(166, 54);
            this.safariPortTextBox.Name = "safariPortTextBox";
            this.safariPortTextBox.Size = new System.Drawing.Size(100, 20);
            this.safariPortTextBox.TabIndex = 3;
            this.safariPortTextBox.TextChanged += new System.EventHandler(this.safariPortTextBox_TextChanged);
            // 
            // safariAddressTextBox
            // 
            this.safariAddressTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "SafariAddress", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.safariAddressTextBox.Location = new System.Drawing.Point(166, 24);
            this.safariAddressTextBox.Name = "safariAddressTextBox";
            this.safariAddressTextBox.Size = new System.Drawing.Size(100, 20);
            this.safariAddressTextBox.TabIndex = 1;
            // 
            // BrowserSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BrowserGroupBox);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "BrowserSettings";
            this.Size = new System.Drawing.Size(392, 338);
            this.Load += new System.EventHandler(this.BrowserSettings_Load);
            this.BrowserGroupBox.ResumeLayout(false);
            this.IESettingsGroupBox.ResumeLayout(false);
            this.IESettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).EndInit();
            this.FirefoxSettingsBox.ResumeLayout(false);
            this.FirefoxSettingsBox.PerformLayout();
            this.SafariSettingsGroupBox.ResumeLayout(false);
            this.SafariSettingsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox BrowserGroupBox;
        private System.Windows.Forms.GroupBox IESettingsGroupBox;
        private System.Windows.Forms.CheckBox IEEnableHighlightingCheckBox;
        private System.Windows.Forms.Label IEEnableAutohighlighting;
        private System.Windows.Forms.GroupBox FirefoxSettingsBox;
        private System.Windows.Forms.Button FirefoxDirectoryBrowseButton;
        private System.Windows.Forms.TextBox FirefoxDirectoryTextBox;
        private System.Windows.Forms.Label FirefoxPathLabel;
        private System.Windows.Forms.GroupBox SafariSettingsGroupBox;
        private System.Windows.Forms.TextBox safariPortTextBox;
        private System.Windows.Forms.TextBox safariAddressTextBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.BindingSource settingsDataEntityBindingSource;
    }
}
