using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using CocosSharp;

namespace LevelHelper
{
	public class LHSprite : CCSprite, LHNodeProtocol, LHPhysicsProtocol
	{		
		string _imageFilePath;
		string _spriteFrameName;

		LHNodeProtocolImp _nodeProtocolImp = new LHNodeProtocolImp();

		LHPhysicsProtocolImp _physicsProtocolImp = new LHPhysicsProtocolImp();


		public LHSprite (CCSpriteFrame spriteFrame, PlistDictionary dict, CCNode prnt, String spriteFrameName, String imageDevPath) : base(spriteFrame)
		{	
			Debug.WriteLine ("DID INIT SPRITE WITH FRAME " + spriteFrame);

			this._spriteFrameName = spriteFrameName;
			this._imageFilePath = imageDevPath;


			prnt.AddChild (this);

//				[self setColor:[dict colorForKey:@"colorOverlay"]];
//
			_nodeProtocolImp.loadGenericInfoFromDictionary (dict, this);

			Debug.WriteLine ("LOADING PHYSICS...................");
			Debug.WriteLine ("SPR NAME " + this.getSpriteFrameName());
			Debug.WriteLine ("IMG NAME " + this.getImageFilePath());
			Debug.WriteLine (dict);

			_physicsProtocolImp.loadPhysicsInfoFromDictionary (dict["nodePhysics"].AsDictionary, this);


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
			LHScene scene = ((LHNodeProtocol)prnt).getScene ();

			string imageFileName = dict ["imageFileName"].AsString;
			string relativeImagePath = dict ["relativeImagePath"].AsString;


//			string imagePath = LHUtils.imagePathWithFilename (imageFileName, relativeImagePath, scene.currentDeviceSuffix (false));
//			string imageDevPath = LHUtils.imagePathWithFilename (imageFileName, relativeImagePath, scene.currentDeviceSuffix (true));

			string imagePath = LHUtils.imagePathWithFilename (imageFileName, "", scene.currentDeviceSuffix (false));
			string imageDevPath = LHUtils.imagePathWithFilename (imageFileName, "", scene.currentDeviceSuffix (true));


			CCTexture2D texture = scene.textureWithImagePath (imagePath);

			CCSpriteFrame spriteFrame = null;

			string imageFilePath = imageDevPath;
			string spriteFrameName = dict["spriteName"].AsString;

			if(spriteFrameName != null){

				LHSprite.cacheSpriteFramesInfo (imageDevPath, scene);
				spriteFrame = CCSpriteFrameCache.SharedSpriteFrameCache [spriteFrameName];
			}
			else{

				//spriteFrame = [texture createSpriteFrame];
			}

			LHSprite spr = new LHSprite (spriteFrame, dict, prnt, spriteFrameName, imageDevPath);


			return spr;
		}

		static void cacheSpriteFramesInfo(string imageDevPath, LHScene scene)
		{
			string sceneRelative = scene.relativePath;
			string curDevSuffix  = scene.currentDeviceSuffix(true);

			string atlasName = LHUtils.stripExtension (imageDevPath);
//			string atlasName = Path.GetFileNameWithoutExtension(imageDevPath);

			string atlasPlist = atlasName + ".plist";
//			string atlasPlist = Path.ChangeExtension (atlasName, "plist");

			string sceneSuf = sceneRelative + curDevSuffix;


//			atlasPlist = sceneSuf + atlasPlist;

//			Debug.WriteLine ("atlasPlist");
//			Debug.WriteLine (atlasPlist);


//			string sceneSuf = Path.Combine (sceneRelative, curDevSuffix);
//			atlasPlist = Path.Combine (sceneSuf, atlasPlist);



			CCSpriteFrameCache cache = CCSpriteFrameCache.SharedSpriteFrameCache;
			cache.AddSpriteFrames (atlasPlist);


			atlasName = sceneRelative + atlasName;
//			atlasName = Path.Combine (sceneRelative, atlasName);

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
							Debug.WriteLine ("CACHING BODY " + sprName + " atlas " + atlasName + " body " + bodyInfo);

							scene.setEditorBodyInfoForSpriteName (sprName, atlasName, bodyInfo);
						}
					}
				}
			}
		}

		public String getSpriteFrameName()
		{
			Debug.WriteLine ("REDING SPRITE FRAME ANEM " + _spriteFrameName);

			return this._spriteFrameName;
		}

		public String getImageFilePath()
		{
			return this._imageFilePath;
		}

		public override void Visit ()
		{
			if(_physicsProtocolImp != null){
				_physicsProtocolImp.visit ();
			}

			base.Visit ();

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

		//LHPhysicsProtocol methods
		public void updatePosition(CCPoint position)
		{
			base.Position = position;
		}
		public void updateRotation(float rotation)
		{
			base.Rotation = rotation;
		}


	}
}

