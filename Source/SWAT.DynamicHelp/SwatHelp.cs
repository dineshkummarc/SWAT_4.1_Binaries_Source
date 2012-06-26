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
using System.ComponentModel;
using System.Net.NetworkInformation;
using SWAT.Utilities;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace SWAT.DynamicHelp
{
    /// <summary>
    /// Manages pulling help dynamically from sourgeforge and caching it locally.
    /// </summary>
    public class SwatHelp
    {
        #region Attributes
        /// <summary>
        /// Runs the downloading and updating of help files in a separate thread.
        /// </summary>
        private BackgroundWorker worker = new BackgroundWorker ();
        private BackgroundWorker copyWorker = new BackgroundWorker ();
        //private int currentLineNo;
        /// <summary>
        /// Path to store help files (html format) pulled from sourceforge.
        /// </summary>
        private String appDataPath = "";
        private String helpFolderPath = "";
        /// <summary>
        /// SWAT editor's main form.
        /// </summary>
        private Form editorForm = new Form ();
        /// <summary>
        /// The control used to display html help files retrieved from sourceforge.
        /// </summary>
        private WebBrowser helpBrowser = new WebBrowser ();
        /// <summary>
        /// Stores caching status of sourceforge files. True if sourceforge help files
        /// have been successfully saved locally. False otherwise.
        /// </summary>
        private bool cached;
        //private bool activeConnection;
        /// <summary>
        /// Indicates the existance of local help files. True if there are local help files, 
        /// false otherwise.
        /// </summary>
        private bool helpFilesExist = false;
        /// <summary>
        /// Dictionary with commandname/localpath key value pairs for
        /// easy loading of the help files on the webbrowser control.
        /// </summary>
        Dictionary<String, String> helpPaths;
        private int downloadMessageCount;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of SwatHelp.
        /// </summary>
        /// <param name="helpBrowser">SWAT editor's help browser control on the editor's main form.</param>
        public SwatHelp(WebBrowser helpBrowser)
        {
            /* SWAT HELP TAB DISABLED
            if (helpBrowser == null)
            {
                throw new ArgumentException("The WebBrowser control attached to the SWAT editor's main" +
                   " form must be specified.");
            }

            this.helpBrowser = helpBrowser;

            //this.currentLineNo = 0;

            this.editorForm = helpBrowser.FindForm();

            this.appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            //Set up the worker (downloads help files from wiki)
            this.worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            //set up the copy worker (updates currently used help files if different from downloaded ones)
            this.copyWorker = new BackgroundWorker();
            copyWorker.DoWork += new DoWorkEventHandler(copyWorker_DoWork);
            copyWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(copyWorker_RunWorkerCompleted);

            this.cached = false;
            //this.activeConnection = false;
            if (this.hasHelpFiles())
            {
                this.helpFilesExist = true;
            }
            else this.helpFilesExist = false;

            this.helpPaths = null;

            StringBuilder helpFilePath = new StringBuilder();
            helpFilePath.Append(this.appDataPath);
            helpFilePath.Append('\\');
            helpFilePath.Append(SWATHelpConstants.SWAT_FOLDER_NAME);
            helpFilePath.Append('\\');
            helpFilePath.Append(SWATHelpConstants.SWAT_HELP_FOLDER_NAME);
            this.helpFolderPath = helpFilePath.ToString();

            this.downloadMessageCount = 0;

            this.cacheHelpFiles();
             */
        }

        #endregion


        #region Properties
        /// <summary>
        /// Gets the WebBrowser object associated with a SwatHelp instance.
        /// </summary>
        public WebBrowser HelpBrowser
        {
            get { return this.helpBrowser; }
        }
        /// <summary>
        /// Gets the help cache status. True if help files have been cached from the web, false otherwise.
        /// </summary>
        public bool Cached
        {
            get { return this.cached; }
        }
        #endregion


        #region Methods

        /// <summary>
        /// Displays the main help file.
        /// </summary>
        public void displayHelp()
        {
            if (this.downloadMessageCount > 1)
            {
                //do nothing
            }
            else if (!this.hasHelpFiles() && this.downloadMessageCount<=1)
            {
                this.downloadMessageCount++;
                //MessageBox.Show("No help files available yet. Help files are being downloaded.");
            }
            else
            {
                if (this.helpBrowser.Document !=null && this.helpBrowser.Document.Url.AbsoluteUri.Contains("MainHelp"))
                {
                    //main help is already displayed, no need to refresh
                    return;
                }
                StringBuilder path = new StringBuilder(this.helpFolderPath);
                path.Append("\\");
                path.Append(SWATHelpConstants.SWAT_HELP_FILE_PREFIX);
                path.Append("MainHelp.html");
                this.helpBrowser.Navigate(path.ToString());
            }
            //displayHelp("MainHelp");
        }
        /// <summary>
        /// Displays command specific help files.
        /// </summary>
        /// <param name="swatCommandName">the command name.</param>
        public void displayHelp(String swatCommandName)
        {
            if (this.downloadMessageCount > 1)
            {
                //do nothing
            }
            else if (!this.hasHelpFiles())
            {
                //MessageBox.Show("No help files available yet. Help files are being downloaded.");
            }
            else
            {
                if (this.helpPaths == null)
                {

                    StringBuilder helpFilePath = new StringBuilder(this.helpFolderPath);
                    helpFilePath.Append('\\');
                    helpFilePath.Append(SWATHelpConstants.SWAT_HELP_FILE_PREFIX);
                    helpFilePath.Append("MainHelp.html");
                    //this.helpBrowser.Navigate(mainHelpFilePath.ToString());

                    this.helpPaths = this.getCommandLinks(helpFilePath.ToString());

                    Dictionary<String, String> updatedHelpPaths = new Dictionary<String, String>(helpPaths.Count);

                    //now update the links
                    DirectoryInfo dir = new DirectoryInfo(this.helpFolderPath);

                    //get only help files that are not temp
                    FileInfo[] files = dir.GetFiles("_swat*.html");

                    try
                    {
                        foreach (FileInfo file in files)
                        {
                            if (file.Name.Contains("Temp"))
                            {
                                //skip temporary files
                                continue;
                            }
                            //adds commandName/localHelpFilePath pairs to dictionary
                            updatedHelpPaths.Add(extractCmdName(file.FullName), file.FullName);
                        }
                    }
                    catch
                    //catch (ArgumentException ex1)  //UNUSED VARIABLE
                    {
                        //occurs when adding a key that has been added alrady
                        //MessageBox.Show(ex1.Message + "\nError code: 1a");
                    }

                    //catch (Exception ex)
                    //{
                    //    //MessageBox.Show(ex.Message + "\nError code: 1b");
                    //}

                    //update helpPaths
                    this.helpPaths = updatedHelpPaths;
                }
                try
                {
                    //------------temp fix for command modifiers----------------
                    String cmd = SWAT.Utilities.StringUtil.removeFirstNonAlpha(swatCommandName);
                    //----------------------------------------------------------
                    if (this.helpBrowser.Document != null && this.helpBrowser.Document.Url.AbsoluteUri.Contains(cmd))
                    {
                        //help for this command is already displayed, no need to refresh
                        return;
                    }
                    this.helpBrowser.Navigate(this.helpPaths[cmd]);
                }
                catch
                //catch (KeyNotFoundException e)  //UNUSED VARIABLE
                {
                    //MessageBox.Show(String.Format("{0} is not a valid SWAT command.", swatCommandName) + "\nError code: 1");
                }
            }
        }
        /// <summary>
        /// Displays help for a command which is the first token in the given line.
        /// </summary>
        /// <param name="lineFromEditor">a line of text form the swat editor.</param>
        public void displayHelpForLine(String lineFromEditor)
        {
            if (String.IsNullOrEmpty(lineFromEditor))
            {
                displayHelp();
                return;
            }
            //extract command from line and call displayHelp(commandname)
            StringTokenizer st = new StringTokenizer(lineFromEditor, "|");

            displayHelp(st.NextToken());
        }
        /*
        /// <summary>
        /// Searces the all help files for a search string. If this string is found a new help file is
        ///  created with links to all the help files containing that string. The newly created help file
        /// must look just like the main help file but listing only found commands.
        /// </summary>
        /// <param name="searchString">the string to search for.</param>
        /// <returns>True if there is at least on occurrence of "searchString" in the help files.</returns>
        public bool searchHelp(String searchString)
        {

        }
         */ 
        #endregion


        #region Util
        /// <summary>
        /// Compared downloaded help files with currently used help files.
        /// </summary>
        /// <returns>true if downloaded files differ from currently used files, false otherwise.</returns>
        private bool filesChanged()
        {
            FileInfo[] helpFiles = 
                FileUtils.getFilesContaining(this.helpFolderPath, SWATHelpConstants.SWAT_HELP_FILE_PREFIX + "*" + SWATHelpConstants.SWAT_HELP_FILE_EXTENSION);

            if (SWAT.Utilities.NumericUtil.isEven(helpFiles.Length))
            {
                int halfCount = helpFiles.Length / 2;
                Dictionary<int, FileInfo> downloadedFiles = new Dictionary<int,FileInfo>(halfCount);
                Dictionary<int, FileInfo> currentFiles = new Dictionary<int,FileInfo>(halfCount);

                foreach (FileInfo fi in helpFiles)
                {
                    if (fi.Name.Contains(SWATHelpConstants.SWAT_TEMP_HELP_FILE_PREFIX))
                    {
                        downloadedFiles.Add(downloadedFiles.Count, fi);
                    }
                    else
                    {
                        currentFiles.Add(currentFiles.Count, fi);
                    }
                }

                for (int i = 0; i < currentFiles.Count; i++)
                {
                    if (!FileUtils.FileCompare(currentFiles[i], downloadedFiles[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// Runs in the background and caches help files from sourceforge.
        /// </summary>
        /// <returns></returns>
        private void cacheHelpFiles()
        {
            //start the worker
            this.worker.RunWorkerAsync();
        }
        /// <summary>
        /// Ensures that SWAT and Help folders exist.
        /// </summary>
        private void prepareStorage()
        {
            //Check if folders exist, if not create them
            StringBuilder path = new StringBuilder();
            path.Append(this.appDataPath);
            path.Append('\\');
            path.Append(SWATHelpConstants.SWAT_FOLDER_NAME);

            if (!FileUtils.DirExists(path.ToString()))
            {
                //create dir
                try
                {
                    Directory.CreateDirectory(path.ToString());
                }
                catch
                //catch (Exception e) //UNUSED VARIABLE
                {
                    //MessageBox.Show(e.Message + "\nError code: 2");
                }
            }

            path.Append('\\');
            path.Append(SWATHelpConstants.SWAT_HELP_FOLDER_NAME);

            if (!FileUtils.DirExists(path.ToString()))
            {
                //create dir
                try
                {
                    Directory.CreateDirectory(path.ToString());
                }
                catch
                //catch (Exception e) //UNUSED VARIABLE
                {
                    //MessageBox.Show(e.Message + "\nError code: 3");
                }
            }
        }

        /// <summary>
        /// Reads main help file from sourceforge wiki. This file is stored under %userprofile%\_swatMainHelpTemp.html.
        /// </summary>
        /// <returns>The contens of the main help file.</returns>
        private String readMainHtmlHelp()
        {
            StringBuilder contents = new StringBuilder();
            String currentLine = String.Empty;
            String mainHelpUrl = SWATHelpConstants.SRCFRG_BASE + SWATHelpConstants.ALLCMD;

            int retryCount = 0;
            while (true)
            {
                try
                {
                    HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(mainHelpUrl);
                    bool targetNotReached = true;
                    using (HttpWebResponse webresp = (HttpWebResponse)webreq.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(webresp.GetResponseStream()))
                        {
                            //Read only swat help
                            while (!reader.EndOfStream)
                            {
                                currentLine = reader.ReadLine();

                                if (!currentLine.Contains("<div class=\"wiki\" id=\"content_view\" >") && targetNotReached)
                                {
                                    continue;
                                }
                                else if (currentLine.Contains("<img"))
                                {
                                    //discard image references
                                    continue;
                                }
                                else
                                {
                                    targetNotReached = false;
                                    //read lines
                                    contents.AppendLine(currentLine);

                                    if (currentLine.Contains("</div> <!-- /wiki -->"))
                                    {
                                        //stop reading
                                        break;
                                    }
                                }
                            }
                            reader.Close();
                        }
                        webresp.Close();
                    }
                    break;
                }
                catch
                //catch (WebException e)
                {
                    //connection got interrupted
                    retryCount++;
                    if (retryCount >= SWATHelpConstants.SWAT_HELP_RETRY_MAX_COUNT) break;
                }
            }

            //done getting contents

            //Write contents to a file
            StringBuilder mainHelpFilePath = new StringBuilder();
            mainHelpFilePath.Append(this.appDataPath);
            mainHelpFilePath.Append('\\');
            mainHelpFilePath.Append(SWATHelpConstants.SWAT_FOLDER_NAME);
            mainHelpFilePath.Append('\\');
            mainHelpFilePath.Append(SWATHelpConstants.SWAT_HELP_FOLDER_NAME);
            mainHelpFilePath.Append('\\');
            mainHelpFilePath.Append(SWATHelpConstants.SWAT_TEMP_HELP_FILE_PREFIX);
            mainHelpFilePath.Append("MainHelp.html");

            String mainPath = mainHelpFilePath.ToString();
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText(mainPath);
                writer.Write(contents.ToString());
            }
            catch            
            //catch (UnauthorizedAccessException e)
            {
                /*MessageBox.Show(String.Format("Error creating help files due to " +
                    "access restrictions. Please ensure that {0} is accessible to the current user.",
                    mainHelpFilePath.ToString()));*/
            }
            /*
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "\nError code: 4");
            }
             */
            finally
            {
                writer.Close();
            }

            return mainPath;
        }

        /// <summary>
        /// Helper method to readAllCmdHelp. Reads individual help files from sourceforge.
        /// </summary>
        /// <param name="cmdLink">A KeyValuePair containing: CommandName/CommandLinkSuffix.</param>
        /// <returns>Path to the newly read command help file.</returns>
        private String readCmdHtmlHelp(KeyValuePair<String, String> cmdLink)
        {
            //cmdLink : CommandName/CommandLinkSuffix
            StringBuilder contents = new StringBuilder();
            String currentLine = String.Empty;
            String cmdHelpUrl = SWATHelpConstants.SRCFRG_BASE + cmdLink.Value;
            int retryCount = 0;
            while (true)
            {
                try
                {
                    HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(cmdHelpUrl);
                    bool targetNotReached = true;
                    using (HttpWebResponse webresp = (HttpWebResponse)webreq.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(webresp.GetResponseStream()))
                        {
                            //Read only swat help
                            while (!reader.EndOfStream)
                            {
                                currentLine = reader.ReadLine();

                                if (!currentLine.Contains("<div class=\"wiki\" id=\"content_view\" >") && targetNotReached)
                                {
                                    continue;
                                }
                                else if (currentLine.Contains("<img"))
                                {
                                    //discard image references
                                    continue;
                                }
                                else
                                {
                                    targetNotReached = false;
                                    //read lines
                                    contents.AppendLine(currentLine);

                                    if (currentLine.Contains("</div> <!-- /wiki -->"))
                                    {
                                        //stop reading
                                        break;
                                    }
                                }
                            }
                            reader.Close();
                        }
                        webresp.Close();
                    }
                    break;
                }
                catch
                //catch (WebException e) //UNUSED VARIABLE
                {
                    //connection got interrupted
                    retryCount++;
                    if (retryCount >= SWATHelpConstants.SWAT_HELP_RETRY_MAX_COUNT) break;
                }
            }
            //done getting contents

            //Write contents to a file
            StringBuilder cmdHelpFilePath = new StringBuilder();
            cmdHelpFilePath.Append(this.appDataPath);
            cmdHelpFilePath.Append('\\');
            cmdHelpFilePath.Append(SWATHelpConstants.SWAT_FOLDER_NAME);
            cmdHelpFilePath.Append('\\');
            cmdHelpFilePath.Append(SWATHelpConstants.SWAT_HELP_FOLDER_NAME);
            cmdHelpFilePath.Append('\\');
            cmdHelpFilePath.Append(SWATHelpConstants.SWAT_TEMP_HELP_FILE_PREFIX);
            cmdHelpFilePath.Append(cmdLink.Key);
            cmdHelpFilePath.Append(".html");

            String cmdPath = cmdHelpFilePath.ToString();
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText(cmdPath);
                writer.Write(contents.ToString());
            }
            catch
            //catch (UnauthorizedAccessException e)
            {
                /*MessageBox.Show(String.Format("Error creating help files due to " +
                    "access restrictions. Please ensure that {0} is accessible to the current user.",
                    cmdHelpFilePath.ToString()));*/
            }
            /*
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "\nError code: 5");
            }
            */
            finally
            {
                writer.Close();
            }

            return cmdPath;
        }


        /// <summary>
        /// Reads all html help files defined in links. Creates html files for each.
        /// </summary>
        /// <param name="links">A dictionary storing CommandName/CommandLinkSuffix pairs.</param>
        private void readAllCmdHelp(String mainHelpFilePath, Dictionary<String, String> links)
        {
            //Reads all cmd help files defined in main help and updates links in
            //main help to point to cached cmd help files (i.e. the ones being read here)

            //use xmlreader to cycle through elements in main help file
            //when an a element is found extract the text, use this text to find the desired link in the links dictionary
            //then call readCmdHtmlHelp(..)
            //then update the current node hdref attrib with the path returned by readCmdHtmlHelp(..) command

            Dictionary<String, String> updatedLinks = new Dictionary<String, String>(links.Count);
            String currentFilePath = String.Empty;
            foreach (KeyValuePair<String, String> linkKvp in links)
            {
                //read html help for each command and update path to local path
                //linkKvp.Value = readCmdHtmlHelp(linkKvp);
                //links[linkKvp.Key] = readCmdHtmlHelp(linkKvp);
              currentFilePath = SWAT.Utilities.StringUtil.removeLastSubstring(readCmdHtmlHelp(linkKvp), "Temp");
                updatedLinks.Add(linkKvp.Key, currentFilePath);
            }

            links = updatedLinks;//update links

            try
            {
                XmlDocument mainHelpDoc = new XmlDocument();
                mainHelpDoc.Load(mainHelpFilePath);

                //get all the nodes in main help to be updated
                XmlNodeList updateNodes = mainHelpDoc.SelectNodes("/div/a");

                XmlAttributeCollection currentAttribs = null;

                foreach (XmlNode n in updateNodes)
                {
                    currentAttribs = n.Attributes;
                    foreach (XmlAttribute a in currentAttribs)
                    {
                        if (a.Name.Equals("href"))
                        {
                            //update link
                            a.Value = links[SWAT.Utilities.StringUtil.removeSubstring(n.FirstChild.Value, " ")];
                            break;
                        }
                    }
                }
            
            //save the updated links back to the main help file
            mainHelpDoc.Save(mainHelpFilePath);
        }
        catch (Exception) { }
        }

        /// <summary>
        /// Reads all command help file links defined in main help file.
        /// </summary>
        /// <param name="mainHelpFileContents">Main help file path.</param>
        /// <returns>A dictionary storing CommandName/CommandLinkSuffix pairs.</returns>
        private Dictionary<String, String> getCommandLinks(String mainHelpFilePath)
        {
            //StringBuilder links = new StringBuilder();
            Dictionary<String, String> links = new Dictionary<String, String>();
            try
            {
                XmlTextReader xmlReader = new XmlTextReader(mainHelpFilePath);
                String cmdName = String.Empty;
                String cmdLink = String.Empty;
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (xmlReader.Name.Equals("a") && xmlReader.AttributeCount >= 2)
                            {
                                //a element, these are the elements containing the links

                                //stores key/value pairs as CommandName/CommandLinkSuffix
                                cmdLink = SWAT.Utilities.StringUtil.removeSubstring(xmlReader.GetAttribute(1), @"/");
                                xmlReader.Read();
                                if (xmlReader.NodeType.Equals(XmlNodeType.Text))
                                {
                                  cmdName = SWAT.Utilities.StringUtil.removeSubstring(xmlReader.Value, " ");
                                }
                                else
                                {
                                    //skip invalid node
                                    continue;
                                }
                                links.Add(cmdName, cmdLink);
                            }
                            break;
                        default: continue;
                    }
                }

                //At this point links must contain the links and command names
            }
            catch
            //catch (Exception e)
            {
                //MessageBox.Show(e.Message + "\nError code: 6");
            }

            return links;
        }

        /// <summary>
        /// Checks if the mainhelp file exists localy.
        /// </summary>
        /// <returns>true if the main help file exits, false otherwise.</returns>
        private bool hasHelpFiles()
        {
            StringBuilder mainHelpFilePath = new StringBuilder(this.helpFolderPath);
            mainHelpFilePath.Append('\\');
            mainHelpFilePath.Append(SWATHelpConstants.SWAT_HELP_FILE_PREFIX);
            mainHelpFilePath.Append("MainHelp.html");
            return FileUtils.FileExists(mainHelpFilePath.ToString());
        }

        /// <summary>
        /// Checks if the main temp help file exists localy.
        /// </summary>
        /// <returns>true if the main temp help file exits, false otherwise.</returns>
        private bool hasTempHelpFiles()
        {
            StringBuilder mainHelpFilePath = new StringBuilder(this.helpFolderPath);
            mainHelpFilePath.Append('\\');
            mainHelpFilePath.Append(SWATHelpConstants.SWAT_TEMP_HELP_FILE_PREFIX);
            mainHelpFilePath.Append("MainHelp.html");
            return FileUtils.FileExists(mainHelpFilePath.ToString());
        }

        /// <summary>
        /// Removes "Temp" from the temporary file name.
        /// </summary>
        /// <param name="tempPath">the path to the temporary help file.</param>
        /// <returns>The original file name with the string "Temp" removed.</returns>
        private String renameTempFile(String tempPath)
        {
            /*
            if (!tempPath.Contains("Temp"))
            {
                throw new ArgumentException("Invalid name for a temporary help file. " +
                    "Temporary help file names must contain the word Temp");
            }
            int i = tempPath.LastIndexOf("Temp");
            String firstPart = tempPath.Substring(0, i);
            return firstPart + tempPath.Substring(i + 4);
             */
          return SWAT.Utilities.StringUtil.removeLastSubstring(tempPath, "Temp");
        }

        /// <summary>
        /// Extracts the command name from a help file name. It takes either 
        /// a temporary file name or a cached help file name.
        /// </summary>
        /// <param name="localHelpFilePath"></param>
        /// <returns></returns>
        private String extractCmdName(String localHelpFilePath)
        {

            String cmdName = String.Empty;
            FileInfo fi = new FileInfo(localHelpFilePath);
            cmdName = fi.Name;
            cmdName = SWAT.Utilities.StringUtil.removeSubstring(cmdName, ".html");//remove extension
            cmdName = SWAT.Utilities.StringUtil.removeSubstring(cmdName, "_swat");//remove prefix
            cmdName = SWAT.Utilities.StringUtil.removeSubstring(cmdName, "Temp");
            cmdName = SWAT.Utilities.StringUtil.removeFirstNonAlpha(cmdName);
            return cmdName;
        }
        /// <summary>
        /// Extracts a command name from a wiki's attrib containing the link to the command's help page.
        /// </summary>
        /// <param name="xmlAttrib">the contents of a link attribute.</param>
        /// <returns>the command name.</returns>
        private String extractCommandFromAttrib(String xmlAttrib)
        {
          return SWAT.Utilities.StringUtil.removeSubstring(xmlAttrib,
                SWATHelpConstants.SWAT_WIKI_CMD_LINK_PREFIX);
        }
        #endregion


        #region Event handlers
        /// <summary>
        /// Downloads and updates help files from sourceforge.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //check for active connection
            //remove last forward slash for ping test
            String wiki = SWATHelpConstants.SRCFRG_BASE;
            
            if ("/".Equals(wiki.Substring(wiki.Length - 1)))
            {
                wiki = wiki.Substring(0, wiki.Length - 1);
            }
             
            if (!SWAT.Utilities.NetUtil.activeConnection())
            {
                /*MessageBox.Show("The SWAT editor is unable to updated help files at this time.\n" +
                    "No internet connection is available.", "Warning");*/
            }
            else if (!SWAT.Utilities.NetUtil.activeConnection(wiki))
            {
                /*MessageBox.Show("Unable to updated help files at this time.\n" +
                    "The Help repository is down.", "Warning");*/
            }
            else
            {
                //if a connection exists, proceed to cache files

                //this.activeConnection = true;

                this.prepareStorage();

                //read main help file
                String mainHelpFilePath = readMainHtmlHelp();

                //process main help file. i.e. read all command help files
                Dictionary<String, String> links = getCommandLinks(mainHelpFilePath);

                //reads all cmd help files and updates links in main help file to point to local help files
                readAllCmdHelp(mainHelpFilePath, links);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Run the copying process to have the files being displayed on the webbrowser control updated
            //maybe create another background worker to do this
            if (this.hasTempHelpFiles())
            {
                copyWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Sets cached to true and creates a dictionary that stores commandName/localHelpFilePath 
        /// pairs for easy help display on webbrowser control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void copyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //create a dictionary with commandname/localpath key value pairs for
            //easy loading of the help files on the webbrowser control

            helpPaths = new Dictionary<String, String>();

            StringBuilder path = new StringBuilder();
            path.Append(this.appDataPath);
            path.Append('\\');
            path.Append(SWATHelpConstants.SWAT_FOLDER_NAME);
            path.Append('\\');
            path.Append(SWATHelpConstants.SWAT_HELP_FOLDER_NAME);

            DirectoryInfo dir = new DirectoryInfo(path.ToString());

            //get only temp help files
            FileInfo[] files = dir.GetFiles(SWATHelpConstants.SWAT_HELP_FILE_PREFIX+"*"+SWATHelpConstants.SWAT_HELP_FILE_EXTENSION);

            try
            {
                foreach (FileInfo file in files)
                {
                    if (file.Name.Contains("Temp"))
                    {
                        //skip temporary files
                        continue;
                    }
                    //adds commandName/localHelpFilePath pairs to dictionary
                    helpPaths.Add(extractCmdName(file.FullName), file.FullName);
                }
            }
            catch
            //catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\nError code: 7");
            }

            this.cached = true;

            this.downloadMessageCount = 0;

            if (filesChanged())
            {

                

                this.displayHelp();

                
            }
                /*
            else
            {
                if (activeConnection)
                {
                    //MessageBox.Show("SWAT help is current");
                }
            }
                 */ 
        }
        /// <summary>
        /// Copies temporary help files to help files used by the dynamic help.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void copyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //copy all html files starting with "_swat", use the same original name but remove the "Temp" at the end
            DirectoryInfo dir = new DirectoryInfo(this.helpFolderPath);

            //get only temp help files
            FileInfo[] files = dir.GetFiles(SWATHelpConstants.SWAT_TEMP_HELP_FILE_PREFIX+"*"+SWATHelpConstants.SWAT_HELP_FILE_EXTENSION);

            try
            {
                foreach (FileInfo file in files)
                {
                    File.Copy(file.FullName, this.renameTempFile(file.FullName));
                }
            }
            /*
            catch (IOException eio)
            {
                //occurs when a file is overriden, ignore
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message+ "\nError code: 8");
            }
            */
            catch
            {
            }
        }
        #endregion


    }
}
