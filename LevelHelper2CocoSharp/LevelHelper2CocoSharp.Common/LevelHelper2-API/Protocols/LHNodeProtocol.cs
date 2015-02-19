using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using CocosSharp;

namespace LevelHelper
{
	public interface LHNodeProtocol // Defines a set of functionality that a class must implement.
	{
		/**
 		Returns the unique name of the node.
 		*/
		string getName ();

		/**
 		Returns the unique identifier of the node.
 		*/
		string getUuid();

		/**
 		Returns all tag values of the node.
 		*/
		//-(NSArray*)tags;

		/**
 		Returns the user property object assigned to this object or nil.
 		*/
		//-(id<LHUserPropertyProtocol>)userProperty;

		/**
 		Returns the scene to which this node belongs to.
 		*/
		LHScene getScene();
	}

	public class LHNodeProtocolImp
	{
		public string name;
		public string uuid;

		bool b2WorldDirty = false;
		CCNode _node = null;

		public LHNodeProtocolImp ()
		{

		}
			
		public static void loadChildrenForNode(CCNode prntNode, PlistDictionary dict)
		{
			PlistArray childrenInfo = dict["children"].AsArray;
			if(null != childrenInfo)
			{
				foreach(PlistDictionary childInfo in childrenInfo)
				{
					/*CCNode node =*/ LHNodeProtocolImp.createLHNodeWithDictionary(childInfo, prntNode);				
				}
			}
		}

		public void loadGenericInfoFromDictionary(PlistDictionary dict, CCNode nd)
		{
			b2WorldDirty = false;
			_node = nd;

			if(null != dict)
			{
				name = dict ["name"].AsString;
				uuid = dict ["uuid"].AsString;

				//tags loading
//				{
//					NSArray* loadedTags = [dict objectForKey:@"tags"];
//					if(loadedTags){
//						_tags = [[NSMutableArray alloc] initWithArray:loadedTags];
//					}
//				}

				//user properties loading
//				{
//					NSDictionary* userPropInfo  = [dict objectForKey:@"userPropertyInfo"];
//					NSString* userPropClassName = [dict objectForKey:@"userPropertyName"];
//					if(userPropInfo && userPropClassName)
//					{
//						Class userPropClass = NSClassFromString(userPropClassName);
//						if(userPropClass){
//							#pragma clang diagnostic push
//							#pragma clang diagnostic ignored "-Wundeclared-selector"
//							_userProperty = [userPropClass performSelector:@selector(customClassInstanceWithNode:)
//								withObject:_node];
//							#pragma clang diagnostic pop
//							if(_userProperty){
//								[_userProperty setPropertiesFromDictionary:userPropInfo];
//							}
//						}
//					}
//				}

				if (dict.ContainsKey ("alpha")) {
					_node.Opacity = (byte)dict ["alpha"].AsFloat;
				}

				if (dict.ContainsKey ("rotation")) {
					_node.Rotation = - dict ["rotation"].AsFloat;
				}

				if (dict.ContainsKey ("zOrder")) {
					_node.ZOrder = dict ["zOrder"].AsInt;
				}

				if (dict.ContainsKey ("scale")) {
				
					CCPoint scl = CCPoint.Parse (dict ["scale"].AsString);
					_node.ScaleX = scl.X;
					_node.ScaleY = scl.Y;
				}

				//for sprites the content size is set from the CCSpriteFrame
				if (dict.ContainsKey ("size") && _node.GetType () != typeof(LHSprite)) {
					_node.ContentSize = CCSize.Parse (dict ["size"].AsString);
				}
					
				if (dict.ContainsKey ("generalPosition")
					&&
					_node.GetType() != typeof(LHUINode)
					&&
					_node.GetType() != typeof(LHBackUINode)
					&&
					_node.GetType() != typeof(LHGameWorldNode))
				{
					CCPoint unitPos = CCPoint.Parse (dict ["generalPosition"].AsString);
					CCPoint pos = LHUtils.positionForNode(_node, unitPos);

					_node.Position = pos;
				}
//				if([dict objectForKey:@"generalPosition"]&&
//					![_node isKindOfClass:[LHUINode class]] &&
//					![_node isKindOfClass:[LHBackUINode class]] &&
//					![_node isKindOfClass:[LHGameWorldNode class]])
//				{
//
//					CGPoint unitPos = [dict pointForKey:@"generalPosition"];
//					CGPoint pos = [LHUtils positionForNode:_node
//						fromUnit:unitPos];
//
//					NSDictionary* devPositions = [dict objectForKey:@"devicePositions"];
//					if(devPositions)
//					{
//
//						NSString* unitPosStr = [LHUtils devicePosition:devPositions
//							forSize:LH_SCREEN_RESOLUTION];
//
//						if(unitPosStr){
//							CGPoint unitPos = LHPointFromString(unitPosStr);
//							pos = [LHUtils positionForNode:_node
//								fromUnit:unitPos];
//						}
//					}
//
//					[_node setPosition:pos];
//				}

//				if([dict objectForKey:@"anchor"] &&
//					![_node isKindOfClass:[LHUINode class]] &&
//					![_node isKindOfClass:[LHBackUINode class]] &&
//					![_node isKindOfClass:[LHGameWorldNode class]])
//				{
//					CGPoint anchor = [dict pointForKey:@"anchor"];
//					anchor.y = 1.0f - anchor.y;
//					[_node setAnchorPoint:anchor];
//				}
			}  
		}



