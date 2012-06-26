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
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SWAT
{
    [SWAT.NCover.CoverageExclude]
    public static class ChromeProcess
    {
        #region Constants

        private const string ExtensionDir = @"C:\SWAT\trunk\SWAT.Core\Chrome\Extension\ChromeSWAT";

        #endregion

        #region Public Methods

        public static void Start(int port)
        {
            Utilities.WriteToConsole("Starting Google Chrome process...");

            Process proc = new Process();

            proc.StartInfo.FileName = GetPathToExecutable();

            if (!File.Exists(proc.StartInfo.FileName))
                throw new BrowserNotInstalledException("Google Chrome isn't installed on this user account.");

            proc.StartInfo.Arguments = GetCommandLineArgs(port);

            proc.Start();
        }

        #endregion

        #region Private Methods

        private static string GetPathToExecutable()
        {
            return string.Format("{0}{1}",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                            @"\Google\Chrome\Application\chrome.exe");
        }

        private static string GetCommandLineArgs(int port)
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append(string.Format(" --load-extension=\"{0}\"", ExtensionDir));
            arguments.Append(" --activate-on-launch");
            arguments.Append(" --homepage=about:blank");
            arguments.Append(" --no-first-run");
            arguments.Append(" --disable-hang-monitor");
            arguments.Append(" --disable-popup-blocking");
            arguments.Append(" --disable-prompt-on-repost");
            arguments.Append(" --no-default-browser-check");
            arguments.Append(string.Format(" http://{0}:{1}/?doConnect=true", Environment.MachineName.ToLower(), port));

            return arguments.ToString();
        }

        #endregion
    }
}
