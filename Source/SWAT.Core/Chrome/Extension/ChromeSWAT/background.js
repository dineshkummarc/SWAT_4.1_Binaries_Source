/** @namespace */
SWAT = {};

/**
* Array of all information about currently loaded tabs (where a SWAT
* window is probably a tab)
* Entries of form:
* {Int tabId, String windowName, Port mainPort, Boolean isFrameset, FrameData[] frames}
* FrameData ::= {[Int frameId], String frameName, Port framePort, FrameData[]}
* frameId can be undefined, if it has not yet been looked up, but should be
* added once it is known
* @type {Array.<Object>}  TODO: Type info
*/
SWAT.tabs = [];


/**
* Port to the currently active frame (or tab, if the current page is not a
* frameset).
* @type {?Port}
*/
SWAT.activePort = null;


/**
* ID of the currently active tab.
* @type {?string}
*/
SWAT.activeTabId = null;

/**
* ID of the last disconnected tab.
* @type {?string}
*/
SWAT.lastDisconnectedTabId = null;


/**
* Place to temporarily store the URL currently being loaded, so that we can
* retry if needed, because opening a URL is an async callback.
* @type {?string}
*/
SWAT.urlBeingLoaded = null;


/**
* URL we believe we're currently on.
* @type {?string}
*/
SWAT.currentUrl = null;


/**
* Whether we are loading a new URL that difers from the current URL only in
* fragment.
* @type {boolean}
*/
SWAT.isGettingUrlButOnlyChangingByFragment = false;

/**
* Whether we are currently executing a OpenBrowser, and
* accordingly should send a success response when the tab is opened.
* @type {boolean}
*/
SWAT.isOpeningTab = false;

/**
* Whether we are currently executing a CloseBrowser, and
* accordingly should send a success response when the tab closes.
* @type {boolean}
*/
SWAT.isClosingTab = false;


/**
* Whether we believe a page is open to which we have no content script.
* @type {boolean}
*/
SWAT.hasNoConnectionToPage = true;


/**
* Stores the remaining frames to traverse when switching frames.
* @type {Array.<string>}
*/
SWAT.restOfCurrentFramePath = [];  //TODO Remove!


/**
* The last request we sent that has not been answered, so that if we change
* page between sending a request and receiving a response, we can re-send it to
* the newly loaded page.
* @type {*}
*/
SWAT.lastRequestToBeSentWhichHasntBeenAnsweredYet = null;


/**
* The last XMLHttpRequest we made (used for communication with test language
* bindings).
* @type {?XMLHttpRequest}
*/
SWAT.xmlHttpRequest = null;

/**
* URL to ping for commands.
* @type {string}
*/
SWAT.xmlHttpRequestUrl = null;

/**
* Prefix prepended to the hopefully unique javascript window name, in hopes of
* further removing conflict.
* @type {string}
*/
SWAT.windowHandlePrefix = 'SWAT_windowhandle';

/**
* Whether we will not execute any commands because we are already executing
* one.
* @type {boolean}
*/
SWAT.isBlockedWaitingForResponse = false;

/**
* Current waiting time for navigate browser
* @type {number} unit: milliseconds
*/
SWAT.navigateBrowserTime = 0;

/**
* Maximum time allowed to wait for navigate browser
* @type {number} unit: milliseconds
*/
SWAT.navigateBrowserTimeout = 300000;

/**
* Time interval between checking whether the page has loaded
* @type {number} unit: milliseconds
*/
SWAT.navigateBrowserTimeIncrement = 100;

/**
* Current waiting time for navigate browser
* @type {number} unit: milliseconds
*/
SWAT.refreshBrowserTime = 0;

/**
* Maximum time allowed to wait for refresh browser
* @type {number} unit: milliseconds
*/
SWAT.refreshBrowserTimeout = 60000;

/**
* Time interval between checking whether the page has loaded
* @type {number} unit: milliseconds
*/
SWAT.refreshBrowserTimeIncrement = 1000;

/**
* Used in RefreshBrowser command.
* When the background page executes the reload script into the tab
* we set this variable to true.
* The Tab will disconnect from the port because of the reload
* the background will not check the state of the page as long as the tab is still disconnected
* When the tab connects again we set this variable to false 
* and the background will check for page load completion.
* @type {bool}
*/
SWAT.waitingOnTabReConnect = false;

/**
* It's possible that the page has completed loading,
* but the content script has not yet fired.
* In this case, to not report that there is no page,
* when we are just too fast, we wait up to this amount of time.
* @type {number} unit: milliseconds.
*/
SWAT.timeoutUntilGiveUpOnContentScriptLoading = 5000;

/**
* How long we are currently waiting for the content script to load
* after loading the page
* @type {number} unit: milliseconds
*/
SWAT.currentlyWaitingUntilGiveUpOnContentScriptLoading;

/**
* How long we wait between poling whether we have a content script,
* when loading a new page, up until
* SWAT.timeoutUntilGiveUpOnContentScriptLoading
* @type {number} unit: milliseconds
*/
SWAT.waitForContentScriptIncrement = 100;

/**
* Used when KillAllOpenBrowsers is called to signal a response 
* should be sent back to SWAT
* @type (boolean)
*/
SWAT.isKillAllOpenBrowsers = false;

SWAT.currentServerConnectionTabId = null;

SWAT.tabWasDisconnected = false;

/**
* Title of the HTTP SWAT server tab
* @type {string}
*/
SWAT.HTTPServerTitle = "SWAT HTTP Server";

SWAT.doConnectParam = "?doConnect=true";

SWAT.ChromeTabUrlPrefix = "chrome://";

SWAT.ChromeTabUrlExtensionPrefix = "chrome-extension://";

SWAT.reconnectingTabId = null;

SWAT.changePort = false;

SWAT.currentRequest = null;

/**
 * Timeout for checking for onBeforeUnload
 */
SWAT.checkKillOnBeforeUnloadTimeout = 10000;

/**
 * Increment for the above timeout
 */
SWAT.checkKillOnBeforeUnloadTimeIncrement = 50; 

/**
 * Current time spent checking if onBeforeUnload
 */
SWAT.checkOnBeforeUnloadTime = 0;   

/**
 * Timeout for checking if onBeforeUnload
 */
SWAT.checkOnBeforeUnloadTimeout = 10000;

/**
 * Timeout value for checking if onBeforeUnload
 */
