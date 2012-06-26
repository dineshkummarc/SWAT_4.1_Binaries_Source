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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using SHDocVw;
using mshtml;


namespace SWAT_Editor.Recorder
{

	public class WebBrowserEvents
	{
		private SHDocVw.InternetExplorer ie;
		//First time connecting to events
		private bool init = true;


		private static List<WebBrowserEvents> _browserList = new List<WebBrowserEvents>();
		//Ok to reset events
		public static bool resetEvents = false;

		private Timer timer;

		private SWATWikiGenerator _writer;
		//list of currently attached events
		List<HTMLEvents> _eventsList = new List<HTMLEvents>();

		private int browserId = 0;

		//public WebBrowserEvents(InternetExplorer _intExp, ref SWATWikiGenerator _write)
		//{
		//    _writer = _write;

		//    Random rand = new Random();
		//    browserId = rand.Next(100000);

		//    if (_intExp == null)
		//    {
		//        manualNavigation = true;
		//        ie = new InternetExplorerClass();
		//        //add a property for identification
		//        ie.PutProperty("ID", browserId);
		//        ie.Visible = true;
		//        object url = "about:blank";
		//        object nullObj = String.Empty;
		//        ConnectWindow(ie);
		//        ie.Navigate2(ref url, ref nullObj, ref nullObj, ref nullObj, ref nullObj);
		//        HTMLEvents.lastBrowser = browserId;
		//    }
		//    else
		//    {
		//        manualNavigation = false;
		//        ie = _intExp;
		//        ie.PutProperty("ID", browserId);
		//        ConnectWindow(ie);
		//    }


		//}


		public WebBrowserEvents(String flag, SHDocVw.InternetExplorer _intExp, ref SWATWikiGenerator _write)
		{
			_writer = _write;
			Random rand = new Random();
			browserId = rand.Next(100000);

			if (flag.Equals("AttachToWindow"))
			{
				init = false;
				manualNavigation = false;
				ie = _intExp;
				ie.PutProperty("ID", browserId);
				ConnectWindow(ie);
				//Debug.WriteLine((((HTMLDocument)ie.Document).frames).length);
				try
				{
					FramesCollection frames = (((HTMLDocument)ie.Document).frames);

					for (int i = 0; i < frames.length; i++)
					{
						object refIndex = i;
						try
						{
							HTMLDocument doc = (HTMLDocument)((HTMLWindow2Class)frames.item(ref refIndex)).document;
							if (doc.Equals((HTMLDocument)ie.Document)) continue;
							HTMLEvents aEvent = new HTMLEvents(_writer, doc, ie);
							_eventsList.Add(aEvent);
						}
						catch (UnauthorizedAccessException) { }
					}
				}
				catch (Exception) { }
				_eventsList.Insert(0, new HTMLEvents(_writer, ie.Document, ie));
				checkAttach();
			}
			else if (flag.Equals("NewWindow"))
			{
				manualNavigation = true;
				ie = new InternetExplorerClass();
				//add a property for identification
				ie.PutProperty("ID", browserId);
				ie.Visible = true;
				object url = "about:blank";
				object nullObj = String.Empty;
				ConnectWindow(ie);
				ie.Navigate2(ref url, ref nullObj, ref nullObj, ref nullObj, ref nullObj);
				HTMLEvents.lastBrowser = browserId;
			}
			else if (flag.Equals("PopUp"))
			{
				manualNavigation = false;
				ie = _intExp;
				ie.PutProperty("ID", browserId);
				ConnectWindow(ie);
			}
			else if (flag.Equals("AttachToModalWindow"))
			{
				IntPtr dialogHwnd = IntPtr.Zero;
				//int indexCount = NativeMethods.GetWindowWithSubstring("content.html", 0, 0, ref dialogHwnd);
				dialogHwnd = NativeMethods.GetForegroundWindow();


				if (dialogHwnd != IntPtr.Zero)
				{
					// This block of code SHOULD take a window handle and return an IHTMLDocument object to dialogDoc
					// Then it will assign that object to _doc. It follows an example in the MSDN library and WatiN uses it as well.
					// If it performs correctly, then we should be attached to the Dialog.
					dialogHwnd = NativeMethods.GetChildWindowHwnd(dialogHwnd, "Internet Explorer_Server");
					//dialogHwnd = NativeMethods.GetForegroundWindow();

					Int32 result = 0;
					Int32 dialog;
					Int32 message;

					message = NativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");
					NativeMethods.SendMessageTimeout(dialogHwnd, message, 0, 0, NativeMethods.SMTO_ABORTIFHUNG, 1000, ref result);

					//NativeMethods.SendMessage(dialogHwnd, NativeMethods.WM_CLOSE, 0, 0);

					IHTMLDocument2 dialogDoc = null;
					System.Guid dialogID = typeof(mshtml.HTMLDocument).GUID;
					dialog = NativeMethods.ObjectFromLresult(result, ref dialogID, 0, ref dialogDoc);

					//HTMLFrameBase dialogDoc = null;
					//System.Guid dialogID = typeof(mshtml.HTMLFrameBase).GUID;
					//dialog = NativeMethods.ObjectFromLresult(result, ref dialogID, 0, ref dialogDoc);

					//dialogDoc.title = "Hello?";

					//IHTMLDialog dialogDoc = null;
					//System.Guid dialogID = typeof(mshtml.HTMLDialog).GUID;
					//dialog = NativeMethods.ObjectFromLresult(result, ref dialogID, 0, ref dialogDoc);




					//IHTMLWindow2 dialogDoc = null;
					//System.Guid dialogID = typeof(IHTMLWindow2).GUID;
					//dialog = NativeMethods.ObjectFromLresult(result, ref dialogID, 0, ref dialogDoc);

					//dialogDoc.close();

					//Console.WriteLine("URL: " + dialogDoc.close + "\nTitle: " + dialogDoc.title);

					//ie = (InternetExplorer)dialogDoc;
					//ie.PutProperty("ID", browserId);
					//ConnectWindow(ie);

					//doc = (HTMLDocument)dialogDoc;



				}
			}
			_browserList.Add(this);

		}
		/**
		 * Event handlers
		 **/
		void ConnectWindow(SHDocVw.InternetExplorer webBrowser)
		{

			resetEvents = false;
			//timer used for resetting events
			timer = new Timer();
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer.Enabled = true;
			timer.AutoReset = false;

			webBrowser.DocumentComplete += new DWebBrowserEvents2_DocumentCompleteEventHandler(ie_DocumentComplete);
			webBrowser.BeforeNavigate2 += new DWebBrowserEvents2_BeforeNavigate2EventHandler(webBrowser_BeforeNavigate2);
			webBrowser.NavigateComplete2 += new DWebBrowserEvents2_NavigateComplete2EventHandler(webBrowser_NavigateComplete2);
			webBrowser.OnQuit += new DWebBrowserEvents2_OnQuitEventHandler(webBrowser_OnQuit);
			webBrowser.NewWindow3 += new DWebBrowserEvents2_NewWindow3EventHandler(webBrowser_NewWindow3);
		}


