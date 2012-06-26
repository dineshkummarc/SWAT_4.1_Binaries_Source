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

namespace SWAT
{
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException(string elementIdentifier, IdentifierType identType, string tagName)
            : base(string.Format("Unable to find element with {0} '{1}' and tag '{2}'", Enum.GetName(typeof(IdentifierType), identType), elementIdentifier, tagName))
        {

        }
        public ElementNotFoundException(string elementIdentifier, IdentifierType identType)
            : base(string.Format("Unable to find element with {0} '{1}'", Enum.GetName(typeof(IdentifierType), identType), elementIdentifier))
        {

        }
    }

    public class ElementNotActiveException : Exception
    {
        public ElementNotActiveException(string elementIdentifier, IdentifierType identType, string tagName)
            : base(string.Format("Element with {0} '{1}' and tag '{2}' is not active.", Enum.GetName(typeof(IdentifierType), identType), elementIdentifier, tagName))
        {

        }
        public ElementNotActiveException(string elementIdentifier, IdentifierType identType)
            : base(string.Format("Element with {0} '{1}' is not active.", Enum.GetName(typeof(IdentifierType), identType), elementIdentifier))
        {

        }
    }

    public class ComparisonFailedException : Exception
    {
        public ComparisonFailedException(String message)
            : base(message)
        { }
    }
    public class WindowNotFoundException : Exception
    {
        public WindowNotFoundException(string windowTitle)
            : base(string.Format("Unable to find window with title containing \"{0}\"", windowTitle))
        {

        }
    }

    public class IncorrectBrowserException : Exception
    {
        public IncorrectBrowserException(string message)
            : base(message)
        {
        }
    }

    public class AssertionFailedException : Exception
    {
        public AssertionFailedException(string message)
            : base(message)
        { }
    }


    public class InvalidEventException : Exception
    {
        public InvalidEventException(string theEvent)
            : base(string.Format("\"{0}\" is an invalid event name.", theEvent))
        { }
    }

    public class StimulateElementException : Exception
    {
        public StimulateElementException(string identifier, string eventName)
            : base(string.Format("Could not fire {1} on element with {0}", identifier, eventName))
        { }
    }

    public class FireFoxClientIsNotConnectedException : Exception
    {
        public FireFoxClientIsNotConnectedException(string message)
            : base(message)
        { }
    }

    public class ChromeContentScriptIsNotConnectedException : Exception
    {
        public ChromeContentScriptIsNotConnectedException()
            : base("The command could not be executed because the content script was disconnected")
        { }
    }

    public class UserConfigFileDoesNotExistException : Exception
    {
        public UserConfigFileDoesNotExistException(string message)
            : base(message)
        { }
    }

    public class BrowserDidNotLoadException : Exception
    {
        public BrowserDidNotLoadException(string message)
            : base(message)
        { }
    }

    public class BrowserExistException : Exception
    {
        public BrowserExistException(string message)
            : base(message)
        { }
    }

    public class BrowserDocumentNotHtmlException : Exception
    {
        public BrowserDocumentNotHtmlException(string message)
            : base(message)
        { }
    }
    public class LanguageNotImplementedException : Exception
    {
        public LanguageNotImplementedException(string message)
            : base(message)
        { }
    }

    public class RunScriptRuntimeException : Exception
    {
        public RunScriptRuntimeException(string message)
            : base(message)
        { }
    }

    public class RunScriptCompilerException : Exception
    {
        public RunScriptCompilerException(string message)
            : base(message)
        { }
    }

    public class NavigationTimeoutException : Exception
    {
        public NavigationTimeoutException(string message)
            : base(message)
        { }
    }

    public class BrowserNotInstalledException : Exception
    {
        public BrowserNotInstalledException(string message)
            : base(message)
        { }
    }

    public class IllegalDirectoryException : Exception
    {
        public IllegalDirectoryException(string message)
            : base(message)
        { }
    }

    public class ChromeException : Exception
    {
        public ChromeException(string message)
            : base(message)
        { }
    }

    public class ClickJSDialogException : Exception
    {
        public ClickJSDialogException()
            : base("Failed to click JSDialog")
        { }
    }

    public class DialogNotFoundException : Exception
    {
        public DialogNotFoundException()
            : base("There is no javascript dialog open")
        { }
    }

    public class WindowHandleNotFoundException : Exception
    {
        public WindowHandleNotFoundException(string message)
            : base(message)
        { }
    }

    public class AttributeErrorException : Exception
    {
        public AttributeErrorException(string message)
            : base(message)
        { }
    }

    public class ConfigurationItemException : Exception
    {
        public ConfigurationItemException(string message)
            :base(message)
            { }
    }

    public class NoAttachedWindowException : Exception
    {
        public NoAttachedWindowException()
            : base("Not attached to a browser window. Can't execute the command.")
        { }
    }

    public class TimerDoesNotExistException : Exception
    {
        public TimerDoesNotExistException(string message) : base(message) { }
    }

    public class SWATVariableDoesNotExistException : Exception
    {
        public SWATVariableDoesNotExistException(string varName) : base(string.Format("The variable {0} does not exist.", varName)) { }
    }

    public class ChromeExtensionNotConnectedException : Exception
    {
        public ChromeExtensionNotConnectedException() : base("Failed to connect to the Google Chrome SWAT extension. Please make sure it is installed and enabled.") { }
    }

    public class ChromeCommandTimedOutException : Exception
    {
        public ChromeCommandTimedOutException(string command, int timeout) : base(string.Format("The command {0} timed out after {1} seconds.", command, timeout)) { }
    }

    public class LockedDesktopEnvironmentException : Exception
    {
        public LockedDesktopEnvironmentException() : base("The key code sequence cannot be processed in a locked desktop environment.") { }
    }

    public class PressKeysFailureException : Exception
    {
        public PressKeysFailureException() : base("PressKeys failed to type any characters.") { }
    }

    public class NoAvailablePortException : Exception
    {
        public NoAvailablePortException() : base("Failed to find an available port. All ports are either blocked or currently in use.") { }
    }

    public class TopWindowMismatchException : Exception
    {
        public TopWindowMismatchException(string windowTitle) : base("\"" + windowTitle + "\" is not the current top window.") { }
        public TopWindowMismatchException(string windowTitle, string actualTop) : base("\"" + windowTitle + "\" is not the current top window. The actual top window is: \"" + actualTop + "\"") { }
        public TopWindowMismatchException(string windowTitle, int index) : base("Window title \"" + windowTitle + "\" with index " + index + " is not the current top window.") { }
        public TopWindowMismatchException(string windowTitle, int index, string actualTop) : base("Window title \"" + windowTitle + "\" with index " + index + " is not the current top window. The actual top window is: \"" + actualTop + "\"") { }
    }

    public class NonBrowserWindowExistException : Exception
    {
        public NonBrowserWindowExistException(string windowTitle) : base(string.Format("Window with title : {0} was not found", windowTitle)) { }
        public NonBrowserWindowExistException(string windowTitle, int index) : base(string.Format("Window with title : {0} at index {1} was not found", windowTitle, index)) { }
    }
}