SWAT.checkOnBeforeUnloadTimeIncrement = 100;

/**
 * Used for FindBrowser
 */
SWAT.expectPositiveResult = false;

/**
 * Used for FindBrowser (AssertBrowserExists, AssertBrowserDoesNotExist, AttachToWindow)
 */
SWAT.foundTabIds = [];

/**
 * Used in kill all open browsers
 */
SWAT.tabCount = 0;

/**
 * Dictionary to hold all tabs that are currently open beofre killAllOpenBrowsers
 * Uses tab.id as key and boolean as value to see if the tab was killed or not
 */
SWAT.tabsToKill = new Dictionary();

/**
 * Dictionary to hold the onBeforeUnload timeout values for each tab in SWAT.tabsToKill
 */
SWAT.tabsToKillTimeouts = new Dictionary();

SWAT.windowTitle;

SWAT.windowIndex;

SWAT.doneAttaching;

SWAT.targetTabId;

SWAT.portQueue = [];

SWAT.OnBeforeUnloadCancel = false;

//Used to restore tab information in the event of an OnBeforeUnload-Cancel
SWAT.backUp = new Object;

SWAT.requestState = { None: 0, Sent: 1, Received: 2 };
SWAT.contentScriptRequestState = SWAT.requestState.None;

SWAT.onRequest = function(request, sender, sendResponse) {
    console.log("onRequest handled request: " + request.action.toString());
    sendResponse({ action: 'started' });
};

chrome.extension.onRequest.addListener(SWAT.onRequest);

chrome.extension.onConnect.addListener(
	function (port) {
	    try {
	        //SWAT.logTabInformation(port.tab);

	        if (port.name == SWAT.HTTPServerTitle) {

	            if (SWAT.currentServerConnectionTabId != null) {
	                chrome.tabs.remove(SWAT.currentServerConnectionTabId);
	            }

	            SWAT.currentServerConnectionTabId = port.tab.id;

	            //This is the first content script, so is from the URL we need to connect to
	            SWAT.xmlHttpRequestUrl = port.tab.url.replace(SWAT.doConnectParam, '');

	            //Tell the SWAT that we are here
	            var comm = new SWAT.SWATCommunication();
	            comm.sendResponseByXHR("", false);

	            //chrome.tabs.remove(port.tab.id);
	            return;
	        }
	        // else if (port.tab.url.indexOf(SWAT.xmlHttpRequestUrl) == 0)
	        // {
	        // //We have reloaded the xmlHttpRequest page.  Ignore the connection.
	        // return;
	        // } 
	        // @TEMPORARY commented this out so we can see code execute and behave as if a webpage 
	        // was loaded in the browser which doesn't have the same url as SWAT.xmlHttpRequestUrl

	        //SWAT.tabWasDisconnected = false;
	        console.log("Tab " + port.tab.id + " connected on port name '" + port.name + "'");

	        SWAT.hasNoConnectionToPage = false;
	        var foundTab = false;

	        // look for the tab which matches the tab id in the port
	        for (var tab in SWAT.tabs) {
	            //SWAT.logTabInformation(tab);

	            if (SWAT.tabs[tab].tabId == port.tab.id) {
	                //console.log("SWAT.tabs[tab].tabId == port.tab.id == " + port.tab.id);
	                //We must be a new [i]frame in the page, because when a page closes, it is
	                // removed from SWAT.tabs
	                //TODO(danielwh): Work out WHICH page it's a sub-frame of (I don't look
	                // forward to this)
	                SWAT.tabs[tab].frames.push(
						{
						    frameName: port.name,
						    framePort: port,
						    frames: []
						}
					);
	                console.log("found an existing tab at " + SWAT.tabs[tab].tabId);
	                foundTab = true;
	                break;
	            }
	        }

	        if (!foundTab) {
	            //New tab!
	            //We don't know if it's a frameset yet, so we leave that as undefined
	            SWAT.tabs.push(
					{
					    tabId: port.tab.id,
					    windowName: SWAT.windowHandlePrefix + "_" + port.tab.id,
					    mainPort: port,
					    frames: []
					}
				);
	            console.log("found a new tab at " + port.tab.id);
	        }

	        if (SWAT.changePort || (SWAT.lastDisconnectedTabId == port.tab.id)) {
	            SWAT.changePort = false;
	            SWAT.activePort = port;
	            SWAT.setActiveTabDetails(port.tab);
	        }

	        if (SWAT.isOpeningTab) {
	            SWAT.isOpeningTab = false;
	            var message = "Opened tab " + port.tab.id;
	            console.log(message);
	            SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message }, false);
	        }

	        port.onMessage.addListener(SWAT.parsePortMessage);

	        port.onDisconnect.addListener(
				function disconnectPort(port) {
				    if (SWAT.currentRequest != null) {
				        if (SWAT.currentRequest.Command == "RefreshBrowser" || SWAT.currentRequest.Command == "NavigateBrowser")
				            SWAT.tabWasDisconnected = true;

				        if (SWAT.currentRequest.Command == "KillAllOpenBrowsers")
				            SWAT.tabsToKill.Add(port.tab.id, true);
				    }

				    SWAT.lastDisconnectedTabId = port.tab.id;
				    var remainingTabs = [];

				    for (var tab in SWAT.tabs) {
				        if (SWAT.tabs[tab].tabId == port.tab.id) {
				            if (SWAT.tabs[tab].mainPort == port) {
				                //This main tab is being closed.
				                //Don't include it in the new version of SWAT.tabs.
				                //Any subframes will also disconnect,
				                //but their tabId won't be present in the array,
				                //so they will be ignored.
				                
				                continue;
				            }
				            else {
				                //This is a subFrame being ditched
				                var remainingFrames = [];

				                for (var frame in SWAT.tabs[tab].frames) {
				                    if (SWAT.tabs[tab].frames[frame].framePort == port) {
				                        continue;
				                    }

				                    remainingFrames.push(SWAT.tabs[tab].frames[frame]);
				                }

				                SWAT.tabs[tab].frames = remainingFrames;
				            }
				        }

				        remainingTabs.push(SWAT.tabs[tab]);
				    }

				    SWAT.tabs = remainingTabs;

				    if ((SWAT.tabs.length == 0) || (SWAT.activePort == null) || (SWAT.activePort.tab.id == port.tab.id)) {
				        SWAT.resetCurrentlyWaitingOnContentScriptTime();
				    }

				    console.log("Tab " + port.tab.id + " disconnected from port name '" + port.name + "'");

				    // CloseBrowser was called
				    if (SWAT.isClosingTab && !SWAT.isKillAllOpenBrowsers) {
				        SWAT.isClosingTab = false;
				        var message = "Closed tab " + port.tab.id;
				        console.log(message);
				        SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message }, false);
				    }
				    else if (SWAT.isKillAllOpenBrowsers) { // KillAllOpenBrowsers was called
				        var cmds = new SWAT.backgroundCommands();
				        cmds.handleKillAllOpenBrowsers();
				    }
				    else if (SWAT.xmlHttpRequestUrl && SWAT.xmlHttpRequestUrl != '' && SWAT.contentScriptRequestState == SWAT.requestState.Sent) {
                        // content script disconnected while a command was executing
				        SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.CONTENTSCRIPTDISCONNECTED, Value: "The content script disconnected while executing a command." }, false);
				    }
				}
			);
	        chrome.tabs.onUpdated.addListener(
				function (tabId, info) {
				    if (info.status === 'complete') {
				        if (SWAT.reconnectingTabId == port.tab.id) {
				            SWAT.waitingOnTabReConnect = false;
				            SWAT.reconnectingTabId = null;
				        }
				    }
				});
	        chrome.tabs.onRemoved.addListener(
                function (tabId) {
                    if (SWAT.activeTabId == tabId)
                        SWAT.resetActiveTabDetails();
                });
	    }
	    catch (e) {
	        SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/onConnect.addListener.");
	    }
	}
);

