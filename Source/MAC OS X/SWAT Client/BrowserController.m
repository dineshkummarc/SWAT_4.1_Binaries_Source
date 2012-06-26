//
//  BrowserController.m
//  SWAT Client
//
//  //  Last Modified by Brandon Foote 6/29/10
//  Copyright 2008 __MyCompanyName__. All rights reserved.
//

#import "BrowserController.h"
#import "Safari.h"

@implementation BrowserController
@synthesize screenshotPath, waitForNetworkTimeout;

-(id)init
{
	if(self = [super init])
	{
		currentWindowID = [[NSString alloc] initWithString:@""];
		currentStatus = BrowserStatusNormal;
		notToCloseTitle = [[NSString alloc] initWithString:@""];
		currentlyAttachedNonBrowserApplication = nil;
	}
	
	return self;
}

#pragma mark -
#pragma mark JSDialog Methods

-(NSString*)AssertJSDialogContent
{
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithString:@"tell application \"Safari\" to activate\rtell application \"System Events\" to tell process \"Safari\" to return value of static text 2 of window 1"];	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	//[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	
	if (desc != nil)
		return [desc stringValue];
	return @"";
}

-(NSString*)AssertJSDialogExists
{
	NSString *processResult = [self doesProcessExist];
	
	if (processResult == nil || [processResult isEqualToString:@"NO"])
		return @"JSDialog Not Found";
	
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithString:@"tell application \"Safari\" to activate\rtell application \"System Events\" to tell process \"Safari\" to set theType to role description of window 1\rtell application \"System Events\" to tell process \"Safari\" to set theTitle to title of window 1\rif theTitle does not contain \"modal\" then\rreturn theType\rend if\r"];	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	[appleScript release];
	[scriptSource release];
	
	if (desc != nil && [[desc stringValue] isEqualToString:@"dialog"])
	{
		return @"JSDialog Found";
	}
	return @"JSDialog Not Found";
}

-(NSString*) ClickJSDialog:(NSString*)buttonText
{
	NSString *processResult = [self doesProcessExist];
	
	if (processResult == nil || [processResult isEqualToString:@"NO"])
		return @"FAILED";
	
	NSString* appleScriptString = [[NSString alloc] initWithFormat:@"tell application \"Safari\" to activate\rtell application \"System Events\" to tell process \"Safari\" to tell window 1 to click button \"%@\"", buttonText];
	NSAppleScript* script = [[NSAppleScript alloc] initWithSource:appleScriptString];
	NSAppleEventDescriptor *desc = [script executeAndReturnError:nil];
	
	[script release];
	[appleScriptString release];
	
	[NSThread sleepForTimeInterval:1];
	
	if(desc != nil)
	{
		return [self completePreviousProcess];
	}
	else
	{
		return @"FAILED";
	}
}

-(NSString*)ClickJSDialogCancel
{
	return [self ClickJSDialog:@"Cancel"];
}

-(NSString*)ClickJSDialogOk
{
	return [self ClickJSDialog:@"OK"];
}

#pragma mark -
#pragma mark Navigation Methods

-(NSString*)AssertBrowserDoesOrDoesNotExist:(NSString*)windowTitle
{
	currentStatus = BrowserStatusNormal;
	NSString *processResult = [self doesProcessExist];
	
	if (processResult == nil)
		return @"failed";
	
	if ([processResult isEqualToString:@"NO"])
		return @"doesNotExist";
	
	NSString *theTitle = [windowTitle retain];
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource =  [[NSString alloc] initWithFormat:@"try\rtell application \"Safari\"\rset theTitle to \"%@\"\rset i to 1\rrepeat while i is less than or equal to (number of windows)\rset actualTitle to name of document i\rif (actualTitle contains theTitle) then\rreturn \"windowExists\"\rend if\rset i to i + 1\rend repeat\rreturn \"doesNotExist\"\rend tell\ron error errText number errNumber\rif errNumber is equal to -1719 then return \"doesNotExist\"\rend try", theTitle];
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	//[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	
	return [desc stringValue];
}

-(NSString*)AttachToWindow:(NSString*)windowTitle windowIndex:(NSString*)windowIndex
{
	currentStatus = BrowserStatusNormal;
	NSString *processResult = [self doesProcessExist];
	
	if (processResult == nil || [processResult isEqualToString:@"NO"])
		return @"failed-0";
	
	NSString *theTitle = [windowTitle retain];
	theTitle = [theTitle stringByReplacingOccurrencesOfString:@"\\" withString:@"\\\\"];
	theTitle = [theTitle stringByReplacingOccurrencesOfString:@"\"" withString:@"\\\""];
	theTitle = [theTitle stringByReplacingOccurrencesOfString:@"%" withString:@"\\%"];
	
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\" to activate\rtell application \"System Events\"\rtell process \"Safari\"\rset windowIndex to %@\rset counter to 0\rset theTitle to \"%@\"\rrepeat with i from 17 to (number of menu items of menu \"Window\" of menu bar 1)\rclick menu item i of menu \"Window\" of menu bar 1\rtell application \"Safari\"\rset actualTitle to name of document 1\rend tell\rif theTitle is equal to \"\" or theTitle is in actualTitle then\rif (counter is equal to windowIndex) then\rtell application \"Safari\"\rreturn id of window 1\rend tell\rend if\rset counter to counter + 1\rend if\rset i to i + 1\rend repeat\rreturn \"failed-\" & (counter as string)\rend tell\rend tell", windowIndex, theTitle];
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	//[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	
	if (desc == nil)
		return @"failed";
	if ([[desc stringValue] rangeOfString:@"failed" options:NSCaseInsensitiveSearch].location != NSNotFound)
		return [desc stringValue];
	currentWindowID = [[NSString alloc] initWithString:[desc stringValue]];
	return @"OK";
}

-(NSString*)CloseBrowser
{
	currentStatus = BrowserStatusNormal;
	
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\"\rset theIndex to index of window id %@\rtell application \"System Events\"\rtell process \"Safari\"\rclick button 1 in window theIndex\rend tell\rend tell\rend tell",[currentWindowID retain]];	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	[appleScript executeAndReturnError:&scriptError];
	
	[appleScript release];
	[scriptSource release];
	
	//[NSThread sleepForTimeInterval:1];
	
	//currentWindowID = [[NSString alloc] initWithString:@""];
	return @"OK";
}


