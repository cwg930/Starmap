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
		public PathNodeSensor(Game1 game, Unit owner, float range) : base (game, owner, range){
			tilesInRange = new List<Tile> ();
		}
		#endregion

		#region Methods
		//Finds all tiles in range of owner
		public override void Update ()
		{
			tilesInRange.Clear ();
			foreach (Tile a in game.GetGrid().GetTileList())
			{
				if (Vector2.Distance (owner.Position, a.Center) <= range && a.TileStatus != Tile.Status.Closed) 
				{
					tilesInRange.Add (a);
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