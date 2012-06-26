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



namespace SWAT
{
    [SWAT.NCover.CoverageExclude]
    public enum StatusCode
    {
        ATTRIBUTEERROR = 34,				// Failed to get or set an element's attribute
        BADCOMMAND = 9, 					// Command does not exist
        BADJAVASCRIPT = 17,
        BROWSEREXISTS = 66,                // AsssertBrowserDoesNotExist error
        CONNECTED = 100, 					// Connection to content script successful
        CONTENTSCRIPTCONNECTFAIL = 408,	// Failed to connect to the content script
        CONTENTSCRIPTERROR = 2,			// Content script encountered an error
        CONTENTSCRIPTDISCONNECTED = 409,    // The content script disconnected while executing a command
        ELEMENTDOESNOTEXIST = 7,			// Failed to find specified element
        ELEMENTNOTACTIVE = 82,
        ELEMENTNOTVISIBLE = 11,
        INVALIDELEMENTSTATE = 12,
        LOADING = 33,                      // Browser status is currently loading
        NAVIGATEFAIL = 404,				// Failed to navigate page
        NOATTACHEDWINDOW = 14,             // No SWAT.activePort is set in the background.js
        NOSUCHFRAME = 8,
        NOSUCHWINDOW = 3,   				// Failed to find a window
        PORTDISCONNECTED = 92,
        STALEELEMENTREFERENCE = 10,
        SUCCESS = 0,						// Command successfully called
        TOPWINDOWMISMATCH = 867,
        UNHANDLEDERROR = 13,
        UNDEFINEDTITLE = 15,				// Tried to get an undefined document title
        UNDEFINEDURL = 19,                 // Tried to get an undefined tab location (URL)
        UNSUPPORTEDCOMMAND = -1,			// Unsupported command detected
        UNSUPPORTEDEVENT = 4,              // Unsupported event detected
        WINDOWINDEXOUTOFBOUNDS = 5		    // Attach to window by index out of bounds        
    }
}
