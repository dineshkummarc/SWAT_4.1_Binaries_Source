namespace SWAT_Editor.Controls
{
    partial class DatabaseSettings
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
            System.Windows.Forms.Label connectionTimeoutLabel;
            this.DatabaseGroupBox = new System.Windows.Forms.GroupBox();
            this.connectionTimeoutTextBox = new System.Windows.Forms.TextBox();
            this.settingsDataEntityBindingSource = new System.Windows.Forms.BindingSource(this.components);
            connectionTimeoutLabel = new System.Windows.Forms.Label();
            this.DatabaseGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // connectionTimeoutLabel
            // 
            connectionTimeoutLabel.AutoSize = true;
            connectionTimeoutLabel.Location = new System.Drawing.Point(8, 34);
            connectionTimeoutLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            connectionTimeoutLabel.Name = "connectionTimeoutLabel";
            connectionTimeoutLabel.Size = new System.Drawing.Size(205, 17);
            connectionTimeoutLabel.TabIndex = 4;
            connectionTimeoutLabel.Text = "Connection Timeout (seconds):";
            // 
            // DatabaseGroupBox
            // 
            this.DatabaseGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DatabaseGroupBox.Controls.Add(this.connectionTimeoutTextBox);
            this.DatabaseGroupBox.Controls.Add(connectionTimeoutLabel);
            this.DatabaseGroupBox.Location = new System.Drawing.Point(4, 4);
            this.DatabaseGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.DatabaseGroupBox.Name = "DatabaseGroupBox";
            this.DatabaseGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.DatabaseGroupBox.Size = new System.Drawing.Size(512, 404);
            this.DatabaseGroupBox.TabIndex = 14;
            this.DatabaseGroupBox.TabStop = false;
            this.DatabaseGroupBox.Text = "Database Settings";
            // 
            // connectionTimeoutTextBox
            // 
            this.connectionTimeoutTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "ConnectionTimeout", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.connectionTimeoutTextBox.Location = new System.Drawing.Point(221, 31);
            this.connectionTimeoutTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.connectionTimeoutTextBox.Name = "connectionTimeoutTextBox";
            this.connectionTimeoutTextBox.Size = new System.Drawing.Size(49, 22);
            this.connectionTimeoutTextBox.TabIndex = 5;
            this.connectionTimeoutTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ConnectionTimeout_KeyPress);
            // 
            // settingsDataEntityBindingSource
            // 
            this.settingsDataEntityBindingSource.DataSource = typeof(SWAT_Editor.Controls.Options_Control.SettingsDataEntity);
            // 
            // DatabaseSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.DatabaseGroupBox);
            this.Name = "DatabaseSettings";
            this.Size = new System.Drawing.Size(523, 416);
            this.DatabaseGroupBox.ResumeLayout(false);
            this.DatabaseGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox DatabaseGroupBox;
        private System.Windows.Forms.TextBox connectionTimeoutTextBox;
        private System.Windows.Forms.BindingSource settingsDataEntityBindingSource;
    }
}
