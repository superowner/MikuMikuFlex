﻿using System.Diagnostics;

namespace MikuMikuFlex
{
	internal class InvalidMMEEffectShader例外 : MMEEffect例外
	{
		public InvalidMMEEffectShader例外( string message ) : base( message )
		{
			Debug.WriteLine( message );
		}
	}
}
