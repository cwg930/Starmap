using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public class Tower : Agent
	{
		#region Fields
		private int damage;
		private AdjacentAgentSensor AASensor;
		#endregion

		#region Properties
		public int Damage { get { return damage; } }
		public override Rectangle BoundingBox {
			get {
				return new Rectangle ((int)position.X, (int)position.Y, Width, Height);
			}
		}
		#endregion

		public Tower (Vector2 position, int damage, float range)
		{
			this.center = new Vector2 (Game1.Instance.GameSettings.TowerWidth / 2, Game1.Instance.GameSettings.TowerHeight / 2);
			this.position = position;
			this.damage = damage;
			Initialize (range);
		}

		private void Initialize(float range)
		{
			AASensor = new AdjacentAgentSensor (this, range);
		}

		public void Update()
		{
			AASensor.Update ();
			var targets = from pair in AASensor.AgentsInRange
			              orderby pair.Value.Item1 ascending
			              select pair;
			foreach (KeyValuePair<Unit, Tuple<float,float>> p in targets) {
				Console.WriteLine (p.Key + " " + p.Value.Item1 + " " + p.Value.Item2);
			}
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw (agentTexture, position, null, Color.White, heading, center, 1.0f, SpriteEffects.None, 0f);
		}
	}
}

