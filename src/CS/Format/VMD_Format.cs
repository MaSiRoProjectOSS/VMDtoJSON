namespace MaSiRoProject
{
    namespace Format
    {
        /// <summary>
        /// VMD フォーマット
        /// </summary>
        public class VMD_Format : VMD_Format_Struct
        {
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