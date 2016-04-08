using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public struct Button
	{
		#region Fields
		private string text;
		private Texture2D image;
		private Rectangle bounds;
		private Vector2 position;
		#endregion

		#region Properties
		public Rectangle Bounds { get { return bounds; } }
		#endregion

		#region Constructors

		public Button(string text, Texture2D image, Vector2 position)
		{
			this.text = text;
			this.image = image;
			this.position = position;
			bounds = new Rectangle ((int)position.X, (int)position.Y, image.Width, image.Height);
		}

		public void Draw(SpriteBatch sb, SpriteFont font)
		{
			sb.Draw (image, position, Color.White);
			sb.DrawString (font, text, calcTextPosition (font), Color.Black);

		}

		private Vector2 calcTextPosition(SpriteFont font)
		{
			Vector2 textSize = font.MeasureString (text);
			return new Vector2 (bounds.Center.X - textSize.X / 2, bounds.Center.Y - textSize.Y / 2);
		}
		#endregion
	}
}

