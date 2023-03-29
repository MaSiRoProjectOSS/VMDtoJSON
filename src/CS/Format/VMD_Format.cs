namespace MaSiRoProject
{
    namespace Format
    {
        /// <summary>
        /// VMD フォーマット
        /// </summary>
        public class VMD_Format : VMD_Format_Struct
        {
            /// <summary>
            /// スタートフレームを設定する関数
            /// </summary>
            /// <param name="startframe"></param>
            public void SetStartFrame(int startframe)
            {
                if (VMD_Format.StartFrame != startframe)
                {
                    VMD_Format.StartFrame = startframe;
                }
            }

            /// <summary>
            /// 単位設定
            /// </summary>
            /// <param name="value">定義</param>
            public void SetUnitOfLength(VMD_UNIT_LENGTH value)
            {
                VMD_Format.unit_of_length = value;
            }

            /// <summary>
            /// 単位
            /// </summary>
            public enum VMD_UNIT_LENGTH
            {
                /// <summary>
                /// MMDのデフォルトのまま
                /// </summary>
                VMD_UNIT_LENGTH_DEFAULT = 0,

                /// <summary>
                /// 単位を[cm]へ
                /// </summary>
                VMD_UNIT_LENGTH_CM = 1,

                /// <summary>
                /// 単位を[mm]へ
                /// </summary>
                VMD_UNIT_LENGTH_MM = 2,
            }

            /// <summary>
            /// モーション開始位置
            /// </summary>
            public static int StartFrame = 0;

            /// <summary>
            /// 出力長さ単位
            /// </summary>
            private static VMD_UNIT_LENGTH unit_of_length = VMD_UNIT_LENGTH.VMD_UNIT_LENGTH_DEFAULT;

            ////////////////////////////////////////////////////////////
            // 関数
            ////////////////////////////////////////////////////////////
            /// <summary>
            /// ファイルシグニチャを設定する関数
            /// </summary>
            /// <param name="signature"></param>
            public void SetFileSignature(string signature)
            {
                this.Header.SetFileSignature(signature);
                this.Expansion.SetFileSignature(signature);
            }

            /// <summary>
            /// 設定したスタートフレーム数分スライドさせる関数
            /// </summary>
            /// <param name="frameNo">フレーム番号</param>
            /// <returns></returns>
            public static uint ShiftFrameNo(uint frameNo)
            {
                if (-1 == VMD_Format.StartFrame)
                {
                    return frameNo;
                }
                else
                {
                    return (frameNo + (uint)VMD_Format.StartFrame);
                }
            }

            /// <summary>
            /// 単位取得
            /// </summary>
            /// <param name="value">返還前の値</param>
            /// <returns>返還後の値</returns>
            public static float GetLength(float value)
            {
                switch (VMD_Format.unit_of_length)
                {
                    case VMD_UNIT_LENGTH.VMD_UNIT_LENGTH_MM:
                        return value * 80.0f;

                    case VMD_UNIT_LENGTH.VMD_UNIT_LENGTH_CM:
                        return value * 8.0f;

                    default:
                        return value;
                }
            }

            ////////////////////////////////////////////////////////////
            // データ一覧
            ////////////////////////////////////////////////////////////
            /// <summary>
            /// ヘッダー データ
            /// </summary>
            public FORMAT_Header Header = new FORMAT_Header();

            /// <summary>
            /// モーションデータ
            /// </summary>
            public FORMAT_Motion Motion = new FORMAT_Motion();

            /// <summary>
            /// スキンデータ
            /// </summary>
            public FORMAT_Skin Skin = new FORMAT_Skin();

            /// <summary>
            /// カメラデータ
            /// </summary>
            public FORMAT_Camera Camera = new FORMAT_Camera();

            /// <summary>
            /// 照明データ
            /// </summary>
            public FORMAT_Illumination Illumination = new FORMAT_Illumination();

            /// <summary>
            /// セルフシャドウデータ
            /// </summary>
            public FORMAT_SelfShadow SelfShadow = new FORMAT_SelfShadow();

            /// <summary>
            /// IKデータ
            /// </summary>
            public FORMAT_IK IK = new FORMAT_IK();

            /// <summary>
            /// 拡張データ
            /// </summary>
            /// <remarks>
            ///   VMD ファイルフォーマット以外で、別途把握/操作するためのデータ
            /// </remarks>
            public FORMAT_Expansion Expansion = new FORMAT_Expansion();
        }

        // namespace
    }
}