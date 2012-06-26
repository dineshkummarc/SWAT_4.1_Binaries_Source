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

namespace SWAT_Editor.Controls
{
    public partial class SaveDialog : Form
    {
        public String dialogResult;
        
        public SaveDialog()
        {
            InitializeComponent();
            dialogResult = null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            dialogResult = "Save";            
            this.Close();
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            dialogResult = "Save All";
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            dialogResult = "Cancel";
            this.Close();
        }

    }
}
