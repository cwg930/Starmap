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
		List<Tile> path;
		Texture2D wallTexture;
		Texture2D towerTexture; 
		Texture2D[] creatureSprites;
		KeyboardState currentKeyboardState;
		KeyboardState previousKeyboardState;
		MouseState mouseState;
		bool wallPlacementMode = false;
		bool towerPlacementMode = false;
		#endregion

		#region Properties
		public List<Wall> Walls 
		{
			get { return walls; }
		}
		public List<Unit> Units
		{
			get { return units; }
		}
		#endregion
		
		public GameplayScreen () : base()
		{
			if (instance != this)
				instance = this;
			LoadContent ();
			Initialize ();
		}

		protected void Initialize()
		{
			mapGrid = new Grid (Game1.Instance.GameSettings.TileWidth, 0);
			units = new List<Unit> ();
			walls = new List<Wall> ();
			towers = new List<Tower> ();
			int i = Game1.Instance.GameSettings.NumWalls;
			foreach (Tile t in mapGrid.GetTileList()) {
				if (Game1.RandomGenerator.NextDouble () < Game1.Instance.GameSettings.WallGenChance && i > 0) {
					walls.Add (new Wall (t.Position));
					i--;
				}
			}
			foreach (Wall w in walls) {
				w.AgentTexture = wallTexture;
			}
			startTile = mapGrid.GetTileAtPosition (new Point (0, Game1.RandomGenerator.Next(Game1.Instance.GraphicsDevice.Viewport.Height)));
			endTile = mapGrid.GetTileAtPosition (new Point (Game1.Instance.GraphicsDevice.Viewport.Width, Game1.RandomGenerator.Next(Game1.Instance.GraphicsDevice.Viewport.Height)));
			mapGrid.Update ();
			path = mapGrid.FindPath (startTile.Position.ToPoint(), endTile.Position.ToPoint());
		}

		protected override void LoadContent ()
		{
			gameTextFont = Game1.Instance.Content.Load<SpriteFont> ("Fonts/MenuFont");
			wallTexture = Game1.Instance.Content.Load<Texture2D> ("Graphics/WallQuad");

			Texture2D spriteSheet = Game1.Instance.Content.Load<Texture2D> ("Graphics/Creatures");
			int spriteHeight = Game1.Instance.GameSettings.UnitHeight;
			int numSlices = spriteSheet.Height / spriteHeight;
			creatureSprites = new Texture2D[numSlices];
			for (int i = 0; i < numSlices; i++) {
				Rectangle source = new Rectangle (0, i * spriteHeight, spriteSheet.Width, spriteHeight);
				Color[] data = new Color[spriteHeight * spriteSheet.Width];
				spriteSheet.GetData (0, source, data, 0, spriteHeight * spriteSheet.Width);
				creatureSprites [i] = new Texture2D (Game1.Instance.GraphicsDevice, spriteSheet.Width, spriteHeight);
				creatureSprites [i].SetData (data);
			}
			towerTexture = Game1.Instance.Content.Load<Texture2D> ("Graphics/Player");
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
				wallPlacementMode = !wallPlacementMode;
				towerPlacementMode = false;
				Console.WriteLine ("Placement Mode: " + wallPlacementMode);
			}
			if (currentKeyboardState.IsKeyDown (Keys.X) && previousKeyboardState.IsKeyUp (Keys.X)) {
				Thread t = new Thread(SpawnNewWave);
				t.Start (10);
			}

			if (currentKeyboardState.IsKeyDown (Keys.D1) && previousKeyboardState.IsKeyUp (Keys.D1)) {
				towerPlacementMode = !towerPlacementMode;
				wallPlacementMode = false;
			}

			if (mouseState.LeftButton == ButtonState.Pressed) {
				if (wallPlacementMode) {
					Tile t = mapGrid.GetTileAtPosition (mouseState.Position);
					Wall w = new Wall (t.Position);
					w.AgentTexture = wallTexture;
					walls.Add (w);
					mapGrid.Update ();
				}
				if (towerPlacementMode) {
					foreach (Wall w in walls) {
						if(w.BoundingBox.Contains(mouseState.Position)){
							Tower t = new Tower(w.Center, 10, 100);
							t.AgentTexture = towerTexture;
							towers.Add(t);
						}
					}
				}
				wallPlacementMode = false;
				towerPlacementMode = false;
			}

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
			foreach (Tower t in towers) {
				t.Draw (sb);
			}
			if (wallPlacementMode) {
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
				Unit u = new Unit (10, creatureSprites[textureIndex], startTile.Position, path);
				units.Add (u);
				Thread.Sleep (300);
			}
		}
	}
}

