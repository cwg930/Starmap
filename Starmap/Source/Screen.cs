using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public abstract class Screen
	{
		#region Fields
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		#endregion

		#region Methods
		public Screen (GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			this.graphics = graphics;
			this.spriteBatch = spriteBatch;
		}
		public virtual void Draw (GameTime gameTime) 
		{
		}
		#endregion
	}
}

