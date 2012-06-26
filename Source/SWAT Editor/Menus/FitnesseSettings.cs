using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SWAT_Editor.Controls
{
    public partial class FitnesseSettings : UserControl
    {
        private Options_Control.OptionsControl optionControl;

        public FitnesseSettings(Options_Control.OptionsControl option_Control)
        {
            optionControl = option_Control;
            InitializeComponent();
            settingsDataEntityBindingSource.DataSource = option_Control._data;
        }

        private void FitnesseRootBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);

            if (result == DialogResult.OK)
                fitnesseRootDirectoryTextBox.Text = folderBrowserDialog1.SelectedPath.TrimEnd('\\') + "\\";
        }

        public void Save()
        {
            optionControl._data.FitnesseRootDirectory = fitnesseRootDirectoryTextBox.Text;
        }

    }
}
