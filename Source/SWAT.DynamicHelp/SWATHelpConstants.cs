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

namespace SWAT.DynamicHelp
{
    class SWATHelpConstants
    {
        public const String SRCFRG_BASE = @"http://ulti-swat.wiki.sourceforge.net/";
        public const String ALLCMD = "AllSwatCommands";
        //public const String OPEN_BROWSER = "SwatCommandsOpenBrowser";
        /// <summary>
        /// Folder to be created under AppData for swat related data.
        /// </summary>
        public const String SWAT_FOLDER_NAME = "SWAT";
        /// <summary>
        /// Folder to store html files pulled from sourceforge.
        /// </summary>
        public const String SWAT_HELP_FOLDER_NAME = "Help";
        /// <summary>
        /// Prefix for help files used by a Form.
        /// </summary>
        public const String SWAT_HELP_FILE_PREFIX = "_swat";
        public const String SWAT_HELP_FILE_EXTENSION = "html";
        /// <summary>
        /// Prefix for help files downloaded from sourceforge.
        /// </summary>
        public const String SWAT_TEMP_HELP_FILE_PREFIX = "_swatTemp";
        /// <summary>
        /// Html prefixing swat command links in sourceforge wiki help.
        /// </summary>
        public const String SWAT_WIKI_CMD_LINK_PREFIX = "/SwatCommands";
        public const int SWAT_HELP_RETRY_MAX_COUNT = 3;
        public const char SWAT_EDITOR_DELIMITER = '|';
    }
}
