using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
			LoadContent ();
		}
		#endregion

		#region Methods
		public override void Update()
		{
			if (Keyboard.GetState ().IsKeyDown (Keys.N)) {
				Game1.Instance.UpdateGameState (GameState.Gameplay);
				Console.WriteLine ("Switch to gameplay screen");
			}
		}

		protected override void LoadContent()
		{
			menuFont = Game1.Instance.Content.Load<SpriteFont> ("Fonts/MenuFont");
		}

		public override void Draw (SpriteBatch sb, GameTime deltaTime)
		{
			sb.Begin ();
			sb.DrawString (menuFont, "MENU SCREEN\n" + Game1.Instance.GameSettings.NumWalls + " " + Game1.Instance.GameSettings.WallGenChance, new Vector2(Game1.Instance.GraphicsDevice.Viewport.Width / 2, Game1.Instance.GraphicsDevice.Viewport.Height / 2), Color.Black);
			sb.End ();
		}
		#endregion
	}
}

