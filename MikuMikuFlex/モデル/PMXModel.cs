﻿// ***********************************************************************
// Assembly         : MikuMikuFlex
// Author           : Lime
// Created          : 01-17-2014
//
// Last Modified By : Lime
// Last Modified On : 02-02-2014
// ***********************************************************************
// <copyright file="MMDModel.cs" company="MMF Development Team">
//     Copyright (c) MMF Development Team. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Diagnostics;
using System.IO;
using MMDFileParser.PMXModelParser;
using MMF.ボーン;
using MMF.エフェクト;
using MMF.モーフ;
using MMF.モーション;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace MMF.モデル
{
	public class PMXModel : ISubsetDivided, IMovable, IEdgeDrawable, IGroundShadowDrawable
	{
		public サブリソースローダー サブリソースローダー { get; private set; }

        /// <summary>
        ///     PMXファイルから生成されたPMXモデル
        /// </summary>
		public PMXモデル モデル { get; private set; }

		public バッファ管理 バッファ管理 { get; private set; }

		public サブセット管理 サブセット管理 { get; private set; }

		public モーフ管理 モーフ管理 { get; private set; }

		public エフェクト管理 エフェクト管理 { get; private set; }

		public トゥーンテクスチャ管理 トゥーン管理 { get; private set; }


        /// <summary>
        ///     コンストラクタ。
        ///     初期化だけ行い、読み込み（Load）は行わない。
        /// </summary>
        public PMXModel( PMXモデル modeldata, サブリソースローダー subResourceLoader, string filename )
        {
            モデル = modeldata;
            サブリソースローダー = subResourceLoader;
            モデル状態 = new Transformer基本実装();
            サブセット管理 = new サブセット管理( this, modeldata );
            トゥーン管理 = new PMXトゥーンテクスチャ管理();
            セルフシャドウ色 = new Vector4( 0, 0, 0, 1 );
            地面影色 = new Vector4( 0, 0, 0, 1 );
            ファイル名 = filename;
            表示中 = true;
        }

        /// <summary>
        ///     デフォルトコンストラクタは非公開。
        /// </summary>
        private PMXModel()
        {
        }

        public void モデルを初期化する()
        {
            var sw = new Stopwatch();
            sw.Start();   // 読み込み時間の計測開始

            if( !_初期化済み )
            {
                // PMXファイルからのPMXモデルの読み込み → しない

                // シェーダーの読み込み

                トゥーン管理.初期化する( サブリソースローダー );

                サブセット管理.初期化する( トゥーン管理, サブリソースローダー );

                エフェクト管理 = エフェクト管理を初期化して返す();

                
                // 定数バッファ

                バッファ管理を初期化する();

                
                // スキニング

                スキニング = スキニングを初期化して返す();


                // モーフ管理

                モーフ管理 = new モーフ管理( this );


                _初期化済み = true;
            }


            // モーション管理

            モーション管理 = モーション管理を初期化して返す();


            // その他

            その他の初期化( RenderContext.Instance.DeviceManager.D3DDevice );


            sw.Stop();     // 読み込み時間の計測終了
            Trace.WriteLine( sw.ElapsedMilliseconds + "ms" );   // 読み込みにかかった時間をログ表示
        }

        public void エフェクトをファイルから読み込む( string filePath, サブリソースローダー loader = null, bool 既定にする = false )
        {
            if( null == loader )
            {
                if( Path.IsPathRooted( filePath ) )
                    loader = new サブリソースローダー( Path.GetDirectoryName( filePath ) );
                else
                    loader = サブリソースローダー;
            }

            var effect = エフェクト.エフェクト.ファイルをエフェクトとして読み込む( filePath, this, loader );

            エフェクト管理.エフェクトを登録する( effect, 既定にする );
        }


        public virtual void Dispose()
        {
            エフェクト管理?.Dispose();
            エフェクト管理 = null;

            バッファ管理?.Dispose();
            バッファ管理 = null;

            トゥーン管理?.Dispose();
            トゥーン管理 = null;

            サブセット管理?.Dispose();
            サブセット管理 = null;
        }


        // IDrawable の実装

        public bool 表示中 { get; set; }

		public string ファイル名 { get; set; }

        public int サブセット数 => サブセット管理.サブセットリストの要素数;

        public int 頂点数 => バッファ管理.頂点数;

        public Vector4 セルフシャドウ色 { get; set; }

        public Vector4 地面影色 { get; set; }

        public モデル状態 モデル状態 { get; private set; }

        public void 更新する()
        {
            バッファ管理?.必要であれば頂点を再作成する();

            モーフ管理.更新する();

            // モーフの更新結果をスキニングに反映
            スキニング.更新する( モーフ管理 );

            // モーフの更新結果をエフェクト用材質情報に反映
            foreach( var pmxSubset in サブセット管理.サブセットリスト )
                pmxSubset.エフェクト用材質情報.更新する();
        }

        public virtual void 描画する()
        {
            var IA = RenderContext.Instance.DeviceManager.D3DDevice.ImmediateContext.InputAssembler;

            // 既定のエフェクト
            エフェクト管理.既定のエフェクト.モデルごとに更新するエフェクト変数を更新する();

            スキニング.エフェクトを適用する( エフェクト管理.既定のエフェクト.D3DEffect );

            IA.SetVertexBuffers( 0, new VertexBufferBinding( バッファ管理.D3D頂点バッファ, 頂点レイアウト.SizeInBytes, 0 ) );
            IA.SetIndexBuffer( バッファ管理.D3Dインデックスバッファ, Format.R32_UInt, 0 );
            IA.InputLayout = バッファ管理.D3D頂点レイアウト;
            IA.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            サブセット管理.すべてを描画する( エフェクト管理.既定のエフェクト );

            // 既定じゃないエフェクト
            //if( エフェクト管理.エフェクトリスト.Count > 1 )
            //{
            //    foreach( var effect in エフェクト管理.エフェクトリスト )
            //    {
            //        if( effect != エフェクト管理.既定のエフェクト )
            //        {
            //            effect.モデルごとに更新するエフェクト変数を更新する();

            //            スキニングプロバイダ.エフェクトを適用する( effect.D3DEffect );

            //            IA.SetVertexBuffers( 0, new VertexBufferBinding( バッファ管理.D3D頂点バッファ, 頂点レイアウト.SizeInBytes, 0 ) );
            //            IA.SetIndexBuffer( バッファ管理.D3Dインデックスバッファ, Format.R32_UInt, 0 );
            //            IA.InputLayout = バッファ管理.D3D頂点レイアウト;
            //            IA.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            //            サブセット管理.すべてを描画する( effect );
            //        }
            //    }
            //}
        }


        // IEdgeDrawable, IGroundShadowDrawable の実装

        public void エッジを描画する()
        {
            // TODO: エッジ描画を実装する

            //MMEエフェクト管理.モデルごとに登録すべきエフェクト変数を割り当てる();

            //スキニングプロバイダ.エフェクトを適用する( MMEエフェクト管理.D3DEffect );

            //var IA = RenderContext.Instance.DeviceManager.D3DDevice.ImmediateContext.InputAssembler;
            //IA.SetVertexBuffers( 0, new VertexBufferBinding( バッファ管理.D3D頂点バッファ, 頂点レイアウト.SizeInBytes, 0 ) );
            //IA.SetIndexBuffer( バッファ管理.D3Dインデックスバッファ, Format.R32_UInt, 0 );
            //IA.InputLayout = バッファ管理.D3D頂点レイアウト;
            //IA.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            //サブセット管理.エッジを描画する();
        }


        // IGroundShadowDrawable の実装

        public void 地面影を描画する()
        {
            // TODO: 地面影描画を実装する

            //MMEエフェクト管理.モデルごとに登録すべきエフェクト変数を割り当てる();

            //スキニングプロバイダ.エフェクトを適用する( MMEエフェクト管理.D3DEffect );

            //var IA = RenderContext.Instance.DeviceManager.D3DDevice.ImmediateContext.InputAssembler;
            //IA.SetVertexBuffers( 0, new VertexBufferBinding( バッファ管理.D3D頂点バッファ, 頂点レイアウト.SizeInBytes, 0 ) );
            //IA.SetIndexBuffer( バッファ管理.D3Dインデックスバッファ, Format.R32_UInt, 0 );
            //IA.InputLayout = バッファ管理.D3D頂点レイアウト;
            //IA.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            //サブセット管理.地面影を描画する();
        }


        // IMovable の実装

        public モーション管理 モーション管理 { get; private set; }

        public スキニング スキニング { get; protected set; }

        /// <summary>
        ///     1000/30msごとに呼び出され、フレームを更新します。
        /// </summary>
        public virtual void 移動する()
        {
        }


        // 初期化系

		protected virtual スキニング スキニングを初期化して返す()
		{
			return new PMXスケルトン( モデル );
		}

		protected virtual モーション管理 モーション管理を初期化して返す()
		{
			var mm = new MMDモーション管理();

			mm.初期化する( モデル, モーフ管理, スキニング, バッファ管理 );

            ( (PMXスケルトン) スキニング ).変形更新リスト.Insert( 0, mm );   // 必ず先頭に突っ込む

            return mm;
		}

		protected virtual エフェクト管理 エフェクト管理を初期化して返す()
		{
            var effectManager = new エフェクト管理();

            var defaultEffect = エフェクト.エフェクト.リソースをエフェクトとして読み込む( エフェクト.エフェクト.既定のシェーダのリソースパス, this, this.サブリソースローダー );

            effectManager.エフェクトを登録する( defaultEffect, 既定にする: true );

            return effectManager;
		}

		protected virtual void その他の初期化( Device device )
		{
		}

		protected virtual void バッファ管理を初期化する()
		{
			バッファ管理 = new バッファ管理();
			バッファ管理.初期化する( モデル, エフェクト管理.既定のエフェクト.D3DEffect );
		}


        private bool _初期化済み { get; set; }


        // static: ファイルから開く（Openのみ、未Load）

        /// <summary>
        /// モデルファイルを開く
        /// </summary>
        /// <param name="filePath">PMXのファイルパス、テクスチャは同じフォルダから読み込まれる</param>
        /// <returns>MMDModelのインスタンス</returns>
        public static PMXModel ファイルから開く( string filePath )
		{
			string folder = Path.GetDirectoryName( filePath );
			return ファイルから開く( filePath, folder );
		}

		/// <summary>
		/// モデルファイルを開く
		/// </summary>
		/// <param name="filePath">PMXのファイルパス</param>
		/// <param name="textureFolder">テクスチャを読み込むフォルダ</param>
		/// <returns>MMDModelのインスタンス</returns>
		public static PMXModel ファイルから開く( string filePath, string textureFolder )
		{
			return ファイルから開く( filePath, new サブリソースローダー( textureFolder ) );
		}

		/// <summary>
		/// モデルファイルを開く
		/// </summary>
		/// <param name="filePath">PMXのファイルパス</param>
		/// <param name="loader">テクスチャのパス解決インターフェース</param>
		/// <returns>MMDModelのインスタンス</returns>
		public static PMXModel ファイルから開く( string filePath, サブリソースローダー loader )
		{
			using( FileStream fs = File.OpenRead( filePath ) )
			{
				return new PMXModel( PMXモデル.読み込む( fs ), loader, Path.GetFileName( filePath ) );
			}
		}


		// static: ファイルから読み込む（Open＆Load）

		/// <summary>
		/// 開いて初期化
		/// </summary>
		/// <param name="filePath">PMXのファイルパス</param>
		/// <param name="context">レンダリングコンテキスト</param>
		/// <returns>MMDModelのインスタンス</returns>
		public static PMXModel ファイルから読み込む( string filePath )
		{
			PMXModel model = ファイルから開く( filePath );
			model.モデルを初期化する();
			return model;
		}

		/// <summary>
		/// 開いて初期化
		/// </summary>
		/// <param name="filePath">PMXファイルのパス</param>
		/// <param name="textureFolder">テクスチャのフォルダ</param>
		/// <param name="context">レンダリングコンテキスト</param>
		/// <returns>MMDModelのインスタンス</returns>
		public static PMXModel ファイルから読み込む( string filePath, string textureFolder )
		{
			PMXModel model = ファイルから開く( filePath, textureFolder );
			model.モデルを初期化する();
			return model;
		}

		/// <summary>
		/// 開いて初期化
		/// </summary>
		/// <param name="filePath">PMXファイルのパス</param>
		/// <param name="loader">テクスチャなどのパスの解決インターフェース</param>
		/// <param name="context">レンダリングコンテキスト</param>
		/// <returns>MMDModelのインスタンス</returns>
		public static PMXModel ファイルから読み込む( string filePath, サブリソースローダー loader )
		{
			PMXModel model = ファイルから開く( filePath, loader );
			model.モデルを初期化する();
			return model;
		}
    }
}
