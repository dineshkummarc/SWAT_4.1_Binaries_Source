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
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SWAT_Editor.Controls
{
    public class TabControlExtension : TabControl
    {
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            RectangleF tabTextArea = RectangleF.Empty;

            for (int nIndex = 0; nIndex < this.TabCount; nIndex++)
            {
                if (nIndex != this.SelectedIndex)
                {
                    /*if not active draw ,inactive close button*/
                    tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                    using (Bitmap bmp = SWAT_Editor.Properties.Resources.Close2_Inactive)
                    {
                        e.Graphics.DrawImage(bmp,
                            tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
                    }
                }
                else
                {
                    tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                    LinearGradientBrush br = new LinearGradientBrush(tabTextArea,
                         SystemColors.ControlLightLight, SystemColors.Control, LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(br, tabTextArea);

                    /*if active draw ,inactive close button*/
                    using (Bitmap bmp = SWAT_Editor.Properties.Resources.Close2_Active)
                    {
                        e.Graphics.DrawImage(bmp,
                            tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
                    }

                    br.Dispose();
                }

                string str = this.TabPages[nIndex].Text;
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                using (SolidBrush brush = new SolidBrush(this.TabPages[nIndex].ForeColor))
                {
                    //*Draw the tab header text
                    e.Graphics.DrawString(str, this.Font, brush,
                    tabTextArea, stringFormat);
                }
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            CommandEditor parent = (CommandEditor) this.Parent;
            RectangleF tabTextArea = (RectangleF)this.GetTabRect(SelectedIndex);
            tabTextArea =
                new RectangleF(tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
            Point pt = new Point(e.X, e.Y);
            if (tabTextArea.Contains(pt))
            {
                parent.closeCurrentDocument(this, new EventArgs());
            }
        }

        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Graphics g = CreateGraphics();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int nIndex = 0; nIndex < this.TabCount; nIndex++)
            {
                  RectangleF tabTextArea = (RectangleF) this.GetTabRect(nIndex);
                  tabTextArea =
                      new RectangleF(tabTextArea.X + tabTextArea.Width - 16, 4, tabTextArea.Height - 5,
                                     tabTextArea.Height - 5);
                
                  Point pt = new Point(e.X, e.Y);

                  if (nIndex == this.SelectedIndex)
                  {
                      if (tabTextArea.Contains(pt))
                      {
                          tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                          using (Bitmap bmp = SWAT_Editor.Properties.Resources.Close2_Active_Selected)
                          {
                              g.DrawImage(bmp,
                                  tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
                          }
                      }
                      else
                      {
                          tabTextArea = (RectangleF)this.GetTabRect(nIndex);
                          using (Bitmap bmp = SWAT_Editor.Properties.Resources.Close2_Active)
                          {
                              g.DrawImage(bmp,
                                  tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13);
                          }
                      }
                  }
                       
             }
        }
            
        

        private Stream GetContentFromResource(string filename)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(
                "MyControlLibrary." + filename);
            return stream;
        }

    }
}
