﻿using SharpDX;
using SharpDX.Direct3D11;

namespace MikuMikuFlex.エフェクト変数管理
{
	internal sealed class VIEWINVERSE変数 : 行列変数
	{
        public override string セマンティクス => "VIEWINVERSE";


        private VIEWINVERSE変数( Object種別 Object ) 
            : base( Object )
		{
		}

		public VIEWINVERSE変数()
		{
		}

        protected override 変数管理 行列変数登録インスタンスを生成して返す( Object種別 Object )
        {
            return new VIEWINVERSE変数( Object );
        }

        public override void 変数を更新する( EffectVariable 変数, 変数更新時引数 引数 )
		{
            switch( ターゲットオブジェクト )
            {
                case Object種別.カメラ:
                    行列を登録する( 
                        Matrix.Invert(
                            RenderContext.Instance.行列管理.ビュー行列管理.ビュー行列 ), 変数 );
                    break;

                case Object種別.ライト:
                    行列を登録する(
                        Matrix.Invert(
                            RenderContext.Instance.照明行列管理.ビュー行列管理.ビュー行列 ), 変数 );
                    break;
            }
		}
	}
}
