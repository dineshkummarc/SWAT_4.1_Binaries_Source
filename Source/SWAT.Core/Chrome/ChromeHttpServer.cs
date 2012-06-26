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
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace SWAT
{
    [NCover.CoverageExclude]
    internal class ChromeHttpServer : IDisposable
    {
        #region Constructors

        public ChromeHttpServer(Chrome b)
        {
            browser = b;
            chromeListener = CreateListener();
            chromeListener.BeginGetContext(OnClientConnect, chromeListener);
        }

        #endregion

        #region Public Properties

        public int Port { get; private set; }
        public bool Connected { get { return pendingRequestQueue.Count > 0; } }
        internal bool Disposed { get; private set; }

        #endregion

        #region Private Variables

        private readonly Chrome browser;
        private HttpListener chromeListener;
        private static readonly HashSet<int> FailedPorts = new HashSet<int>();
        private static readonly Random PortRandomizer = new Random();
        private const int PacketTimeoutSeconds = 300;
        private const int ConnectionTimeoutSeconds = 60;
        private bool Connecting;
        private bool receivedResponse;
        readonly object responseLock = new object();

        private readonly List<ExtensionRequestPacket> pendingRequestQueue = new List<ExtensionRequestPacket>();
        private readonly ManualResetEvent postRequestReceived = new ManualResetEvent(false);

        //private const string HostPageHtml = "<html><head><script type='text/javascript'>if (window.location.search == '') { setTimeout(\"window.location = window.location.href + '?reloaded'\", 5000); }</script></head><body><p>SWAT server started and connected.  Please leave this tab open.</p></body></html>";
        private const string HostPageHtml = "<html><head><title>SWAT HTTP Server</title></head><body><p>SWAT server started and connected.  Please leave this tab open.</p></body></html>";
        private const string BlankPageHtml = "<html><head><title>about:swat</title></head><body></body></html>";

        #endregion

        #region Public Methods

        public bool ConnectToExtension()
        {
            if (!Connected)
            {
                ChromeProcess.Start(Port);
                return postRequestReceived.WaitOne(TimeSpan.FromSeconds(ConnectionTimeoutSeconds));
            }

            // Wait for a POST request to be pending from the Chrome extension.
            // When one is received, get the packet from the queue.
            return postRequestReceived.WaitOne(TimeSpan.FromSeconds(PacketTimeoutSeconds)); ;
        }

        public void SendMessage(ChromeCommand command)
        {
            SendMessage(command, true);
        }

        public void SendMessage(ChromeCommand command, bool printConsoleMessages)
        {
            if (printConsoleMessages)
            {
                Utilities.WriteCommand(command);
            }

            if (!ConnectToExtension())
            {
                throw new ChromeExtensionNotConnectedException();
            }

            // ExtensionRequestPacket packet = pendingRequestQueue.Dequeue();
            ExtensionRequestPacket packet = pendingRequestQueue[0];

            pendingRequestQueue.RemoveAt(0);

            // Send the response to the Chrome extension.
            string commandString = command.ToString();
            //Utilities.WriteToConsole(string.Format("Executing {0} ", commandString));
            browser.PreviousRequest = command.Command;

            Send(packet, commandString, "application/json; charset=UTF-8");
        }

        public ChromeResponse SendMessageWithTimeout(ChromeCommand command, int timeout)
        {
            ChromeResponse response;

            Utilities.WriteCommand(command);
            DateTime endTime = DateTime.Now.AddSeconds(timeout);
            do
            {
                SendMessage(command, false);
                response = HandleResponse(command, false);

                if (response.StatusCode == StatusCode.SUCCESS)
                    break;
            }
            while (DateTime.Now < endTime);

            Utilities.WriteResponse(response);
            return response;
        }

        public ChromeResponse HandleResponse(ChromeCommand command)
        {
            return HandleResponse(command, true);
        }

        public ChromeResponse HandleResponse(ChromeCommand command, bool printConsoleMessages)
        {
            // Wait for a POST request to be pending from the Chrome extension.
            // Note that we need to leave the packet in the queue for the next
            // send message.

            DateTime timeout = DateTime.Now.AddSeconds(PacketTimeoutSeconds);
            browser.DialogInterrupted = false;

            using (DialogWatcher watcher = new DialogWatcher(browser))
            {
                while (DateTime.Now < timeout)
                {
                    lock (responseLock)
                    {
                        if (watcher.FoundDialog)
                        {
                            browser.DialogInterrupted = true;
                            break;
                        }

                        if (receivedResponse)
                            break;
                    }
                }
            }

            ChromeResponse response = new ChromeResponse();

            if (browser.DialogInterrupted)
            {
                response.StatusCode = StatusCode.SUCCESS;
                response.Value = "A JSDialog caused a timeout.";
                return response;
            }

            if (!receivedResponse)
            {
                throw new ChromeCommandTimedOutException(command.Command, PacketTimeoutSeconds);
            }

            // ExtensionRequestPacket packet = pendingRequestQueue.Peek();
            // If the page cannot be found, this gets index out of bounds
            ExtensionRequestPacket packet = pendingRequestQueue[0];

            // Parse the packet content, and deserialize from a JSON object.
            string responseString = ParseResponse(packet.Content);

            if (!string.IsNullOrEmpty(responseString))
            {
                response = JsonConvert.DeserializeObject<ChromeResponse>(responseString);

                if (response == null)
                    throw new ChromeException("No response was returned by the extension.");

                response.Value = response.Value ?? "";
            }

            string valueAsString = response.Value as string;
            if (valueAsString != null)
            {
                // First, collapse all \r\n pairs to \n, then replace all \n with
                // System.Environment.NewLine. This ensures the consistency of 
                // the values.
                response.Value = valueAsString.Replace("\r\n", "\n").Replace("\n", System.Environment.NewLine);
            }
            //Utilities.WriteToConsole(string.Format("Response was {0}", response.ToString()));

            if (printConsoleMessages)
            {
                Utilities.WriteResponse(response);
            }

            switch (response.StatusCode)
            {
                case StatusCode.BADCOMMAND:
                case StatusCode.BADJAVASCRIPT:
                case StatusCode.CONTENTSCRIPTCONNECTFAIL:
                case StatusCode.CONTENTSCRIPTERROR:
                case StatusCode.ELEMENTNOTVISIBLE:
                case StatusCode.INVALIDELEMENTSTATE:
                case StatusCode.NOSUCHFRAME:
                case StatusCode.STALEELEMENTREFERENCE:
                case StatusCode.UNHANDLEDERROR:
                    throw new ChromeException(response.Value.ToString());
            }

            return response;
        }

        #endregion

        #region Private Methods

        private static int GetAvailablePort()
        {
            int port;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            while (true)
            {
                port = PortRandomizer.Next(1025, 65535); // be nice, don't use ports below 1025
                if (FailedPorts.Contains(port))
                    continue;
                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
                bool isAvailable = tcpConnInfoArray.All(tcpInfo => tcpInfo.LocalEndPoint.Port != port);

                if (isAvailable)
                    break;
            }

            return port;
        }

        private HttpListener CreateListener()
        {
            HttpListener httpListener;
            DateTime timeout = DateTime.Now.AddSeconds(30);
            bool foundPort = false;
            do
            {
                httpListener = new HttpListener();
                Port = GetAvailablePort();
                string uriPref = string.Format("http://{0}:{1}/", Environment.MachineName.ToLower(), Port);
                httpListener.Prefixes.Add(uriPref);
                try
                {
                    httpListener.Start();
                    foundPort = true;
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    FailedPorts.Add(Port);
                }
            } while (DateTime.Now < timeout);

            if (!foundPort)
                throw new NoAvailablePortException();

            return httpListener;
        }

        private void OnClientConnect(IAsyncResult asyncResult)
        {
            try
            {
                HttpListener listener = (HttpListener)asyncResult.AsyncState;

                // Here we complete/end the BeginGetContext() asynchronous call
                // by calling EndGetContext() - which returns the reference to
                // a new HttpListenerContext object. Then we can set up a new
                // thread to listen for the next connection.
                HttpListenerContext workerContext = listener.EndGetContext(asyncResult);
                listener.BeginGetContext(OnClientConnect, listener);

                ExtensionRequestPacket packet = new ExtensionRequestPacket(workerContext);

                // Is the request asking for a connection to extension?
                Connecting = workerContext.Request.QueryString["doConnect"] != null;

                // Console.WriteLine("ID {0} connected.", packet.ID);
                if (packet.PacketType == ChromeExtensionPacketType.Get)
                {
                    // Console.WriteLine("Received GET request from from {0}", packet.ID);
                    if (!Connected && Connecting)
                        Send(packet, HostPageHtml, "text/html");
                    else
                        Send(packet, BlankPageHtml, "text/html");
                }
                else
                {
                    lock (responseLock)
                    {
                        // Console.WriteLine("Received from {0}:\n{1}", packet.ID, packet.Content);
                        // pendingRequestQueue.Enqueue(packet);
                        pendingRequestQueue.Add(packet);
                        postRequestReceived.Set();
                        receivedResponse = true;
                    }
                }

                // Console.WriteLine("ID {0} disconnected.", packet.ID);
            }
            catch (ObjectDisposedException)
            {
                //Utilities.WriteToConsole("ChromeHttpServer was disposed.");
            }
            catch (SocketException e)
            {
                Utilities.WriteToConsole(FormatOnClientConnectException(e));
            }
            catch (HttpListenerException e)
            {
                Utilities.WriteToConsole(FormatOnClientConnectException(e));
            }
        }

        private static string FormatOnClientConnectException(Exception e)
        {
            return string.Format("OnClientConnect threw: {0}", e.Message);
        }

        private void Send(ExtensionRequestPacket packet, string data, string sendAsContentType)
        {
            if (packet.PacketType == ChromeExtensionPacketType.Post)
            {
                // Console.WriteLine("Sending to {0}:\n{1}", packet.ID, data);
                // Reset the signal so that the processor will wait for another
                // POST message.
                lock (responseLock)
                {
                    postRequestReceived.Reset();
                    receivedResponse = false;
                }
            }

            byte[] byteData = Encoding.UTF8.GetBytes(data);
            HttpListenerResponse response = packet.Context.Response;
            response.KeepAlive = true;
            response.StatusCode = 200;
            response.StatusDescription = "OK";
            response.ContentType = sendAsContentType;
            response.ContentLength64 = byteData.LongLength;
            response.Close(byteData, true);
        }

        private static string ParseResponse(string rawResponse)
        {
            return rawResponse.Substring(0, rawResponse.IndexOf("\nEOResponse\n"));
        }

        private void StopListening()
        {
            Reflection.ReflectionHelper.InvokeMethod(chromeListener, "RemoveAll", new object[] {false});
            chromeListener.Close();
            pendingRequestQueue.Clear();
            chromeListener = null;
        }

        #endregion

        #region IDisposable Members

        ~ChromeHttpServer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                if (chromeListener != null)
                {
                    StopListening();
                }

                // Note disposing has been done.
                Disposed = true;
            }
        }

        #endregion
    }
}
