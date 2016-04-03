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
		#endregion

		#region Properties
		#endregion

		#region Constructors
		public Button()
		{
		}

		public Button(string text, Texture2D image)
		{
			this.text = text;
			this.image = image;
		}
		#endregion
	}
}

