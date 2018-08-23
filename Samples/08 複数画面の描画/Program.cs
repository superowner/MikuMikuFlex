﻿/*
 * *****************************************************************************************************************************************************************
 * MMFチュートリアル 08「MMFで複数の画面を描画する」
 * 
 * ◎このセクションの目的
 * 1,MMFでどのように複数の画面に対して描画するか習得する
 * 
 * ◎所要時間
 * 10分
 * 
 * ◎難易度
 * カンタン♪
 * 
 * ◎このチュートリアルの工程
 * ①～③
 * ・Form1.cs
 * ・ChildForm.cs
 * ・ControllerForm.cs
 * の3ファイルのみ
 * 
 * ◎必須ランタイム
 * DirectX エンドユーザーランタイム
 * .Net Framework 4.5
 * 
 * ◎終着点
 * モデルが出ればOK
 * 
 ********************************************************************************************************************************************************************/

using System;
using MikuMikuFlex;

namespace _08_MultiScreenRendering
{
	internal static class Program
	{
		/// <summary>
		///     アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		private static void Main()
		{
			MessagePump.Run( new Form1() );
		}
	}
}