		private void checkAttach()
		{
			if (HTMLEvents.lastBrowser != browserId)
			{
				if (String.IsNullOrEmpty(ie.LocationName)) return;

				int index = 0;
				string title = ((mshtml.HTMLDocument)ie.Document).title;
				ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
				foreach (InternetExplorer Browser in m_IEFoundBrowsers)
				{
					if (Browser.Document is mshtml.HTMLDocument)
					{
						if (((HTMLDocument)(Browser.Document)).title.Equals(title))
						{
							if ((Browser.GetProperty("ID") != null))
							{
								if (((int)Browser.GetProperty("ID")) == ((int)ie.GetProperty("ID"))) break;
							}
							index++;
						}
					}
				}

				_writer.AttachBrowser(ie.LocationName, index);
				HTMLEvents.lastBrowser = browserId;
			}



		}

		/**
		 * Called when a browser window has been closed
		 **/
		void webBrowser_OnQuit()
		{
			resetEvents = false;
			checkAttach();
			////((InternetExplorer)_browserList[lastbrowser]).Quit();
			//if (Recording)
			//List<HTMLEvents> temp = (List<HTMLEvents>)_eventsList[lastbrowser];
			//foreach (HTMLEvents events in temp) events.DisconnectHtmlEvents();
			//_eventsList.Remove(lastbrowser);
			_writer.CloseBrowser();
			HTMLEvents._previousClickedElement = null;


		}

		/**
		 * Event fired after navigation. Events are reset here to prevent duplicate
		 * events from firing.
		 **/
		void webBrowser_NavigateComplete2(object pDisp, ref object URL)
		{
			//Debug.WriteLine("***NAVIGATE COMPLETE***");

			//Stop timer as page has not finished loading
			timer.Stop();
			int id = (int)(((IWebBrowser2)pDisp).GetProperty("ID"));

			//reset events
			if (_eventsList != null && resetEvents && _eventsList.Count > 0)
			{
				//Debug.WriteLine("Browser " + id + "'s EVENTS HAVE BEEN RESET");
				foreach (HTMLEvents e in _eventsList)
				{
					if (e.Equals(_eventsList[0])) continue;
					e.DisconnectHtmlEvents();
				}
				HTMLEvents doc = _eventsList[0];
				_eventsList.Clear();
				_eventsList.Add(doc);
			}

			resetEvents = false;
		}

