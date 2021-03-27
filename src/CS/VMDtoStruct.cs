using MaSiRoProject.Common;
using MaSiRoProject.Format;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static MaSiRoProject.Format.VMD_Format;
using static MaSiRoProject.Format.VMD_Format_Struct.FORMAT_Expansion;

namespace MaSiRoProject
{
    /// <summary>
    /// VMD ファイルからJsonへ変換するためのクラス
    /// </summary>
    public class VMDtoStruct
    {
        #region データ

        /// <summary>
        /// 指定されたファイルの変換したVMDフォーマットデータ
        /// </summary>
        public VMD_Format VMD_Data = new VMD_Format();

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
        public void SetStartFrame(int startframe)
        {
            VMD_Data.SetStartFrame(startframe);
        }

        /// <summary>
        /// 単位設定
        /// </summary>
        /// <param name="value">定義</param>
        public void SetUnitOfLength(VMD_UNIT_LENGTH value)
        {
            VMD_Data.SetUnitOfLength(value);
        }

        #endregion 設定関数

        #region Converter (構造体からJson)

        /// <summary>
        /// ファイル変換
        /// </summary>
        /// <param name="vmd_filepath">変換したいVMDファイルのパス</param>
        public bool Convert(string vmd_filepath)
        {
            bool retflag = true;
            string err_message = string.Empty;

            try
            {
                if (true == retflag)
                {
                    retflag = this.Convert_VMDFileToStruct(vmd_filepath, ref err_message);
                }
            }
            catch (Exception ex)
            {
                //CommonLogger.Log(CommonLogger.LEVEL.ERROR, ex.Message, false);
                err_message = ex.Message;
                retflag = false;
            }

            return retflag;
        }

        #endregion Converter (構造体からJson)

