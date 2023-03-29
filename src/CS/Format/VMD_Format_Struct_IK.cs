﻿using System.Collections.Generic;

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
            /// IKデータ フォーマット
            /// </summary>
            public class FORMAT_IK
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
                /// フレーム毎に記録されたIKデータ
                /// </summary>
                public List<IK_VISIBLE_Data> Data = new List<IK_VISIBLE_Data>();
            }

            /// <summary>
            /// IK データ
            /// </summary>
            public class IK_VISIBLE_Data
            {
                /// <summary>
                ///  フレーム番号
                ///  (現在のフレーム位置を0とした相対位置だが、構造体上はファイルの値とする)
                /// </summary>
                /// <bytesize>4</bytesize>
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
                /// 表示
                ///     0:off 1:on
                /// </summary>
                public bool Visible = false;

                /// <summary>
                /// データ数
                /// </summary>
                public int IKCount
                {
                    get
                    {
                        return Data.Count;
                    }
                }

                /// <summary>
                /// フレーム毎に記録されたIKデータ
                /// </summary>
                public List<IK_Data> Data = new List<IK_Data>();
            }

            /// <summary>
            /// 有効IK データ
            /// </summary>

            public class IK_Data
            {
                /// <summary>
                /// IKボーン名
                /// </summary>
                public string ikBoneName = string.Empty;

                /// <summary>
                /// IK有効
                ///      0:off 1:on
                /// </summary>
                public bool ikEnabled = false;
            }
        }
    }
}