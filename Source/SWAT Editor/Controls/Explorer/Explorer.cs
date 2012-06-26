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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SWAT_Editor.Controls.Explorer
{
  public delegate void LoadLocalFileHandler(string filePath);

  public partial class Explorer : UserControl
  {
    public event LoadLocalFileHandler commandEditorLoadLocalFile;
    public event LoadLocalFileHandler dBBuilderLoadLocalFile;    

    private List<string> _expandedNodes = new List<string>();    

    public Explorer()
    {
      InitializeComponent();
      this.menuStrip1.Renderer = new MyToolStripRenderer(); //this will give the background gradient the same look as a toolstip.
    }

    protected override void OnPaint(PaintEventArgs e)
    {
    //ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.DarkBlue,
    //ButtonBorderStyle.Solid);
      base.OnPaint(e);
    }

    public new TreeNode Load(string location)
    {   
      IExplorerDatasource ds = Factory.GetDatasource(location);      

      TreeNode lastAdded = this.treeView.Nodes.Add(location);
      lastAdded.ImageIndex = 1;
      lastAdded.SelectedImageIndex = 1;
      addItemsToTree(ds, lastAdded);
      return lastAdded;
      
      //addItemsToTree(ds, this.treeView.Nodes.Add(location));
    }

    public void clear()
    {
      treeView.Nodes.Clear();
    }

    private void addItemsToTree(IExplorerDatasource ds, TreeNode parentNode)
    {
        try
        {
            foreach (IExplorerDataItem item in ds.GetDataItems())
            {
                string key = item.Name;

                if (parentNode.Nodes.ContainsKey(key))
                    continue;

                if (item is SWAT_Editor.Controls.Explorer.Datasources.Directory) //it's a "folder"
                {
                    TreeNode folderNode = parentNode.Nodes.Add(key);
                    folderNode.Name = item.Name.ToString();
                    folderNode.ImageIndex = 1;
                    folderNode.SelectedImageIndex = 1;
                }
                else
                {
                    TreeNode fileNode = parentNode.Nodes.Add(key);
                    fileNode.Name = key;
                    fileNode.SelectedImageIndex = 0;
                    fileNode.ImageIndex = 0;
                }
            }
        }
        catch (Exception) //If a file path is not found, or any misc exception
        {
            if (parentNode != null && parentNode.Parent != null && parentNode.Parent.Nodes != null)
                parentNode.Parent.Nodes.Remove(parentNode);
        }
    }

    private void expandTreeItem(IExplorerDatasource ds, TreeNode parentNode)
    {
        foreach (IExplorerDataItem item in ds.GetDataItems())
        {
            if (item is SWAT_Editor.Controls.Explorer.Datasources.Directory) //it's a "folder"
            {
                addItemsToTree(item as IExplorerDatasource, parentNode.Nodes[item.Name]);                
            }
        }
    }

    private void addDirectory_Click(object sender, EventArgs e)
    {
        FolderBrowserDialog folderDialog = new FolderBrowserDialog();
        DialogResult result = folderDialog.ShowDialog();
        
        if (result == DialogResult.OK)
        {
            Load(folderDialog.SelectedPath);

                SWAT_Editor.Properties.Settings.Default.explorerDirectory = folderDialog.SelectedPath;
                SWAT_Editor.Properties.Settings.Default.Save();            
        }        
    }

    private void toolStripMenuItem1_Click(object sender, EventArgs e)
    {
      addDirectory_Click(sender, e);
    }    

    private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (e.Node.FullPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
      {
        commandEditorLoadLocalFile(e.Node.FullPath);
      }
      else if(e.Node.FullPath.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
      {
        dBBuilderLoadLocalFile(e.Node.FullPath);
      }
    }

    private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {        
        IExplorerDatasource ds = Factory.GetDatasource(e.Node.FullPath.ToString());
        expandTreeItem(ds, e.Node);
        
        if(!_expandedNodes.Contains(e.Node.Text))
            _expandedNodes.Add(e.Node.Text);
    }

      private void treeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
      {
          _expandedNodes.Remove(e.Node.Text);

          foreach (TreeNode node in e.Node.Nodes)
          {
              _expandedNodes.Remove(node.Text);
          }
      }

    private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
    {        
        treeView.Visible = false;
        foreach (TreeNode node in this.treeView.Nodes)
        {
            string path = node.FullPath.ToString();
            this.treeView.Nodes.Remove(node);
            TreeNode lastAdded = Load(path);
            
            if(_expandedNodes.Contains(lastAdded.Text))
                expandNode(lastAdded);
        }

        treeView.TreeViewNodeSorter = new NodeSorter();
        this.treeView.Sort();
        
        treeView.Visible = true;
    }
            
    private void expandNode(TreeNode node)
    {        
        node.Expand();

        foreach (TreeNode childNode in node.Nodes)
        {
            if (_expandedNodes.Contains(childNode.Text) && childNode.Parent.Equals(node))
                expandNode(childNode);
        }
    }

      private void treeview_MouseUp(object sender, MouseEventArgs e)
      {
          if (e.Button == MouseButtons.Right)
          {
              Point ClickPoint = new Point(e.X, e.Y);
              TreeNode ClickNode = treeView.GetNodeAt(ClickPoint);
              treeView.SelectedNode = ClickNode;
              if (ClickNode == null) return;
              // Convert from Tree coordinates to Screen coordinates
              Point ScreenPoint = treeView.PointToScreen(ClickPoint);
              // Convert from Screen coordinates to Form coordinates
              Point FormPoint = this.PointToClient(ScreenPoint);

              // Show context appropriate menu
              string filePath = treeView.SelectedNode.FullPath.ToString();


              if (isNodeADirectory(filePath))
                  contextMenuDirectories.Show(this, FormPoint);
              else
                  contextMenuFiles.Show(this, FormPoint);              
          }
      }

      private bool isNodeADirectory(string filePath)
      {
          FileAttributes attr = File.GetAttributes(filePath);
          return ((attr & FileAttributes.Directory) == FileAttributes.Directory);
      }

      private void treeview_MouseDown(object sender, MouseEventArgs e)
      {
          if (e.Button == MouseButtons.Left)
          {
              TreeNode node = treeView.GetNodeAt(e.X, e.Y);
              treeView.SelectedNode = node;
              if (node != null)
              {
                  treeView.DoDragDrop(node.FullPath.ToString(), DragDropEffects.Copy);
              }
          }
      }      

      private void removeContextMenu_Click(object sender, EventArgs e)
      {
          string filePath = treeView.SelectedNode.FullPath.ToString();

          DialogResult result = MessageBox.Show(string.Format("Are you sure you want to delete {0}?", treeView.SelectedNode.Text), "Delete File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
          if (result == DialogResult.Yes)
          {
              File.Delete(filePath);
              treeView.Nodes.Remove(treeView.SelectedNode);              
          }      
      }

      private void addToDirectoryMenuItem_Click(object sender, EventArgs e)
      {
          addFile(treeView.SelectedNode);
      }

      private void addToFileParentDirMenuItem_Click(object sender, EventArgs e)
      {
          addFile(treeView.SelectedNode.Parent);
      }

      private void addFile(TreeNode node)
      {
          string dirPath = node.FullPath.ToString();
          SaveFileDialog saveFileDialog1 = new SaveFileDialog();
          saveFileDialog1.InitialDirectory = dirPath.Replace(@"\\",@"\").Replace(@"\\\",@"\").Replace(@"\\\\", @"\");
          saveFileDialog1.Filter = "Text Files (*.txt)|*.txt|SQL Files (*.sql)|*.sql|All Files (*.*)|*.*";
          saveFileDialog1.FilterIndex = 1;

          if (saveFileDialog1.ShowDialog() == DialogResult.OK)
          {              
              string fileName = saveFileDialog1.FileName;

              if(!fileName.Contains(treeView.TopNode.FullPath.ToString()))
              {
                  MessageBox.Show("The file must be in a directory that is part of the tree", "Invalid directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                  addFile(node);
                  return;
              }

              FileStream file = File.Create(fileName);
              file.Close();

              IExplorerDatasource ds = Factory.GetDatasource(node.FullPath.ToString());
              addItemsToTree(ds, node);
          }
      }      

      private void addAFileToolStripMenuItem_Click(object sender, EventArgs e)
      {
          if (treeView.SelectedNode == null)
              treeView.SelectedNode = treeView.TopNode;

          string filePath = treeView.SelectedNode.FullPath.ToString();

          if(isNodeADirectory(filePath))
              addFile(treeView.SelectedNode);
          else
              addFile(treeView.SelectedNode.Parent);
      }
}

public class MyToolStripRenderer
        : ToolStripProfessionalRenderer
  {
    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
      if (e.ToolStrip is MenuStrip)
      {
        ControlPaint.DrawBorder(e.Graphics, e.ToolStrip.ClientRectangle, Color.DarkBlue,
        ButtonBorderStyle.Solid);
      }
      else
        base.OnRenderToolStripBorder(e);
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
      if (e.ToolStrip is MenuStrip)
      {
        // RenderToolStripBackgroundInternal(ToolStripRenderEventArgs e)
        MethodInfo mi = typeof(ToolStripProfessionalRenderer).GetMethod("RenderToolStripBackgroundInternal",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (mi != null)
        {
          mi.Invoke(this, new object[] { e });
        }
      }
      else
      {
        base.OnRenderToolStripBackground(e);
      }
    }
  }

    public class NodeSorter : IComparer
    {        
        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            if (tx.ImageIndex == ty.ImageIndex)
                return string.Compare(tx.Text, ty.Text);
            else
                return tx.ImageIndex == 1 ? -1 : 1;
        }
    }

}
