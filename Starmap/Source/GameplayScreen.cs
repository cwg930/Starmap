using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public class GameplayScreen : Screen
	{
		#region Fields
		Grid mapGrid;
		#endregion

		#region Properties
		#endregion
		
		public GameplayScreen () : base()
		{
			if (instance != this)
				instance = this;	
		}

		protected void Initialize()
		{
			mapGrid = new Grid (32, 2);
		}

		protected override void LoadContent ()
		{
		}

		public override void Draw (SpriteBatch sb)
		{
			mapGrid.Draw (sb);
			base.Draw (sb);
		}
	}
}

