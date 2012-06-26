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
using System.Net.NetworkInformation;

namespace SWAT.Utilities
{
    public static class NetUtil
    {
        /// <summary>
        /// Checks if there is an active internet connection by pinging google.
        /// </summary>
        /// <returns>true if the internet is accessible.</returns>
        public static bool activeConnection()
        {
            return activeConnection("www.google.com");
        }

        /// <summary>
        /// Checks if there is an active internet connection by pinging the given url.
        /// </summary>
        /// <param name="url">the url to a page to check connection for.</param>
        /// <returns>true if pinging url is successful. Note: if url is empty or null, 
        /// google will be pinged instead.</returns>
        public static bool activeConnection(String url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return activeConnection();
            }
            Ping pingSender = new Ping();
            PingReply reply=null;
            try
            {
                reply = pingSender.Send(url);
            }
            catch //UNUSED Variable
            //catch (PingException e)
            {
                return false;
            }
            return reply.Status == IPStatus.Success;
        }
    }
}
