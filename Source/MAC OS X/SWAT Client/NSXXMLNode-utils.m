//
//  NSXXMLNode-utils.m
//  SWAT Client
//
//  Created by Jared Solomon on 10/7/08.
//  Copyright 2008 __MyCompanyName__. All rights reserved.
//

@implementation NSXMLNode(utils)

- (NSXMLNode *)childNamed:(NSString *)name
{
	NSEnumerator *e = [[self children] objectEnumerator];
	
	NSXMLNode *node;
	while (node = [e nextObject]) 
		if ([[node name] isEqualToString:name])
			return node;
    
	return nil;
}

- (NSArray *)childrenAsStrings
{
	NSMutableArray *ret = [[NSMutableArray arrayWithCapacity:
							[[self children] count]] retain];
	NSEnumerator *e = [[self children] objectEnumerator];
	NSXMLNode *node;
	while (node = [e nextObject])
		[ret addObject:[node stringValue]];
	
	return [ret autorelease];
}

@end
