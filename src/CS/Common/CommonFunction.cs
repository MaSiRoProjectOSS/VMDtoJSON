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
    }
}