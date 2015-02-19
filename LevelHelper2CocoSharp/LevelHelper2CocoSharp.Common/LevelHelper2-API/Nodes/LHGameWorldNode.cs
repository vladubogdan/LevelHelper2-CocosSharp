using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Box2D.Common;
using Box2D.Dynamics;
using CocosSharp;
using CocosSharp;

namespace LevelHelper
{
	public class LHGameWorldNode : CCLayer, LHNodeProtocol
	{		
		LHNodeProtocolImp _nodeProtocolImp = new LHNodeProtocolImp();
		b2World _world = null;
		LHBox2dDraw _debugDraw = null;

		public LHGameWorldNode (PlistDictionary dict, CCNode prnt) : base()
		{
			Debug.WriteLine ("DID LOAD GAME WORLD NODE");

			prnt.AddChild (this);

			_nodeProtocolImp.loadGenericInfoFromDictionary (dict, this);

			this.ZOrder = 0;
			this.Position = new CCPoint ();

			LHNodeProtocolImp.loadChildrenForNode (this, dict);
		}
				
		public b2World box2dWorld()
		{
			if (null == _world) {
				var gravity = new b2Vec2 (0.0f, -10.0f);
				_world = new b2World (gravity);

				_world.SetAllowSleeping (true);
				_world.SetContinuousPhysics (true);


				_debugDraw = new LHBox2dDraw("fonts/MarkerFelt-16");
				_world.SetDebugDraw(_debugDraw);
				_debugDraw.AppendFlags(b2DrawFlags.e_shapeBit);

				Schedule (t => {
					_world.Step (t, 8, 1);
				});

			}

			return _world;
		}

		protected override void Draw()
		{
			base.Draw();

			if (_debugDraw != null)
			{
				_debugDraw.Begin();
				_world.DrawDebugData();
				_debugDraw.End();
			}
		}


		public static LHGameWorldNode nodeWithDictionary(PlistDictionary dict, CCNode prnt)
		{
			return new LHGameWorldNode (dict, prnt);
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

