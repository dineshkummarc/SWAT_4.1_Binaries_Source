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
using SWAT.AbstractionEngine.Configuration;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Threading;
using System.IO;

namespace SWAT.AbstractionEngine
{
  public class InvokeResult
  {
    private bool _success;
    private string _failureMessage;
    private string _returnValue;
    private string _instanceId;
    private string _commandName;
   //Needed to save values
   public string ReturnValue
   {
      get { return _returnValue; }
      set { _returnValue = value; }

   }

    public string CommandName
    {
      get { return _commandName; }
      set { _commandName = value; }

    }

    public string FailureMessage
    {
      get { return _failureMessage; }
      set { _failureMessage = value; }
    }
	

    public bool Success
    {
      get { return _success; }
      set { _success = value; }
    }

    public string InstanceID
    {
      get { return _instanceId; }
      set { _instanceId = value; }
    }
  }

  public class InvokeManager : IDisposable
  {
    private Assembly swatAssembly;
    private MethodInfo[] iBrowserCommandsMethods;
    public MethodMappings _mappings = new SWAT.AbstractionEngine.Configuration.MethodMappings();
    private WebBrowser _browser;

    public InvokeManager(SWAT.BrowserType browserType, IVariableRetriever vars)
    {
        //_mappings = (MethodMappings)System.Configuration.ConfigurationManager.GetSection("SWATMethodMappings");
        //Assembly browserAssem = Assembly.LoadFrom(_mappings.WebBrowserAssembly);
        //_browser = (SWAT.WebBrowser)browserAssem.CreateInstance(_mappings.WebBrowserType, false, BindingFlags.CreateInstance, null, new object[] { browserType }, null, new object[] { });
        _browser = new WebBrowser(browserType, vars);
        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
        path = new Uri(Path.Combine(path, "SWAT.Core.dll")).LocalPath;
        swatAssembly = Assembly.LoadFrom(path);        
        iBrowserCommandsMethods = swatAssembly.GetType("SWAT.IBrowserCommands").GetMethods();
    }

    public InvokeResult Invoke(string browserCommand)
    {
      return Invoke(browserCommand, new System.Collections.Specialized.StringCollection());
    }

    public InvokeResult Invoke(string browserCommand, System.Collections.Specialized.StringDictionary parameters)
    {
      MethodMapping methodMapping = _mappings.Mappings[browserCommand];
      System.Collections.Specialized.StringCollection newParameters = new System.Collections.Specialized.StringCollection();

      //for (int i = 0; i < methodMapping.Parameters.Count; i++)
      //{
      //  newParameters.Add(parameters[methodMapping.Parameters[i].ParameterName]);
      //}

      foreach (ParameterInfo parInfo in _browser.GetType().GetMethod(browserCommand).GetParameters())
      {
        newParameters.Add(parameters[parInfo.Name]);
      }

      return Invoke(browserCommand, newParameters);
    }

    public InvokeResult Invoke(string browserCommand, System.Collections.Specialized.StringCollection parameters)
    {
      InvokeResult invokeResult = new InvokeResult();
      invokeResult.CommandName = browserCommand;
      
      try
      {
        object[] convertedParams = getParameters(browserCommand, parameters);
        object result = _browser.GetType().InvokeMember(browserCommand, System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.GetProperty | BindingFlags.ExactBinding | BindingFlags.OptionalParamBinding, null, _browser, convertedParams);

        invokeResult.InstanceID = this.GetHashCode().ToString();

        invokeResult.Success = true;
        //if (result != null)
        //    invokeResult.ReturnValue = (String)result;
        if (result != null) //GetElementAttribute returns a string
            invokeResult.ReturnValue = result.ToString(); //Set ReturnValue to the value returned by the invoker
        if (result is bool)
          invokeResult.Success = (bool)result;
      }
      catch (TargetInvocationException ex)
      {
          string image = _browser.TakeScreenshot(browserCommand);
          
          invokeResult.Success = false;
          invokeResult.FailureMessage = ex.InnerException.Message + image;
      }
      if (isBrowserCommand(browserCommand))
          Thread.Sleep(SWAT.WantDelayBetweenCommands.DelayBetweenCommands * 1000);
      return invokeResult;
    }

    private bool isBrowserCommand(string command)
    {
        foreach (MethodInfo method in iBrowserCommandsMethods)
        {
            if (method.Name == command)
                return true;
        }
        return false;
    }

    protected object[] getParameters(string methodName, System.Collections.Specialized.StringCollection parameters)
    {
        System.Collections.ArrayList paramList = new System.Collections.ArrayList();

        //for (int i = 0; i < parameters.Count; i++)
        //{
        //  if (Type.GetType(mappingParams[i].Type).IsEnum)
        //    paramList.Add(Enum.Parse(Type.GetType(mappingParams[i].Type), parameters[i].ToString(), true));
        //  else
        //    paramList.Add(Convert.ChangeType(parameters[i], Type.GetType(mappingParams[i].Type)));
        //}


        //bool foundMatchingMethod = false; //UNUSED VARIABLE
        foreach (MethodInfo  method in _browser.GetType().GetMethods())
        {
            if(method.Name == methodName && method.GetParameters().Length == parameters.Count)
            {
            //    foundMatchingMethod = true; //UNUSED VARIABLE
                int i = 0;
                try
                {
                    foreach (ParameterInfo parInfo in method.GetParameters())
                    {
                        if (parInfo.ParameterType.IsEnum)
                            paramList.Add(Enum.Parse(parInfo.ParameterType, parameters[i].ToString(), true));
                        else if (parInfo.ParameterType == typeof(DateTime))
                            paramList.Add(DateTime.Parse(parameters[i]));
                        else if (parInfo.ParameterType == typeof(int))
                        {
                            paramList.Add(Convert.ToInt32(parameters[i], 10)); //certain machines crash if you don't specify the base (10)
                        }
                        else
                            paramList.Add(Convert.ChangeType(parameters[i], parInfo.ParameterType));

                        i++;
                    }
                }
                catch (FormatException) //If it find a method with the same name and amount of parameters, but with different param types, it will throw an exception
                {
                    paramList = new System.Collections.ArrayList();
                    continue;
                }

            break;
        }
    }
    
    return paramList.ToArray();
}



    // Create a custom section.
    // It will contain a nested section as 
    // deined by the UrlsSection (<urls>...</urls>).
    private void CreateSection(string sectionName)
    {
      // Get the current configuration (file).
      System.Configuration.Configuration config =
              ConfigurationManager.OpenExeConfiguration(
              ConfigurationUserLevel.None);

      MethodMappings mappingsSection;

      // Create an entry in the <configSections>. 
      if (config.Sections[sectionName] == null)
      {
        mappingsSection = new MethodMappings();
        config.Sections.Add(sectionName, mappingsSection);
        config.Save();
      }

      // Create the actual target section and write it to 
      // the configuration file.
      if (config.Sections["/configuration/" + sectionName] == null)
      {
        mappingsSection = config.GetSection(sectionName) as MethodMappings;
        mappingsSection.SectionInformation.ForceSave = true;
        config.Save(ConfigurationSaveMode.Full);
      }
    }

    // Exposes the KillAllOpenBrowsers for CloseBrowsersBeforeTestStart implementation.
    public void KillBrowsers()
    {
        _browser.KillAllOpenBrowsers();
    }

    public void Dispose()
    {
      _browser.Dispose();
    }

    public string InstanceID
    {
      get { return this.GetHashCode().ToString(); }
    }

  }

}
