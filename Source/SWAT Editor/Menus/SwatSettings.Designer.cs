namespace SWAT_Editor.Controls
{
    partial class SwatSettings
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
            System.Windows.Forms.Label overrideTestBrowserLabel;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label doesElementExistTimeOutLabel;
            System.Windows.Forms.Label doesElementNotExistTimeOutLabel;
            System.Windows.Forms.Label findElementLabel;
            System.Windows.Forms.Label waitForBrowserTimeoutLabel;
            System.Windows.Forms.Label attachToWindowTimeoutLabel;
            System.Windows.Forms.Label getInformativeExceptionsLabel;
            System.Windows.Forms.Label delayBetweenCommandsLabel;
            this.SWATGroupBox = new System.Windows.Forms.GroupBox();
            this.TimeoutSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.assertBrowserExistsTimeout = new System.Windows.Forms.TextBox();
            this.settingsDataEntityBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.waitForDocumentLoadTimeoutTextBox = new System.Windows.Forms.TextBox();
            this.waitForDocumentLoadTimeoutLabel = new System.Windows.Forms.Label();
            this.TimeoutSecondsLabel = new System.Windows.Forms.Label();
            this.doesElementExistTimeOutTextBox = new System.Windows.Forms.TextBox();
            this.doesElementNotExistTimeOutTextBox = new System.Windows.Forms.TextBox();
            this.findElementTextBox = new System.Windows.Forms.TextBox();
            this.waitForBrowserTimeoutTextBox = new System.Windows.Forms.TextBox();
            this.attachToWindowTimeoutTextBox = new System.Windows.Forms.TextBox();
            this.CloseBrowsersBeforeTestStartLabel = new System.Windows.Forms.Label();
            this.overrideTestBrowserCheckBox = new System.Windows.Forms.CheckBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.getInformativeExceptionsCheckBox = new System.Windows.Forms.CheckBox();
            this.CloseBrowsersBeforeTestStartCheckBox = new System.Windows.Forms.CheckBox();
            this.delayBetweenCommandsTextBox = new System.Windows.Forms.TextBox();
            this.suspendCheckBox = new System.Windows.Forms.CheckBox();
            this.suspendLabel = new System.Windows.Forms.Label();
            overrideTestBrowserLabel = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            doesElementExistTimeOutLabel = new System.Windows.Forms.Label();
            doesElementNotExistTimeOutLabel = new System.Windows.Forms.Label();
            findElementLabel = new System.Windows.Forms.Label();
            waitForBrowserTimeoutLabel = new System.Windows.Forms.Label();
            attachToWindowTimeoutLabel = new System.Windows.Forms.Label();
            getInformativeExceptionsLabel = new System.Windows.Forms.Label();
            delayBetweenCommandsLabel = new System.Windows.Forms.Label();
            this.SWATGroupBox.SuspendLayout();
            this.TimeoutSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // overrideTestBrowserLabel
            // 
            overrideTestBrowserLabel.AutoSize = true;
            overrideTestBrowserLabel.Location = new System.Drawing.Point(15, 35);
            overrideTestBrowserLabel.Name = "overrideTestBrowserLabel";
            overrideTestBrowserLabel.Size = new System.Drawing.Size(115, 13);
            overrideTestBrowserLabel.TabIndex = 0;
            overrideTestBrowserLabel.Text = "Override Test Browser:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 183);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(214, 13);
            label1.TabIndex = 17;
            label1.Text = "AssertBrowserExists/DoesNotExist Timeout:";
            // 
            // doesElementExistTimeOutLabel
            // 
            doesElementExistTimeOutLabel.AutoSize = true;
            doesElementExistTimeOutLabel.Location = new System.Drawing.Point(6, 158);
            doesElementExistTimeOutLabel.Name = "doesElementExistTimeOutLabel";
            doesElementExistTimeOutLabel.Size = new System.Drawing.Size(142, 13);
            doesElementExistTimeOutLabel.TabIndex = 12;
            doesElementExistTimeOutLabel.Text = "Does Element Exist Timeout:";
            // 
            // doesElementNotExistTimeOutLabel
            // 
            doesElementNotExistTimeOutLabel.AutoSize = true;
            doesElementNotExistTimeOutLabel.Location = new System.Drawing.Point(6, 133);
            doesElementNotExistTimeOutLabel.Name = "doesElementNotExistTimeOutLabel";
            doesElementNotExistTimeOutLabel.Size = new System.Drawing.Size(162, 13);
            doesElementNotExistTimeOutLabel.TabIndex = 8;
            doesElementNotExistTimeOutLabel.Text = "Does Element Not Exist Timeout:";
            // 
            // findElementLabel
            // 
            findElementLabel.AutoSize = true;
            findElementLabel.Location = new System.Drawing.Point(6, 109);
            findElementLabel.Name = "findElementLabel";
            findElementLabel.Size = new System.Drawing.Size(112, 13);
            findElementLabel.TabIndex = 6;
            findElementLabel.Text = "Find Element Timeout:";
            // 
            // waitForBrowserTimeoutLabel
            // 
            waitForBrowserTimeoutLabel.AutoSize = true;
            waitForBrowserTimeoutLabel.Location = new System.Drawing.Point(6, 84);
            waitForBrowserTimeoutLabel.Name = "waitForBrowserTimeoutLabel";
            waitForBrowserTimeoutLabel.Size = new System.Drawing.Size(132, 13);
            waitForBrowserTimeoutLabel.TabIndex = 4;
            waitForBrowserTimeoutLabel.Text = "Wait For Browser Timeout:";
            // 
            // attachToWindowTimeoutLabel
            // 
            attachToWindowTimeoutLabel.AutoSize = true;
            attachToWindowTimeoutLabel.Location = new System.Drawing.Point(6, 60);
            attachToWindowTimeoutLabel.Name = "attachToWindowTimeoutLabel";
            attachToWindowTimeoutLabel.Size = new System.Drawing.Size(140, 13);
            attachToWindowTimeoutLabel.TabIndex = 0;
            attachToWindowTimeoutLabel.Text = "Attach To Window Timeout:";
            // 
            // getInformativeExceptionsLabel
            // 
            getInformativeExceptionsLabel.AutoSize = true;
            getInformativeExceptionsLabel.Location = new System.Drawing.Point(15, 15);
            getInformativeExceptionsLabel.Name = "getInformativeExceptionsLabel";
            getInformativeExceptionsLabel.Size = new System.Drawing.Size(137, 13);
            getInformativeExceptionsLabel.TabIndex = 2;
            getInformativeExceptionsLabel.Text = "Get Informative Exceptions:";
            // 
            // delayBetweenCommandsLabel
            // 
            delayBetweenCommandsLabel.AutoSize = true;
            delayBetweenCommandsLabel.Location = new System.Drawing.Point(15, 55);
            delayBetweenCommandsLabel.Name = "delayBetweenCommandsLabel";
            delayBetweenCommandsLabel.Size = new System.Drawing.Size(186, 13);
            delayBetweenCommandsLabel.TabIndex = 2;
            delayBetweenCommandsLabel.Text = "Delay Between Commands (seconds):";
            // 
            // SWATGroupBox
            // 
            this.SWATGroupBox.Controls.Add(this.suspendLabel);
            this.SWATGroupBox.Controls.Add(this.suspendCheckBox);
            this.SWATGroupBox.Controls.Add(overrideTestBrowserLabel);
            this.SWATGroupBox.Controls.Add(this.TimeoutSettingsGroupBox);
            this.SWATGroupBox.Controls.Add(this.CloseBrowsersBeforeTestStartLabel);
            this.SWATGroupBox.Controls.Add(this.overrideTestBrowserCheckBox);
            this.SWATGroupBox.Controls.Add(this.errorLabel);
            this.SWATGroupBox.Controls.Add(getInformativeExceptionsLabel);
            this.SWATGroupBox.Controls.Add(this.getInformativeExceptionsCheckBox);
            this.SWATGroupBox.Controls.Add(this.CloseBrowsersBeforeTestStartCheckBox);
            this.SWATGroupBox.Controls.Add(delayBetweenCommandsLabel);
            this.SWATGroupBox.Controls.Add(this.delayBetweenCommandsTextBox);
            this.SWATGroupBox.Location = new System.Drawing.Point(3, 3);
            this.SWATGroupBox.Name = "SWATGroupBox";
            this.SWATGroupBox.Size = new System.Drawing.Size(384, 328);
            this.SWATGroupBox.TabIndex = 5;
            this.SWATGroupBox.TabStop = false;
            this.SWATGroupBox.Text = "SWAT Settings";
            this.SWATGroupBox.Enter += new System.EventHandler(this.SWATGroupBox_Enter);
            // 
            // TimeoutSettingsGroupBox
            // 
            this.TimeoutSettingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TimeoutSettingsGroupBox.Controls.Add(this.assertBrowserExistsTimeout);
            this.TimeoutSettingsGroupBox.Controls.Add(label1);
            this.TimeoutSettingsGroupBox.Controls.Add(this.waitForDocumentLoadTimeoutTextBox);
            this.TimeoutSettingsGroupBox.Controls.Add(this.waitForDocumentLoadTimeoutLabel);
            this.TimeoutSettingsGroupBox.Controls.Add(this.TimeoutSecondsLabel);
            this.TimeoutSettingsGroupBox.Controls.Add(doesElementExistTimeOutLabel);
            this.TimeoutSettingsGroupBox.Controls.Add(this.doesElementExistTimeOutTextBox);
            this.TimeoutSettingsGroupBox.Controls.Add(doesElementNotExistTimeOutLabel);
            this.TimeoutSettingsGroupBox.Controls.Add(this.doesElementNotExistTimeOutTextBox);
            this.TimeoutSettingsGroupBox.Controls.Add(findElementLabel);
            this.TimeoutSettingsGroupBox.Controls.Add(this.findElementTextBox);
            this.TimeoutSettingsGroupBox.Controls.Add(waitForBrowserTimeoutLabel);
            this.TimeoutSettingsGroupBox.Controls.Add(this.waitForBrowserTimeoutTextBox);
            this.TimeoutSettingsGroupBox.Controls.Add(attachToWindowTimeoutLabel);
            this.TimeoutSettingsGroupBox.Controls.Add(this.attachToWindowTimeoutTextBox);
            this.TimeoutSettingsGroupBox.Location = new System.Drawing.Point(17, 116);
            this.TimeoutSettingsGroupBox.Name = "TimeoutSettingsGroupBox";
            this.TimeoutSettingsGroupBox.Size = new System.Drawing.Size(328, 206);
            this.TimeoutSettingsGroupBox.TabIndex = 1;
            this.TimeoutSettingsGroupBox.TabStop = false;
            this.TimeoutSettingsGroupBox.Text = "Timeout Settings";
            this.TimeoutSettingsGroupBox.Enter += new System.EventHandler(this.TimeoutSettingsGroupBox_Enter);
            // 
            // assertBrowserExistsTimeout
            // 
            this.assertBrowserExistsTimeout.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "AssertBrowserExistsTimeout", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.assertBrowserExistsTimeout.Location = new System.Drawing.Point(246, 177);
            this.assertBrowserExistsTimeout.Name = "assertBrowserExistsTimeout";
            this.assertBrowserExistsTimeout.Size = new System.Drawing.Size(50, 20);
            this.assertBrowserExistsTimeout.TabIndex = 18;
            this.assertBrowserExistsTimeout.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBrowserTimeout_KeyPress);
            // 
            // settingsDataEntityBindingSource
            // 
            this.settingsDataEntityBindingSource.DataSource = typeof(SWAT_Editor.Controls.Options_Control.SettingsDataEntity);
            // 
            // waitForDocumentLoadTimeoutTextBox
            // 
            this.waitForDocumentLoadTimeoutTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "WaitForDocumentLoadTimeOut", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.waitForDocumentLoadTimeoutTextBox.Location = new System.Drawing.Point(246, 31);
            this.waitForDocumentLoadTimeoutTextBox.Name = "waitForDocumentLoadTimeoutTextBox";
            this.waitForDocumentLoadTimeoutTextBox.Size = new System.Drawing.Size(50, 20);
            this.waitForDocumentLoadTimeoutTextBox.TabIndex = 16;
            this.waitForDocumentLoadTimeoutTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBrowserTimeout_KeyPress);
            // 
            // waitForDocumentLoadTimeoutLabel
            // 
            this.waitForDocumentLoadTimeoutLabel.AutoSize = true;
            this.waitForDocumentLoadTimeoutLabel.Location = new System.Drawing.Point(6, 35);
            this.waitForDocumentLoadTimeoutLabel.Name = "waitForDocumentLoadTimeoutLabel";
            this.waitForDocumentLoadTimeoutLabel.Size = new System.Drawing.Size(167, 13);
            this.waitForDocumentLoadTimeoutLabel.TabIndex = 15;
            this.waitForDocumentLoadTimeoutLabel.Text = "Wait for Document Load Timeout:";
            // 
            // TimeoutSecondsLabel
            // 
            this.TimeoutSecondsLabel.AutoSize = true;
            this.TimeoutSecondsLabel.ForeColor = System.Drawing.Color.Red;
            this.TimeoutSecondsLabel.Location = new System.Drawing.Point(6, 15);
            this.TimeoutSecondsLabel.Name = "TimeoutSecondsLabel";
            this.TimeoutSecondsLabel.Size = new System.Drawing.Size(190, 13);
            this.TimeoutSecondsLabel.TabIndex = 4;
            this.TimeoutSecondsLabel.Text = "All the browser timeouts are in seconds";
            // 
            // doesElementExistTimeOutTextBox
            // 
            this.doesElementExistTimeOutTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "DoesElementExistTimeOut", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.doesElementExistTimeOutTextBox.Location = new System.Drawing.Point(246, 153);
            this.doesElementExistTimeOutTextBox.Name = "doesElementExistTimeOutTextBox";
            this.doesElementExistTimeOutTextBox.Size = new System.Drawing.Size(50, 20);
            this.doesElementExistTimeOutTextBox.TabIndex = 13;
            this.doesElementExistTimeOutTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBrowserTimeout_KeyPress);
            // 
            // doesElementNotExistTimeOutTextBox
            // 
            this.doesElementNotExistTimeOutTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "DoesElementNotExistTimeOut", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.doesElementNotExistTimeOutTextBox.Location = new System.Drawing.Point(246, 128);
            this.doesElementNotExistTimeOutTextBox.Name = "doesElementNotExistTimeOutTextBox";
            this.doesElementNotExistTimeOutTextBox.Size = new System.Drawing.Size(50, 20);
            this.doesElementNotExistTimeOutTextBox.TabIndex = 9;
            this.doesElementNotExistTimeOutTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBrowserTimeout_KeyPress);
            // 
            // findElementTextBox
            // 
            this.findElementTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "FindElement", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.findElementTextBox.Location = new System.Drawing.Point(246, 104);
            this.findElementTextBox.Name = "findElementTextBox";
            this.findElementTextBox.Size = new System.Drawing.Size(50, 20);
            this.findElementTextBox.TabIndex = 7;
            this.findElementTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBrowserTimeout_KeyPress);
            // 
            // waitForBrowserTimeoutTextBox
            // 
            this.waitForBrowserTimeoutTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "WaitForBrowserTimeout", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.waitForBrowserTimeoutTextBox.Location = new System.Drawing.Point(246, 80);
            this.waitForBrowserTimeoutTextBox.Name = "waitForBrowserTimeoutTextBox";
            this.waitForBrowserTimeoutTextBox.Size = new System.Drawing.Size(50, 20);
            this.waitForBrowserTimeoutTextBox.TabIndex = 5;
            this.waitForBrowserTimeoutTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBrowserTimeout_KeyPress);
            // 
            // attachToWindowTimeoutTextBox
            // 
            this.attachToWindowTimeoutTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "AttachToWindowTimeout", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.attachToWindowTimeoutTextBox.Location = new System.Drawing.Point(246, 55);
            this.attachToWindowTimeoutTextBox.Name = "attachToWindowTimeoutTextBox";
            this.attachToWindowTimeoutTextBox.Size = new System.Drawing.Size(50, 20);
            this.attachToWindowTimeoutTextBox.TabIndex = 1;
            this.attachToWindowTimeoutTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBrowserTimeout_KeyPress);
            // 
            // CloseBrowsersBeforeTestStartLabel
            // 
            this.CloseBrowsersBeforeTestStartLabel.AutoSize = true;
            this.CloseBrowsersBeforeTestStartLabel.Location = new System.Drawing.Point(15, 75);
            this.CloseBrowsersBeforeTestStartLabel.Name = "CloseBrowsersBeforeTestStartLabel";
            this.CloseBrowsersBeforeTestStartLabel.Size = new System.Drawing.Size(165, 13);
            this.CloseBrowsersBeforeTestStartLabel.TabIndex = 5;
            this.CloseBrowsersBeforeTestStartLabel.Text = "Close Browsers Before Test Start:";
            // 
            // overrideTestBrowserCheckBox
            // 
            this.overrideTestBrowserCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "OverrideTestBrowser", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.overrideTestBrowserCheckBox.Location = new System.Drawing.Point(263, 31);
            this.overrideTestBrowserCheckBox.Name = "overrideTestBrowserCheckBox";
            this.overrideTestBrowserCheckBox.Size = new System.Drawing.Size(14, 20);
            this.overrideTestBrowserCheckBox.TabIndex = 1;
            this.overrideTestBrowserCheckBox.UseVisualStyleBackColor = true;
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.ForeColor = System.Drawing.Color.Red;
            this.errorLabel.Location = new System.Drawing.Point(78, 316);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(0, 13);
            this.errorLabel.TabIndex = 14;
            this.errorLabel.Visible = false;
            // 
            // getInformativeExceptionsCheckBox
            // 
            this.getInformativeExceptionsCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "GetInformativeExceptions", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.getInformativeExceptionsCheckBox.Location = new System.Drawing.Point(263, 11);
            this.getInformativeExceptionsCheckBox.Name = "getInformativeExceptionsCheckBox";
            this.getInformativeExceptionsCheckBox.Size = new System.Drawing.Size(14, 19);
            this.getInformativeExceptionsCheckBox.TabIndex = 3;
            this.getInformativeExceptionsCheckBox.UseVisualStyleBackColor = true;
            // 
            // CloseBrowsersBeforeTestStartCheckBox
            // 
            this.CloseBrowsersBeforeTestStartCheckBox.AutoSize = true;
            this.CloseBrowsersBeforeTestStartCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "CloseBrowsersBeforeTestStart", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.CloseBrowsersBeforeTestStartCheckBox.Location = new System.Drawing.Point(263, 76);
            this.CloseBrowsersBeforeTestStartCheckBox.Name = "CloseBrowsersBeforeTestStartCheckBox";
            this.CloseBrowsersBeforeTestStartCheckBox.Size = new System.Drawing.Size(15, 14);
            this.CloseBrowsersBeforeTestStartCheckBox.TabIndex = 4;
            this.CloseBrowsersBeforeTestStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // delayBetweenCommandsTextBox
            // 
            this.delayBetweenCommandsTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.settingsDataEntityBindingSource, "DelayBetweenCommands", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.delayBetweenCommandsTextBox.Location = new System.Drawing.Point(263, 51);
            this.delayBetweenCommandsTextBox.Name = "delayBetweenCommandsTextBox";
            this.delayBetweenCommandsTextBox.Size = new System.Drawing.Size(75, 20);
            this.delayBetweenCommandsTextBox.TabIndex = 3;
            this.delayBetweenCommandsTextBox.TextChanged += new System.EventHandler(this.delayBetweenCommandsTextBox_TextChanged);
            this.delayBetweenCommandsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDelayBetweenCommans_KeyPress);
            // 
            // suspendCheckBox
            // 
            this.suspendCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.settingsDataEntityBindingSource, "SuspendTestOnFail", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.suspendCheckBox.AutoSize = true;
            this.suspendCheckBox.Location = new System.Drawing.Point(263, 96);
            this.suspendCheckBox.Name = "suspendCheckBox";
            this.suspendCheckBox.Size = new System.Drawing.Size(15, 14);
            this.suspendCheckBox.TabIndex = 15;
            this.suspendCheckBox.UseVisualStyleBackColor = true;
            // 
            // suspendLabel
            // 
            this.suspendLabel.AutoSize = true;
            this.suspendLabel.Location = new System.Drawing.Point(15, 95);
            this.suspendLabel.Name = "suspendLabel";
            this.suspendLabel.Size = new System.Drawing.Size(86, 13);
            this.suspendLabel.TabIndex = 16;
            this.suspendLabel.Text = "Suspend on Fail:";
            // 
            // SwatSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.SWATGroupBox);
            this.Name = "SwatSettings";
            this.Size = new System.Drawing.Size(392, 338);
            this.SWATGroupBox.ResumeLayout(false);
            this.SWATGroupBox.PerformLayout();
            this.TimeoutSettingsGroupBox.ResumeLayout(false);
            this.TimeoutSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataEntityBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox TimeoutSettingsGroupBox;
        private System.Windows.Forms.TextBox assertBrowserExistsTimeout;
        private System.Windows.Forms.TextBox waitForDocumentLoadTimeoutTextBox;
        private System.Windows.Forms.Label waitForDocumentLoadTimeoutLabel;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Label TimeoutSecondsLabel;
        private System.Windows.Forms.TextBox doesElementExistTimeOutTextBox;
        private System.Windows.Forms.TextBox doesElementNotExistTimeOutTextBox;
        private System.Windows.Forms.TextBox findElementTextBox;
        private System.Windows.Forms.TextBox waitForBrowserTimeoutTextBox;
        private System.Windows.Forms.TextBox attachToWindowTimeoutTextBox;
        private System.Windows.Forms.Label CloseBrowsersBeforeTestStartLabel;
        private System.Windows.Forms.CheckBox overrideTestBrowserCheckBox;
        private System.Windows.Forms.CheckBox getInformativeExceptionsCheckBox;
        private System.Windows.Forms.CheckBox CloseBrowsersBeforeTestStartCheckBox;
        private System.Windows.Forms.TextBox delayBetweenCommandsTextBox;
        public System.Windows.Forms.GroupBox SWATGroupBox;
        private System.Windows.Forms.BindingSource settingsDataEntityBindingSource;
        private System.Windows.Forms.Label suspendLabel;
        private System.Windows.Forms.CheckBox suspendCheckBox;

    }
}
