using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CocosSharp;

namespace GoneBananas
{
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
		CCDisplayOrientation PlatformSupportedOrientations()
		{
			return CCDisplayOrientation.LandscapeRight | CCDisplayOrientation.LandscapeLeft;
		}

        public override void FinishedLaunching (UIApplication app)
        {
            var application = new CCApplication ();
						 		
            application.ApplicationDelegate = new GoneBananasApplicationDelegate ();
            application.StartGame ();
        }
    }
}