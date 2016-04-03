using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public abstract class Screen
	{
		#region Fields
		#endregion

		#region Methods
		public Screen ()
		{
		}
		public virtual void Draw (SpriteBatch sb) 
		{
		}
		#endregion
	}
}

