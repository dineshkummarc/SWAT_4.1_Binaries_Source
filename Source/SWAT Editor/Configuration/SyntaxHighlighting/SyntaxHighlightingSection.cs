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


namespace SWAT_Editor.Configuration.SyntaxHighlighting
{
  public class SyntaxHighlightingSection : ConfigurationSection
  {
    public SyntaxHighlightingSection()
    {
      base["patterns"] = new PatternCollection();
    }

    /// <summary>
    /// This property represents the <code>protocols</code> subsection 
    /// in the configuration file.
    /// </summary>
    [ConfigurationProperty("patterns", IsDefaultCollection = true)]
    public PatternCollection Patterns
    {
      get { return (PatternCollection)base["patterns"]; }
    } 

  }

  [ConfigurationCollection(typeof(Pattern), AddItemName="pattern")]
  public class PatternCollection : ConfigurationElementCollection
  {
    protected override ConfigurationElement CreateNewElement()
    {
      return new Pattern();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (element as Pattern).Expression;
    }
  }

  public class Pattern : ConfigurationElement
  {
    
    [ConfigurationProperty("expression", IsRequired = true)]
    public string Expression
    {
      get { return (string)base["expression"]; }
      set { base["expression"] = value; }
    }

    [ConfigurationProperty("color", IsRequired = true)]
    public string Color
    {
      get { return (string)base["color"]; }
      set { base["color"] = value; }
    }

    [ConfigurationProperty("bold", IsRequired = false, DefaultValue = false)]
    public bool Bold
    {
      get { return (bool)base["bold"]; }
      set { base["bold"] = value; }
    }

  }

}
