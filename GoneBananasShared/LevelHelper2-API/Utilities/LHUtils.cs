using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using CocosSharp;

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


	}
}

