/**
* All functions which take elements assume that they are not null,
* and are present as passed on the DOM.
*/

SWATContentScript = {};

SWATContentScript.internalElementArray = [];
SWATContentScript.port = null;
SWATContentScript.currentDocument = window.document;
SWATContentScript.currentElement = null;
SWATContentScript.scrollTop = 0;

SWATContentScript.bgConnected = false;
SWATContentScript.MatchType = { Equals: 0, Contains: 1, Unknown: 2 };
SWATContentScript.ancestorWithStyle = null;
SWATContentScript.isStyle = false;

$.extend(
	$.expr[':'], 
    {
	    InnerHtml: function(elem, index, match) 
	    {
	        match = SWATReplacement.restoreProblemChars(match);
	        var identifier = match[3];
	        var innerHTML = $(elem).html();
	        if (!SWATUtilities.isValid(innerHTML))
	        {
	            return false;
	        }
	        return (innerHTML + "") == identifier;
	    }
	}
);

$.extend(
	$.expr[':'], 
    {
		InnerHtmlContains: function(elem, index, match) 
		{
		    match = SWATReplacement.restoreProblemChars(match);
		    var identifier = match[3];
		    var innerHTML = $(elem).html();
			if (!SWATUtilities.isValid(innerHTML)) 
			{
	            return false;
	        }
	        return (innerHTML + "").indexOf(identifier) >= 0;
		}
	}
);

	$.extend(
	$.expr[':'], 
	    {
	    Expression: function (elem, index, match) 
	    {
	        SWATContentScript.isStyle = false;
	        SWATContentScript.ancestorWithStyle = null;
	        var isMatch = true;
	        var ancestors = new Array();
	        match = SWATReplacement.restoreProblemChars(match);
	        var identifier = match[3].replace("\\;", "\\@").split(';');
	        var functions = new SWATContentScript.findElementFunctions();

	        for (var regexp in identifier) 
	        {

	            var identExp = identifier[regexp].replace("\\@", ";");

	            var colonIndex = identExp.indexOf(':');
	            var equalsIndex = identExp.indexOf('=');

	            var matchType = functions.getMatchType(colonIndex, equalsIndex);
	            if (matchType == SWATContentScript.MatchType.Unknown)
	            {
	                return false;
	            }

	            var attributeBeginIndex = 0;
	            var attributeEndIndex = (matchType == SWATContentScript.MatchType.Contains) ? colonIndex : equalsIndex;
	            var attributeValueIndex = (matchType == SWATContentScript.MatchType.Contains) ? colonIndex + 1 : equalsIndex + 1;

	            var currentElement = elem;
	            var attribute = identExp.substring(attributeBeginIndex, attributeEndIndex);

                //fix for edge case for find element when searching for checkbox using CHECKED would cause a failure
	            var indexOfAttribute = attribute.lastIndexOf('.') + 1;

	            if (attribute.substring(indexOfAttribute, attribute.length).toLowerCase() == "checked")
	                attribute = attribute.replace(attribute.substring(indexOfAttribute, attribute.length), attribute.substring(indexOfAttribute, attribute.length).toLowerCase());

	            var matchCount = attribute.split('#');

	            var numMatches = parseInt(matchCount[1]);

	            var isMatchCount = false;
	            if (matchCount.length == 2) 
	            {

	                if (isNaN(numMatches) || matchType != SWATContentScript.MatchType.Contains)
	                {
	                    return false;
	                }

	                attribute = matchCount[0];
	                isMatchCount = true;
	            }

	            if (matchCount.length > 2) 
	            {
	                return false;
	            }

	            var attributeValue = identExp.substring(attributeValueIndex);

	            var generationCount = functions.getGenerationCount(attribute);
	            if (generationCount == null || attributeBeginIndex == attributeEndIndex) 
	            {
	                return false;
	            }

	            var expressionToken = {};
	            expressionToken.element = currentElement;
	            expressionToken.attribute = attribute;
	            expressionToken.attributeValue = attributeValue;
	            expressionToken.matchType = matchType;
	            expressionToken.count = numMatches;
	            expressionToken.isMatchCount = isMatchCount;

	            if (generationCount > 0)
	            {

	                expressionToken.element = currentElement = functions.getAncestor(elem, generationCount);
	                if (currentElement == null) 
	                {
	                    return false;
	                }

	                attributeBeginIndex = attribute.lastIndexOf('.') + 1;
	                if (attributeBeginIndex == attributeEndIndex)
	                {
	                    return false;
	                }

	                expressionToken.attribute = attribute = attribute.substring(attributeBeginIndex);

	                if (expressionToken.attribute == "style")
	                {
	                    SWATContentScript.ancestorWithStyle = expressionToken.element;

	                    //get all text after "style:" and do regexp on elem.attr(style) text
	                    //	if found return true; else keep on going
	                    isMatch = functions.matchStyleAttributeText(identifier, regexp, expressionToken);

	                    if (isMatch)
	                    {
	                        //style is assumed to be the last check so safely return on success
	                        return true;
	                    }

	                    //if style and parentElement then matchRegex
	                    isMatch = functions.matchRegex(expressionToken);
	                }
	                else 
	                {
	                    ancestors.push(expressionToken);
	                }
	            }
	            else
	            {
	                isMatch = functions.matchStyleAttributeText(identifier, regexp, expressionToken);

	                if (isMatch) 
	                {
	                    //style is assumed to be the last check so safely return on success
	                    return true;
	                }

	                isMatch = functions.matchRegex(expressionToken);
	            }

	            if (!isMatch)
	            {
	                return false;
	            }
	        }

	        for (var i = 0; i < ancestors.length; i++)
	        {
	            isMatch = functions.matchRegex(ancestors[i]);

	            if (!isMatch)
	            {
	                return false;
	            }
	        }

	        return isMatch;
	    }
	}
);

