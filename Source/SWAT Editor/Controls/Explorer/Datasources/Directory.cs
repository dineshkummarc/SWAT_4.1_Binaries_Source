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
using System.Text;
using System.IO;
using SWAT_Editor.Controls.Explorer.DataItems;

namespace SWAT_Editor.Controls.Explorer.Datasources
{
  public class Directory : IExplorerDatasource
  {
    #region IExplorerDatasource Members

    private string _location;

    public Directory(string location)
    {
      _location = location;
    }

    public List<IExplorerDataItem> GetDataItems()
    {

      List<IExplorerDataItem> items = new List<IExplorerDataItem>();

      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(_location);
    
      foreach (DirectoryInfo dirInfo in dir.GetDirectories())
      {
        items.Add(new DirectoryFolder(dirInfo));
      }

      foreach (FileInfo file in dir.GetFiles())
      {
          if (file.Name.EndsWith(".txt"))
              items.Add(new DirectoryFile(file));
          else if (file.Name.EndsWith(".sql"))
              items.Add(new DirectoryFile(file));
      }

      return items;
    }
    #endregion
  }
}
