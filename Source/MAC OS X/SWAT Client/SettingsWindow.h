//
//  SettingsWindow.h
//  SWAT Client
//
//  Created by Brandon Foote 6/29/10
//  Copyright 2010 __MyCompanyName__. All rights reserved.
//

#import <Cocoa/Cocoa.h>


@interface SettingsWindow : NSWindow {
	NSTextField *thePathOfScreenShots;
	NSTextField *waitForNetworkTextBox;
	NSView *screenshotsView;
	NSView *timeoutSettingsView;
}
@property(nonatomic,retain) IBOutlet NSTextField *thePathOfScreenShots;
@property(nonatomic,retain) IBOutlet NSView *screenshotsView;
@property(nonatomic,retain) IBOutlet NSView *timeoutSettingsView;
@property(nonatomic,retain) IBOutlet NSTextField *waitForNetworkTextBox;

@end
