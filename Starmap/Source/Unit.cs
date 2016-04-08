using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public class Unit : Agent
	{

		#region Fields
		private int moveSpeed;
		private int reward;
		#endregion

		#region Properties
		public int HitPoints { get; set; }
		public int MoveSpeed { get { return moveSpeed; } }
		public int Reward { get { return reward; } }
		public override Rectangle BoundingBox {
			get {
				return new Rectangle ((int)position.X, (int)position.Y, Width, Height);
			}
		}
		#endregion

		#region Methods
		public Unit (int moveSpeed, int reward, Texture2D texture, Vector2 position)
		{
			this.moveSpeed = moveSpeed;
			this.reward = reward;
			this.agentTexture = texture;
			this.position = position;
		}			

		public void Update(GameTime gameTime)
		{
			
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw (agentTexture, position, Color.White);
		}	
		#endregion
	}
}

