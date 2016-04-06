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
		public MenuScreen ()
		{
			if (instance != this)
				instance = this;
		}
		#endregion

		#region Methods
		public void Update()
		{
			
		}

		protected override void LoadContent()
		{
			menuFont = Game1.Instance.Content.Load<SpriteFont> ("Fonts/MenuFont");
		}

		public override void Draw (SpriteBatch sb)
		{
		}
		#endregion
	}
}

