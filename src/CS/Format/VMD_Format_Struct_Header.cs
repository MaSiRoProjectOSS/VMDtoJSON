namespace MaSiRoProject
{
    /// <summary>
    /// VMD フォーマットの構造
    /// </summary>
    public partial class VMD_Format_Struct
    {
        /// <summary>
        /// ヘッダー フォーマット
        /// </summary>
        /// <bytesize>50</bytesize>
        public class FORMAT_Header
        {
            /// <summary>
            /// [内部変数]VMDファイルのシグニチャ
            /// </summary>
            /// <bytesize>30</bytesize>
            private string inner_fileSignature = string.Empty;

            /// <summary>
            /// [読み取り専用] VMDファイルのシグニチャ
            /// </summary>
            public string FileSignature
            {
                get
                {
                    return inner_fileSignature;
                }
            }

            /// <summary>
            /// VMDファイルのシグニチャを設定する関数
            /// </summary>
            /// <param name="signature">ファイルのシグニチャ</param>
            public void SetFileSignature(string signature)
            {
                inner_fileSignature = signature;
            }

            /// <summary>
            ///  モーションデータ保存時のモデル名
            /// </summary>
            /// <bytesize>20</bytesize>
            /// <remarks>
            /// モデルやそのVersionでもボーン名などが違うので
            /// どのデータで保存したを特定するための情報
            /// </remarks>
            public string ModelName = string.Empty;
        }
    }
}