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
using System.Reflection;

namespace SWAT.Auto_Complete.AssemblyReader
{
    /// <summary>
    /// A utility class to read the SWAT assembly (SWAT.dll) and gather swat command definitions for intellisense.
    /// It builds an autocompletion lookup tree is created with SWAT command definitions.
    /// </summary>
    public class SwatReader
    {
        #region Custom members

        /// <summary>
        /// SWAT.dll
        /// </summary>
        private Assembly swatAssembly;

        /// <summary>
        /// This is the class in SWAT.dll where the swat commands are defined
        /// </summary>
        private Type webBrowserClass;

        /// <summary>
        /// A general tree to store swat command definition
        /// </summary>
        //private GeneralTree<KeyValuePair<String, Type>> tree;
        private List<String> commands;

        private List<MethodInfo> methods = new List<MethodInfo>();

        #endregion

        #region properties

        public List<MethodInfo> swatMethods
        {
            get { return this.methods; }
        }

        public Assembly swatAssemblyProperty
        {
            get { return this.swatAssembly; }
        }

        #endregion

        #region Static members

        private static Type stringType = "".GetType ( );

        /// <summary>
        /// Public methods in WebBrowser that are NOT swat commands.
        /// </summary>
        private static String[] discardMethods = { "Dispose", "get_CurrentLocation", 
            "GetType", "ToString", "Equals", "GetHashCode" };
        
        #endregion

        #region Constructor

        //disable default constructor.
        private SwatReader() { }

        /// <summary>
        /// Creates a new SwatReader instance.
        /// </summary>
        /// <param name="filepath">Full path + filename of SWAT.dll assembly.</param>
        /// <exception cref="ArgumentException">if filepath does not refer to SWAT.dll</exception>
        public SwatReader(String filepath)
        {
            //Exceptions to be caught at higher level
            //loads the SWAT.dll assembly
            swatAssembly = Assembly.LoadFrom (filepath);

            //make sure we are loading the right assembly.
            if ( swatAssembly.GetName ( ).Name.Equals ("SWAT.dll") )
                throw new ArgumentException ("Error processing SWAT assembly. A file other than SWAT.dll was supplied.");

            //frees resources automatically
            webBrowserClass = swatAssembly.GetType ("SWAT.WebBrowser");

            Array.Sort (discardMethods);
            this.commands = new List<string> ( );
            //tree.Root = new GeneralTreeNode<string> ("root");
            //reusableNodes = new Dictionary<string, LinkedList<GeneralTreeNode<string>>> ( );

            this.readCommands ( );//populate the tree
        }
        #endregion

        #region Methods

        //returns the parameter type
        public string getParameter(string methodName, int parameterIndex)
        {
            foreach (MethodInfo method in methods)
            {
                if (method.Name.Equals(methodName))
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    try
                    {
                        return parameters[parameterIndex].ParameterType.ToString();
                    }
                    catch
                    {
                        return "none";
                    }
                }
            }

            return "none";
        }

        //returns the parameter name
        public string getParameterByName(string methodName, int parameterIndex)
        {
            foreach (MethodInfo method in methods)
            {
                if (method.Name.Equals(methodName))
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    try
                    {
                        return parameters[parameterIndex].Name;
                    }
                    catch
                    {
                        return "none";
                    }
                }
            }

            return "none";
        }

        private void readCommands()
        {
            MethodInfo[] allMethods = this.webBrowserClass.GetMethods ( );
            foreach ( MethodInfo method in allMethods )
            {
                string cmdName = method.Name;

                if ( method.IsPublic && !method.IsConstructor && !this.isDiscardMethod (cmdName) )
                {
                    if(!commands.Contains(cmdName))
                        commands.Add (cmdName);
                    
                    methods.Add(method);
                }
            }

            stripMethods();
        }

        //leave only the methods with more parameters
        private void stripMethods()
        {
            List<MethodInfo> duplicates = new List<MethodInfo>();
            
            //methods.Sort(); not sorting now because all overloaded methods are just one next to the other
            //but will need to sort later to keep consistency with futures commands
            for (int i = 0; i < methods.Count - 1; i++)
            {
                //leave only the 'GetElementAttribute' that has 4 parameters
                if (methods[i].Name.Equals("GetElementAttribute"))
                {
                    if (methods[i].GetParameters().Length != 4)
                    {
                        duplicates.Add(methods[i]);
                        continue;
                    }
                    else
                        continue;
                }
                //leave only the 'SetElementAttribute' that has 5 parameters and don't have 'AttributeType'
                else if (methods[i].Name.Equals("SetElementAttribute"))
                {
                    if (methods[i].GetParameters().Length != 5
                        || getParameter("SetElementAttribute", 2).Equals("SWAT.AttributType"))
                    {
                        duplicates.Add(methods[i]);
                        continue;
                    }
                    else
                        continue;
                }

                //leave only 1 overriden version of the method with more parameters
                if (methods[i].Name.Equals(methods[i + 1].Name))
                {
                    if (methods[i].GetParameters().Length <= methods[i + 1].GetParameters().Length)
                        duplicates.Add(methods[i]);
                    else
                        duplicates.Add(methods[i + 1]);
                }
            } 


            for (int i = 0; i < duplicates.Count; i++)
                methods.Remove(duplicates[i]);
        }

        public List<String> getCommands()
        {
            return this.commands;
        }
        
        #endregion

        #region Util
        
        /// <summary>
        /// Checks if a given method name is not a SWAT command.
        /// </summary>
        /// <param name="methodName">the name of the method to check.</param>
        /// <returns>true if method with name methodName does not refer to a SWAT 
        /// command, false otherwise.</returns>
        private bool isDiscardMethod(String methodName)
        {
            foreach ( String name in discardMethods )
            {
                if ( name.Contains (methodName) ) return true;
            }
            return false;
        }
        
        #endregion

    }
}
