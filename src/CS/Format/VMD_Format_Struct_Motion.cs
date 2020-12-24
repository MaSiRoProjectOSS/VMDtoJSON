using System;
using System.Collections.Generic;
using MaSiRoProject.Common;

namespace MaSiRoProject
{
    /// <summary>
    /// VMD フォーマットの構造
    /// </summary>
    public partial class VMD_Format_Struct
    {
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
            public uint FrameNo = 0; // ByteSize 4 フレーム番号

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
                Interpolation.Xaxis.Start.X = value[index]; index++;
                Interpolation.Yaxis.Start.X = value[index]; index++;
                Interpolation.Zaxis.Start.X = value[index]; index++;
                Interpolation.Rotation.Start.X = value[index]; index++;

                Interpolation.Xaxis.Start.Y = value[index]; index++;
                Interpolation.Yaxis.Start.Y = value[index]; index++;
                Interpolation.Zaxis.Start.Y = value[index]; index++;
                Interpolation.Rotation.Start.Y = value[index]; index++;

                Interpolation.Xaxis.End.X = value[index]; index++;
                Interpolation.Yaxis.End.X = value[index]; index++;
                Interpolation.Zaxis.End.X = value[index]; index++;
                Interpolation.Rotation.End.X = value[index]; index++;

                Interpolation.Xaxis.End.Y = value[index]; index++;
                Interpolation.Yaxis.End.Y = value[index]; index++;
                Interpolation.Zaxis.End.Y = value[index]; index++;
                Interpolation.Rotation.End.Y = value[index]; index++;

                if (0 == Interpolation.Yaxis.Start.X) { Interpolation.Yaxis.Start.X = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.Start.X) { Interpolation.Zaxis.Start.X = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.Start.X) { Interpolation.Rotation.Start.X = value[index]; }
                index++;

                if (0 == Interpolation.Xaxis.Start.Y) { Interpolation.Xaxis.Start.Y = value[index]; }
                index++;
                if (0 == Interpolation.Yaxis.Start.Y) { Interpolation.Yaxis.Start.Y = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.Start.Y) { Interpolation.Zaxis.Start.Y = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.Start.Y) { Interpolation.Rotation.Start.Y = value[index]; }
                index++;

                if (0 == Interpolation.Xaxis.End.X) { Interpolation.Xaxis.End.X = value[index]; }
                index++;
                if (0 == Interpolation.Yaxis.End.X) { Interpolation.Yaxis.End.X = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.End.X) { Interpolation.Zaxis.End.X = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.End.X) { Interpolation.Rotation.End.X = value[index]; }
                index++;

                if (0 == Interpolation.Xaxis.End.Y) { Interpolation.Xaxis.End.Y = value[index]; }
                index++;
                if (0 == Interpolation.Yaxis.End.Y) { Interpolation.Yaxis.End.Y = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.End.Y) { Interpolation.Zaxis.End.Y = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.End.Y) { Interpolation.Rotation.End.Y = value[index]; }
                index++;

                if (0 == Interpolation.Yaxis.Start.X) { Interpolation.Yaxis.Start.X = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.Start.X) { Interpolation.Zaxis.Start.X = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.Start.X) { Interpolation.Rotation.Start.X = value[index]; }
                index++;

                if (0 == Interpolation.Xaxis.Start.Y) { Interpolation.Xaxis.Start.Y = value[index]; }
                index++;
                if (0 == Interpolation.Yaxis.Start.Y) { Interpolation.Yaxis.Start.Y = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.Start.Y) { Interpolation.Zaxis.Start.Y = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.Start.Y) { Interpolation.Rotation.Start.Y = value[index]; }
                index++;

                if (0 == Interpolation.Xaxis.End.X) { Interpolation.Xaxis.End.X = value[index]; }
                index++;
                if (0 == Interpolation.Yaxis.End.X) { Interpolation.Yaxis.End.X = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.End.X) { Interpolation.Zaxis.End.X = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.End.X) { Interpolation.Rotation.End.X = value[index]; }
                index++;

                if (0 == Interpolation.Xaxis.End.Y) { Interpolation.Xaxis.End.Y = value[index]; }
                index++;
                if (0 == Interpolation.Yaxis.End.Y) { Interpolation.Yaxis.End.Y = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.End.Y) { Interpolation.Zaxis.End.Y = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.End.Y) { Interpolation.Rotation.End.Y = value[index]; }
                index++;

                if (0 == Interpolation.Yaxis.Start.X) { Interpolation.Yaxis.Start.X = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.Start.X) { Interpolation.Zaxis.Start.X = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.Start.X) { Interpolation.Rotation.Start.X = value[index]; }
                index++;

                if (0 == Interpolation.Xaxis.Start.Y) { Interpolation.Xaxis.Start.Y = value[index]; }
                index++;
                if (0 == Interpolation.Yaxis.Start.Y) { Interpolation.Yaxis.Start.Y = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.Start.Y) { Interpolation.Zaxis.Start.Y = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.Start.Y) { Interpolation.Rotation.Start.Y = value[index]; }
                index++;

                if (0 == Interpolation.Xaxis.End.X) { Interpolation.Xaxis.End.X = value[index]; }
                index++;
                if (0 == Interpolation.Yaxis.End.X) { Interpolation.Yaxis.End.X = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.End.X) { Interpolation.Zaxis.End.X = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.End.X) { Interpolation.Rotation.End.X = value[index]; }
                index++;

