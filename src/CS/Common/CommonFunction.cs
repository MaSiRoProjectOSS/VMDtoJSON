using System;
using System.Text;

namespace MaSiRoProject.Common
{
    /// <summary>
    /// 共通関数のまとめたクラス
    /// </summary>
    internal class CommonFunction
    {
        /// <summary>
        /// バイト列を指定した文字コードで文字に変換する関数
        /// </summary>
        /// <param name="bytes">変換したバイト列</param>
        /// <param name="src_encoding">入力したバイト列の文字コード</param>
        /// <param name="dest_encoding">変換した文字コード</param>
        /// <returns></returns>
        public static string GetTextFromByte(Byte[] bytes, Encoding src_encoding, Encoding dest_encoding)
        {
            string buffer = dest_encoding.GetString(
                                    System.Text.Encoding.Convert(src_encoding,
                                    dest_encoding,
                                    bytes));
            int lastIndex = buffer.IndexOf("\0");

            if (0 < lastIndex)
            {
                return buffer.Substring(0, lastIndex);
            }
            else
            {
                return buffer;
            }
        }

        /// <summary>
        /// Radian to Degree 変換
        /// </summary>
        /// <param name="value">変換前の値</param>
        /// <returns>変換後の値</returns>
        public static float RadianToDegree(float radian)
        {
            //deg = rad∗( 180 / π )
            return CommonFunction.DegreeLimit180((float) ( radian * ( 180.0f / Math.PI ) ));
        }

        public static float DegreeLimit180(float value)
        {
            while (180.0f < value)
            {
                value = 360.0f - value;
            }
            while (-180.0f >= value)
            {
                value = 360.0f + value;
            }
            return value;
        }

        /// <summary>
        /// 少数点の有効な桁数まで四捨五入する関数
        /// </summary>
        /// <param name="decimals">少数点の有効な桁数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public static string GetRound(int decimals, float value)
        {
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero).ToString();
        }

        /// <summary>
        /// オイラー角度からクォータニオンに変換
        /// </summary>
        /// <param name="value">オイラー角</param>
        /// <returns>クォータニオン</returns>
        public static Quaternion<float> EulerAnglesToQuaternion(AxisOfRotation<float> value)
        {
            Quaternion<float> axis = new Quaternion<float>();
            double cosRoll = Math.Cos(value.Roll / 2.0);
            double sinRoll = Math.Sin(value.Roll / 2.0);
            double cosPitch = Math.Cos(value.Pitch / 2.0);
            double sinPitch = Math.Sin(value.Pitch / 2.0);
            double cosYaw = Math.Cos(value.Yaw / 2.0);
            double sinYaw = Math.Sin(value.Yaw / 2.0);

            axis.Set(
                (float) ( ( cosRoll * cosPitch * cosYaw ) + ( sinRoll * sinPitch * sinYaw ) ),
                (float) ( ( sinRoll * cosPitch * cosYaw ) - ( cosRoll * sinPitch * sinYaw ) ),
                (float) ( ( cosRoll * sinPitch * cosYaw ) + ( sinRoll * cosPitch * sinYaw ) ),
                (float) ( ( cosRoll * cosPitch * sinYaw ) - ( sinRoll * sinPitch * cosYaw ) )
            );
            Console.WriteLine("X : " + axis.X);
            Console.WriteLine("Y : " + axis.Y);
            Console.WriteLine("Z : " + axis.Z);
            Console.WriteLine("W : " + axis.W);
            return axis;
        }

        /// <summary>
        /// クォータニオンからオイラー角に変換
        /// </summary>
        /// <param name="value">クォータニオン</param>
        /// <returns>オイラー角</returns>
        public static AxisOfRotation<float> QuaternionToEulerAngles(Quaternion<float> value)
        {
            AxisOfRotation<float> axis = new AxisOfRotation<float>();

            double x2z2 = Math.Pow(value.X, 2) - Math.Pow(value.Z, 2);
            double y2w2 = Math.Pow(value.Y, 2) - Math.Pow(value.W, 2);

            axis.Set(
                    (float) Math.Atan2(2.0 * ( ( value.X * value.Y ) + ( value.Z * value.W ) ), x2z2 - y2w2),
                    (float) Math.Asin(2.0 * ( ( value.X * value.Z ) - ( value.Y * value.W ) )),
                    (float) Math.Atan2(2.0 * ( ( value.Y * value.Z ) + ( value.X * value.W ) ), x2z2 + y2w2)
                );

            Console.WriteLine("pitch : " + CommonFunction.GetRound(3, CommonFunction.RadianToDegree((float) axis.Pitch)));
            Console.WriteLine("yaw   : " + CommonFunction.GetRound(3, CommonFunction.RadianToDegree((float) axis.Yaw)));
            Console.WriteLine("roll  : " + CommonFunction.GetRound(3, CommonFunction.RadianToDegree((float) axis.Roll)));

            return axis;
        }
    }
}