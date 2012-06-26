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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Reflection;


namespace SWAT_Editor
{
    class RecentFilesHandler
    {
        private int MAX_ITEMS = 5;
        //string[] recentFiles = new string[5];
        List<string> recentFiles = new List<string>();
        string fileName = "recentFiles.dat";
        string filePath = "";

        public void Init()
        {
            filePath = Assembly.GetExecutingAssembly().Location;
            filePath = filePath.Remove(filePath.LastIndexOf("\\")+1);
            filePath += fileName;
            for (int i = 0; i < MAX_ITEMS; i++) recentFiles.Add("");
            Stream stream = null;

            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                recentFiles = (List<string>)formatter.Deserialize(stream);
            }
            catch
            {
                // do nothing, just ignore any possible errors
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
        }

        public List<string> GetAll()
        {
            return recentFiles;
        }

        private void push(string fileFullPath)
        {
            if (!recentFiles.Contains(fileFullPath))
            {
                for (int i = MAX_ITEMS - 2; i >= 0; i--)
                {
                    try
                    {
                        recentFiles[i + 1] = recentFiles[i];
                    }
                    catch { }
                }
                recentFiles[0] = fileFullPath;
            }
        }

        public void Add(string fileFullPath)
        {
            this.push(fileFullPath);

            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, recentFiles);
            }
            catch
            {
                // do nothing, just ignore any possible errors
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
        }

        public string Get(int index)
        {
            try
            {
                return recentFiles[index];
            }
            catch
            {
                return "";
            }
        }

        /*private void populateRecentFiles()
        {
            toolStripMenuRecentFiles.DropDownItems.Clear();
            recentFiles[0] = "file001";
            recentFiles[1] = "file002";
            foreach (string filename in recentFiles)
            {
                if (filename != null) toolStripMenuRecentFiles.DropDownItems.Add(filename);
            }
        }*/

    }
}