                if (0 == Interpolation.Xaxis.End.Y) { Interpolation.Xaxis.End.Y = value[index]; }
                index++;
                if (0 == Interpolation.Yaxis.End.Y) { Interpolation.Yaxis.End.Y = value[index]; }
                index++;
                if (0 == Interpolation.Zaxis.End.Y) { Interpolation.Zaxis.End.Y = value[index]; }
                index++;
                if (0 == Interpolation.Rotation.End.Y) { Interpolation.Rotation.End.Y = value[index]; }
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
                value[index++] = Interpolation.Xaxis.Start.X;
                value[index++] = Interpolation.Yaxis.Start.X;
                value[index++] = Interpolation.Zaxis.Start.X;
                value[index++] = Interpolation.Rotation.Start.X;

                value[index++] = Interpolation.Xaxis.Start.Y;
                value[index++] = Interpolation.Yaxis.Start.Y;
                value[index++] = Interpolation.Zaxis.Start.Y;
                value[index++] = Interpolation.Rotation.Start.Y;

                value[index++] = Interpolation.Xaxis.End.X;
                value[index++] = Interpolation.Yaxis.End.X;
                value[index++] = Interpolation.Zaxis.End.X;
                value[index++] = Interpolation.Rotation.End.X;

                value[index++] = Interpolation.Xaxis.End.Y;
                value[index++] = Interpolation.Yaxis.End.Y;
                value[index++] = Interpolation.Zaxis.End.Y;
                value[index++] = Interpolation.Rotation.End.Y;

                // 2回目
                value[index++] = Interpolation.Yaxis.Start.X;
                value[index++] = Interpolation.Zaxis.Start.X;
                value[index++] = Interpolation.Rotation.Start.X;

                value[index++] = Interpolation.Xaxis.Start.Y;
                value[index++] = Interpolation.Yaxis.Start.Y;
                value[index++] = Interpolation.Zaxis.Start.Y;
                value[index++] = Interpolation.Rotation.Start.Y;

                value[index++] = Interpolation.Xaxis.End.X;
                value[index++] = Interpolation.Yaxis.End.X;
                value[index++] = Interpolation.Zaxis.End.X;
                value[index++] = Interpolation.Rotation.End.X;

                value[index++] = Interpolation.Xaxis.End.Y;
                value[index++] = Interpolation.Yaxis.End.Y;
                value[index++] = Interpolation.Zaxis.End.Y;
                value[index++] = Interpolation.Rotation.End.Y;

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
                value[index++] = Interpolation.Zaxis.Start.X;
                value[index++] = Interpolation.Rotation.Start.X;

                value[index++] = Interpolation.Xaxis.Start.Y;
                value[index++] = Interpolation.Yaxis.Start.Y;
                value[index++] = Interpolation.Zaxis.Start.Y;
                value[index++] = Interpolation.Rotation.Start.Y;

                value[index++] = Interpolation.Xaxis.End.X;
                value[index++] = Interpolation.Yaxis.End.X;
                value[index++] = Interpolation.Zaxis.End.X;
                value[index++] = Interpolation.Rotation.End.X;

                value[index++] = Interpolation.Xaxis.End.Y;
                value[index++] = Interpolation.Yaxis.End.Y;
                value[index++] = Interpolation.Zaxis.End.Y;
                value[index++] = Interpolation.Rotation.End.Y;

                // 4回目
                value[index++] = 0;
                value[index++] = 0;
                value[index++] = Interpolation.Rotation.Start.X;

                value[index++] = Interpolation.Xaxis.Start.Y;
                value[index++] = Interpolation.Yaxis.Start.Y;
                value[index++] = Interpolation.Zaxis.Start.Y;
                value[index++] = Interpolation.Rotation.Start.Y;

                value[index++] = Interpolation.Xaxis.End.X;
                value[index++] = Interpolation.Yaxis.End.X;
                value[index++] = Interpolation.Zaxis.End.X;
                value[index++] = Interpolation.Rotation.End.X;

                value[index++] = Interpolation.Xaxis.End.Y;
                value[index++] = Interpolation.Yaxis.End.Y;
                value[index++] = Interpolation.Zaxis.End.Y;
                value[index++] = Interpolation.Rotation.End.Y;

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
            public Rectangle<T> Xaxis = new Rectangle<T>();

            /// <summary>
            ///  Y軸の補間パラメタ
            ///      キーフレーム間のモーションを算出するためのパラメタ
            /// </summary>
            /// <bytesize>2つ×1 byte ×2軸</bytesize>
            public Rectangle<T> Yaxis = new Rectangle<T>();

            /// <summary>
            ///  Z軸の補間パラメタ
            ///      キーフレーム間のモーションを算出するためのパラメタ
            /// </summary>
            /// <bytesize>2つ×1 byte ×2軸</bytesize>
            public Rectangle<T> Zaxis = new Rectangle<T>();

            /// <summary>
            ///  回転の補間パラメタ
            ///      キーフレーム間のモーションを算出するためのパラメタ
            /// </summary>
            /// <bytesize>2つ×1 byte ×2軸</bytesize>
            public Rectangle<T> Rotation = new Rectangle<T>();

            /*
            public Rectangle<T> unkown_5 = new Rectangle<T>();

            public Rectangle<T> unkown_6 = new Rectangle<T>();

            public Rectangle<T> unkown_7 = new Rectangle<T>();

            public Rectangle<T> unkown_8 = new Rectangle<T>();

            public Rectangle<T> unkown_9 = new Rectangle<T>();

            public Rectangle<T> unkown_10 = new Rectangle<T>();

            public Rectangle<T> unkown_11 = new Rectangle<T>();

            public Rectangle<T> unkown_12 = new Rectangle<T>();

            public Rectangle<T> unkown_13 = new Rectangle<T>();

            public Rectangle<T> unkown_14 = new Rectangle<T>();

            public Rectangle<T> unkown_15 = new Rectangle<T>();

            public Rectangle<T> unkown_16 = new Rectangle<T>();
            */
        }
    }
}