-(NSString*)KillAllOpenBrowsers
{
	return [self KillAllOpenBrowsers:nil];
}


-(NSString*)KillAllOpenBrowsers:(NSString*)title
{
	currentStatus = BrowserStatusNormal;
	NSString *processResult = [self doesProcessExist];
	
	if (processResult == nil)
		return @"failed";
	
	if ([processResult isEqualToString:@"NO"])
		return @"OK";
	
	NSString *currentID = [[NSString alloc] initWithString:currentWindowID];
	if ([currentID isEqualToString:@""])
		//currentID = [[NSString alloc] initWithString:@"-1"];
		currentID = @"-1";
	NSString *theTitle = [title retain];
	NSString *scriptSource;	
	
	if (title == nil)
	{
		scriptSource =  [[NSString alloc] initWithFormat:@"set app_name to \"Safari\"\rset the_pid to (do shell script \"ps ax | grep \" & (quoted form of app_name) & \" | grep -v grep | awk '{print $1}'\")\rif the_pid is not \"\" then do shell script (\"kill -9 \" & the_pid)\rreturn \"OK\""];
	}
	else
	{
		currentStatus = BrowserStatusKillAllExcept;
		scriptSource =  [[NSString alloc] initWithFormat:@"tell application \"Safari\"\ractivate\rset theTitle to \"%@\"\rset i to 1\rset total to (number of windows)\rrepeat while i is less than or equal to total\rtry\rtell application \"System Events\"\rtell process \"Safari\"\rset actualTitle to title of window i\rend tell\rend tell\rif (actualTitle contains theTitle) then\rset i to i + 1\relse\rset theIndex to index of window i\rtell application \"System Events\"\rtell process \"Safari\"\rtry\rwith timeout of 2 seconds\rclick button 1 in window i\rend timeout\rend try\rif role description of window 1 contains \"dialog\" then\rreturn \"JSDialog Appeared\"\rend if\rend tell\rend tell\rset i to 1\rdelay 0.5\rend if\ron error\rset i to i + 1\rend try\rend repeat\rreturn \"OK\"\rend tell", theTitle, [currentID retain]];
		if (notToCloseTitle != nil)
			[notToCloseTitle release];
		notToCloseTitle = [[NSString alloc] initWithString:theTitle];
	}
	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:nil];
	
	[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	[currentID release];
	
	if (desc == nil)
	{
		return @"Failed";
	}
	if (title == nil)
	{
		currentWindowID = [[NSString alloc] initWithString:@""];
		return @"OK";
	}
	return [desc stringValue];
}

-(NSString*)NavigateBrowser:(NSString*)url
{
	currentStatus = BrowserStatusNavigate;
	
	NSString *newUrl;
	if ([[url substringToIndex:7] isEqualToString:@"http://"] || [[url substringToIndex:8] isEqualToString:@"https://"])
		newUrl = [url retain];
	else
		newUrl = [[NSString alloc] initWithFormat:@"http://%@", [url retain]];
	
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\"\rtry\rwith timeout of 2 seconds\rset URL of document 1 of window id %@ to \"%@\"\rend timeout\rend try\rdelay 1\rend tell", [currentWindowID retain], newUrl];	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	[appleScript executeAndReturnError:&scriptError];
	
	[appleScript release];
	[scriptSource release];
	[newUrl release];
	[currentWindowID release];
	
	NSString *result = [self pageHasLoaded];
	
	if ([result isEqualToString:@"failed"])
		return @"failed";
	
	return @"OK";
}

-(NSString*)OpenBrowser
{	
	currentStatus = BrowserStatusNormal;
	NSString *processResult = [self doesProcessExist];
	
	if (processResult == nil)
		return @"failed";
	
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource;
	if ([processResult isEqualToString:@"YES"])
	{
		scriptSource = [[NSString alloc] initWithString:@"tell application \"Safari\"\rmake new document at end of documents\rset URL of document 1 to \"about:swat\"\ractivate\rreturn id of window 1\rend tell"];	
	}
	else
	{
		scriptSource = [[NSString alloc] initWithString:@"tell application \"Safari\"\ropen location \"about:swat\"\rtell application \"System Events\" to tell process \"Safari\" to set value of attribute \"AXMain\" of window 1 to true\ractivate\rreturn id of window 1\rend tell"];
	}
	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	//[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	
	if (desc == nil)
		return @"failed";
	if (currentWindowID != nil)
		[currentWindowID release];
	currentWindowID = [[NSString alloc] initWithString:[desc stringValue]];
	return @"OK";
}

#pragma mark -
#pragma mark Press Keys Methods

