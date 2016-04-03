using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace Starmap
{
	public class MenuScreen : Screen
	{
		#region Fields
		private SpriteFont menuFont;
		#endregion

		#region Constructors
		public MenuScreen (SpriteFont font)
		{
			this.menuFont = font;
		}
		#endregion

		#region Methods
		public void Update()
		{
			
		}

		public void LoadContent()
		{
		}

		public override void Draw (SpriteBatch sb)
		{
		}
		#endregion
	}
}

