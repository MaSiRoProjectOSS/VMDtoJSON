using System;
using System.Drawing;
using System.IO;
using System.Text;
using MaSiRoProject.Common;

namespace MaSiRoProject
{
    /// <summary>
    /// VMD ファイルからJsonへ変換するためのクラス
    /// </summary>
    internal class VMDtoJSON
    {
        #region 出力設定

        /// <summary>
        /// 少数点の有効な桁数 [照明のLocation]
        /// </summary>
        private const int DECIMALS_ILLUMINATION = 1;

        /// <summary>
        /// 少数点の有効な桁数 [Location]
        /// </summary>
        private const int DECIMALS_POSITION = 2;

        /// <summary>
        /// 少数点の有効な桁数 [ROTATION]
        /// </summary>
        private const int DECIMALS_ROTATION = 1;

        /// <summary>
        /// 少数点の有効な桁数 [ROTATION] with Motion
        /// </summary>
        private const int DECIMALS_ROTATION_MOTION = 1;

        #endregion 出力設定

        #region 文字コード

        /// <summary>
        /// VMD ファイル内の文字コード
        /// </summary>
        private Encoding text_encoding_sjis = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// JSONの文字コード
        /// </summary>
        private Encoding text_encoding_utf8 = new UTF8Encoding(false);

        #endregion 文字コード

        #region データ

        /// <summary>
        /// 指定されたファイルの変換したVMDフォーマットデータ
        /// </summary>
        private VMD_Format VMD_Data = new VMD_Format();

        /// <summary>
        /// 文字化したVMDフォーマットのデータ
        /// </summary>
        private StringBuilder sb_VMD_Data = new StringBuilder();

        /// <summary>
        /// JSONの出力タイプ
        /// </summary>
        private bool innner_minimumJson = false;

        /// <summary>
        /// ファイル出力中のロックオブジェクト
        /// </summary>
        private readonly object lock_outputfile = new object();

        #endregion データ

        #region 設定関数

        /// <summary>
        /// 座標系を指定するための関数
        /// </summary>
        /// <param name="coordinatesystem">座標系</param>
        public void SetCoordinateSystem(VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList value)
        {
            if (value != this.VMD_Data.Expansion.CoordinateSystem)
            {
                this.VMD_Data.Expansion.CoordinateSystem = value;
            }
        }

        /// <summary>
        /// ターゲットIDを指定するための関数
        /// </summary>
        /// <param name="targetid">ターゲットID</param>
        public void SetTargetID(int targetid)
        {
            if (this.VMD_Data.Expansion.TargetID != targetid)
            {
                this.VMD_Data.Expansion.TargetID = targetid;
            }
        }

        /// <summary>
        /// スタートフレームを設定する関数
        /// </summary>
        /// <param name="startframe"></param>
        public void SetStartFram(int startframe)
        {
            if (this.VMD_Data.Expansion.StartFrame != startframe)
            {
                this.VMD_Data.Expansion.StartFrame = startframe;
            }
        }

        /// <summary>
        /// 出力するJSONのタイプを指定する関数
        /// </summary>
        /// <param name="startframe"></param>
        public void SetOutputJsonType(bool minimumJson)
        {
            innner_minimumJson = minimumJson;
        }

        #endregion 設定関数

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VMDtoJSON()
        { }

        #endregion コンストラクタ

        #region 設定関数

        /// <summary>
        /// 設定の登録
        /// </summary>
        /// <param name="startframe">スタートフレーム番号</param>
        /// <param name="minimumJson">JSONの出力タイプ：trueならば改行がないタイプのJSON</param>
        /// <param name="targetid">ターゲットID</param>
        /// <param name="coordinatesystem">座標系</param>
        /// <returns></returns>
        public bool Setting(
            int startframe = 0,
            bool minimumJson = false,
            int targetid = VMD_Format.FORMAT_Expansion.TARGETID_NONE,
            VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList coordinatesystem = VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand)
        {
            this.SetCoordinateSystem(coordinatesystem);
            this.SetOutputJsonType(minimumJson);
            this.SetTargetID(targetid);
            this.SetStartFram(startframe);
            return true;
        }

        #endregion 設定関数

        #region 出力関数

