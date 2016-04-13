using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public class Wall : Agent
	{
		#region Properties
		public override Rectangle BoundingBox {
			get { return new Rectangle ((int)Position.X, (int)Position.Y, Width, Height);}
		}
		// Used like costmap inflation to prevent pathing too close to walls.
		public Rectangle GridBox {
			get { return new Rectangle ((int)Position.X, (int)Position.Y, Width, Height);}
		}
		#endregion

		#region Constructors
		public Wall(Vector2 position)
		{
			this.position = position;
		}
		#endregion

		#region Methods
		public void Draw(SpriteBatch sb)
		{
			sb.Draw (AgentTexture, Position, Color.White);
		}
		#endregion
	}
}

