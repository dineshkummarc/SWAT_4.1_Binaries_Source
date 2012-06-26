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
using System.IO;

namespace SWAT_Editor
{
  public partial class CreateSnippetForm : Form
  {
    public CreateSnippetForm()
    {
      InitializeComponent();
    }

    private void browseButton_Click(object sender, EventArgs e)
    {
      openfile.DefaultExt = ".txt";
      openfile.Filter = "Text files|*.txt";
      
      DialogResult result = openfile.ShowDialog(this);
      
      if (result == DialogResult.OK)
      {
        StreamReader stream = new StreamReader(openfile.OpenFile());
        txtFunction.AppendText(stream.ReadToEnd());
      }
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
      string docpath = SWAT_Editor.Properties.Settings.Default.CustomSnippetDirectory;
      DirectoryInfo swatPath = new DirectoryInfo(docpath);

      SaveFileDialog saveWin = new SaveFileDialog();
      saveWin.AddExtension = true;
      saveWin.DefaultExt = ".txt";
      saveWin.Filter = "Text files|*.txt";
      saveWin.InitialDirectory = swatPath.ToString();
      saveWin.ValidateNames = true;
      DialogResult result = saveWin.ShowDialog();

      if (result == DialogResult.OK)
      {


        StreamWriter saveWrite = new StreamWriter(saveWin.FileName);
        saveWrite.Write(txtFunction.Text);
        saveWrite.Close();
        this.Close();
      }
      


    }

    private void txtFunction_TextChanged(object sender, EventArgs e)
    {
      if (txtFunction.Text == "" || txtFunction.Text == null)
        saveButton.Enabled = false;
      else
        saveButton.Enabled = true;
    }
  }
}