-(NSString*)PressKeys:(NSString*)word
{
	currentStatus = BrowserStatusNormal;
	NSString *theWord = [word retain];
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource =  [[NSString alloc] initWithFormat:@"tell application \"Safari\" to activate\rtell application \"Safari\" to set index of window id %@ to 1\rtell application \"System Events\" to tell process \"Safari\" to set value of attribute \"AXMain\" of window 1 to true\rdelay 1\rset theWord to \"%@\"\rset theLength to length of theWord\rrepeat with i from 1 to theLength\rperformKeyPress(character i of theWord)\rend repeat\rreturn \"OK\"\r\ron performKeyPress(theCharacter)\rconsidering case\rtell application \"System Events\"\rtell process \"Safari\"\rif theCharacter is equal to \"â\" then\rkey code 22 using option down\rkeystroke \"a\"\relse if theCharacter is equal to \"ä\" then\rkeystroke \"u\" using option down\rkeystroke \"a\"\relse if theCharacter is equal to \"à\" then\rkeystroke \"`\" using option down\rkeystroke \"a\"\relse if theCharacter is equal to \"á\" then\rkeystroke \"e\" using option down\rkeystroke \"a\"\relse if theCharacter is equal to \"ã\" then\rkeystroke \"n\" using option down\rkeystroke \"a\"\relse if theCharacter is equal to \"å\" then\rkeystroke \"k\" using option down\rkeystroke \"a\"\relse if theCharacter is equal to \"ð\" then\rkeystroke \"d\" using option down\relse if theCharacter is equal to \"æ\" then\rkeystroke \"'\" using option down\relse if theCharacter is equal to \"Æ\" then\rkeystroke \"'\" using option down & shift down\relse if theCharacter is equal to \"Ä\" then\rkeystroke \"u\" using option down\rkeystroke \"A\"\relse if theCharacter is equal to \"Å\" then\rkeystroke \"k\" using option down\rkeystroke \"A\"\relse if theCharacter is equal to \"À\" then\rkeystroke \"`\" using option down\rkeystroke \"A\"\relse if theCharacter is equal to \"Á\" then\rkeystroke \"e\" using option down\rkeystroke \"A\"\relse if theCharacter is equal to \"Â\" then\rkey code 22 using option down\rkeystroke \"A\"\relse if theCharacter is equal to \"Ã\" then\rkeystroke \"n\" using option down\rkeystroke \"A\"\relse if theCharacter is equal to \"ß\" then\rkeystroke \"s\" using option down\relse if theCharacter is equal to \"Þ\" then\rkeystroke \"t\" using option down & shift down\relse if theCharacter is equal to \"þ\" then\rkeystroke \"t\" using option down\relse if theCharacter is equal to \"ç\" then\rkeystroke \"c\" using option down\rkeystroke \"c\"\relse if theCharacter is equal to \"Ç\" then\rkeystroke \"c\" using option down\rkeystroke \"C\"\relse if theCharacter is equal to \"œ\" then\rkeystroke \"q\" using option down\relse if theCharacter is equal to \"Œ\" then\rkeystroke \"q\" using option down & shift down\relse if theCharacter is equal to \"Ð\" then\rkeystroke \"d\" using option down & shift down\relse if theCharacter is equal to \"ê\" then\rkey code 22 using option down\rkeystroke \"e\"\relse if theCharacter is equal to \"ë\" then\rkeystroke \"u\" using option down\rkeystroke \"e\"\relse if theCharacter is equal to \"è\" then\rkeystroke \"`\" using option down\rkeystroke \"e\"\relse if theCharacter is equal to \"é\" then\rkeystroke \"e\" using option down\rkeystroke \"e\"\relse if theCharacter is equal to \"È\" then\rkeystroke \"`\" using option down\rkeystroke \"E\"\relse if theCharacter is equal to \"É\" then\rkeystroke \"e\" using option down\rkeystroke \"E\"\relse if theCharacter is equal to \"Ê\" then\rkey code 22 using option down\rkeystroke \"E\"\relse if theCharacter is equal to \"Ë\" then\rkeystroke \"u\" using option down\rkeystroke \"E\"\relse if theCharacter is equal to \"ï\" then\rkeystroke \"u\" using option down\rkeystroke \"i\"\relse if theCharacter is equal to \"î\" then\rkey code 22 using option down\rkeystroke \"i\"\relse if theCharacter is equal to \"ì\" then\rkeystroke \"`\" using option down\rkeystroke \"i\"\relse if theCharacter is equal to \"í\" then\rkeystroke \"e\" using option down\rkeystroke \"i\"\relse if theCharacter is equal to \"Î\" then\rkey code 22 using option down\rkeystroke \"I\"\relse if theCharacter is equal to \"Ì\" then\rkeystroke \"`\" using option down\rkeystroke \"I\"\relse if theCharacter is equal to \"Í\" then\rkeystroke \"e\" using option down\rkeystroke \"I\"\relse if theCharacter is equal to \"Ï\" then\rkeystroke \"u\" using option down\rkeystroke \"I\"\relse if theCharacter is equal to \"ñ\" then\rkeystroke \"n\" using option down\rkeystroke \"n\"\relse if theCharacter is equal to \"Ñ\" then\rkeystroke \"n\" using option down\rkeystroke \"N\"\relse if theCharacter is equal to \"ô\" then\rkey code 22 using option down\rkeystroke \"o\"\relse if theCharacter is equal to \"ö\" then\rkeystroke \"u\" using option down\rkeystroke \"o\"\relse if theCharacter is equal to \"ò\" then\rkeystroke \"`\" using option down\rkeystroke \"o\"\relse if theCharacter is equal to \"ó\" then\rkeystroke \"e\" using option down\rkeystroke \"o\"\relse if theCharacter is equal to \"õ\" then\rkeystroke \"n\" using option down\rkeystroke \"o\"\relse if theCharacter is equal to \"ø\" then\rkeystroke \"o\" using option down\relse if theCharacter is equal to \"Ø\" then\rkeystroke \"o\" using option down & shift down\relse if theCharacter is equal to \"Ô\" then\rkey code 22 using option down\rkeystroke \"O\"\relse if theCharacter is equal to \"Ö\" then\rkeystroke \"u\" using option down\rkeystroke \"O\"\relse if theCharacter is equal to \"Õ\" then\rkeystroke \"n\" using option down\rkeystroke \"O\"\relse if theCharacter is equal to \"Ò\" then\rkeystroke \"`\" using option down\rkeystroke \"O\"\relse if theCharacter is equal to \"Ó\" then\rkeystroke \"e\" using option down\rkeystroke \"O\"\relse if theCharacter is equal to \"š\" then\rkeystroke \"v\" using option down\rkeystroke \"s\"\relse if theCharacter is equal to \"Š\" then\rkeystroke \"v\" using option down\rkeystroke \"S\"\relse if theCharacter is equal to \"û\" then\rkey code 22 using option down\rkeystroke \"u\"\relse if theCharacter is equal to \"ú\" then\rkeystroke \"e\" using option down\rkeystroke \"u\"\relse if theCharacter is equal to \"ù\" then\rkeystroke \"`\" using option down\rkeystroke \"u\"\relse if theCharacter is equal to \"ü\" then\rkeystroke \"u\" using option down\rkeystroke \"u\"\relse if theCharacter is equal to \"Ü\" then\rkeystroke \"u\" using option down\rkeystroke \"U\"\relse if theCharacter is equal to \"Ù\" then\rkeystroke \"`\" using option down\rkeystroke \"U\"\relse if theCharacter is equal to \"Ú\" then\rkeystroke \"e\" using option down\rkeystroke \"U\"\relse if theCharacter is equal to \"Û\" then\rkey code 22 using option down\rkeystroke \"U\"\relse if theCharacter is equal to \"ÿ\" then\rkeystroke \"u\" using option down\rkeystroke \"y\"\relse if theCharacter is equal to \"ý\" then\rkeystroke \"e\" using option down\rkeystroke \"y\"\relse if theCharacter is equal to \"Ÿ\" then\rkeystroke \"u\" using option down\rkeystroke \"Y\"\relse if theCharacter is equal to \"Ý\" then\rkeystroke \"e\" using option down\rkeystroke \"Y\"\relse if theCharacter is equal to \"ž\" then\rkeystroke \"v\" using option down\rkeystroke \"z\"\relse if theCharacter is equal to \"Ž\" then\rkeystroke \"v\" using option down\rkeystroke \"Z\"\relse if theCharacter is equal to \"£\" then\rkey code 20 using option down\relse if theCharacter is equal to \"¥\" then\rkeystroke \"y\" using option down\relse if theCharacter is equal to \"€\" then\rkey code 19 using option down & shift down\relse if theCharacter is equal to \"¢\" then\rkey code 21 using option down\relse if theCharacter is equal to \"ƒ\" then\rkeystroke \"f\" using option down\relse if theCharacter is equal to \"°\" then\rkey code 28 using option down & shift down\relse if theCharacter is equal to \"¿\" then\rkey code 44 using option down & shift down\relse if theCharacter is equal to \"¡\" then\rkey code 18 using option down\relse if theCharacter is equal to \"«\" then\rkeystroke \"\\\\\" using option down\relse if theCharacter is equal to \"»\" then\rkeystroke \"\\\\\" using option down & shift down\relse if theCharacter is equal to \"‘\" then\rkeystroke \"]\" using option down\relse if theCharacter is equal to \"’\" then\rkeystroke \"]\" using option down & shift down\relse if theCharacter is equal to \"“\" then\rkeystroke \"[\" using option down\relse if theCharacter is equal to \"”\" then\rkeystroke \"[\" using option down & shift down\relse\rkeystroke theCharacter\rend if\rend tell\rend tell\rend considering\rend performKeyPress\r", [currentWindowID retain], theWord];
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	
	//[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	
	if (desc != nil)
		return [desc stringValue];
	return @"failed";
}

