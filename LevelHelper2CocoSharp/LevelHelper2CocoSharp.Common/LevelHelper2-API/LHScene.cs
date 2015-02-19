using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;


using Box2D.Common;
using Box2D.Dynamics;
using CocosSharp;


namespace LevelHelper
{

	public class LHScene : CCScene, LHNodeProtocol
	{
		public CCSize _designResolutionSize;
		List<LHDevice> devices = new List<LHDevice>();
		LHDevice currentDev = null;
		Dictionary<string, CCTexture2D> loadedTextures =  null;
		Dictionary<string, Dictionary<string, PlistDictionary> > editorBodiesInfo = null;
		CCPoint _designOffset = new CCPoint();
		LHNodeProtocolImp _nodeProtocolImp = new LHNodeProtocolImp();
		LHBackUINode _backUINode = null;
		LHGameWorldNode _gameWorldNode = null;
		LHUINode _uiNode = null;
		PlistDictionary tracedFixtures = null;

		public string relativePath { get; set; }

		//--------------------------------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------------------------
		public LHScene (PlistDictionary dict, string plistLevelFile, CCWindow mainWindow) : base (mainWindow)
		{

			_designResolutionSize = CCSize.Parse(dict ["designResolution"].AsString);

			foreach (PlistDictionary devInf in dict["devices"].AsArray)
			{
				devices.Add (new LHDevice (devInf));
			}				
			currentDev = LHDevice.currentDeviceFromArray(devices, this);
			CCSize sceneSize = currentDev.getSize();
			float ratio = currentDev.getRatio();
			sceneSize.Width = sceneSize.Width/ratio;
			sceneSize.Height = sceneSize.Height/ratio;

			var aspect = dict ["aspect"].AsInt;
			if(aspect == 0)//exact fit
			{
				sceneSize = _designResolutionSize;
			}
			else if(aspect == 1)//no borders
			{
			}
			else if(aspect == 2)//show all
			{
				_designOffset.X = (sceneSize.Width  - _designResolutionSize.Width)*0.5f;
				_designOffset.Y = (sceneSize.Height - _designResolutionSize.Height)*0.5f;
			}


			//loadingInProgress = true;
			Debug.WriteLine ("plistLevelFile:|" + plistLevelFile + "|");

			this.relativePath = LHUtils.folderFromPath (plistLevelFile);
//			this.relativePath = Path.GetDirectoryName (plistLevelFile);

			Debug.WriteLine ("SCENE REL |" + this.relativePath + "|");
//			Console.WriteLine ("SCENE REL |" + this.relativePath + "|");

			if(this.relativePath == null){
				this.relativePath = "";
			}

//			loadingInProgress = true;

//			[[CCDirector sharedDirector] setContentScaleFactor:ratio];
//			#if __CC_PLATFORM_IOS
//			[[CCFileUtils sharedFileUtils] setiPhoneContentScaleFactor:curDev.ratio];
//			#endif

//			[self setName:relativePath];

			_nodeProtocolImp.loadGenericInfoFromDictionary(dict, this);

//			self.contentSize= CGSizeMake(curDev.size.width/curDev.ratio, curDev.size.height/curDev.ratio);
//			self.position   = CGPointZero;

			PlistDictionary tracedFixInfo = dict["tracedFixtures"].AsDictionary;
			if(tracedFixInfo != null){
				tracedFixtures = new PlistDictionary();
				foreach (var pair in tracedFixInfo)
				{
					string fixUUID = pair.Key;
					PlistArray fixInfo = pair.Value.AsArray;
					if(null != fixInfo)
					{
						tracedFixtures.Add (fixUUID, fixInfo);
					}
				}
			}

//			supportedDevices = [[NSArray alloc] initWithArray:devices];

//			[self loadBackgroundColorFromDictionary:dict];
//			[self loadGameWorldInfoFromDictionary:dict];



			LHNodeProtocolImp.loadChildrenForNode (this, dict);
		
//			[self loadGlobalGravityFromDictionary:dict];
//			[self loadPhysicsBoundariesFromDictionary:dict];

//			[self setUserInteractionEnabled:YES];

//			#if __CC_PLATFORM_IOS
//			pinchRecognizer = [[UIPinchGestureRecognizer alloc] initWithTarget:self action:@selector(pinch:)];
//			[[[CCDirector sharedDirector] view] addGestureRecognizer:pinchRecognizer];
//			#endif



//			#if LH_USE_BOX2D
//			_box2dCollision = [[LHBox2dCollisionHandling alloc] initWithScene:self];
//			#else//cocos2d

//			#endif        
//			[self performLateLoading];

//			loadingInProgress = false;

			Debug.WriteLine ("SCENE has children count " + this.ChildrenCount);
		}
		//--------------------------------------------------------------------------------------------------------------
		public static LHScene createWithContentOfFile(string plistLevelFile, CCWindow mainWindow, CCApplication application)
		{

			PlistDictionary dict = CCContentManager.SharedContentManager.Load<PlistDocument> (plistLevelFile).Root.AsDictionary;

			if(null == dict){
				Debug.WriteLine("\nERROR: Could not load level file %s. The file does not appear to exist.\n", plistLevelFile);
				return null;
			}

			var aspect = dict ["aspect"].AsInt;
			if(aspect == 0)//exact fit
			{
				CCSize winSize = CCSize.Parse(dict ["designResolution"].AsString);
				CCScene.SetDefaultDesignResolution(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ExactFit);
			}
			else if(aspect == 1)//no borders
			{
				CCSize winSize = CCSize.Parse(dict ["designResolution"].AsString);
				CCScene.SetDefaultDesignResolution(winSize.Width, winSize.Height, CCSceneResolutionPolicy.NoBorder);
			}
			else if(aspect == 2)//show all
			{
				CCSize winSize = mainWindow.WindowSizeInPixels;
				CCScene.SetDefaultDesignResolution(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ShowAll);
			}

			PlistArray devsInfo = dict["devices"].AsArray;
			if(null == devsInfo){
				Debug.WriteLine("\nERROR: Level doesn't contain valid devices.\n");
				return null;
			}

			foreach (PlistDictionary devInf in devsInfo)
			{
				string suffix 	= dict ["suffix"].AsString;
				application.ContentSearchPaths.Add(suffix);
			}

			application.ContentSearchPaths.Add ("hd");
			application.ContentSearchPaths.Add ("568");
				
			return new LHScene (dict, plistLevelFile, mainWindow);
		}	
		//--------------------------------------------------------------------------------------------------------------