        //////////////////////////////////////////////////////////////

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
                    if (0 == filelength)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.WARNING, "'" + vmd_filepath + "'は空のファイルです。");
                        err_message = "'" + vmd_filepath + "'は空のファイルです。";
                        retReturn = false;
                    }
                    else
                    {
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
                                data.SetMotionInterpolation(reader.ReadBytes(64));
                                /*
                                data.Interpolation.Xaxis.Start.Time = reader.ReadByte();// OK
                                data.Interpolation.Yaxis.Start.Time = reader.ReadByte();// OK
                                data.Interpolation.unkown_15.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_7.Stop.Amount = reader.ReadByte();
                                data.Interpolation.Xaxis.Start.Amount = reader.ReadByte();// OK
                                data.Interpolation.Yaxis.Start.Amount = reader.ReadByte();// OK
                                data.Interpolation.Zaxis.Start.Amount = reader.ReadByte();
                                data.Interpolation.Rotation.Start.Amount = reader.ReadByte();
                                data.Interpolation.Xaxis.Stop.Time = reader.ReadByte();// OK
                                data.Interpolation.Yaxis.Stop.Time = reader.ReadByte();// OK
                                data.Interpolation.Zaxis.Stop.Time = reader.ReadByte();// OK
                                data.Interpolation.Rotation.Stop.Time = reader.ReadByte();// OK
                                data.Interpolation.Xaxis.Stop.Amount = reader.ReadByte();// OK
                                data.Interpolation.Yaxis.Stop.Amount = reader.ReadByte();// OK
                                data.Interpolation.Zaxis.Stop.Amount = reader.ReadByte();// OK
                                data.Interpolation.Rotation.Stop.Amount = reader.ReadByte();// OK
                                data.Interpolation.unkown_14.Stop.Amount = reader.ReadByte();
                                data.Interpolation.Zaxis.Start.Time = reader.ReadByte();// OK
                                data.Interpolation.unkown_15.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_5.Stop.Amount = reader.ReadByte();
                                data.Interpolation.unkown_14.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_8.Stop.Amount = reader.ReadByte();
                                data.Interpolation.unkown_10.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_13.Stop.Amount = reader.ReadByte();
                                data.Interpolation.unkown_5.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_6.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_6.Stop.Amount = reader.ReadByte();
                                data.Interpolation.unkown_8.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_9.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_11.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_11.Stop.Amount = reader.ReadByte();
                                data.Interpolation.unkown_13.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_15.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_14.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_7.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_9.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_10.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_12.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_5.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_6.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_7.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_8.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_9.Stop.Amount = reader.ReadByte();
                                data.Interpolation.unkown_11.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_12.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_13.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_15.Stop.Amount = reader.ReadByte();
                                data.Interpolation.unkown_16.Start.Time = reader.ReadByte();
                                data.Interpolation.Rotation.Start.Time = reader.ReadByte();// OK
                                data.Interpolation.unkown_9.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_10.Stop.Amount = reader.ReadByte();
                                data.Interpolation.unkown_14.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_12.Stop.Amount = reader.ReadByte();
                                data.Interpolation.unkown_5.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_6.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_7.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_8.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_10.Start.Time = reader.ReadByte();
                                data.Interpolation.unkown_11.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_12.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_13.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_16.Start.Amount = reader.ReadByte();
                                data.Interpolation.unkown_16.Stop.Time = reader.ReadByte();
                                data.Interpolation.unkown_16.Stop.Amount = reader.ReadByte();
                                */
                                /////////////////////////////////////////////////
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
                                data.Interpolation.Xaxis.Start.Time = reader.ReadByte();
                                data.Interpolation.Xaxis.Stop.Time = reader.ReadByte();
                                data.Interpolation.Xaxis.Start.Amount = reader.ReadByte();
                                data.Interpolation.Xaxis.Stop.Amount = reader.ReadByte();
                                data.Interpolation.Yaxis.Start.Time = reader.ReadByte();
                                data.Interpolation.Yaxis.Stop.Time = reader.ReadByte();
                                data.Interpolation.Yaxis.Start.Amount = reader.ReadByte();
                                data.Interpolation.Yaxis.Stop.Amount = reader.ReadByte();
                                data.Interpolation.Zaxis.Start.Time = reader.ReadByte();
                                data.Interpolation.Zaxis.Stop.Time = reader.ReadByte();
                                data.Interpolation.Zaxis.Start.Amount = reader.ReadByte();
                                data.Interpolation.Zaxis.Stop.Amount = reader.ReadByte();
                                data.Interpolation.Rotation.Start.Time = reader.ReadByte();
                                data.Interpolation.Rotation.Stop.Time = reader.ReadByte();
                                data.Interpolation.Rotation.Start.Amount = reader.ReadByte();
                                data.Interpolation.Rotation.Stop.Amount = reader.ReadByte();
                                data.Interpolation.Length.Start.X = reader.ReadByte();
                                data.Interpolation.Length.Stop.X = reader.ReadByte();
                                data.Interpolation.Length.Start.Y = reader.ReadByte();
                                data.Interpolation.Length.Stop.Y = reader.ReadByte();
                                data.Interpolation.ViewingAngle.Start.Time = reader.ReadByte();
                                data.Interpolation.ViewingAngle.Stop.Time = reader.ReadByte();
                                data.Interpolation.ViewingAngle.Start.Amount = reader.ReadByte();
                                data.Interpolation.ViewingAngle.Stop.Amount = reader.ReadByte();
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
                                data.RGB = Color.FromArgb((int)(reader.ReadSingle() * 256.0f),
                                        (int)(reader.ReadSingle() * 256.0f),
                                        (int)(reader.ReadSingle() * 256.0f));
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
            if (true == retReturn)
            {
                //////////////////////////////
                // -- Analysis [Motion] --
                //////////////////////////////
                retReturn = this.AnalysisMotion();
            }

            return retReturn;
        }

        #endregion Converter (VMD Fileから構造体)

        #region 解析関数

        /// <summary>
        /// Motionのアクセスを簡単にするため解析を行う
        /// </summary>
        /// <returns></returns>
        private bool AnalysisMotion()
        {
            bool result = true;
            try
            {
                CommonLogger.Log(CommonLogger.LEVEL.INFO, "-- Analysis [Motion] --");
                Dictionary<string, int> dic = new Dictionary<string, int>();
                for (int i = 0; i < this.VMD_Data.Motion.Data.Count; i++)
                {
                    if (true == dic.ContainsKey(this.VMD_Data.Motion.Data[i].Name))
                    {
                        // すでに登録済み

                        this.VMD_Data.Motion.Data[dic[this.VMD_Data.Motion.Data[i].Name]].IndexInfo.next =
                            i;

                        this.VMD_Data.Motion.Data[i].IndexInfo.previous =
                            dic[this.VMD_Data.Motion.Data[i].Name];

                        dic[this.VMD_Data.Motion.Data[i].Name] = i;
                    }
                    else
                    {
                        // まだ未登録
                        dic.Add(this.VMD_Data.Motion.Data[i].Name, i);
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #endregion 解析関数

        #region 変換関数

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
                (float)Math.Atan2(2.0 * ((value.X * value.Y) + (value.Z * value.W)),
                    Math.Pow(value.X, 2) - Math.Pow(value.Y, 2) - Math.Pow(value.Z, 2) + Math.Pow(value.W, 2)),
                flag_signinversion *
                (float)Math.Asin(2.0 * ((value.X * value.Z) - (value.Y * value.W))),
                flag_signinversion *
                (float)(Math.PI
                    - Math.Atan2(2.0 * ((value.Y * value.Z) + (value.X * value.W)),
                        Math.Pow(value.X, 2) + Math.Pow(value.Y, 2) - Math.Pow(value.Z, 2) - Math.Pow(value.W, 2)))
            );
            return euler;
        }

        #endregion 変換関数

        //////////////////////////////////////////////////////////////

        #region 文字コード

        /// <summary>
        /// VMD ファイル内の文字コード
        /// </summary>
        /// <remarks>
        ///     設定はコンストラクタで実施
        /// </remarks>
        internal Encoding text_encoding_sjis;

        /// <summary>
        /// JSONの文字コード
        /// </summary>
        public Encoding text_encoding_utf8 = new UTF8Encoding(false);

        #endregion 文字コード

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VMDtoStruct()
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            text_encoding_sjis = Encoding.GetEncoding("Shift_JIS");
            VMD_Data.SetUnitOfLength(VMD_UNIT_LENGTH.VMD_UNIT_LENGTH_DEFAULT);
        }

        #endregion コンストラクタ
    }
}