/**
* Parse messages coming in on the port (responses from the content script).
* @param message JSON message of format:
*                {response: "some command",
*                 value: {StatusCode: STATUS_CODE
*                 [, optional params]}}
*/
	SWAT.parsePortMessage = function (message) {

	SWAT.contentScriptRequestState = SWAT.requestState.Received;
    try {
        var contentScriptMessage = JSON.stringify(message);
        //console.log("Received response [" + contentScriptMessage + "] from content script");
        var toSend;

        if (!SWATUtilities.isValid(message.request.Command)
		 || !SWATUtilities.isValid(message.response.StatusCode)
		 || !SWATUtilities.isValid(message.response.Value)
		 || message.response.StatusCode == SWATStatusCode.UNHANDLEDERROR) {

            // Should only ever happen if we sent a bad request,
            // or the content script is broken
            var errorMessage = "Received an invalid response or error from the content script";
            console.log(errorMessage);
            SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.CONTENTSCRIPTERROR, Value: errorMessage }, false);
        } else {

            SWAT.lastRequestToBeSentWhichHasntBeenAnsweredYet = null;

            toSend = { StatusCode: message.response.StatusCode, Value: null };
            if (message.response.Value !== undefined && message.response.Value != null) {
                toSend.Value = message.response.Value;
            }

            switch (message.request.Command) {

                case "RunJavaScript":
                    SWAT.sendResponseToParsedRequest(message.response, false);
                    break;
                case "GetElementAttribute":
                case "SetElementAttribute":
                case "StimulateElement":
                case "GetDocumentAttribute":
                case "SetDocumentAttribute":
                case "PressKeys":
                case "AssertElementIsActive":
					var CSComm = new SWAT.contentScriptCommunication();
                    CSComm.handlePortResponse(message);
                    break;

                case "newTabInformation": 
					var cmds = new SWAT.backgroundCommands();
					cmds.updateTabInformation(message); 
					break;
                default: SWAT.sendResponseToParsedRequest(toSend, message.wait); break;
            }
        }
    }
    catch (e) {
        SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/parsePortMessage.");
    }
}

/**
* Parses a request received from the SWAT and either sends the
* response, or sends a message to the content script with a command to execute
* @param request object encapsulating the request (e.g.
*     {Command: "NavigateBrowser", Arguments: {url:"http://www.google.com"}})
*/
SWAT.parseRequest = function (request) {

    SWAT.contentScriptRequestState = SWAT.requestState.None;
    try {
        if (SWAT.isBlockedWaitingForResponse) {
            console.log("Already sent a request which hasn't been replied to yet. Not parsing any more.");
            return;
        }

        SWAT.isBlockedWaitingForResponse = true;

        //turned off toggling the extension icon to
        //avoid giving users epilepsy
        //SWAT.setExtensionBusyIndicator(true);

        SWAT.currentRequest = request;
        var CSComm = new SWAT.contentScriptCommunication();
        var cmds = new SWAT.backgroundCommands();

        console.log("received command: " + request.Command);
        switch (request.Command) {
			case "GetNumberOfSWATTabs":
				cmds.GetNumberOfSWATTabs();
				break;
            case "AssertBrowserIsAttached":
                cmds.AssertBrowserIsAttached();
                break;
            case "CheckTabStatus":
                cmds.CheckTabStatus();
                break;
            case "OpenBrowser":
                SWAT.changePort = true;
                cmds.OpenBrowser();
                break;
            case "CloseBrowser": cmds.CloseBrowser(); break;
            case "KillAllOpenBrowsers": cmds.KillAllOpenBrowsers(request); break;
            case "NavigateBrowser": cmds.NavigateBrowser(request.Arguments.url); break;
            case "AttachToWindow":
                SWAT.changePort = true;
                cmds.AttachToWindow(request);
                break;
            case "GetWindowTitle": chrome.tabs.get(SWAT.activePort.tab.id, cmds.GetWindowTitle); break;
            case "GetLocation": chrome.tabs.get(SWAT.activePort.tab.id, cmds.GetLocation); break;
            case "FindBrowser": cmds.FindBrowser(request); break;
            case "RunJavaScript":
                cmds.processRunScriptRequest(request);
                break;
            case "GetElementAttribute":
            case "SetElementAttribute":
            case "StimulateElement":
            case "AssertElementExists":
            case "GetDocumentAttribute":
            case "SetDocumentAttribute":
            case "PressKeys":
            case "AssertElementIsActive":
                CSComm.processContentScriptRequest(request);
                break;
            case "RefreshBrowser":
                cmds.RefreshBrowser();
                break;
            case "AssertTopWindow":
				cmds.AssertTopWindow(request);
				break;
            default:
                CSComm.sendMessageOnActivePortAndAlsoKeepTrackOfIt({ request: request });
                break;
        }
    }
    catch (e) {
        SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/parseRequest.");
    }
}

