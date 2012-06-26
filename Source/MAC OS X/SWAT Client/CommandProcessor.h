//
//  CommandProcessor.h
//  SWAT Client
//
//  Last Modified by Brandon Foote 6/29/10
//  Copyright 2008 __MyCompanyName__. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "SimpleCocoaServer.h"
#import "Safari.h"
#import "BrowserController.h"
#import "SettingsWindow.h"

@interface CommandProcessor : NSObject {
	@private
	
	SimpleCocoaServer *server;	
	BrowserController *controller;
	NSMutableString *messageBuffer;
	
	NSWindow *aboutWindow;
	SettingsWindow *settingsWindow;
	NSMenu *menu;
}

@property(nonatomic,retain) IBOutlet NSWindow *aboutWindow;
@property(nonatomic,retain) IBOutlet NSMenu *menu;
@property(nonatomic,retain) IBOutlet SettingsWindow *settingsWindow;

-(IBAction)quit:(id)sender;
-(IBAction)showAbout:(id)sender;
-(IBAction)showSettings:(id)sender;
-(IBAction)closeSettings:(id)sender;
-(IBAction)applyScreenShotSettings:(id)sender;
-(IBAction)applyTimeoutSettings:(id)sender;

@end
