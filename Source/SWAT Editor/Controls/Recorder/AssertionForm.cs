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
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using mshtml;
using SHDocVw;
using SWAT_Editor.Recorder;
using SWAT_Editor.Controls.Recorder.UnsupportedHTMLAttributes;

namespace SWAT_Editor.Recorder
{
    public partial class AssertionForm : Form
    {

        private IHTMLElement _srcElement;
        private string selectedItem;
        private string _assertion;
        private AssertionType _assertionType;

        public AssertionType AssertionType
        {
            get { return _assertionType; }
        }

        public string Assertion
        {
            get { return _assertion; }
            //set { _assertion = value; }
        }



        public AssertionForm(IHTMLElement srcElement)
        {
            _srcElement = srcElement;
            InitializeComponent();
            
            FillListBox(showEmpty.Checked);
            FillTagBox();
        }
        /**
         * Fills the list box. If nullOrEmpty is true attributes with null or empty
         * values will be added to the box.
         */
        private void FillListBox(Boolean nullOrEmpty)
        {
            //Regex q = new Regex("<" + _srcElement.tagName + "[\\d\\D]*?>");
            //Match m = q.Match(_srcElement.outerHTML);
            //// Create a regex object.
            //Regex r = new Regex("\\s[A-Za-z0-9]*=");
            //// Get all the matches.
            //MatchCollection mc = r.Matches(m.Value);
            //foreach (Match a in mc)
            //{
            //    string param = a.Value.Substring(1, a.Value.Length - 2);
            //    if (param.Equals("style")) continue;
            //    paramListBox.Items.Add(param);
            //}
            paramListBox.Items.Clear();
            HTMLDOMTextNode test = (HTMLDOMTextNode)_srcElement;

            if (_srcElement is HTMLSelectElement && _srcElement.innerHTML != null)
            {
                Regex exp = new Regex("<OPTION[\\d\\D]*?</OPTION>",RegexOptions.IgnoreCase);
                MatchCollection matches = exp.Matches(_srcElement.innerHTML);
                HTMLElementCollection options = (HTMLElementCollection)((HTMLSelectElement)_srcElement).options;
                int i = 0;
                foreach (HTMLOptionElement o in options)
                {
                    paramListBox.Items.Add(new OptionElement(o, matches[i].ToString() ));
                    i++;
                }
            }

            // Gets unsupported attributes from app.config
            UnsupportedHTMLAttributesSection section = SWAT_Editor.Controls.Recorder.UnsupportedHTMLAttributes.ConfigurationSections.GetUnsupportedAttributes();
            UnsupportedHTMLAttributesCollection unsupportedAttrColl = section.Attributes;

            IHTMLAttributeCollection testAtt = (IHTMLAttributeCollection)test.attributes;
            foreach (IHTMLDOMAttribute a in testAtt)
            {
                try
                {
                    if (!unsupportedAttrColl.Contains(a.nodeName)) // Added to fix unsupported HTML element attributes in Assertion window
                    {
                        if (nullOrEmpty)
                        {
                            paramListBox.Items.Add(a.nodeName);
                        }
                        else
                        {
                            if (a.nodeValue == null) continue;
                            if (!String.IsNullOrEmpty(a.nodeValue.ToString())) paramListBox.Items.Add(a.nodeName);
                        }
                    }
                }
                catch (NullReferenceException) { System.Diagnostics.Debug.WriteLine("null reference"); }
            }

            paramListBox.Items.Add("innerHTML");
            paramListBox.Refresh();
        }

        public void FillTagBox(){
            String tagString = _srcElement.tagName;
            if (tagString == null)
            {
                tagTextLabel.Text = " ";
                return;
            }
            tagTextLabel.Text = tagString;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _assertion = assertionBox.Text;
            _assertionType = assertExistButton.Checked ? AssertionType.ElementExists : AssertionType.ElementDoesNotExist;         
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {


        }

        private void paramListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(attribValueBox.Text))
                addAssertBut.Enabled = false;
            else
                addAssertBut.Enabled = true;

            selectedItem = ((ListBox)sender).Text;


            if (selectedItem.Equals("innerHTML"))
                attribValueBox.Text = SWAT_Editor.Recorder.HtmlElement.StripChars(_srcElement.innerHTML);
            else
                try
                {
                    if (((ListBox)sender).SelectedItem is OptionElement)
                    {
                        OptionElement element = (OptionElement)((ListBox)sender).SelectedItem;
                        selectedItem = "innerHTML";
                        attribValueBox.Text = element.getAssertionString();
                    }
                    else
                        attribValueBox.Text = SWAT_Editor.Recorder.HtmlElement.StripChars(((IHTMLElement4)_srcElement).getAttributeNode(selectedItem).nodeValue.ToString());
                }
                catch (NullReferenceException)
                {
                    attribValueBox.Text = "";
                }
        }

        private void addAssertBut_Click(object sender, EventArgs e)
        {
            String assertBoxText = assertionBox.Text;
            if (String.IsNullOrEmpty(assertBoxText))
                assertionBox.Text = selectedItem + ":" + attribValueBox.Text;
            else
                assertionBox.Text = assertBoxText + ";" + selectedItem + ":" + attribValueBox.Text;
        }

        private void attribValueBox_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(attribValueBox.Text))
                addAssertBut.Enabled = false;
            else
                addAssertBut.Enabled = true;
        }

        private void assertionBox_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(assertionBox.Text))
                okButton.Enabled = false;
            else
                okButton.Enabled = true;
        }

        private void assertModeBut_Click(object sender, EventArgs e)
        {
            HTMLEvents.AssertElement = false;
        }

        private void showEmpty_CheckedChanged(object sender, EventArgs e)
        {
            FillListBox(showEmpty.Checked);
            FillTagBox();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private class OptionElement
        {
            private HTMLOptionElement _element;
            private string assertionString;

            public OptionElement(HTMLOptionElement element, string assertion)
            {
                _element = element;
                assertionString = SWAT_Editor.Recorder.HtmlElement.StripChars(assertion);
            }

            public string getAssertionString()
            {
                return assertionString;
            }

            public override string ToString()
            {
                return "Option: " + (!String.IsNullOrEmpty(_element.innerHTML) ? _element.innerHTML : _element.value);
            }
        }

        private void AssertionForm_Load(object sender, EventArgs e)
        {
            this.Activate();
        }

    }
}
