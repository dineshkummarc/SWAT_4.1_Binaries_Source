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


// Safari.cs created with MonoDevelop
// User: Jared at 11:05 11/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace SWAT
{
    [NCover.CoverageExclude]
    public class Safari : Browser, IBrowser, IDisposable
    {
        #region Private vars
        Socket _socket;
        bool disposed = false;
        int currentlyAttachedNonBrowserWindowId = -1;
        #endregion

        #region Constructor

        public Safari() : base(BrowserType.Safari, BrowserProcess.safari) { }

        #endregion

        #region Help methods

        private static string escapeChars(string ident)
        {
            Regex reg = new Regex("(?<!\\\\)/");
            ident = reg.Replace(ident, "\\/");
            reg = new Regex("(?<!\\\\)'");
            return reg.Replace(ident, "\\'");
        }

        // This method must be called multiple times since the connection can be dropped during execution
        //  at random.
        private void connectToSafariPlugin()
        {
            if (_socket == null || !_socket.Connected)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Net.IPEndPoint localEndPoint = new System.Net.IPEndPoint(System.Net.Dns.GetHostAddresses(SafariSettings.SafariAddress)[0], SafariSettings.SafariPort);
                DateTime timeout = DateTime.Now.AddSeconds(20);

                while (!_socket.Connected && DateTime.Now < timeout)
                {
                    try
                    {
                        _socket.Connect(localEndPoint);
                    }
                    catch
                    {
                        Sleep(0);
                    }
                }

                if (!_socket.Connected)
                    throw new ApplicationException("Unable to establish connection with safari plugin.");
            }
        }

        private void disconnectSafariPlugin()
        {
            _socket.Disconnect(false);
            _socket = null;
        }

        private string sendMessage(Socket client, string msg)
        {

            string result = string.Empty;

            if (client.Connected)
            {
                while (client.Available > 0)
                {
                    getMessage(client);
                }

                DateTime timeout = DateTime.Now.AddSeconds(1);

                while (!client.Poll(5000, SelectMode.SelectWrite) && DateTime.Now < timeout)
                {
                    Sleep(0);
                }

                timeout = DateTime.Now.AddSeconds(5);
                int sent = 0;
                int size = Encoding.GetEncoding(1252).GetBytes(msg).GetLength(0);
                Byte[] data = Encoding.GetEncoding(1252).GetBytes(msg);


                while (sent < size)
                {
                    try
                    {
                        if (DateTime.Now > timeout)
                            return "SOCKET Exception: Send Message Timeout";

                        sent += client.Send(data, sent, size - sent, SocketFlags.None);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.WouldBlock ||
                         ex.SocketErrorCode == SocketError.IOPending ||
                         ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                        {
                            Sleep(30);
                        }
                        else
                        {
                            return string.Format("SOCKET Exception: {0}", ex.Message); ;
                        }
                    }

                    //client.Send(UTF8Encoding.Default.GetBytes(msg + "\n\r"));
                    //client.Send((System.Text.ASCIIEncoding.Default.GetBytes(msg + "\n\r")));
                    //this.writeToConsole(string.Format("Send: {0}", msg));
                }
                //is the socket ready to be read.



                 //waitCount = 0;
                //wait for getMessage to return something other then an empty string if we expect a result.

                timeout = DateTime.Now.AddSeconds(SafariSettings.MacResponseTimeout);
                while(result.Equals("") && DateTime.Now < timeout)
                {
                    result = getMessage(client);
                }

            }
            else
            {
                result = "SOCKET DISCONNECTED.";
            }

            //this.writeToConsole(string.Format("Received: {0}", result));

            if (!result.Equals("SOCKET DISCONNECTED.") && result.StartsWith("a") && result.EndsWith("a"))
            {
                result = result.Substring(1, result.Length - 2);
            }
            
            return result;
        }

        private void writeToConsole(string message)
        {
            //System.Diagnostics.Debug.WriteLine(string.Format("({01}) - {1}", DateTime.Now.ToString(), message));
#if DEBUG
            //Console.WriteLine(string.Format("({01}) - {1}", DateTime.Now.ToString(), message));
#endif
        }
        
        private string getMessage(Socket client)
        {
            byte[] buffer = new byte[5000];

            DateTime timeout = DateTime.Now.AddMilliseconds(100);

            while (!client.Poll(5000, SelectMode.SelectRead) && DateTime.Now < timeout)
            {
                Sleep(0);
            }

            int recieved = 0;
            int size = client.Available;

            while (recieved < size)
            {
                try
                {
                    recieved += client.Receive(buffer, recieved, size - recieved, SocketFlags.None);
                    Sleep(0);
                    size += client.Available;
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                     ex.SocketErrorCode == SocketError.IOPending ||
                     ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        Sleep(30);
                    }
                    else
                    {
                        return string.Format("SOCKET Exception: {0}", ex.Message);
                    }
                }
            }

            return Encoding.GetEncoding(1252).GetString(buffer).Replace("\n>", "").Replace("\0", "").Trim();
        }
