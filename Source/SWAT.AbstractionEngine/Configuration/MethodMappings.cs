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
using System.Collections;
using System.Configuration;

namespace SWAT.AbstractionEngine.Configuration
{

  public class MethodMappings : System.Configuration.ConfigurationSection
  {

    // Declare the urls collection property.
    // Note: the "IsDefaultCollection = false" instructs 
    // .NET Framework to build a nested section of 
    // the kind <urls> ...</urls>.
    [ConfigurationProperty("Mappings", IsDefaultCollection = false)]
    [ConfigurationCollection(typeof(MethodMappingCollection),
        AddItemName = "addMethodMapping",
        ClearItemsName = "clearMethodMapping",
        RemoveItemName = "removeMethodMapping")]
    public MethodMappingCollection Mappings
    {
      get
      {
        MethodMappingCollection methodCollection =
        (MethodMappingCollection)base["Mappings"];
        return methodCollection;
      }
    }

      /// <summary>
      /// Modified from SWAT.dll to SWAT.Core.dll
      /// </summary>
    [ConfigurationProperty("WebBrowserAssembly", DefaultValue="SWAT.Core.dll")]
    public string WebBrowserAssembly
    {
      get
      {
        return (string)this["WebBrowserAssembly"];
      }

      set
      {
        this["WebBrowserAssembly"] = value;
      }
    }

    [ConfigurationProperty("WebBrowserType", DefaultValue = "SWAT.WebBrowser")]
    public string WebBrowserType
    {
      get
      {
        return (string)this["WebBrowserType"];
      }

      set
      {
        this["WebBrowserType"] = value;
      }
    }

  }

  public class MethodMappingCollection : System.Configuration.ConfigurationElementCollection
  {
    
        public MethodMappingCollection()
        {
            //MethodMapping mapping = (MethodMapping)CreateNewElement();
            //Add(mapping);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MethodMapping();
        }
        
        protected override Object GetElementKey(ConfigurationElement element)
        {
          return ((MethodMapping)element).CommandName;
        }
        
        public MethodMapping this[int index]
        {
            get
            {
                return (MethodMapping)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

      new public MethodMapping this[string commandName]
      {
        get
        {
          return (MethodMapping)BaseGet(commandName);
        }
      }
        
        public int IndexOf(MethodMapping mapping)
        {
            return BaseIndexOf(mapping);
        }
        
        public void Add(MethodMapping mapping)
        {
            BaseAdd(mapping);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
        
        public void Remove(MethodMapping mapping)
        {
            if (BaseIndexOf(mapping) >= 0)
                BaseRemove(mapping.MethodName);
        }
        
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string commandName)
        {
          BaseRemove(commandName);
        }
        
        public void Clear()
        {
            BaseClear();
        }


  }


  public class MethodParameterCollection : System.Configuration.ConfigurationElementCollection
  {

    public MethodParameterCollection()
    {
      //MethodMapping mapping = (MethodMapping)CreateNewElement();
      //Add(mapping);
    }

    public override ConfigurationElementCollectionType CollectionType
    {
      get
      {
        return ConfigurationElementCollectionType.AddRemoveClearMap;
      }
    }

    protected override ConfigurationElement CreateNewElement()
    {
      return new MethodParameter();
    }

    protected override Object GetElementKey(ConfigurationElement element)
    {
      return ((MethodParameter)element).Index;
    }

    public MethodParameter this[int index]
    {
      get
      {
        return (MethodParameter)BaseGet(index);
      }
      set
      {
        if (BaseGet(index) != null)
        {
          BaseRemoveAt(index);
        }
        BaseAdd(index, value);
      }
    }

    public int IndexOf(MethodParameter parameter)
    {
      return BaseIndexOf(parameter);
    }

    public void Add(MethodParameter parameter)
    {
      BaseAdd(parameter);
    }
    protected override void BaseAdd(ConfigurationElement element)
    {
      BaseAdd(element, false);
    }

    public void Remove(MethodParameter parameter)
    {
      if (BaseIndexOf(parameter) >= 0)
        BaseRemove(parameter.Index);
    }

    public void RemoveAt(int index)
    {
      BaseRemoveAt(index);
    }

    public void Remove(int index)
    {
      BaseRemove(index);
    }

    public void Clear()
    {
      BaseClear();
    }


  }

  public class MethodParameter : System.Configuration.ConfigurationElement
  {
    [ConfigurationProperty("Index",
            IsRequired = true, IsKey = true)]
    public int Index
    {
      get
      {
        return (int)this["Index"];
      }

      set
      {
        this["Index"] = value;
      }
    }

    [ConfigurationProperty("Type",
            IsRequired = true, IsKey = false)]
    public string Type
    {
      get
      {
        return this["Type"].ToString();
      }

      set
      {
        this["Type"] = value;
      }
    }


    [ConfigurationProperty("ParameterName",
            IsRequired = true, IsKey = false)]
    public string ParameterName
    {
      get
      {
        return this["ParameterName"].ToString();
      }

      set
      {
        this["ParameterName"] = value;
      }
    }
  }



  public class MethodMapping : System.Configuration.ConfigurationElement
  {
    [ConfigurationProperty("MethodName",
            IsRequired = true, IsKey = false)]
    public string MethodName
    {
      get
      {
        return this["MethodName"].ToString();
      }

      set
      {
        this["MethodName"] = value;
      }
    }

    [ConfigurationProperty("CommandName",
            IsRequired = true, IsKey = true)]
    public string CommandName
    {
      get
      {
        return this["CommandName"].ToString();
      }

      set
      {
        this["CommandName"] = value;
      }
    }

    [ConfigurationProperty("Parameters", IsDefaultCollection = false)]
    [ConfigurationCollection(typeof(MethodParameterCollection),
        AddItemName = "addMethodParameter",
        ClearItemsName = "clearMethodParameter",
        RemoveItemName = "removeMethodParameter")]
    public MethodParameterCollection Parameters
    {
      get
      {
        MethodParameterCollection methodCollection =
        (MethodParameterCollection)base["Parameters"];
        return methodCollection;
      }
    }

  }

}
