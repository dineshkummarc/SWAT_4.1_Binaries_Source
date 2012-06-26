//
//  CommandProcessor.m
//  SWAT Client
//
//  Last Modified by Brandon Foote 6/29/10
//  Copyright 2008 __MyCompanyName__. All rights reserved.
//

#import "CommandProcessor.h"
#import "SimpleCocoaServer.h"
#import "Safari.h"
#import "BrowserController.h"

@implementation CommandProcessor
@synthesize aboutWindow;
@synthesize menu;
@synthesize settingsWindow;

-(void)awakeFromNib
{
	NSStatusItem *statusItem = [[[NSStatusBar systemStatusBar] statusItemWithLength:NSVariableStatusItemLength] retain];
	//[statusItem setTitle:@"Swat"];
	[statusItem setImage:[NSImage imageNamed:@"quitswat.png"]];
	[menu setAutoenablesItems:NO];
	[statusItem setMenu:menu];
	[statusItem setHighlightMode:NO];
	//[statusItem setTarget:self];
	//[statusItem setAction:@selector(Quit)];
}

-(IBAction)quit:(id)sender
{
	exit(0);
}

-(IBAction)showAbout:(id)sender
{
	//[NSApp runModalForWindow:aboutWindow];
	NSString *scriptSource = [[NSString alloc] initWithString:@"tell application \"SWAT Client\" to activate"];	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	[appleScript executeAndReturnError:nil];
	[appleScript release];
	[scriptSource release];
	[aboutWindow makeKeyAndOrderFront:aboutWindow];
}

-(IBAction)showSettings:(id)sender
{
	//[NSApp runModalForWindow:screenShotsWindow];
	NSString *scriptSource = [[NSString alloc] initWithString:@"tell application \"SWAT Client\" to activate"];	
	NSAppleScript *appleScript = [[NSAppleScript alloc] initWithSource:scriptSource]; 
	[appleScript executeAndReturnError:nil];
	[appleScript release];
	[scriptSource release];
	[settingsWindow.thePathOfScreenShots setStringValue:[[NSUserDefaults standardUserDefaults] objectForKey:@"ScreenShotsLocation"]];
	[settingsWindow.waitForNetworkTextBox setStringValue:[[NSUserDefaults standardUserDefaults] objectForKey:@"WaitForNetwork"]];
	[settingsWindow makeKeyAndOrderFront:settingsWindow];
}

-(IBAction)closeSettings:(id)sender
{
	[settingsWindow performClose:settingsWindow];
}

-(IBAction)applyScreenShotSettings:(id)sender
{
	[[NSUserDefaults standardUserDefaults] setObject:[settingsWindow.thePathOfScreenShots stringValue] forKey:@"ScreenShotsLocation"];
	[[NSUserDefaults standardUserDefaults] synchronize];
	controller.screenshotPath = [[NSString alloc] initWithString:[[settingsWindow.thePathOfScreenShots stringValue] retain]];
	[self closeSettings:nil];
}

-(IBAction)applyTimeoutSettings:(id)sender
{
	[[NSUserDefaults standardUserDefaults] setObject:[settingsWindow.waitForNetworkTextBox stringValue] forKey:@"WaitForNetwork"];
	[[NSUserDefaults standardUserDefaults] synchronize];
	controller.waitForNetworkTimeout = [[NSString alloc] initWithString:[[settingsWindow.waitForNetworkTextBox stringValue] retain]];
	[self closeSettings:nil];
}