SWATContentScript.sendRequestToBg = function()
{
    if (!SWATContentScript.bgConnected)
    {
        setTimeout(SWATContentScript.sendRequestToBg, 500);
     
        chrome.extension.sendRequest({ action: 'start' }, function (response)
        {
            SWATContentScript.bgConnected = true;
        });
    }
    else
    {
        SWATContentScript.connectToBackgroundScript();
    }
}

if (SWATContentScript.currentDocument.location != "about:blank")
{
    //If loading windows using window.open, the port is opened
    //while we are on about:blank (which always reports window.name as ''),
    //and we use port-per-tab semantics, so don't open the port if
    //we're on about:blank

    SWATContentScript.sendRequestToBg();
}

SWATContentScript.connectToBackgroundScript = function()
{
    console.log("Connecting to background script");

    var portName = (SWATContentScript.currentDocument.title == "SWAT HTTP Server") ? "SWAT HTTP Server" : window.name;
    SWATContentScript.port = chrome.extension.connect({ name: portName });

    SWATContentScript.port.onMessage.addListener(SWATContentScript.parsePortMessage);

    var isFrameset = (SWATContentScript.currentDocument.getElementsByTagName("frameset").length > 0);
    SWATContentScript.port.postMessage(
		{
		    request: { Command: "newTabInformation" },
		    response:
				{
				    StatusCode: SWATStatusCode.SUCCESS,
				    Value:
					{
					    isFrameset: isFrameset,
					    frameCount: window.frames.length,
					    portName: SWATContentScript.port.name
					}
				}
		}
	);
}

