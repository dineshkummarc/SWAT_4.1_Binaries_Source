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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SWAT_Editor.Controls.DBBuilder;
namespace SWAT_Editor.Controls.DBBuilder.ColumnEditor
{
    public partial class ColumnEditor : Form
    {
        private DBBuilder dBuilder;

        public ColumnEditor()
        {
            InitializeComponent();
        }

        public ColumnEditor(DBBuilder dBuilder)
        {
            this.dBuilder = dBuilder;
            InitializeComponent();
            Load();
        }

        public new void Load()
        {
            foreach (DataGridViewColumn c in dBuilder.GetGrid.Columns)
            {
                if (!(c.Name.Equals("includeRow") || c.Name.Equals("modifier")))
                {
                    if (c.Visible)
                        this.selectedColumns.Items.Add(c.HeaderText);
                    else
                        this.hiddenColumns.Items.Add(c.HeaderText);
                }
            }
            updateStatusOfButtons();
        }

        private void selectAll_Click(object sender, EventArgs e)
        {
            foreach (object item in this.hiddenColumns.Items)
            {
                this.selectedColumns.Items.Add(item);
            }
            this.hiddenColumns.Items.Clear();
            updateStatusOfButtons();
        }

        private void deselectAll_Click(object sender, EventArgs e)
        {
            foreach (object item in this.selectedColumns.Items)
            {
                this.hiddenColumns.Items.Add(item);
            }
            this.selectedColumns.Items.Clear();
            updateStatusOfButtons();
        }

        private void selectOne_Click(object sender, EventArgs e)
        {
            List<object> removedItems = new List<object>();
            foreach (object item in this.hiddenColumns.SelectedItems)
            {
                this.selectedColumns.Items.Add(item);
                removedItems.Add(item);
            }
            foreach (object item in removedItems)
            {
                this.hiddenColumns.Items.Remove(item);
            }

            updateStatusOfButtons();
        }

        private void deselectOne_Click(object sender, EventArgs e)
        {
            List<object> removedItems = new List<object>();
            foreach (object item in this.selectedColumns.SelectedItems)
            {
                this.hiddenColumns.Items.Add(item);
                removedItems.Add(item);
            }
            foreach (object item in removedItems)
            {
                this.selectedColumns.Items.Remove(item);
            }

            updateStatusOfButtons();
        }

        private void UpdateColumns()
        {
            foreach (DataGridViewColumn c in dBuilder.GetGrid.Columns)
            {
                if (!(c.Name.Equals("includeRow") || c.Name.Equals("modifier")))
                {
                    if (selectedColumns.Items.Contains(c.Name))
                        c.Visible = true;
                    else
                        c.Visible = false;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            UpdateColumns();
            this.Close();
        }

        private void updateStatusOfButtons()
        {
            if (selectedColumns.Items.Count <= 0)
            {
                deselectAll.Enabled = false;
                deselectOne.Enabled = false;
            }
            else
            {
                deselectAll.Enabled = true;
                deselectOne.Enabled = true;
            }

            if (hiddenColumns.Items.Count <= 0)
            {
                selectAll.Enabled = false;
                selectOne.Enabled = false;
            }
            else
            {
                selectAll.Enabled = true;
                selectOne.Enabled = true;
            }
        }                   
    }
}
