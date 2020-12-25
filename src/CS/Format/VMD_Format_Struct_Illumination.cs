using MaSiRoProject.Common;
using System.Collections.Generic;
using System.Drawing;

namespace MaSiRoProject
{
    namespace Format
    {
        /// <summary>
        /// VMD フォーマットの構造
        /// </summary>
        public partial class VMD_Format_Struct
        {
            /// <summary>
            /// 照明データ フォーマット
            /// </summary>
            public class FORMAT_Illumination
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
                /// フレーム毎に記録された照明データ
                /// </summary>
                public List<Illumination_Data> Data = new List<Illumination_Data>();
            }

            /// <summary>
            /// 照明データ
            ///      28 バイトのデータ
            /// </summary>
            public class Illumination_Data
            {
                /// <summary>
                ///  フレーム番号
                /// </summary>
                /// <bytesize>4</bytesize>
                /// <remarks>
                ///     現在のフレーム位置を0とした相対位置だが、構造体上はファイルの値とする
                /// </remarks>
                public uint FrameNo = 0; // 4 // フレーム番号(読込時は現在のフレーム位置を0とした相対位置)

                /// <summary>
                /// カラーデータ
                /// </summary>
                /// <bytesize>3つ×4 byte</bytesize>
                /// <remarks>
                ///     保存データはFloat型で value*256したのが一般的に使われるRGB
                /// </remarks>
                public Color RGB = Color.FromArgb(255, 255, 255, 255);

                /// <summary>
                /// 照明位置(x,y,z)
                /// </summary>
                /// <bytesize>3つ×4 byte</bytesize>
                public Position<float> Location = new Position<float>();
            }
        }
    }
}