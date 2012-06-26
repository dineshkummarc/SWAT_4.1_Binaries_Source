//
//  SettingsTableDataSource.h
//  SWAT Client
//
//  Created by Brandon Foote 6/29/10
//  Copyright 2010 __MyCompanyName__. All rights reserved.
//
#import "SettingsWindow.h"


@interface SettingsWindow (TableView)

- (int)numberOfRowsInTableView:(NSTableView *)aTableView;

- (id)tableView:(NSTableView *)aTableView
objectValueForTableColumn:(NSTableColumn *)aTableColumn
			row:(int)rowIndex;

- (BOOL)tableView:(NSTableView *)aTableView shouldSelectRow:(NSInteger)rowIndex;
@end

