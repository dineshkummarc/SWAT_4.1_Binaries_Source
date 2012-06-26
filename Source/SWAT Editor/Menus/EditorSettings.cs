using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SWAT_Editor.Controls
{
    public partial class EditorSettings : UserControl
    {
        private Options_Control.OptionsControl optionControl;

        public EditorSettings(Options_Control.OptionsControl option_Control)
        {
            optionControl = option_Control;
            InitializeComponent();
            settingsDataEntityBindingSource.DataSource = option_Control._data;
        }

        private void txtAutosaveFrequency_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 31 && (e.KeyChar < '0' || e.KeyChar > '9'))
            {
                e.Handled = true;
            }
        }

        private void AutosaveBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);

            if (result == DialogResult.OK)
                autosaveDirectoryTextBox.Text = folderBrowserDialog1.SelectedPath.TrimEnd('\\') + "\\";
        }

        public void Save()
        {
            optionControl._data.LoadBlankForm = loadBlankFormCheckBox.Checked;
            optionControl._data.CtrlRightClickShowRecMenu = ctrlRightClickShowRecMenuCheckBox.Checked;
            optionControl._data.UseAutoComplete = useAutoCompleteCheckBox.Checked;
            optionControl._data.MinimizeToTray = minToTrayChkBox.Checked;
            optionControl._data.AutosaveEnabled = autosaveEnabledCheckBox.Checked;
            optionControl._data.AutosaveFrequency = autosaveFrequencyTextBox.Text;
            optionControl._data.AutosaveFrequencyDirectory = autosaveDirectoryTextBox.Text;
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
            
        }

    }

          
}
