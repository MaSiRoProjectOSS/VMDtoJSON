using MaSiRoProject.Common;

namespace MaSiRoProject
{
    internal class VMDtoJSON_Converter
    {
        public VMDtoJSON_Converter(string[] args)
        {
            VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList
                coordinate_system = VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand;
            bool flag_LeftHand_agrs = false;
            bool flag_RightHand_agrs = false;
            bool flag_MMDHand_agrs = false;
            bool flag_help = false;
            bool flag_version = false;
            string input_filename = string.Empty;
            string output_filename = string.Empty;

            ////////////////////////
            VMDtoJSON vmdtojson = new VMDtoJSON();
            vmdtojson.SetOutputJsonType(false);

            //vmdtojson.Setting(...));  関数は用意してますが、この関数では使わない

            ////////////////////////
            /// 設定
            ////////////////////////

            for (int i = 0; i < args.Length; i++)
            {
                if ("-F".Equals(args[i]))
                {
                    // 変換対象のVMDファイル
                    if (i + 1 < args.Length)
                    {
                        input_filename = args[i + 1];

                        i = i + 1;
                    }
                }
                else if ("-O".Equals(args[i]))
                {
                    // 出力ファイルパス
                    if (i + 1 < args.Length)
                    {
                        output_filename = args[i + 1];

                        i = i + 1;
                    }
                }
                else if ("-S".Equals(args[i]))
                {
                    // スタートフレーム番号
                    if (i + 1 < args.Length)
                    {
                        int startFrameNo = 0;
                        int.TryParse(args[i + 1], out startFrameNo);
                        vmdtojson.SetStartFrame(startFrameNo);

                        i = i + 1;
                    }
                }
                else if ("-T".Equals(args[i]))
                {
                    // ターゲットID
                    if (i + 1 < args.Length)
                    {
                        int startFrameNo = 0;
                        int.TryParse(args[i + 1], out startFrameNo);
                        vmdtojson.SetTargetID(startFrameNo);

                        i = i + 1;
                    }
                }
                else if ("-M".Equals(args[i]))
                {
                    // minimum json
                    vmdtojson.SetOutputJsonType(true);
                }
                else if ("--lefthand".Equals(args[i].ToLower()))
                {
                    // 座標を左手系に変更する。(デフォルト)
                    flag_LeftHand_agrs = true;
                }
                else if ("--righthand".Equals(args[i].ToLower()))
                {
                    //  座標を右手系に変更する。 [--LeftHand] が指定されていると無効になります。
                    flag_RightHand_agrs = true;
                }
                else if ("--mmdhand".Equals(args[i].ToLower()))
                {
                    //  座標を右手系に変更する。 [--LeftHand] が指定されていると無効になります。
                    flag_MMDHand_agrs = true;
                }
                else if (( "-v".Equals(args[i].ToLower()) || ( "--version".Equals(args[i].ToLower()) ) ))
                {
                    // バージョン
                    flag_version = true;
                }
                else if (( "-q".Equals(args[i]) ))
                {
                    // ログを出力させない
                    CommonLogger.OutputBorderLevel = CommonLogger.LEVEL.REPORT;
                }
                else if (( "-h".Equals(args[i].ToLower()) || ( "--help".Equals(args[i].ToLower()) ) ))
                {
                    // ヘルプ
                    flag_version = true;
                    flag_help = true;
                    break;
                }
            }
            ////////////////////////
            /// 入力設定の変更
            ////////////////////////
            if (true == flag_LeftHand_agrs)
            {
                coordinate_system = VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand;
            }
            else if (true == flag_RightHand_agrs)
            {
                coordinate_system = VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.RightHand;
            }
            else if (true == flag_MMDHand_agrs)
            {
                coordinate_system = VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.MMDHand;
            }
            else
            {
                coordinate_system = VMD_Format_Struct.FORMAT_Expansion.CoordinateSystemList.LeftHand;
            }
            vmdtojson.SetCoordinateSystem(coordinate_system);

            ////////////////////////
            /// 変換
            ////////////////////////
            if (false == flag_help)
            {
                if (true == flag_version)
                {
                    CommonLogger.Log(CommonLogger.LEVEL.INFO, "==================");
                    CommonLogger.Log(CommonLogger.LEVEL.INFO, CommonFunction.ProductName() + " Ver." + CommonFunction.ProductVersion());
                }
                if (0 != input_filename.Length)
                {
                    vmdtojson.Convert(input_filename);
                    if (0 != output_filename.Length)
                    {
                        vmdtojson.OutputFile(output_filename);
                    }
                    else
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, "==================");
                        CommonLogger.Log(Common.CommonLogger.LEVEL.REPORT, vmdtojson.GetJsonTest());
                    }
                }
                else
                {
                    if (true != flag_version)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.ERROR, "NOT found VMD FILE.");
                    }
                }
            }
            else
            {
                CommonLogger.Log(CommonLogger.LEVEL.REPORT,
                                 "==================" + System.Environment.NewLine
                               + CommonFunction.ProductName()
                                + " Ver." + CommonFunction.ProductVersion() + System.Environment.NewLine
                               + "==================");

                // usage
                CommonLogger.Log(Common.CommonLogger.LEVEL.REPORT,
                    " usage: VMDtoJSON"
                        + " [-v | --version] [-h | --help]"
                        + " [-F <Input VMD file path>]"
                        + " [-O <Output JSON file path>]"
                        + " [-S <FrameNo>]"
                        + " [-T <TargetID>]"
                        + " [-M]"
                        + " [-q]" + System.Environment.NewLine
                );

                // Get application information
                CommonLogger.Log(Common.CommonLogger.LEVEL.REPORT,
                          " Options and arguments" + System.Environment.NewLine
                        + "   Get application information" + System.Environment.NewLine
                        + "     -v             : " + "print this software version number (also --version)" + System.Environment.NewLine
                        + "     -h             : " + "print this help message and exit (also --help)" + System.Environment.NewLine
                );

                // Manipulating output data
                CommonLogger.Log(Common.CommonLogger.LEVEL.REPORT,
                          "   Manipulating output data" + System.Environment.NewLine
                        + "     -F input_file  : " + "convert this file. <input_file>" + System.Environment.NewLine
                        + "     -O output_file : " + "output JSON file to this path. <output_file>" + System.Environment.NewLine
                        + "     -S frame_no    : " + "start the frame number<frame_no> from the specified number." + System.Environment.NewLine
                        + "     -T targetID    : " + "set the <targetID> in the extension header." + System.Environment.NewLine
                        + "     -M             : " + "make a JSON file with no line breaks or spaces." + System.Environment.NewLine
                        + "     -q             : " + "don't print version and copyright messages on interactive startup." + System.Environment.NewLine
                );

                // Manipulating output data
                CommonLogger.Log(Common.CommonLogger.LEVEL.REPORT,
                          "   CoordinateSystem(" + System.Environment.NewLine
                        + "     --LeftHand     : " + "Output Left hand Coordinate System. Priorty Hight (defalut) " + System.Environment.NewLine
                        + "     --RightHand    : " + "Output Left hand Coordinate System. Priorty Middle" + System.Environment.NewLine
                        + "     --MMDtHand     : " + "Output Left hand Coordinate System. Priorty Low" + System.Environment.NewLine
                );
            }
        }
    }
}