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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SWAT_Editor.Controls.TextEditor
{    
	public partial class DocumentTextBox : RichTextBox
	{        
		private const int SB_LINEUP = 1;
		private const int SB_LINEDOWN = 0;
		private const int SB_VERT = 1;
		private const int WM_MOUSEWHEEL = 0x020A;
		private const int WM_PAINT = 0xF;
		private const int WM_PASTE = 0x302;
		private const int WM_VSCROLL = 0x115;

        public DocumentTextBox()
            : base()
        { AllowDrop = true; }


		protected override bool ProcessCmdKey(ref Message m, Keys keydata)
		{
			if (keydata == (Keys.Control | Keys.V) || keydata == (Keys.Shift | Keys.Insert))
			{
				Paste();
				return true;
			}
			else
				return base.ProcessCmdKey(ref m, keydata);
		}

		//Intercept WM_MOUSEWHEEL event messages and replace them with
		//VSCROLL messages to disable smooth mousewheel scrolling. It sends
		//the message 3 times because most scroll wheel events cause 3 line changes.
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_MOUSEWHEEL)
			{
				if (m.WParam.ToInt32() < 0)
					for (int x = 0; x < 2; x++)
						SendMessage(this.Handle, WM_VSCROLL, SB_LINEUP, 0);
				else
					for (int x = 0; x < 3; x++)
						SendMessage(this.Handle, WM_VSCROLL, SB_LINEDOWN, 0);
			}
			else
				base.WndProc(ref m);
		}

		public new void Paste()
		{
			try
			{
				IDataObject obj = Clipboard.GetDataObject();
				if (obj.GetFormats().ToString().Contains("String"))
				{
					String text = Clipboard.GetData(DataFormats.Text).ToString();
					InsertAtCaret(text);
				}
			}
			catch
			{ }
		}

		public void InsertAtCaret(String value)
		{
			this.SelectedText = value;
		}

		//Make sure on new lines we start fresh with the standard color settings. We do not want breakpoint unless users set them up themselves.
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyCode == Keys.Enter)
			{
				this.SelectionColor = Color.Black;
				this.SelectionBackColor = Color.White;
			}
		}        

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);
	}
}
