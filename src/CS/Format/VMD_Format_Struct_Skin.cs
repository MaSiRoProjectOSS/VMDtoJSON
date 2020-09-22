using System.Collections.Generic;

namespace MaSiRoProject
{
    /// <summary>
    /// VMD フォーマットの構造
    /// </summary>
    internal partial class VMD_Format_Struct
    {
        /// <summary>
        /// 表情データ フォーマット
        /// </summary>
        public class FORMAT_Skin
        {
            /// <summary>
            /// データ数
            /// </summary>
            public int Count
            {
                get
                {
                    return Data.Count;
                }
            }

            /// <summary>
            /// フレーム毎に記録された表情データ
            /// </summary>
            public List<Skin_Data> Data = new List<Skin_Data>();
        }

        /// <summary>
        /// 表情データ
        ///      23 バイトのデータ
        /// </summary>
        public class Skin_Data
        {
            /// <summary>
            ///  表情データ名
            /// </summary>
            /// <bytesize>15</bytesize>
            public string Name = string.Empty;

            /// <summary>
            ///  フレーム番号
            /// </summary>
            /// <bytesize>4</bytesize>
            /// <remarks>
            ///     現在のフレーム位置を0とした相対位置だが、構造体上はファイルの値とする
            /// </remarks>
            public uint FrameNo = 0; // 4 // フレーム番号(読込時は現在のフレーム位置を0とした相対位置)

            /// <summary>
            ///  重み
            /// </summary>
            /// <bytesize>4</bytesize>
            public float Weight = 0;
        }
    }
}