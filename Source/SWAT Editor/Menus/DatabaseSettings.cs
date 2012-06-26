using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SWAT_Editor.Controls
{
    public partial class DatabaseSettings : UserControl
    {
        private Options_Control.OptionsControl optionControl;

        public DatabaseSettings(Options_Control.OptionsControl option_Control)
        {
            optionControl = option_Control;
            InitializeComponent();
            settingsDataEntityBindingSource.DataSource = option_Control._data;
        }

        private void ConnectionTimeout_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 31 && (e.KeyChar < '0' || e.KeyChar > '9'))
            {
                e.Handled = true;
            }
        }

        public void Save()
        {
            optionControl._data.ConnectionTimeout = connectionTimeoutTextBox.Text;
        }
    }
}
