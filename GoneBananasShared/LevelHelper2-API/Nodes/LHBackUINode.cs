using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using CocosSharp;

namespace LevelHelper
{
	public class LHBackUINode : CCLayer, LHNodeProtocol
	{		
		LHNodeProtocolImp _nodeProtocolImp = new LHNodeProtocolImp();

		public LHBackUINode (PlistDictionary dict, CCNode prnt) : base()
		{
			Console.WriteLine ("DID LOAD BACK UI NODE");

			prnt.AddChild (this);

			_nodeProtocolImp.loadGenericInfoFromDictionary (dict, this);

			this.ZOrder = -1;
			this.Position = new CCPoint ();

			LHNodeProtocolImp.loadChildrenForNode (this, dict);
		}

		public static LHBackUINode nodeWithDictionary(PlistDictionary dict, CCNode prnt)
		{
			return new LHBackUINode (dict, prnt);
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

