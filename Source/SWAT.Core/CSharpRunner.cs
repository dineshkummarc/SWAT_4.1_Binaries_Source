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
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace SWAT
{
    public class CSharpRunner : CodeRunner
    {        
        public CSharpRunner(IVariableRetriever v) : base(v)
        { }

        protected override string RunCode(string source, WebBrowser browser, string assems)
        {
            assems = System.Reflection.Assembly.GetAssembly(typeof(FireFox)).Location + ";" + assems.Trim();
            string[] assemArray = assems.Split(';');
            using (CSharpCodeProvider compiler = new CSharpCodeProvider())
            {
				source = source.Trim();

                //include SWAT.Core.dll when compiling
                CompilerParameters options = new CompilerParameters(assemArray);
                options.GenerateInMemory = true;

				// Put newlines at the end of #define and /define so that they work
				// Defines are always #define _symbol_ or /define:_symbol_[;_symbol_]
				Regex defineFixer = new Regex("((#define +\\S+)|(/d(efine)?:(\\S+;?)+))\\s*");
				source = defineFixer.Replace(source,"\n$1\n");

				// Parentheses matching for preprocessor arguments, since they must 
				// go on their own line
				Regex preprocessors = new Regex("(#if|#elif|#else|#endif)");
				int index = 0;
				while (index != -1)
				{
					Match match = preprocessors.Match(source, index);
					if (!match.Success)
						break;
					index = match.Index;
					source = source.Insert(index, "\n");
					bool noargs = match.ToString().Contains("endif") || match.ToString().Contains("else");
					bool foundSpace = false;
					int parenCount = 0;
					for (; index<source.Length; index++)
					{
						if (source[index] == ' ' && !foundSpace)
						{
							for (; (source[index] == ' ' || source[index] == '\t') && index < source.Length; index++);
							foundSpace = true;

							if (noargs)
								break;
							
							if (source[index] != '(')
							{
								for (; (source[index] != ' ' && source[index] != '\t') && index < source.Length; index++);
								break;
							}
						}
						if (source[index] == '(')
							parenCount++;
						if (source[index] == ')')
							parenCount--;
						if (parenCount == 0 && foundSpace)
						{
							index++;
							break;
						}
					}
					source = source.Insert(index, "\n");
				}

				// The usings go right before the namespace or the class declaration
				// This way they appear right after the #defines
				Regex usingFixer = new Regex("(namespace|public|class)");
				source = source.Insert(usingFixer.Match(source).Index,
					"using System.Collections.Generic;");
				
                //insert a method to add references to this and the runtime variables
				int indexToInsert = -1;
                if (source.Contains("namespace"))
				{
                    //get the second curly bracket so the method is inside a class
                    indexToInsert = source.IndexOf('{', source.IndexOf('{') + 1) + 1;
				}

                else
				{
                    //if there is no namespace, then just find the first curly bracket
                    indexToInsert = source.IndexOf('{') + 1;
				}

                source = source.Insert(indexToInsert, 
					"public static WebBrowser browser; "+
					"public static IVariableRetriever swatVars;" +
                    "public static void SetVars(WebBrowser b, IVariableRetriever vars)"+
						"{browser=b; swatVars=vars;}");

                //Compile
                CompilerResults results = compiler.CompileAssemblyFromSource(options, source);
                if (results.Errors.Count > 0 || results.NativeCompilerReturnValue != 0)
                {
                    StringBuilder errormessage = new StringBuilder("Error in compiling dynamic C#\n");
                    foreach (object error in results.Errors)
                        errormessage.Append(error + "\n");
                    throw new RunScriptCompilerException(errormessage.ToString());
                }

                string classWithMain = null, classWithVars = null;
                Assembly assembly = results.CompiledAssembly;

                //Find the class with the Main method and the SetVars method
                Type[] listOfTypes = assembly.GetTypes();
                foreach (Type t in listOfTypes)
                {
                    MethodInfo[] listOfMethods = t.GetMethods();
                    foreach (MethodInfo m in listOfMethods)
                    {
                        if (m.ToString().Contains("Main"))
                        {
                            classWithMain = t.ToString();
                        }
                        else if (m.ToString().Contains("SetVars"))
                        {
                            classWithVars = t.ToString();
                        }
                    }
                    if (classWithMain != null && classWithVars != null)
                        break;
                }
                if (classWithMain == null)
                    throw new RunScriptCompilerException("Main method not present");

                Type type = assembly.GetType(classWithMain);
                object obj = assembly.CreateInstance(classWithMain);

                //Run SetVars
                MethodInfo method = type.GetMethod("SetVars");
                method.Invoke(obj, new object[] { browser, variables });

                string result;

                //Run the Main
                try
                {
                    method = type.GetMethod("Main");
                    result = method.Invoke(obj, null).ToString();
                }
                catch (Exception ex)
                {
                    throw new RunScriptRuntimeException("Error in the Main Method : " + ex.Message);
                }    
                
                return result;
            }
        }
    }
}
