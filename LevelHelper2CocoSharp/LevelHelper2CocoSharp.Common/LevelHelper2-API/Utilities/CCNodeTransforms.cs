using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using CocosSharp;
using Microsoft.Xna.Framework.Graphics;
//using Foundation;

namespace LevelHelper
{
	public class CCNodeTransforms
	{
		public CCNodeTransforms ()
		{
		}

//		public static float GlobalXAngleFromLocalAngle(CCNode node, float localAngle)
//		{
//			CCNode prnt = node.Parent;
//			while(prnt != null && prnt.GetType() != typeof(CCScene))
//			{
//				localAngle += prnt.RotationX;
//				prnt = prnt.Parent;
//
//			}
//				
//			return localAngle;				
//		}

		public static float GlobalXAngleFromLocalAngle(CCNode node, float localAngle)
		{
			for(CCNode p = node.Parent; p != null && p.GetType() != typeof(CCScene); p = p.Parent)
			{
				localAngle += p.RotationX;
			}
			return localAngle;
		}


		public static float LocalXAngleFromGlobalAngle(CCNode node, float globalAngle)
		{
			for(CCNode p = node.Parent; p != null && p.GetType() != typeof(CCScene); p = p.Parent)
			{
				globalAngle -= p.RotationX;
			}
			return globalAngle;
		}


		public static CCPoint ConvertToWorldScale(CCNode node, CCPoint nodeScale)
		{
			for(CCNode p = node.Parent; p != null && p.GetType() != typeof(CCScene); p = p.Parent)
			{
				nodeScale.X *= p.ScaleX;
				nodeScale.Y *= p.ScaleY;
			}
			return nodeScale;
		}

	}
}

