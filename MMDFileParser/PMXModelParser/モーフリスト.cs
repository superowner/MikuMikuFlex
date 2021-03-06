﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDFileParser.PMXModelParser
{
    public class モーフリスト : List<モーフ>
    {
        public モーフリスト()
            : base()
        {
        }
        public モーフリスト( int capacity )
            : base( capacity )
        {
        }

        /// <summary>
        ///     指定されたストリームから読み込む。
        /// </summary>
        internal static モーフリスト 読み込む( FileStream fs, PMXヘッダ header )
        {
            int モーフ数 = ParserHelper.get_Int( fs );
            Debug.WriteLine( $"モーフ数: {モーフ数}" );

            var list = new モーフリスト( モーフ数 );

            for( int i = 0; i < モーフ数; i++ )
                list.Add( モーフ.読み込む( fs, header ) );

            return list;
        }
    }
}