-(NSString*)PressKeyCode:(NSString*)keycode
{
	currentStatus = BrowserStatusNormal;
	NSString *theKeyCode = [keycode retain];
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource =  [[NSString alloc] initWithFormat:@"tell application \"Safari\" to activate\rtell application \"Safari\" to set index of window id %@ to 1\rtell application \"System Events\"\rtell process \"Safari\"\rset value of attribute \"AXMain\" of window 1 to true\rdelay 1\rkey code %@\rreturn \"OK\"\rend tell\rend tell", [currentWindowID retain], theKeyCode];
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	//[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	
	if (desc != nil)
		return [desc stringValue];
	return @"failed";
}

-(NSString*)PressModifiedKeyCode:(NSString*)modifier withKeyCode:(NSString*)keyCode
{
	currentStatus = BrowserStatusNormal;
	NSString *theModifier = modifier;
	NSString *theKeyCode = keyCode;
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource =  [[NSString alloc] initWithFormat:@"tell application \"Safari\" to activate\rtell application \"Safari\" to set index of window id %@ to 1\rtell application \"System Events\"\rtell process \"Safari\"\rset value of attribute \"AXMain\" of window 1 to true\rdelay 1\rkey down %@\rkey code %@\rkey up %@\rreturn \"OK\"\rend tell\rend tell", [currentWindowID retain], theModifier, theKeyCode, theModifier];
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource];
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	//[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	
	if (desc != nil)
		return [desc stringValue];
	return @"failed";
	
}

-(NSString*)PressKeyCombination:(NSString*)modifier withCharacter:(NSString*)character
{
	currentStatus = BrowserStatusNormal;
	NSString *theModifier = [modifier retain];
	NSString *theCharacter = [character retain];
	
	NSNumberFormatter * f = [[NSNumberFormatter alloc] init];
	[f setNumberStyle:NSNumberFormatterDecimalStyle];
	NSNumber * theNumber = [f numberFromString:theCharacter];
	[f release];
	
	NSString *scriptSource;
	if (theNumber == nil) 
		scriptSource =  [[NSString alloc] initWithFormat:@"tell application \"Safari\" to activate\rtell application \"Safari\" to set index of window id %@ to 1\rtell application \"System Events\"\rtell process \"Safari\"\rset value of attribute \"AXMain\" of window 1 to true\rdelay 1\rkeystroke \"%@\" using %@ down\rreturn \"OK\"\rend tell\rend tell", [currentWindowID retain], theCharacter, theModifier];
	else
	{
		NSString *theKeyCode;
		switch([theNumber integerValue])
		{
			case 1:
				theKeyCode = @"18";
				break;
			case 2:
				theKeyCode = @"19";
				break;
			case 3:
				theKeyCode = @"20";
				break;
			case 4:
				theKeyCode = @"21";
				break;
			case 5:
				theKeyCode = @"23";
				break;
			case 6:
				theKeyCode = @"22";
				break;
			case 7:
				theKeyCode = @"26";
				break;
			case 8:
				theKeyCode = @"28";
				break;
			case 9:
				theKeyCode = @"25";
				break;
			default:
				theKeyCode = @"29";
				break;
		}
		scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\" to activate\rtell application \"Safari\" to set index of window id %@ to 1\rtell application \"System Events\"\rtell process \"Safari\"\rset value of attribute \"AXMain\" of window 1 to true\rdelay 1\rkey code %@ using %@ down\rreturn \"OK\"\rend tell\rend tell", [currentWindowID retain], theKeyCode, theModifier];
		
	}
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	//[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	
	if (desc != nil)
		return [desc stringValue];
	return @"failed";
	
	
	
}

