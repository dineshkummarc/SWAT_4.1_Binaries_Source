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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;
using SWAT_Editor.Configuration.SyntaxHighlighting;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace SWAT_Editor.Controls.TextEditor
{
  public class SyntaxHighlightingRichTextBox : RichTextBox
  {
    bool _ignoreTextChange = false;
    Dictionary<Color, int> _addedColors = new Dictionary<Color, int>();
    bool _blockPaint = false;
    private const int WM_PAINT = 0xF;
    //bool logging = false;
    Panel _buffer = new Panel();

    public SyntaxHighlightingRichTextBox() : base()
    {            
        AllowDrop = true;
    }

    protected override void InitLayout()
    {
      base.InitLayout();

      _buffer.Height = this.Height;
      _buffer.Width = this.Width;
      _buffer.Left = this.Left;
      _buffer.Top = this.Top;
      
      
      Parent.Controls.Add(_buffer);
      _buffer.SendToBack();
    }
    //protected override void WndProc(ref Message m)
    //{
    //  //if(logging)
    //  //  System.Diagnostics.Debug.WriteLine(_blockPaint + " - Message " + m.ToString());



    //  if (_blockPaint && m.Msg == 0x000)
    //  {
    //    //DO NOTHING. We are blocking the paint event when we are selecting text for highlighting
    //    //so that the user does not see a flicker.
    //    //_blockPaint = false;
    //    Application.DoEvents();
    //  }
    //  else
    //  {
    //    //_lastMessage = m;
    //    base.WndProc(ref m);
    //  }
    //}

    protected override void OnPaint(PaintEventArgs e)
    {
      if (!_blockPaint)
      {
        base.OnPaint(e);
      }
      else
        Debug.WriteLine("BLOCKING PAINT");
    }

    

    
    //protected override void OnVScroll(EventArgs e)
    //{
    //  System.Diagnostics.Debug.WriteLine(_blockPaint + " - Message " + _lastMessage.ToString());
    //  base.OnVScroll(e);
    //}

    private Point getActualPoint()
    {
      int x = 0;
      int y = 0;
      Control container = this.Parent;

      x = this.Location.X;
      y = this.Location.Y;

      while (container.Parent != null)
      {
        x = x + container.Location.X;
        y = y + container.Location.Y;
        container = container.Parent;
        
      }


      return new Point(x, y);
    }

      protected override void OnTextChanged(EventArgs e)
      {
        
        
        if (!_ignoreTextChange)
        {
          //Bitmap map = new Bitmap(this.Width, this.Height);
          //Graphics grp = Graphics.FromImage(map);
          Point screenCords = Program.EntryForm.PointToScreen(getActualPoint());
          Point screenCordsOffset = screenCords;
          screenCordsOffset.Offset(this.Width, this.Height);

          ////grp.CopyFromScreen(screenCords, screenCordsOffset, new Size(this.Width, this.Height));
          //grp.CopyFromScreen(0, 0, 500, 500, new Size(500, 500));
          
          //map.Save("C:\\Temp\\gggggg.bmp");

          Bitmap bmp = new Bitmap(this.Width, this.Height);
          Graphics g = Graphics.FromImage(bmp);
          g.CopyFromScreen(screenCords.X, screenCords.Y, 0, 0, this.Size, CopyPixelOperation.SourceCopy);
          //_buffer.CreateGraphics().FillRectangle(Brushes.Blue, 0, 0, this.Width, this.Height);
          //_buffer.BringToFront();
          this.CreateGraphics().Clear(Color.Transparent);
          this.CreateGraphics().DrawImageUnscaled(bmp, 0, 0);

          bmp.Save("c:\\captured.jpg");

          base.OnTextChanged(e);
          
          bool textChanged = false;
          int caretPos = this.SelectionStart;

          SyntaxHighlightingSection configSection = (SyntaxHighlightingSection)System.Configuration.ConfigurationManager.GetSection("SyntaxHighlighting");

          string rtf = this.Rtf;
          cleanColorCodes(ref rtf);

          foreach (Pattern pattern in configSection.Patterns)
          {
            if (highlightString(Color.FromName(pattern.Color), new Regex(pattern.Expression, RegexOptions.Multiline | RegexOptions.IgnoreCase), ref rtf))
              textChanged = true;
          }

          if (textChanged)
          {
            _ignoreTextChange = true;
            _blockPaint = true;
            this.Rtf = rtf;
            _blockPaint = false;
            this.SelectionStart = caretPos;
            _ignoreTextChange = false;
          }

          _buffer.SendToBack();
        }
      }

      private void cleanColorCodes(ref string rtf)
      {
        rtf = Regex.Replace(rtf, "\\\\cf. ?", string.Empty, RegexOptions.None);
      }

      private int getColorPos(string rtf, Color color)
      {
        int count = 0;
        foreach (Match match in Regex.Matches(rtf, "\\\\red.+?\\\\green.+?\\\\blue.+?;"))
        {
          count++;

          if (match.Value.Contains("red" + color.R) &&
             match.Value.Contains("blue" + color.B) &&
             match.Value.Contains("green" + color.G))
          {
            return count;
          }
        }

        return 1;
      }

      private bool highlightString(Color color, Regex pattern, ref string rtf)
      {
        
        bool foundMatch = false;
        //rtf = Rtf;
        int charOffSet = 0;

        foreach (Match match in pattern.Matches(rtf))
        {
          foundMatch = true;
          int colorIndex = -1;
          string colorIndexStatement = string.Empty;


          if (_addedColors.ContainsKey(color))
            colorIndex = getColorPos(rtf, color);
          else
            colorIndex = _addedColors.Count + 1;

          colorIndexStatement = string.Format("\\cf{0} ", colorIndex);

          rtf = rtf.Insert(match.Index + charOffSet, colorIndexStatement);
          rtf = rtf.Insert(match.Index + match.Length + colorIndexStatement.Length + charOffSet, "\\cf0 ");
          charOffSet = charOffSet + colorIndexStatement.Length + 5;
        }

        if (!_addedColors.ContainsKey(color) && foundMatch)
        {

          int colorTabPos = this.Rtf.IndexOf("{\\colortbl");

          if (colorTabPos == -1)
          {
            int firstIndex = rtf.IndexOf("{\\fonttbl");

            int insertPos = rtf.IndexOf(";}}", firstIndex) + 3;
            rtf = rtf.Insert(insertPos, "\r\n{" + string.Format("\\colortbl ;\\red{0}\\green{1}\\blue{2};", color.R, color.G, color.B) + "}");
          }
          else
          {

            int insertPos = rtf.IndexOf(";}", colorTabPos) + 1;
            rtf = rtf.Insert(insertPos, string.Format("\\red{0}\\green{1}\\blue{2};", color.R, color.G, color.B));
          }

          _addedColors.Add(color, _addedColors.Count + 1);
        }

        return foundMatch;
      }
     
  }
}
