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


/*
 * Created by SharpDevelop.
 * User: jared
 * Date: 9/14/2007
 * Time: 11:15 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace SWAT_Editor.Recorder
{
    /// <summary>
    /// Description of HtmlElement.
    /// </summary>
    public class HtmlElement
    {
        private string _tagName;
        private string _id;
        private string _name;
        private string _innerHtml;
        private string _href;
        private string _value;
        private string _selectedIndex;
        

        private string _onclick;

        public string OnClick
        {
            get { return _onclick; }
            set { _onclick = StripChars(value); }
        }

        public string SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = StripChars(value); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = StripChars(value); }
        }


        public string Href
        {
            get { return _href; }
            set { _href = StripChars(value); }
        }

        public string Id
        {
            get { return _id; }
            set { _id = StripChars(value); }
        }

        public string InnerHtml
        {
            get { return _innerHtml; }
            set { _innerHtml = StripChars(value); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = StripChars(value); }
        }

        public string TagName
        {
            get { return _tagName; }
            set { _tagName = StripChars(value); }
        }

        public static string StripChars(string element)
        {
            char[] charlist = { '[' , '\\' , '^' , '$' , '.' , '|' , '?' , '*' , '+' , '(' , ')', ';', '/', '<', '>'};

            if (string.IsNullOrEmpty(element)) return element;
            string temp = element;
            //if (temp.Length > 500) temp = temp.Substring(0, 500);
            for (int i = 0; i < charlist.Length;i++) temp = temp.Replace(charlist[i], '.');
            temp = temp.Replace("\n", @"[\n]+");
            temp = temp.Replace("\r", @"[\r]+");
            return temp;
        }
        public HtmlElement()
        {

        }

        public bool Equals(HtmlElement rhs)
        {
            if (rhs == null) return false;
            if (!string.IsNullOrEmpty(Id)) if (!Id.Equals(rhs.Id)) return false;
            if (!string.IsNullOrEmpty(Name)) if (!Name.Equals(rhs.Name)) return false;
            if (!string.IsNullOrEmpty(OnClick)) if (!OnClick.Equals(rhs.OnClick)) return false;
            if (!string.IsNullOrEmpty(TagName)) if (!TagName.Equals(rhs.TagName)) return false;
            return true;
        }



    }
}
