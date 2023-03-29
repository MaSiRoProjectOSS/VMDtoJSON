using System;
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
            /// セルフシャドウデータ フォーマット
            /// </summary>
            public class FORMAT_SelfShadow
            {
                //9 × レコード数  シャドウデータ   フレーム毎に記録
                // unsigned int 4バイト
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
                /// フレーム毎に記録されたセルフシャドウデータ
                /// </summary>
                public List<SelfShadow_Data> Data = new List<SelfShadow_Data>();
            }

            /// <summary>
            /// セルフ影データ
            ///      9 バイトのデータ
            /// </summary>
            public class SelfShadow_Data
            {
                /// <summary>
                /// [内部変数] モード
                /// </summary>
                private SelfShadow_MODE inner_mode = SelfShadow_MODE.MODE_NONE;

                /// <summary>
                ///  フレーム番号
                /// </summary>
                /// <bytesize>4</bytesize>
                /// <remarks>
                ///     現在のフレーム位置を0とした相対位置だが、構造体上はファイルの値とする
                /// </remarks>
                private uint inner_FrameNo = 0;

                /// <summary>
                ///  フレーム番号
                /// </summary>
                /// <bytesize>4</bytesize>
                /// <remarks>
                ///     現在のフレーム位置を0とした相対位置だが、構造体上はファイルの値とする
                /// </remarks>
                public uint FrameNo
                {
                    set { this.inner_FrameNo = value; }
                    get
                    {
                        return VMD_Format.ShiftFrameNo(this.inner_FrameNo);
                    }
                }

                /// <summary>
                ///  モード
                /// </summary>
                /// <bytesize>1</bytesize>
                public SelfShadow_MODE Mode
                {
                    get
                    {
                        return inner_mode;    // 00-02 // モード
                    }
                }

                /// <summary>
                ///  距離（設定値）
                /// </summary>
                /// <bytesize>4</bytesize>
                public float Distance_Value = 0;

                /// <summary>
                ///  距離（出力値 = 0.1 - (dist * 0.00001)）
                /// </summary>
                public float Distance
                {
                    get
                    {
                        return (0.1f - this.Distance_Value) * 100000;
                    }
                }

                /// <summary>
                ///  モード値の設定する関数
                /// </summary>
                public void SetMode(Byte mode)
                {
                    switch (mode)
                    {
                        case 1:
                            inner_mode = SelfShadow_MODE.MODE_FIRST;
                            break;

                        case 2:
                            inner_mode = SelfShadow_MODE.MODE_SECOND;
                            break;

                        default:
                            inner_mode = SelfShadow_MODE.MODE_NONE;
                            break;
                    }
                }

                /// <summary>
                ///  モード値の戻り値
                /// </summary>
                public enum SelfShadow_MODE
                {
                    /// <summary>
                    ///  影なし
                    /// </summary>
                    MODE_NONE = 0,

                    /// <summary>
                    ///  モード１
                    /// </summary>
                    MODE_FIRST = 1,

                    /// <summary>
                    ///  モード２
                    /// </summary>
                    MODE_SECOND = 2
                }
            }
        }
    }
}