		public LHBackUINode getBackUINode()
		{
			if(null == _backUINode)
			{
				foreach (CCNode child in this.Children) {
					if (child.GetType () == typeof(LHBackUINode)) {
						_backUINode = (LHBackUINode)child;
						break;
					}
				}					
			}
			return _backUINode;
		}
		public LHGameWorldNode getGameWorldNode()
		{
			if(null == _gameWorldNode)
			{
				foreach (CCNode child in this.Children) {
					if (child.GetType () == typeof(LHGameWorldNode)) {
						_gameWorldNode = (LHGameWorldNode)child;
						break;
					}
				}					
			}
				
			return _gameWorldNode;
		}
		public LHUINode getUINode()
		{
			if(null == _uiNode)
			{
				foreach (CCNode child in this.Children) {
					if (child.GetType () == typeof(LHUINode)) {
						_uiNode = (LHUINode)child;
						break;
					}
				}					
			}
				
			return _uiNode;
		}

		public b2World getBox2dWorld()
		{
			return this.getGameWorldNode ().box2dWorld ();
		}
			
		public float ptm(){
			return 32.0f;
		}

		public b2Vec2 metersFromPoint(CCPoint point){
			return new b2Vec2(point.X/this.ptm(), point.Y/this.ptm());
		}
		public CCPoint pointFromMeters(b2Vec2 vec){
			return new CCPoint(vec.x*this.ptm(), vec.y*this.ptm());
		}
		public float metersFromValue(float val){
			return val/this.ptm();
		}
		public float valueFromMeters(float meter){
			return meter*this.ptm();
		}



