using MaSiRoProject.Common;
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
            /// カメラデータ フォーマット
            /// </summary>
            public class FORMAT_Camera
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
                /// フレーム毎に記録されたカメラデータ
                /// </summary>
                public List<Camera_Data> Data = new List<Camera_Data>();
            }

            /// <summary>
            /// カメラデータ
            ///      61 バイトのデータ
            /// </summary>
            public class Camera_Data
            {
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
                /// 距離
                /// </summary>
                /// <remarks>
                /// MMDの表示に -1 をかけた値
                /// </remarks>
                public float Length;

                /// <summary>
                ///  カメラの位置(x, y, Z)
                /// </summary>
                /// <bytesize>3つ×4 byte</bytesize>
                public Position<float> Location = new Position<float>();

                /// <summary>
                ///  カメラのオイラー角度(x, y, Z)
                /// </summary>
                /// <bytesize>3つ×4 byte</bytesize>
                /// <remarks>
                ///     X軸は符号が反転しているので注意
                /// </remarks>
                public AxisOfRotation<float> Rotation = new AxisOfRotation<float>();

                /// <summary>
                ///  補完データ
                /// </summary>
                public CameraInterpolation_Data<byte> Interpolation = new CameraInterpolation_Data<byte>();

                /// <summary>
                /// 視界角
                /// </summary>
                /// <bytesize>4</bytesize>
                public uint ViewingAngle;

                /// <summary>
                /// パースペクティブ
                ///       0:on 1:off
                /// </summary>
                /// <bytesize>1</bytesize>
                public bool Perspective;
            }

            /// <summary>
            /// カメラ用の補間パラメタのクラス
            /// </summary>
            public class CameraInterpolation_Data<T>
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

                /// <summary>
                /// 視界角の補間パラメタ
                ///      キーフレーム間のモーションを算出するためのパラメタ
                /// </summary>
                /// <bytesize>2つ×1 byte ×2軸</bytesize>
                public MotionInterpolation_Rectangle<T> ViewingAngle = new MotionInterpolation_Rectangle<T>();

                /// <summary>
                /// 距離の補間パラメタ
                ///      キーフレーム間のモーションを算出するためのパラメタ
                /// </summary>
                /// <bytesize>2つ×1 byte ×2軸</bytesize>
                public Rectangle<T> Length = new Rectangle<T>();
            }
        }
    }
}