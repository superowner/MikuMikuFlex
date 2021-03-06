﻿using System;
using System.IO;

namespace MMDFileParser.PMXModelParser.JointParam
{
    public abstract class ジョイントパラメータ
    {
        /// <summary>
        ///     指定されたストリームから読み込む。
        /// </summary>
        internal static ジョイントパラメータ 読み込む( Stream fs, PMXヘッダ header, ジョイント.ジョイント種別 type )
        {
            switch( type )
            {
                // PMX 2.0
                case ジョイント.ジョイント種別.ばね付き6DOF:
                    var sp6 = new ばね付き6DOFジョイントパラメータ();
                    sp6.読み込む( fs, header );
                    return sp6;

                // PMX 2.1
                case ジョイント.ジョイント種別.基本6DOF:
                case ジョイント.ジョイント種別.P2P:
                case ジョイント.ジョイント種別.円錐回転:
                case ジョイント.ジョイント種別.スライダー:
                case ジョイント.ジョイント種別.ヒンジ:
                    throw new NotSupportedException( "PMX2.1 以降には未対応です。" );     // todo: これらの実装

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        ///     指定されたストリームから読み込む。（派生クラスで実装のこと。）
        /// </summary>
        internal abstract void 読み込む( Stream fs, PMXヘッダ header );
    }
}