SWAT.backgroundCommands = function () {

	this.AssertTopWindow = function (request) {
		var index = request.Arguments.index;
		var title = request.Arguments.title;
		var count = -1;

		for (var i = 0; i < SWAT.tabs.length && count < index; i++) {
			if (SWAT.tabs[i].mainPort.tab.title.toLowerCase().indexOf(title) != -1)
			{
				count++;
				if (count == index && SWAT.tabs[i].tabId == SWAT.activeTabId)
				{ 
					SWAT.sendResponseToParsedRequest({ StatusCode : SWATStatusCode.SUCCESS, Value : "Top window is correct" });
					return;
				}
			}
		}
		
		if (count < index) {
			SWAT.sendResponseToParsedRequest({ StatusCode : SWATStatusCode.WINDOWINDEXOUTOFBOUNDS, Value : count + 1});
			return;
		}
		SWAT.sendResponseToParsedRequest({ StatusCode : SWATStatusCode.TOPWINDOWMISMATCH, Value : "The top window is incorrect"});
	}

	this.GetNumberOfSWATTabs = function () {
		SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: "" + SWAT.tabs.length });
	}

    this.OpenBrowser = function () {
        chrome.tabs.create({ url: SWAT.xmlHttpRequestUrl, selected: true });
        SWAT.isOpeningTab = true;
    }

    this.CloseBrowser = function () {
        var message = "Closed tab " + SWAT.activeTabId;
        SWAT.tabWasDisconnected = false;
        SWAT.checkOnBeforeUnloadTime = 0;
        chrome.tabs.remove(SWAT.activeTabId, SWAT.checkIfOnBeforeUnload)
        SWAT.isClosingTab = true;
    }

    this.NavigateBrowser = function (url) {
        console.log("Navigating tab " + SWAT.activeTabId + " to " + url);
        var request = { request: { request: "NavigateBrowser", url: url} };
        var tempActiveTagId = SWAT.activeTabId;
        var scriptDoNavigate = "window.navigate(" + url + ");";

        if (url.indexOf("#") > -1 && SWAT.currentUrl != null &&
			SWAT.currentUrl.split("#")[0] == url.split("#")[0]) {
            SWAT.isGettingUrlButOnlyChangingByFragment = true;
        }
        else {
            SWAT.resetActiveTabDetails();
        }
        SWAT.currentUrl = url;
        SWAT.waitingOnTabReConnect = true;
        SWAT.reconnectingTabId = tempActiveTagId;
        SWAT.tabWasDisconnected = false;
        SWAT.checkOnBeforeUnloadTime = 0;

        if (tempActiveTagId == null) {
            chrome.tabs.create({ url: url, selected: true }, navigateBrowserCallback);
        }
        else {
            SWAT.activeTabId = tempActiveTagId;
            //chrome.tabs.executeScript(SWAT.activeTabId, { code: scriptDoNavigate }, navigateBrowserCallback);
            chrome.tabs.update(SWAT.activeTabId, { url: url, selected: true }, navigateBrowserCallback);
        }
    }

    function navigateBrowserCallback(tab) {
        try {
            if (chrome.extension.lastError) {
                // An error probably arose because Chrome didn't have a window yet
                // (see crbug.com 19846)
                // If we retry, we *should* be fine. Unless something really bad is
                // happening, in which case we will probably hang indefinitely trying to
                // reload the same URL
                NavigateBrowser(SWAT.urlBeingLoaded);
                return;
            }
            if (!SWATUtilities.isValid(tab)) {
                //chrome.tabs.update's callback doesn't pass a Tab argument,
                //so we need to populate it ourselves
                chrome.tabs.get(SWAT.activeTabId, navigateBrowserCallback);
                return;
            }
            if (SWAT.isGettingUrlButOnlyChangingByFragment) {
                SWAT.resetCurrentlyWaitingOnContentScriptTime();
                SWAT.sendResponseToParsedRequest({ statusCode: SWATStatusCode.SUCCESS }, false);
                SWAT.isGettingUrlButOnlyChangingByFragment = false;
            }

            if (SWAT.navigateBrowserTime == 0 && tab.title != "about:swat")
                SWAT.checkIfOnBeforeUnload();

            if (SWAT.waitingOnTabReConnect) {
                console.log("Waiting for tab " + tab.id + " to reconnect: " + SWAT.currentUrl);

                if (SWAT.navigateBrowserTime > SWAT.navigateBrowserTimeout) {
                    message = "Navigation timeout for URL " + SWAT.urlBeingLoaded + " at " + (SWAT.navigateBrowserTimeout / 1000) + " second(s).";
                    SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.NAVIGATEFAIL, Value: message }, false);
                }
                else {
                    SWAT.navigateBrowserTime += SWAT.navigateBrowserTimeIncrement;
                    var instance = new SWAT.backgroundCommands();
                    setTimeout(function () { instance.navigateBrowserCallbackById(tab.id) }, SWAT.navigateBrowserTimeIncrement);
                }
                return;
            }
            else if (tab.status != "complete") {
                // Use the helper calback so that we actually get updated version of the tab
                // we're getting
                console.log(tab.title + " is currently " + tab.status);

                if (SWAT.navigateBrowserTime > SWAT.navigateBrowserTimeout) {
                    console.log("Navigation has timed out");
                    message = "Navigation timeout for URL " + SWAT.urlBeingLoaded + " at " + (SWAT.navigateBrowserTimeout / 1000) + " second(s).";
                    SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.NAVIGATEFAIL, Value: message }, false);
                }
                else {
                    SWAT.navigateBrowserTime += SWAT.navigateBrowserTimeIncrement;
                    var instance = new SWAT.backgroundCommands();
                    setTimeout(function () { instance.navigateBrowserCallbackById(tab.id) }, SWAT.navigateBrowserTimeIncrement);
                }
                return;
            }
            else {
                if (SWAT.activePort == null) {
                    if (SWAT.currentlyWaitingUntilGiveUpOnContentScriptLoading <= 0) {
                        SWAT.hasNoConnectionToPage = true;
                        //sendEmptyResponseWhenTabIsLoaded(tab);
                    }
                    else {
                        SWAT.currentlyWaitingUntilGiveUpOnContentScriptLoading -=
							SWAT.waitForContentScriptIncrement;
                        console.log("Port not ready yet...");
                        var instance = new SWAT.backgroundCommands();
                        setTimeout(function () { instance.navigateBrowserCallbackById(tab.id) }, SWAT.waitForContentScriptIncrement);
                        return;
                    }
                }

				//Done navigating
                SWAT.navigateBrowserTime = 0;
                SWAT.waitingOnTabReConnect = false;
                SWAT.reconnectingTabId = null;
				var message = null;
                
				if (SWAT.OnBeforeUnloadCancel)
					message = "OnBeforeUnloadCancel scenario has finished";
				else
					message = "Navigated to " + tab.url;
				
				console.log(message);
				SWAT.setActiveTabDetails(tab);
				SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message }, false);
            }
            return;
        }
        catch (e) {
            SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/navigateBrowserCallback.");
        }
    }

    this.navigateBrowserCallbackById = function (tabId) {
        chrome.tabs.get(tabId, navigateBrowserCallback);
    }

    this.RefreshBrowser = function () {
        console.log("RefreshBrowser tab " + SWAT.activeTabId + " having url: " + SWAT.currentUrl);
        var scriptDoReload = "window.location.reload(true);";

        SWAT.waitingOnTabReConnect = true;
        SWAT.tabWasDisconnected = false;
        SWAT.checkOnBeforeUnloadTime = 0;
        SWAT.reconnectingTabId = SWAT.activeTabId;

        chrome.tabs.executeScript(SWAT.activeTabId, { code: scriptDoReload }, SWAT.refreshBrowserCallback);
    }

    this.KillAllOpenBrowsers = function (request) {
        SWAT.windowTitle = request.Arguments.windowTitle;
        SWAT.tabCount = 0;
        SWAT.tabsToKill.Clear;
        SWAT.tabsToKillTimeouts.Clear;

        if (SWATUtilities.isValid(SWAT.windowTitle)) {
            chrome.windows.getAll({ populate: true }, killWindowsExceptWindowTitle);
        }

        else {
            chrome.windows.getAll({ populate: true }, killWindows);
        }
        SWAT.isKillAllOpenBrowsers = true;
        var cmds = new SWAT.backgroundCommands();
        cmds.handleKillAllOpenBrowsers();
    }

    function killWindows(windows) {
        try {
            var devToolsUrl = "chrome://devtools/devtools.html";
            for (var windowIndex in windows) {

                for (var tabIndex in windows[windowIndex].tabs) {
                    var tabTitle = windows[windowIndex].tabs[tabIndex].title;
                    var tabId = windows[windowIndex].tabs[tabIndex].id;
                    var tabUrl = windows[windowIndex].tabs[tabIndex].url;

                    if (tabTitle != SWAT.HTTPServerTitle && !(tabUrl.indexOf(devToolsUrl) == 0)) {
                        console.log("Killing tab with ID: " + tabId + " title: " + tabTitle);
                        chrome.tabs.remove(windows[windowIndex].tabs[tabIndex].id);
                    }
                }
            }
        }
        catch (e) {
            SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/killWindows.");
        }
    }

    function killWindowsExceptWindowTitle(windows) {
        try {
            var devToolsUrl = "chrome://devtools/devtools.html";
            for (var windowIndex in windows) {
                for (var tabIndex in windows[windowIndex].tabs) {
                    var tabTitle = windows[windowIndex].tabs[tabIndex].title;
                    var tabId = windows[windowIndex].tabs[tabIndex].id;
                    var tabUrl = windows[windowIndex].tabs[tabIndex].url;

                    if (tabTitle != SWAT.HTTPServerTitle && !(tabUrl.indexOf(devToolsUrl) >= 0)) {
                        if (tabTitle.toLowerCase().indexOf(SWAT.windowTitle.toLowerCase()) >= 0) {
                            SWAT.tabCount++;
                        }
                        else {
                            console.log("Killing tab with ID: " + tabId + " title: " + tabTitle);
                            SWAT.tabsToKill.Add(tabId, false);
                            SWAT.tabsToKillTimeouts.Add(tabId, 0);
                            chrome.tabs.remove(tabId, SWAT.checkForOnBeforeUnload(tabId));
                        }
                    }
                }
            }
            var cmds = new SWAT.backgroundCommands();
            cmds.handleKillAllOpenBrowsers();
        }
        catch (e) {
            SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/killWindowsExceptWindowTitle.");
        }
    }

    this.handleKillAllOpenBrowsers = function () {
        if (SWAT.tabs.length == SWAT.tabCount && SWAT.isKillAllOpenBrowsers) {
            SWAT.isKillAllOpenBrowsers = false;
            var message = "Killed all open tabs";
            console.log(message);
            SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message }, false);
        }
    }

    this.updateTabInformation = function (message) {
        for (var tab in SWAT.tabs) {
            //RACE CONDITION!!!
            //This call should happen before another content script
            //connects and returns this value,
            //but if it doesn't, we may get mismatched information
            if (SWAT.tabs[tab].isFrameset === undefined) {
                SWAT.tabs[tab].isFrameset = message.response.Value.isFrameset;
                return;
            }
            else {
                for (var frame in SWAT.tabs[tab].frames) {
                    var theFrame = SWAT.tabs[tab].frames[frame];
                    if (theFrame.isFrameset === undefined) {
                        theFrame.isFrameset = message.response.Value.isFrameset;
                        return;
                    }
                }
            }
        }
    }

    this.AttachToWindow = function (request) {
        console.log("Attach to window reached");
        try {
            SWAT.doneAttaching = false;
            SWAT.windowTitle = request.Arguments.windowTitle.toLowerCase();
            SWAT.windowIndex = request.Arguments.windowIndex;
            console.log("Attaching to tab containing tab title '" + SWAT.windowTitle + "' and with index " + SWAT.windowIndex);

            if (SWAT.foundTabIds.length == 0) {
                console.log("found no tabs");
                SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.NOSUCHWINDOW, Value: SWAT.windowTitle }, false);
            }
            else {
                var tabId = SWAT.foundTabIds[SWAT.windowIndex];
                if (!SWATUtilities.isValid(tabId)) {
                    SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.WINDOWINDEXOUTOFBOUNDS, Value: (SWAT.foundTabIds.length) }, false);
                }
                else {
                    console.log("was able to attach to window");
                    AttachTo(tabId);
                }
            }
            SWAT.changePort = false;
        }
        catch (e) {
            SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/AttachToWindow.");
        }
    }

    function AttachTo(tabId) {
        console.log("attachto called");
        SWAT.targetTabId = tabId;
        for (i = 0; i < SWAT.tabs.length; i++) {
            var currentTabId = SWAT.tabs[i].tabId;
            if (currentTabId == tabId) {
                SWAT.activePort = SWAT.tabs[i].mainPort;
                chrome.tabs.get(tabId, AttachToWindowCallback);
                SWAT.doneAttaching = true;
                break;
            }
        }
        if (!SWAT.doneAttaching) {
            var cmds = new SWAT.backgroundCommands();
            setTimeout(function () { cmds.TryAttachCallback() }, 50);
        }
    }

    this.TryAttachCallback = function () {
        AttachTo(SWAT.targetTabId);
    }

    function AttachToWindowCallback(tab) {
        console.log("attachtowindowcallback called");
        SWAT.setActiveTabDetails(tab);
        chrome.tabs.update(tab.id, { selected: true });
        console.log("Attached to tab " + tab.id + " with title '" + tab.title + "'");
        SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: tab.title }, false);
    }

    this.FindBrowser = function (request) {
        SWAT.foundTabIds = [];
        SWAT.windowTitle = request.Arguments.windowTitle;
        SWAT.windowIndex = request.Arguments.windowIndex;
        SWAT.expectPositiveResult = request.Arguments.expectPositiveResult;
        chrome.windows.getAll({ populate: true }, FindBrowserCallback);
    }

    function FindBrowserCallback(windows) {
        var windowTitleLowerCase = SWAT.windowTitle.toLowerCase();
        try {
            for (windowIndex in windows) {
                var tabs = windows[windowIndex].tabs;
                for (tabIndex in tabs) {
                    var tab = tabs[tabIndex];
                    var title = tab.title.toLowerCase();
                    if (IsNormalTab(tab.title, tab.url) && title.indexOf(windowTitleLowerCase) >= 0) {
                        if (tab.status == "loading") {
                            SendFindBrowserResponse(false);
                            return;
                        }
                        SWAT.foundTabIds.push(tab.id);
                    }
                }
            }
            var foundTabCount = SWAT.foundTabIds.length;
            if (foundTabCount > 0 && (foundTabCount - 1) >= SWAT.windowIndex) {
                SendFindBrowserResponse(true);
            }
            else {
                SendFindBrowserResponse(false);
            }
        } catch (e) {
            SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/FindBrowserCallback.");
        }
    }

    function IsNormalTab(title, url) {
        return !(url.indexOf(SWAT.ChromeTabUrlPrefix) == 0 || url.indexOf(SWAT.ChromeTabUrlExtensionPrefix) == 0 || title.indexOf(SWAT.HTTPServerTitle) == 0);
    }

    function SendFindBrowserResponse(foundBrowser) {
        var message;

        if (foundBrowser) {
            message = 'There is a browser with title "' + SWAT.windowTitle + '" open.';
            if (SWAT.expectPositiveResult) //AssertBrowserExists
            {
                SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message }, false);
                return;
            }
            else { //AssertBrowserDoesNotExist
                SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.BROWSEREXISTS, Value: message }, false);
                return;
            }
        }
        else {
            message = 'There is no browser with title "' + SWAT.windowTitle + '" open.';
            if (SWAT.expectPositiveResult) { //AssertBrowserExists
                SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.BROWSEREXISTS, Value: message }, false);
                return;
            }
            else { //AssertBrowserDoesNotExist
                SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message }, false);
                return;
            }
        }
    }

    this.AssertBrowserIsAttached = function () {
        var message;
        if (SWATUtilities.isValid(SWAT.activeTabId)) {
            message = "ActiveTabId is " + SWAT.activeTabId;
            console.log(message);
            SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: SWAT.activeTabId }, false);
        }
        else {
            message = "ActiveTabId is null";
            console.log(message);
            SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.NOATTACHEDWINDOW, Value: message }, false);
        }
    }

    this.CheckTabStatus = function () {
        if (SWAT.activeTabId != null) {
            chrome.tabs.get(SWAT.activeTabId, statusOfTab);
        }
        else if (SWAT.activeTabId == null) {
            //continue with sending the next request
            var message = "ActiveTabId is null";
            console.log(message);
            SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message });
        }
    }

    function statusOfTab(tab) {
        var message;
        if (tab.status == "complete") {
            //send message that tab is complete and you can continue
            message = "Tab status is complete";
            console.log(message);
            SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message });
        }
        else if (tab.status == "loading") {
            //send message that tab is still loading and you need to check again
            message = "Tab status is loading";
            console.log(message);
            SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.LOADING, Value: message });
        }
    }
    this.GetWindowTitle = function (tab) {
        try {
            if (tab.title === undefined) {
                SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.UNDEFINEDTITLE, Value: "" }, false);
            }
            else {
                SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: tab.title }, false);
            }
        }
        catch (e) {
            SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/GetWindowTitle.");
        }
    }

    this.GetLocation = function (tab) {
        try {
            if (tab.url === undefined) {
                SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.UNDEFINEDURL, Value: "" }, false);
            }
            else {
                SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: tab.url }, false);
            }
        }
        catch (e) {
            SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/GetLocation.");
        }
    }

    this.processRunScriptRequest = function (request) {
        for (var tab in SWAT.tabs) {
            if (SWAT.tabs[tab].mainPort.tab.id == SWAT.activePort.tab.id) {
                console.log("Tab " + SWAT.tabs[tab].tabId + " has " + SWAT.tabs[tab].frames.length + " frames.");
                SWAT.tabs[tab].mainPort.postMessage({ request: request });
                SWAT.contentScriptRequestState = SWAT.requestState.Sent;
            }
        }
    }
}

