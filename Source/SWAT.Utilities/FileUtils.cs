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

namespace SWAT.Utilities
{
    public static class FileUtils
    {
        /// <summary>
        /// Determines whether the specified directory name exists.
        /// </summary>
        /// <param name="sDirName">name of directory to check for</param>
        /// <returns>True if the directory exists, False otherwise</returns>
        public static bool DirExists(string sDirName)
        {
            try
            {
                return (System.IO.Directory.Exists(sDirName));    //Check for file
            }
            catch (Exception)
            {
                return (false);                                 //Exception occured, return False
            }
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="sPathName">file to check for</param>
        /// <returns>True if file exists, False otherwise</returns>
        public static bool FileExists(string sPathName)
        {
            return File.Exists(sPathName);
            /*
            try
            {
                return (System.IO.Directory.Exists(sPathName));  //Exception for folder
            }
            catch (Exception)
            {
                return (false);                                   //Error occured, return False
            }
             */
        }
        /// <summary>
        /// Compares the contents of two files.
        /// </summary>
        /// <param name="f1">a file.</param>
        /// <param name="f2">another file.</param>
        /// <returns></returns>
        public static bool FileCompare(FileInfo f1, FileInfo f2)
        {
            FileStream fs1 = File.OpenRead(f1.FullName);
            FileStream fs2 = File.OpenRead(f2.FullName);

            //compare contents
            // Compare files  
            int i = 0, j = 0;
            try
            {
                do
                {
                    i = fs1.ReadByte();
                    j = fs2.ReadByte();
                    if (i != j) break;
                } while (i != -1 && j != -1);
            }
            finally
            {
                fs1.Close();
                fs2.Close();
            }
            if (i != j) return false;
            return true;
        }
        /// <summary>
        /// Gets all the files in a directory whose name contains a given string. 
        /// </summary>
        /// <param name="folderPath">a path to the folder to examine.</param>
        /// <param name="searchPattern">i.e. "*.txt"</param>
        /// <returns>a collection of files whose name contain "fileNameSubstring"</returns>
        public static FileInfo[] getFilesContaining(String folderPath, String searchPattern)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            return dir.GetFiles(searchPattern);
        }
    }
}
