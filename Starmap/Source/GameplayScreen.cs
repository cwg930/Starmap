using System;
using System.Threading;
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
		Tile startTile;
		Tile endTile;
		SpriteFont gameTextFont;
		List<Unit> units;
		List<Tower> towers;
		List<Wall> walls;
		Texture2D wallTexture;
		Texture2D[] creatureSprites;
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
			units = new List<Unit> ();
			walls = new List<Wall> ();
			int i = Game1.Instance.GameSettings.NumWalls;
			foreach (Tile t in mapGrid.GetTileList()) {
				if (Game1.RandomGenerator.NextDouble () < Game1.Instance.GameSettings.WallGenChance && i > 0) {
					walls.Add (new Wall (t.Position));
					i--;
				}
			}
			startTile = mapGrid.GetTileAtPosition (new Point (0, Game1.RandomGenerator.Next(Game1.Instance.GraphicsDevice.Viewport.Height)));
			endTile = mapGrid.GetTileAtPosition (new Point (Game1.Instance.GraphicsDevice.Viewport.Width, Game1.RandomGenerator.Next(Game1.Instance.GraphicsDevice.Viewport.Height)));
		}

		protected override void LoadContent ()
		{
			gameTextFont = Game1.Instance.Content.Load<SpriteFont> ("Fonts/MenuFont");
			wallTexture = Game1.Instance.Content.Load<Texture2D> ("Graphics/WallQuad");
			foreach (Wall w in walls) {
				w.AgentTexture = wallTexture;
			}
			Texture2D spriteSheet = Game1.Instance.Content.Load<Texture2D> ("Graphics/Creatures");
			int spriteHeight = Game1.Instance.GameSettings.SpriteHeight;
			int numSlices = spriteSheet.Height / spriteHeight;
			creatureSprites = new Texture2D[numSlices];
			for (int i = 0; i < numSlices; i++) {
				Rectangle source = new Rectangle (0, i * spriteHeight, spriteSheet.Width, spriteHeight);
				Color[] data = new Color[spriteHeight * spriteSheet.Width];
				spriteSheet.GetData (0, source, data, 0, spriteHeight * spriteSheet.Width);
				creatureSprites [i] = new Texture2D (Game1.Instance.GraphicsDevice, spriteSheet.Width, spriteHeight);
				creatureSprites [i].SetData (data);
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
			if (currentKeyboardState.IsKeyDown (Keys.X) && previousKeyboardState.IsKeyUp (Keys.X)) {
				Thread t = new Thread(SpawnNewWave);
				t.Start (10);
			}

			if (isPlacementMode) {
				if (mouseState.LeftButton == ButtonState.Pressed) {
					Tile t = mapGrid.GetTileAtPosition (mouseState.Position);
					Wall w = new Wall (t.Position);
					w.AgentTexture = wallTexture;
					walls.Add (w);
				}
			}
			foreach (Unit u in units) {
				u.Update (endTile);
			}
			mapGrid.Update ();
		}

		public Grid GetGrid ()
		{
			return mapGrid;
		}

		public override void Draw (SpriteBatch sb, GameTime deltaTime)
		{
			sb.Begin ();
			sb.DrawString (gameTextFont, "GAMEPLAY SCREEN", new Vector2 (Game1.Instance.GraphicsDevice.Viewport.Width / 2, Game1.Instance.GraphicsDevice.Viewport.Height / 2), Color.Black);
			mapGrid.Draw (sb);
			foreach (Wall w in walls) {
				w.Draw (sb);
			}
			foreach (Unit u in units) {
				u.Draw (sb, deltaTime);
			}
			if (isPlacementMode) {
				sb.Draw (wallTexture, new Vector2(mouseState.Position.X, mouseState.Position.Y), Color.TransparentBlack);
			}

			sb.End ();
		}

		private void SpawnNewWave(object data)
		{
			int numUnits = Convert.ToInt32(data);
			units.Clear();
			int textureIndex = Game1.RandomGenerator.Next (creatureSprites.Length);
			for (int i = 0; i < numUnits; i++) {
				Unit u = new Unit (10, creatureSprites[textureIndex], startTile.Position);
				units.Add (u);
				Thread.Sleep (250);
			}
		}
	}
}