		public static CCNode createLHNodeWithDictionary(PlistDictionary childInfo, CCNode prnt)
		{
			string nodeType = childInfo["nodeType"].AsString;

			LHScene scene = ((LHNodeProtocol)prnt).getScene();

			string subclassNodeType = childInfo["subclassNodeType"].AsString;
			if(subclassNodeType != null && subclassNodeType.Length > 0)
			{
				//TODO handle subclasses

				//this will not work as we do not have the class included in the api
//				Class classObj = [scene createNodeObjectForSubclassWithName:subclassNodeType superTypeName:nodeType];
//				if(classObj){
//					return [classObj nodeWithDictionary:childInfo parent:prnt];
//				}
//				else{
//					NSLog(@"\n\nWARNING: Expected a class of type %@ subclassed from %@, but nothing was returned. Check your \"createNodeObjectForSubclassWithName:superTypeName:\" method and make sure you return a valid Class.\n\n", subclassNodeType, nodeType);
//				}
			}

			if(nodeType == "LHGameWorldNode")			
			{
				LHGameWorldNode pNode = LHGameWorldNode.nodeWithDictionary(childInfo, prnt);
				pNode.ContentSize = scene._designResolutionSize;
//				#if LH_DEBUG
//				[pNode setDebugDraw:YES];
//				#endif
				return pNode;
			}
			else if(nodeType == "LHBackUINode")
			{
				LHBackUINode pNode = LHBackUINode.nodeWithDictionary(childInfo, prnt);
				return pNode;
			}
			else if(nodeType == "LHUINode")
			{
				LHUINode pNode = LHUINode.nodeWithDictionary(childInfo, prnt);
				return pNode;
			}
			else if(nodeType == "LHSprite")
			{
				LHSprite spr = LHSprite.nodeWithDictionary(childInfo, prnt);
				return spr;
			}
//			else if([nodeType isEqualToString:@"LHNode"])
//			{
//				LHNode* nd = [LHNode nodeWithDictionary:childInfo
//					parent:prnt];
//				return nd;
//			}
//			else if([nodeType isEqualToString:@"LHBezier"])
//			{
//				LHBezier* bez = [LHBezier nodeWithDictionary:childInfo
//					parent:prnt];
//				return bez;
//			}
//			else if([nodeType isEqualToString:@"LHTexturedShape"])
//			{
//				LHShape* sp = [LHShape nodeWithDictionary:childInfo
//					parent:prnt];
//				return sp;
//			}
//			else if([nodeType isEqualToString:@"LHWaves"])
//			{
//				LHWater* wt = [LHWater nodeWithDictionary:childInfo
//					parent:prnt];
//				return wt;
//			}
//			else if([nodeType isEqualToString:@"LHAreaGravity"])
//			{
//				LHGravityArea* gv = [LHGravityArea nodeWithDictionary:childInfo
//					parent:prnt];
//				return gv;
//			}
//			else if([nodeType isEqualToString:@"LHParallax"])
//			{
//				LHParallax* pr = [LHParallax nodeWithDictionary:childInfo
//					parent:prnt];
//				return pr;
//			}
//			else if([nodeType isEqualToString:@"LHParallaxLayer"])
//			{
//				LHParallaxLayer* lh = [LHParallaxLayer nodeWithDictionary:childInfo
//					parent:prnt];
//				return lh;
//			}
//			else if([nodeType isEqualToString:@"LHAsset"])
//			{
//				LHAsset* as = [LHAsset nodeWithDictionary:childInfo
//					parent:prnt];
//				return as;
//			}
//			else if([nodeType isEqualToString:@"LHCamera"])
//			{
//				LHCamera* cm = [LHCamera nodeWithDictionary:childInfo
//					parent:prnt];
//				return cm;
//			}
//			else if([nodeType isEqualToString:@"LHRopeJoint"])
//			{
//				LHRopeJointNode* jt = [LHRopeJointNode nodeWithDictionary:childInfo
//					parent:prnt];
//				return jt;
//			}
//			else if([nodeType isEqualToString:@"LHWeldJoint"])
//			{
//				LHWeldJointNode* jt = [LHWeldJointNode nodeWithDictionary:childInfo
//					parent:prnt];
//				return jt;
//			}
//			else if([nodeType isEqualToString:@"LHRevoluteJoint"]){
//
//				LHRevoluteJointNode* jt = [LHRevoluteJointNode nodeWithDictionary:childInfo
//					parent:prnt];
//				return jt;
//			}
//			else if([nodeType isEqualToString:@"LHDistanceJoint"]){
//
//				LHDistanceJointNode* jt = [LHDistanceJointNode nodeWithDictionary:childInfo
//					parent:prnt];
//				return jt;
//
//			}
//			else if([nodeType isEqualToString:@"LHPulleyJoint"]){
//
//				LHPulleyJointNode* jt = [LHPulleyJointNode nodeWithDictionary:childInfo
//					parent:prnt];
//				return jt;
//			}
//			else if([nodeType isEqualToString:@"LHPrismaticJoint"]){
//
//				LHPrismaticJointNode* jt = [LHPrismaticJointNode nodeWithDictionary:childInfo
//					parent:prnt];
//				return jt;
//			}
//			else if([nodeType isEqualToString:@"LHWheelJoint"]){
//
//				LHWheelJointNode* jt = [LHWheelJointNode nodeWithDictionary:childInfo
//					parent:prnt];
//				return jt;
//			}
//			else if([nodeType isEqualToString:@"LHGearJoint"]){
//
//				LHGearJointNode* jt = [LHGearJointNode nodeWithDictionary:childInfo
//					parent:prnt];
//				return jt;
//			}
//
//
//			else{
//				NSLog(@"UNKNOWN NODE TYPE %@", nodeType);
//			}

			return null;
		}



	}//LHNodeProtocolImp
}//namespace
