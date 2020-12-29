using System.Collections.Generic;

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
                private uint inner_FrameNo = 0;
                public uint FrameNo
                {
                    set { this.inner_FrameNo = value; }
                    get
                    {
                        return VMD_Format.ShiftFrameNo(this.inner_FrameNo);
                    }
                }

                /// <summary>
                ///  重み
                /// </summary>
                /// <bytesize>4</bytesize>
                public float Weight = 0;
            }
        }
    }
}