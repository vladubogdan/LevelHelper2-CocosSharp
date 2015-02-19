using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Box2D.Common;
using Box2D.Dynamics;
using CocosSharp;
using Box2D.Collision.Shapes;

namespace LevelHelper
{
	public class LHBodyShape
	{
		int _shapeID = 0;
		String _shapeName = "UntitledShape";

		static void LHSetupb2FixtureWithInfo(b2FixtureDef fixture, PlistDictionary dict)
		{
			fixture.density = dict ["density"].AsFloat;
			fixture.friction = dict ["friction"].AsFloat;
			fixture.restitution = dict ["restitution"].AsFloat;
			fixture.isSensor    = dict["sensor"].AsBool;

			fixture.filter.maskBits = (ushort)dict ["mask"].AsInt;
			fixture.filter.categoryBits = (ushort)dict ["category"].AsInt;
		}

		static bool LHValidateCentroid(b2Vec2[] vs, int count)
		{
//			if(count < 3 || count > 8)
//				return false;
//
//			int n = b2Min(count, 8);
//
//			// Perform welding and copy vertices into local buffer.
//			b2Vec2[] ps = new b2Vec2[b2_maxPolygonVertices];
//			int32 tempCount = 0;
//			for (int32 i = 0; i < n; ++i)
//			{
//				b2Vec2 v = vs[i];
//
//				bool unique = true;
//				for (int32 j = 0; j < tempCount; ++j)
//				{
//					if (b2DistanceSquared(v, ps[j]) < 0.5f * b2_linearSlop)
//					{
//						unique = false;
//						break;
//					}
//				}
//
//				if (unique)
//				{
//					ps[tempCount++] = v;
//				}
//			}
//
//			n = tempCount;
//			if (n < 3)
//			{
//				return false;
//			}

			return true;
		}



		public String shapeName(){
			return _shapeName;
		}
		public void setShapeName(string value){
			_shapeName = value;
		}

		public int shapeID(){
			return _shapeID;
		}
		public void setShapeID(int val){
			_shapeID = val;
		}


		public LHBodyShape ()
		{

		}
			
		public void createRectangleWithDictionary(PlistDictionary dict, b2Body body, CCNode node, LHScene scene, CCSize size)
		{
			_shapeID = dict ["shapeID"].AsInt;
			_shapeName = dict ["name"].AsString;

			b2PolygonShape shape = new b2PolygonShape ();

			shape.SetAsBox (size.Width * 0.5f, size.Height * 0.5f);

			b2FixtureDef fixture = new b2FixtureDef ();

			LHSetupb2FixtureWithInfo (fixture, dict);

			fixture.userData = this;
			fixture.shape = shape;

			body.CreateFixture (fixture);
		}				

		public void createCircleWithDictionary(PlistDictionary dict, b2Body body, CCNode node, LHScene scene, CCSize size)
		{
			_shapeID = dict ["shapeID"].AsInt;
			_shapeName = dict ["name"].AsString;

			b2CircleShape shape = new b2CircleShape ();

			shape.Radius = size.Width * 0.5f;

			b2FixtureDef fixture = new b2FixtureDef ();

			LHSetupb2FixtureWithInfo (fixture, dict);

			fixture.userData = this;
			fixture.shape = shape;

			body.CreateFixture (fixture);
		}

		public void createShapeWithDictionary (PlistDictionary dict, PlistArray shapePoints, b2Body body, CCNode node, LHScene scene, CCPoint scale)
		{
			_shapeID =dict ["shapeID"].AsInt;
			_shapeName = dict ["name"].AsString;

			int flipx = scale.X < 0 ? -1 : 1;
			int flipy = scale.Y < 0 ? -1 : 1;


			for(int f = 0; f < shapePoints.Count; ++f)
			{
				PlistArray fixPoints = shapePoints [f].AsArray;
				int count = fixPoints.Count;
				if(count > 2)
				{
					b2Vec2[] verts = new b2Vec2[count];
					b2PolygonShape shapeDef = new b2PolygonShape();

					int i = 0;
					for(int j = count-1; j >=0; --j)
					{

						int idx = (flipx < 0 && flipy >= 0) || (flipx >= 0 && flipy < 0) ? count - i - 1 : i;

						String pointStr = fixPoints [j].AsString;
						CCPoint point = CCPoint.Parse (pointStr);

						point.X *= scale.X;
						point.Y *= scale.Y;

						point.Y = -point.Y;

						b2Vec2 vec = new b2Vec2 (point.X, point.Y);

						verts[idx] = vec;
						++i;
					}

					if(LHValidateCentroid(verts, count))
					{
						shapeDef.Set(verts, count);

						b2FixtureDef fixture = new b2FixtureDef();

						LHSetupb2FixtureWithInfo(fixture, dict);

						fixture.userData = this;
						fixture.shape = shapeDef;
						body.CreateFixture(fixture);
					}
						
				}
			}
		}

		public void createEditorWithDictionary (PlistDictionary dict, b2Body body, CCNode node, LHScene scene, CCPoint scale)
		{
			_shapeID =dict ["shapeID"].AsInt;
			_shapeName = dict ["name"].AsString;

			int flipx = scale.X < 0 ? -1 : 1;
			int flipy = scale.Y < 0 ? -1 : 1;


			PlistArray fixtures = dict ["points"].AsArray;

			if (fixtures != null) 
			{
				for (int f = 0; f < fixtures.Count; ++f) 
				{
					PlistArray fixPoints = fixtures [f].AsArray;

					int count = fixPoints.Count;
					if (count > 2) {
						b2Vec2[] verts = new b2Vec2[count];
						b2PolygonShape shapeDef = new b2PolygonShape ();

						int i = 0;
						for (int j = count - 1; j >= 0; --j) {

							int idx = (flipx < 0 && flipy >= 0) || (flipx >= 0 && flipy < 0) ? count - i - 1 : i;

							String pointStr = fixPoints [j].AsString;
							CCPoint point = CCPoint.Parse (pointStr);

							point.X *= scale.X;
							point.Y *= scale.Y;

							point.Y = -point.Y;

							b2Vec2 vec = new b2Vec2 (point.X, point.Y);

							verts [idx] = vec;
							++i;
						}

						if (LHValidateCentroid (verts, count)) {
							shapeDef.Set (verts, count);

							b2FixtureDef fixture = new b2FixtureDef ();

							LHSetupb2FixtureWithInfo (fixture, dict);

							fixture.userData = this;
							fixture.shape = shapeDef;
							body.CreateFixture (fixture);
						}

					}
				}
			}
			else{
				float radius = dict ["radius"].AsFloat;
				String centerStr = dict ["center"].AsString;
				CCPoint point = CCPoint.Parse(centerStr);

				radius *= scale.X;

				point.X *= scale.X;
				point.Y *= scale.Y;

				point.Y = -point.Y;

				b2CircleShape shape = new b2CircleShape();
				shape.Radius = radius;
				shape.Position = new b2Vec2(point.X, point.Y);

				b2FixtureDef fixture = new b2FixtureDef ();

				LHSetupb2FixtureWithInfo(fixture, dict);

				fixture.userData = this;
				fixture.shape = shape;
				body.CreateFixture(fixture);
			}
		}




	}//LHBodyShape

}//namespace