/**
* Parse messages coming in on the port.
* Sends relevant response back down the port
* @param message JSON message of format: 
*  request: 
*  { 
*      Command: "CommandName", 
*      Arguments: 
*      {
*          Arg1: Arg1Val, 
*          Arg2: Arg2Val 
*      }
*  }
*/
SWATContentScript.parsePortMessage = function(message)
{
    try {
        if (!SWATUtilities.isValid(message.request.Command))
        {
            var errorMessage = "Received bad request: " + JSON.stringify(message);
            console.log(errorMessage);
            SWATContentScript.port.postMessage(
				{
				    request: message.request,
				    response: { StatusCode: SWATStatusCode.CONTENTSCRIPTERROR, Value: errorMessage }
				}
			);

        } else 
        {
            console.log("Content Script received request: " + JSON.stringify(message) + " (" + window.name + ")");

            //wait indicates whether this is a potentially page-changing change (see background.js's sendResponseByXHR)
            var response = { Command: message.request.Command, StatusCode: SWATStatusCode.SUCCESS, Value: null, wait: false };
			var cmds = new SWATContentScript.ContentScriptCommands();
			
            switch (message.request.Command)
            {

                case "GetElementAttribute":
                    response = cmds.GetElementAttribute(message);
                    break;
                case "SetElementAttribute":
                    response = cmds.SetElementAttribute(message);
                    break;
                case "StimulateElement":
                    response = cmds.StimulateElement(message);
                    break;
                case "GetDocumentAttribute":
                    response = cmds.GetDocumentAttribute(message);
                    break;
                case "SetDocumentAttribute":
                    response = cmds.SetDocumentAttribute(message);
                    break;
                case "RunJavaScript":
                    response = cmds.RunScript(message);
                    break;
                case "PressKeys":
                    response = cmds.PressKeys(message);
                    break;
				case "AssertElementIsActive":
					response = cmds.AssertElementIsActive(message);
					break;

                default:
                    console.log(JSON.stringify(message.request));
                    response =
					    {
					        request: message.request,
					        response:
							    {
							        StatusCode: SWATStatusCode.UNSUPPORTEDCOMMAND,
							        Value: message.request.Command + " is unsupported"
							    }
					    };
                    break;
            }

            SWATContentScript.port.postMessage(response);
            console.log("Sent response: " + JSON.stringify(response));
        }
    }
    catch (e) 
    {
        console.log("Caught unhandled exception '" + e.toString() + "', sending error response");
        SWATContentScript.port.postMessage(
			{
				request: message.request,
			    response:
					{
					    StatusCode: SWATStatusCode.UNHANDLEDERROR,
					    Value: "An unexpected error occured while executing " + message.request.Command + ", exception dump: " + e
					}
			}
		);
    }
}