		public float currentDeviceRatio(){

			if (currentDev != null) {
				return currentDev.getRatio ();
			}

			return 1.0f;
		}

		public CCSize designResolutionSize(){
			return _designResolutionSize;
		}
		public CCSize currentDeviceSize(){
			return LHDevice.LH_SCREEN_RESOLUTION(this.Window);
		}
		public CCPoint designOffset(){
			return _designOffset;
		}

		public string currentDeviceSuffix(bool keep2x)
		{
			CCSize scrSize = LHDevice.LH_SCREEN_RESOLUTION(this.Window);

			foreach(LHDevice dev in devices){
				CCSize devSize = dev.getSize();

				if (devSize.Equals (scrSize) || 
					(devSize.Width == scrSize.Height && devSize.Height == scrSize.Width)) 
				{
					string suffix = dev.getSuffix ();

					if(false == keep2x)
					{
						suffix = suffix.Replace ("2x", "");
					}
					return suffix;
				}					
			}
			return "";
		}

		public CCTexture2D textureWithImagePath(string imagePath)
		{
			if(null == loadedTextures){
				loadedTextures =  new Dictionary<string, CCTexture2D>();
			}
				
			if(imagePath != null && imagePath.Length > 0)
			{
				if (loadedTextures.ContainsKey (imagePath)) {
					return loadedTextures [imagePath];
				}
				else{
					CCTexture2D texture = new CCTexture2D (imagePath);
					if(texture != null){
						loadedTextures.Add (imagePath, texture);
						return texture;
					}
				}
			}
			return null;
		}

		public void setEditorBodyInfoForSpriteName(string sprName, string atlasPlist, PlistDictionary bodyInfo)
		{
			if(null == editorBodiesInfo){
				editorBodiesInfo = new Dictionary<string, Dictionary<string, PlistDictionary> >();
			}

			if(null == bodyInfo || null == sprName || null == atlasPlist)return;

			Dictionary<string, PlistDictionary> imagesDict = null;

			if(editorBodiesInfo.ContainsKey(atlasPlist))
			{
				imagesDict = editorBodiesInfo[atlasPlist];
			}
			else{
				imagesDict = new Dictionary<string, PlistDictionary> ();
			}

			if(imagesDict.ContainsKey(sprName) == false)
			{
				imagesDict.Add(sprName, bodyInfo);
				editorBodiesInfo.Remove (atlasPlist);
				editorBodiesInfo.Add (atlasPlist, imagesDict);
			}
		}

		public PlistArray tracedFixturesWithUUID(String uuid)
		{
			return tracedFixtures[uuid].AsArray;
		}

		public PlistDictionary getEditorBodyInfoForSpriteName(string sprName, string atlasPlist)
		{
			Debug.WriteLine ("TRY TO GET BODY INFO FOR " + sprName + " atlas " + atlasPlist);


			if(null == atlasPlist || null == sprName)return null;

			if(editorBodiesInfo.ContainsKey(atlasPlist))
			{
				Dictionary<string, PlistDictionary> spritesInf = editorBodiesInfo[atlasPlist];
				if(spritesInf != null){
					if(spritesInf.ContainsKey(sprName))
					{
						return spritesInf[sprName];
					}
				}
			}
			return null;
		}
		public bool hasEditorBodyInfoForImageFilePath(string atlasImgFile)
		{		
			if (null == editorBodiesInfo)return false;
			if(null == atlasImgFile)return false;
			return editorBodiesInfo.ContainsKey (atlasImgFile);
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
			return this;
		}
	}
}