SWAT.contentScriptCommunication = function () {

    this.handlePortResponse = function (message) {
        if (message.response.StatusCode == SWATStatusCode.SUCCESS || isAssertElementIsActiveFailure(message) || (SWAT.portQueue.length == 0 && message.response.StatusCode != SWATStatusCode.PORTDISCONNECTED))
            SWAT.sendResponseToParsedRequest(message.response, false);
        else
            sendRequestUsingPortInQueue(SWAT.currentRequest);
    }

    function isAssertElementIsActiveFailure(message) {
        return message.request.Command == "AssertElementIsActive" && message.response.StatusCode == SWATStatusCode.ELEMENTNOTACTIVE;
    }

    this.processContentScriptRequest = function (request) {
        if (!SWATUtilities.isValid(SWAT.activeTabId)) {
            var message = "ActiveTabId cannot be null";
            SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.NOATTACHEDWINDOW, Value: message });
            return;
        }

        buildPortQueue();
        sendRequestUsingPortInQueue(request);
    }

    function sendRequestUsingPortInQueue(request) {
        var port = SWAT.portQueue.shift();
        if (port) {
			port.postMessage({ request: request });
		} else {
			buildPortQueue();
			SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.PORTDISCONNECTED, Value: "Port is no longer open" });
		}
		SWAT.contentScriptRequestState = SWAT.requestState.Sent;
    }

    function buildPortQueue() {
        SWAT.portQueue = [];
        for (var tab in SWAT.tabs) {
            if (SWAT.tabs[tab].mainPort.tab.id == SWAT.activePort.tab.id) {
                console.log("Tab " + SWAT.tabs[tab].tabId + " has " + SWAT.tabs[tab].frames.length + " frames.");
                SWAT.portQueue.push(SWAT.tabs[tab].mainPort);
                for (var frame in SWAT.tabs[tab].frames) {
                    SWAT.portQueue.push(SWAT.tabs[tab].frames[frame].framePort);
                }
            }
        }
    }

    this.sendMessageOnActivePortAndAlsoKeepTrackOfIt = function (message) {
        try {
            SWAT.lastRequestToBeSentWhichHasntBeenAnsweredYet = message.request;
            SWAT.activePort.postMessage(message);
        }
        catch (e) {
            console.log("Tried to send request without an active port. Request will retry when connected, but will hang until then.");
        }
    }
}

