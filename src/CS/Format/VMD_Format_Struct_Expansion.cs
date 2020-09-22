namespace MaSiRoProject
{
    /// <summary>
    /// VMD フォーマットの構造
    /// </summary>
    internal partial class VMD_Format_Struct
    {
        /// <summary>
        /// 拡張 フォーマット
        /// </summary>
        /// <remarks>
        ///   VMD ファイルフォーマット以外で、別途把握/操作するためのデータ
        /// </remarks>
        public class FORMAT_Expansion
        {
            /// <summary>
            /// モーション開始位置
            /// </summary>
            public int StartFrame = 0;

            /// <summary>
            /// ファイルタイプ
            /// </summary>
            public string FileType
            {
                get
                {
                    return inner_filetype;
                }
            }

            /// <summary>
            /// ターゲットID
            /// </summary>
            /// <remarks>
            ///    複数のデータを呼び出した際にどのデータが分からなくなるので
            ///    IDを指定できるようにするため。
            /// </remarks>
            public int TargetID
            {
                get
                {
                    return inner_targetID;
                }

                set
                {
                    inner_targetID = value;
                }
            }

            /// <summary>
            /// VMD バージョンの値
            /// </summary>
            /// <remarks>
            /// VMD ファイルシグニチャでも判断できるが、数字として取得するためのもの。
            /// </remarks>
            public int Version
            {
                get
                {
                    return (int) inner_version;
                }
            }

            ////////////////////////////////////////////////////////////
            // 定数
            ////////////////////////////////////////////////////////////
            /// <summary>
            /// ターゲットと未指定の場合の数値
            /// </summary>
            public const int TARGETID_NONE = -1;

            /// <summary>
            /// VMDを示すファイルタイプ
            /// </summary>
            public const string FILETYPE_VMD = "VMD";

            /// <summary>
            /// ファイルタイプが不明
            /// </summary>
            public const string FILETYPE_UNKNOWN = "UNKNOWN";

            ////////////////////////////////////////////////////////////
            // 列挙子
            ////////////////////////////////////////////////////////////
            public enum EXPANSION_VERSION
            {
                /// <summary>
                /// バージョン不明
                /// </summary>
                VMD_VERSION_UNKOWN = -1,

                /// <summary>
                /// Version 0000
                /// </summary>
                VMD_VERSION_000 = 0,

                /// <summary>
                /// Version 0100
                /// </summary>
                VMD_VERSION_001 = 1,

                /// <summary>
                /// Version 0200
                /// </summary>
                VMD_VERSION_002 = 2,
            }

            ////////////////////////////////////////////////////////////
            // 内部変数
            ////////////////////////////////////////////////////////////
            /// <summary>
            /// [内部変数] バージョン
            /// </summary>
            private EXPANSION_VERSION inner_version = EXPANSION_VERSION.VMD_VERSION_UNKOWN;

            /// <summary>
            /// [内部変数] ターゲットID
            /// </summary>
            private int inner_targetID = TARGETID_NONE;

            /// <summary>
            /// [内部変数] ファイルタイプ
            /// </summary>
            private string inner_filetype = FILETYPE_UNKNOWN;

            ////////////////////////////////////////////////////////////
            // 関数
            ////////////////////////////////////////////////////////////
            /// <summary>
            /// ファイルシグニチャからVersionを特定する関数
            /// </summary>
            /// <param name="signature">ファイルシグニチャ</param>
            /// <remarks>
            ///    TODO: 旧バージョンのシグニチャは未確認
            /// </remarks>
            public void SetFileSignature(string signature)
            {
                if (0 <= signature.IndexOf("Vocaloid Motion Data 0002"))
                {
                    this.inner_version = EXPANSION_VERSION.VMD_VERSION_002;
                    inner_filetype = FILETYPE_VMD;
                }
                else if (0 <= signature.IndexOf("Vocaloid Motion Data 0001"))
                {
                    this.inner_version = EXPANSION_VERSION.VMD_VERSION_001;
                    inner_filetype = FILETYPE_VMD;
                }
                else if (0 <= signature.IndexOf("Vocaloid Motion Data 0000"))
                {
                    this.inner_version = EXPANSION_VERSION.VMD_VERSION_000;
                    inner_filetype = FILETYPE_VMD;
                }
                else
                {
                    inner_filetype = FILETYPE_UNKNOWN;
                }
            }
        }
    }
}