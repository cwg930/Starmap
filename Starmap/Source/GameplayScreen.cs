using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Starmap
{
	public class GameplayScreen : Screen
	{
		#region Fields
		Grid mapGrid;
		SpriteFont gameTextFont;
		List<Unit> units;
		List<Tower> towers;
		List<Wall> walls;
		Texture2D wallTexture;
		KeyboardState currentKeyboardState;
		KeyboardState previousKeyboardState;
		MouseState mouseState;
		bool isPlacementMode = false;
		#endregion

		#region Properties
		public List<Wall> Walls 
		{
			get { return walls; }
		}
		#endregion
		
		public GameplayScreen () : base()
		{
			if (instance != this)
				instance = this;
			Initialize ();
			LoadContent ();
		}

		protected void Initialize()
		{
			mapGrid = new Grid (32, 0);
			walls = new List<Wall> ();
			int i = Game1.Instance.GameSettings.NumWalls;
			foreach (Tile t in mapGrid.GetTileList()) {
				if (Game1.RandomGenerator.NextDouble () < Game1.Instance.GameSettings.WallGenChance && i > 0) {
					walls.Add (new Wall (t.Position));
					i--;
				}
			}
		}

		protected override void LoadContent ()
		{
			gameTextFont = Game1.Instance.Content.Load<SpriteFont> ("Fonts/MenuFont");
			wallTexture = Game1.Instance.Content.Load<Texture2D> ("Graphics/WallQuad");
			foreach (Wall w in walls) {
				w.AgentTexture = wallTexture;
			}
		}

		public override void Update()
		{
			previousKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState ();
			mouseState = Mouse.GetState ();
			if (currentKeyboardState.IsKeyDown (Keys.M)) {
				Game1.Instance.UpdateGameState (GameState.MainMenu);
				Console.WriteLine ("Switch to menu screen");
				return;
			}
			if (currentKeyboardState.IsKeyDown (Keys.W) && previousKeyboardState.IsKeyUp (Keys.W)) {
				isPlacementMode = !isPlacementMode;
				Console.WriteLine ("Placement Mode: " + isPlacementMode);
			}

			if (isPlacementMode) {
				if (mouseState.LeftButton == ButtonState.Pressed) {
					Tile t = mapGrid.GetTileAtPosition (mouseState.Position);
					Wall w = new Wall (t.Position);
					w.AgentTexture = wallTexture;
					walls.Add (w);
				}
			}

			mapGrid.Update ();
		}

		public Grid GetGrid ()
		{
			return mapGrid;
		}

		public override void Draw (SpriteBatch sb)
		{
			sb.Begin ();
			sb.DrawString (gameTextFont, "GAMEPLAY SCREEN", new Vector2 (Game1.Instance.GraphicsDevice.Viewport.Width / 2, Game1.Instance.GraphicsDevice.Viewport.Height / 2), Color.Black);
			mapGrid.Draw (sb);
			foreach (Wall w in walls) {
				w.Draw (sb);
			}
			if (isPlacementMode) {
				sb.Draw (wallTexture, new Vector2(mouseState.Position.X, mouseState.Position.Y), Color.TransparentBlack);
			}
			sb.End ();
		}
	}
}