SWAT.SWATCommunication = function() {
	/**
	* Sends the passed argument as the result of a command
	* @param result object encapsulating result to send
	* @param wait whether we expect this command to possibly make changes
	* we need to wait for (e.g. adding elements, opening windows) - if so,
	* we wait until we think these effects are done
	*/
	this.sendResponseByXHR = function(result, wait) {
		var resultMessage = JSON.stringify(result);

		if (SWAT.xmlHttpRequest != null) {
			SWAT.xmlHttpRequest.abort();
		}

		SWAT.xmlHttpRequest = new XMLHttpRequest();
		SWAT.xmlHttpRequest.onreadystatechange = handleXmlHttpRequestReadyStateChange;
		SWAT.xmlHttpRequest.open("POST", SWAT.xmlHttpRequestUrl, true);
		SWAT.xmlHttpRequest.setRequestHeader("Content-type", "application/json");

		//Default to waiting for page changes, just in case
		//TODO(danielwh): Iterate over tabs checking their status
		if (wait === undefined || wait == null || wait) {
			//console.log("Sending result: [" + resultMessage + "] by XHR with timeout");
			setTimeout(sendResult, 600, [result]);
		}
		else {
			//console.log("Sending result: [" + resultMessage + "] by XHR without timeout");
			sendResult(result);
		}
	}
	
	/**
	* When we receive a request, dispatches parseRequest to execute it
	*/
	function handleXmlHttpRequestReadyStateChange() {
		if (this.readyState == 4) {
			if (this.status != 200) {
				console.log("Request state was 4 but status: " + this.status + ".  responseText: " + this.responseText);
			}
			else {
				console.log("GOT XHR RESPONSE: " + this.responseText);
				var request = JSON.parse(this.responseText);

				if (request.Command == "quit") {
					//We're only allowed to send a response if we're blocked waiting for one, so pretend
					console.log("SENDING QUIT XHR");
					var comm = new SWAT.SWATCommunication();
					comm.sendResponseByXHR(JSON.stringify({ StatusCode: SWATStatusCode.SUCCESS }), false);
				}
				else {
					console.log("Got request to execute from XHR: " + this.responseText);
					SWAT.parseRequest(request);
				}
			}
		}
		console.log("XHR ready state: " + this.readyState);
	}

	/**
	* Actually sends the result by XHR
	* Should only EVER be called by sendResponseByXHR,
	* as it ignores things like setting up XHR and blocking,
	* and just forces the sending over an assumed open XHR
	* @param result String to send
	*/
	function sendResult(result) {
		//TODO: Iterate over tabs checking their status
		SWAT.xmlHttpRequest.send(result + "\nEOResponse\n");
		console.log("Sent result by XHR: " + JSON.stringify(result));
	}
}

