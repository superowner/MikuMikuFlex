﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace MikuMikuFlex.モデル.Shape.Overlay
{
	public class OverlayCubeShape : キューブシェイプ
	{
		private Vector4 baseColor;

		private Vector4 overlayColor;

		public OverlayCubeShape( Vector4 color, Vector4 overlayColor )
            : base( color )
		{
			this.baseColor = color;
			this.overlayColor = overlayColor;
		}

		public override void HitTestResult( bool result, bool mouseState, System.Drawing.Point mousePosition )
		{
			base.HitTestResult( result, mouseState, mousePosition );
			_color = result ? overlayColor : baseColor;
		}
	}
}
