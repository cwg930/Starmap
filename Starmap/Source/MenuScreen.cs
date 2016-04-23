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
		#region Strings
		const string newGameStr = "Press N to start game";
		const string gameOverStr = "GAME OVER";
		const string tryAgainStr = "Press N to try again";
		#endregion

		#region Fields
		private SpriteFont menuFont;
		bool gameLost = false;
		Vector2 newGameMsgSize;
		Vector2 gameOverMsgSize;
		Vector2 tryAgainMsgSize;
		#endregion

		#region Constructors
		public MenuScreen ()
		{
			if (instance != this)
				instance = this;
			LoadContent ();
		}

		public MenuScreen(bool gameLost)
		{
			this.gameLost = gameLost;
			if (instance != this)
				instance = this;
			LoadContent ();
		}
		#endregion

		#region Methods
		public void Initialize()
		{
			newGameMsgSize = menuFont.MeasureString (newGameStr);
			gameOverMsgSize = menuFont.MeasureString (gameOverStr);
			tryAgainMsgSize = menuFont.MeasureString (tryAgainStr);
		}

		public override void Update(GameTime gameTime)
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
			if (gameLost) {
				sb.DrawString (menuFont, gameOverStr, 
					new Vector2 (Game1.Instance.GraphicsDevice.Viewport.Width / 2 - gameOverMsgSize.X / 2,
						Game1.Instance.GraphicsDevice.Viewport.Height / 2 - gameOverMsgSize.Y / 2), Color.DarkRed);
				sb.DrawString (menuFont, tryAgainStr, 
					new Vector2 (Game1.Instance.GraphicsDevice.Viewport.Width / 2 - tryAgainMsgSize.X / 2,
						Game1.Instance.GraphicsDevice.Viewport.Height / 2 - tryAgainMsgSize.Y / 2 + gameOverMsgSize.Y), Color.Black);
			} else {
				sb.DrawString (menuFont, newGameStr, 
					new Vector2 (Game1.Instance.GraphicsDevice.Viewport.Width / 2 - newGameMsgSize.X / 2, 
						Game1.Instance.GraphicsDevice.Viewport.Height / 2 - newGameMsgSize.Y / 2), Color.Black);
			}
			sb.End ();
		}
		#endregion
	}
}