#pragma mark -
#pragma mark Screen Shot Methods

-(NSString*)TakeScreenshot:(NSString*)windowOrScreen withPrefix:(NSString*)filePrefix
{
	currentStatus = BrowserStatusNormal;
	NSString *processResult = [self doesProcessExist];
	
	if (processResult == nil || [processResult isEqualToString:@"NO"])
		return @"failed";
	if ([windowOrScreen isEqualToString:@"Window"])
	{
		if ([currentWindowID isEqualToString:@""] || [[self doesWindowExist] isEqualToString:@"NO"])
		{	
			if (currentWindowID != nil)
				[currentWindowID release];
			currentWindowID = [[NSString alloc] initWithString:@""];
			return @"~-^No Browser Attached^-~";
		}
		return [self getWindowScreenshot:[filePrefix retain]];
	}
	else if ([windowOrScreen isEqualToString:@"Screen"])
		return [self getDesktopScreenshot:[filePrefix retain]];
	return @"failed";
}

- (NSString*)getWindowScreenshot:(NSString*)filePrefix
{
	NSInteger totalHeight = 0;
	NSInteger browserHeight;
	NSInteger clientWidth;
	NSInteger clientHeight;
	CGWindowID theWindowID = [currentWindowID intValue];
	
	NSDictionary *scriptErrorBefore = [[NSDictionary alloc] init];
	NSString *scriptSourceBefore =  [[NSString alloc] initWithFormat:@"tell application \"Safari\"\rset windowID to %@\rdo JavaScript \"window.scrollTo(0,0)\" in document 1 of window id windowID\rset bounds of window id windowID to {0, 0, 200, 200}\rset theWidth to do JavaScript \"document.documentElement.scrollWidth\" in document 1 of window id windowID\rset bounds of window id windowID to {0, 0, theWidth + 40, 2000}\rset theHeight to do JavaScript \"document.documentElement.scrollHeight\" in document 1 of window id windowID\rset bounds of window id windowID to {0, 0, theWidth + 40, theHeight + 72}\rset theIndex to index of window id windowID\rset bounds of window id windowID to {0, 0, theWidth + 40, theHeight + 72}\rset theIndex to index of window id windowID\rtell application \"System Events\" to tell process \"Safari\" to set theWindowSize to size of window theIndex\rreturn (theHeight as string) & \",\" & item 1 of theWindowSize & \",\" & item 2 of theWindowSize as string\rend tell",[currentWindowID retain]];
	NSAppleScript *appleScriptBefore = [[NSAppleScript alloc] initWithSource:scriptSourceBefore]; 
	NSAppleEventDescriptor *descBefore = [appleScriptBefore executeAndReturnError:&scriptErrorBefore];
	NSArray *messageParts = [[descBefore stringValue] componentsSeparatedByString:@","];
	
	[appleScriptBefore release];
	[scriptSourceBefore release];
	
	browserHeight = [[messageParts objectAtIndex:0] integerValue];
	clientWidth = [[messageParts objectAtIndex:1] integerValue];
	clientHeight = [[messageParts objectAtIndex:2] integerValue];
	
	if (browserHeight >= clientHeight)
	{
		clientWidth -= 15 ;
	}
	
	NSImage *finalImage = [[NSImage alloc] initWithSize:NSMakeSize(clientWidth, browserHeight)];
	NSMutableArray *images = [[NSMutableArray alloc] init];
	do
	{
		NSString *scriptSource =  [[NSString alloc] initWithFormat:@"tell application \"Safari\"\rdo javascript \"window.scrollTo(0,%d)\" in document 1 of window id %@\rend tell", totalHeight,[currentWindowID retain]];
		NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
		[appleScript executeAndReturnError:nil];
		
		
		[appleScript release];
		[scriptSource release];
		
		[NSThread sleepForTimeInterval:.5];
		
		CGImageRef windowImage = CGWindowListCreateImage(CGRectMake(0, 92, clientWidth, clientHeight - 72 + 2) , kCGWindowListOptionIncludingWindow, theWindowID, kCGWindowImageDefault);
		
		// little bit of error checking
		if(CGImageGetWidth(windowImage) <= 1) {
			CGImageRelease(windowImage);
			[finalImage release];
			[images release];
			return @"failed";
		}
		
		// Create a bitmap rep from the window and convert to NSImage...
		NSBitmapImageRep *bitmapRep = [[NSBitmapImageRep alloc] initWithCGImage: windowImage];
		NSImage *image = [[NSImage alloc] init];
		[image addRepresentation: bitmapRep];
		[images addObject:image];
		[image release];
		[bitmapRep release];
		
		totalHeight += clientHeight - 72;
	} while (totalHeight < browserHeight);
	
	totalHeight = 0;
	for(int i = [images count] - 1; i >= 0; i--)
	{
		NSImage *image = (NSImage *)[images objectAtIndex:i];
		[finalImage lockFocus];
		[image drawInRect:NSMakeRect(0, totalHeight, clientWidth, [image size].height) fromRect:NSMakeRect(0, 0, clientWidth, [image size].height) operation:NSCompositeCopy fraction:1];
		[finalImage unlockFocus];
		
		if (i == [images count] - 1)
		{
			totalHeight += browserHeight % (clientHeight - 72);
		}
		else
		{
			totalHeight += clientHeight - 72;
		}
	}
	[images release];
	NSBitmapImageRep *bmpImageRep = [[NSBitmapImageRep alloc]initWithData:[finalImage TIFFRepresentation]];
	//add the NSBitmapImage to the representation list of the target
	[finalImage addRepresentation:bmpImageRep];
	NSString *result = [self saveScreenShot:bmpImageRep withPrefix:[filePrefix retain]];
	[finalImage release];
	return result;
}

