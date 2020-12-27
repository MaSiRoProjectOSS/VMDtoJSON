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
            /// X軸とY軸をまとめたクラス
            /// </summary>
            public class MotionInterpolation_Rectangle<T>
            {
                /// <summary>
                /// 開始座標
                /// </summary>
                public MotionInterpolation_Coordinate<T> Start = new MotionInterpolation_Coordinate<T>();

                /// <summary>
                /// 終了座標
                /// </summary>
                public MotionInterpolation_Coordinate<T> Stop = new MotionInterpolation_Coordinate<T>();

                /// <summary>
                /// コンストラクタ
                /// </summary>
                /// <param name="startTime">開始座標のX軸</param>
                /// <param name="startAmount">開始座標のY軸</param>
                /// <param name="endTime">終了座標のX軸</param>
                /// <param name="endAmount">終了座標のY軸</param>
                public MotionInterpolation_Rectangle(T startTime, T startAmount, T endTime, T endAmount)
                {
                    this.Set(startTime, startAmount, endTime, endAmount);
                }

                /// <summary>
                /// コンストラクタ
                /// </summary>
                public MotionInterpolation_Rectangle()
                {
                }

                /// <summary>
                /// 設定関数
                /// </summary>
                /// <param name="startTime">開始座標のX軸</param>
                /// <param name="startAmount">開始座標のY軸</param>
                /// <param name="stopTime">終了座標のX軸</param>
                /// <param name="stopAmount">終了座標のY軸</param>
                public void Set(T startTime, T startAmount, T stopTime, T stopAmount)
                {
                    this.Start.Time = startTime;
                    this.Start.Amount = startAmount;
                    this.Stop.Time = stopTime;
                    this.Stop.Amount = stopAmount;
                }
            }

            /// <summary>
            /// X軸とY軸をまとめたクラス
            /// </summary>
            public class MotionInterpolation_Coordinate<T>
            {
                /// <summary>
                /// X軸
                /// </summary>
                public T Time;

                /// <summary>
                /// Y軸
                /// </summary>
                public T Amount;

                /// <summary>
                /// コンストラクタ
                /// </summary>
                /// <param name="time">X軸</param>
                /// <param name="amount">Y軸</param>
                public MotionInterpolation_Coordinate(T time, T amount)
                {
                    this.Set(time, amount);
                }

                /// <summary>
                /// コンストラクタ
                /// </summary>
                public MotionInterpolation_Coordinate()
                {
                }

                /// <summary>
                /// 設定関数
                /// </summary>
                /// <param name="time">X軸</param>
                /// <param name="amount">Y軸</param>
                public void Set(T time, T amount)
                {
                    this.Time = time;
                    this.Amount = amount;
                }
            }

            /// <summary>
            /// 拡張 フォーマット
            /// </summary>
            /// <remarks>
            ///   VMD ファイルフォーマット以外で、別途把握/操作するためのデータ
            /// </remarks>
            public class FORMAT_Expansion
            {
                /// <summary>
                /// 座標系
                /// </summary>
                public CoordinateSystemList CoordinateSystem = CoordinateSystemList.LeftHand;

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
                        return (int)inner_version;
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
                public enum CoordinateSystemList
                {
                    /// <summary>
                    /// 左手系
                    /// </summary>
                    LeftHand,

                    /// <summary>
                    /// 右手系
                    /// </summary>
                    RightHand,

                    /// <summary>
                    /// MMDの座標系
                    /// </summary>
                    MMDHand
                }

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
                ///    旧バージョンのシグニチャは未確認
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
}