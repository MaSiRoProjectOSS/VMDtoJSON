using MaSiRoProject.Common;
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
            /// 前後のボーンインデックス
            /// </summary>
            public class Index_Info
            {
                public int previous = -1;
                public int next = -1;
            }

            /// <summary>
            /// モーションデータ フォーマット
            /// </summary>
            public class FORMAT_Motion
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
                /// フレーム毎に記録されたモーションデータ
                /// </summary>
                public List<Motion_Data> Data = new List<Motion_Data>();
            }

            /// <summary>
            /// モーションデータ
            ///      111 バイトのデータ
            /// </summary>
            public class Motion_Data
            {
                /// <summary>
                /// 同名のボーンの前後インデックス情報
                /// </summary>
                public Index_Info IndexInfo = new Index_Info();

                /// <summary>
                ///  ボーン名
                /// </summary>
                /// <bytesize>15</bytesize>
                public string Name = string.Empty;// ByteSize 15

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
                ///  ボーンの位置(x, y, z)
                /// </summary>
                /// <bytesize>3つ×4 byte</bytesize>
                public Position<float> Location = new Position<float>();

                /// <summary>
                ///  ボーンの回転(Quaternion)
                /// </summary>
                /// <remarks>
                ///     VMDで保存されている値
                /// </remarks>
                /// <bytesize>4つ×4 byte</bytesize>
                public Quaternion<float> Quaternion = new Quaternion<float>();

                /// <summary>
                ///  ボーンの回転(Quaternion) 左手系
                /// </summary>
                /// <remarks>
                ///     左手系に変換したクォータニオン
                /// </remarks>
                public Quaternion<float> Quaternion_left = new Quaternion<float>();

                /// <summary>
                ///  ボーンの回転(Quaternion) 右手系
                /// </summary>
                /// <remarks>
                ///     右手系に変換したクォータニオン
                /// </remarks>
                public Quaternion<float> Quaternion_right = new Quaternion<float>();

                /// <summary>
                ///  ボーンの回転(オイラー角)
                /// </summary>
                /// <remarks>
                ///     MMDで表示されている値
                /// </remarks>
                public AxisOfRotation<float> Euler = new AxisOfRotation<float>();

                /// <summary>
                /// 補間パラメタ
                /// </summary>
                public MotionInterpolation_Data<byte> Interpolation = new MotionInterpolation_Data<byte>();

                /// <summary>
                /// 補間データのセット関数
                /// </summary>
                /// <param name="value">64byteの補間データ</param>
                public void SetMotionInterpolation(byte[] value)
                {
                    if (64 != value.Length)
                    {
                        throw new Exception("Motion.Interpolation : There is not enough data entered.");
                    }
                    /*
                     *   X  Y  Z  R  X  Y  Z  R  X  Y  Z  R  X  Y  Z  R
                         x  x  x  x  y  y  y  y  x  x  x  x  y  y  y  y
                        start------------------ end-------------------
                        06 15 00 00 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
                           15 21 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
                           00 21 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
                           00 00 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F 00 00 00
                   */
                    int index = 0;
                    Interpolation.Xaxis.Start.Time = value[index]; index++;
                    Interpolation.Yaxis.Start.Time = value[index]; index++;
                    Interpolation.Zaxis.Start.Time = value[index]; index++;
                    Interpolation.Rotation.Start.Time = value[index]; index++;

                    Interpolation.Xaxis.Start.Amount = value[index]; index++;
                    Interpolation.Yaxis.Start.Amount = value[index]; index++;
                    Interpolation.Zaxis.Start.Amount = value[index]; index++;
                    Interpolation.Rotation.Start.Amount = value[index]; index++;

                    Interpolation.Xaxis.Stop.Time = value[index]; index++;
                    Interpolation.Yaxis.Stop.Time = value[index]; index++;
                    Interpolation.Zaxis.Stop.Time = value[index]; index++;
                    Interpolation.Rotation.Stop.Time = value[index]; index++;

                    Interpolation.Xaxis.Stop.Amount = value[index]; index++;
                    Interpolation.Yaxis.Stop.Amount = value[index]; index++;
                    Interpolation.Zaxis.Stop.Amount = value[index]; index++;
                    Interpolation.Rotation.Stop.Amount = value[index]; index++;

                    if (0 == Interpolation.Yaxis.Start.Time) { Interpolation.Yaxis.Start.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Start.Time) { Interpolation.Zaxis.Start.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Start.Time) { Interpolation.Rotation.Start.Time = value[index]; }
                    index++;

                    if (0 == Interpolation.Xaxis.Start.Amount) { Interpolation.Xaxis.Start.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Yaxis.Start.Amount) { Interpolation.Yaxis.Start.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Start.Amount) { Interpolation.Zaxis.Start.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Start.Amount) { Interpolation.Rotation.Start.Amount = value[index]; }
                    index++;

                    if (0 == Interpolation.Xaxis.Stop.Time) { Interpolation.Xaxis.Stop.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Yaxis.Stop.Time) { Interpolation.Yaxis.Stop.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Stop.Time) { Interpolation.Zaxis.Stop.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Stop.Time) { Interpolation.Rotation.Stop.Time = value[index]; }
                    index++;

                    if (0 == Interpolation.Xaxis.Stop.Amount) { Interpolation.Xaxis.Stop.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Yaxis.Stop.Amount) { Interpolation.Yaxis.Stop.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Stop.Amount) { Interpolation.Zaxis.Stop.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Stop.Amount) { Interpolation.Rotation.Stop.Amount = value[index]; }
                    index++;

                    if (0 == Interpolation.Yaxis.Start.Time) { Interpolation.Yaxis.Start.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Start.Time) { Interpolation.Zaxis.Start.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Start.Time) { Interpolation.Rotation.Start.Time = value[index]; }
                    index++;

                    if (0 == Interpolation.Xaxis.Start.Amount) { Interpolation.Xaxis.Start.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Yaxis.Start.Amount) { Interpolation.Yaxis.Start.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Start.Amount) { Interpolation.Zaxis.Start.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Start.Amount) { Interpolation.Rotation.Start.Amount = value[index]; }
                    index++;

                    if (0 == Interpolation.Xaxis.Stop.Time) { Interpolation.Xaxis.Stop.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Yaxis.Stop.Time) { Interpolation.Yaxis.Stop.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Stop.Time) { Interpolation.Zaxis.Stop.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Stop.Time) { Interpolation.Rotation.Stop.Time = value[index]; }
                    index++;

                    if (0 == Interpolation.Xaxis.Stop.Amount) { Interpolation.Xaxis.Stop.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Yaxis.Stop.Amount) { Interpolation.Yaxis.Stop.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Stop.Amount) { Interpolation.Zaxis.Stop.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Stop.Amount) { Interpolation.Rotation.Stop.Amount = value[index]; }
                    index++;

                    if (0 == Interpolation.Yaxis.Start.Time) { Interpolation.Yaxis.Start.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Start.Time) { Interpolation.Zaxis.Start.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Start.Time) { Interpolation.Rotation.Start.Time = value[index]; }
                    index++;

                    if (0 == Interpolation.Xaxis.Start.Amount) { Interpolation.Xaxis.Start.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Yaxis.Start.Amount) { Interpolation.Yaxis.Start.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Start.Amount) { Interpolation.Zaxis.Start.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Start.Amount) { Interpolation.Rotation.Start.Amount = value[index]; }
                    index++;

                    if (0 == Interpolation.Xaxis.Stop.Time) { Interpolation.Xaxis.Stop.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Yaxis.Stop.Time) { Interpolation.Yaxis.Stop.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Stop.Time) { Interpolation.Zaxis.Stop.Time = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Stop.Time) { Interpolation.Rotation.Stop.Time = value[index]; }
                    index++;

                    if (0 == Interpolation.Xaxis.Stop.Amount) { Interpolation.Xaxis.Stop.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Yaxis.Stop.Amount) { Interpolation.Yaxis.Stop.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Zaxis.Stop.Amount) { Interpolation.Zaxis.Stop.Amount = value[index]; }
                    index++;
                    if (0 == Interpolation.Rotation.Stop.Amount) { Interpolation.Rotation.Stop.Amount = value[index]; }
                    index++;
                }

                /// <summary>
                /// 補間データ取得
                /// </summary>
                /// <returns>64バイトの補間データ</returns>
                public byte[] GetMotionInterpolation()
                {
                    /*
                     *   X  Y  Z  R  X  Y  Z  R  X  Y  Z  R  X  Y  Z  R
                         x  x  x  x  y  y  y  y  x  x  x  x  y  y  y  y
                        start------------------ end-------------------
                        06 15 00 00 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
                           15 21 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
                           00 21 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
                           00 00 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F 00 00 00
                   */
                    byte[] value = new byte[64];
                    int index = 0;

                    // 1回目
                    value[index++] = Interpolation.Xaxis.Start.Time;
                    value[index++] = Interpolation.Yaxis.Start.Time;
                    value[index++] = Interpolation.Zaxis.Start.Time;
                    value[index++] = Interpolation.Rotation.Start.Time;

                    value[index++] = Interpolation.Xaxis.Start.Amount;
                    value[index++] = Interpolation.Yaxis.Start.Amount;
                    value[index++] = Interpolation.Zaxis.Start.Amount;
                    value[index++] = Interpolation.Rotation.Start.Amount;

                    value[index++] = Interpolation.Xaxis.Stop.Time;
                    value[index++] = Interpolation.Yaxis.Stop.Time;
                    value[index++] = Interpolation.Zaxis.Stop.Time;
                    value[index++] = Interpolation.Rotation.Stop.Time;

                    value[index++] = Interpolation.Xaxis.Stop.Amount;
                    value[index++] = Interpolation.Yaxis.Stop.Amount;
                    value[index++] = Interpolation.Zaxis.Stop.Amount;
                    value[index++] = Interpolation.Rotation.Stop.Amount;

                    // 2回目
                    value[index++] = Interpolation.Yaxis.Start.Time;
                    value[index++] = Interpolation.Zaxis.Start.Time;
                    value[index++] = Interpolation.Rotation.Start.Time;

                    value[index++] = Interpolation.Xaxis.Start.Amount;
                    value[index++] = Interpolation.Yaxis.Start.Amount;
                    value[index++] = Interpolation.Zaxis.Start.Amount;
                    value[index++] = Interpolation.Rotation.Start.Amount;

                    value[index++] = Interpolation.Xaxis.Stop.Time;
                    value[index++] = Interpolation.Yaxis.Stop.Time;
                    value[index++] = Interpolation.Zaxis.Stop.Time;
                    value[index++] = Interpolation.Rotation.Stop.Time;

                    value[index++] = Interpolation.Xaxis.Stop.Amount;
                    value[index++] = Interpolation.Yaxis.Stop.Amount;
                    value[index++] = Interpolation.Zaxis.Stop.Amount;
                    value[index++] = Interpolation.Rotation.Stop.Amount;

                    /*
                     *   X  Y  Z  R  X  Y  Z  R  X  Y  Z  R  X  Y  Z  R
                         x  x  x  x  y  y  y  y  x  x  x  x  y  y  y  y
                        start------------------ end-------------------
                        06 15 00 00 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
                           15 21 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
                           00 21 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
                           00 00 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F 00 00 00
                   */

                    // 3回目
                    value[index++] = 0;
                    value[index++] = Interpolation.Zaxis.Start.Time;
                    value[index++] = Interpolation.Rotation.Start.Time;

                    value[index++] = Interpolation.Xaxis.Start.Amount;
                    value[index++] = Interpolation.Yaxis.Start.Amount;
                    value[index++] = Interpolation.Zaxis.Start.Amount;
                    value[index++] = Interpolation.Rotation.Start.Amount;

                    value[index++] = Interpolation.Xaxis.Stop.Time;
                    value[index++] = Interpolation.Yaxis.Stop.Time;
                    value[index++] = Interpolation.Zaxis.Stop.Time;
                    value[index++] = Interpolation.Rotation.Stop.Time;

                    value[index++] = Interpolation.Xaxis.Stop.Amount;
                    value[index++] = Interpolation.Yaxis.Stop.Amount;
                    value[index++] = Interpolation.Zaxis.Stop.Amount;
                    value[index++] = Interpolation.Rotation.Stop.Amount;

                    // 4回目
                    value[index++] = 0;
                    value[index++] = 0;
                    value[index++] = Interpolation.Rotation.Start.Time;

                    value[index++] = Interpolation.Xaxis.Start.Amount;
                    value[index++] = Interpolation.Yaxis.Start.Amount;
                    value[index++] = Interpolation.Zaxis.Start.Amount;
                    value[index++] = Interpolation.Rotation.Start.Amount;

                    value[index++] = Interpolation.Xaxis.Stop.Time;
                    value[index++] = Interpolation.Yaxis.Stop.Time;
                    value[index++] = Interpolation.Zaxis.Stop.Time;
                    value[index++] = Interpolation.Rotation.Stop.Time;

                    value[index++] = Interpolation.Xaxis.Stop.Amount;
                    value[index++] = Interpolation.Yaxis.Stop.Amount;
                    value[index++] = Interpolation.Zaxis.Stop.Amount;
                    value[index++] = Interpolation.Rotation.Stop.Amount;

                    // フッター
                    value[index++] = 0;
                    value[index++] = 0;
                    value[index++] = 0;

                    return value;
                }
            }

            /// <summary>
            /// モーション用の補間パラメタのクラス
            /// </summary>
            public class MotionInterpolation_Data<T>
            {
                /// <summary>
                ///  X軸の補間パラメタ
                ///      キーフレーム間のモーションを算出するためのパラメタ
                /// </summary>
                /// <bytesize>2つ×1 byte ×2軸</bytesize>
                public MotionInterpolation_Rectangle<T> Xaxis = new MotionInterpolation_Rectangle<T>();

                /// <summary>
                ///  Y軸の補間パラメタ
                ///      キーフレーム間のモーションを算出するためのパラメタ
                /// </summary>
                /// <bytesize>2つ×1 byte ×2軸</bytesize>
                public MotionInterpolation_Rectangle<T> Yaxis = new MotionInterpolation_Rectangle<T>();

                /// <summary>
                ///  Z軸の補間パラメタ
                ///      キーフレーム間のモーションを算出するためのパラメタ
                /// </summary>
                /// <bytesize>2つ×1 byte ×2軸</bytesize>
                public MotionInterpolation_Rectangle<T> Zaxis = new MotionInterpolation_Rectangle<T>();

                /// <summary>
                ///  回転の補間パラメタ
                ///      キーフレーム間のモーションを算出するためのパラメタ
                /// </summary>
                /// <bytesize>2つ×1 byte ×2軸</bytesize>
                public MotionInterpolation_Rectangle<T> Rotation = new MotionInterpolation_Rectangle<T>();

                /*
                public MotionInterpolation_Rectangle<T> unkown_5 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_6 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_7 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_8 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_9 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_10 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_11 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_12 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_13 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_14 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_15 = new MotionInterpolation_Rectangle<T>();

                public MotionInterpolation_Rectangle<T> unkown_16 = new MotionInterpolation_Rectangle<T>();
                */
            }
        }
    }
}