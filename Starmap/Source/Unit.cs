using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public class Unit
	{

		#region Fields
		private int moveSpeed;
		private int reward;
		#endregion

		#region Properties
		public int HitPoints { get; set; }

		public int MoveSpeed { get { return moveSpeed; } }
		public int Reward { get { return reward; } }
		#endregion

		#region Methods
		public Unit (int moveSpeed, int reward)
		{
			this.moveSpeed = moveSpeed;
			this.reward = reward;
		}

		public void Initialize()
		{
		}
		#endregion
	}
}

