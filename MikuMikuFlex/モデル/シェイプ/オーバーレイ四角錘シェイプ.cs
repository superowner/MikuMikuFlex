﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace MikuMikuFlex.モデル.シェイプ
{
	class オーバーレイ四角錘シェイプ : 四角錐シェイプ
	{
		private readonly new Vector4 _color;

		private readonly Vector4 _overlayColor;

		public オーバーレイ四角錘シェイプ( Vector4 color, Vector4 overlayColor ) 
            : base( color )
		{
			_color = color;
			_overlayColor = overlayColor;
		}

		public override void HitTestResult( bool result, bool mouseState, System.Drawing.Point mousePosition )
		{
			base.HitTestResult( result, mouseState, mousePosition );
			base._color = result ? _overlayColor : _color;
		}
	}
}
