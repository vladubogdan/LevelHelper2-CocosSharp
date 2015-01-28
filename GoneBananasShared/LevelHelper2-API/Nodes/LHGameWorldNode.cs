using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using CocosSharp;

namespace LevelHelper
{
	public class LHGameWorldNode : CCLayer, LHNodeProtocol
	{		
		LHNodeProtocolImp _nodeProtocolImp = new LHNodeProtocolImp();

		public LHGameWorldNode (PlistDictionary dict, CCNode prnt) : base()
		{
			Console.WriteLine ("DID LOAD GAME WORLD NODE");

			prnt.AddChild (this);

			_nodeProtocolImp.loadGenericInfoFromDictionary (dict, this);

			this.ZOrder = 0;
			this.Position = new CCPoint ();

			LHNodeProtocolImp.loadChildrenForNode (this, dict);
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