        /// <summary>
        /// JSON ファイルの出力
        /// </summary>
        /// <param name="output_filepath">出力したいJSONファイルのパス</param>
        /// <returns></returns>
        public bool OutputFile(string output_filepath)
        {
            bool flag_notice = false;
#if DEBUG
            flag_notice = true;
#endif
            lock (lock_outputfile)
            {
                try
                {
                    if (string.Empty != output_filepath)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "==================");
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Output file --");
                        FileInfo fileInfo = new FileInfo(output_filepath);
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, " File path : " + fileInfo.FullName);

                        // ファイルの書き込み
                        StreamWriter writer = new StreamWriter(fileInfo.FullName, false, text_encoding_utf8);
                        writer.Write(sb_VMD_Data.ToString());
                        writer.Close();
                    }
                    DateTime dt = DateTime.Now;
                    CommonLogger.Log(CommonLogger.LEVEL.INFO,
                          " Completion time : "
                        + dt.ToString("yyyy/MM/dd HH:mm:ss"));
                }
                catch (Exception ex)
                {
                    DateTime dt = DateTime.Now;
                    Console.WriteLine(dt);
                    CommonLogger.Log(CommonLogger.LEVEL.ERROR,
                        "An error occurred while outputting the file." + System.Environment.NewLine
                        + "  TargetID : " + VMD_Data.Expansion.TargetID.ToString() + System.Environment.NewLine
                        + "  Happened : " + dt.ToString("yyyy/MM/dd HH:mm:ss") + System.Environment.NewLine
                    + System.Environment.NewLine
                        + ex.Message, flag_notice);
                }
            }
            return true;
        }

        /// <summary>
        /// JSON テキストの取得
        /// </summary>
        /// <returns></returns>
        public string GetJsonTest()
        {
            return sb_VMD_Data.ToString();
        }

        #endregion 出力関数

        #region Converter (構造体からJson)

        /// <summary>
        /// ファイル変換（JSON出力機能付き）
        /// </summary>
        /// <param name="vmd_filepath">変換したいVMDファイルのパス</param>
        /// <param name="output_filepath">出力したいJSONファイルのパス</param>
        /// <returns></returns>
        public bool Convert(string vmd_filepath, string output_filepath)
        {
            bool retflag = true;

            if (true == retflag)
            {
                retflag = this.Convert(vmd_filepath);
            }

            if (true == retflag)
            {
                retflag = this.OutputFile(output_filepath);
            }

            return retflag;
        }

        /// <summary>
        /// ファイル変換
        /// </summary>
        /// <param name="vmd_filepath">変換したいVMDファイルのパス</param>
        public bool Convert(string vmd_filepath)
        {
            bool retflag = true;
            string err_message = string.Empty;
            bool flag_notice = false;
#if DEBUG
            flag_notice = true;
#endif

            if (true == retflag)
            {
                retflag = this.Convert_VMDFileToStruct(vmd_filepath, ref err_message);
            }

            if (true == retflag)
            {
                retflag = this.Convert_StructToJson(innner_minimumJson, ref err_message);
            }

            if (false == retflag)
            {
                CommonLogger.Log(CommonLogger.LEVEL.ERROR, err_message, flag_notice);
            }

            return retflag;
        }

        /// <summary>
        /// 構造体からJsonへ変換する関数
        /// </summary>
        /// <param name="minimumJson">JSONの出力タイプ：trueならば改行がないタイプのJSON</param>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <returns></returns>
        private bool Convert_StructToJson(bool minimumJson, ref string err_message)
        {
            bool retReturn = true;

            try
            {
                lock (lock_outputfile)
                {
                    CommonLogger.Log(CommonLogger.LEVEL.INFO, "==================");
                    CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Write Json --");
                    this.sb_VMD_Data.Clear();
                    sb_VMD_Data.Append("{" + ( minimumJson ? "" : Environment.NewLine ));

                    if (true == retReturn)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.DEBUG, "    - [Header]");

                        if (false == this.Convert_StructToJson_Header(ref err_message, minimumJson)) { retReturn = false; }
                    }

                    if (true == retReturn)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.DEBUG, "    - [Motion]");

                        if (false == this.Convert_StructToJson_Motion(ref err_message, minimumJson)) { retReturn = false; }
                    }

                    if (true == retReturn)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.DEBUG, "    - [Skin]");

                        if (false == this.Convert_StructToJson_Skin(ref err_message, minimumJson)) { retReturn = false; }
                    }

                    if (true == retReturn)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.DEBUG, "    - [Camera]");

                        if (false == this.Convert_StructToJson_Camera(ref err_message, minimumJson)) { retReturn = false; }
                    }

                    if (true == retReturn)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.DEBUG, "    - [Illumination]");

                        if (false == this.Convert_StructToJson_Illumination(ref err_message, minimumJson)) { retReturn = false; }
                    }

                    if (true == retReturn)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.DEBUG, "    - [SelfShadow]");

                        if (false == this.Convert_StructToJson_SelfShadow(ref err_message, minimumJson)) { retReturn = false; }
                    }

                    if (true == retReturn)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.DEBUG, "    - [IK]");

                        if (false == this.Convert_StructToJson_IK(ref err_message, minimumJson)) { retReturn = false; }
                    }

                    if (true == retReturn)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.DEBUG, "    - [Expansion]");

                        if (false == this.Convert_StructToJson_Expansion(ref err_message, minimumJson)) { retReturn = false; }
                    }

                    sb_VMD_Data.Append("}");
                }
            }
            catch (Exception ex)
            {
                //CommonLogger.Log(CommonLogger.LEVEL.ERROR, ex.Message, false);
                err_message = ex.Message;
                retReturn = false;
            }

            return retReturn;
        }

        /// <summary>
        /// ヘッダのJSONテキストを生成
        /// </summary>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <param name="minimumJson">false:スペースや改行をいれる</param>
        /// <returns>true: エラーなし</returns>
        private bool Convert_StructToJson_Header(ref string err_message, bool minimumJson = false)
        {
            sb_VMD_Data.Append(
                            ( minimumJson ? "" : "  " ) + "\"Header\": {" + ( minimumJson ? "" : Environment.NewLine ) +
                            ( minimumJson ? "" : "      " ) + "\"FileSignature\": " + "\"" + this.VMD_Data.Header.FileSignature + "\"," + ( minimumJson ? "" : Environment.NewLine ) +
                            ( minimumJson ? "" : "      " ) + "\"ModelName\": " + "\"" + this.VMD_Data.Header.ModelName + "\"" + ( minimumJson ? "" : Environment.NewLine ) +
                            ( minimumJson ? "" : "  " ) + "}," + ( minimumJson ? "" : Environment.NewLine )
            );
            return true;
        }

        /// <summary>
        /// モーションのJSONテキストを生成
        /// </summary>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <param name="minimumJson">false:スペースや改行をいれる</param>
        /// <returns>true: エラーなし</returns>
        private bool Convert_StructToJson_Motion(ref string err_message, bool minimumJson = false)
        {
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "\"Motion\": {" + ( minimumJson ? "" : Environment.NewLine ));
            int count = this.VMD_Data.Motion.Count;
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Count\": " + count + "," + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Data\": [" + ( minimumJson ? "" : Environment.NewLine ));

            for (int i = 0; i < count; i++)
            {
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "{" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.Motion.Data[i].FrameNo) + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Name\": " + "\"" + this.VMD_Data.Motion.Data[i].Name + "\"," + ( minimumJson ? "" : Environment.NewLine ));

                switch (this.VMD_Data.Expansion.CoordinateSystem)
                {
                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand:
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Z)
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Rotation\": {" + ( minimumJson ? "" : Environment.NewLine ));

                        sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Quaternion\": ["
                                             + this.VMD_Data.Motion.Data[i].Quaternion_left.X + ", "
                                             + ( this.VMD_Data.Motion.Data[i].Quaternion_left.Y ) + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion_left.Z + ", "
                                             + ( this.VMD_Data.Motion.Data[i].Quaternion_left.W )
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Euler\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree(this.VMD_Data.Motion.Data[i].Euler.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree(-this.VMD_Data.Motion.Data[i].Euler.Yaw)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree(-this.VMD_Data.Motion.Data[i].Euler.Roll))
                                             + "]" + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.RightHand:
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, -this.VMD_Data.Motion.Data[i].Location.Z) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Y)
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Rotation\": {" + ( minimumJson ? "" : Environment.NewLine ));

                        sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Quaternion\": ["
                                             + ( this.VMD_Data.Motion.Data[i].Quaternion_right.X ) + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion_right.Y + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion_right.Z + ", "
                                             + ( this.VMD_Data.Motion.Data[i].Quaternion_right.W )
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Euler\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree(-this.VMD_Data.Motion.Data[i].Euler.Roll)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree(-this.VMD_Data.Motion.Data[i].Euler.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree(this.VMD_Data.Motion.Data[i].Euler.Yaw))
                                             + "]" + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.MMDHand:
                    default:
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Z)
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Rotation\": {" + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Quaternion\": ["
                                             + this.VMD_Data.Motion.Data[i].Quaternion.X + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion.Y + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion.Z + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion.W
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Euler\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree(this.VMD_Data.Motion.Data[i].Euler.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree(this.VMD_Data.Motion.Data[i].Euler.Yaw)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree(this.VMD_Data.Motion.Data[i].Euler.Roll))
                                             + "]" + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
                        break;
                }
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Interpolation\": {" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"X\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Xaxis.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Xaxis.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Xaxis.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Xaxis.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Y\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Yaxis.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Yaxis.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Yaxis.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Yaxis.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Z\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Zaxis.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Zaxis.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Zaxis.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Zaxis.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Rotation\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Rotation.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Rotation.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Rotation.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Rotation.End.Y + "]"
                                     + "}" + ( minimumJson ? "" : Environment.NewLine ));
                /*

                // 同じ値なので不要なので未出力
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_5\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_5.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_5.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_5.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_5.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_6\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_6.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_6.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_6.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_6.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_7\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_7.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_7.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_7.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_7.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_8\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_8.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_8.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_8.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_8.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_9\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_9.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_9.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_9.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_9.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_10\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_10.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_10.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_10.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_10.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_11\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_11.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_11.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_11.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_11.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_12\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_12.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_12.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_12.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_12.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_13\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_13.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_13.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_13.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_13.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_14\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_14.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_14.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_14.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_14.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_15\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_15.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_15.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_15.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_15.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"unkown_16\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_16.Start.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_16.Start.Y + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_16.End.X + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_16.End.Y + "]"
                                     + "}" + ( minimumJson ? "" : Environment.NewLine ));
                */
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "      " ) + "}" + ( ( ( count - 1 ) != i ) ? "," : "" ) + ( minimumJson ? "" : Environment.NewLine ));
            }

            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "]" + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
            return true;
        }

        /// <summary>
        /// 表情のJSONテキストを生成
        /// </summary>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <param name="minimumJson">false:スペースや改行をいれる</param>
        /// <returns>true: エラーなし</returns>
        private bool Convert_StructToJson_Skin(ref string err_message, bool minimumJson = false)
        {
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "\"Skin\": {" + ( minimumJson ? "" : Environment.NewLine ));
            int count = this.VMD_Data.Skin.Count;
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Count\": " + count + "," + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Data\": [" + ( minimumJson ? "" : Environment.NewLine ));

            for (int i = 0; i < count; i++)
            {
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "{" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.Skin.Data[i].FrameNo) + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Name\": " + "\"" + this.VMD_Data.Skin.Data[i].Name + "\"," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Weight\": " + this.VMD_Data.Skin.Data[i].Weight + "" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}" + ( ( ( count - 1 ) != i ) ? "," : "" ) + ( minimumJson ? "" : Environment.NewLine ));
            }

            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "]" + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
            return true;
        }

        /// <summary>
        /// カメラのJSONテキストを生成
        /// </summary>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <param name="minimumJson">false:スペースや改行をいれる</param>
        /// <returns>true: エラーなし</returns>
        private bool Convert_StructToJson_Camera(ref string err_message, bool minimumJson = false)
        {
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "\"Camera\": {" + ( minimumJson ? "" : Environment.NewLine ));
            int count = this.VMD_Data.Camera.Count;
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Count\": " + count + "," + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Data\": [" + ( minimumJson ? "" : Environment.NewLine ));

            for (int i = 0; i < count; i++)
            {
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "{" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.Camera.Data[i].FrameNo) + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Length\": " + this.VMD_Data.Camera.Data[i].Length + "," + ( minimumJson ? "" : Environment.NewLine ));

                switch (this.VMD_Data.Expansion.CoordinateSystem)
                {
                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand:
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Z)
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Rotation\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree(-this.VMD_Data.Camera.Data[i].Rotation.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree(-this.VMD_Data.Camera.Data[i].Rotation.Yaw)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree(this.VMD_Data.Camera.Data[i].Rotation.Roll))
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.RightHand:
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, -this.VMD_Data.Camera.Data[i].Location.Z) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Y)
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Rotation\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree(-this.VMD_Data.Camera.Data[i].Rotation.Roll)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree(this.VMD_Data.Camera.Data[i].Rotation.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree(this.VMD_Data.Camera.Data[i].Rotation.Yaw))
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.MMDHand:
                    default:
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Z)
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Rotation\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree(this.VMD_Data.Camera.Data[i].Rotation.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree(this.VMD_Data.Camera.Data[i].Rotation.Yaw)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree(this.VMD_Data.Camera.Data[i].Rotation.Roll))
                                             + "]," + ( minimumJson ? "" : Environment.NewLine ));
                        break;
                }

                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Interpolation\": {" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"X\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Xaxis.Start.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Xaxis.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Xaxis.End.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Xaxis.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Y\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Yaxis.Start.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Yaxis.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Yaxis.End.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Yaxis.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Z\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Zaxis.Start.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Zaxis.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Zaxis.End.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Zaxis.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Rotation\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Rotation.Start.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Rotation.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Rotation.End.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Rotation.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"Length\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Length.Start.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Length.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Length.End.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Length.End.Y + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "\"ViewingAngle\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.ViewingAngle.Start.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.ViewingAngle.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.ViewingAngle.End.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.ViewingAngle.End.Y + "]"
                                     + "}" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"ViewingAngle\": " + this.VMD_Data.Camera.Data[i].ViewingAngle + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Perspective\": " + ( this.VMD_Data.Camera.Data[i].Perspective ? "true" : "false" ) + "" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}" + ( ( ( count - 1 ) != i ) ? "," : "" ) + ( minimumJson ? "" : Environment.NewLine ));
            }

            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "]" + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
            return true;
        }

        /// <summary>
        /// 照明のJSONテキストを生成
        /// </summary>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <param name="minimumJson">false:スペースや改行をいれる</param>
        /// <returns>true: エラーなし</returns>
        private bool Convert_StructToJson_Illumination(ref string err_message, bool minimumJson = false)
        {
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "\"Illumination\": {" + ( minimumJson ? "" : Environment.NewLine ));
            int count = this.VMD_Data.Illumination.Count;
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Count\": " + count + "," + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Data\": [" + ( minimumJson ? "" : Environment.NewLine ));

            for (int i = 0; i < count; i++)
            {
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "{" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.Illumination.Data[i].FrameNo) + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"RGB\": [" + this.VMD_Data.Illumination.Data[i].RGB.R + ", "
                                     + this.VMD_Data.Illumination.Data[i].RGB.G + ", "
                                     + this.VMD_Data.Illumination.Data[i].RGB.B
                                     + "]," + ( minimumJson ? "" : Environment.NewLine ));

                switch (this.VMD_Data.Expansion.CoordinateSystem)
                {
                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand:
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.Z)
                                             + "]" + ( minimumJson ? "" : Environment.NewLine ));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.RightHand:
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, this.VMD_Data.Illumination.Data[i].Location.Z) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.Y)
                                             + "]" + ( minimumJson ? "" : Environment.NewLine ));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.MMDHand:
                    default:
                        sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, this.VMD_Data.Illumination.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, this.VMD_Data.Illumination.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, this.VMD_Data.Illumination.Data[i].Location.Z)
                                             + "]" + ( minimumJson ? "" : Environment.NewLine ));
                        break;
                }

                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}" + ( ( ( count - 1 ) != i ) ? "," : "" ) + ( minimumJson ? "" : Environment.NewLine ));
            }

            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "]" + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
            return true;
        }

        /// <summary>
        /// セルフシャドウのJSONテキストを生成
        /// </summary>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <param name="minimumJson">false:スペースや改行をいれる</param>
        /// <returns>true: エラーなし</returns>
        private bool Convert_StructToJson_SelfShadow(ref string err_message, bool minimumJson = false)
        {
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "\"SelfShadow\": {" + ( minimumJson ? "" : Environment.NewLine ));
            int count = this.VMD_Data.SelfShadow.Count;
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Count\": " + count + "," + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Data\": [" + ( minimumJson ? "" : Environment.NewLine ));

            for (int i = 0; i < count; i++)
            {
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "{" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.SelfShadow.Data[i].FrameNo) + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Mode\": " + (int) this.VMD_Data.SelfShadow.Data[i].Mode + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Distance_Value\": " + this.VMD_Data.SelfShadow.Data[i].Distance_Value + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Distance\": " + this.VMD_Data.SelfShadow.Data[i].Distance + "" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}" + ( ( ( count - 1 ) != i ) ? "," : "" ) + ( minimumJson ? "" : Environment.NewLine ));
            }

            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "]" + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
            return true;
        }

        /// <summary>
        /// IKのJSONテキストを生成
        /// </summary>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <param name="minimumJson">false:スペースや改行をいれる</param>
        /// <returns>true: エラーなし</returns>
        private bool Convert_StructToJson_IK(ref string err_message, bool minimumJson = false)
        {
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "\"IK\": {" + ( minimumJson ? "" : Environment.NewLine ));
            int ik_visible_count = this.VMD_Data.IK.Count;
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Count\": " + ik_visible_count + "," + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "\"Data\": [" + ( minimumJson ? "" : Environment.NewLine ));

            for (int i = 0; i < ik_visible_count; i++)
            {
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "{" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.IK.Data[i].FrameNo) + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Visible\": " + ( this.VMD_Data.IK.Data[i].Visible ? "true" : "false" ) + "," + ( minimumJson ? "" : Environment.NewLine ));
                int ik_count = this.VMD_Data.IK.Data[i].IKCount;
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Count\": " + ik_count + "," + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "\"Data\": [" + ( minimumJson ? "" : Environment.NewLine ));

                for (int j = 0; j < ik_count; j++)
                {
                    sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "{" + ( minimumJson ? "" : Environment.NewLine ));
                    sb_VMD_Data.Append(( minimumJson ? "" : "            " ) + "\"BoneName\": " + "\"" + this.VMD_Data.IK.Data[i].Data[j].ikBoneName + "\"," + ( minimumJson ? "" : Environment.NewLine ));
                    sb_VMD_Data.Append(( minimumJson ? "" : "            " ) + "\"Enabled\": " + ( this.VMD_Data.IK.Data[i].Data[j].ikEnabled ? "true" : "false" ) + "" + ( minimumJson ? "" : Environment.NewLine ));
                    sb_VMD_Data.Append(( minimumJson ? "" : "          " ) + "}" + ( ( ( ik_count - 1 ) != j ) ? "," : "" ) + ( minimumJson ? "" : Environment.NewLine ));
                }

                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "]" + ( minimumJson ? "" : Environment.NewLine ));
                sb_VMD_Data.Append(( minimumJson ? "" : "        " ) + "}" + ( ( ( ik_visible_count - 1 ) != i ) ? "," : "" ) + ( minimumJson ? "" : Environment.NewLine ));
            }

            sb_VMD_Data.Append(( minimumJson ? "" : "    " ) + "]" + ( minimumJson ? "" : Environment.NewLine ));
            sb_VMD_Data.Append(( minimumJson ? "" : "  " ) + "}," + ( minimumJson ? "" : Environment.NewLine ));
            return true;
        }

        /// <summary>
        /// 拡張ヘッダのJSONテキストを生成
        /// </summary>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <param name="minimumJson">false:スペースや改行をいれる</param>
        /// <returns>true: エラーなし</returns>
        private bool Convert_StructToJson_Expansion(ref string err_message, bool minimumJson = false)
        {
            //this. VMD_Data.  Expansion
            sb_VMD_Data.Append(
                            ( minimumJson ? "" : "  " ) + "\"Expansion\": {" + ( minimumJson ? "" : Environment.NewLine ) +
                            ( minimumJson ? "" : "      " ) + "\"TargetID\": " + this.VMD_Data.Expansion.TargetID + "," + ( minimumJson ? "" : Environment.NewLine ) +
                            ( minimumJson ? "" : "      " ) + "\"StartFrame\": " + this.VMD_Data.Expansion.StartFrame + "," + ( minimumJson ? "" : Environment.NewLine ) +
                            ( minimumJson ? "" : "      " ) + "\"Version\": " + this.VMD_Data.Expansion.Version + "," + ( minimumJson ? "" : Environment.NewLine ) +
                            ( minimumJson ? "" : "      " ) + "\"FileType\": " + "\"" + this.VMD_Data.Expansion.FileType + "\"," + ( minimumJson ? "" : Environment.NewLine ) +
                            ( minimumJson ? "" : "      " ) + "\"CoordinateSystem\": " + "\"" + this.VMD_Data.Expansion.CoordinateSystem.ToString() + "\"" + ( minimumJson ? "" : Environment.NewLine ) +
                            ( minimumJson ? "" : "  " ) + "}" + ( minimumJson ? "" : Environment.NewLine )
            );
            return true;
        }

        #endregion Converter (構造体からJson)

        #region Converter (VMD Fileから構造体)

        /// <summary>
        /// VMD Fileから構造体へ変換する関数
        /// </summary>
        /// <param name="vmd_filepath">VMD ファイルのパス</param>
        /// <param name="err_message">エラー発生時のエラーメッセージ</param>
        /// <returns>true: エラーなし</returns>
        private bool Convert_VMDFileToStruct(string vmd_filepath, ref string err_message)
        {
            bool retReturn = true;

            if (System.IO.File.Exists(vmd_filepath))
            {
                BinaryReader reader = null;

                try
                {
                    CommonLogger.Log(CommonLogger.LEVEL.INFO, "==================");
                    CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Open file --");
                    FileInfo fileInfo = new FileInfo(vmd_filepath);
                    CommonLogger.Log(CommonLogger.LEVEL.INFO, " Load VMD file : " + fileInfo.FullName);
                    reader = new BinaryReader(File.Open(fileInfo.FullName, FileMode.Open));
                    long filelength = reader.BaseStream.Length;
                    CommonLogger.Log(CommonLogger.LEVEL.INFO, " Size : " + filelength);

                    //////////////////////////////
                    // -- Loading [Header] --
                    //////////////////////////////
                    if (filelength > reader.BaseStream.Position)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Loading [Header] --");
                        VMD_Data.SetFileSignature(CommonFunction.GetTextFromByte(
                                                                    reader.ReadBytes(30),
                                                                    text_encoding_sjis,
                                                                    text_encoding_utf8));

                        if (!VMD_Format.FORMAT_Expansion.FILETYPE_VMD.Equals(VMD_Data.Expansion.FileType))
                        {
                            throw new Exception("Is this a VMD file ? ");
                        }

                        VMD_Data.Header.ModelName = CommonFunction.GetTextFromByte(
                                                                    reader.ReadBytes(20),
                                                                    text_encoding_sjis,
                                                                    text_encoding_utf8);
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "    FileSignature : " + VMD_Data.Header.FileSignature.TrimEnd());
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "    ModelName     : " + VMD_Data.Header.ModelName.TrimEnd());
                    }

                    //////////////////////////////
                    // -- Loading [Motion] --
                    //////////////////////////////
                    if (filelength > reader.BaseStream.Position)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Loading [Motion] --");
                        uint motion_count = reader.ReadUInt32();
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "    count : " + motion_count);

                        for (int i = 0; i < motion_count; i++)
                        {
                            // 111 Byte
                            VMD_Format.Motion_Data data = new VMD_Format.Motion_Data();
                            data.Name = CommonFunction.GetTextFromByte(
                                                        reader.ReadBytes(15),
                                                        text_encoding_sjis,
                                                        text_encoding_utf8);
                            data.FrameNo = reader.ReadUInt32();
                            data.Location.Set(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            /////////////////////////////////////////////////
                            data.Quaternion.Y = reader.ReadSingle();
                            data.Quaternion.X = reader.ReadSingle();
                            data.Quaternion.Z = reader.ReadSingle();
                            data.Quaternion.W = reader.ReadSingle();
                            data.Euler = this.QuaternionToEuler(data.Quaternion);

                            // 座標系によりクォータニオンが違うため再計算
                            data.Quaternion_left = CommonFunction.EulerAnglesToQuaternion(
                                -data.Euler.Roll,
                                data.Euler.Pitch,
                                -data.Euler.Yaw
                                );
                            data.Quaternion_right = CommonFunction.EulerAnglesToQuaternion(
                                -data.Euler.Roll,
                                -data.Euler.Pitch,
                                data.Euler.Yaw
                                );

                            /////////////////////////////////////////////////
                            data.Interpolation.Xaxis.Start.X = reader.ReadByte();// OK
                            data.Interpolation.Yaxis.Start.X = reader.ReadByte();// OK
                            data.Interpolation.unkown_15.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_7.End.Y = reader.ReadByte();
                            data.Interpolation.Xaxis.Start.Y = reader.ReadByte();// OK
                            data.Interpolation.Yaxis.Start.Y = reader.ReadByte();// OK
                            data.Interpolation.Zaxis.Start.Y = reader.ReadByte();
                            data.Interpolation.Rotation.Start.Y = reader.ReadByte();
                            data.Interpolation.Xaxis.End.X = reader.ReadByte();// OK
                            data.Interpolation.Yaxis.End.X = reader.ReadByte();// OK
                            data.Interpolation.Zaxis.End.X = reader.ReadByte();// OK
                            data.Interpolation.Rotation.End.X = reader.ReadByte();// OK
                            data.Interpolation.Xaxis.End.Y = reader.ReadByte();// OK
                            data.Interpolation.Yaxis.End.Y = reader.ReadByte();// OK
                            data.Interpolation.Zaxis.End.Y = reader.ReadByte();// OK
                            data.Interpolation.Rotation.End.Y = reader.ReadByte();// OK
                            data.Interpolation.unkown_14.End.Y = reader.ReadByte();
                            data.Interpolation.Zaxis.Start.X = reader.ReadByte();// OK
                            data.Interpolation.unkown_15.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_5.End.Y = reader.ReadByte();
                            data.Interpolation.unkown_14.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_8.End.Y = reader.ReadByte();
                            data.Interpolation.unkown_10.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_13.End.Y = reader.ReadByte();
                            data.Interpolation.unkown_5.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_6.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_6.End.Y = reader.ReadByte();
                            data.Interpolation.unkown_8.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_9.End.X = reader.ReadByte();
                            data.Interpolation.unkown_11.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_11.End.Y = reader.ReadByte();
                            data.Interpolation.unkown_13.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_15.End.X = reader.ReadByte();
                            data.Interpolation.unkown_14.End.X = reader.ReadByte();
                            data.Interpolation.unkown_7.End.X = reader.ReadByte();
                            data.Interpolation.unkown_9.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_10.End.X = reader.ReadByte();
                            data.Interpolation.unkown_12.End.X = reader.ReadByte();
                            data.Interpolation.unkown_5.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_6.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_7.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_8.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_9.End.Y = reader.ReadByte();
                            data.Interpolation.unkown_11.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_12.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_13.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_15.End.Y = reader.ReadByte();
                            data.Interpolation.unkown_16.Start.X = reader.ReadByte();
                            data.Interpolation.Rotation.Start.X = reader.ReadByte();// OK
                            data.Interpolation.unkown_9.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_10.End.Y = reader.ReadByte();
                            data.Interpolation.unkown_14.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_12.End.Y = reader.ReadByte();
                            data.Interpolation.unkown_5.End.X = reader.ReadByte();
                            data.Interpolation.unkown_6.End.X = reader.ReadByte();
                            data.Interpolation.unkown_7.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_8.End.X = reader.ReadByte();
                            data.Interpolation.unkown_10.Start.X = reader.ReadByte();
                            data.Interpolation.unkown_11.End.X = reader.ReadByte();
                            data.Interpolation.unkown_12.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_13.End.X = reader.ReadByte();
                            data.Interpolation.unkown_16.Start.Y = reader.ReadByte();
                            data.Interpolation.unkown_16.End.X = reader.ReadByte();
                            data.Interpolation.unkown_16.End.Y = reader.ReadByte();
                            VMD_Data.Motion.Data.Add(data);
                        }
                    }

                    //////////////////////////////
                    // -- Loading [Skin] --
                    //////////////////////////////
                    if (filelength > reader.BaseStream.Position)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Loading [Skin] --");
                        uint skin_count = reader.ReadUInt32();
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "    count : " + skin_count);

                        for (int i = 0; i < skin_count; i++)
                        {
                            // 23 Byte
                            VMD_Format.Skin_Data data = new VMD_Format.Skin_Data();
                            data.Name = CommonFunction.GetTextFromByte(
                                                        reader.ReadBytes(15),
                                                        text_encoding_sjis,
                                                        text_encoding_utf8);
                            data.FrameNo = reader.ReadUInt32();
                            data.Weight = reader.ReadSingle();
                            VMD_Data.Skin.Data.Add(data);
                        }
                    }

                    //////////////////////////////
                    // -- Loading [Camera] --
                    //////////////////////////////
                    if (filelength > reader.BaseStream.Position)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Loading [Camera] --");
                        uint camera_count = reader.ReadUInt32();
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "    count : " + camera_count);

                        for (int i = 0; i < camera_count; i++)
                        {
                            // 61 Byte
                            VMD_Format.Camera_Data data = new VMD_Format.Camera_Data();
                            data.FrameNo = reader.ReadUInt32();
                            data.Length = reader.ReadSingle();
                            data.Location.Set(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            /////////////////////////////////////////////////
                            data.Rotation.Pitch = -reader.ReadSingle();
                            data.Rotation.Yaw = reader.ReadSingle();
                            data.Rotation.Roll = reader.ReadSingle();
                            /////////////////////////////////////////////////
                            data.Interpolation.Xaxis.Start.X = reader.ReadByte();
                            data.Interpolation.Xaxis.End.X = reader.ReadByte();
                            data.Interpolation.Xaxis.Start.Y = reader.ReadByte();
                            data.Interpolation.Xaxis.End.Y = reader.ReadByte();
                            data.Interpolation.Yaxis.Start.X = reader.ReadByte();
                            data.Interpolation.Yaxis.End.X = reader.ReadByte();
                            data.Interpolation.Yaxis.Start.Y = reader.ReadByte();
                            data.Interpolation.Yaxis.End.Y = reader.ReadByte();
                            data.Interpolation.Zaxis.Start.X = reader.ReadByte();
                            data.Interpolation.Zaxis.End.X = reader.ReadByte();
                            data.Interpolation.Zaxis.Start.Y = reader.ReadByte();
                            data.Interpolation.Zaxis.End.Y = reader.ReadByte();
                            data.Interpolation.Rotation.Start.X = reader.ReadByte();
                            data.Interpolation.Rotation.End.X = reader.ReadByte();
                            data.Interpolation.Rotation.Start.Y = reader.ReadByte();
                            data.Interpolation.Rotation.End.Y = reader.ReadByte();
                            data.Interpolation.Length.Start.X = reader.ReadByte();
                            data.Interpolation.Length.End.X = reader.ReadByte();
                            data.Interpolation.Length.Start.Y = reader.ReadByte();
                            data.Interpolation.Length.End.Y = reader.ReadByte();
                            data.Interpolation.ViewingAngle.Start.X = reader.ReadByte();
                            data.Interpolation.ViewingAngle.End.X = reader.ReadByte();
                            data.Interpolation.ViewingAngle.Start.Y = reader.ReadByte();
                            data.Interpolation.ViewingAngle.End.Y = reader.ReadByte();
                            data.ViewingAngle = reader.ReadUInt32();
                            data.Perspective = !reader.ReadBoolean(); // 0:on 1:off
                            VMD_Data.Camera.Data.Add(data);
                        }
                    }

                    //////////////////////////////
                    // -- Loading [Illumination] --
                    //////////////////////////////
                    if (filelength > reader.BaseStream.Position)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Loading [Illumination] --");
                        uint illumination_count = reader.ReadUInt32();
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "    count : " + illumination_count);

                        for (int i = 0; i < illumination_count; i++)
                        {
                            // 28 Byte
                            VMD_Format.Illumination_Data data = new VMD_Format.Illumination_Data();
                            data.FrameNo = reader.ReadUInt32();

                            // RGB各値/256 // 赤、緑、青 3*4
                            data.RGB = Color.FromArgb((int) ( reader.ReadSingle() * 256.0f ),
                                                        (int) ( reader.ReadSingle() * 256.0f ),
                                                        (int) ( reader.ReadSingle() * 256.0f ));
                            data.Location.Set(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            VMD_Data.Illumination.Data.Add(data);
                        }
                    }

                    //////////////////////////////
                    // -- Loading [SelfShadow] --
                    //////////////////////////////
                    if (filelength > reader.BaseStream.Position)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Loading [SelfShadow] --");
                        uint selfshadow_count = reader.ReadUInt32();
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "    count : " + selfshadow_count);

                        for (int i = 0; i < selfshadow_count; i++)
                        {
                            // 9 Byte
                            VMD_Format.SelfShadow_Data data = new VMD_Format.SelfShadow_Data();
                            data.FrameNo = reader.ReadUInt32(); // 4 // フレーム番号(読込時は現在のフレーム位置を0とした相対位置)
                            data.SetMode(reader.ReadByte()); // 00-02 // モード
                            data.Distance_Value = reader.ReadSingle();
                            VMD_Data.SelfShadow.Data.Add(data);
                        }
                    }

                    //////////////////////////////
                    // -- Loading [IK] --
                    //////////////////////////////
                    if (filelength > reader.BaseStream.Position)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Loading [IK] --");
                        uint ik_count = reader.ReadUInt32();
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "    count : " + ik_count);

                        for (int i = 0; i < ik_count; i++)
                        {
                            // ? Byte
                            VMD_Format.IK_VISIBLE_Data visible_data = new VMD_Format.IK_VISIBLE_Data();
                            visible_data.FrameNo = reader.ReadUInt32();
                            visible_data.Visible = reader.ReadBoolean();
                            uint ik_visible_count = reader.ReadUInt32();
                            CommonLogger.Log(CommonLogger.LEVEL.INFO, "  -- Loading [IK_VISIBLE] --");
                            CommonLogger.Log(CommonLogger.LEVEL.INFO, "      count : " + ik_visible_count);

                            for (int j = 0; j < ik_visible_count; j++)
                            {
                                VMD_Format.IK_Data data = new VMD_Format.IK_Data();
                                data.ikBoneName = CommonFunction.GetTextFromByte(
                                                                  reader.ReadBytes(20),
                                                                  text_encoding_sjis,
                                                                  text_encoding_utf8);
                                data.ikEnabled = reader.ReadBoolean();
                                visible_data.Data.Add(data);
                            }

                            VMD_Data.IK.Data.Add(visible_data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //CommonLogger.Log(CommonLogger.LEVEL.ERROR, ex.Message, false);
                    err_message = ex.Message;
                    retReturn = false;
                }
                finally
                {
                    if (null != reader)
                    {
                        reader.Close();
                    }
                }
            }
            else
            {
                CommonLogger.Log(CommonLogger.LEVEL.WARNING, "'" + vmd_filepath + "'は存在しません。");
                err_message = "'" + vmd_filepath + "'は存在しません。";
                retReturn = false;
            }

            return retReturn;
        }

        #endregion Converter (VMD Fileから構造体)

        #region 変換関数

        /// <summary>
        /// 設定したスタートフレーム数分スライドさせる関数
        /// </summary>
        /// <param name="frameNo">フレーム番号</param>
        /// <returns></returns>
        private string ShiftFrameNo(uint frameNo)
        {
            return ( frameNo + this.VMD_Data.Expansion.StartFrame ).ToString();
        }

        /// <summary>
        /// Quaternion を Euler に変換する関する
        /// </summary>
        /// <param name="value">Quaternion</param>
        /// <returns>Euler</returns>
        /// <remarks>
        ///  - MMDの表示に合うように計算してます。
        ///  - 計算結果の少数がMMDの表示と若干異なっています。
        /// </remarks>
        private AxisOfRotation<float> QuaternionToEuler(Quaternion<float> value)
        {
            AxisOfRotation<float> euler = new AxisOfRotation<float>();
            float flag_signinversion = -1;
            euler.Set(
                flag_signinversion *
                    (float) Math.Atan2(2.0 * ( ( value.X * value.Y ) + ( value.Z * value.W ) ),
                                        Math.Pow(value.X, 2) - Math.Pow(value.Y, 2) - Math.Pow(value.Z, 2) + Math.Pow(value.W, 2)),

                flag_signinversion *
                    (float) Math.Asin(2.0 * ( ( value.X * value.Z ) - ( value.Y * value.W ) )),

                flag_signinversion *
                    (float) ( Math.PI
                          - Math.Atan2(2.0 * ( ( value.Y * value.Z ) + ( value.X * value.W ) ),
                                        Math.Pow(value.X, 2) + Math.Pow(value.Y, 2) - Math.Pow(value.Z, 2) - Math.Pow(value.W, 2)) )
                );
            return euler;
        }

        #endregion 変換関数
    }
}