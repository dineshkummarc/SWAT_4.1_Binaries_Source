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
using System.Reflection;

namespace SWAT_Editor
{
  public partial class AboutForm : Form
  {
    public AboutForm()
    {
      InitializeComponent();
    }

    private void tabPage1_Click(object sender, EventArgs e)
    {

    }

      private string FormatAssembly(Assembly assem)
      {
          string formatedAssembly = assem.ToString();
          formatedAssembly = formatedAssembly.Remove(formatedAssembly.IndexOf("Culture"));
          if (formatedAssembly.Contains(".D"))
              formatedAssembly = formatedAssembly.Replace(".D", " D");
          formatedAssembly = formatedAssembly.Replace(',', ' ');
          formatedAssembly = formatedAssembly.Replace('=', '.');
          formatedAssembly = formatedAssembly.Replace("Version", "v");
          return formatedAssembly;
      }

    private void AboutForm_Load(object sender, EventArgs e)
    {
        string assemblyInfo;
      foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
      {
          if (assem.ToString().Contains("SWAT"))
          {
              assemblyInfo = FormatAssembly(assem);
              label3.Text = label3.Text.ToString() + assemblyInfo.ToString() + "\n";
          }
        //listBox1.Items.Add(assem.ToString());
      }
    }


  }
}
