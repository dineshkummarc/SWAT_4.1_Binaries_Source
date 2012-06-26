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
using SWAT_Editor;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SWAT_Editor.Controls.TextEditor
{
	public partial class TextEditor : UserControl
	{
		#region Class variables

		private int lineCount = 1;
		private Font lineNumberFont;
		private IAutoCompleteProvider completionProvider;
		private bool _textEditorIsChanged;
		private bool _TextEditorTempIsSaved;
		CommandEditorPage editorPage;
                     
		//private int topLine = 1; //UNUSED Variable

		//Defines the different ways in that an item can be selected 
		public enum SelectedModeValues
		{
			ToInsertBreakPoint = 0,
			ToGoToLine = 1
		}

		#endregion


		#region Control Events


		public TextEditor(CommandEditorPage page)
		{
			editorPage = page;
			SetUpComponent();
			DrawLineNumbers();
			//completionProvider = new IntelliSense();
			//SetProvider();
		}

		protected override bool ProcessCmdKey(ref Message m, Keys keydata)
		{
			if (keydata == (Keys.Control | Keys.V) || keydata == (Keys.Shift | Keys.Insert))
			{
				this.Paste();
				return true;
			}
			else
				return base.ProcessCmdKey(ref m, keydata);
		}

		public void Paste()
		{
			this.txtTextArea.Paste();
			_textEditorIsChanged = true;
		}

		public void SetUpComponent()
		{
			InitializeComponent();
			this.pnlLineNums.Paint += new PaintEventHandler(pnlLineNums_Paint);
			CreateLineNumberFont();
		}

		private void TextEditor_Load(Object sender, EventArgs e)
		{
			this.txtTextArea.Focus();
		}

		private void SWATTextEditor_Paint(Object sender, PaintEventArgs e)
		{
			DrawLineNumbers();
		}

		private void SWATTextEditor_Resize(Object sender, EventArgs e)
		{
			this.SuspendLayout();
			pnlResults.Height = this.Height;
			pnlLineNums.Height = this.Height;
			txtTextArea.Height = this.Height;
			txtTextArea.Left = 38 + pnlLineNums.Width;
			txtTextArea.Width = this.Width - txtTextArea.Left;

			if (pnlResults.Visible == false)
			{
				pnlLineNums.Left = 0;
				txtTextArea.Left = pnlLineNums.Width;
				txtTextArea.Width += pnlResults.Width;
			}

			this.ResumeLayout();
		}

		private void txtTextArea_MouseDown(Object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				this.popUpMenu.MenuItems[0].Enabled = editorPage.CanUndo();
				this.popUpMenu.MenuItems[1].Enabled = editorPage.CanRedo();
				this.popUpMenu.Show(this, new System.Drawing.Point(e.X, e.Y));
			}
		}

		private void pnlLineNums_MouseDown(Object sender, MouseEventArgs e)
		{
			int topLine = txtTextArea.GetLineFromCharIndex(txtTextArea.GetCharIndexFromPosition(new Point(0, 0))) - 1;
			if (topLine < 1)
				topLine = 1;

			int linePos = 0;
			int lineNum;
			for (lineNum = topLine; linePos < e.Y && lineNum <= lineCount; lineNum++)
				linePos = txtTextArea.GetPositionFromCharIndex(txtTextArea.GetFirstCharIndexFromLine(lineNum)).Y;

			SelectLine(SelectedModeValues.ToInsertBreakPoint, lineNum - 1);
		}

		private void txtTextArea_FontChanged(Object sender, EventArgs e)
		{
			CreateLineNumberFont();
			DrawLineNumbers();
		}

		private void txtTextArea_VScroll(Object sender, EventArgs e)
		{
			DrawLineNumbers();
		}

		private void txtTextArea_TextChanged(Object sender, EventArgs e)
		{
			int oldLineCount = lineCount;
			lineCount = this.txtTextArea.Lines.Length;

			if (lineCount != oldLineCount)
			{
				DrawLineNumbers();
				if (!_textEditorIsChanged)
					_textEditorIsChanged = true;
			}
		}

		private void txtTextArea_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!_textEditorIsChanged)
				_textEditorIsChanged = true;
		}

		public delegate void EnableButton();

		//TODO: Autocomplete

		void txtTextArea_KeyUp(object sender, KeyEventArgs e)
		{
			if (completionProvider != null)
				completionProvider.ProcessChar((Char)e.KeyValue);
		}

		private void popUpMenuCut_Click(object sender, EventArgs e)
		{
			editorPage.Cut();
		}

		private void popUpMenuCopy_Click(object sender, EventArgs e)
		{
			editorPage.Copy();
		}

		private void popUpMenuPaste_Click(object sender, EventArgs e)
		{
			editorPage.Paste();
		}

		private void popUpMenuComment_Click(object sender, EventArgs e)
		{
			editorPage.CommentLines();
		}

		private void popUpMenuUnComment_Click(object sender, EventArgs e)
		{
			editorPage.UnCommentLines();
		}

		private void popUpMenuUndo_Click(object sender, EventArgs e)
		{
			editorPage.Undo();
		}

		private void popUpMenuRedo_Click(object sender, EventArgs e)
		{
			editorPage.Redo();
		}

        private void txtArea_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            if (e.Data.GetDataPresent(typeof(string)))
            {
                //TODO: if no tabs are open add one
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void txtArea_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Macros m = new Macros((string)e.Data.GetData(typeof(string)));
           

                if (string.IsNullOrEmpty(Document.Text))
                    m.IncludeCommand += System.Environment.NewLine;                
                else
                    m.IncludeCommand = System.Environment.NewLine + m.IncludeCommand + System.Environment.NewLine;
                    

                Document.SelectedText = m.IncludeCommand;

            }
            catch (IllegalMacrosException ex)
            { 
                MessageBox.Show(ex.Message, "Illegal macro directory", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }
                
		#endregion


		#region Helper Methods

        private int GetLineNumberFromPos(int x, int y)
        {
            Point pt = new Point(x, y);
            int charIndex = txtTextArea.GetCharIndexFromPosition(pt);
            return txtTextArea.GetLineFromCharIndex(charIndex);
        }

        private int GetStartIndexOfLine(int lineNumber)
        {
            if (lineNumber > lineCount)
                return txtTextArea.Text.Length;

            int count = 1;
            int index = 0;
            char[] cText = txtTextArea.Text.ToCharArray();
            for (index = 0; count < lineNumber; index++)
                if (cText[index] == '\n')
                    count++;
            return index;
        }

		private void CreateLineNumberFont()
		{
			lineNumberFont = new Font("Courier New", txtTextArea.Font.SizeInPoints);
		}

		private void DrawLineNumbers()
		{
			Application.DoEvents();
			List<BreakPoint> breakPoints = editorPage.BreakPoints;
			Graphics g = pnlLineNums.CreateGraphics();
			SolidBrush redBrush = new SolidBrush(Color.Red);
			Bitmap tempBitMap = new Bitmap(pnlLineNums.Width > 0 ? pnlLineNums.Width : 1, pnlLineNums.Height > 0 ? pnlLineNums.Height : 1);
			Graphics bitmapGraphics = Graphics.FromImage(tempBitMap);

			bitmapGraphics.Clear(Color.White);

			int maxNumberLength = lineCount.ToString().Length;
			int maxNumberWidth = (int)g.MeasureString(lineCount.ToString(), lineNumberFont).Width;

			if (maxNumberWidth > this.pnlLineNums.Width)
			{
				int docLeft = this.txtTextArea.Left;
				this.pnlLineNums.Width = maxNumberWidth + 5;
				this.txtTextArea.Left = this.pnlLineNums.Left + this.pnlLineNums.Width + 3;
				this.txtTextArea.Width -= this.txtTextArea.Left - docLeft;
			}

			if (maxNumberLength < 3) maxNumberLength = 3;

			lineCount = this.txtTextArea.Lines.Length;
			int topLine = txtTextArea.GetLineFromCharIndex(txtTextArea.GetCharIndexFromPosition(new Point(0, 0))) - 1;

			if (topLine < 1)
				topLine = 1;

			int bottomLine = topLine + ((int)(this.Height / this.Font.Size)) + 5;
			if (bottomLine > lineCount)
				bottomLine = lineCount;

			if (bottomLine == 0)
				bottomLine = 1;

			for (int x = topLine; (x <= bottomLine); x++)
			{
				int lineYPos = txtTextArea.GetPositionFromCharIndex(txtTextArea.GetFirstCharIndexFromLine(x - 1)).Y;
				if (lineYPos >= -txtTextArea.Font.Height && lineYPos <= txtTextArea.ClientSize.Height + txtTextArea.Font.Height)
				{
					bitmapGraphics.DrawString(x.ToString().PadLeft(maxNumberLength), lineNumberFont, Brushes.Red, 1, lineYPos);
				}
			}
			g.DrawImageUnscaled(tempBitMap, 0, 0);
			for (int x = topLine; (x <= bottomLine); x++)
			{
				int lineYPos = txtTextArea.GetPositionFromCharIndex(txtTextArea.GetFirstCharIndexFromLine(x - 1)).Y;
				if (lineYPos >= -txtTextArea.Font.Height && lineYPos <= txtTextArea.ClientSize.Height + txtTextArea.Font.Height)
				{
					for (int i = 0; i < breakPoints.Count; i++)
					{
						if (breakPoints[i].BPLineNumber == x)
						{
							g.FillEllipse(redBrush, 0, lineYPos + 2, 10, 10);
						}
					}
				}
			}
		}

		private int CountTotalLines()
		{
			int count = 1;
			char[] cText = txtTextArea.Text.ToCharArray();
			for (int x = 0; x < cText.Length; x++)
				if (cText[x] == '\n')
					count++;
			return count;
		}		

		void pnlLineNums_Paint(object sender, PaintEventArgs e)
		{
			DrawLineNumbers();
		}

       
		#endregion

      
        #region BreakPoint Methods

        public List<BreakPoint> Remove(List<BreakPoint> breakPoints, int line)
		{
			for (int i = 0; i < breakPoints.Count; i++)
			{
				if (breakPoints[i].BPLineNumber == line)
					breakPoints.RemoveAt(i);
			}
			return breakPoints;
		}

		public List<BreakPoint> Add(List<BreakPoint> breakPoints, int line)
		{
			for (int i = 0; i < breakPoints.Count; i++)
			{
				if (breakPoints[i].BPLineNumber == line)
					return breakPoints;
			}
			BreakPoint node = new BreakPoint(true, line);
			breakPoints.Add(node);
			return breakPoints;
		}

		public void SelectLine(SelectedModeValues selectedMode, int lineNumber)
		{

			int startIndex = GetStartIndexOfLine(lineNumber);

			if (selectedMode == SelectedModeValues.ToInsertBreakPoint)
			{
				List<BreakPoint> breakPoints = editorPage.BreakPoints;
				txtTextArea.Focus();
				if (lineNumber == lineCount)
				{
					txtTextArea.Select(startIndex, (txtTextArea.Text.Length) - startIndex);
					if (txtTextArea.SelectionBackColor.Equals(Color.Red))
					{
						breakPoints = Remove(breakPoints, lineNumber);
						txtTextArea.SelectionColor = Color.Black;
						txtTextArea.SelectionBackColor = Color.White;
					}
					else
					{
						breakPoints = Add(breakPoints, lineNumber);
						txtTextArea.SelectionColor = Color.White;
						txtTextArea.SelectionBackColor = Color.Red;
					}
					txtTextArea.DeselectAll();
				}
				else
				{
					int stopIndex = GetStartIndexOfLine(lineNumber + 1) - 1;
					txtTextArea.Select(startIndex, stopIndex - startIndex);
					if (txtTextArea.SelectionBackColor.Equals(Color.Red))
					{
						breakPoints = Remove(breakPoints, lineNumber);
						txtTextArea.SelectionColor = Color.Black;
						txtTextArea.SelectionBackColor = Color.White;
					}
					else
					{
						breakPoints = Add(breakPoints, lineNumber);
						txtTextArea.SelectionColor = Color.White;
						txtTextArea.SelectionBackColor = Color.Red;
					}
					txtTextArea.DeselectAll();
				}
				editorPage.BreakPoints = breakPoints;
				DrawLineNumbers();

			}
			else if (selectedMode == SelectedModeValues.ToGoToLine)
			{
				int stopIndex = GetStartIndexOfLine(lineNumber + 1) - 1;
				txtTextArea.Select(startIndex, stopIndex - startIndex);
			}
		}

		public void EnableSelectedLine(int lineNumber)
		{
			if (this.txtTextArea.InvokeRequired)
			{
				this.txtTextArea.BeginInvoke(new MethodInvoker(delegate() { EnableSelectedLine(lineNumber); }));
			}
			else
			{
				int startIndex = GetStartIndexOfLine(lineNumber);
				txtTextArea.Focus();
				if (lineNumber == lineCount)
				{
					txtTextArea.Select(startIndex, (txtTextArea.Text.Length) - startIndex);
					txtTextArea.SelectionColor = Color.Black;
					txtTextArea.SelectionBackColor = Color.Yellow;
					txtTextArea.DeselectAll();
				}
				else
				{
					int stopIndex = GetStartIndexOfLine(lineNumber + 1) - 1;
					txtTextArea.Select(startIndex, stopIndex - startIndex);
					txtTextArea.SelectionColor = Color.Black;
					txtTextArea.SelectionBackColor = Color.Yellow;
					txtTextArea.DeselectAll();
				}
			}
		}

		public void DisableSelectedLine(int lineNumber)
		{
			if (this.txtTextArea.InvokeRequired)
			{
				this.txtTextArea.BeginInvoke(new MethodInvoker(delegate() { DisableSelectedLine(lineNumber); }));
			}
			else
			{
				int startIndex = GetStartIndexOfLine(lineNumber);
				txtTextArea.Focus();
				if (lineNumber == lineCount)
				{
					txtTextArea.Select(startIndex, (txtTextArea.Text.Length) - startIndex);
					txtTextArea.SelectionColor = Color.White;
					txtTextArea.SelectionBackColor = Color.Red;
					txtTextArea.DeselectAll();
				}
				else
				{
					int stopIndex = GetStartIndexOfLine(lineNumber + 1) - 1;
					txtTextArea.Select(startIndex, stopIndex - startIndex);
					txtTextArea.SelectionColor = Color.White;
					txtTextArea.SelectionBackColor = Color.Red;
					txtTextArea.DeselectAll();
				}
			}
		}

		public void DisableSelectedLine(int lineNumber, string breakPointType)
		{
			if (this.txtTextArea.InvokeRequired)
			{
				this.txtTextArea.BeginInvoke(new MethodInvoker(delegate() { DisableSelectedLine(lineNumber, breakPointType); }));
			}
			else
			{
				int startIndex = GetStartIndexOfLine(lineNumber);
				txtTextArea.Focus();
				if (lineNumber == lineCount)
				{
					txtTextArea.Select(startIndex, (txtTextArea.Text.Length) - startIndex);
					if (breakPointType.Equals("normal"))
						txtTextArea.SelectionBackColor = Color.Red;
					else
						txtTextArea.SelectionBackColor = Color.White;
					txtTextArea.DeselectAll();
				}
				else
				{
					int stopIndex = GetStartIndexOfLine(lineNumber + 1) - 1;
					txtTextArea.Select(startIndex, stopIndex - startIndex);
					if (breakPointType.Equals("normal"))
						txtTextArea.SelectionBackColor = Color.Red;
					else
						txtTextArea.SelectionBackColor = Color.White;
					txtTextArea.DeselectAll();
				}
			}
		}

		public void CleanUp()
		{
			for (int lineNumber = 1; lineNumber < lineCount + 1; lineNumber++)
			{
				int startIndex = GetStartIndexOfLine(lineNumber);
				txtTextArea.Focus();
				if (lineNumber == lineCount)
				{
					txtTextArea.Select(startIndex, (txtTextArea.Text.Length) - startIndex);
					if (txtTextArea.SelectionBackColor != Color.Red)
						txtTextArea.SelectionBackColor = Color.White;
					txtTextArea.DeselectAll();
				}
				else
				{
					int stopIndex = GetStartIndexOfLine(lineNumber + 1) - 1;
					txtTextArea.Select(startIndex, stopIndex - startIndex);
					if (txtTextArea.SelectionBackColor != Color.Red)
						txtTextArea.SelectionBackColor = Color.White;
					txtTextArea.DeselectAll();
				}
			}
		}

		public string GetLineText(int lineNumber)
		{
			if (txtTextArea.Text != "")
			{
				int startIndex = GetStartIndexOfLine(lineNumber);
				if (lineNumber == lineCount)
				{
					return (txtTextArea.Text.Substring(startIndex, (txtTextArea.Text.Length) - startIndex));
				}
				else
				{
					int stopIndex = GetStartIndexOfLine(lineNumber + 1) - 1;
					return (txtTextArea.Text.Substring(startIndex, stopIndex - startIndex));
				}
			}
			else
				return "";
		}

		public int GetLineCount()
		{
			return lineCount;
		}

		#endregion


		#region Properties

		//public IAutoCompleteProvider AutoCompleteProvider
		//{
		//    get { return completionProvider; }
		//    set { SetProvider(value); }
		//}

		public bool TextEditorTempIsSaved
		{
			get { return _TextEditorTempIsSaved; }
			set { _TextEditorTempIsSaved = value; }
		}

		public bool TextEditorIsChanged
		{
			get { return _textEditorIsChanged; }
			set { _textEditorIsChanged = value; }
		}

		public int DocumentOffset
		{
			get { return txtTextArea.Left; }
		}

		public DocumentTextBox Document
		{
			get { return txtTextArea; }
		}

		public bool DisplayBreakpointPanel
		{
			get { return pnlResults.Visible; }
			set
			{
				pnlResults.Visible = value;
				//SWATTextEditor_Resize(this, new EventArgs()); 
			}
		}

		bool _enabled = true;
		new public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				//this.Document.Enabled = _enabled;
				//this.pnlLineNums.Enabled = _enabled;
				//this.Invalidate();
			}
		}

		#endregion

        #region Internal Classes
        protected class Macros
        {
            private string _includeCommand;            

            public Macros(string filePath)
            {
                Regex regex = new Regex(@"FitNesseRoot\\+");
                Regex regex1 = new Regex(@"\\+");
                Regex regex2 = new Regex(@".content.txt$");

                Match match = regex.Match(filePath);

                if (match.Length <= 0)
                    throw new IllegalMacrosException("The specified folder is not a child of the FitNesseRoot folder");
                else
                {
                    _includeCommand = filePath.Substring(match.Index);
                    _includeCommand = regex.Replace(_includeCommand, ".");
                    _includeCommand = regex1.Replace(_includeCommand, ".");
                    _includeCommand = "!include " + _includeCommand;
                    match = regex2.Match(_includeCommand);
                    if(match.Length > 0)
                        _includeCommand = _includeCommand.Substring(0, match.Index);
                }   
            }

            public string IncludeCommand
            {
                get { return _includeCommand; }
                set { _includeCommand = value; }
            }
        }

        public class IllegalMacrosException : Exception
        {
            public IllegalMacrosException(string msg) : base(msg) { }
        }

        #endregion
    }
}
