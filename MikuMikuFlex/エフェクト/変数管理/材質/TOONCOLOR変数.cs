﻿using SharpDX.Direct3D11;

namespace MikuMikuFlex.エフェクト変数管理
{
	internal sealed class TOONCOLOR変数 : 材質変数
	{
        public override string セマンティクス => "TOONCOLOR";

        public override 変数型[] 使える型の配列 => new[] { 変数型.Float4, 変数型.Float4 };


        private TOONCOLOR変数( ターゲット種別 target, bool isVector3 ) 
            : base( target, isVector3 )
		{
            if( target == ターゲット種別.ライト )
                throw new System.NotSupportedException( "Object として 'Light' を指定することはできません。" );
        }

        public TOONCOLOR変数()
        {
        }

        protected override 変数管理 材質変数登録インスタンスを生成して返す( ターゲット種別 target, bool isVector3 )
        {
            if( target == ターゲット種別.未使用 )
                target = ターゲット種別.ジオメトリ; // 省略値

            return new TOONCOLOR変数( target, isVector3 );
        }

        public override void 変数を更新する( EffectVariable 変数, 変数更新時引数 引数 )
		{
            switch( ターゲットオブジェクト )
            {
                case ターゲット種別.ジオメトリ:
                    エフェクト変数にベクトルを設定する( 引数.材質.トゥーン色, 変数, Vector3である );
                    break;

                case ターゲット種別.ライト:
                    break;  // 対応しない。
            }
		}
	}
}
