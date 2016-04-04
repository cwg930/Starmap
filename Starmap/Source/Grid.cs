using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public class Grid
	{
		#region Fields
		private Game1 game;
		private List<Tile> tileList;
		private Texture2D openTileTexture;
		private Texture2D closedTileTexture;
		private Texture2D pathTileTexture;
		private Tile[,] grid;
		private bool debugDrawEnabled;
		private int tileWidth;
		#endregion

		#region Properties
		public int TileWidth
		{
			get { return tileWidth; }
			private set { }
		}
		#endregion

		#region Constructors
		public Grid (Game1 game, int tileWidth, int drawPadding)
		{
			this.game = game;
			this.tileWidth = tileWidth;

			Color[] data = new Color[(tileWidth-drawPadding) * (tileWidth-drawPadding)];

			// Build texture for non-obstructed tiles (Green)
			for (int i = 0; i < data.Length; i++)
				data [i] = Color.Green;
			openTileTexture = new Texture2D (game.GraphicsDevice, (tileWidth-drawPadding), (tileWidth-drawPadding));
			openTileTexture.SetData (data);

			// Build texture for obstructed tiles (Red)
			for (int i = 0; i < data.Length; i++)
				data [i] = Color.Red;
			closedTileTexture = new Texture2D (game.GraphicsDevice, (tileWidth-drawPadding), (tileWidth-drawPadding));
			closedTileTexture.SetData (data);

			// Build texture for path element tiles (Yellow)
			for (int i = 0; i < data.Length; i++)
				data [i] = Color.Yellow;
			pathTileTexture = new Texture2D (game.GraphicsDevice, (tileWidth-drawPadding), (tileWidth-drawPadding));
			pathTileTexture.SetData (data);

			// Create a new grid of navigation tiles.
			grid = new Tile[game.GraphicsDevice.DisplayMode.Width / tileWidth, game.GraphicsDevice.DisplayMode.Height / tileWidth];
			tileList = new List<Tile> ();
			for (int i = 0; i < grid.GetLength(0); i++)
			{
				for (int j = 0; j < grid.GetLength(1); j++)
				{
					grid [i, j] = new Tile (new Point (i*tileWidth, j*tileWidth), tileWidth, 2);
					tileList.Add (grid [i, j]);
				}
			}
		}
		#endregion

		#region Methods
		public void Draw(SpriteBatch spriteBatch)
		{
			if (debugDrawEnabled)
			{
				for (int i = 0; i < grid.GetLength (0); i++)
				{
					for (int j = 0; j < grid.GetLength (1); j++)
					{
						// Draw Tile differently based on status for best indication
						switch (grid [i, j].TileStatus)
						{
						case Tile.Status.Open:
							grid [i, j].Draw (spriteBatch, openTileTexture);
							break;
						case Tile.Status.Closed:
							grid [i, j].Draw (spriteBatch, closedTileTexture);
							break;
						case Tile.Status.Path:
							grid [i, j].Draw (spriteBatch, pathTileTexture);
							break;
						}
					}
				}
			}
		}

		// Reconfigure tile open/closed status due to walls.
		public void Update()
		{
			for (int i = 0; i < grid.GetLength (0); i++)
			{
				for (int j = 0; j < grid.GetLength (1); j++)
				{
					foreach (Wall wall in game.GetWalls())
					{
						// Use wall GridBox to give us some padding around walls.
						if (grid [i, j].RealBoundary.Intersects (wall.GridBox))
							grid [i, j].SetClosed ();
						else if (grid [i, j].TileStatus != Tile.Status.Closed)
							grid [i, j].SetOpen ();
					}
				}
			}
		}

		// Get a 1D list of all tiles in the grid.
		public List<Tile> GetTileList()
		{
			return tileList;
		}

		// Find shortest path using A* from a start position to an end position.
		public List<Tile> FindPath(Point start, Point end)
		{
			// Make sure start and end points are appropriate.
			if (start.X < 0 || start.Y < 0 || end.X < 0 || end.Y < 0)
				return null;
				
			// Open and closed lists, initially both empty.
			List<Tile> openList = new List<Tile> ();
			List<Tile> closedList = new List<Tile> ();

			// Configure start and end tiles.
			Tile current = grid[start.X, start.Y];
			Tile finish = grid [end.X, end.Y];
			// Cannot continue to path to blocked tile.
			if (finish.TileStatus == Tile.Status.Closed)
				return null;

			// Start tile is first added to open list.
			openList.Add (current);

			// Dictionaries for easy storage/retrieval of distances.
			Dictionary<Tile, int> heuristic = new Dictionary<Tile, int> ();
			Dictionary<Tile, int> distanceTravelled = new Dictionary<Tile, int> ();
			Dictionary<Tile, int> totalDistance = new Dictionary<Tile, int> ();
			Dictionary<Tile, Tile> previousTiles = new Dictionary<Tile,Tile> ();

			// Add all of the heuristic values
			for (int i = 0; i < grid.GetLength (0); i++)
			{
				for (int j = 0; j < grid.GetLength (1); j++)
				{
					heuristic.Add (grid [i, j], Math.Abs (grid [i, j].GridLocation.X - finish.GridLocation.X) + Math.Abs (grid [i, j].GridLocation.Y - finish.GridLocation.Y));
				}
			}

			distanceTravelled.Add (current, 0);
			totalDistance [current] = distanceTravelled [current] + heuristic [current];
			// Set previous for start node equal to null as marker for path building later.
			previousTiles.Add (current, null);

			// Loop until the finish tile is in the closed list (ie visited)
			while (!closedList.Contains (finish))
			{
				// Get tiles surrounding the current tile.
				List<Tile> surrounding = GetSurroundingTiles (current);
				// For each of the surrounding tiles, update distances.
				foreach (Tile item in surrounding)
				{
					if (!closedList.Contains (item))
					{
						// First time seeing item
						if (!openList.Contains (item))
						{
							openList.Add (item);
							distanceTravelled [item] = distanceTravelled [current] + 1;
							totalDistance [item] = distanceTravelled [item] + heuristic [item];
							previousTiles [item] = current;
						}
						// If the new estimate is less than old, update it
						if ((distanceTravelled [current] + 1) < distanceTravelled [item])
						{
							distanceTravelled [item] = distanceTravelled [current] + 1;
							totalDistance [item] = distanceTravelled [item] + heuristic [item];
							previousTiles [item] = current;
						}
					}
				}

				// We've now visited this node, add to closed remove from open
				closedList.Add (current);
				openList.Remove (current);

				// Start min distance at ~infinity
				int min = 1000000;
				// Find minimum distance (goofy instead of priority queue but whatever)
				foreach (Tile item in openList)
				{
					if (totalDistance [item] < min)
					{
						current = item;
						min = totalDistance [current];
					}
				}
			}

			// Path found, construct the path in reverse order.
			List<Tile> path = new List<Tile> ();
			path.Add (finish);
			Tile prevTile = previousTiles [finish];
			while (prevTile != null)
			{
				path.Add (prevTile);
				prevTile = previousTiles [prevTile];
			}

			// Reverse to get correct order of path.
			path.Reverse ();

			// Closed list debug output.
			String closedListString = "";
			foreach (Tile item in closedList)
			{
				closedListString += item.GridLocation.ToString ();
			}
			Debug.WriteLine (closedListString);

			// Return properly ordered path from start to end.
			return path;
		}

		public void ToggleDebugDraw()
		{
			debugDrawEnabled = !debugDrawEnabled;
		}

		private List<Tile> GetSurroundingTiles(Tile tile)
		{
			Point location = tile.GridLocation;
			List<Tile> surroundingTiles = new List<Tile> ();
			if (location.X - 1 >= 0 && grid [location.X - 1, location.Y].TileStatus != Tile.Status.Closed)
				surroundingTiles.Add (grid [location.X - 1, location.Y]);
			if (location.X + 1 < grid.GetLength (0) && grid [location.X + 1, location.Y].TileStatus != Tile.Status.Closed)
				surroundingTiles.Add (grid [location.X + 1, location.Y]);
			if (location.Y - 1 >= 0 && grid [location.X, location.Y - 1].TileStatus != Tile.Status.Closed)
				surroundingTiles.Add (grid [location.X, location.Y - 1]);
			if (location.Y + 1 < grid.GetLength (1) && grid [location.X, location.Y + 1].TileStatus != Tile.Status.Closed)
				surroundingTiles.Add (grid [location.X, location.Y + 1]);

			return surroundingTiles;
		}
		#endregion
	}
}

