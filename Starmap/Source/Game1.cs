#region Using Statements
using System;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Starmap
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private static Game1 instance;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		GameState gameState;
		Screen currentScreen;
		Settings gameSettings;

		public Settings GameSettings {
			get { return gameSettings; }
		}

		public static Game1 Instance {
			get {
				return instance;
			}
		}

		public static Random RandomGenerator { 
			get; 
			private set;
		}

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;
			this.IsMouseVisible = true;
			if(instance == null)
				instance = this;
			RandomGenerator = new Random ();
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			gameState = GameState.MainMenu;
			currentScreen = new MenuScreen ();
			LoadSettings ();
			base.Initialize ();
				
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
			    Keyboard.GetState ().IsKeyDown (Keys.Escape)) {
				Exit ();
			}
			#endif
			// TODO: Add your update logic here	
			currentScreen.Update();
			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
			currentScreen.Draw (spriteBatch);
			//TODO: Add your drawing code here
            
			base.Draw (gameTime);
		}

		public void UpdateGameState(GameState state)
		{
			switch (state) {
			case GameState.MainMenu:
				currentScreen = new MenuScreen ();
				break;
			case GameState.Gameplay:
				currentScreen = new GameplayScreen ();
				break;
			default: 
				break;
			}
		}

		private void LoadSettings()
		{
			try{
				FileStream fs = File.OpenRead("GameSettings.xml");
				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				gameSettings = (Settings)serializer.Deserialize(fs);
			}catch(Exception e){
				Console.WriteLine ("Error: Cannot load settings file, using default values");
				gameSettings.NumWalls = 5;
				gameSettings.WallGenChance = 0.15;
			}
		}
	}
}

