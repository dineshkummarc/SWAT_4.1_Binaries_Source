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
using System.Drawing;

namespace SWAT_Editor.Controls.TextEditor
{
    public partial class CompletionListBox : ListBox
    {
        private ImageList images = new ImageList();

        public CompletionListBox() : base()
        {
            base.DrawMode = DrawMode.OwnerDrawVariable;
            base.DrawItem += new DrawItemEventHandler(myListBox_DrawItem);
            base.MeasureItem += new MeasureItemEventHandler(CompletionListBox_MeasureItem);
        }

        private void myListBox_DrawItem(Object sender, DrawItemEventArgs e)
        {
            if (base.Items.Count > 0)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                e.Graphics.DrawString(base.Items[e.Index].ToString(), base.Font, Brushes.Black, e.Bounds);
                
            }
        }

        private void CompletionListBox_MeasureItem(Object sender, MeasureItemEventArgs e)
        {
            // = ((TextEditor)this.Parent).Font.Height;
        }

        public void showListAtCharPosition(Point p)
        {
            this.Location = p;
            this.Visible = true;
        }
    }
}
