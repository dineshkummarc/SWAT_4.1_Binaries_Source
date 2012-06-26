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
using System.Net;
using System.Text;

namespace SWAT
{
    /// <summary>
    /// Provides a mechanism to handle requests from the Chrome Extension
    /// </summary>
    [SWAT.NCover.CoverageExclude]
    internal class ExtensionRequestPacket
    {
        private HttpListenerContext packetContext;
        private ChromeExtensionPacketType extensionPacketType = ChromeExtensionPacketType.Unknown;
        private Guid packetId = Guid.NewGuid();
        private string content = string.Empty;

        /// <summary>
        /// Initializes a new instance of the ExtensionRequestPacket class
        /// </summary>
        /// <param name="currentContext">Current HTTP Context in use</param>
        public ExtensionRequestPacket(HttpListenerContext currentContext)
        {
            packetContext = currentContext;

            if (string.Compare(packetContext.Request.HttpMethod, "get", StringComparison.OrdinalIgnoreCase) == 0)
            {
                extensionPacketType = ChromeExtensionPacketType.Get;
            }
            else
            {
                extensionPacketType = ChromeExtensionPacketType.Post;
            }

            DateTime readTimeout = DateTime.Now.AddSeconds(10);
            HttpListenerRequest request = packetContext.Request;

            int length = (int)request.ContentLength64;
            byte[] packetDataBuffer = new byte[length];
            int totalBytesRead = 0;

            do
            {
                totalBytesRead += request.InputStream.Read(packetDataBuffer, totalBytesRead, length - totalBytesRead);
            }
            while (totalBytesRead < length && DateTime.Now <= readTimeout);

            content = Encoding.UTF8.GetString(packetDataBuffer);
        }

        ///// <summary>
        ///// Gets the Unique packet ID
        ///// </summary>
        //public Guid ID
        //{
        //    get { return packetId; }
        //}

        /// <summary>
        /// Gets the Packet Type
        /// </summary>
        public ChromeExtensionPacketType PacketType
        {
            get { return extensionPacketType; }
        }

        /// <summary>
        /// Gets the Context
        /// </summary>
        public HttpListenerContext Context
        {
            get { return packetContext; }
        }

        /// <summary>
        /// Gets the Content 
        /// </summary>
        public string Content
        {
            get { return content; }
        }
    }
}
