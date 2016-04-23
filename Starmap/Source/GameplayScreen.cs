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
		const string mapWipeString = "You blocked the path, unleashing DOOM";
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
		bool newWaveStarted = false;
		int playerResources = Game1.Instance.GameSettings.StartingResources;
		int playerLives = Game1.Instance.GameSettings.PlayerLives;
		int currentUnitHP = Game1.Instance.GameSettings.UnitStartingHP;
		int currentNumUnits = Game1.Instance.GameSettings.StartingWaveUnits;
		int currentWave = 0;
		float spawnTimer;
		float waveTimer;
		int spawnCounter = 0;
		bool badPlayer = false;
		List<int> toDelete;
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
		public SpriteFont GameTextFont {
			get { return gameTextFont; }
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
			if (path == null) {
				Console.WriteLine ("Bad path, regenerating");
				Game1.Instance.UpdateGameState (GameState.Gameplay);
			}
			toDelete = new List<int> ();
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

		public override void Update(GameTime gameTime)
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
				SpawnNewWave (gameTime);
			}

			if (currentKeyboardState.IsKeyDown (Keys.T) && previousKeyboardState.IsKeyUp (Keys.T)) {
				towerPlacementMode = !towerPlacementMode;
				wallPlacementMode = false;
			}

			if (playerLives <= 0) {
				Game1.Instance.UpdateGameState (GameState.EndOfGame);
				return;
			}

			if (mouseState.LeftButton == ButtonState.Pressed) {
				if (wallPlacementMode && playerResources >= Game1.Instance.GameSettings.WallCost) {
					Tile t = mapGrid.GetTileAtPosition (mouseState.Position);
					Wall w = new Wall (t.Position);
					w.AgentTexture = wallTexture;
					walls.Add (w);
					mapGrid.Update ();
					path = mapGrid.FindPath (startTile.Position.ToPoint(), endTile.Position.ToPoint());
					if (path == null) {
						walls.Clear ();
						towers.Clear ();
						badPlayer = true;
					}
					foreach (Unit u in units)
						u.ChangePath (path);
					playerResources -= Game1.Instance.GameSettings.WallCost;
				}
				if (towerPlacementMode && playerResources >= Game1.Instance.GameSettings.TowerCost) {
					foreach (Wall w in walls) {
						if(w.BoundingBox.Contains(mouseState.Position)){
							Tower t = new Tower(w.Center, 10, 100);
							t.AgentTexture = towerTexture;
							towers.Add(t);
							playerResources -= Game1.Instance.GameSettings.TowerCost;
							break;
						}
					}
				}
				wallPlacementMode = false;
				towerPlacementMode = false;
			}
			toDelete.Clear ();
			for (int i = 0; i < units.Count; i++) {
				units[i].Update ();
				if (endTile.RealBoundary.Contains (units[i].Position + units[i].Center)) {
					playerLives--;
					toDelete.Add (i);
				}
			}
			foreach (int i in toDelete) {
				units.RemoveAt (i);
			}
			foreach (Tower t in towers) {
				t.Update (gameTime);
			}

			if (newWaveStarted == true) {
				SpawnNewWave (gameTime);
			}

			if (units.Count <= 0 && newWaveStarted == false) {
				waveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
				while (waveTimer >= Game1.Instance.GameSettings.WaveDelay) {
					currentWave++;
					if (currentWave > 1) {
						currentNumUnits += Game1.Instance.GameSettings.UnitsPerWave;
						currentUnitHP = (int)(currentUnitHP * Game1.Instance.GameSettings.UnitWaveHPMult);
						playerResources += Game1.Instance.GameSettings.WaveReward;
					}
					newWaveStarted = true;
					waveTimer = 0;
				}
			}
		}

		public Grid GetGrid ()
		{
			return mapGrid;
		}

		public override void Draw (SpriteBatch sb, GameTime deltaTime)
		{
			sb.Begin ();
			//sb.DrawString (gameTextFont, "GAMEPLAY SCREEN", new Vector2 (Game1.Instance.GraphicsDevice.Viewport.Width / 2, Game1.Instance.GraphicsDevice.Viewport.Height / 2), Color.Black);
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
				sb.Draw (wallTexture, new Vector2(mouseState.Position.X, mouseState.Position.Y), Color.White);
			}
			if (badPlayer) {
				Vector2 markerSize = gameTextFont.MeasureString (mapWipeString);
				sb.DrawString (gameTextFont, mapWipeString, 
					new Vector2(Game1.Instance.GraphicsDevice.Viewport.Width/2 - markerSize.X/2, 
						Game1.Instance.GraphicsDevice.Viewport.Height/2 - markerSize.Y/2), Color.Red);
			}
			sb.DrawString (gameTextFont, "Wave: " + currentWave + "Lives: " + playerLives + "\nResources: " + playerResources, new Vector2 (0,0), Color.Black); 
			sb.End ();
		}
		/*
		 * SpawnNewWave for multithreaded approach. 
		 * deprecated - LIST IS NOT THREAD SAFE
		private void SpawnNewWave(object data)
		{
			int numUnits = Convert.ToInt32(data);
			units.Clear();
			Thread.Sleep (TimeSpan.FromSeconds(15).Milliseconds);
			int textureIndex = Game1.RandomGenerator.Next (creatureSprites.Length);
			for (int i = 0; i < numUnits; i++) {
				Unit u = new Unit (i, currentUnitHP, Game1.Instance.GameSettings.UnitReward, creatureSprites[textureIndex], startTile.Position, path);
				units.Add (u);
				Thread.Sleep (300);
			}
			newWaveStarted = false;
		}
		*/
		private void SpawnNewWave(GameTime gameTime)
		{
			spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			while (spawnTimer > Game1.Instance.GameSettings.SpawnDelay && spawnCounter < currentNumUnits) {
				int textureIndex = Game1.RandomGenerator.Next (creatureSprites.Length);
				Unit u = new Unit (spawnCounter, currentUnitHP, Game1.Instance.GameSettings.UnitReward, 
					         creatureSprites [textureIndex], startTile.Position, path);
				units.Add (u);
				spawnTimer = 0f;
				spawnCounter++;
				Console.WriteLine ("Created unit: " + spawnCounter);
			}
			if (spawnCounter >= currentNumUnits) {
				spawnCounter = 0;
				newWaveStarted = false;
			}
		}

		public void AddReward(int value)
		{
			playerResources += value;
		}
	}
}

