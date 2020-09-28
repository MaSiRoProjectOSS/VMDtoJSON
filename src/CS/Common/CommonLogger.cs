using System;

namespace MaSiRoProject.Common
{
    /// <summary>
    /// ログ出力用のクラス
    /// </summary>
    internal class CommonLogger
    {
        /// <summary>
        /// ログレベル
        /// </summary>
        public enum LEVEL
        {
            /// <summary>
            /// レポート
            /// </summary>
            REPORT,

            /// <summary>
            /// 重大なエラー
            /// </summary>
            CRITICAL,

            /// <summary>
            /// エラー
            /// </summary>
            ERROR,

            /// <summary>
            /// 警告
            /// </summary>
            WARNING,

            /// <summary>
            /// 情報
            /// </summary>
            INFO,

            /// <summary>
            /// デバック情報
            /// </summary>
            DEBUG
        }

        /// <summary>
        /// ログレベルの指定
        /// </summary>
#if DEBUG

        public static LEVEL OutputBorderLevel = LEVEL.DEBUG;

#else

        public static LEVEL OutputBorderLevel = LEVEL.INFO;

#endif

        /// <summary>
        /// ログ出力
        ///     WARNING 以上はコンソールの色を変えて表示
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="message">メッセージ</param>
        /// <param name="notice">メッセージボックスの表示の有無(デフォルトはfalse)</param>
        public static void Log(LEVEL level, string message, bool notice = false)
        {
            if (LEVEL.REPORT == level)
            {
                Console.WriteLine(message);
            }
            else
            {
                if (OutputBorderLevel >= level)
                {
                    if (LEVEL.WARNING >= level)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine("[" + level.ToString() + "] " + message);
                        Console.ResetColor();// 色のリセット
                        /*
                        if (notice)
                        {
                            MessageBox.Show("[" + level.ToString() + "] " + Environment.NewLine + Environment.NewLine +
                                           "    " + message,
                                           System.Windows.Forms.Application.ProductName + " Ver." + System.Windows.Forms.Application.ProductVersion,
                                           MessageBoxButtons.OK,
                                           MessageBoxIcon.Error);
                        }
                        */
                    }
                    else
                    {
                        Console.WriteLine(message);
                    }
                }
            }
        }
    }
}