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
using System.Diagnostics;

namespace SWAT_Editor.Controls
{
  [ToolboxItem(true)]
  public partial class FitnesseEngine : UserControl, IDisposable
  {

    private Process _fitProcess = new Process();
    public FitnesseEngine()
    {
      InitializeComponent();
      System.Windows.Forms.Application.ApplicationExit += new EventHandler(Application_ApplicationExit); 
    }

    void Application_ApplicationExit(object sender, EventArgs e)
    {
      _fitProcess.Kill();
    }

    public void RunCode(string code)
    {
      _fitProcess.StartInfo.CreateNoWindow = true;
      _fitProcess.StartInfo.UseShellExecute = false;
      _fitProcess.StartInfo.FileName = "java";
      
      _fitProcess.StartInfo.WorkingDirectory = SWAT.FitnesseSettings.FitnesseRootDirectory;
      _fitProcess.StartInfo.Arguments = string.Format("-cp {0}fitnesse.jar fitnesse.FitNesse -p 5676", SWAT.FitnesseSettings.FitnesseRootDirectory);
      _fitProcess.Start();

      //this.webBrowser1.Navigate("about:Please wait...");
      //here we create temp code file and execute in fitness within the web browser control.
      webBrowser1.Navigate("http://localhost:5676/SwatMacros.UltiproEverest.SwatDbDemo?test");
      //System.Net.WebClient client = new System.Net.WebClient();
      //byte[] result = client.DownloadData("http://localhost:5676/SwatMacros.UltiproEverest.SwatDbDemo?test");
      
      //this.webBrowser1.Document.Write(Encoding.Default.GetString(result));

      //_fitProcess.Kill();
    }

    #region IDisposable Members

    new void Dispose()
    {
      
      base.Dispose();
    }

    #endregion
  }
}
