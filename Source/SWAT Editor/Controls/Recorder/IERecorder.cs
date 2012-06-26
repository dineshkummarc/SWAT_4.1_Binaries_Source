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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using SWAT_Editor.Controls.Recorder;
using Microsoft.Win32;
using System.Drawing;


namespace SWAT_Editor.Recorder
{
	public partial class IERecorder : Component
	{

		private bool _recording;
		private SWAT_Editor.Recorder.WebBrowserEvents _inetExplorer;
		private bool IsCtrlKeyPressed = false;
		private ContextMenuStrip cmsMenu;
		SWATWikiGenerator _writer;


		public bool Recording
		{
			get { return _recording; }
		}

		public IERecorder()
		{
			InitializeComponent();
		}

		public IERecorder(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}

		public string Stop()
		{
			//foreach (List<HTMLEvents> ev in _eventsList.Values)
			//{
			//    foreach (HTMLEvents evt in ev) evt.DisconnectHtmlEvents();
			//}
			WebBrowserEvents.clearEvents();
			_recording = false;

			// stop to record the whole system's events
			actHook.Stop();

			return _writer.GeneratedCode;
		}

		//Begin recording. Opens a new browser window and connects to browser events.
		public void Record(ContextMenuStrip cmsMenu)
		{
			this.InitializeContextMenu(cmsMenu);
			_writer = new SWATWikiGenerator();
			_writer.Initialize(BrowserType.InternetExplorer, true);
			_recording = true;
			_inetExplorer = new WebBrowserEvents("NewWindow", null, ref _writer);
			RecordJScriptBoxSetup();
		}

		public void Record(ContextMenuStrip cmsMenu, ArrayList browserNames, ArrayList indexes, System.Windows.Forms.CheckedListBox.CheckedIndexCollection checkedIndices)
		{
			this.InitializeContextMenu(cmsMenu);
			_writer = new SWATWikiGenerator();
			_writer.Initialize(BrowserType.InternetExplorer, false);
			_recording = true;

			RecordJScriptBoxSetup();

			foreach (int a in checkedIndices)
			{
				string titleContents = (String)browserNames[a];
				int _index = (int)indexes[a];
				int indexCount = 0;
				ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
				foreach (InternetExplorer Browser in m_IEFoundBrowsers)
				{
                    try
                    {
                        if (Browser.LocationName != "" && Browser.Document is mshtml.HTMLDocument)
                        {
                            if ((((mshtml.HTMLDocument)Browser.Document).title).Equals(titleContents))
                            {
                                if (indexCount == _index)
                                {
                                    _inetExplorer = new WebBrowserEvents("AttachToWindow", Browser, ref _writer);
                                    break;
                                }
                                else
                                    indexCount++;
                            }
                        }
                    }
                    catch (System.Runtime.InteropServices.COMException) { continue; }
				}
			}
		}

		public string Pause()
		{
			string result = _writer.GeneratedCode;
			_writer.Paused = true;
			_writer.clearGeneratedCode();
			actHook.Stop();
			return result;
		}

		public void Resume()
		{

			_writer.Paused = false;
			actHook.Start();
		}

		public Boolean IsPaused()
		{
			return _writer.Paused;
		}

