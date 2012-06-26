//
//  SettingsTableDataSource.m
//  SWAT Client
//
//  Created by Brandon Foote 6/29/10
//  Copyright 2010 __MyCompanyName__. All rights reserved.
//

#import "SettingsTableDataSource.h"


@implementation SettingsWindow (TableView)

- (int)numberOfRowsInTableView:(NSTableView *)aTableView
{
	return 2;	
}

- (id)tableView:(NSTableView *)aTableView objectValueForTableColumn:(NSTableColumn *)aTableColumn row:(int)rowIndex
{
	if (rowIndex == 0)
	{
		return 	@"Screenshots";
	}
	else {
		return 	@"Timeout Settings";
	}
	
}

- (BOOL)tableView:(NSTableView *)aTableView shouldSelectRow:(NSInteger)rowIndex
{
	if (rowIndex == 0)
	{
		[self.screenshotsView setHidden:NO];
		[self.timeoutSettingsView setHidden:YES];
	}
	else {
		[self.screenshotsView setHidden:YES];
		[self.timeoutSettingsView setHidden:NO];
	}
	return YES;
}

@end