- (NSString*)getDesktopScreenshot:(NSString*)filePrefix
{
	CGImageRef windowImage = CGWindowListCreateImage(CGRectInfinite, kCGWindowListOptionOnScreenOnly, kCGNullWindowID, kCGWindowImageDefault);
    
    // little bit of error checking
    if(CGImageGetWidth(windowImage) <= 1) {
        CGImageRelease(windowImage);
        return @"failed";
    }
    
    // Create a bitmap rep from the window and convert to NSImage...
    NSBitmapImageRep *bitmapRep = [[NSBitmapImageRep alloc] initWithCGImage: windowImage];
    return [self saveScreenShot:bitmapRep withPrefix:[filePrefix retain]];
}

-(NSString *)saveScreenShot:(NSBitmapImageRep*)bitmapRep withPrefix:(NSString*)filePrefix
{
	NSData *data = [bitmapRep representationUsingType:NSJPEGFileType properties:[NSDictionary dictionaryWithObject:[NSNumber numberWithInteger:0.8] forKey:NSImageCompressionFactor]]; 
	NSFileManager *filemgr = [NSFileManager defaultManager];
	
	if ([filemgr contentsOfDirectoryAtPath:screenshotPath error:nil] == nil)
	{
		[filemgr createDirectoryAtPath:screenshotPath withIntermediateDirectories:YES attributes:nil error:nil];
	}
	NSDate *today = [NSDate date];
	NSArray *messageParts = [[today description] componentsSeparatedByString:@" "];
	NSString *theFileName = [[NSString alloc] initWithFormat:@"%@%@ %@ at %@.jpg",screenshotPath, filePrefix, [messageParts objectAtIndex:0],[messageParts objectAtIndex:1]];
	theFileName = [theFileName stringByReplacingOccurrencesOfString:@":" withString:@"_"];
	[data writeToFile:theFileName atomically: NO];
	return theFileName;
}

#pragma mark -
#pragma mark Misc Methods


-(NSString*)SetFileInput:(NSString*)filePath
{
	NSString *selectFilePath = [[NSString alloc] initWithString:@""];
	if ([filePath hasPrefix:@"\\\\"])
		//filePath = [[NSString alloc] initWithString:[filePath substringFromIndex:2]];
		filePath = [filePath substringFromIndex:2];
	NSArray *messageParts = [filePath componentsSeparatedByString:@"\\"];
	NSString *harddrive = [[NSString alloc] initWithString:[messageParts objectAtIndex:0]];
	for (int i = 1; i < [messageParts count]; i++)
	{
		selectFilePath = [NSString stringWithFormat:@"%@set ready to \"NO\"\rdelay 1\rrepeat with i from 1 to %@\rtry\rset temp to value of busy indicator 1 of sheet 1 of window theIndex\ron error errText\rif (errText contains \"get busy indicator\") then\rset ready to \"YES\"\rexit repeat\rend if\rend try\rdelay 1\rend repeat\rif (ready is equal to \"NO\") then\rclick button \"Cancel\" of sheet 1 of window theIndex\rreturn \"failed timeout\"\rend if\rtry\rselect static text \"%@\" of list 1 of scroll area %d of scroll area 1 of browser 1 of splitter group 1 of group 1 of sheet 1 of window theIndex\ron error\rclick button \"Cancel\" of sheet 1 of window theIndex\rend try\r",selectFilePath, self.waitForNetworkTimeout, [messageParts objectAtIndex:i], i];
	}
	
	NSString *scriptSource = [[NSString alloc] initWithFormat:@"tell application \"System Events\"\rtell process \"Safari\"\rset harddrive to \"%@\"\rtell application \"Safari\"\rset theIndex to index of window id %@\rend tell\rclick radio button 3 of radio group 1 of group 1 of sheet 1 of window theIndex\rset total to number of rows of outline 1 of scroll area 1 of splitter group 1 of group 1 of sheet 1 of window theIndex\rset i to 1\rset ready to \"NO\"\rrepeat with j from 1 to %@\rif (ready is equal to \"YES\") then\rexit repeat\rend if\rrepeat while i is less than or equal to total\rtry\rset theHarddrive to value of static text 1 of row i of outline 1 of scroll area 1 of splitter group 1 of group 1 of sheet 1 of window theIndex\rif (theHarddrive is equal to harddrive) then\rset ready to \"YES\"\rexit repeat\rend if\rend try\rset i to i + 1\rend repeat\rif (ready is equal to \"YES\") then\rselect row i of outline 1 of scroll area 1 of splitter group 1 of group 1 of sheet 1 of window theIndex\rexit repeat\rend if\rdelay 1\rend repeat\rif (ready is equal to \"NO\") then\rclick button \"Cancel\" of sheet 1 of window theIndex\rreturn \"failed\"\rend if\r%@\rclick button \"Choose\" of sheet 1 of window theIndex\rreturn \"passed\"\rend tell\rend tell", harddrive, currentWindowID, self.waitForNetworkTimeout, selectFilePath];
	
	
	//NSLog(@"%@",scriptSource);
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	[appleScript release];
	[scriptSource release];
	[harddrive release];
	
	if (desc == nil || [desc stringValue] == nil)
	{
		return @"failed";
	}
	
	return [desc stringValue];
}

-(NSString*)StimulateElement:(NSString*)javascriptToExecute
{
	NSString *theJavascript = [javascriptToExecute retain];
 	
	NSString *theResult = [self DoJavascript:theJavascript];
	
	currentStatus = BrowserStatusStimulate;
	
	[javascriptToExecute release];
	
	return theResult;
}

-(NSString*)SetElementAttribute:(NSString*)javascriptToExecute
{
	NSString *theJavascript = [javascriptToExecute retain];
 	
	NSString *theResult = [self executeJavascript:theJavascript];
	
	currentStatus = BrowserStatusNormal;
	
	[javascriptToExecute release];
	
	return theResult;
}

-(NSString*)GetElementAttribute:(NSString*)javascriptToExecute
{
	NSString *theJavascript = [javascriptToExecute retain];
 	
	NSString *theResult = [self executeJavascript:theJavascript];
	
	currentStatus = BrowserStatusNormal;
	
	[javascriptToExecute release];
	
	return theResult;
}