		#region UserActivityHook
		[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		static extern System.IntPtr FindWindowByCaption(int ZeroOnly, string lpWindowName);

		[DllImport("user32.dll")]
		static extern Boolean SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

		[DllImport("user32.dll")]
		static extern IntPtr WindowFromPoint(Point point);

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		static extern IntPtr GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		//Thread that detects JS message boxes and Modal Windows
		private Boolean JScriptWindowFound = false;
		IntPtr dialogHwnd = IntPtr.Zero;
		IntPtr dialogHWnd_buttonOK = IntPtr.Zero;
		IntPtr dialogHWnd_buttonCancel = IntPtr.Zero;

		IntPtr modalWindowHwnd = IntPtr.Zero;
		const int nChars = 256;
		StringBuilder Buff = new StringBuilder(nChars);
		Boolean modalWindowFound = false;

		public void JScriptPump()
		{
			while (_recording)
			{
				// polling time checking for JS box in ms
				Thread.Sleep(230);
				//Console.WriteLine("Thread Pump running...");

				// JScriptWindow Polling
				if (!JScriptWindowFound)
				{
					dialogHwnd = FindWindowByCaption(32770, "Microsoft Internet Explorer");
					if (dialogHwnd == IntPtr.Zero) {
						dialogHwnd = FindWindowByCaption(32770, "Windows Internet Explorer");
					} 

					// case the JavaScript window has been found. We need to attach a listener to it
					if (dialogHwnd != IntPtr.Zero)
					{
						Console.WriteLine("Window Found...");
						JScriptWindowFound = true;
						dialogHWnd_buttonOK = FindWindowEx(dialogHwnd, IntPtr.Zero, "Button", "OK");
						dialogHWnd_buttonCancel = FindWindowEx(dialogHwnd, IntPtr.Zero, "Button", "Cancel");
					}
				}

				// WARNING: TODO: create its own thread pump:Modal Window Polling
				modalWindowHwnd = GetForegroundWindow();
				if ((int)GetWindowText(modalWindowHwnd, Buff, nChars) > 0)
				{
					if (Buff.ToString().Contains("-- Webpage Dialog") && (!modalWindowFound))
					{
						Console.WriteLine("Modal Window Found: " + Buff.ToString());
						_writer.AttachBrowser(Buff.ToString(), 0);
						modalWindowFound = true;

						//_browser.AttachToWindow(Buff.ToString(), 0);
						//_inetExplorer = new WebBrowserEvents("PopUp", null, ref _writer);


						ShellWindows m_IEFoundBrowsers = new ShellWindowsClass();
						foreach (IWebBrowser2 Browser in m_IEFoundBrowsers)
						{

							if (Browser.Document is mshtml.HTMLDocument)
							{
								Console.WriteLine("found IE window: " + ((mshtml.HTMLDocument)Browser.Document).uniqueID + ", " + ((mshtml.HTMLDocument)Browser.Document).referrer + ", " + ((mshtml.HTMLDocument)Browser.Document).protocol);
								//if ((((mshtml.HTMLDocument)Browser.Document).title).Contains("-- Webpage Dialog"))
								if ((((mshtml.HTMLDocument)Browser.Document).title).Contains(""))
								{
									Console.WriteLine("Attempt to attach to modal window");
									_inetExplorer = new WebBrowserEvents("AttachToModalWindow", (InternetExplorer)Browser, ref _writer);
									//Console.WriteLine("doc: " + ((mshtml.HTMLDocument)Browser.Document).body.style.backgroundColor.ToString());
									break;
								}
							}
						}
					}
				}
			}
		}
		UserActivityHook actHook;
		private void RecordJScriptBoxSetup()
		{
			try
			{
				actHook = new UserActivityHook(); // create an instance with global hooks
				// hang on events
				actHook.OnMouseActivity += new MouseEventHandler(MouseMoved);
				actHook.KeyDown += new KeyEventHandler(MyKeyDown);
				actHook.KeyPress += new KeyPressEventHandler(MyKeyPress);
				actHook.KeyUp += new KeyEventHandler(MyKeyUp);
				actHook.Start();
			}
			catch (Exception)
			{
				Console.WriteLine("UNCHECK: Project -> Project Properties... -> Debug -> Enable the Visual Studio hosting process.");
			}

			Thread pump = new Thread(new ThreadStart(this.JScriptPump));
			pump.IsBackground = true;
			pump.Start();
		}

		public void MouseMoved(object sender, MouseEventArgs e)
		{
			if (e.Clicks > 0)
			{
				if (JScriptWindowFound)
				{
					if (e.Button == MouseButtons.Left)
					{
						string tmpString = String.Format("Left click: x={0}  y={1} wheel={2}", e.X, e.Y, e.Delta);
						Console.WriteLine(tmpString);
						IntPtr btnHandle = WindowFromPoint(new Point(e.X, e.Y));
						if ((btnHandle == dialogHWnd_buttonOK))
						{
							_writer.ClickJSDialog(SWAT.JScriptDialogButtonType.Ok);
							JScriptWindowFound = false;
						}
						if ((btnHandle == dialogHWnd_buttonCancel))
						{
							_writer.ClickJSDialog(SWAT.JScriptDialogButtonType.Cancel);
							JScriptWindowFound = false;
						}
					}
				}
				//On right clicks while the Ctrl key is pressed, we want to show the Recorder menu items.
				if ((e.Button == MouseButtons.Right) && (this.IsCtrlKeyPressed))
				{
					this.cmsMenu.Show(e.X, e.Y);
					this.cmsMenu.BringToFront();
				}
			}
		}

		public void MyKeyDown(object sender, KeyEventArgs e)
		{
			//LogWrite("KeyDown 	- " + e.KeyData.ToString());
			if (SWAT_Editor.Properties.Settings.Default.showIEAppMenu)
			{
				if (e.KeyCode == Keys.RControlKey || e.KeyCode == Keys.LControlKey) {
					this.IsCtrlKeyPressed = true;
				}
			}
		}

		public void MyKeyPress(object sender, KeyPressEventArgs e)
		{
			//LogWrite("KeyPress 	- " + e.KeyChar);
		}

		public void MyKeyUp(object sender, KeyEventArgs e)
		{
			//LogWrite("KeyUp 		- " + e.KeyData.ToString());
			if (e.KeyCode == Keys.RControlKey || e.KeyCode == Keys.LControlKey)
			{
				this.IsCtrlKeyPressed = false;
			}
		}

		private void InitializeContextMenu(ContextMenuStrip cmsMenu)
		{
			//Make sure we get the context menu that will be accessible from the IE window.
			this.cmsMenu = cmsMenu;
			this.cmsMenu.AutoClose = true;
		}

		#endregion

		#region modal dialogs UNDER CONSTRUCTION
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(
				IntPtr hWnd,      // handle to destination window
				uint Msg,       // message
				long wParam,  // first message parameter
				long lParam   // second message parameter
				);
		internal const int WM_CLOSE = 0x0010;
		public void FindModalDialog()
		{
			//_inetExplorer.
			//IntPtr dialogHwnd = FindWindowByCaption("Internet Explorer_TridentDlgFrame", " -- Webpage Dialog");
			IntPtr dialogHwnd = FindWindowByCaption(0, " -- Webpage Dialog");

			//SendMessage(dialogHwnd, WM_CLOSE, 0, 0);
			//_inetExplorer = new WebBrowserEvents("AttachToWindow", Browser, ref _writer);

			//SWAT.WebBrowser _browser = new SWAT.WebBrowser(SWAT.BrowserType.InternetExplorer);
			//_browser.AttachToWindow("-- Webpage Dialog", 0);
			//_browser.StimulateElement(SWAT.IdentifierType.Id, "button001", "onclick");
		}
		#endregion
	}
}