SWATContentScript.ContentScriptCommands = function ()
{

    this.AssertElementIsActive = function (message)
    {
        var identifierType = message.request.Arguments.identifierType;
        var identifier = message.request.Arguments.identifier;
        var tagName = message.request.Arguments.tagName;
        var finder = new SWATContentScript.findElementFunctions();
        var responseMessage = finder.findElement(identifierType, identifier, tagName);
        var elem = SWATContentScript.currentElement;
        var responseVal, statusCode;
        
        if (responseMessage.StatusCode == SWATStatusCode.SUCCESS)
        {
            var result = SWATContentScript.currentDocument.activeElement == elem;
            if (result)
            {
                responseVal = "Element is current active element";
                statusCode = SWATStatusCode.SUCCESS;
            } else 
            {
                responseVal = "Element with identifier: " + identifier + " is not current active element";
                statusCode = SWATStatusCode.ELEMENTNOTACTIVE;
            }
        } else
        {
            responseVal = "Element with identifier: " + identifier + " was not found.";
            statusCode = SWATStatusCode.ELEMENTDOESNOTEXIST;
        }
        return { request: message.request, response: { StatusCode: statusCode, Value: responseVal} };
    }

    this.PressKeys = function (message) 
    {
        var keyCodes = message.request.Arguments.keyCodes;
        var identifierType = message.request.Arguments.identifierType;
        var identifier = message.request.Arguments.identifier;
        var tagName = message.request.Arguments.tagName;

        var finder = new SWATContentScript.findElementFunctions();
        var responseMessage = finder.findElement(identifierType, identifier, tagName);
        var elem = SWATContentScript.currentElement;

        if (responseMessage.StatusCode != SWATStatusCode.SUCCESS) //If it didn't find the element, we're not on the right ContentScript
        {
            responseMessage = {
                StatusCode: SWATStatusCode.ELEMENTDOESNOTEXIST,
                Value: "The element with type: " + identifierType + ", identifier: " + identifier + ", and tag: " + tagName + " was not found."
            };

            return { request: message.request, response: responseMessage };
        }

        console.log("Found the PressKeys element!");

        //Key codes are sepparated by a "-"
        while (keyCodes.indexOf("-", 0) != -1) 
        {
            var keyCode = keyCodes.substring(0, keyCodes.indexOf("-", 0));
            keyCodes = keyCodes.substr(keyCodes.indexOf("-", 0) + 1);

            //Key down event
            var keyDownEvent = document.createEvent("KeyboardEvent");
            keyDownEvent.initEvent("keydown", true, true, null, false, false, false, false, keyCode, keyCode);
            elem.dispatchEvent(keyDownEvent);

            //Key press event
            var keyPressEvent = document.createEvent("KeyboardEvent");
            keyPressEvent.initEvent("keypress", true, true, null, false, false, false, false, 0, keyCode);
            elem.dispatchEvent(keyPressEvent);

            //Key up event
            var keyUpEvent = document.createEvent("KeyboardEvent");
            keyUpEvent.initEvent("keyup", true, true, null, false, false, false, false, keyCode, keyCode);
            elem.dispatchEvent(keyUpEvent);
        }

        //Set attribute
        var cmds = new SWATContentScript.ContentScriptCommands();
        var attribute = cmds.GetElementAttribute(message).response.Value;
        if (SWATUtilities.isValid(attribute))
            message.request.Arguments.attributeValue = attribute + message.request.Arguments.attributeValue;
        var cmds = new SWATContentScript.ContentScriptCommands();
        responseMessage = cmds.SetElementAttribute(message);

        if (responseMessage.response.StatusCode == SWATStatusCode.SUCCESS)
        {
            responseMessage =
				{
				    StatusCode: SWATStatusCode.SUCCESS,
				    Value: "PressKeys finished successfully."
				};
        }
        else {
            responseMessage =
				{
				    StatusCode: SWATStatusCode.UNHANDLEDERROR,
				    Value: "PressKeys did not finish successfully."
				};
        }
        return { request: message.request, response: responseMessage };
    }

    this.StimulateElement = function (message)
    {
        var identifierType = message.request.Arguments.identifierType;
        var identifier = message.request.Arguments.identifier;
        var tagName = message.request.Arguments.tagName;
        var eventName = message.request.Arguments.eventName;
        var responseMessage;

        switch (eventName)
        {
            case "error":
            case "load":
            case "resize":
            case "unload":
                responseMessage = { StatusCode: SWATStatusCode.SUCCESS };
                break;

            default:
                var finder = new SWATContentScript.findElementFunctions();
                responseMessage = finder.findElement(identifierType, identifier, tagName);
        }

        if (responseMessage.StatusCode == SWATStatusCode.SUCCESS) 
        {

            var result = SWATContentScript.stimulateElement(eventName, SWATContentScript.currentElement);

            var resultString, statusCode;

            if (result == false)
            {
                resultString = "Failed";
                statusCode = SWATStatusCode.UNSUPPORTEDEVENT;
            }
            else
            {
                resultString = "Succeeded";
                statusCode = SWATStatusCode.SUCCESS;
            }

            var responseValue = resultString + " to stimulate element: " + identifier + " with event : " + eventName;
            console.log(responseValue);

            responseMessage =
				{
				    StatusCode: statusCode,
				    Value: responseValue
				};

        }

        return { request: message.request, response: responseMessage };
    }

    this.GetDocumentAttribute = function (message)
    {
        var theAttributeName = message.request.Arguments.theAttributeName;
        var theAttribute = null;
        var result, resultString, statusCode;

        if (theAttributeName == "scrollTop")
        {
            theAttribute = SWATContentScript.scrollTop;
        }
        else
        {
            result = eval("window.document.documentElement." + theAttributeName + ";");
            if (SWATUtilities.isValid(result))
            {
                theAttribute = result;
            }
        }

        if (!SWATUtilities.isValid(theAttribute))
        {
            resultString = "Failed";
            statusCode = SWATStatusCode.ELEMENTDOESNOTEXIST;
            theAttribute = "The document attribute " + theAttributeName + " does not exist.";
        }
        else
        {
            resultString = "Succeeded";
            statusCode = SWATStatusCode.SUCCESS;
        }

        console.log(resultString + " in getting document attribute " + theAttributeName + "'s value");

        responseMessage =
			{
			    StatusCode: statusCode,
			    Value: theAttribute
			};

        return { request: message.request, response: responseMessage };
    }

    this.SetDocumentAttribute = function (message)
    {
        var theAttributeName = message.request.Arguments.theAttributeName;
        var theAttributeValue = message.request.Arguments.theAttributeValue;
        var theAttribute = null;
        var resultString, statusCode, result;

        if (theAttributeName == "scroll")
        {
            return { request: message.request, response: { StatusCode: SWATStatusCode.SUCCESS,
                Value: "Scroll only supported in Internet Explorer, not Chrome"}};
        }

        // Get the document element first to see if it is undefined
        result = eval("window.document.documentElement." + theAttributeName + ";");
        if (SWATUtilities.isValid(result))
        {
            if (theAttributeName == "scrollTop") 
            {
                eval("window.scroll(0, " + theAttributeValue + ");");
                SWATContentScript.scrollTop = theAttributeValue;
                theAttribute = theAttributeValue;
            }
            else 
            {
                theAttribute = eval("window.document.documentElement." + theAttributeName + " = " + theAttributeValue + ";");
            }
        }

        if (!SWATUtilities.isValid(theAttribute) || !SWATUtilities.isValid(result)) 
        {
            resultString = "Failed";
            statusCode = SWATStatusCode.ELEMENTDOESNOTEXIST;
            theAttribute = "The document attribute " + theAttributeName + " does not exist.";
        }
        else 
        {
            resultString = "Succeeded";
            statusCode = SWATStatusCode.SUCCESS;
        }

        console.log("Set" + theAttributeName + "'s value to: " + theAttributeName);

        responseMessage =
			{
			    StatusCode: statusCode,
			    Value: theAttribute
			};

        return { request: message.request, response: responseMessage };
    }

    this.GetElementAttribute = function (message)
    {

        var identifierType = message.request.Arguments.identifierType;
        var identifier = message.request.Arguments.identifier;
        var tagName = message.request.Arguments.tagName;

        var attributeName = message.request.Arguments.attributeName;

        var finder = new SWATContentScript.findElementFunctions();
        var responseMessage = finder.findElement(identifierType, identifier, tagName);

        if (responseMessage.StatusCode == SWATStatusCode.SUCCESS) 
        {

            var result;
			if (attributeName.toLowerCase() == "href")
				result = SWATContentScript.currentElement.href;
			else if (attributeName.toLowerCase() == "value")
				result = $(SWATContentScript.currentElement).val();
			else
				result = $(SWATContentScript.currentElement).attr(attributeName);

            var resultString, statusCode;

            if (!SWATUtilities.isValid(result))
            {
                result = "";
            }

            resultString = "Succeeded";
            statusCode = SWATStatusCode.SUCCESS;

            console.log(resultString + " in getting attribute " + attributeName + "'s value");

            responseMessage =
				{
				    StatusCode: statusCode,
				    Value: result
				};
        }

        return { request: message.request, response: responseMessage };

    }

    this.SetElementAttribute = function (message)
    {
        var identifierType = message.request.Arguments.identifierType;
        var identifier = message.request.Arguments.identifier;
        var tagName = message.request.Arguments.tagName;

        var attributeName = message.request.Arguments.attributeName;
        var attributeValue = message.request.Arguments.attributeValue;

        var finder = new SWATContentScript.findElementFunctions();
        var responseMessage = finder.findElement(identifierType, identifier, tagName);

        if (responseMessage.StatusCode == SWATStatusCode.SUCCESS) 
        {

            if (SWATContentScript.currentElement.type === "file") 
            {
                responseMessage =
				{
				    StatusCode: SWATStatusCode.SUCCESS,
				    Value: "file"
				};

                return { request: message.request, response: responseMessage };
            }

            if (attributeName.toLowerCase() == "checked" && attributeValue.toLowerCase() == "false")
                attributeValue = "";

            var result = $(SWATContentScript.currentElement).attr(attributeName, attributeValue);

            var resultString, statusCode;

            if (!SWATUtilities.isValid(result)) 
            {
                resultString = "Failed";
                statusCode = SWATStatusCode.ATTRIBUTEERROR;
            }
            else
            {
                resultString = "Succeeded";
                statusCode = SWATStatusCode.SUCCESS;
            }

            var responseValue = resultString + " to set attribute with name:" + attributeName + " to value:" + attributeValue;
            console.log(responseValue);

            responseMessage =
				{
				    StatusCode: statusCode,
				    Value: responseValue
				};
        }

        return { request: message.request, response: responseMessage };
    }

    this.RunScript = function (message)
    {
        var evalScript = message.request.Arguments.theScript;
        
        //The new script stored on the page so that it can be accessed within the scope of the page
        var scriptParent = document.createElement("div");
        scriptParent.setAttribute("id", "SWAT_scriptParent33326");
        scriptParent.innerHTML = "<input type=\"hidden\" name=\"SWAT_scriptCode33326\" id=\"SWAT_scriptCode33326\" value=\"" + evalScript + "\" />"
        document.getElementsByTagName("head")[0].appendChild(scriptParent);
        console.log(document.getElementById("SWAT_scriptCode33326").value);

        //An input field is created on the page in order to store the result of the script
        var resultParent = document.createElement("div");
        resultParent.setAttribute("id", "SWAT_resultParent33326");
        resultParent.innerHTML = "<input type=\"hidden\" name=\"SWAT_scriptResult33326\" id=\"SWAT_scriptResult33326\" />";
        document.getElementsByTagName("head")[0].appendChild(resultParent);

        //This function acts as a wrapper that will tell the page to eval the script and store the result in the designated input field

        function SWAT_fxnWrapper33326()
        {
            var scriptRes = document.getElementById("SWAT_scriptResult33326");
            var res;
            try 
            {
                res = eval(document.getElementById("SWAT_scriptCode33326").value);
            }
            catch (e) 
            {
                res = e.message;
            }
            scriptRes.value = res;
        }

        //Adds the above function Wrapper to page as JavaScript, the syntax provided here prompts the page to run the script
        var newScript = document.createElement("script");
        newScript.appendChild(document.createTextNode('(' + SWAT_fxnWrapper33326 + ')();'));
        document.getElementsByTagName("head")[0].appendChild(newScript);

        //Retrieves the return value stored in the input field generated earlier
        var responseValue = document.getElementById("SWAT_scriptResult33326").value;
        console.log(responseValue);
        var responseMessage = { StatusCode: SWATStatusCode.SUCCESS, Value: responseValue };

        removeElement('SWAT_scriptParent33326', 'SWAT_scriptCode33326');
        removeElement('SWAT_resultParent33326', 'SWAT_scriptResult33326');
        removeScript();

        return { request: message.request, response: responseMessage };
    }

    //Used by RunScript to remove the elements that it adds to the page
    function removeElement(parentID, childID) 
    {
        var parent = document.getElementById(parentID);
        var child = document.getElementById(childID);
        parent.removeChild(child);
        document.getElementsByTagName("head")[0].removeChild(parent);
    }

    //Used by RunScript to remove the script that it adds to the page
    function removeScript() {
        var allScripts = document.getElementsByTagName("script");
        var ourscript = null;

        for(var i = 0; i < allScripts.length; i++)
        {
            if (allScripts[i].innerHTML.indexOf("SWAT_fxnWrapper33326") != -1)
            {
                ourScript = allScripts[i];
                break;
            }
        }

        document.getElementsByTagName("head")[0].removeChild(ourScript);
    }
}