/*
        private string getMessage(Socket client)
        {
            byte[] buffer = new byte[5000];

            try
            {
                client.ReceiveTimeout = 2000;
                client.NoDelay = false;
                client.Blocking = true;
                //StringBuilder sb = new StringBuilder();
            }
            catch
            {
                return "exception";
            }

            try
            {
                if (client.Available > 0)
                {
                    try
                    {
                        client.Receive(buffer);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.WouldBlock ||
                         ex.SocketErrorCode == SocketError.IOPending ||
                         ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                        {
                            Sleep(30);
                        }
                        else
                        {
                            return string.Format("SOCKET Exception: {0}", ex.Message);
                        }
                    }
                }
                else
                    return string.Empty;
            }
            catch
            //catch (System.Net.Sockets.SocketException e) //UNUSED Variable
            {
                //do nothing.
                return "exception";
            }

            return System.Text.Encoding.GetEncoding(1252).GetString(buffer).Replace("\n>", "").Replace("\0", "").Trim();
        }
        */
        private string sendJavascriptMessage(Socket client, string javascript)
        {
            return sendMessage(client, string.Format("DoJavascript:~-^{0}^-~", javascript));
        }

        private string sendAppleScriptMessage(Socket client, string appleScript)
        {
            return sendMessage(client, string.Format("DoAppleScript:~-^{0}^-~", appleScript));
        }


        private bool needToWait;
        private void setCurrentURI(bool causesAWait)
        {
            if (needToWait)
                waitForBrowserReady();
            needToWait = causesAWait;
        }

        private bool isBrowserDoneLoadingDocument
        {
            get
            {
                string result = sendMessage(_socket, "pageHasLoaded^-~").Trim();

                return result == "true" || result == "exception" || result.Contains("~-^No Browser Attached^-~");
            }
        }

        //private void waitForBrowserReady(int millisecondTimeout)
        //{
        //    int count = 0;
        //    while (!isBrowserDoneLoadingDocument && count < millisecondTimeout)
        //    {
        //        count++;
        //        System.Threading.Thread.Sleep(1000);
        //    }

        //}

        public void WaitForBrowserReadyState()
        {
            connectToSafariPlugin();
            setCurrentURI(false);
        }

        private void waitForBrowserReady()
        {
            DateTime timeout = DateTime.Now.AddSeconds(SWAT.DefaultTimeouts.WaitForBrowserTimeout);

            while (!isBrowserDoneLoadingDocument && DateTime.Now < timeout)
            {
                System.Threading.Thread.Sleep(100);
            }
        }

        private void buildFlickerElement(StringBuilder msg)
        {
            //flicker element
            msg.Append("var origColor = '';");
            msg.Append("if(elem != null){origColor = elem.style.backgroundColor;if(origColor == null)origColor = '';elem.style.backgroundColor = 'yellow';}");
        }

        private void buildElementFindCommands(IdentifierType identType, string identifier, string elementTagName, StringBuilder msg)
        {
            msg.Append("var elem = null;");
            identifier = escapeChars(identifier);
            string searchStr;
            switch (identType)
            {
                case IdentifierType.Id:
                    buildElementFindCommands(IdentifierType.Expression, "id=" + identifier, elementTagName, msg);
                    break;

                case IdentifierType.Name:
                    buildElementFindCommands(IdentifierType.Expression, "name=" + identifier, elementTagName, msg);
                    break;

                case IdentifierType.InnerHtml:
                    searchStr = string.Format("var elems = document.getElementsByTagName('{0}'); for(var a=0;a < elems.length;a++){{ if(elems[a].innerHTML != null && elems[a].innerHTML.toString().toLowerCase() == '{1}')elem = elems[a];}}", elementTagName, identifier.ToLower());
                    buildElementFindHelper(searchStr, msg);
                    break;

                case IdentifierType.InnerHtmlContains:
                    searchStr = string.Format("var elems = document.getElementsByTagName('{0}'); for(var a=0;a < elems.length;a++){{ if(elems[a].innerHTML != null && elems[a].innerHTML.toString().toLowerCase().indexOf('{1}') > -1)elem = elems[a]; }}", elementTagName, identifier.ToLower());
                    buildElementFindHelper(searchStr, msg);
                    break;

                case IdentifierType.Expression:

                    string regularExpStmt;

                    StringBuilder completeRegularExp = new StringBuilder();
                    if (identifier.ToLower().Contains("style"))
                    {
                        int styleInd = identifier.ToLower().IndexOf("style:");
                        if (styleInd > 0)
                        {
                            identifier = identifier.Remove(styleInd, 6).Insert(styleInd, "style:");
                            IdentifierExpression expPt1 = new IdentifierExpression(identifier.Substring(0, styleInd - (identifier[styleInd - 1] == ';' ? 1 : 0)), new IsMatchHandler(IsMatchMethod));
                            completeRegularExp.Append("(");
                            completeRegularExp.Append((string)expPt1.IsMatch("", BrowserType.Safari));
                            completeRegularExp.Append(")&&");
                        }

                        IdentifierExpression expPt2 = new IdentifierExpression(identifier.Substring(styleInd), new IsMatchHandler(IsMatchMethod));
                        completeRegularExp.Append("(");
                        completeRegularExp.Append((string)expPt2.IsMatch("", BrowserType.Safari));
                        completeRegularExp.Append(")");

                        //If there is a colon in the style string, meaning there are
                        //attribute-value pairs instead of seeking to match the entire style string
                        if (identifier.IndexOf(':', styleInd + 7) != -1)
                        {
                            //6 is the length of "style:"
                            completeRegularExp.Append("||(");
                            completeRegularExp.Append(buildStyleString(identType, identifier.Substring(styleInd + 6), elementTagName));
                            completeRegularExp.Append(")");
                        }

                        regularExpStmt = completeRegularExp.ToString();
                    }
                    else
                    {
                        IdentifierExpression exp = new IdentifierExpression(identifier, new IsMatchHandler(IsMatchMethod));
                        regularExpStmt = (string)exp.IsMatch("", BrowserType.Safari);
                    }

                    searchStr = string.Format("var elems = document.getElementsByTagName('{0}'); for(var a=0;a < elems.length;a++){{ if(elems[a] != null && elems[a].hasAttributes() == true && elems[a].type == 'checkbox'){{ if(elems[a].getAttribute('value') === null){{ elems[a].value = 'on'; }}}} if({1})elem = elems[a]; }}", elementTagName, regularExpStmt);
                    searchStr = searchStr.Replace("[i]", "[a]");
                    searchStr = searchStr.Replace("&& elems[a].innerHTML.toString() == ''", "");
                    buildElementFindHelper(searchStr, msg);

                    break;
            }
        }

        private static void buildElementFindHelper(string searchStr, StringBuilder msg)
        {
            msg.Append(searchStr);
            msg.Append("var found = false;");
            //make sure the window object is defined correctly
            //msg.Append("var window = null; for(var i=0; i < firefoxWindow.frames.length; i++){if(firefoxWindow.frames[i].toString().toLowerCase().indexOf('object window') > -1){window = firefoxWindow.frames[i]; break;}}");
            msg.Append("function recursiveSearch(frames){ for(var i=0; i<frames.length; i++){" + searchStr.Replace("document.", "frames[i].document.") + " if(elem){found = true; return;} else{ if(frames[i].frames.length>0){recursiveSearch(frames[i].frames);}}}}");
            msg.Append("if(!elem && window.frames.length > 0){ recursiveSearch(window.frames); }");
        }

        private static String buildStyleString(IdentifierType identType, string identifier, string elementTagName)
        {
            StringBuilder msg = new StringBuilder();

            identifier = escapeChars(identifier);

            string[] expressions = identifier.Split(';');

            ExpressionToken[] tokens = (from expression in expressions
                                        where !expression.Contains("parentElement")
                                        select new ExpressionToken(expression)).ToArray();

            msg.Append("elems[a]!=null&&elems[a].hasAttributes() == true ");
            const string matcherPart2 = "&& document.defaultView != null && document.defaultView.getComputedStyle(elems[a], null).getPropertyCSSValue('{0}') != null && document.defaultView.getComputedStyle(elems[a], null).getPropertyCSSValue('{0}').cssText != null && document.defaultView.getComputedStyle(elems[a], null).getPropertyCSSValue('{0}').cssText.toLowerCase().match('{1}') != null && document.defaultView.getComputedStyle(elems[a], null).getPropertyCSSValue('{0}').cssText.toLowerCase().match('{1}').length>=0 ";

            foreach (ExpressionToken t in tokens)
                msg.AppendFormat(matcherPart2, t.Attribute.Trim(), t.Value.Trim());

            return msg.ToString();
        }

        private string HexToDecimal(string value)
        {
            while (value.Contains("#"))
            {
                int start = value.IndexOf("#");
                string hexNum = value.Substring(start, 7);
                int red = int.Parse(hexNum.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                int green = int.Parse(hexNum.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                int blue = int.Parse(hexNum.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                string decConverted = string.Format("rgb\\({0}, {1}, {2}\\)", red, green, blue);
                value = value.Replace(hexNum, decConverted);
            }
            return value;
        }

        public void IsMatchMethod(object value, ExpressionToken token, IsMatchResult isMatchResult)
        {
            if (isMatchResult.ReturnValue != null && isMatchResult.ToString() != string.Empty)
                isMatchResult.ReturnValue += " && ";

            //Custom attribute if searching by style
            string tokenAttribute = token.Attribute.Replace("parentElement", "parentNode").Replace("parentWindow", "ownerDocument.defaultView");
            string tokenValue = token.Value;
            if (tokenAttribute.Equals("style", StringComparison.OrdinalIgnoreCase))
            {
                if (tokenValue.Contains("#"))
                    tokenValue = HexToDecimal(tokenValue);

                tokenAttribute += ".cssText";
            }

            string parentString = "";
            while (tokenAttribute.StartsWith("parentNode."))
            {
                tokenAttribute = tokenAttribute.Remove(0, "parentNode.".Length);
                parentString += ".parentNode";
            }

            tokenAttribute = AttributeNormalizer.Normalize(tokenAttribute);

            if (!string.IsNullOrEmpty(tokenValue))
            {
                if (token.MatchType == MatchType.Contains)
                {
                    if (token.ExpectedMatchCount != int.MinValue)
                    {
                        isMatchResult.ReturnValue += string.Format(" ((elems[i]" + parentString + " != null && elems[i]" + parentString + ".hasAttributes() == true && elems[i]" + parentString + ".getAttribute('{0}') != null && elems[i]" + parentString + ".getAttribute('{0}').toString().match(/{1}/gim) != null && elems[i]" + parentString + ".getAttribute('{0}').toString().match(/{1}/gim).length == {2}) || (elems[i]" + parentString + " != null  && elems[i]" + parentString + ".{0} != null && elems[i]" + parentString + ".{0}.toString().match(/{1}/gim) != null && elems[i]" + parentString + ".{0}.toString().match(/{1}/gim).length == {2}))", tokenAttribute, tokenValue, token.ExpectedMatchCount);
                    }
                    else
                    {

                        isMatchResult.ReturnValue += string.Format(" ((elems[i]" + parentString + " != null && elems[i]" + parentString + ".hasAttributes() == true && elems[i]" + parentString + ".getAttribute('{0}') != null && elems[i]" + parentString + ".getAttribute('{0}').toString().match(/{1}/gim) != null && elems[i]" + parentString + ".getAttribute('{0}').toString().match(/{1}/gim).length >= 0) || (elems[i]" + parentString + " != null  && elems[i]" + parentString + ".{0} != null && elems[i]" + parentString + ".{0}.toString().match(/{1}/gim) != null && elems[i]" + parentString + ".{0}.toString().match(/{1}/gim).length >= 0))", tokenAttribute, tokenValue);
                    }
                }
                else
                {
                    isMatchResult.ReturnValue += string.Format(" ((elems[i]" + parentString + " != null && elems[i]" + parentString + ".hasAttributes() == true && elems[i]" + parentString + ".getAttribute('{0}') != null && elems[i]" + parentString + ".getAttribute('{0}').toString().match(/{1}/gim) != null && elems[i]" + parentString + ".getAttribute('{0}').toString().match(/{1}/gim)[0].length == elems[i]" + parentString + ".getAttribute('{0}').toString().length) || (elems[i]" + parentString + " != null  && elems[i]" + parentString + ".{0} != null && elems[i]" + parentString + ".{0}.toString().match(/{1}/gim) != null && elems[i]" + parentString + ".{0}.toString().match(/{1}/gim)[0].length == elems[i]" + parentString + ".{0}.toString().length))", tokenAttribute, tokenValue, token.Value.Length);
                }
            }
            else
            {
                isMatchResult.ReturnValue += string.Format(" ((elems[i]" + parentString + " != null && elems[i]" + parentString + ".hasAttributes() == true && elems[i]" + parentString + ".getAttribute('{0}') != null && elems[i]" + parentString + ".getAttribute('{0}').toString() != null && elems[i]" + parentString + ".getAttribute('{0}').toString() == '') || (elems[i]" + parentString + " != null  && elems[i]" + parentString + ".{0} != null && elems[i]" + parentString + ".{0}.toString() != null && elems[i]" + parentString + ".{0}.toString() == '')) ", tokenAttribute, tokenValue, token.Value.Length);
            }
            isMatchResult.ContinueChecking = true;
        }
        #endregion

        public void OpenBrowser()
        {
            connectToSafariPlugin();
            sendMessage(_socket, "OpenBrowser^-~");
            //disconnectSafariPlugin();
        }

        public void NavigateBrowser(string url)
        {
            connectToSafariPlugin();
            setCurrentURI(true);
            sendMessage(_socket, string.Format("NavigateBrowser:~-^{0}^-~", url));
            //disconnectSafariPlugin();
        }

        public void CloseBrowser()
        {
            connectToSafariPlugin();
            sendMessage(_socket, "CloseBrowser^-~");
            disconnectSafariPlugin();
        }

        internal override IntPtr GetJSDialogHandle(int timeoutSeconds)
        {
            return IntPtr.Zero;
        }

        public override string TakeScreenshot(string filePrefix)
        {
            string result = "";
            if (ScreenShotSettings.SnapShotOption)
            {
                connectToSafariPlugin();
                string windowOrScreen = ScreenShotSettings.ScreenShotAllScreens ? "Screen" : "Window";
                result = sendMessage(_socket, string.Format("TakeScreenshot:withPrefix:~-^{0}~-^{1}^-~", windowOrScreen, filePrefix));
            }
            return result;
        }

        public void ClickJSDialog(JScriptDialogButtonType buttonType)
        {
            connectToSafariPlugin();
            //setCurrentURI();
            string result = string.Empty;
            DateTime timeout = DateTime.Now.AddSeconds(15);

            while (!result.Contains("OK") && DateTime.Now < timeout)
            {
                result = sendMessage(_socket, buttonType == JScriptDialogButtonType.Cancel ? "ClickJSDialogCancel^-~" : "ClickJSDialogOk^-~");
            }
            //disconnectSafariPlugin();
            if (!result.Contains("OK"))
                throw new ClickJSDialogException();
        }

        public void AttachBrowserToWindow(string windowTitle)
        {
            AttachBrowserToWindow(windowTitle, 0);
        }

        public void AttachBrowserToWindow(string windowTitle, int windowIndex)
        {
            connectToSafariPlugin();
            //setCurrentURI(true);
            needToWait = true;
            string result = "";
            DateTime timeout = DateTime.Now.AddSeconds(DefaultTimeouts.AttachToWindowBrowserTimeout);

            while (DateTime.Now < timeout)
            {
                result = sendMessage(_socket, string.Format("AttachToWindow:windowIndex:~-^{0}~-^{1}^-~", windowTitle, windowIndex));

                if (result.Equals("OK"))
                {
                    break;
                }
            }

            //disconnectSafariPlugin();
            if (result.Equals("failed-0", StringComparison.OrdinalIgnoreCase))
                throw new WindowNotFoundException(windowTitle);
            if (result.Contains("failed-"))
            {
                int finalCount = Convert.ToInt32(result.Substring(7));
                string message;
                if (finalCount == 1)
                    message = "There is only one window";
                else
                    message = "There are only " + finalCount + " windows";

                throw new IndexOutOfRangeException(message + " that has " + windowTitle + " in the title.");
            }
        }

        public void AttachToNonBrowserWindow(ApplicationType appType, string title, int index, int timeout)
        {
            connectToSafariPlugin();
            string appName;
            switch (appType)
            {
                default: appName = "Microsoft Excel"; break;
            }
            string message = string.Format("AttachToNonBrowserWindow:~-^{0}^-~", appName);
            bool success = false;
            DateTime end = DateTime.Now.AddSeconds(timeout);
            while (!success && DateTime.Now < end)
            {
                string windowsString = sendMessage(_socket, message).Trim();
                string[] appWindowNames = windowsString.Split(new char[]{' '});
                int found = 0;
                foreach (string windowName in appWindowNames)
                {
                    if (windowName.Contains(title))
                    {
                        if (found == index)
                        {
                            success = true;
                            currentlyAttachedNonBrowserWindowId = appWindowNames.Length - found;
                            break;
                        }
                        found++;
                    }
                }
            }
            if (!success)
            {
                if (index == 0)
                    throw new NonBrowserWindowExistException(title);
                throw new NonBrowserWindowExistException(title, index);
            }
        }

        public void CloseNonBrowserWindow()
        {
            connectToSafariPlugin();
            if (currentlyAttachedNonBrowserWindowId == -1)
                throw new NoAttachedWindowException();
            string message = string.Format("CloseNonBrowserWindow:~-^{0}^-~", currentlyAttachedNonBrowserWindowId);
            string result = sendMessage(_socket, message);
            currentlyAttachedNonBrowserWindowId = -1;
            if (result.Contains("failed")) //edge case, if application has crashed or application busy
                throw new Exception("An error occurred when trying to close the non browser window");
        }

        public override void SetWindowPosition(WindowPositionTypes winPosition)
        {
            connectToSafariPlugin();
            setCurrentURI(false);
            sendMessage(_socket, String.Format("SetWindowPosition:~-^{0}^-~", winPosition.ToString().ToUpper()));
            //disconnectSafariPlugin();
        }

        public void Sleep(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void ElementFireEvent(IdentifierType identType, string identifier, string tagName, string eventName)
        {
            eventName = eventName.ToLower();

            if (eventName.StartsWith("on"))
                eventName = eventName.Remove(0, 2);

            StimulateElement(identType, identifier, tagName, eventName);
        }

        public void StimulateElement(IdentifierType identType, string identifier, string elementTagName, string eventType)
        {
            StimulateElement(identType, identifier, elementTagName, eventType, DefaultTimeouts.FindElementTimeout);
        }

        public void StimulateElement(IdentifierType identType, string identifier, string elementTagName, string eventType, int waitTime)
        {
            connectToSafariPlugin();
            bool foundElement = false;
            setCurrentURI(true);
            DateTime timeout = DateTime.Now.AddSeconds(waitTime);

            while(DateTime.Now < timeout)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("StimulateElement:~-^");
                msg.Append("var elem = null;");

                buildElementFindCommands(identType, identifier, elementTagName, msg);
                //buildFlickerElement(msg);


                if (eventType.Equals("change") || eventType.Equals("blur"))
                    msg.Append("if(elem != null){ var event = document.createEvent(\"HTMLEvents\"); event.initEvent('" + eventType + "',true,true);elem.dispatchEvent(event);}else{'failed';}");
                else if (eventType.ToLower().Equals("keyup") || eventType.ToLower().Equals("keydown") || eventType.ToLower().Equals("keypress"))
                    msg.Append("if(elem != null){ var event = document.createEvent(\"UIEvents\"); event.initUIEvent('" + eventType + "',true, true, null, false, false, false, false, 9, 0);elem.dispatchEvent(event);}else{'failed';}");
                else if (eventType.ToLower().Equals("focus") && !elementTagName.ToLower().Equals("option"))
                    msg.Append("if(elem != null){ elem.focus();}else{ 'failed';}");
                else msg.Append("if(elem != null){ var event = document.createEvent(\"MouseEvents\"); event.initMouseEvent('" + eventType + "',true,true,window,1,0,0,0,0,false,false,false,false,0,null);elem.dispatchEvent(event);}else{'failed';}");

                msg = msg.Replace("\\", "\\\\");
                msg = msg.Replace("\"", "\\\"");
                msg.Append("^-~");

                string result = sendMessage(_socket, msg.ToString()).Trim();

                if (result == "failed" || result.Contains("(null)") || result.Contains("exception"))
                {    
                    Sleep(100);
                }
                else
                {
                    foundElement = true;
                    //TODO:sendJavascriptMessage(_socket, "if(elem != null){elem.style.backgroundColor = origColor;}"); //there is a delay between send messages, so that will make the yellow flicker.
                    break;
                }
            }
            //disconnectSafariPlugin();
            if (!foundElement)
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotFoundException(identifier, identType, elementTagName);
                throw new ElementNotFoundException(identifier, identType);
            }
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, string attributeValue)
        {
            setElementProperty(identType, identifier, tagName, attributeType, attributeName, attributeValue, DefaultTimeouts.FindElementTimeout);
        }

        private void setElementProperty(IdentifierType identType, string identifier, string elementTagName, AttributeType attrType, string propertyName, string propertyValue, int waitTime)
        {

            //propertyValue = propertyValue.Replace("\\", "\\\\"); //should this ONLY BE DONE with file input only?
            connectToSafariPlugin();
            setCurrentURI(false);
            bool foundElement = false;

            StringBuilder msg = new StringBuilder();
            //setDocument();
            buildElementFindCommands(identType, identifier, elementTagName, msg);
            //buildFlickerElement(msg);

            StringBuilder msgBefore = new StringBuilder(msg.ToString());

            string isFileInputResult = isElementFileInput(msgBefore, waitTime);
            if (isFileInputResult.Equals("failed"))
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotFoundException(identifier, identType, elementTagName);
                throw new ElementNotFoundException(identifier, identType);
            }

            if (isFileInputResult.Contains("file"))
            {
                SetFileInputPath(identType, identifier, propertyValue, elementTagName);
                return;
            }

            propertyValue = propertyValue.Replace("\\", "\\\\\\\\");
            propertyValue = propertyValue.Replace("\"", "\\\"");
            //propertyValue = propertyValue.Replace("%", "\\");
            propertyValue = propertyValue.Replace("'", "\\\\'");

            bool hasQuotes = false;
            if (!propertyValue.Equals("true", StringComparison.OrdinalIgnoreCase) && !propertyValue.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                propertyValue = propertyValue.Insert(0, "'") + "'";
                hasQuotes = true;
            }
            else
                propertyValue = propertyValue.ToLower();

            msg = msg.Replace("\\", "\\\\");
            msg = msg.Replace("\"", "\\\"");

            string withoutQuotes = (hasQuotes) ? propertyValue.Substring(1, propertyValue.Length - 2) : propertyValue;
            if (elementTagName.Equals("select") && propertyName.Equals("value"))
                msg.Append("if(elem != null){for(var i = 0; i < elem.options.length; i++){if (elem.options[i].value.match(/" + withoutQuotes + "/i))elem.selectedIndex=i;}}else{ 'failed';}");
            else
                msg.Append("if(elem != null){ elem." + propertyName + " = " + propertyValue.Replace("\r", @"\\r").Replace("\n", @"\\n") + ";}else{ 'failed';}");

            DateTime timeout = DateTime.Now.AddSeconds(waitTime);

            while(DateTime.Now < timeout)
            {
                string result = sendMessage(_socket, string.Format("SetElementAttribute:~-^{0}^-~", msg.ToString()));

                if (result == "failed" || result.Contains("(null)") || result.Contains("exception"))
                {
                    Sleep(0);
                }
                else
                {
                    foundElement = true;
                    //Sleep(200);
                    //TODO:sendJavascriptMessage(_socket, "if(elem != null){elem.style.backgroundColor = origColor;elem = null;}"); //there is a delay between send messages, so that will make the yellow flicker.
                    break;
                }
            }
            //disconnectSafariPlugin();
            if (!foundElement)
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotFoundException(identifier, identType, elementTagName);
                throw new ElementNotFoundException(identifier, identType);
            }
            // setCurrentURI();
        }

        private void SetFileInputPath(IdentifierType identType, string identifier, string filePath, string tagName)
        {
            string finalFilePath;
            if (filePath.Contains(":\\"))
            {
                if (File.Exists(filePath))
                {
                    finalFilePath = string.Format("{0}\\{1}", Environment.MachineName.ToLower(), filePath.Replace(":\\", "$\\"));
                }
                else
                {
                    throw new FileNotFoundException(string.Format("Could not find file {0}", filePath));
                }
            }
            else
            {
                finalFilePath = filePath;
            }

            if (finalFilePath != string.Empty)
            {
                ElementFireEvent(identType, identifier, tagName, "onclick");
                Sleep(2000);
                string result = sendMessage(_socket, string.Format("SetFileInput:~-^{0}^-~", finalFilePath));
                
                if (result.Equals("failed"))
                {
                    throw new FileNotFoundException(string.Format("Could not find file {0}", filePath));
                }
                if (result.Equals("failed timeout"))
                {
                    throw new TimeoutException("Connecting to Network Timed Out When Trying To Access A File");
                }
                Sleep(2000); //Time for filepath to be displayed
            }
        }

        private string isElementFileInput(StringBuilder msg, int waitTime)
        {
            msg.Append("if(elem){if(elem.tagName == 'INPUT'){elem.type}else{elem.tagName}}else{'failed'}");
            string result = string.Empty;
            bool foundElement = false;
            DateTime timeout = DateTime.Now.AddSeconds(waitTime);

            while(DateTime.Now < timeout)
            {
                result = sendJavascriptMessage(_socket, msg.ToString());

                if (result == "failed" || result.Contains("(null)") || result.Contains("exception"))
                {
                    Sleep(0);
                }
                else
                {
                    foundElement = true;
                    break;
                }
            }
            if (!foundElement)
                return "failed";
            return result;
        }

        private string getElementProperty(IdentifierType identType, string identifier, string elementTagName, AttributeType attrType, string propertyName, int waitTime)
        {
            connectToSafariPlugin();
            setCurrentURI(false);
            bool foundElement = false;
            string result = "";

            StringBuilder msg = new StringBuilder();
            //setDocument();
            buildElementFindCommands(identType, identifier, elementTagName, msg);
            //buildFlickerElement(msg);

            if (attrType == AttributeType.Custom)
                msg.Append("if(elem != null){ elem.getAttribute('" + propertyName + "')}else{ 'failed';}");
            else
                msg.Append("if(elem != null){ elem." + propertyName + ";}else{ 'failed';}");

            msg.Replace("\\", "\\\\");
            msg.Replace("\"", "\\\"");

            DateTime timeout = DateTime.Now.AddSeconds(waitTime);

            while(DateTime.Now < timeout)
            {
                result = sendMessage(_socket, string.Format("GetElementAttribute:~-^{0}^-~", msg));

                if (result == "failed" || result.Contains("(null)") || result.Contains("exception"))
                {
                    Sleep(0);
                }
                else
                {
                    foundElement = true;
                    if (result.ToLower().Contains("syntaxerror"))
                        result = "";
                    //Sleep(200);
                    //TODO:  sendMessage(_client, "if(elem != null){elem.style.backgroundColor = origColor;elem = null;}"); //there is a delay between send messages, so that will make the yellow flicker. 
                    break;
                }
            }
            //disconnectSafariPlugin();
            if (!foundElement)
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotFoundException(identifier, identType, elementTagName);
                throw new ElementNotFoundException(identifier, identType);
            }

            return result;
        }

        public void SetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, string attributeValue, int timeOut)
        {
            setElementProperty(identType, identifier, tagName, attributeType, attributeName, attributeValue, timeOut);
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName)
        {
            return GetElementAttribute(identType, identifier, tagName, attributeType, attributeName, DefaultTimeouts.FindElementTimeout);
        }

        public string GetElementAttribute(IdentifierType identType, string identifier, string tagName, AttributeType attributeType, string attributeName, int timeOut)
        {
            return getElementProperty(identType, identifier, tagName, attributeType, attributeName, timeOut);
        }

        public string GetCurrentLocation()
        {
            connectToSafariPlugin();
            setCurrentURI(false);
            string location = sendJavascriptMessage(_socket, "window.location");
            //disconnectSafariPlugin();
            return location;
        }

        public void KillAllOpenBrowsers()
        {
            connectToSafariPlugin();
            sendMessage(_socket, "KillAllOpenBrowsers^-~");
            disconnectSafariPlugin();
            //throw new NotImplementedException();
        }

        public void KillAllOpenBrowsers(string windowTitle)
        {
            connectToSafariPlugin();
            setCurrentURI(false);
            sendMessage(_socket, string.Format("KillAllOpenBrowsers:~-^{0}^-~", windowTitle));
            disconnectSafariPlugin();
            //throw new NotImplementedException();
        }

        protected override string runJavaScript(String theScript)
        {
            connectToSafariPlugin();
            setCurrentURI(true);

            theScript = theScript.Replace("\"", "\\\"");
            string result = string.Empty;
            DateTime timeout = DateTime.Now.AddSeconds(DefaultTimeouts.FindElementTimeout);

            while(DateTime.Now < timeout)
            {
                result = sendJavascriptMessage(_socket, theScript);

                if (String.IsNullOrEmpty(result))
                {
                    Sleep(0);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        protected override string runAppleScript(String theScript)
        {
            connectToSafariPlugin();
            setCurrentURI(true);

            return sendAppleScriptMessage(_socket, theScript);
        }

        public override void AssertBrowserIsAttached()
        {
            connectToSafariPlugin();
            string result = sendMessage(_socket, "isCurrentBrowserAttached^-~");
            disconnectSafariPlugin();

            if (result.ToUpper().Equals("OK"))
                return;
            if (result.ToUpper().Equals("FAILED"))
                throw new NoAttachedWindowException();
            throw new Exception("Safari.isWindowAttached received an invalid result from the SafariPlugin: " + result);
        }

        public void AssertTopWindow(string browserTitle, int index, int timeout)
        {
            connectToSafariPlugin();
            setCurrentURI(false);

            string actualTitle = "";
            int count = 0;

            DateTime end = DateTime.Now.AddSeconds(timeout);
            do
            {
                string msgResult = sendMessage(_socket, String.Format("AssertTopWindow:~-^{0}^-~", index));
                string[] windows = msgResult.Split(new string[] { "~-" }, StringSplitOptions.RemoveEmptyEntries);

                if (windows.Length <= 0 || msgResult.Contains("failed"))
                    continue;
                if (index == 0 && windows[0].ToLower().Contains(browserTitle.ToLower()))
                    return;

                actualTitle = windows[0];

                count = -1;
                for (int i = windows.Length - 1; i >= 0; i--) //Search from the bottom up
                {
                    if (windows[i].ToLower().Contains(browserTitle.ToLower()))
                    {
                        count++;

                        if (count == index)
                        {
                            if (i == 0) //If top-most window
                                return;
                            break;
                        }
                    }
                }
            } while (DateTime.Now < end);


            if (index == 0)
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new TopWindowMismatchException(browserTitle, actualTitle);
                throw new TopWindowMismatchException(browserTitle);
            }
            if (count < index)
                throw new IndexOutOfRangeException("Index: " + index + " is too large, there are only " + (count+1) + " window(s) with title " + browserTitle);
            if (WantInformativeExceptions.GetInformativeExceptions)
                throw new TopWindowMismatchException(browserTitle, index, actualTitle);
            throw new TopWindowMismatchException(browserTitle, index);
        }

        #region IBrowser Members


        public void RefreshBrowser()
        {
            connectToSafariPlugin();
            setCurrentURI(true);
            //writeToConsole("Refresh browser...");
            sendJavascriptMessage(_socket, "window.location.reload();'OK';");
            setCurrentURI(true);
            disconnectSafariPlugin();
        }

        public string GetCurrentDocumentTitle()
        {
            connectToSafariPlugin();
            setCurrentURI(false);

            StringBuilder msg = new StringBuilder();
            msg.Append("var elem=document.title;");
            msg.Append("if(elem != null){elem}");
            string docTitle = sendJavascriptMessage(_socket, msg.ToString());

            if (docTitle.Contains("?"))
            {
                StringBuilder windowText = new StringBuilder(NativeMethods.GetWindowTextLength(curWindowHandle) + 1);
                NativeMethods.GetWindowText(curWindowHandle, windowText, windowText.Capacity);
            }

            return docTitle;
        }

        public string GetCurrentWindowTitle()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AssertBrowserExists(string windowTitle)
        {
            findBrowser(windowTitle, DefaultTimeouts.AssertBrowserExists, true);
        }

        public void AssertBrowserExists(string windowTitle, double timeOut)
        {
            findBrowser(windowTitle, (int)Math.Ceiling(timeOut / 1000), true);
        }

        public void AssertBrowserDoesNotExist(string windowTitle)
        {
            findBrowser(windowTitle, DefaultTimeouts.AssertBrowserExists, false);
        }

        private void findBrowser(string windowTitle, int waitTime, bool expectPositiveResult)
        {
            connectToSafariPlugin();
            //setCurrentURI();
            //windowTitle = escapeChars(windowTitle).ToLower();
            windowTitle = windowTitle.ToLower();

            string message = string.Format("AssertBrowserDoesOrDoesNotExist:~-^{0}^-~", windowTitle);
            string windowExists = string.Empty;

            DateTime timeout = DateTime.Now.AddSeconds(waitTime);

            while (DateTime.Now < timeout)
            {
                windowExists = sendMessage(_socket, message);

                Sleep(0);

                if (expectPositiveResult && windowExists.Contains("windowExists")) //AssertBrowserExists
                {
                    //disconnectSafariPlugin();
                    return;
                }
                if (!expectPositiveResult && !windowExists.Contains("windowExists")) //AssertBrowserDoesNotExist
                {
                    //disconnectSafariPlugin();
                    return;
                }
            }
            //disconnectSafariPlugin();
            if (!expectPositiveResult && windowExists.Contains("windowExists")) //AssertBrowserDoesNotExist
                throw new BrowserExistException(string.Format("There is a browser with title \"{0}\" open.", windowTitle));
            if (expectPositiveResult) //AssertBrowserExists
                throw new BrowserExistException(string.Format("There is no browser with title \"{0}\" open.", windowTitle));
        }

        public void AssertBrowserDoesNotExist(string windowTitle, double timeOut)
        {
            findBrowser(windowTitle, (int)Math.Ceiling(timeOut / 1000), false);
        }

        public void AssertJSDialogContent(string dialogContent)
        {
            AssertJSDialogContent(dialogContent, DefaultTimeouts.FindElementTimeout);
        }

        public void AssertJSDialogContent(string dialogContent, int timeoutSeconds)
        {
            connectToSafariPlugin();
            //setCurrentURI();
            string result;
            DateTime timeout = DateTime.Now.AddSeconds(timeoutSeconds);

            do {
                result = sendMessage(_socket, "AssertJSDialogContent^-~");
            } while (!result.Contains(dialogContent) && DateTime.Now < timeout);

            if (result.Contains("JSDialog Not Found") || (result.Length == 0 && dialogContent.Length > 0))
                throw new DialogNotFoundException();
            if (!result.Contains(dialogContent))
                throw new AssertionFailedException(string.Format("The open javascript dialog content is not equal to \"{0}\".", dialogContent));
        }

        public void AssertElementIsActive(IdentifierType identType, string identifier, string tagName, int timeoutSeconds)
        {
            //function will find the element then traverse through all frames and iframes to see if the activeElement is the element that was found
            connectToSafariPlugin();
            setCurrentURI(false);
            StringBuilder msg = new StringBuilder();

            buildElementFindCommands(identType, identifier, tagName, msg);

            msg.Append("var elementIsActive = false;");
            msg.Append("var resultMessage = '';");

            msg.Append("function checkFrames(doc){ var contDoc = null; var iframes = doc.getElementsByTagName('iframe'); var frames = doc.getElementsByTagName('frame');");
            msg.Append("for(var i = 0; i < iframes.length; i++){ contDoc = iframes[i].contentDocument; checkFrames(contDoc); }");
            msg.Append("for(var i = 0; i < frames.length; i++){ contDoc = frames[i].contentDocument; checkFrames(contDoc); }");
            msg.Append("if(!elementIsActive){ var actElem = doc.activeElement; if(actElem == elem){ elementIsActive = true; } }");
            msg.Append("}");

            msg.Append("if(elem != null){ resultMessage = 'elem found'; ");
            msg.Append("if(document.activeElement == elem){ elementIsActive = true; }");
            msg.Append("if(!elementIsActive){ checkFrames(document); }");
            msg.Append("}");
            msg.Append("resultMessage = resultMessage + elementIsActive;");
            msg.Append("resultMessage;");

            string result;
            DateTime timeout = DateTime.Now.AddSeconds(timeoutSeconds);

            do
            {
                result = sendMessage(_socket, string.Format("AssertElementIsActive:~-^{0}^-~", msg.ToString()));
            } while (!result.Contains("true") && DateTime.Now < timeout);

            if (result.Contains("false") && result.Contains("elem found"))
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotActiveException(identifier, identType, tagName);
                throw new ElementNotActiveException(identifier, identType);
            }
            if (result.Contains("false"))
            {
                if (WantInformativeExceptions.GetInformativeExceptions)
                    throw new ElementNotFoundException(identifier, identType, tagName);
                throw new ElementNotFoundException(identifier, identType);
            }
        }

        public void sendKeyPress(uint keyToPressCode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SendInputString(string windowTitle)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void PressKeys(IdentifierType identType, string identifier, string word, string tagName)
        {
            WaitForBrowserReadyState();

            const string begKeyPattern = "^(\\\\{)";
            const string endKeyPattern = "(\\\\})$";

            Regex begPattern = new Regex(begKeyPattern);
            if (begPattern.IsMatch(word))
            {
                Regex endPattern = new Regex(endKeyPattern);
                if (!endPattern.IsMatch(word))
                    throw new ArgumentException("The key code sequence has not been completed. Please add a \\} at the end of the keyword.");

                string keyValue = word.Substring(2, word.Length - 4);

                if (keyValue.ToUpper().StartsWith("COMMAND") || keyValue.ToUpper().StartsWith("CONTROL") || keyValue.ToUpper().StartsWith("OPTION") || keyValue.ToUpper().StartsWith("ALT"))
                {
                    char[] separator = { '+' };
                    string[] Combination = keyValue.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    Regex alphaNumeric = new Regex("[^a-zA-Z0-9]");

                    if (!((Combination.Length > 1) && (Combination[0].Equals("CONTROL", StringComparison.OrdinalIgnoreCase) || Combination[0].Equals("OPTION", StringComparison.OrdinalIgnoreCase) || Combination[0].Equals("ALT", StringComparison.OrdinalIgnoreCase) || Combination[0].Equals("COMMAND", StringComparison.OrdinalIgnoreCase))
                        && (Combination[1].Length == 1) && (!alphaNumeric.IsMatch(Combination[1]))))
                        throw new ArgumentException("Inproper MODIFIER+CHAR combination. MODIFIER cannot be used alone, and only alphanumeric characters can be appended.");

                    if (Combination[0].ToUpper().Equals("ALT"))
                        Combination[0] = "option";
                    sendMessage(_socket, string.Format("PressKeyCombination:withCharacter:~-^{0}~-^{1}^-~", Combination[0].ToLower(), Combination[1]));
                }
                else if (keyValue.ToUpper().Equals("SHIFT+TAB"))
                {
                    sendMessage(_socket, string.Format("PressModifiedKeyCode:withKeyCode:~-^{0}~-^{1}^-~", "shift", GetMappedKey("TAB")));
                }
                else
                {
                    sendMessage(_socket, string.Format("PressKeyCode:~-^{0}^-~", GetMappedKey(keyValue)));
                }
            }
            else
            {
                word = word.Replace("\\", "\\\\");
                word = word.Replace("\"", "\\\"");
                sendMessage(_socket, string.Format("PressKeys:~-^{0}^-~", word));
            }
        }

        private uint GetMappedKey(string keyValue)
        {
            uint mappedKey = getKeyValue(keyValue.ToUpper());
            if (mappedKey == 0)
                throw new ArgumentException(string.Format("There is no key with name {0} in the configuration table.", keyValue));
            return mappedKey;
        }

        public override IntPtr GetContentHandle()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override void SetDocumentAttribute(string theAttributeName, object theAttributeValue)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override object GetDocumentAttribute(string theAttributeName)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
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
                if (_socket != null)
                    _socket.Close();


                // Note disposing has been done.
                disposed = true;

            }

        }

        #endregion
    }
}
