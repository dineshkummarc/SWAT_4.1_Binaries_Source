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
using System.Windows.Forms;


namespace SWAT_Editor.Controls.TextEditor
{
    partial class TextEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        //private System.ComponentModel.IContainer components = null; //UNUSED Variable

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlResults = new System.Windows.Forms.Panel();
            this.pnlLineNums = new System.Windows.Forms.Panel();
            this.txtTextArea = new SWAT_Editor.Controls.TextEditor.DocumentTextBox();
            this.SuspendLayout();
            this.popUpMenu = new System.Windows.Forms.ContextMenu();
            //
            //  popUpMenu
            //
            this.popUpMenu.MenuItems.Add("Undo", new System.EventHandler(this.popUpMenuUndo_Click));
            this.popUpMenu.MenuItems.Add("Redo", new System.EventHandler(this.popUpMenuRedo_Click));
            this.popUpMenu.MenuItems.Add("-");
            this.popUpMenu.MenuItems.Add("Cut", new System.EventHandler(this.popUpMenuCut_Click));
            this.popUpMenu.MenuItems.Add("Copy", new System.EventHandler(this.popUpMenuCopy_Click));
            this.popUpMenu.MenuItems.Add("Paste", new System.EventHandler(this.popUpMenuPaste_Click));
            this.popUpMenu.MenuItems.Add("-");
            this.popUpMenu.MenuItems.Add("Comment Lines", new System.EventHandler(this.popUpMenuComment_Click));
            this.popUpMenu.MenuItems.Add("Uncomment Lines", new System.EventHandler(this.popUpMenuUnComment_Click));
            // 
            // pnlResults
            // 
            this.pnlResults.BackColor = System.Drawing.Color.White;
            this.pnlResults.Location = new System.Drawing.Point(0, 0);
            this.pnlResults.Name = "pnlResults";
            this.pnlResults.Size = new System.Drawing.Size(30, 228);
            this.pnlResults.TabIndex = 2;
            // 
            // pnlLineNums
            // 
            this.pnlLineNums.BackColor = System.Drawing.Color.White;
            this.pnlLineNums.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlLineNums.Location = new System.Drawing.Point(30, 0);
            this.pnlLineNums.Name = "pnlLineNums";
            this.pnlLineNums.Size = new System.Drawing.Size(30, 228);
            this.pnlLineNums.TabIndex = 3;
            this.pnlLineNums.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlLineNums_MouseDown);
            // 
            // txtTextArea
            // 
            this.txtTextArea.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTextArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTextArea.HideSelection = false;
            this.txtTextArea.Location = new System.Drawing.Point(61, 0);
            this.txtTextArea.Name = "txtTextArea";
            this.txtTextArea.Size = new System.Drawing.Size(307, 231);
            this.txtTextArea.TabIndex = 0;
            this.txtTextArea.Text = "";
            this.txtTextArea.WordWrap = false;
            this.txtTextArea.VScroll += new System.EventHandler(this.txtTextArea_VScroll);
            this.txtTextArea.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtTextArea_KeyUp);
            this.txtTextArea.TextChanged += new System.EventHandler(this.txtTextArea_TextChanged);
            this.txtTextArea.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTextArea_KeyPress);
            this.txtTextArea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtTextArea_MouseDown);
            this.txtTextArea.DragOver += new System.Windows.Forms.DragEventHandler(this.txtArea_DragOver);
            this.txtTextArea.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtArea_DragDrop);
            
            // 
            // TextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.pnlLineNums);
            this.Controls.Add(this.pnlResults);
            this.Controls.Add(this.txtTextArea);
            this.Name = "TextEditor";
            this.Size = new System.Drawing.Size(367, 230);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SWATTextEditor_Paint);
            this.Resize += new System.EventHandler(this.SWATTextEditor_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel pnlResults;
        private Panel pnlLineNums;
        private DocumentTextBox txtTextArea;
        private System.Windows.Forms.ContextMenu popUpMenu;


    }
}
