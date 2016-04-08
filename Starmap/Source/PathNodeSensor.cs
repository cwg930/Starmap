using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Starmap
{
	public class PathNodeSensor : Sensor
	{
		private List<Tile> tilesInRange;

		#region Properties
		public List<Tile> TilesInRange
		{
			get { return tilesInRange; }
			set { }
		}
		#endregion

		#region Constructors
		public PathNodeSensor(Unit owner, float range) : base (owner, range){
			tilesInRange = new List<Tile> ();
		}
		#endregion

		#region Methods
		//Finds all tiles in range of owner
		public void Update ()
		{
			tilesInRange.Clear ();
			if (Screen.Instance.GetType() == typeof(GameplayScreen)) {
				foreach (Tile a in (Screen.Instance as GameplayScreen).GetGrid().GetTileList()) {
					if (Vector2.Distance (owner.Position, a.Center) <= range && a.TileStatus != Tile.Status.Closed) {
						tilesInRange.Add (a);
					}
				}
			}
		}

		public Tile ClosestTile()
		{
			float min = 100000f;
			Tile closest = null;
			foreach (Tile item in tilesInRange)
			{
				if(Vector2.Distance(item.Center, owner.Position) < min)
				{
					min = Vector2.Distance(item.Center, owner.Position);
					closest = item;
				}
			}

			return closest;
		}
		#endregion
	}
}