SWAT.refreshBrowserCallback = function(tab) {
	try {
		if (!SWATUtilities.isValid(tab)) {
			//executeScript does not pass the tab to the callback function
			//so let's get it.
			chrome.tabs.get(SWAT.activeTabId, SWAT.refreshBrowserCallback);
			return;
		}

		if (SWAT.refreshBrowserTime == 0)
			SWAT.checkIfOnBeforeUnload();

		if (SWAT.waitingOnTabReConnect) {
			//wait till we re-connect to port.
			console.log("Tab has not re-connected on port yet. (" + SWAT.refreshBrowserTime + ") seconds.");

			if (SWAT.refreshBrowserTime > SWAT.refreshBrowserTimeout) {
				messageDisconnect = "Refresh Browser timeout for URL " + SWAT.currentUrl + " at " + (SWAT.refreshBrowserTimeout / 1000) + " second(s). Tab did not re-connect on time";

				//The RefreshBrowser operation did not complete on time.
				SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.NAVIGATEFAIL, Value: messageDisconnect }, false);
				return;
			}

			SWAT.refreshBrowserTime += SWAT.refreshBrowserTimeIncrement;
			setTimeout("SWAT.refreshBrowserCallback()", SWAT.refreshBrowserTimeIncrement);
			//var instance = new SWAT.backgroundCommands();
			//setTimeout(function(){instance.refreshBrowserCallback(undefined)},SWAT.refreshBrowserTimeIncrement);
			return;
		} else {

			if (tab.status != "complete") {
				console.log("Page not refreshed, currently: " + tab.status);

				if (SWAT.refreshBrowserTime > SWAT.refreshBrowserTimeout) {
					message = "Refresh Browser timeout for URL " + SWAT.currentUrl + " at " + (SWAT.refreshBrowserTimeout / 1000) + " second(s).";
					SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.NAVIGATEFAIL, Value: message }, false);
					return;
				}
				else {
					SWAT.refreshBrowserTime += SWAT.refreshBrowserTimeIncrement;
					setTimeout("SWAT.refreshBrowserCallback()", SWAT.refreshBrowserTimeIncrement);
					//var instance = new SWAT.backgroundCommands();
					//setTimeout(function(){instance.refreshBrowserCallback(undefined)},SWAT.refreshBrowserTimeIncrement);
					return;
				}
			}
			else {
				//SUCCESS
				SWAT.refreshBrowserTime = 0;
				SWAT.reconnectingTabId = null;
				var message = "Refreshed " + tab.url;
				console.log(message);

				SWAT.setActiveTabDetails(tab);
				SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message }, false);
			}
		}
	}
	catch (e) {
		SWAT.printAndSendUnhandledErrorMessage("Caught unhandled exception '" + e.toString() + "' in background/refreshBrowserCallback.");
	}
}

