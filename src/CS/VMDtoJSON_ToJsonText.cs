using MaSiRoProject.Common;
using MaSiRoProject.Format;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MaSiRoProject
{
    /// <summary>
    /// VMD ファイルからJsonへ変換するためのクラス
    /// </summary>
    internal class VMDtoJSON_ToJsonText : VMDtoStruct
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

        #region データ

        /// <summary>
        /// 文字化したVMDフォーマットのデータ
        /// </summary>
        private StringBuilder sb_VMD_Data = new StringBuilder();

        /// <summary>
        /// JSONの出力タイプ
        /// </summary>
        private bool inner_minimumJson = false;

        /// <summary>
        /// 出力グループの設定
        /// </summary>
        private bool group_by_name = false;

        /// <summary>
        /// ファイル出力中のロックオブジェクト
        /// </summary>
        private readonly object lock_outputfile = new object();

        #endregion データ

        #region 設定関数

        /// <summary>
        /// 出力グループの変更する
        /// </summary>
        /// <param name="value">trueならばパーツ名でグルーピングする</param>
        public void GroupByName(bool value)
        {
            this.group_by_name = value;
        }

        /// <summary>
        /// 出力するJSONのタイプを指定する関数
        /// </summary>
        /// <param name="startframe"></param>
        public void SetOutputJsonType(bool minimumJson)
        {
            inner_minimumJson = minimumJson;
        }

        #endregion 設定関数

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
            this.SetStartFrame(startframe);
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
                    if (0 != output_filepath.Length)
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
            string err_message = string.Empty;

            if (true == retflag)
            {
                retflag = this.Convert(vmd_filepath);
            }

            if (true == retflag)
            {
                retflag = this.Convert_StructToJson(inner_minimumJson, ref err_message);
            }

            if (true == retflag)
            {
                retflag = this.OutputFile(output_filepath);
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
                    sb_VMD_Data.Append("{" + (minimumJson ? "" : Environment.NewLine));

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
                            (minimumJson ? "" : "  ") + "\"Header\": {" + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "      ") + "\"FileSignature\": " + "\"" + this.VMD_Data.Header.FileSignature + "\"," + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "      ") + "\"ModelName\": " + "\"" + this.VMD_Data.Header.ModelName + "\"" + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "  ") + "}," + (minimumJson ? "" : Environment.NewLine)
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
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "\"Motion\": {" + (minimumJson ? "" : Environment.NewLine));
            int count = this.VMD_Data.Motion.Count;
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Count\": " + count + "," + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Data\": " + (group_by_name ? "{" : "[") + (minimumJson ? "" : Environment.NewLine));

            Dictionary<string, string> dic = new Dictionary<string, string>();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                sb.Clear();
                sb.Append((minimumJson ? "" : "      ") + "{" + (minimumJson ? "" : Environment.NewLine));
                sb.Append((minimumJson ? "" : "        ") + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.Motion.Data[i].FrameNo) + "," + (minimumJson ? "" : Environment.NewLine));
                sb.Append((minimumJson ? "" : "        ") + "\"Name\": " + "\"" + this.VMD_Data.Motion.Data[i].Name + "\"," + (minimumJson ? "" : Environment.NewLine));

                switch (this.VMD_Data.Expansion.CoordinateSystem)
                {
                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand:
                        sb.Append((minimumJson ? "" : "        ") + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Z)
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "        ") + "\"Rotation\": {" + (minimumJson ? "" : Environment.NewLine));

                        sb.Append((minimumJson ? "" : "          ") + "\"Quaternion\": ["
                                             + this.VMD_Data.Motion.Data[i].Quaternion_left.X + ", "
                                             + (this.VMD_Data.Motion.Data[i].Quaternion_left.Y) + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion_left.Z + ", "
                                             + (this.VMD_Data.Motion.Data[i].Quaternion_left.W)
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "          ") + "\"Euler\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Motion.Data[i].Euler.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree<float>(-this.VMD_Data.Motion.Data[i].Euler.Yaw)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree<float>(-this.VMD_Data.Motion.Data[i].Euler.Roll))
                                             + "]" + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "        ") + "}," + (minimumJson ? "" : Environment.NewLine));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.RightHand:
                        sb.Append((minimumJson ? "" : "        ") + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, -this.VMD_Data.Motion.Data[i].Location.Z) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Y)
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "        ") + "\"Rotation\": {" + (minimumJson ? "" : Environment.NewLine));

                        sb.Append((minimumJson ? "" : "          ") + "\"Quaternion\": ["
                                             + (this.VMD_Data.Motion.Data[i].Quaternion_right.X) + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion_right.Y + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion_right.Z + ", "
                                             + (this.VMD_Data.Motion.Data[i].Quaternion_right.W)
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "          ") + "\"Euler\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree<float>(-this.VMD_Data.Motion.Data[i].Euler.Roll)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree<float>(-this.VMD_Data.Motion.Data[i].Euler.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Motion.Data[i].Euler.Yaw))
                                             + "]" + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "        ") + "}," + (minimumJson ? "" : Environment.NewLine));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.MMDHand:
                    default:
                        sb.Append((minimumJson ? "" : "        ") + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Motion.Data[i].Location.Z)
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "        ") + "\"Rotation\": {" + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "          ") + "\"Quaternion\": ["
                                             + this.VMD_Data.Motion.Data[i].Quaternion.X + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion.Y + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion.Z + ", "
                                             + this.VMD_Data.Motion.Data[i].Quaternion.W
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "          ") + "\"Euler\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Motion.Data[i].Euler.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Motion.Data[i].Euler.Yaw)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION_MOTION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Motion.Data[i].Euler.Roll))
                                             + "]" + (minimumJson ? "" : Environment.NewLine));
                        sb.Append((minimumJson ? "" : "        ") + "}," + (minimumJson ? "" : Environment.NewLine));
                        break;
                }
                sb.Append((minimumJson ? "" : "        ") + "\"Interpolation\": {" + (minimumJson ? "" : Environment.NewLine));
                sb.Append((minimumJson ? "" : "          ") + "\"X\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Xaxis.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Xaxis.Start.Amount + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Xaxis.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Xaxis.Stop.Amount + "]"
                                     + "}," + (minimumJson ? "" : Environment.NewLine));
                sb.Append((minimumJson ? "" : "          ") + "\"Y\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Yaxis.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Yaxis.Start.Amount + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Yaxis.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Yaxis.Stop.Amount + "]"
                                     + "}," + (minimumJson ? "" : Environment.NewLine));
                sb.Append((minimumJson ? "" : "          ") + "\"Z\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Zaxis.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Zaxis.Start.Amount + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Zaxis.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Zaxis.Stop.Amount + "]"
                                     + "}," + (minimumJson ? "" : Environment.NewLine));
                sb.Append((minimumJson ? "" : "          ") + "\"Rotation\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Rotation.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Rotation.Start.Amount + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.Rotation.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.Rotation.Stop.Amount + "]"
                                     + "}" + (minimumJson ? "" : Environment.NewLine));
                /*

                // 同じ値なので不要なので未出力
                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_5\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_5.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_5.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_5.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_5.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_6\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_6.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_6.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_6.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_6.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_7\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_7.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_7.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_7.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_7.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_8\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_8.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_8.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_8.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_8.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_9\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_9.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_9.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_9.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_9.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_10\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_10.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_10.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_10.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_10.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_11\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_11.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_11.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_11.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_11.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_12\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_12.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_12.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_12.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_12.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_13\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_13.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_13.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_13.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_13.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_14\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_14.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_14.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_14.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_14.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_15\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_15.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_15.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_15.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_15.Stop.Amount + "]"
                                     + "}," + ( minimumJson ? "" : Environment.NewLine ));

                sb.Append(( minimumJson ? "" : "          " ) + "\"unkown_16\": {" +
                                    "\"start\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_16.Start.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_16.Start.Amount + "], " +
                                    "\"end\":[" +
                                     +this.VMD_Data.Motion.Data[i].Interpolation.unkown_16.Stop.Time + ", "
                                     + this.VMD_Data.Motion.Data[i].Interpolation.unkown_16.Stop.Amount + "]"
                                     + "}" + ( minimumJson ? "" : Environment.NewLine ));
                */
                sb.Append((minimumJson ? "" : "        ") + "}" + (minimumJson ? "" : Environment.NewLine));
                sb.Append((minimumJson ? "" : "      ") + "}");
                if (true == group_by_name)
                {
                    if (true == dic.ContainsKey(this.VMD_Data.Motion.Data[i].Name))
                    {
                        dic[this.VMD_Data.Motion.Data[i].Name] = dic[this.VMD_Data.Motion.Data[i].Name] + "," + (minimumJson ? "" : Environment.NewLine) + sb.ToString();
                    }
                    else
                    {
                        dic.Add(this.VMD_Data.Motion.Data[i].Name, sb.ToString());
                    }
                }
                else
                {
                    sb.Append((((count - 1) != i) ? "," : "") + (minimumJson ? "" : Environment.NewLine));
                    sb_VMD_Data.Append(sb.ToString());
                }
            }
            if (true == group_by_name)
            {
                bool flag_comma = false;
                foreach (string key in dic.Keys)
                {
                    sb_VMD_Data.Append(
                          (flag_comma ? "," + (minimumJson ? "" : Environment.NewLine) : "")
                        + (minimumJson ? "" : "    ") + "\"" + key + "\": [" + (minimumJson ? "" : Environment.NewLine)
                        + dic[key]
                        + (minimumJson ? "]" : Environment.NewLine + "    " + "]")
                        );
                    flag_comma = true;
                }
                if (false == minimumJson)
                {
                    sb_VMD_Data.Append(Environment.NewLine);
                }
            }

            sb_VMD_Data.Append(
                (minimumJson ? "" : "    ") + (group_by_name ? "}" : "]") + (minimumJson ? "" : Environment.NewLine)
                + (minimumJson ? "" : "  ") + "}," + (minimumJson ? "" : Environment.NewLine));

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
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "\"Skin\": {" + (minimumJson ? "" : Environment.NewLine));
            int count = this.VMD_Data.Skin.Count;
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Count\": " + count + "," + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Data\": [" + (minimumJson ? "" : Environment.NewLine));

            for (int i = 0; i < count; i++)
            {
                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "{" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.Skin.Data[i].FrameNo) + "," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Name\": " + "\"" + this.VMD_Data.Skin.Data[i].Name + "\"," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Weight\": " + this.VMD_Data.Skin.Data[i].Weight + "" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "}" + (((count - 1) != i) ? "," : "") + (minimumJson ? "" : Environment.NewLine));
            }

            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "]" + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "}," + (minimumJson ? "" : Environment.NewLine));
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
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "\"Camera\": {" + (minimumJson ? "" : Environment.NewLine));
            int count = this.VMD_Data.Camera.Count;
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Count\": " + count + "," + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Data\": [" + (minimumJson ? "" : Environment.NewLine));

            for (int i = 0; i < count; i++)
            {
                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "{" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.Camera.Data[i].FrameNo) + "," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Length\": " + this.VMD_Data.Camera.Data[i].Length + "," + (minimumJson ? "" : Environment.NewLine));

                switch (this.VMD_Data.Expansion.CoordinateSystem)
                {
                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand:
                        sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Z)
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Rotation\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree<float>(-this.VMD_Data.Camera.Data[i].Rotation.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree<float>(-this.VMD_Data.Camera.Data[i].Rotation.Yaw)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree<float>(-this.VMD_Data.Camera.Data[i].Rotation.Roll))
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.RightHand:
                        sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, -this.VMD_Data.Camera.Data[i].Location.Z) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Y)
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Rotation\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Camera.Data[i].Rotation.Roll)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Camera.Data[i].Rotation.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Camera.Data[i].Rotation.Yaw))
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.MMDHand:
                    default:
                        sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_POSITION, this.VMD_Data.Camera.Data[i].Location.Z)
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Rotation\": ["
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Camera.Data[i].Rotation.Pitch)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Camera.Data[i].Rotation.Yaw)) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ROTATION, CommonFunction.RadianToDegree<float>(this.VMD_Data.Camera.Data[i].Rotation.Roll))
                                             + "]," + (minimumJson ? "" : Environment.NewLine));
                        break;
                }

                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Interpolation\": {" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "          ") + "\"X\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Xaxis.Start.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Xaxis.Start.Amount + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Xaxis.Stop.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Xaxis.Stop.Amount + "]"
                                     + "}," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "          ") + "\"Y\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Yaxis.Start.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Yaxis.Start.Amount + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Yaxis.Stop.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Yaxis.Stop.Amount + "]"
                                     + "}," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "          ") + "\"Z\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Zaxis.Start.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Zaxis.Start.Amount + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Zaxis.Stop.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Zaxis.Stop.Amount + "]"
                                     + "}," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "          ") + "\"Rotation\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Rotation.Start.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Rotation.Start.Amount + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Rotation.Stop.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Rotation.Stop.Amount + "]"
                                     + "}," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "          ") + "\"Length\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Length.Start.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Length.Start.Y + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.Length.Stop.X + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.Length.Stop.Y + "]"
                                     + "}," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "          ") + "\"ViewingAngle\": {" +
                                     "\"start\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.ViewingAngle.Start.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.ViewingAngle.Start.Amount + "], " +
                                     "\"end\":[" +
                                     +this.VMD_Data.Camera.Data[i].Interpolation.ViewingAngle.Stop.Time + ", "
                                     + this.VMD_Data.Camera.Data[i].Interpolation.ViewingAngle.Stop.Amount + "]"
                                     + "}" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "}," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"ViewingAngle\": " + this.VMD_Data.Camera.Data[i].ViewingAngle + "," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Perspective\": " + (this.VMD_Data.Camera.Data[i].Perspective ? "true" : "false") + "" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "}" + (((count - 1) != i) ? "," : "") + (minimumJson ? "" : Environment.NewLine));
            }

            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "]" + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "}," + (minimumJson ? "" : Environment.NewLine));
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
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "\"Illumination\": {" + (minimumJson ? "" : Environment.NewLine));
            int count = this.VMD_Data.Illumination.Count;
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Count\": " + count + "," + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Data\": [" + (minimumJson ? "" : Environment.NewLine));

            for (int i = 0; i < count; i++)
            {
                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "{" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.Illumination.Data[i].FrameNo) + "," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"RGB\": [" + this.VMD_Data.Illumination.Data[i].RGB.R + ", "
                                     + this.VMD_Data.Illumination.Data[i].RGB.G + ", "
                                     + this.VMD_Data.Illumination.Data[i].RGB.B
                                     + "]," + (minimumJson ? "" : Environment.NewLine));

                switch (this.VMD_Data.Expansion.CoordinateSystem)
                {
                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand:
                        sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.Z)
                                             + "]" + (minimumJson ? "" : Environment.NewLine));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.RightHand:
                        sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, this.VMD_Data.Illumination.Data[i].Location.Z) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, -this.VMD_Data.Illumination.Data[i].Location.Y)
                                             + "]" + (minimumJson ? "" : Environment.NewLine));
                        break;

                    case VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.MMDHand:
                    default:
                        sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Location\": ["
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, this.VMD_Data.Illumination.Data[i].Location.X) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, this.VMD_Data.Illumination.Data[i].Location.Y) + ", "
                                             + CommonFunction.GetRound(DECIMALS_ILLUMINATION, this.VMD_Data.Illumination.Data[i].Location.Z)
                                             + "]" + (minimumJson ? "" : Environment.NewLine));
                        break;
                }

                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "}" + (((count - 1) != i) ? "," : "") + (minimumJson ? "" : Environment.NewLine));
            }

            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "]" + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "}," + (minimumJson ? "" : Environment.NewLine));
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
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "\"SelfShadow\": {" + (minimumJson ? "" : Environment.NewLine));
            int count = this.VMD_Data.SelfShadow.Count;
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Count\": " + count + "," + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Data\": [" + (minimumJson ? "" : Environment.NewLine));

            for (int i = 0; i < count; i++)
            {
                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "{" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.SelfShadow.Data[i].FrameNo) + "," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Mode\": " + (int)this.VMD_Data.SelfShadow.Data[i].Mode + "," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Distance_Value\": " + this.VMD_Data.SelfShadow.Data[i].Distance_Value + "," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Distance\": " + this.VMD_Data.SelfShadow.Data[i].Distance + "" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "}" + (((count - 1) != i) ? "," : "") + (minimumJson ? "" : Environment.NewLine));
            }

            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "]" + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "}," + (minimumJson ? "" : Environment.NewLine));
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
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "\"IK\": {" + (minimumJson ? "" : Environment.NewLine));
            int ik_visible_count = this.VMD_Data.IK.Count;
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Count\": " + ik_visible_count + "," + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "\"Data\": [" + (minimumJson ? "" : Environment.NewLine));

            for (int i = 0; i < ik_visible_count; i++)
            {
                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "{" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"FrameNo\": " + this.ShiftFrameNo(this.VMD_Data.IK.Data[i].FrameNo) + "," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Visible\": " + (this.VMD_Data.IK.Data[i].Visible ? "true" : "false") + "," + (minimumJson ? "" : Environment.NewLine));
                int ik_count = this.VMD_Data.IK.Data[i].IKCount;
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Count\": " + ik_count + "," + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "\"Data\": [" + (minimumJson ? "" : Environment.NewLine));

                for (int j = 0; j < ik_count; j++)
                {
                    sb_VMD_Data.Append((minimumJson ? "" : "          ") + "{" + (minimumJson ? "" : Environment.NewLine));
                    sb_VMD_Data.Append((minimumJson ? "" : "            ") + "\"BoneName\": " + "\"" + this.VMD_Data.IK.Data[i].Data[j].ikBoneName + "\"," + (minimumJson ? "" : Environment.NewLine));
                    sb_VMD_Data.Append((minimumJson ? "" : "            ") + "\"Enabled\": " + (this.VMD_Data.IK.Data[i].Data[j].ikEnabled ? "true" : "false") + "" + (minimumJson ? "" : Environment.NewLine));
                    sb_VMD_Data.Append((minimumJson ? "" : "          ") + "}" + (((ik_count - 1) != j) ? "," : "") + (minimumJson ? "" : Environment.NewLine));
                }

                sb_VMD_Data.Append((minimumJson ? "" : "        ") + "]" + (minimumJson ? "" : Environment.NewLine));
                sb_VMD_Data.Append((minimumJson ? "" : "      ") + "}" + (((ik_visible_count - 1) != i) ? "," : "") + (minimumJson ? "" : Environment.NewLine));
            }

            sb_VMD_Data.Append((minimumJson ? "" : "    ") + "]" + (minimumJson ? "" : Environment.NewLine));
            sb_VMD_Data.Append((minimumJson ? "" : "  ") + "}," + (minimumJson ? "" : Environment.NewLine));
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
                            (minimumJson ? "" : "  ") + "\"Expansion\": {" + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "      ") + "\"TargetID\": " + this.VMD_Data.Expansion.TargetID + "," + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "      ") + "\"StartFrame\": " + this.VMD_Data.Expansion.StartFrame + "," + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "      ") + "\"Version\": " + this.VMD_Data.Expansion.Version + "," + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "      ") + "\"FileType\": " + "\"" + this.VMD_Data.Expansion.FileType + "\"," + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "      ") + "\"CoordinateSystem\": " + "\"" + this.VMD_Data.Expansion.CoordinateSystem.ToString() + "\"," + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "      ") + "\"GroupType\": " + "\"" + (group_by_name ? "NAME" : "NONE") + "\"" + (minimumJson ? "" : Environment.NewLine) +
                            (minimumJson ? "" : "  ") + "}" + (minimumJson ? "" : Environment.NewLine)
            );
            return true;
        }

        #endregion Converter (構造体からJson)
    }
}