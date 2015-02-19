using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CocosSharp;
using Microsoft.Xna.Framework.Graphics;

namespace LevelHelper
{
	public class LHUtils
	{
		public LHUtils ()
		{
		}

		public static string imagePathWithFilename(string filename, string folder, string suffix)
		{
			return folder + suffix + "/" + filename;

//			string ext = Path.GetExtension (filename);
//			string fileNoExt = filename.Substring(0, filename.Length - ext.Length);
//
//			string folderWithFileNoExt = Path.Combine(folder, fileNoExt);
//
//			string folderWithFileAndSuffix = folderWithFileNoExt + suffix;
//
//			return Path.ChangeExtension (folderWithFileAndSuffix, ext);
		}

		public static CCPoint positionForNode(CCNode node, CCPoint unitPos)
		{
			LHScene scene = ((LHNodeProtocol)node).getScene ();

			CCSize designSize   = scene.designResolutionSize();
			CCPoint offset      = scene.designOffset();

			CCPoint designPos   = new CCPoint();

			if( node.Parent == null ||
				node.Parent == scene ||
				node.Parent == scene.getGameWorldNode() ||
				node.Parent == scene.getUINode()  ||
				node.Parent == scene.getBackUINode()
			)
			{
				designPos =new CCPoint(designSize.Width*unitPos.X, (designSize.Height - designSize.Height*unitPos.Y));
				designPos.X += offset.X;
				designPos.Y += offset.Y;
			}
			else{

				designPos = new CCPoint(designSize.Width*unitPos.X, node.Parent.ContentSize.Height - designSize.Height*unitPos.Y);
				CCNode p = node.Parent;
				designPos.X += p.ContentSize.Width*0.5f;
				designPos.Y -= p.ContentSize.Height*0.5f;
			}
					
			return designPos;
		}

		public static float LH_DEGREES_TO_RADIANS(float angle)
		{
			return angle*0.01745329252f;
		} 
			
		public static float LH_RADIANS_TO_DEGREES(float angle)
		{
			return angle*57.29577951f;
		} 

		public static String filenameExtension(String path)
		{
			Regex regex = new Regex(@"%.([^%.]+)$", RegexOptions.IgnoreCase);
			Match match = regex.Match(path);
			if (match.Success){
				return match.Value;
			}
			return "";
		}

		public static String stripExtension(String path)
		{
			Debug.WriteLine ("PATH:"+ path);
			Regex regex = new Regex(@"\.([A-Za-z0-9]+)$", RegexOptions.IgnoreCase);
			Match match = regex.Match(path);
			if (match.Success){
				Debug.WriteLine ("FOUND MATHCH FOR STRIP EXTENSION");

				return path.Substring(0, match.Index);
			}
			return "";
		}

		public static String folderFromPath(String path)
		{
			Regex regex = new Regex(@"^(.*[/\\])[^/\\]-$");
			Match match = regex.Match(path);
			if (match.Success){
				return match.Value;
			}
			return "";
		}

		public static String fileFromPath(String path)
		{
			Regex regex = new Regex(@"[\\/]([^/\\]+)$");
			Match match = regex.Match(path);
			if (match.Success){
				return match.Value;
			}
			return "";
		}

	}
}

