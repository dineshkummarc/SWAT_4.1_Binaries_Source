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
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

namespace SWAT
{
    [SWAT.NCover.CoverageExclude]
    internal class Utilities
    {
        public static void WriteToConsole(string message)
        {
            string formattedMessage = string.Format("({0}) - {1}", DateTime.Now.ToString(), message);

            #if DEBUG
            Console.WriteLine(formattedMessage);
            #endif

            Debug.WriteLine(formattedMessage);
        }

        public static string FormatUrl(string url)
        {
            if (url.Contains(@"\"))
                return string.Format("file://{0}", url.Replace('\\', '/'));

            Regex regexp = new Regex(@"^(http|https|ftp|file)\://",
                            RegexOptions.ECMAScript | RegexOptions.IgnoreCase);

            if (!regexp.IsMatch(url))
                return string.Format("http://{0}", url);

            return url;
        }

        public static void WriteResponse(ChromeResponse response)
        {
            string value = "null";
            if (response.Value != null)
                value = response.Value.ToString();
            string formattedMessage = string.Format("({0}) Response: Status Code: {1} - Value: ( {2} )\n", 
                DateTime.Now.ToString(), response.StatusCode.ToString(), value);
            
#if DEBUG
            //Console.WriteLine(formattedMessage);
#endif

            Debug.WriteLine(formattedMessage);
        }

        public static void WriteCommand(ChromeCommand message)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append(string.Format("({0}) Command: {1} - Arguments: [", DateTime.Now.ToString(), message.Command));

            foreach (string arg in message.Arguments.Keys)
            {
                cmd.Append(string.Format("({0} : {1}) ", arg, message.Arguments[arg]));    
            }

            cmd.Append("]");

#if DEBUG
            //Console.WriteLine(cmd);
#endif

            Debug.WriteLine(cmd);
        }
    }
}
