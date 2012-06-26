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
using System.Configuration;

namespace SWAT_Editor.Controls.Recorder.UnsupportedHTMLAttributes
{
    public class UnsupportedHTMLAttributesSection : ConfigurationSection
    {
        public UnsupportedHTMLAttributesSection()
        {
            base["Attributes"] = new UnsupportedHTMLAttributesCollection();
        }

        [ConfigurationProperty("Attributes", IsDefaultCollection = true)]
        public UnsupportedHTMLAttributesCollection Attributes
        {
            get { return (UnsupportedHTMLAttributesCollection)base["Attributes"]; }
        }
    }

    [ConfigurationCollection(typeof(Attribute), AddItemName = "attribute")]
    public class UnsupportedHTMLAttributesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Attribute();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Attribute)(element)).Value;
        }

        public bool Contains(String attributeName)
        {
            foreach (Attribute attribute in this)
            {
                if (attribute.Value == attributeName) return true;
            }

            return false;
        }
    }

    public sealed class Attribute : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }
    }
}