SWATContentScript.findElementFunctions = function () 
{

    this.findElement = function (identifierType, identifier, tagName) 
    {
        console.log("Looking for element with identifierType:" + identifierType + ", identifier:" + identifier + ", tagName:" + tagName);

        if (identifierType.toLowerCase() != "expression")
        {
            if (identifierType.toLowerCase() != "innerhtmlcontains")
                identifier = identifierType + "=" + identifier;
            else
                identifier = "innerhtml:" + identifier;
            identifierType = "Expression";
        }

        identifier = SWATReplacement.replaceProblemChars(identifier);
        SWATContentScript.currentElement = getElement(identifierType, identifier, tagName);
        var resultString, statusCode, responseValue;

        if (!SWATUtilities.isValid(SWATContentScript.currentElement))
        {
            resultString = " not ";
            statusCode = SWATStatusCode.ELEMENTDOESNOTEXIST;
        }
        else 
        {
            resultString = " ";
            statusCode = SWATStatusCode.SUCCESS;
        }

        responseValue = "Element" + resultString + "found with identifier:" + identifier + ", tagName:" + tagName;
        console.log(responseValue);

        return { StatusCode: statusCode, Value: responseValue };
    }

    function getElement(identifierType, identifier, tagName)
    {
        switch (identifierType) 
        {
            case "Id":
            case "Name":
                return $(tagName.toLowerCase() + "[" + identifierType + "=" + identifier + "]").get(0);

            case "InnerHtml":
            case "InnerHtmlContains":
            case "Expression":
                return $(tagName + ":" + identifierType + "(" + identifier + ")").get(0);
        }
    }

    function transformToExpression(identifierType, identifier) 
    {
        return;
    }

    this.matchStyleAttributeText = function (identifier, pos, expressionToken) 
    {
        pos++;
        if (expressionToken.attribute == "style") 
        {
            if (pos <= identifier.length)
            {
                identifier = identifier.slice(pos);
            }

            var identifierText = $.trim(expressionToken.attributeValue);

            while (identifierText.indexOf(" ") >= 0)
            {
                identifierText = identifierText.replace(" ", "");
            }

            var identifierRest = identifier.join(";");

            if (identifierRest.length > 0) 
            {
                identifierText += ";";
                identifierText += identifierRest;
            }

            var strRegExp = new RegExp(identifierText, 'gim');
            var actualAttributeValue = $(expressionToken.element).attr(expressionToken.attribute);
            var isMatch = strRegExp.test(actualAttributeValue);

            return isMatch;
        }
        return false;
    };

    function modifyExpressionTokenForStyle(expressionToken)
    {
        SWATContentScript.isStyle = true;
        var newPair = expressionToken.attributeValue.split(":");
        expressionToken.attribute = newPair.shift();
        expressionToken.attributeValue = newPair.shift();
        expressionToken.attributeValue = $.trim(expressionToken.attributeValue);

        return expressionToken;
    };

    this.getGenerationCount = function (attribute) 
    {

        var parentElements = attribute.split('.');
        for (var i = 0; i < parentElements.length - 1; i++)
        {
            if (parentElements[i] != "parentElement")
                return null;
        }
        return parentElements.length - 1;
    };

    this.getAncestor = function (currentElement, generationCount)
    {

        for (var i = 0; i < generationCount; i++) 
        {
            currentElement = $(currentElement).parent();
        }
        return currentElement;
    };

    this.getMatchType = function (colonIndex, equalsIndex) 
    {

        if (colonIndex < 0 && equalsIndex < 0)
        {
            return SWATContentScript.MatchType.Unknown;
        }

        if (equalsIndex < 0 || (colonIndex < equalsIndex && colonIndex > 0))
        {
            return SWATContentScript.MatchType.Contains;
        }
        else
        {
            return SWATContentScript.MatchType.Equals;
        }
    };

    this.matchRegex = function (expressionToken) 
    {
        if (expressionToken.attribute == "style") 
        {
            expressionToken = modifyExpressionTokenForStyle(expressionToken);
        }

        var regex = new RegExp(expressionToken.attributeValue, 'gim');

        var actualAttributeValue = null;

        if (isHyperLink(expressionToken)) 
        {
            actualAttributeValue = expressionToken.element.href;
        } else if (isInnerHtml(expressionToken))
        {
            actualAttributeValue = $(expressionToken.element).html();
        }
        else if (SWATContentScript.isStyle) 
        {
            if (SWATContentScript.ancestorWithStyle) 
            {
                expressionToken.element = SWATContentScript.ancestorWithStyle;
            }
            actualAttributeValue = $(expressionToken.element).css(expressionToken.attribute);
        }
        if (!SWATUtilities.isValid(actualAttributeValue))
        {
			if (expressionToken.attribute.toLowerCase() == "value")
				actualAttributeValue = $(expressionToken.element).val();
			else
				actualAttributeValue = $(expressionToken.element).attr(expressionToken.attribute);
        }

        if (!SWATUtilities.isValid(actualAttributeValue))
        {
            return false;
        }

        actualAttributeValue += "";
        var matches = actualAttributeValue.match(regex);

        if (!SWATUtilities.isValid(matches) || matches.length == 0) 
        {
            return false;
        }

        if (expressionToken.matchType == SWATContentScript.MatchType.Equals
			&& matches[0].length != actualAttributeValue.length)
            return false;

        if (expressionToken.isMatchCount && expressionToken.count == matches.length) 
        {
            return true;
        }
        else if (!expressionToken.isMatchCount)
        {
            return true;
        }

        return false;
    };

    function isHyperLink(expressionToken) 
    {
        return expressionToken.attribute.toLowerCase() == "href";
    };

    function isInnerHtml(expressionToken) 
    {
        return expressionToken.attribute.toLowerCase() == "innerhtml";
    };

}

