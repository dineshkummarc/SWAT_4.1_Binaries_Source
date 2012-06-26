namespace SWAT_Editor.Controls
{
    partial class DDEditor
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
            this.XMLLabel = new System.Windows.Forms.Label();
            this.XMLTextBox = new System.Windows.Forms.TextBox();
            this.TestLabel = new System.Windows.Forms.Label();
            this.TestFileTextBox = new System.Windows.Forms.TextBox();
            this.xMLBtn = new System.Windows.Forms.Button();
            this.testFileBtn = new System.Windows.Forms.Button();
            this.DDEPanel = new System.Windows.Forms.Panel();
            this.DestinationLabel = new System.Windows.Forms.Label();
            this.destinationBtn = new System.Windows.Forms.Button();
            this.DestinationTextBox = new System.Windows.Forms.TextBox();
            this.overrideFitnesseChkBox = new System.Windows.Forms.CheckBox();
            this.xmlFileErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.testFileErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.destinationErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.xmlFileErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testFileErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.destinationErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // XMLLabel
            // 
            this.XMLLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.XMLLabel.AutoSize = true;
            this.XMLLabel.Location = new System.Drawing.Point(153, 112);
            this.XMLLabel.Name = "XMLLabel";
            this.XMLLabel.Size = new System.Drawing.Size(48, 13);
            this.XMLLabel.TabIndex = 0;
            this.XMLLabel.Text = "XML File";
            // 
            // XMLTextBox
            // 
            this.XMLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlFileErrorProvider.SetIconAlignment(this.XMLTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
            this.xmlFileErrorProvider.SetIconPadding(this.XMLTextBox, 2);
            this.XMLTextBox.Location = new System.Drawing.Point(150, 132);
            this.XMLTextBox.MinimumSize = new System.Drawing.Size(80, 20);
            this.XMLTextBox.Name = "XMLTextBox";
            this.XMLTextBox.Size = new System.Drawing.Size(359, 20);
            this.XMLTextBox.TabIndex = 6;
            this.XMLTextBox.TextChanged += new System.EventHandler(this.XMLTextBox_TextChanged);
            // 
            // TestLabel
            // 
            this.TestLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.TestLabel.AutoSize = true;
            this.TestLabel.Location = new System.Drawing.Point(153, 232);
            this.TestLabel.Name = "TestLabel";
            this.TestLabel.Size = new System.Drawing.Size(47, 13);
            this.TestLabel.TabIndex = 0;
            this.TestLabel.Text = "Test File";
            // 
            // TestFileTextBox
            // 
            this.TestFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.testFileErrorProvider.SetIconAlignment(this.TestFileTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
            this.testFileErrorProvider.SetIconPadding(this.TestFileTextBox, 2);
            this.TestFileTextBox.Location = new System.Drawing.Point(150, 252);
            this.TestFileTextBox.MinimumSize = new System.Drawing.Size(80, 20);
            this.TestFileTextBox.Name = "TestFileTextBox";
            this.TestFileTextBox.Size = new System.Drawing.Size(359, 20);
            this.TestFileTextBox.TabIndex = 8;
            this.TestFileTextBox.TextChanged += new System.EventHandler(this.TestFileTextBox_TextChanged);
            // 
            // xMLBtn
            // 
            this.xMLBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.xMLBtn.Location = new System.Drawing.Point(512, 131);
            this.xMLBtn.Margin = new System.Windows.Forms.Padding(20);
            this.xMLBtn.Name = "xMLBtn";
            this.xMLBtn.Size = new System.Drawing.Size(35, 22);
            this.xMLBtn.TabIndex = 7;
            this.xMLBtn.Text = "...";
            this.xMLBtn.UseVisualStyleBackColor = true;
            this.xMLBtn.Click += new System.EventHandler(this.xMLBtn_Click);
            // 
            // testFileBtn
            // 
            this.testFileBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.testFileBtn.Location = new System.Drawing.Point(512, 251);
            this.testFileBtn.Name = "testFileBtn";
            this.testFileBtn.Size = new System.Drawing.Size(35, 22);
            this.testFileBtn.TabIndex = 9;
            this.testFileBtn.Text = "...";
            this.testFileBtn.UseVisualStyleBackColor = true;
            this.testFileBtn.Click += new System.EventHandler(this.testFileBtn_Click);
            // 
            // DDEPanel
            // 
            this.DDEPanel.AutoSize = true;
            this.DDEPanel.Location = new System.Drawing.Point(150, 62);
            this.DDEPanel.Name = "DDEPanel";
            this.DDEPanel.Size = new System.Drawing.Size(397, 331);
            this.DDEPanel.TabIndex = 11;
            // 
            // DestinationLabel
            // 
            this.DestinationLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DestinationLabel.AutoSize = true;
            this.DestinationLabel.Location = new System.Drawing.Point(153, 352);
            this.DestinationLabel.Name = "DestinationLabel";
            this.DestinationLabel.Size = new System.Drawing.Size(95, 13);
            this.DestinationLabel.TabIndex = 2;
            this.DestinationLabel.Text = "Test Output Folder";
            // 
            // destinationBtn
            // 
            this.destinationBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.destinationBtn.Location = new System.Drawing.Point(512, 371);
            this.destinationBtn.Name = "destinationBtn";
            this.destinationBtn.Size = new System.Drawing.Size(35, 22);
            this.destinationBtn.TabIndex = 11;
            this.destinationBtn.Text = "...";
            this.destinationBtn.UseVisualStyleBackColor = true;
            this.destinationBtn.Click += new System.EventHandler(this.destinationBtn_Click);
            // 
            // DestinationTextBox
            // 
            this.DestinationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.destinationErrorProvider.SetIconAlignment(this.DestinationTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
            this.destinationErrorProvider.SetIconPadding(this.DestinationTextBox, 2);
            this.DestinationTextBox.Location = new System.Drawing.Point(150, 372);
            this.DestinationTextBox.MinimumSize = new System.Drawing.Size(80, 20);
            this.DestinationTextBox.Name = "DestinationTextBox";
            this.DestinationTextBox.Size = new System.Drawing.Size(359, 20);
            this.DestinationTextBox.TabIndex = 12;
            this.DestinationTextBox.TextChanged += new System.EventHandler(this.DestinationTextBox_TextChanged);
            // 
            // overrideFitnesseChkBox
            // 
            this.overrideFitnesseChkBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.overrideFitnesseChkBox.AutoSize = true;
            this.overrideFitnesseChkBox.Location = new System.Drawing.Point(150, 62);
            this.overrideFitnesseChkBox.Name = "overrideFitnesseChkBox";
            this.overrideFitnesseChkBox.Size = new System.Drawing.Size(196, 17);
            this.overrideFitnesseChkBox.TabIndex = 1;
            this.overrideFitnesseChkBox.Text = "Override duplicate fitnesse variables";
            this.overrideFitnesseChkBox.UseVisualStyleBackColor = true;
            // 
            // xmlFileErrorProvider
            // 
            this.xmlFileErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.xmlFileErrorProvider.ContainerControl = this;
            this.xmlFileErrorProvider.DataSource = this.XMLTextBox;
            // 
            // testFileErrorProvider
            // 
            this.testFileErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.testFileErrorProvider.ContainerControl = this;
            this.testFileErrorProvider.DataSource = this.TestFileTextBox;
            // 
            // destinationErrorProvider
            // 
            this.destinationErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.destinationErrorProvider.ContainerControl = this;
            this.destinationErrorProvider.DataSource = this.TestFileTextBox;
            // 
            // DDEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.overrideFitnesseChkBox);
            this.Controls.Add(this.DestinationLabel);
            this.Controls.Add(this.DestinationTextBox);
            this.Controls.Add(this.destinationBtn);
            this.Controls.Add(this.testFileBtn);
            this.Controls.Add(this.xMLBtn);
            this.Controls.Add(this.TestFileTextBox);
            this.Controls.Add(this.TestLabel);
            this.Controls.Add(this.XMLTextBox);
            this.Controls.Add(this.XMLLabel);
            this.Controls.Add(this.DDEPanel);
            this.Name = "DDEditor";
            this.Size = new System.Drawing.Size(693, 476);
            ((System.ComponentModel.ISupportInitialize)(this.xmlFileErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testFileErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.destinationErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label XMLLabel;
        private System.Windows.Forms.TextBox XMLTextBox;
        private System.Windows.Forms.Label TestLabel;
        private System.Windows.Forms.TextBox TestFileTextBox;
        private System.Windows.Forms.Button xMLBtn;
        private System.Windows.Forms.Button testFileBtn;
        private System.Windows.Forms.Panel DDEPanel;
        private System.Windows.Forms.Label DestinationLabel;
        private System.Windows.Forms.Button destinationBtn;
        private System.Windows.Forms.TextBox DestinationTextBox;
        private System.Windows.Forms.CheckBox overrideFitnesseChkBox;
        private System.Windows.Forms.ErrorProvider xmlFileErrorProvider;
        private System.Windows.Forms.ErrorProvider testFileErrorProvider;
        private System.Windows.Forms.ErrorProvider destinationErrorProvider;
    }
}
