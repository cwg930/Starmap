using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public class Tile
	{
		public enum Status
		{
			Open,
			Closed,
			Path
		}

		#region Fields
		private Rectangle realBoundary;
		private Rectangle drawableBoundary;
		private Vector2 position;
		private Vector2 center;
		private Status status;
		private Point gridLocation;
		#endregion

		#region Properties
		public Rectangle RealBoundary
		{
			get { return realBoundary; }
			private set { }
		}
		public Rectangle DrawableBoundary
		{
			get { return drawableBoundary; }
			private set { }
		}
		public Vector2 Center
		{
			get { return center; }
			private set { }
		}
		public Status TileStatus
		{
			get { return status; }
			private set { }
		}
		public Point GridLocation
		{
			get { return gridLocation; }
			private set { }
		}
		public Vector2 Position 
		{
			get { return position; }
		}
		#endregion

		#region Constructors
		public Tile (Point topLeft, int width, int drawPadding)
		{
			this.position = new Vector2 (topLeft.X, topLeft.Y);
			this.gridLocation = new Point (topLeft.X / width, topLeft.Y / width);
			this.realBoundary = new Rectangle (topLeft.X, topLeft.Y, width, width);
			this.drawableBoundary = new Rectangle ((topLeft.X + drawPadding), (topLeft.Y + drawPadding), (width - drawPadding), (width - drawPadding));
			this.center = new Vector2 (topLeft.X + width / 2, topLeft.Y + width / 2);
		}
		#endregion

		#region Methods
		public void Draw(SpriteBatch spriteBatch, Texture2D texture)
		{
			spriteBatch.Draw (texture, DrawableBoundary, Color.White * 0.25f);
		}

		public void SetOpen()
		{
			status = Status.Open;
		}

		public void SetClosed()
		{
			status = Status.Closed;
		}

		public void SetPath()
		{
			status = Status.Path;
		}
		#endregion
	}
}

