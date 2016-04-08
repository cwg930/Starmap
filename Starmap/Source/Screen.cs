using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starmap
{
	public abstract class Screen
	{
		#region Fields
		protected static Screen instance;
		#endregion

		#region Properties
		public static Screen Instance{
			get{
				return instance;
			}
		}
		#endregion

		#region Methods
		public Screen ()
		{
			
		}

		public abstract void Update();

		public virtual void Draw (SpriteBatch sb) 
		{
		}
			
		protected abstract void LoadContent ();
		#endregion
	}
}

