using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SWAT_Editor.Controls
{
    public partial class SwatSettings : UserControl
    {
        private Options_Control.OptionsControl optionControl;

        public SwatSettings(Options_Control.OptionsControl option_Control)
        {
            optionControl = option_Control;
            InitializeComponent();
            settingsDataEntityBindingSource.DataSource = option_Control._data;
        }

        private void txtBrowserTimeout_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 31 && (e.KeyChar < '0' || e.KeyChar > '9'))
            {
                errorLabel.Text = "The browser timeouts should be a positive integer";
                errorLabel.Visible = true;
                e.Handled = true;
            }
            else
            {
                errorLabel.Text = "";
                errorLabel.Visible = false;
            }
        }

        private void txtDelayBetweenCommans_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 31 && (e.KeyChar < '0' || e.KeyChar > '9'))
            {
                e.Handled = true;
            }
        }

        public void Save()
        {
            optionControl._data.GetInformativeExceptions = getInformativeExceptionsCheckBox.Checked;
            optionControl._data.CloseBrowsersBeforeTestStart = CloseBrowsersBeforeTestStartCheckBox.Checked;
            optionControl._data.OverrideTestBrowser = overrideTestBrowserCheckBox.Checked;
            optionControl._data.SuspendTestOnFail = suspendCheckBox.Checked;
            optionControl._data.WaitForDocumentLoadTimeOut = waitForDocumentLoadTimeoutTextBox.Text;
            optionControl._data.AttachToWindowTimeout = attachToWindowTimeoutTextBox.Text;
            optionControl._data.WaitForBrowserTimeout = waitForBrowserTimeoutTextBox.Text;
            optionControl._data.AssertBrowserExistsTimeout = assertBrowserExistsTimeout.Text;
            optionControl._data.FindElement = findElementTextBox.Text;
            optionControl._data.DoesElementExistTimeOut = doesElementExistTimeOutTextBox.Text;
            optionControl._data.DoesElementNotExistTimeOut = doesElementNotExistTimeOutTextBox.Text;
            optionControl._data.DelayBetweenCommands = delayBetweenCommandsTextBox.Text;
        }

        private void SWATGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void TimeoutSettingsGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void delayBetweenCommandsTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
