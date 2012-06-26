namespace SWAT_Editor.Controls
{
    partial class FitnesseSettings
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
            System.Windows.Forms.Label fitnesseRootDirectoryLabel;
            this.FitnesseGroupBox = new System.Windows.Forms.GroupBox();
            this.FitnesseRootBrowseButton = new System.Windows.Forms.Button();
            this.fitnesseRootDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.settingsDataEntityBindingSource = new System.Windows.Forms.BindingSource(this.components);
            fitnesseRootDirectoryLabel = new System.Windows.Forms.Label();
            this.FitnesseGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // fitnesseRootDirectoryLabel
            // 
            fitnesseRootDirectoryLabel.AutoSize = true;
            fitnesseRootDirectoryLabel.Location = new System.Drawing.Point(8, 21);
            fitnesseRootDirectoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            fitnesseRootDirectoryLabel.Name = "fitnesseRootDirectoryLabel";
            fitnesseRootDirectoryLabel.Size = new System.Drawing.Size(160, 17);
            fitnesseRootDirectoryLabel.TabIndex = 0;
            fitnesseRootDirectoryLabel.Text = "Fitnesse Root Directory:";
            // 
            // FitnesseGroupBox
            // 
            this.FitnesseGroupBox.Controls.Add(this.FitnesseRootBrowseButton);
            this.FitnesseGroupBox.Controls.Add(fitnesseRootDirectoryLabel);
            this.FitnesseGroupBox.Controls.Add(this.fitnesseRootDirectoryTextBox);
            this.FitnesseGroupBox.Location = new System.Drawing.Point(4, 4);
            this.FitnesseGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.FitnesseGroupBox.Name = "FitnesseGroupBox";
            this.FitnesseGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.FitnesseGroupBox.Size = new System.Drawing.Size(512, 404);
            this.FitnesseGroupBox.TabIndex = 7;
            this.FitnesseGroupBox.TabStop = false;
            this.FitnesseGroupBox.Text = "Fitnesse Settings";
            // 
            // FitnesseRootBrowseButton
            // 
            this.FitnesseRootBrowseButton.Location = new System.Drawing.Point(384, 42);
            this.FitnesseRootBrowseButton.Margin = new System.Windows.Forms.Padding(4);
            this.FitnesseRootBrowseButton.Name = "FitnesseRootBrowseButton";
            this.FitnesseRootBrowseButton.Size = new System.Drawing.Size(100, 30);
            this.FitnesseRootBrowseButton.TabIndex = 7;
            this.FitnesseRootBrowseButton.Text = "Browse";
            this.FitnesseRootBrowseButton.UseVisualStyleBackColor = true;
            this.FitnesseRootBrowseButton.Click += new System.EventHandler(this.FitnesseRootBrowseButton_Click);
            // 
            // fitnesseRootDirectoryTextBox
            // 
            this.fitnesseRootDirectoryTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "FitnesseRootDirectory", true));
            this.fitnesseRootDirectoryTextBox.Location = new System.Drawing.Point(12, 44);
            this.fitnesseRootDirectoryTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.fitnesseRootDirectoryTextBox.Name = "fitnesseRootDirectoryTextBox";
            this.fitnesseRootDirectoryTextBox.Size = new System.Drawing.Size(363, 22);
            this.fitnesseRootDirectoryTextBox.TabIndex = 1;
            // 
            // settingsDataEntityBindingSource
            // 
            this.settingsDataEntityBindingSource.DataSource = typeof(SWAT_Editor.Controls.Options_Control.SettingsDataEntity);
            // 
            // FitnesseSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FitnesseGroupBox);
            this.Name = "FitnesseSettings";
            this.Size = new System.Drawing.Size(523, 416);
            this.FitnesseGroupBox.ResumeLayout(false);
            this.FitnesseGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox FitnesseGroupBox;
        private System.Windows.Forms.Button FitnesseRootBrowseButton;
        private System.Windows.Forms.TextBox fitnesseRootDirectoryTextBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.BindingSource settingsDataEntityBindingSource;
    }
}
