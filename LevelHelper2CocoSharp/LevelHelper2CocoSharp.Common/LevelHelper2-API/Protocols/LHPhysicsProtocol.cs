using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Box2D.Common;
using Box2D.Dynamics;
using CocosSharp;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace LevelHelper
{
	public interface LHPhysicsProtocol
	{
	 	void updatePosition(CCPoint position);
	 	void updateRotation(float rotation);
	}

	public class LHPhysicsProtocolImp
	{
		CCNode _node = null;
		b2Body _body = null;
		CCPoint previousScale;
		List<LHBodyShape> _subShapes = new List<LHBodyShape>();
		bool scheduledForRemoval = false;

		public LHPhysicsProtocolImp ()
		{

		}

		public void removeBody()
		{

		}

		public void visit()
		{
			if(_body != null && scheduledForRemoval == true){
				this.removeBody();
			}

			if(_body != null)
			{
				CCAffineTransform trans = b2BodyToParentTransform(_node, this);
				CCPoint localPos = CCAffineTransform.Transform (_node.AnchorPointInPoints, trans);
				((LHPhysicsProtocol)_node).updatePosition (localPos);
				((LHPhysicsProtocol)_node).updateRotation ( CCNodeTransforms.LocalXAngleFromGlobalAngle(_node, LHUtils.LH_RADIANS_TO_DEGREES(-_body.Angle)));
			}
		}

		CCAffineTransform absoluteTransform() 
		{
			CCAffineTransform transform = CCAffineTransform.Identity;

			LHScene scene = ((LHNodeProtocol)_node).getScene();

			b2Vec2 b2Pos = _body.Position;
			CCPoint globalPos = new CCPoint(b2Pos.x, b2Pos.y);//[scene pointFromMeters:b2Pos];

			transform = CCAffineTransform.Translate(transform, globalPos.X, globalPos.Y);
			transform = CCAffineTransform.Rotate(transform, _body.Angle);


			transform = CCAffineTransform.Translate(transform, - _node.ContentSize.Width*0.5f, - _node.ContentSize.Height*0.5f);

			return transform;
		}

		static CCAffineTransform b2BodyToParentTransform(CCNode node, LHPhysicsProtocolImp physicsImp)
		{
			return CCAffineTransform.Concat(physicsImp.absoluteTransform(), CCAffineTransform.Invert(NodeToB2BodyTransform(node.Parent)));
		}

		static CCAffineTransform NodeToB2BodyTransform(CCNode node)
		{
			CCAffineTransform transform = CCAffineTransform.Identity;

			for(CCNode n = node; 
				n != null &&  n.GetType() != typeof(LHGameWorldNode)
//				&& ![n isKindOfClass:[LHBackUINode class]]
//				&& ![n isKindOfClass:[LHUINode class]]
				;
				n = n.Parent)
			{			
				transform = CCAffineTransform.Concat(transform, n.AffineLocalTransform);
			}				
			return transform;
		}

			
		public void loadPhysicsInfoFromDictionary(PlistDictionary dict, CCNode nd)
		{
			_node = nd;

			if(null != dict)
			{	
				int shapeType = dict ["shape"].AsInt;
				int type = dict ["type"].AsInt;

				LHScene scene = ((LHNodeProtocol)_node).getScene();

				b2World world = scene.getBox2dWorld();

				var bodyDef = new b2BodyDef ();
				bodyDef.type = (b2BodyType)type;

				CCPoint position = _node.Parent.ConvertToWorldspace (_node.Position);
//				bodyDef.position = scene.metersFromPoint (position);
				bodyDef.position = new b2Vec2(position.X, position.Y);

				float angle = CCNodeTransforms.GlobalXAngleFromLocalAngle(_node, _node.RotationX);
				bodyDef.angle = LHUtils.LH_DEGREES_TO_RADIANS (angle);

				bodyDef.userData = _node;

				_body = world.CreateBody (bodyDef);
				_body.UserData = _node;


				Debug.WriteLine ("BODY:" + _body);


				_body.SetFixedRotation (dict ["fixedRotation"].AsBool);
				//_body->SetGravityScale

				_body.SetSleepingAllowed (dict ["allowSleep"].AsBool);
				_body.SetBullet (dict ["bullet"].AsBool);

				_body.AngularDamping = dict ["angularDamping"].AsFloat;
				_body.AngularVelocity = -360.0f * dict ["angularVelocity"].AsFloat;
				_body.LinearDamping = dict ["linearDamping"].AsFloat;

				CCPoint linearVel = CCPoint.Parse (dict ["linearVelocity"].AsString);
				_body.LinearVelocity = new b2Vec2 (linearVel.X, linearVel.Y);


				CCSize size = _node.ContentSize;
//				size.Width = scene.metersFromValue (size.Width);
//				size.Height = scene.metersFromValue (size.Height);

				CCPoint scale = new CCPoint (_node.ScaleX, _node.ScaleY);
				scale = CCNodeTransforms.ConvertToWorldScale (_node, scale);

				previousScale = scale;


				size.Width *= scale.X;
				size.Height *= scale.Y;


				PlistDictionary fixInfo = dict ["genericFixture"].AsDictionary;

				Debug.WriteLine ("FIX INFO " + fixInfo);

				Debug.WriteLine ("SHAPE TYPE " + shapeType);

				if(shapeType == 0)//RECTANGLE
				{
					LHBodyShape shape = new LHBodyShape();
					shape.createRectangleWithDictionary (fixInfo, _body, _node, scene, size);

					_subShapes.Add (shape);
				}
				else if(shapeType == 1)//CIRCLE
				{
					LHBodyShape shape = new LHBodyShape();
					shape.createCircleWithDictionary (fixInfo, _body, _node, scene, size);

					_subShapes.Add (shape);
				}
				else if(shapeType == 4)//oval
				{
					PlistArray shapePoints = dict ["ovalShape"].AsArray;
					if(shapePoints != null)
					{
						LHBodyShape shape = new LHBodyShape ();
						shape.createShapeWithDictionary (fixInfo, shapePoints, _body, _node, scene, scale);
						_subShapes.Add (shape);
					}						
				}
				else if(shapeType == 5)//traced
				{
					String fixUUID = dict ["fixtureUUID"].AsString;

					Debug.WriteLine ("TRACED " + fixUUID);

					PlistArray shapePoints = scene.tracedFixturesWithUUID (fixUUID);

					Debug.WriteLine ("RETURNS " + shapePoints);

					if(shapePoints == null)
					{
						//CHECK IN ASSET
						//LHAsset asset = _node.assetParent;
						///

					}	

					if(shapePoints != null)
					{
						LHBodyShape shape = new LHBodyShape ();

						Debug.WriteLine ("WE HAVE A TRACED SHAPE");

						shape.createShapeWithDictionary (fixInfo, shapePoints, _body, _node, scene, scale);

						_subShapes.Add (shape);
					}
				}
				else if(shapeType == 6)//editor
				{
					LHSprite sprite = (LHSprite)_node;


					if(sprite != null && sprite.GetType() == typeof(LHSprite))
					{

						String imageFile = sprite.getImageFilePath ();

						imageFile = LHUtils.stripExtension (imageFile);

						Debug.WriteLine ("WE HAVE AN EDITOR SHAPE for sprite " + sprite + " node " + _node + " tst " + imageFile);


						PlistDictionary bodyInfo = scene.getEditorBodyInfoForSpriteName (sprite.getSpriteFrameName (), imageFile );

						Debug.WriteLine ("WE HAVE BODY INFO " + bodyInfo);

						if(bodyInfo != null)
						{
							PlistArray fixturesInfo = bodyInfo ["shapes"].AsArray;

							for(int i = 0; i < fixturesInfo.Count; ++i)
							{
								PlistDictionary shapeInfo = fixturesInfo [i].AsDictionary;

								Debug.WriteLine ("SHAPE INFO " + shapeInfo);

								LHBodyShape shape = new LHBodyShape ();

								shape.createEditorWithDictionary (shapeInfo, _body, _node, scene, scale);

								_subShapes.Add (shape);
							}
						}

					}
				}

//				if (dict.ContainsKey ("alpha")) {
//					_node.Opacity = (byte)dict ["alpha"].AsFloat;
//				}
			
			}  
		}				
	}//LHPhysicsProtocolImp

}//namespace
