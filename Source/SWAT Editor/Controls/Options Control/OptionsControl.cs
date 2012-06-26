/********************************************************************************
    This file is part of Simple Web Automation Toolkit, 
    Copyright (C) 2007 by Ultimate Software, Inc. All rights reserved.

    Simple Web Automation Toolkit is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License version 3 as published by
    the Free Software Foundation; 

    Simple Web Automation Toolkit is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

 */

/********************************************************************************/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SWAT_Editor.Controls.Options_Control
{
    public partial class OptionsControl : UserControl
    {
        public SettingsDataEntity _data = new SettingsDataEntity();

        private UserControl swatSettings;
        private UserControl browserSettings;
        private UserControl databaseSettings;
        private UserControl editorSettings;
        private UserControl fitnesseSettings;
        private UserControl screenshotSettings;


        #region Set Up

        public OptionsControl()
        {
            InitializeComponent();
            settingsDataEntityBindingSource.DataSource = _data;

            swatSettings = new SwatSettings(this);
            browserSettings = new BrowserSettings(this);
            databaseSettings = new DatabaseSettings(this);
            editorSettings = new EditorSettings(this);
            fitnesseSettings = new FitnesseSettings(this);
            screenshotSettings = new ScreenshotSettings(this);
        }

        #endregion

        #region events


        private void OKButton_Click(object sender, EventArgs e)
        {
            ((ScreenshotSettings)screenshotSettings).Save();
            ((SwatSettings)swatSettings).Save();
            ((EditorSettings)editorSettings).Save();
            ((BrowserSettings)browserSettings).Save();
            ((DatabaseSettings)databaseSettings).Save();
            ((FitnesseSettings)fitnesseSettings).Save();
            SWAT_Editor.Properties.Settings.Default.Save();
            SWAT.UserConfigHandler.Save();
            SWAT_Editor.Properties.Settings.Default.Reload();
            this.ParentForm.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            SWAT_Editor.Properties.Settings.Default.Reload();
            this.ParentForm.Close();
        }

        private void CategoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string category = "";

            TreeView treeView = (TreeView)sender;
            TreeNode node = treeView.SelectedNode;

            if (node != null)
            {
                category = node.Text;
            }

            switch (category)
            {
                case "SWAT Settings":
                    swatSettings.BringToFront();
                    break;
                case "Editor Settings":
                    editorSettings.BringToFront();
                    break;
                case "Browser Settings":
                    browserSettings.BringToFront();
                    break;
                case "Database Settings":
                    databaseSettings.BringToFront();
                    break;
                case "Fitnesse Settings":
                    fitnesseSettings.BringToFront();
                    break;
                case "Screenshot Settings":
                    screenshotSettings.BringToFront();
                    break;
                default:
                    break;
            }
        }

        #endregion

        private void OptionsControl_Load(object sender, EventArgs e)
        {
            this.mainSplitContainer.Panel2.Controls.Add(swatSettings);
            this.mainSplitContainer.Panel2.Controls.Add(editorSettings);
            this.mainSplitContainer.Panel2.Controls.Add(browserSettings);
            this.mainSplitContainer.Panel2.Controls.Add(databaseSettings);
            this.mainSplitContainer.Panel2.Controls.Add(fitnesseSettings);
            this.mainSplitContainer.Panel2.Controls.Add(screenshotSettings);
            swatSettings.BringToFront();
        }

    }
}