-(NSString*)AssertElementIsActive:(NSString*)javascriptToExecute
{
	NSString *theJavascript = [javascriptToExecute retain];
 	
	NSString *theResult = [self executeJavascript:theJavascript];
	
	currentStatus = BrowserStatusNormal;
	
	[javascriptToExecute release];
	
	return theResult;
}

-(NSString*)executeJavascript:(NSString*)javascriptToExecute
{
	NSString *theJavascript = [javascriptToExecute retain];
 	
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\"\rset result to \"\"\rtry\rwith timeout of 2 seconds\rset result to do JavaScript \"try { %@ } catch (e) { e.message; }\" in document 1 of window id %@\rend timeout\rend try\rtry\rset tmp to result\ron error\rset result to \"SyntaxError: Parse error\"\rend try\rreturn result\rend tell", theJavascript, [currentWindowID retain]];	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	[appleScript release];
	[scriptSource release];
	
	if (desc == nil || [desc stringValue] == nil)
	{
		return @"";
	}
	
	if (![[desc stringValue] isEqualToString:@"failed"])
	{	
		return [desc stringValue];
	}
	//FATAL ERROR
	return @"failed";
}

-(NSString*)DoJavascript:(NSString*)javascriptToExecute
{
	currentStatus = BrowserStatusNormal;
	if ([javascriptToExecute rangeOfString:@".reload();"].location != NSNotFound)
		currentStatus = BrowserStatusRefresh;
	
	NSString *theResult = [self executeJavascript:[javascriptToExecute retain]];
	
	[javascriptToExecute release];
	
	return theResult;
}

-(NSString*)DoAppleScript:(NSString*)applescriptToExecute
{
	NSString *theAppleScript = [applescriptToExecute retain];
 	
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithString:theAppleScript];	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	[appleScript release];
	[scriptSource release];
	
	if (desc == nil || [desc stringValue] == nil)
	{
		return [scriptError description];
	}
	
	return [desc stringValue];
}

-(NSString*)AttachToNonBrowserWindow:(NSString*)applicationName
{
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithFormat:@"set appName to \"%@\"\rtell application \"System Events\" to set appOpen to (name of processes) contains appName\rif appOpen is true then\rtell application appName\rset windowNames to \"\"\rset i to 1\rrepeat while i is less than or equal to (number of windows)\rset windowNames to windowNames & name of document i & \" \"\rset i to i + 1\rend repeat\rreturn windowNames\rend tell\relse\rreturn appName & \" is not open\"\rend if", [applicationName retain]];
	NSAppleScript *script = [[NSAppleScript alloc] initWithSource:scriptSource];
	NSAppleEventDescriptor *desc = [script executeAndReturnError:&scriptError];
	
	[script release];
	[scriptSource release];
	
	if (desc == nil)
		return @"failed";
	else {
		currentlyAttachedNonBrowserApplication = applicationName;
		return [desc stringValue];
	}

}

-(NSString*)CloseNonBrowserWindow:(NSString*)windowIndex
{
	if (currentlyAttachedNonBrowserApplication == nil)
		return @"No attached non browser window, failed";

	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithFormat:@"tell application \"%@\" to close window %@\rreturn \"Window successfully closed\"", currentlyAttachedNonBrowserApplication, windowIndex];
	NSAppleScript *script = [[NSAppleScript alloc] initWithSource:scriptSource];
	NSAppleEventDescriptor *desc = [script executeAndReturnError:&scriptError];
	
	[script release];
	[scriptSource release];
	
	[currentlyAttachedNonBrowserApplication release];
	
	currentlyAttachedNonBrowserApplication = nil;
	
	if (desc == nil)
		return @"failed";
	else
		return [desc stringValue];

}

-(NSString*)AssertTopWindow:(NSString*)windowIndex
{
	currentStatus = BrowserStatusNormal;
	NSString *processResult = [self doesProcessExist];
	
	if (processResult == nil || [processResult isEqualToString:@"NO"])
		return @"failed-0";
	
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] init];
	
	if([windowIndex isEqualToString:@"0"]) 
	{ 
		scriptSource = @"tell application \"System Events\"\rset topApp to name of the first process whose frontmost is true \rtell process topApp \rname of window 1 \rend tell \rend tell";
	}
	else 
	{
		//scriptSource = @"tell application \"System Events\"\rset firstTopApp to name of the first process whose frontmost is true \rset zList to \"\" \rset numTabs to 0 \rset processList to name of every process whose visible is true \rset numProcesses to count processList \rrepeat \rkey down command \rdelay 0.4 \rrepeat with j from 1 to numTabs \rkeystroke tab \rset j to j + 1 \rdelay 0.4 \rend repeat \rkey up command \rdelay 0.4 \rset currentTopApp to name of the first process whose frontmost is true \rtell process currentTopApp \rset numWindows to number of windows \rrepeat with i from 1 to numWindows \rset theTitle to name of window i \rset zList to zList & theTitle & \"~-\" \rset i to i + 1 \rend repeat \rend tell \rif numTabs is equal to numProcesses then \rkey down command \rdelay 0.4 \rrepeat with j from 1 to numTabs - 1 \rkeystroke tab \rset j to j + 1 \rdelay 0.4 \rend repeat \rkey up command \rdelay 0.4 \rexit repeat \relse \rset numTabs to numTabs + 1 \rend if \rend repeat \rset numTabs to 1 \rrepeat with i from 1 to numProcesses - 1 \rkey down command \rdelay 0.4 \rrepeat with j from 1 to numTabs \rkeystroke tab \rset j to j + 1 \rdelay 0.4 \rend repeat \rkey up command \rset numTabs to numTabs + 1 \rdelay 0.4 \rend repeat \rkey down command \rrepeat with j from 1 to numTabs - 1 \rkeystroke tab \rset j to j + 1 \rdelay 0.4 \rend repeat \rkey up command \rdelay 0.4 \rreturn zList \rend tell";
		scriptSource = @"try \rtell application \"Safari\" \rset zList to \"\" \rset i to 1 \rrepeat while i is less than or equal to (number of windows) \rset windowTitle to name of window i \rset zList to zList & windowTitle & \"~-\" \rset i to i + 1 \rend repeat \rreturn zList \rend tell \ron error errText number errNumber \rif errNumber is equal to -1719 then return \"does not exist\" \rend try";
	}
	
	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	[appleScript release];
	[scriptSource release];
	
	if (desc == nil)
		return @"failed";
	else 
		return [desc stringValue];
}

