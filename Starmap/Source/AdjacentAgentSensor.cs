using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Starmap
{
	public class AdjacentAgentSensor : Sensor
	{
		#region Properties
		public Dictionary<Unit, Tuple<float, float>> AgentsInRange { get; }
		#endregion

		#region Constructors
		public AdjacentAgentSensor(Tower owner, float range) : base (owner, range){
			AgentsInRange = new Dictionary<Unit, Tuple<float, float>>();
		}
		#endregion

		#region Methods
		//Finds all agents in range of owner then calculates distance and relative heading
		public void Update (List<Agent> agents)
		{
			AgentsInRange.Clear ();
			foreach (Unit u in agents){
				if (Vector2.Distance (owner.Position, u.Position) <= range) {
					Vector2 v = u.Position - owner.Position;
					//cross product used to determine if target is cw/ccw from heading
					Vector3 crossResult = Vector3.Cross (new Vector3(owner.HeadingVector.X, owner.HeadingVector.Y, 0), new Vector3(v.X, v.Y, 0));
					//dot product of 2 unit vectors = cosine of angle between them
					float relativeHeading = (float)Math.Acos(Vector2.Dot(Vector2.Normalize(owner.HeadingVector), Vector2.Normalize(v)));
					relativeHeading = MathHelper.ToDegrees (relativeHeading);
					//make all angles out of 360 for consistency
					if (crossResult.Z < 0) {
						relativeHeading *= -1;
						relativeHeading += 360;
					}
					AgentsInRange.Add (u, new Tuple<float, float>(Vector2.Distance(owner.Position, u.Position), relativeHeading));
				}
			}
		}
		#endregion
	}
}