using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SWAT_Editor.Controls
{
    public partial class BrowserSettings : UserControl
    {
        
        private Options_Control.OptionsControl optionControl;

        public BrowserSettings(Options_Control.OptionsControl option_Control)
        {
            optionControl = option_Control;
            InitializeComponent();
            settingsDataEntityBindingSource.DataSource = option_Control._data;
        }

        private void FirefoxDirectoryBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);

            if (result == DialogResult.OK)
                FirefoxDirectoryTextBox.Text = folderBrowserDialog1.SelectedPath.TrimEnd('\\') + "\\";
        }

        private void BrowserSettings_Load(object sender, EventArgs e)
        {
        }

        public void Save()
        {
            optionControl._data.FirefoxRootDirectory = FirefoxDirectoryTextBox.Text;
            optionControl._data.SafariAddress = safariAddressTextBox.Text;
            optionControl._data.SafariPort = safariPortTextBox.Text;
            optionControl._data.IEAutoHighlight = IEEnableHighlightingCheckBox.Checked;
        }

        private void safariPortTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }


}
