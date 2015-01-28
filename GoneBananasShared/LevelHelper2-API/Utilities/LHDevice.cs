using System;
using CocosSharp;
using System.Collections;
using System.Collections.Generic;

namespace LevelHelper
{
	internal class LHDevice
	{
		CCSize  size;
		float   ratio;
		String 	suffix;


		public LHDevice (PlistDictionary dict)
		{
			size    = CCSize.Parse(dict["size"].AsString);
			suffix 	= dict ["suffix"].AsString;
			ratio   = dict["ratio"].AsFloat;
		}
			
		public CCSize getSize(){return size;}
		public String getSuffix(){return suffix;}
		public float getRatio(){return ratio;}


		public static CCSize LH_SCREEN_RESOLUTION(CCWindow window)
		{
			return window.WindowSizeInPixels;
		}



		public static LHDevice deviceFromArrayWithSize(List<LHDevice> devices, CCSize size)
		{
			foreach (LHDevice dev in devices) {

				if (dev.getSize ().Equals (size) || 
					(dev.getSize().Width == size.Height && dev.getSize().Height == size.Width)) 
				{
					return dev;
				}
			}

			if(devices.Count > 0){
				return devices[0];
			}

			return null;
		}

		public static LHDevice currentDeviceFromArray(List<LHDevice> devices, CCWindow window)
		{
			return LHDevice.deviceFromArrayWithSize(devices, LHDevice.LH_SCREEN_RESOLUTION(window));
		}

		public static LHDevice currentDeviceFromArray(List<LHDevice> devices, CCScene scene)
		{
			return LHDevice.currentDeviceFromArray (devices, scene.Window);
		}


		public static String devicePosition(PlistDictionary availablePositions, CCSize curScr)
		{					
			string w = Convert.ToInt32(curScr.Width).ToString();
			string h = Convert.ToInt32(curScr.Height).ToString();
			string key = w + "x" + h;

			Console.WriteLine ("devicePosition key is " + key);

			return availablePositions.TryGetValue(key).AsString;
		}			
	}
}

