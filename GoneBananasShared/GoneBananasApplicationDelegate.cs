using CocosDenshion;
using CocosSharp;
using System;
using System.IO;

using LevelHelper;

namespace GoneBananas
{
    public class GoneBananasApplicationDelegate : CCApplicationDelegate
    {





        public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
        {
            application.PreferMultiSampling = false;
            application.ContentRootDirectory = "Content";

            application.ContentSearchPaths.Add("hd");




//            CCSimpleAudioEngine.SharedEngine.PreloadEffect ("Sounds/tap");
//            CCSimpleAudioEngine.SharedEngine.PreloadBackgroundMusic ("Sounds/backgroundMusic");
            
//			CCSize winSize = mainWindow.WindowSizeInPixels;

//			winSize.Width = 640;
//			winSize.Height = 960;

//            mainWindow.SetDesignResolutionSize(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ShowAll);

			//mainWindow.SupportedDisplayOrientations = CCDisplayOrientation.LandscapeLeft | CCDisplayOrientation.LandscapeRight;
			//mainWindow.SupportedDisplayOrientations = CCDisplayOrientation.Portrait | CCDisplayOrientation.LandscapeLeft;



			LHScene scene = LHScene.createWithContentOfFile ("example.lhplist", mainWindow, application);

//			CCScene scene = new CCScene (mainWindow);
//
//			CCLayer layer = new CCLayer ();
//
//			var label = new CCLabelTtf("Tap Screen to Go Bananas!", "arial", 22) {
//				Position = new CCPoint(250,150),
//				Color = CCColor3B.Green,
//				HorizontalAlignment = CCTextAlignment.Center,
//				VerticalAlignment = CCVerticalTextAlignment.Center,
//				AnchorPoint = CCPoint.AnchorMiddle
//			};
//
//			layer.AddChild (label);
//			scene.AddChild (layer);


//			Console.WriteLine ("DID INIT LH SCENE " + scene);

//            CCScene scene = GameStartLayer.GameStartLayerScene(mainWindow);
            mainWindow.RunWithScene (scene);
        }

        public override void ApplicationDidEnterBackground (CCApplication application)
        {
            // stop all of the animation actions that are running.
            application.Paused = true;

            // if you use SimpleAudioEngine, your music must be paused
            CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic ();
        }

        public override void ApplicationWillEnterForeground (CCApplication application)
        {
            application.Paused = false;

            // if you use SimpleAudioEngine, your background music track must resume here. 
            CCSimpleAudioEngine.SharedEngine.ResumeBackgroundMusic ();
        }
    }
}