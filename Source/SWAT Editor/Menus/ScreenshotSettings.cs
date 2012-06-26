using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SWAT_Editor.Controls
{
    public partial class ScreenshotSettings : UserControl
    {
        private Options_Control.OptionsControl optionControl;

        public ScreenshotSettings(Options_Control.OptionsControl option_Control)
        {
            optionControl = option_Control;
            InitializeComponent();
            settingsDataEntityBindingSource.DataSource = option_Control._data;
        }

        private void ScreenshotBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
                imageFileDirectoryTextBox.Text = folderBrowserDialog1.SelectedPath.TrimEnd('\\') + "\\";
        }

        private void AllScreens_CheckedChanged(object sender, EventArgs e)
        {
            if (allScreensSnapshotRadioButton.Checked)
            {
                optionControl._data.AllScreensSnapshot = true;
                optionControl._data.WindowOnlyScreenshot = false;
                lblScreenShotMessage.Visible = true;
            }
            else
            {
                optionControl._data.AllScreensSnapshot = false;
                optionControl._data.WindowOnlyScreenshot = true;
                lblScreenShotMessage.Visible = false;
            }
        }
        private void takeScreenShot_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            ScreenshotDetailsEnable(chk);

            optionControl._data.TakeSnapshots = chk.Checked;
        }

        private void ScreenshotDetailsEnable(CheckBox chk)
        {
            if (chk.Checked)
            {
                ScreenshotDetailsGroupBox.Enabled = true;
            }
            else
            {
                ScreenshotDetailsGroupBox.Enabled = false;
            }
        }
        private void ScreenshotSettings_Load(object sender, EventArgs e)
        {
            takeSnapshotsCheckBox.Checked = optionControl._data.TakeSnapshots;
            ScreenshotDetailsEnable(takeSnapshotsCheckBox);
            allScreensSnapshotRadioButton.Checked = optionControl._data.AllScreensSnapshot;
            windowOnlyScreenshotRadioButton.Checked = optionControl._data.WindowOnlyScreenshot;
        }

        public void Save()
        {
            optionControl._data.ImageFileDirectory = imageFileDirectoryTextBox.Text;
            optionControl._data.TakeSnapshots = takeSnapshotsCheckBox.Checked;
            optionControl._data.AllScreensSnapshot = allScreensSnapshotRadioButton.Checked;
            optionControl._data.WindowOnlyScreenshot = windowOnlyScreenshotRadioButton.Checked;
        }
    }

}