		/**
		 * Event fired just before navigation. Navigation is cancelled if AssertionMode is
		 * enabled.
		 **/
		private Boolean manualNavigation = true;
		void webBrowser_BeforeNavigate2(object pDisp, ref object bstrUrl, ref object dwFlags, ref object frameName, ref object postdata, ref object headers, ref bool Cancel)
		{
			if (HTMLEvents.AssertElement)
			{
				Cancel = true;
				return;
			}

			//User navigated browser by entering an address
			if (manualNavigation && (int)dwFlags == 0 && (frameName == null || ((String)frameName).StartsWith("_No__Name:"))
				 && postdata == null && headers == null && !bstrUrl.ToString().StartsWith("about:"))
			{
				checkAttach();
				_writer.NavigateBrowser((String)bstrUrl);
			}
			manualNavigation = false;
			//Debug.WriteLine("**************BeforeNavigate*************");

		}

		/**
		 * Timer has elapsed, ok to reset events
		 **/
		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			HTMLEvents._previousClickedElement = null;
			resetEvents = true;
			manualNavigation = true;
			//Debug.WriteLine(" TIMER EXPIRED");
		}

		/**
		 * New window, add to list's and attach events.
		 **/
		void webBrowser_NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
		{
			//Debug.WriteLine("New Window");

			//Assertion mode enabled, do not open window
			if (HTMLEvents.AssertElement) Cancel = true;

			ppDisp = new InternetExplorerClass();
			new WebBrowserEvents("PopUp", (InternetExplorerClass)ppDisp, ref _writer);

		}

		/**
		 * Document has completed.
		 **/
		private void ie_DocumentComplete(object pDisp, ref object URL)
		{
			if (((String)URL).Equals("about:blank")) manualNavigation = true;
			//Debug.WriteLine("Document Complete");
			if (HTMLEvents.AssertElement) return;

			//Document complete begin reset timer
			timer.Interval = 3000;
			timer.Start();

			Boolean initSet = false;
			//Top level document?
			if (((HTMLDocument)(((IWebBrowser2)pDisp).Document)).Equals((HTMLDocument)ie.Document))
			{
				//return if not first time processing
				if (!init) return;
				init = false;
				initSet = true;
			}
			HTMLEvents currentElement = new HTMLEvents(_writer, ((IWebBrowser2)pDisp).Document, ie);
			//_eventsList.Add(temp, currentElement);  
			//if (!init) _eventsList.Add(currentElement);
			int id = (int)(((IWebBrowser2)pDisp).GetProperty("ID"));
			if (!initSet) _eventsList.Add(currentElement);
			else _eventsList.Insert(0, currentElement);

		}

		public static void clearEvents()
		{
			foreach (WebBrowserEvents a in _browserList)
			{
				foreach (HTMLEvents e in a._eventsList)
				{
					e.DisconnectHtmlEvents();
				}

				try
				{
					a.ie.DocumentComplete -= new DWebBrowserEvents2_DocumentCompleteEventHandler(a.ie_DocumentComplete);
					a.ie.BeforeNavigate2 -= new DWebBrowserEvents2_BeforeNavigate2EventHandler(a.webBrowser_BeforeNavigate2);
					a.ie.NavigateComplete2 -= new DWebBrowserEvents2_NavigateComplete2EventHandler(a.webBrowser_NavigateComplete2);
					a.ie.OnQuit -= new DWebBrowserEvents2_OnQuitEventHandler(a.webBrowser_OnQuit);
					a.ie.NewWindow3 -= new DWebBrowserEvents2_NewWindow3EventHandler(a.webBrowser_NewWindow3);
				}
				catch (Exception) { }
			}
			_browserList.Clear();
		}

		private void tempCheckAttach()
		{

			//_writer.AttachBrowser(ie.LocationName, index);
			//HTMLEvents.lastBrowser = browserId;

			//Assertion mode enabled, do not open window
			//if (HTMLEvents.AssertElement) Cancel = true;

			//ppDisp = new InternetExplorerClass();
			//new WebBrowserEvents("PopUp", (InternetExplorerClass)ppDisp, ref _writer);


			//SWAT.WebBrowser _browser = new SWAT.WebBrowser(SWAT.BrowserType.InternetExplorer);
			//_browser.AttachToWindow("-- Webpage Dialog", 0);
			//_browser.StimulateElement(SWAT.IdentifierType.Id, "button001", "onclick");

			_writer.AttachBrowser("-- Webpage Dialog", 0);



		}
	}
}
