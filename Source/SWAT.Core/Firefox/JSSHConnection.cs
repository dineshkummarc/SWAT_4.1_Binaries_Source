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
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SWAT
{
    public class JSSHConnection : IDisposable
    {
        #region Constructors

        public JSSHConnection()
        {
            ConnectToJSSH();
        }

        public JSSHConnection(string swatGuid) : this()
        {
            CopySessionVariables(swatGuid);
        }

        #endregion

        #region Private Instance Variables

        private const int port = 9997;
        private bool disposed;
        protected bool isFF4 = false;

        private readonly System.Net.IPEndPoint localEndPoint =
            new System.Net.IPEndPoint(System.Net.Dns.GetHostAddresses("127.0.0.1")[0], port);

        protected Socket jsshSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        #endregion

        #region Properties

        public bool Connected
        {
            get { return IsJSSHConnected(); }
        }

        #endregion

        #region Public Functions

        #region ConnectToJSSH

        public bool ConnectToJSSH()
        {
            if (!Connected)
            {
                jsshSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    jsshSocket.Connect(localEndPoint);
                }
                catch (SocketException)
                {
                    return false;
                }

                SetSocketProperties();
                ClearWelcomeMessage();
            }

            return Connected;
        }

        private void SetSocketProperties()
        {
            jsshSocket.SendTimeout = 2000;
            jsshSocket.ReceiveTimeout = 2000;
            jsshSocket.NoDelay = false;
            jsshSocket.Blocking = true;
            jsshSocket.ReceiveBufferSize = 8192;
            jsshSocket.SendBufferSize = 8192;
        }

        protected virtual void ClearWelcomeMessage()
        {
            string welcome;
            DateTime timeout = DateTime.Now.AddSeconds(5);
            do
            {
                welcome = GetMessage();
            } while (welcome.Equals("") && DateTime.Now < timeout);
        }

        #endregion

        public virtual void Disconnect()
        {
            if (jsshSocket != null)
            {
                if (jsshSocket.Connected)
                {
                    jsshSocket.Shutdown(SocketShutdown.Both);
                    jsshSocket.Disconnect(false);
                }
                jsshSocket = null;
            }
        }

        #region SendMessage

        public string SendMessage(string msg)
        {
            return SendMessage(msg, true, true);
        }

        public string SendMessage(string msg, bool receive)
        {
            return SendMessage(msg, receive, true);
        }

        public virtual string SendMessage(string msg, bool receive, bool setContext)
        {
            //JSSH ignores setContext, Repl uses context evaluation for specific calls on the FirefoxXULDOM);
            if (!ConnectToJSSH())
            {
                return "SOCKET DISCONNECTED.";
            }

            if (isFF4)
                Thread.Sleep(5);

            ClearJSSHBuffer();
            PollJSSHBuffer(SelectMode.SelectWrite, 1000);

            int sent = 0;
            int size = Encoding.Default.GetBytes(msg + "\n\r").GetLength(0);
            Byte[] data = Encoding.Default.GetBytes(msg + "\n\r");

            while (sent < size)
            {
                try
                {
                    sent += jsshSocket.Send(data, sent, size - sent, SocketFlags.None);
                }
                catch (SocketException e)
                {
                    if (IsFatalSocketError(e))
                        return string.Format("SOCKET Exception: {0}", e.Message);
                    Thread.Sleep(25);
                }
            }
            return receive ? GetResponse() : "";
        }

        private void ClearJSSHBuffer()
        {
            if (!Connected)
                return;
            //If there is stuff to be read clear it before continuing
            while (jsshSocket.Available > 0)
            {
                GetMessage();
            }
        }

        private string GetResponse()
        {
            string result = String.Empty;
            DateTime timeout = DateTime.Now.AddSeconds(2);

            while (result.Equals("") && DateTime.Now < timeout)
                result = GetMessage();

            return result;
        }

        #endregion

        #endregion

        #region Private Functions

        protected bool IsJSSHConnected()
        {
            if (jsshSocket == null)
            {
                return false;
            }

            bool currentBlockingState = jsshSocket.Blocking;
            try
            {
                jsshSocket.Blocking = false;
                jsshSocket.Send(new byte[1], 0, SocketFlags.None);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode != SocketError.WouldBlock)
                {
                    return false;
                }
            }
            finally
            {
                jsshSocket.Blocking = currentBlockingState;
            }

            return jsshSocket.Connected;
        }

        protected string GetMessage()
        {
            PollJSSHBuffer(SelectMode.SelectRead, 100);

            byte[] buffer = new byte[8192];

            int bufSize = jsshSocket.Available;
            while (true)
            {
                if (bufSize == jsshSocket.Available)
                    break;
                bufSize = jsshSocket.Available;
            }
            try
            {
                jsshSocket.Receive(buffer);
            }
            catch (SocketException e)
            {
                return IsFatalSocketError(e) ? "SOCKET DISCONNECTED" : "";
            }

            string ret = Encoding.ASCII.GetString(buffer).Replace("\n>", "").Replace("\0", "").Trim();
            if (ret.Contains("?"))
            {
                char[] change = ret.ToCharArray();
                for (int i = 0; i < change.Length; i++)
                    if (change[i] == '?' && buffer[i] > 128)
                        change[i] = (char) buffer[i];
                ret = new String(change);
            }
            return ret;
        }

        private void PollJSSHBuffer(SelectMode mode, int timeoutMilliseconds)
        {
            DateTime timeout = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
            while (!jsshSocket.Poll(5000, mode) && DateTime.Now < timeout)
            {
                Thread.Sleep(0);
            }
        }

        private static bool IsFatalSocketError(SocketException e)
        {
            return (e.SocketErrorCode != SocketError.WouldBlock && e.SocketErrorCode != SocketError.IOPending) &&
                   e.SocketErrorCode != SocketError.NoBufferSpaceAvailable;
        }

        protected void CopySessionVariables(string swatGuid)
        {
            StringBuilder javascript = new StringBuilder();
            javascript.Append("var allWindows = getWindows(); var windowIndex, browser, doc, firefoxWindow;");
            javascript.Append(
                "for (windowIndex = 0; windowIndex < allWindows.length; windowIndex++) { var currentWindow = allWindows[windowIndex];");
            javascript.Append("if (currentWindow.swatGuid && (currentWindow.swatGuid == '" + swatGuid + "'))");
            javascript.Append(
                "{ firefoxWindow = currentWindow; browser = firefoxWindow.getBrowser(); doc = browser.contentDocument; } }");
            do
            {
                SendMessage(javascript.ToString());
            } while (SendMessage("firefoxWindow").Contains("firefoxWindow is undefined"));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Disconnect();
                disposed = true;
            }
        }

        #endregion
    }
}
