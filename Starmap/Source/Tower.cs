using System;
using Microsoft.Xna.Framework;

namespace Starmap
{
	public abstract class Tower : Agent
	{
		#region Fields
		private int damage;
		private Vector2 position;
		private float heading;
		private AdjacentAgentSensor AASensor;
		#endregion

		#region Properties
		public int Damage { get { return damage; } }
		public Vector2 Position { get { return position; } }
		public float Heading { get { return heading; } }
		public Vector2 HeadingVector { get { return new Vector2 (Math.Sin (heading), Math.Cos (heading)); } }
		#endregion

		public Tower ()
		{
		}
	}
}

