using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace Starmap
{
	public class Unit : Agent
	{

		#region Fields
		private float moveSpeed = 1.0f;
		private float turnSpeed = MathHelper.ToRadians(5.0f); 
		private int reward;
		private int id;
		private PathNodeSensor pathSensor;
		private const float ANGLE_TOLERANCE = 0.1f;
		private const float DISTANCE_TOLERANCE = 2.0f;
		private float time;
		private int frameIndex;
		private int totalFrames = Game1.Instance.GameSettings.UnitAnimationFrames;
		private int frameWidth = Game1.Instance.GameSettings.UnitWidth;
		private int frameHeight = Game1.Instance.GameSettings.UnitHeight;
		private LinkedList<Tile> path;
		private Thread pathThread;
		#endregion

		#region Properties
		public int ID { get { return id; } }
		public int HitPoints { get; set; }
		public float MoveSpeed { get { return moveSpeed; } }
		public int Reward { get { return reward; } }
		public override Rectangle BoundingBox {
			get {
				return new Rectangle ((int)position.X, (int)position.Y, Width, Height);
			}
		}
		#endregion

		#region Methods
		public Unit(int id, int hp, int reward, Texture2D texture, Vector2 position, List<Tile> path)
		{
			this.id = id;
			this.HitPoints = hp;
			this.reward = reward;
			this.agentTexture = texture;
			this.position = position;
			this.path = new LinkedList<Tile> (path);
			Initialize ();
		}

		public Unit (float moveSpeed, float turnSpeed, int reward, Texture2D texture, Vector2 position)
		{
			this.moveSpeed = moveSpeed;
			this.reward = reward;
			this.agentTexture = texture;
			this.position = position;
			Initialize ();
		}

		private void Initialize()
		{
			center = new Vector2 (Game1.Instance.GameSettings.UnitWidth / 2, Game1.Instance.GameSettings.UnitHeight / 2);
			pathSensor = new PathNodeSensor (this, 100);
		//	pathThread = new Thread (FollowPath);
		//	pathThread.Start ();
		}

		public void Update()
		{
			Point currentLocation;
			if (path.Count > 0) {
				currentLocation = new Point ((int)(position.X + center.X) / Game1.Instance.GameSettings.TileWidth, 
					(int)(position.Y + center.Y) / Game1.Instance.GameSettings.TileWidth);
				if (currentLocation == path.First.Value.GridLocation) {
					path.RemoveFirst();
				}
				if (path.Count > 0) {
					Seek (path.First.Value.Center);
					//Thread.Sleep (25);
				}
			}
		}

		public void Draw(SpriteBatch sb, GameTime deltaTime)
		{
			time += (float)deltaTime.ElapsedGameTime.TotalSeconds;
			while (time > Game1.Instance.GameSettings.AnimationFrameTime) {
				frameIndex++;
				if (frameIndex >= totalFrames) {
					frameIndex = 0;
				}
				time = 0f;
			}
			Rectangle source = new Rectangle (frameIndex * frameWidth, 0, frameWidth, frameHeight); 
			sb.Draw (agentTexture, position, source, Color.White);
			sb.DrawString ((Screen.Instance as GameplayScreen).GameTextFont, HitPoints.ToString(), position, Color.Red);
		}	

		public void ChangePath(List<Tile> newPath)
		{
			LinkedList<Tile> updatedPath = new LinkedList<Tile> (newPath);
			while (updatedPath.First.Value.GridLocation != this.path.First.Value.GridLocation)
				updatedPath.RemoveFirst();
				
			this.path = new LinkedList<Tile>(updatedPath);
		}
		/*
		 * FollowPath method for multithreaded path following -- NOT NEEDED
		private void FollowPath()
		{
			Point currentLocation;
			while (path.Count > 0) {
				currentLocation = new Point ((int)position.X / Game1.Instance.GameSettings.TileWidth, 
					(int)position.Y / Game1.Instance.GameSettings.TileWidth);
				if (currentLocation == path.First.Value.GridLocation) {
					path.RemoveFirst();
				}
				if (path.Count > 0) {
					Seek (path.First.Value.Center);
					Thread.Sleep (25);
				}
			}
		}
		*/
		private void Seek(Vector2 goal)
		{
			Vector2 goalVector = goal - (this.position + this.center);

			float velX = 0.0f;
			float velY = 0.0f;
			float relativeHeading = (float)Math.Acos(Vector2.Dot(Vector2.Normalize(this.HeadingVector), Vector2.Normalize(goalVector)));

			// Cross product to determine whether angle is positive/negative
			Vector3 crossResult = Vector3.Cross (new Vector3(HeadingVector.X, HeadingVector.Y, 0), new Vector3(goalVector.X, goalVector.Y, 0));

			if (relativeHeading > ANGLE_TOLERANCE)
			{
				// Rotate in the direction that will reach appropriate
				//  heading the fastest
				if (crossResult.Z >= 0)
					heading += turnSpeed;
				else
					heading -= turnSpeed;
			}
			else
			{
				if (Vector2.Distance (center, goal) > DISTANCE_TOLERANCE)
				{
					velX = (float)(Math.Cos (Heading) * moveSpeed);
					velY = (float)(Math.Sin (Heading) * moveSpeed);
				}
			}

			position += new Vector2 (velX, velY);
		}
		#endregion
	}
}

