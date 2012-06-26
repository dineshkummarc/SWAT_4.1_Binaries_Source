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
using System.IO;

namespace SWAT_Editor.Controls
{
  public partial class WorkSpaceTree : UserControl
  {
    public WorkSpaceTree()
    {
      InitializeComponent();
    }

    public void LoadWorkspace(string workspacePath)
    {
      loadNode(this.workSpaceTreeView.Nodes, workspacePath);
    }

    protected void loadNode(TreeNodeCollection nodes, string path)
    {
      foreach (string dir in Directory.GetDirectories(path))
      {
        string[] tokens = dir.Split(Path.DirectorySeparatorChar);
        TreeNode node = nodes.Add(tokens[tokens.Length - 1]);
        if (Directory.GetFiles(dir, "content.txt", SearchOption.TopDirectoryOnly).Length > 0)
        {
          node.ImageIndex = 1;
        }
        else
          node.ImageIndex = 0;

        loadNode(node.Nodes, dir);
      }
    }
  }
}