SWATContentScript.stimulateElement = function(eventName, element) 
{
	
	var result = true;
	switch(eventName)
	{
			
		// MouseEvents
		case "click":
		case "dblclick":
		case "mousedown":
		case "mousemove":
		case "mouseout":
		case "mouseover":
		case "mouseup":
			var event = document.createEvent("MouseEvents");
			event.initMouseEvent(eventName, true, true, window,	0, 0, 0, 0, 0, false, false, false, false, 0, null);
			element.dispatchEvent(event);
			break;
		
		// HTMLEvents			
		case "focus":
			if (!(element.tagName.toLowerCase() === "option"))
			{
				element.focus();
				break;
			}
		case "blur":
		case "change":
		case "reset":
		case "select":
		case "submit":
			var event = document.createEvent("HTMLEvents");
			event.initEvent(eventName, true, true);
			element.dispatchEvent(event);
			break;
			
		// Window events
		// The element passed is irrelevant since the following events work only 
		//	with the document object and not any particular element.
		case "error":
		case "load":
		case "resize":
		case "unload":
			var event = document.createEvent("HTMLEvents");
			event.initEvent(eventName, true, true);
			element = document.documentElement;
			element.dispatchEvent(event);
			break;
			
		// UIEvents
		case "keydown":
		case "keypress":
		case "keyup":
			var event = document.createEvent("UIEvents");
			event.initUIEvent(eventName, true, true, null );
			element.dispatchEvent(event);
			break;
		
		default:
			result = false;
			break;
	}
	return result;
	
}
