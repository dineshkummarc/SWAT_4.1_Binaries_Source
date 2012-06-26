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

namespace SWAT.Utilities
{
    public static class ControlsUtils
    {
        /// <summary>
        /// Gets the current line from a TextBox control.
        /// </summary>
        /// <param name="tb">the TextBox control.</param>
        /// <returns>the current line.</returns>
        public static String getCurrentLine(TextBox tb)
        {
            if (tb.Lines.Length == 0)
            {
                return tb.Text;
            }
            else
            {
                int startIndex = tb.GetFirstCharIndexOfCurrentLine();
                int currentLineNumber = tb.GetLineFromCharIndex(startIndex);
                return tb.Lines[currentLineNumber];
            } 
        }
        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <param name="tb">a TextBox.</param>
        /// <returns>the current line number.</returns>
        public static int getCurrentLineNo(TextBox tb)
        {
            if (tb.Lines.Length == 0)
            {
                return 0;
            }
            else
            {
                int startIndex = tb.GetFirstCharIndexOfCurrentLine();
                return tb.GetLineFromCharIndex(startIndex);
            } 
        }
        /// <summary>
        /// Gets a key/value pair containing currentLineNumber/currentLine.
        /// </summary>
        /// <param name="tb">the TextBox.</param>
        /// <returns>a key/value pair containing currentLineNumber/currentLine.</returns>
        public static KeyValuePair<int, String> getCurrentLineAndLineNo(TextBox tb)
        {
            if (tb.Lines.Length == 0)
            {
                return new KeyValuePair<int, String>(0,tb.Text);
            }
            else
            {
                int startIndex = tb.GetFirstCharIndexOfCurrentLine();
                int currentLineNumber = tb.GetLineFromCharIndex(startIndex);
                return new KeyValuePair<int,String>(currentLineNumber, tb.Lines[currentLineNumber]);
            } 
        }
        /// <summary>
        /// Gets the current line from a RichTextBox control.
        /// </summary>
        /// <param name="rtb">the RichTextBox control.</param>
        /// <returns>the current line.</returns>
        public static String getCurrentLine(RichTextBox rtb)
        {
            if (rtb.Lines.Length == 0)
            {
                return rtb.Text;
            }
            else
            {
                int startIndex = rtb.GetFirstCharIndexOfCurrentLine();
                int currentLineNumber = rtb.GetLineFromCharIndex(startIndex);
                return rtb.Lines[currentLineNumber];
            } 
        }
        /// <summary>
        /// Gets a key/value pair containing currentLineNumber/currentLine.
        /// </summary>
        /// <param name="tb">the TextBox.</param>
        /// <returns>a key/value pair containing currentLineNumber/currentLine.</returns>
        public static KeyValuePair<int, String> getCurrentLineAndLineNo(RichTextBox rtb)
        {
            if (rtb.Lines.Length == 0)
            {
                return new KeyValuePair<int, String>(0, rtb.Text);
            }
            else
            {
                int startIndex = rtb.GetFirstCharIndexOfCurrentLine();
                int currentLineNumber = rtb.GetLineFromCharIndex(startIndex);
                return new KeyValuePair<int, String>(currentLineNumber, rtb.Lines[currentLineNumber]);
            }
        }
        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <param name="tb">a RichTextBox.</param>
        /// <returns>the current line number.</returns>
        public static int getCurrentLineNo(RichTextBox rtb)
        {
            if (rtb.Lines.Length == 0)
            {
                return 0;
            }
            else
            {
                int startIndex = rtb.GetFirstCharIndexOfCurrentLine();
                return rtb.GetLineFromCharIndex(startIndex);
            }
        }
    }
}
