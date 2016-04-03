using System;

namespace Starmap
{
	public abstract class Tower
	{
		#region Fields
		private int damage;
		#endregion

		#region Properties
		public int Damage { get { return damage; } }
		#endregion

		public Tower ()
		{
		}
	}
}

