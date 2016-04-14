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
		private float turnSpeed = MathHelper.ToRadians(5.0f);
		private const float ANGLE_TOLERANCE = 0.5f;
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
			var targets = AASensor.AgentsInRange.OrderBy (i => i.Key.ID);
				/* from pair in AASensor.AgentsInRange
			              orderby pair.Value.Item1 ascending
			              select pair; */
			if (targets.Count () > 0) {
				KeyValuePair<Unit, Tuple<float, float>> target = targets.First ();
				if (target.Value.Item2 < 0 + ANGLE_TOLERANCE && target.Value.Item2 > 0 - ANGLE_TOLERANCE) {
					Shoot (target.Key);
				} else {
					if (target.Value.Item2 > 0 && target.Value.Item2 < 180) {
						heading += turnSpeed;
					} else {
						heading -= turnSpeed;
					}
				}
			}
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw (agentTexture, position, null, Color.White, heading, center, 1.0f, SpriteEffects.None, 0f);
		}

		private void Shoot(Unit target)
		{
			target.HitPoints -= damage;
			if (target.HitPoints <= 0) {
				(Screen.Instance as GameplayScreen).Units.Remove (target);
			}
		}
	}
}