/**
* Sends the response to a request, which has been parsed by parseRequest
* Should be used only from within parseRequest (or methods called from it),
* because it adheres to the blocking semantics of parseRequest
*/
SWAT.sendResponseToParsedRequest = function(toSend, wait) {
    if (!SWAT.isBlockedWaitingForResponse) {
        console.log("Tried to send a response (" + toSend.Value + ") when not waiting for one.  Dropping response.");
        return;
    }

    SWAT.isBlockedWaitingForResponse = false;
    SWAT.lastRequestToBeSentWhichHasntBeenAnsweredYet = null;
    //console.log("SENDING RESPONSE TO PARSED REQUEST");
	var comm = new SWAT.SWATCommunication();
	comm.sendResponseByXHR(JSON.stringify(toSend), wait);
    SWAT.setExtensionBusyIndicator(false);
}

SWAT.checkForOnBeforeUnload = function(tabId) {
    //conditions checking if the specific tab has been disconnected and timeout value
    if (!SWAT.tabsToKill.GetValue(tabId) && SWAT.tabsToKillTimeouts.GetValue(tabId) < SWAT.checkKillOnBeforeUnloadTimeout) {
        var newTime = SWAT.tabsToKillTimeouts.GetValue(tabId) + SWAT.checkKillOnBeforeUnloadTimeIncrement;
        SWAT.tabsToKillTimeouts.Add(tabId, newTime);
        setTimeout("SWAT.checkForOnBeforeUnload(" + tabId + ")", SWAT.checkKillOnBeforeUnloadTimeIncrement);
        return;
    }
    else if (!SWAT.tabsToKill.GetValue(tabId)) //will evaluate to true if the tab was not disconnected
        SWAT.tabCount++;

    var cmds = new SWAT.backgroundCommands();
	cmds.handleKillAllOpenBrowsers();
}

/**
* Used by closeBrowser, refreshBrowser, and navigateBrowser to check for
* an onBeforeUnload page. If a tab hasnt disconnected by the timeout it
* is assumed to be a onBeofreUnload and the user pressed cancel.
*/
SWAT.checkIfOnBeforeUnload = function() {
    //conditions checking if the tab has been disconnected and timeout values
    if ((!SWAT.tabWasDisconnected) && (SWAT.checkOnBeforeUnloadTime < SWAT.checkOnBeforeUnloadTimeout)) {
        SWAT.checkOnBeforeUnloadTime += SWAT.checkOnBeforeUnloadTimeIncrement;
        setTimeout("SWAT.checkIfOnBeforeUnload()", SWAT.checkOnBeforeUnloadTimeIncrement);
        return;
    }
    else if (!SWAT.tabWasDisconnected) {
        //enter here if the tab was not disconnected, this means it was onBeforeUnload and cancel was pressed
		var message = "onBeforeUnload dialog triggered and cancel was pressed.. detected";
        console.log(message);
        SWAT.waitingOnTabReConnect = false;
        SWAT.reconnectingTabId = null;
		
		if (SWAT.isClosingTab)
			SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.SUCCESS, Value: message }, false);
		else
			SWAT.OnBeforeUnloadCancel = true;
		
		SWAT.isClosingTab = false;
    }
}

SWAT.resetActiveTabDetails = function() {
	//console.log("resetActiveTabDetails called!");
	SWAT.backUp.activePort = SWAT.activePort;
	SWAT.backUp.activeTabId = SWAT.activeTabId;
	SWAT.backUp.currentUrl = SWAT.currentUrl;
	
	SWAT.activePort = null;
	SWAT.activeTabId = null;
	SWAT.currentUrl = null;
	SWAT.resetCurrentlyWaitingOnContentScriptTime();
}

SWAT.setActiveTabDetails = function(tab) {
	//console.log("setActiveTabDetails called!");
	if (SWAT.OnBeforeUnloadCancel && SWATUtilities.isValid(SWAT.backUp.activePort) 
	&& SWAT.backUp.activePort.tab.id == tab.id && SWAT.backUp.activePort.tab.windowId == tab.windowId)
	{
		SWAT.OnBeforeUnloadCancel = false;
		SWAT.activePort = SWAT.backUp.activePort;
		SWAT.activeTabId = SWAT.backUp.activeTabId;
		SWAT.currentUrl = SWAT.backUp.currentUrl;
	}
	else
	{
		SWAT.activeTabId = tab.id;
		SWAT.activeWindowId = tab.windowId;
		SWAT.currentUrl = tab.url;
	}
	
	SWAT.resetCurrentlyWaitingOnContentScriptTime();
}

SWAT.resetCurrentlyWaitingOnContentScriptTime = function() {
	SWAT.currentlyWaitingUntilGiveUpOnContentScriptLoading =
		SWAT.timeoutUntilGiveUpOnContentScriptLoading;
}

SWAT.setExtensionBusyIndicator = function(busy) {
	if (busy) {
	    chrome.browserAction.setIcon({ path: "icons/busy.png" });
	}
	else {
	    chrome.browserAction.setIcon({ path: "icons/free.png" });
	}
}

SWAT.printAndSendUnhandledErrorMessage = function(msg) {
	console.log(msg);
	SWAT.sendResponseToParsedRequest({ StatusCode: SWATStatusCode.UNHANDLEDERROR, Value: msg }, false);
}

/**
* Used for debugging tab information as it is used
* @param tab object loaded in the browser
*/
SWAT.logTabInformation = function(tab) {
    console.log("Tab id:" + tab.id + ", index:" + tab.index + ", windowId:" + tab.windowId + ", selected:" + tab.selected + ", url:" + tab.url + ", title:" + tab.title + ", status:" + tab.status);
}
