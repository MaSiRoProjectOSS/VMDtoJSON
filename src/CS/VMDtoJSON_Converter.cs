using MaSiRoProject.Common;

namespace MaSiRoProject
{
    internal class VMDtoJSON_Converter
    {
        public VMDtoJSON_Converter(string[] args)
        {
            bool flag_help = false;
            bool flag_version = false;
            string input_filename = string.Empty;
            string output_filename = string.Empty;

            ////////////////////////
            VMDtoJSON vmdtojson = new VMDtoJSON();
            vmdtojson.SetOutputJsonType(false);

            //vmdtojson.Setting(10, true, 30);  関数は用意してますが、この関数では使わない

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
                        vmdtojson.SetStartFram(startFrameNo);

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
                else if (( "--version".Equals(args[i]) ))
                {
                    // バージョン
                    flag_version = true;
                }
                else if (( "-q".Equals(args[i]) ))
                {
                    // ログを出力させない
                    CommonLogger.OutputBorderLevel = CommonLogger.LEVEL.REPORT;
                }
                else if (( "-h".Equals(args[i]) || ( "--help".Equals(args[i]) ) ))
                {
                    // ヘルプ
                    flag_version = true;
                    flag_help = true;
                    break;
                }
            }

            ////////////////////////
            /// 変換
            ////////////////////////
            if (true == flag_version)
            {
                CommonLogger.Log(CommonLogger.LEVEL.INFO, System.Windows.Forms.Application.ProductName
                    + " Ver." + System.Windows.Forms.Application.ProductVersion
                    + System.Environment.NewLine
                    );
            }
            if (false == flag_help)
            {
                if (!string.Empty.Equals(input_filename))
                {
                    CommonLogger.Log(CommonLogger.LEVEL.INFO, "==================");
                    if (true != flag_version)
                    {
                        CommonLogger.Log(CommonLogger.LEVEL.INFO, System.Windows.Forms.Application.ProductName
                            + " Ver." + System.Windows.Forms.Application.ProductVersion
                            + System.Environment.NewLine
                            );
                    }
                    vmdtojson.Convert(input_filename);
                    if (!string.Empty.Equals(output_filename))
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
                CommonLogger.Log(Common.CommonLogger.LEVEL.INFO, "usage: VMDtoJSON"
                + " [--version] [-h |--help]"
                + " [-F <Input VMD file path>]"
                + " [-O <Output JSON file path>]"
                + " [-S <FrameNo>]"
                + " [-T <TargetID>]"
                + " [-M]"
                + " [-q]"

                );
            }
        }
    }
}