-(id)init {
	if(self = [super init]) {

		if ([[NSUserDefaults standardUserDefaults] objectForKey:@"ScreenShotsLocation"] == nil)
		{
			[[NSUserDefaults standardUserDefaults] setObject:@"/Library/Swat ScreenShots/" forKey:@"ScreenShotsLocation"];
		}
		
		if ([[NSUserDefaults standardUserDefaults] objectForKey:@"WaitForNetwork"] == nil)
		{
			[[NSUserDefaults standardUserDefaults] setObject:@"20" forKey:@"WaitForNetwork"];
		}
		
		controller = [[BrowserController alloc] init];
		
		controller.screenshotPath = [[NSString alloc] initWithString:[[NSUserDefaults standardUserDefaults] objectForKey:@"ScreenShotsLocation"]];
		controller.waitForNetworkTimeout = [[NSString alloc] initWithString:[[NSUserDefaults standardUserDefaults] objectForKey:@"WaitForNetwork"]];
		
		
		server = [[SimpleCocoaServer alloc] initWithPort:9999 delegate:self];
		
		[server startListening];
		
		messageBuffer = [NSMutableString stringWithCapacity:1000];
		//[messageBuffer setString:@""];
		
		[messageBuffer retain];
	}
	
	return self;
}


- (void)fireCommand:(NSString *)message fromConnection:(SimpleCocoaConnection *)conn  {
	[conn retain];
	NSLog(@"'%@' received from client: %@", message, conn);
	
	NSString *result;
	
	if ([message rangeOfString:@"KillAllOpenBrowsers^-~"].location != NSNotFound)
	{
		result = [controller KillAllOpenBrowsers];
	}
	else
	{
		NSString *cleanedMessage = [message substringToIndex:[message length] - 3];
		NSArray *messageParts = [cleanedMessage componentsSeparatedByString:@"~-^"];
		NSString *selectorName = [messageParts objectAtIndex:0];
		NSString *param = nil;
		NSString *param2 = nil;
		
		if([messageParts count] > 1)
		{
			param = [messageParts objectAtIndex:1];
		}
		
		if([messageParts count] > 2)
		{
			param2 = [messageParts objectAtIndex:2];
		}
		
		//MAY HAVE TO USE NSInvocation if need arrises for more then two params.
		
		SEL selectorCall = NSSelectorFromString(selectorName);
		result = @"Invalid Command";
		
		if([controller respondsToSelector:selectorCall])
		{
			if(param != nil)
			{
				if(param2 != nil)
					result = [controller performSelector:selectorCall withObject:param withObject:param2];
				else
					result = [controller performSelector:selectorCall withObject:param];
			}
			else
				result = [controller performSelector:selectorCall];
		}
	}
	NSLog(@"'%@' sent to client: %@", result, conn);
	
	[server sendString:result toConnection:conn]; 
	[conn release];
	//[server startListening];
}

- (void)processMessage:(NSString *)message fromConnection:(SimpleCocoaConnection *)conn  {
    
	//messageBuffer = [messageBuffer stringByAppendingString:[message substringToIndex:[message length] - 2]];
	NSString* cleanMsg = message;
	if([cleanMsg hasSuffix:@"\n\r"])
	{
		cleanMsg = [cleanMsg substringToIndex:[cleanMsg rangeOfString:@"\n\r" options: NSBackwardsSearch].location];
	}
	
	
	[messageBuffer appendString:cleanMsg];
	
	if([messageBuffer hasSuffix:@"^-~"])
	{
		[self fireCommand:messageBuffer fromConnection:conn];
		//[server sendStringToAll:result];
		[messageBuffer setString:@""];
	}
	
}


/*
 
 
 SafariApplication * safari = [SBApplication 
 applicationWithBundleIdentifier:@"com.apple.Safari"];
 
 [safari activate];
 
 SBElementArray *windows = [safari windows];
 SafariWindow * window = [windows objectAtIndex:0];
 SafariTab * wikipediaTab = [[WindowHelper alloc] getTabByTitle:window title:@"Google"];
 
 
 NSLog(@"Found tab: %@", [wikipediaTab name]);
 
 [safari doJavaScript:@"document.getElementsByName('q')[0].value = 'Jared';" in:wikipediaTab];
 [safari doJavaScript:@"document.getElementsByName('btnG')[0].click();" in:wikipediaTab];
 
 
 
 
 
 */


@end
