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
using System.Drawing;

namespace SWAT.Configuration.Normalization
{
    public class NormalizationSection : ConfigurationSection
    {
        public NormalizationSection()
        {
            base["NormalizationAttributes"] = new AttributeCollection();
        }

        [ConfigurationProperty("NormalizationAttributes", IsDefaultCollection = true)]
        public AttributeCollection NormalizationAttributes
        {
            get { return (AttributeCollection)base["NormalizationAttributes"]; }
        }
    }

    [ConfigurationCollection(typeof(NormalizationAttribute), AddItemName = "NormalizationAttribute")]
    public class AttributeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new NormalizationAttribute();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as NormalizationAttribute).Attribute;
        }
    }

    public class NormalizationAttribute : ConfigurationElement
    {
        [ConfigurationProperty("attribute", IsRequired=true, IsKey=true)]
        public string Attribute
        {
            get { return (string)base["attribute"]; }
            set { base["attribute"] = value; }
        }

        [ConfigurationProperty("normalizedAttribute", IsRequired = true, IsKey=false)]
        public string NormalizedAttribute
        {
            get { return (string)base["normalizedAttribute"]; }
            set { base["normalizedAttribute"] = value; }
        }
    }
}
