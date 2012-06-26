using System;
using System.IO;

namespace SWAT.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                string inputPath = args[0];
                string outputPath = args[1];
                string outputDir = outputPath;

                if (outputPath.LastIndexOf('\\') >= 0)  //check that '\\' exists
                    outputDir = outputPath.Substring(0, outputPath.LastIndexOf('\\'));

                try
                {
                    var handler = new CommandLineHandler(args[0]);
                    handler.Start();
                    handler.Save(args[1]);

                    System.Console.WriteLine(@"Finished!");
                }
                catch(DirectoryNotFoundException e)
                {
                    string unknownDirectory = e.Message.Substring(e.Message.IndexOf('\'') + 1);
                    unknownDirectory = unknownDirectory.Substring(0, unknownDirectory.LastIndexOf('\''));
                    string pathNotFound = "";

                    if (unknownDirectory.Equals(inputPath))
                    {
                        pathNotFound = "Input";

                        if (!Directory.Exists(outputDir))
                        {
                            pathNotFound += " and output";
                            unknownDirectory += " " + outputDir;
                        }
                    }

                    else if (unknownDirectory.Equals(outputPath))
                        pathNotFound = "Output";

                    System.Console.WriteLine(pathNotFound + @" directory not found: " + unknownDirectory);
                }
                catch (FileNotFoundException)
                {
                    System.Console.WriteLine(@"File not found: " + args[0]);
                }
            }
            else
            {
                System.Console.WriteLine(@"Error: Unrecognized or incomplete command line.");
                System.Console.WriteLine(@"USAGE: SWAT.Console input.txt output.html");
            }
        }
    }
}