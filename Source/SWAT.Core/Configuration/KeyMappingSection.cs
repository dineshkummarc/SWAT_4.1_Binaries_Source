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

namespace SWAT.Configuration
{

    public class KeyMappingSection : ConfigurationSection
    {
        public KeyMappingSection()
        {
            base["KeyboardCodes"] = new KeyCodeCollection();
        }

        [ConfigurationProperty("KeyboardCodes", IsDefaultCollection = true)]
        public KeyCodeCollection KeyCodes
        {
            get { return (KeyCodeCollection)base["KeyboardCodes"]; }
        }
    }

    [ConfigurationCollection(typeof(KeyCode), AddItemName = "KeyboardCode")]
    public class KeyCodeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyboardCode();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as KeyboardCode).codeValue;
        }
    }

    public class KeyboardCode : ConfigurationElement
    {
        [ConfigurationProperty("codeName", IsRequired = true)]
        public string codeName
        {
            get { return (string)base["codeName"]; }
            set { base["codeName"] = value; }
        }

        [ConfigurationProperty("codeValue", IsRequired = true)]
        public uint codeValue
        {
            get { return (uint)base["codeValue"]; }
            set { base["codeValue"] = value; }
        }
    }
}
