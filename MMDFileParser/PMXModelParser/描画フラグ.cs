﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDFileParser.PMXModelParser
{
    [Flags]
    public enum 描画フラグ
    {
        両面描画 = 0x01,
        地面影 = 0x02,
        セルフ影マップ = 0x04,
        セルフ影 = 0x08,
        エッジ = 0x10,
        /// <summary>
        ///     頂点の "追加UV1" の値(float4)を "照明計算後の色値" として描画に利用するモード
        /// </summary>
        頂点色 = 0x20,    // todo: 未対応

        /// <summary>
        ///     描画プリミティブを PointList で描画
        ///     ※Point／Line 双方とも ON の場合は Point 優先
        /// </summary>
        Point描画 = 0x40,     // todo: 未対応

        /// <summary>
        ///     描画プリミティブを LineList で描画
        ///     ※Point／Line 双方とも ON の場合は Point 優先
        /// </summary>
        Line描画 = 0x80,      // todo: 未対応
    }
}
