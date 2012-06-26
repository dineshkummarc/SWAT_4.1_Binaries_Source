//
//  BrowserController.h
//  SWAT Client
//
//  Last Modified by Brandon Foote 6/29/10
//  Copyright 2008 __MyCompanyName__. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "Safari.h"

typedef enum {
	BrowserStatusNormal = 0,
	BrowserStatusNavigate = 1,
	BrowserStatusRefresh = 2,
	BrowserStatusStimulate = 3,
	BrowserStatusKillAllExcept = 4
} BrowserStatus;


typedef enum {
	BrowserAttachedNo = 0,
	BrowserAttachedYes = 1
} BrowserAttached;

@interface BrowserController : NSObject {
	NSString *currentWindowID;
	NSString *screenshotPath;
	BrowserStatus currentStatus;
	NSString *notToCloseTitle;
	NSString *waitForNetworkTimeout;
	NSString *currentlyAttachedNonBrowserApplication;
}

@property(nonatomic,retain) NSString *screenshotPath;
@property(nonatomic,retain) NSString *waitForNetworkTimeout;

-(id)init;

-(NSString*)OpenBrowser;
-(NSString*)CloseBrowser;
-(NSString*)KillAllOpenBrowsers;
-(NSString*)KillAllOpenBrowsers:(NSString*)title;
-(NSString*)NavigateBrowser:(NSString*)url;
-(NSString*)Sleep:(NSString*)miliseconds;
-(NSString*)AttachToWindow:(NSString*)windowTitle windowIndex:(NSString*)windowIndex;
-(NSString*)DoJavascript:(NSString*)javascriptToExecute;
-(NSString*)DoAppleScript:(NSString*)applescriptToExecute;
-(NSString*)ClickJSDialogOk;
-(NSString*)SetWindowPosition:(NSString*)winPosition;
-(NSString*)AssertTopWindow:(NSString*)windowIndex;
-(NSString*)ClickJSDialogCancel;
-(NSString*)AssertBrowserDoesOrDoesNotExist:(NSString*)windowTitle;
-(NSString*)ClickJSDialog:(NSString*)buttonText;
-(NSString*)StimulateElement:(NSString*)javascriptToExecute;
-(NSString*)AssertJSDialogContent;
-(NSString*)AssertJSDialogExists;
-(NSString*)PressKeys:(NSString*)word;
-(NSString*)PressKeyCode:(NSString*)keycode;
-(NSString*)PressModifiedKeyCode:(NSString*)modifier withKeyCode:(NSString*)keyCode;
-(NSString*)PressKeyCombination:(NSString*)modifier withCharacter:(NSString*)character;
-(NSString*)TakeScreenshot:(NSString*)windowOrScreen withPrefix:(NSString*)filePrefix;
-(NSString*)SetElementAttribute:(NSString*)javascriptToExecute;
-(NSString*)GetElementAttribute:(NSString*)javascriptToExecute;
-(NSString*)AssertElementIsActive:(NSString*)javascriptToExecute;
-(NSString*)SetFileInput:(NSString*)filePath;

- (NSString*)getWindowScreenshot:(NSString*)filePrefix;
- (NSString*)getDesktopScreenshot:(NSString*)filePrefix;
- (NSString*)saveScreenShot:(NSBitmapImageRep*)bitmapRep withPrefix:(NSString*)filePrefix;

//Helper Methods
- (NSString*)doesProcessExist;
- (NSString*)pageHasLoaded;
- (NSString*)completePreviousProcess;
- (NSString*)doesWindowExist;
- (BrowserAttached)isBrowserAttached;
- (NSString*)isCurrentBrowserAttached;
- (NSString*)executeJavascript:(NSString*)javascriptToExecute;

@end
