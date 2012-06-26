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
using System.Xml;
using System.Configuration;
using SWAT.Configuration.Normalization;
using SWAT.Configuration;
using Microsoft.Win32;

namespace SWAT
{
    [UserSetting]
    public static class SafariSettings
    {
        public static int MacResponseTimeout
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("MacResponseTimeout")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("MacResponseTimeout"));
                }
                else
                {
                    return 60;
                }
            }
            set
            {
                if (value > 0)
                {
                    UserConfigHandler.SetUserSetting("MacResponseTimeout", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else
                    UserConfigHandler.LastSettingSuccessful = false;
            }

        }

        public static int SafariPort
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("SafariPort")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("SafariPort"));
                }
                else
                {
                    return 9997;
                }
            }
            set
            {
                if (value > 0)
                {
                    UserConfigHandler.SetUserSetting("SafariPort", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else
                    UserConfigHandler.LastSettingSuccessful = false;
            }

        }

        public static string SafariAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("SafariAddress")))
                {
                    return UserConfigHandler.GetUserSetting("SafariAddress");
                }
                else
                {
                    return "120.0.0.1";
                }
            }
            set
            {
                UserConfigHandler.SetUserSetting("SafariAddress", value.ToString());
                UserConfigHandler.LastSettingSuccessful = true;
            }
        }

    }

    [UserSetting]
    public static class BrowserPaths
    {
        public static string FirefoxRootDirectory
        {
            get
            {
                string userSetting = UserConfigHandler.GetUserSetting("FirefoxRootDirectory");
                if (!string.IsNullOrEmpty(userSetting))
                {
                    if (userSetting.EndsWith(@"\"))
                        return userSetting + @"firefox.exe";

                    if (!userSetting.Contains(@"\firefox.exe"))
                        return userSetting + @"\firefox.exe";
                    else
                        return userSetting;
                }
                else
                {
                    return GetFireFoxExecutablePath();
                }
            }
            set
            {
                string newPath = value;

                if (!string.IsNullOrEmpty(newPath))
                {
                    UserConfigHandler.SetUserSetting("FirefoxRootDirectory", newPath);
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else                
                    UserConfigHandler.LastSettingSuccessful = false;                
            }
        }

        private static string GetFireFoxExecutablePath()
        {
            RegistryKey key = Registry.LocalMachine;
            string path = "";
            key = key.OpenSubKey(@"Software\Mozilla\Mozilla Firefox", true);

            if (key == null)//check to see if Firefox is located in the 64 bit Registry location
            {
                RegistryKey key64bit = Registry.LocalMachine;
                key64bit = key64bit.OpenSubKey(@"Software\Wow6432Node\Mozilla\Mozilla Firefox", true);
                key = key64bit;
            }

            if (key != null)
            {
                string[] arr = key.GetSubKeyNames();
                string versionFolder = arr[arr.Length - 1];

                key = key.OpenSubKey(versionFolder + @"\Main", true);

                path = (string)key.GetValue("PathToExe");
            }

            return path;
        }
    }

    [UserSetting]
    public static class DefaultTimeouts
    {
        public static int WaitForDocumentLoadTimeout
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("WaitForDocumentLoadTimeout")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("WaitForDocumentLoadTimeout"));
                }
                else
                {
                    return 300;
                }
            }
            set
            {
                int newValue = value;

                if (newValue > 0)
                {
                    if (newValue < 30)
                    {
                        newValue = 30;
                        UserConfigHandler.SetUserSetting("WaitForDocumentLoadTimeout", newValue.ToString());
                    }
                    else if (newValue >= 30)
                        UserConfigHandler.SetUserSetting("WaitForDocumentLoadTimeout", value.ToString());
                    
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else                
                    UserConfigHandler.LastSettingSuccessful = false;
            }
        }

        public static int FindElementTimeout
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("FindElementTimeout")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("FindElementTimeout"));
                }
                else
                {
                    return 15;
                }
            }
            set
            {
                if (value > 0)
                {
                    UserConfigHandler.SetUserSetting("FindElementTimeout", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else                
                    UserConfigHandler.LastSettingSuccessful = false;                
            }
        }

        public static int DoesElementExistTimeout
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("DoesElementExistTimeOut")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("DoesElementExistTimeOut"));
                }
                else
                {
                    return 15;
                }
            }
            set
            {
                if (value > 0)
                {
                    UserConfigHandler.SetUserSetting("DoesElementExistTimeOut", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else
                    UserConfigHandler.LastSettingSuccessful = false;
            }
        }

        public static int DoesElementNotExistTimeout
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("DoesElementNotExistTimeOut")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("DoesElementNotExistTimeOut"));
                }
                else
                {
                    return 15;
                }
            }
            set
            {
                if (value > 0)
                {
                    UserConfigHandler.SetUserSetting("DoesElementNotExistTimeOut", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else               
                    UserConfigHandler.LastSettingSuccessful = false;                
            }
        }

        public static int DoesElementNotExistLookTimeout
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("DoesElementNotExistLookTimeout")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("DoesElementNotExistLookTimeout"));
                }
                else
                {
                    return 5;
                }
            }
            set
            {
                if (value > 0)
                {
                    UserConfigHandler.SetUserSetting("DoesElementNotExistLookTimeout", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else                
                    UserConfigHandler.LastSettingSuccessful = false;                
            }
        }

        public static int AttachToWindowBrowserTimeout
        {
            get
            {

                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("AttachToWindowBrowserTimeout")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("AttachToWindowBrowserTimeout"));
                }
                else
                {
                    return 50;
                }
            }
            set
            {
                if (value > 0)
                {
                    UserConfigHandler.SetUserSetting("AttachToWindowBrowserTimeout", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else
                    UserConfigHandler.LastSettingSuccessful = false;
            }
        }

        public static int WaitForBrowserTimeout
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("WaitForBrowserTimeout")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("WaitForBrowserTimeout"));
                }
                else
                {
                    return 300;
                }
            }
            set
            {
                if (value > 0)
                {
                    UserConfigHandler.SetUserSetting("WaitForBrowserTimeout", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else
                    UserConfigHandler.LastSettingSuccessful = false;
            }
        }

      public static int AssertBrowserExists
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("AssertBrowserExists")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("AssertBrowserExists"));
                }
                else
                {
                    return 10;
                }
            }
            set
            {
                if (value > 0)
                {
                    UserConfigHandler.SetUserSetting("AssertBrowserExists", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else
                    UserConfigHandler.LastSettingSuccessful = false;
            }
        }
    }

    [UserSetting]
    public static class WantInformativeExceptions
    {
        public static bool GetInformativeExceptions
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("GetInformativeExceptions")))
                {
                    return bool.Parse(UserConfigHandler.GetUserSetting("GetInformativeExceptions"));
                }
                else
                {
                    return false;
                }
            }
            set
            {
                UserConfigHandler.SetUserSetting("GetInformativeExceptions", value.ToString());
                UserConfigHandler.LastSettingSuccessful = true;
            }
        }
    }

    [UserSetting]
    public static class IESettings
    {
        public static bool HighlightElementsAsTestsRun
        {
            get
            {   
                if(!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("HighlightElementsAsTestsRun")))
                    return bool.Parse(UserConfigHandler.GetUserSetting("HighlightElementsAsTestsRun"));

                return false;
            }

            set
            {
                UserConfigHandler.SetUserSetting("HighlightElementsAsTestsRun", value.ToString().ToLower());
                UserConfigHandler.LastSettingSuccessful = true;              
            }
        }
    }

    [UserSetting]
    public static class WantDelayBetweenCommands
    {
        public static int DelayBetweenCommands
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("DelayBetweenCommands")))
                {
                    return int.Parse(UserConfigHandler.GetUserSetting("DelayBetweenCommands"));
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value >= 0)
                {
                    UserConfigHandler.SetUserSetting("DelayBetweenCommands", value.ToString());
                    UserConfigHandler.LastSettingSuccessful = true;
                }
                else
                {
                    UserConfigHandler.LastSettingSuccessful = false;
                }
            }
        }
    }

    [UserSetting]
    public static class WantCloseBrowsersBeforeTestStart
    {
        public static bool CloseBrowsersBeforeTestStart
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("CloseBrowsersBeforeTestStart")))
                {
                    return bool.Parse(UserConfigHandler.GetUserSetting("CloseBrowsersBeforeTestStart"));
                }
                else
                {
                    return false;
                }
            }
            set
            {
                UserConfigHandler.SetUserSetting("CloseBrowsersBeforeTestStart", value.ToString());
                UserConfigHandler.LastSettingSuccessful = true;
            }
        }
    }

    [UserSetting]
    public static class WantSuspendOnFail
    {
        public static bool SuspendTestOnFail
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("SuspendTestOnFail")))
                {
                    return bool.Parse(UserConfigHandler.GetUserSetting("SuspendTestOnFail"));
                }
                else
                {
                    return false;
                }
            }
            set
            {
                UserConfigHandler.SetUserSetting("SuspendTestOnFail", value.ToString());
                UserConfigHandler.LastSettingSuccessful = true;
            }
        }
    }

    [UserSetting]
    public static class FitnesseSettings
    {
        public static string FitnesseRootDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("FitnesseRootDirectory")))
                {
                    return UserConfigHandler.GetUserSetting("FitnesseRootDirectory");
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                UserConfigHandler.SetUserSetting("FitnesseRootDirectory", value.ToString());
                UserConfigHandler.LastSettingSuccessful = true;
            }
        }
    }

    [UserSetting]
    public static class ScreenShotSettings
    {
        public static bool SnapShotOption
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("SnapShotOption")))
                {
                    return bool.Parse(UserConfigHandler.GetUserSetting("SnapShotOption"));
                }
                else
                {
                    return false;
                }

            }
            set
            {
                UserConfigHandler.SetUserSetting("SnapShotOption", value.ToString());
                UserConfigHandler.LastSettingSuccessful = true;
            }
        }

        public static string SnapShotFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("SnapShotFolder")))
                {
                    return UserConfigHandler.GetUserSetting("SnapShotFolder");
                }
                else
                {
                    return @"\\" + Environment.MachineName + @"\C$\";
                }
            }
            set
            {
                string dirPath = value.ToString();
                if (dirPath.Contains(@":"))
                {
                    string machineName = @"\\" + Environment.MachineName + @"\";
                    dirPath = dirPath.Replace(":", "$");
                    dirPath = machineName + dirPath;
                }
                
                UserConfigHandler.SetUserSetting("SnapShotFolder", dirPath);
                UserConfigHandler.LastSettingSuccessful = true;
            }
        }

        public static bool ScreenShotBrowser
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("ScreenShotBrowser")))
                {
                    return bool.Parse(UserConfigHandler.GetUserSetting("ScreenShotBrowser"));
                }
                else
                {
                    return false;
                }
            }
            set
            {
                UserConfigHandler.SetUserSetting("ScreenShotBrowser", value.ToString());
                UserConfigHandler.LastSettingSuccessful = true;
            }
        }

        public static bool ScreenShotAllScreens
        {
            get
            {
                if (!string.IsNullOrEmpty(UserConfigHandler.GetUserSetting("ScreenShotAllScreens")))
                {
                    return bool.Parse(UserConfigHandler.GetUserSetting("ScreenShotAllScreens"));
                }
                else
                {
                    return false;
                }
            }
            set
            {
                UserConfigHandler.SetUserSetting("ScreenShotAllScreens", value.ToString());
                UserConfigHandler.LastSettingSuccessful = true;
            }
        }

    }

    public static class UserConfigHandler
    {
        private static Dictionary<string, string> settings = new Dictionary<string, string>();
        private static bool isSettingSuccessful;

        private static ExeConfigurationFileMap AppConfigFilePath
        {
            get
            {

                string path = string.Concat(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", ""), ".config");
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = path;

                return map;
            }
        }

        private static string UserConfigFilePath
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().CodeBase)
                    .Replace(@"file:\", ""), "SWAT.user.config");
            }
        }

        internal static string GetUserSetting(string key)
        {
            if (ConfigurationManager.OpenMappedExeConfiguration(AppConfigFilePath, ConfigurationUserLevel.None).AppSettings.Settings[key] != null)
                return ConfigurationManager.OpenMappedExeConfiguration(AppConfigFilePath, ConfigurationUserLevel.None).AppSettings.Settings[key].Value;
            else
                return null;
        }

        internal static void SetUserSetting(string key, string value)
        {
            if (settings.ContainsKey(key))
            {
                settings[key] = value;
            }
            else
            {
                settings.Add(key, value);
            }
        }

        public static void Save()
        {
            XmlDocument config = new XmlDocument();

            if (System.IO.File.Exists(UserConfigFilePath))
                config.Load(UserConfigFilePath);
            else
            {
                throw new UserConfigFileDoesNotExistException("Unable to save settings SWAT.user.config file is missing");
            }

            foreach (string key in settings.Keys)
            {
                SaveUserSettings(key, settings[key], ref config);
            }


            config.PreserveWhitespace = true;
            XmlTextWriter wrtr = new XmlTextWriter(UserConfigFilePath, Encoding.UTF8);
            config.Save(wrtr);
            wrtr.Close();
            ConfigurationManager.RefreshSection("appSettings");

            settings = new Dictionary<string, string>();
        }

        private static void SaveUserSettings(string key, string value, ref XmlDocument config)
        {

            bool matchFound = false;
            XmlNodeList nodeList = config.SelectNodes("//add");

            foreach (XmlNode node in nodeList)
            {
                if (node.Attributes[0].Value.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    node.Attributes[1].Value = value;
                    matchFound = true;
                    break;
                }
            }

            if (!matchFound)
            {
                //settings were not found so we insert a new node into the user.config page with the new settings
                XmlNode appSettings = config.SelectSingleNode("appSettings");
                XmlNode newSetting = config.CreateNode(XmlNodeType.Element, "add", null);
                XmlAttribute attrKey = config.CreateAttribute("key");
                attrKey.Value = key;
                XmlAttribute attrValue = config.CreateAttribute("value");
                attrValue.Value = value;
                newSetting.Attributes.Append(attrKey);
                newSetting.Attributes.Append(attrValue);
                appSettings.AppendChild(newSetting);
            }
        }        

        //Designates if the last attempt to update a UserSetting was successful
        public static bool LastSettingSuccessful
        {
            get
            {
                return isSettingSuccessful;
            }
            set
            {
                isSettingSuccessful = value;
            }
        }
    }    

    //Attribute used to mark classes that contain user setting properties
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UserSettingAttribute : Attribute
    {           
    }
}
