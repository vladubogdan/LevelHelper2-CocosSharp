using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using CocosSharp;

namespace LevelHelper
{
	public class LHSprite : CCSprite, LHNodeProtocol
	{		
		string _imageFilePath;
		string _spriteFrameName;

		LHNodeProtocolImp _nodeProtocolImp = new LHNodeProtocolImp();

		public LHSprite (CCSpriteFrame spriteFrame, PlistDictionary dict, CCNode prnt) : base(spriteFrame)
		{	
			Console.WriteLine ("DID INIT SPRITE WITH FRAME " + spriteFrame);

			Console.WriteLine (spriteFrame.IsRotated);
			Console.WriteLine (spriteFrame.TextureFilename);
			Console.WriteLine (spriteFrame.ContentSize.Width);
			Console.WriteLine (spriteFrame.ContentSize.Height);
			Console.WriteLine (spriteFrame.Texture);


//			this.PositionX = 150;
//			this.PositionY = 100;
//			this.Opacity = 255;
//			this.Color = new CCColor3B (255, 255, 255);

			prnt.AddChild (this);

			Console.WriteLine ("did sprite inside parent " + prnt);

			//			if(self = [super initWithSpriteFrame:spriteFrame]){
//
//				[prnt addChild:self];
//
//				[self setColor:[dict colorForKey:@"colorOverlay"]];
//
			_nodeProtocolImp.loadGenericInfoFromDictionary (dict, this);

//
//				_physicsProtocolImp = [[LHNodePhysicsProtocolImp alloc] initPhysicsProtocolImpWithDictionary:[dict objectForKey:@"nodePhysics"]
//					node:self];
//
//
			LHNodeProtocolImp.loadChildrenForNode (this, dict);				
//
//
//				_animationProtocolImp = [[LHNodeAnimationProtocolImp alloc] initAnimationProtocolImpWithDictionary:dict
//					node:self];        
//			}
//

		}

		public static LHSprite nodeWithDictionary(PlistDictionary dict, CCNode prnt)
		{
			Console.WriteLine ("DID LOAD SPRITE");

			LHScene scene = ((LHNodeProtocol)prnt).getScene ();

			string imageFileName = dict ["imageFileName"].AsString;
			string relativeImagePath = dict ["relativeImagePath"].AsString;

			string imagePath = LHUtils.imagePathWithFilename (imageFileName, relativeImagePath, scene.currentDeviceSuffix (false));

			string imageDevPath = LHUtils.imagePathWithFilename (imageFileName, relativeImagePath, scene.currentDeviceSuffix (true));

			Console.WriteLine (imageDevPath);
			Console.WriteLine (imagePath);

			CCTexture2D texture = scene.textureWithImagePath (imagePath);

			CCSpriteFrame spriteFrame = null;

			string imageFilePath = imageDevPath;
			string spriteFrameName = dict["spriteName"].AsString;

			Console.WriteLine ("SPRITE FRAME NAME " + spriteFrameName);

			if(spriteFrameName != null){

				LHSprite.cacheSpriteFramesInfo (imageDevPath, scene);
				spriteFrame = CCSpriteFrameCache.SharedSpriteFrameCache [spriteFrameName];
			}
			else{

				//spriteFrame = [texture createSpriteFrame];
			}

			LHSprite spr = new LHSprite (spriteFrame, dict, prnt);

			return spr;
		}

		static void cacheSpriteFramesInfo(string imageDevPath, LHScene scene)
		{
			string sceneRelative = scene.relativePath;

			string atlasName = Path.GetFileNameWithoutExtension(imageDevPath);

			Console.WriteLine ("atlasName");
			Console.WriteLine (atlasName);


			string atlasPlist = Path.ChangeExtension (atlasName, "plist");
			atlasPlist = Path.Combine (sceneRelative, atlasPlist);

			Console.WriteLine ("atlasPlist");
			Console.WriteLine (atlasPlist);

			CCSpriteFrameCache cache = CCSpriteFrameCache.SharedSpriteFrameCache;
			cache.AddSpriteFrames (atlasPlist);


			atlasName = Path.Combine (sceneRelative, atlasName);

			if(false == scene.hasEditorBodyInfoForImageFilePath(atlasName))
			{
				string path = CCFileUtils.FullPathFromRelativePath (atlasPlist);
				PlistDocument document = CCContentManager.SharedContentManager.Load<PlistDocument>(path);
				PlistDictionary dict = document.Root.AsDictionary;

				PlistDictionary framesDict = dict ["frames"].AsDictionary;

				foreach (var pair in framesDict)
				{
					string sprName = pair.Key;
					PlistDictionary frmInfo = pair.Value.AsDictionary;
									
					if(null != frmInfo)
					{
						PlistDictionary bodyInfo = frmInfo ["body"].AsDictionary;

						if(null != bodyInfo)
						{
							scene.setEditorBodyInfoForSpriteName (sprName, atlasName, bodyInfo);
						}
					}
				}
			}
		}

		//LHNodeProtocol methods
		//-------------------------------------------------------------------------------------------
		/**
 		Returns the unique name of the node.
 		*/
		public string getName(){
			return _nodeProtocolImp.name;
		}
		/**
 		Returns the unique identifier of the node.
 		*/
		public string getUuid(){
			return _nodeProtocolImp.uuid;
		}

		/**
 		Returns the scene to which this node belongs to.
 		*/
		public LHScene getScene(){
			return ((LHNodeProtocol)(this.Parent)).getScene();
		}

	}
}