-(NSString*)SetWindowPosition:(NSString*)winPosition
{ 
	currentStatus = BrowserStatusNormal;
	
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = @"";
	
	if([winPosition isEqualToString:@"MAXIMIZE"]) 
	{ 
		scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\"\ractivate\rset theIndex to index of window id %@\rtell application \"System Events\"\rtell process \"Safari\"\rset value of attribute \"AXMinimized\" of window theIndex to false\rdelay 2\rtell application \"Safari\" to set theIndex to index of window id %@\rset theBoundsBefore to size of window theIndex\rclick button 2 of window theIndex\rset theBoundsAfter to size of window theIndex\rif (item 1 of theBoundsBefore is greater than or equal to item 1 of theBoundsAfter) and (item 2 of theBoundsBefore is greater than or equal to item 2 of theBoundsAfter) then\rtell application \"Safari\" to set theIndex to index of window id %@\rclick button 2 of window theIndex\rend if\rend tell\rend tell\rend tell", [currentWindowID retain],[currentWindowID retain], [currentWindowID retain]];
	}
	else if([winPosition isEqualToString:@"MINIMIZE"]) 
	{ 
		scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\"\ractivate\rset theIndex to index of window id %@\rtell application \"System Events\" to tell process \"Safari\" to set value of attribute \"AXMinimized\" of window theIndex to true\rend tell", [currentWindowID retain]];
	}
	else if([winPosition isEqualToString:@"BRINGTOTOP"]) 
	{
		scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\"\rset index of window id %@ to 1\rend tell",[currentWindowID retain]];
	}
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	
	[NSThread sleepForTimeInterval:1]; 
	
	[appleScript release];
	[scriptSource release];
	
	if (desc == nil)
		return @"failed";
	return @"OK";
}

-(NSString*)Sleep:(NSString*)miliseconds
{
	double timeInt = [miliseconds doubleValue] / 1000.00;
	[NSThread sleepForTimeInterval:timeInt];	
	return @"OK";
}

#pragma mark -
#pragma mark Helper Methods

- (NSString*)completePreviousProcess
{
	NSString *theResult;
	if (currentStatus == BrowserStatusNavigate || currentStatus == BrowserStatusRefresh || currentStatus == BrowserStatusStimulate)
	{
		[self pageHasLoaded];
	}
	else if (currentStatus == BrowserStatusKillAllExcept)
	{
		theResult = [self KillAllOpenBrowsers:notToCloseTitle];
		if (![theResult isEqualToString:@"JSDialog Appeared"])
			currentStatus = BrowserStatusNormal;
		return @"OK";
	}
	currentStatus = BrowserStatusNormal;
	return @"OK";
}

- (NSString*)doesProcessExist
{
	NSDictionary *scriptErrorBefore = [[NSDictionary alloc] init];
	NSString *scriptSourceBefore = [[NSString alloc] initWithString:@"tell application \"System Events\"\rif (name of processes) contains \"Safari\" then\rreturn \"YES\"\relse\rreturn \"NO\"\rend if\rend tell"];	
	NSAppleScript *appleScriptBefore = [[NSAppleScript alloc] initWithSource:scriptSourceBefore]; 
	NSAppleEventDescriptor *descBefore = [appleScriptBefore executeAndReturnError:&scriptErrorBefore];
	
	[appleScriptBefore release];
	[scriptSourceBefore release];
	
	if (descBefore != nil)
		return [descBefore stringValue];
	return nil;
}

- (NSString*)doesWindowExist
{
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\"\rset windowID to \"%@\"\rtry\rdo JavaScript \"document.readyState\" in document 1 of window id (windowID as number)\ron error\rreturn \"NO\"\rend try\rreturn \"OK\"\rend tell",[currentWindowID retain]];	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	[scriptSource release];
	[appleScript release];
	[currentWindowID release];
	
	if (desc == nil)
		return @"NO";
	return [desc stringValue];
}

- (BrowserAttached)isBrowserAttached
{
	if ([currentWindowID isEqualToString:@""])
		return BrowserAttachedNo;
	
	NSString *processResult = [self doesProcessExist];
	
	if (processResult == nil || [processResult isEqualToString:@"NO"] || [[self doesWindowExist] isEqualToString:@"NO"])
	{
		if (currentWindowID != nil)
			[currentWindowID release];
		currentWindowID = [[NSString alloc] initWithString:@""];
		return BrowserAttachedNo;
	}
	return BrowserAttachedYes;
}

-(NSString*)isCurrentBrowserAttached
{
	BrowserAttached browserIsAttached = [self isBrowserAttached];
	if (browserIsAttached == BrowserAttachedNo)
		return @"failed";
	return @"OK";
}

- (NSString*)pageHasLoaded
{
	NSDictionary *scriptError = [[NSDictionary alloc] init];
	NSString *scriptSource = [[NSString alloc] initWithFormat:@"tell application \"Safari\"\rset windowID to \"%@\"\rset theIndex to index of window id (windowID as number)\rtell application \"System Events\" to tell application process \"Safari\"\rset theTitle to role description of window theIndex\rif (theTitle contains \"dialog\") then\rreturn \"true\"\rend if\rdelay 0.5\rtry\rset a to get properties of button 1 of text field 1 of splitter group 1 of group 2 of tool bar 1 of window theIndex\ron error\rset a to get properties of button 1 of text field 1 of splitter group 1 of group 1 of tool bar 1 of window theIndex\rend try\rset b to description of a\rend tell\rend tell\rif b contains \"reload\" then\rreturn \"true\"\relse\rreturn \"false\"\rend if", [currentWindowID retain]];
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	NSAppleEventDescriptor *desc = [appleScript executeAndReturnError:&scriptError];
	
	[scriptSource release];
	[appleScript release];
	[currentWindowID release];
	
	if (desc == nil)
		return @"exception";
	return [desc stringValue];
}

@end