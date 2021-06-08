using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TieredCreator
{
    class Program
    {
        static IConfiguration Config { get; set; }
        static bool IsPartition { get; set; }
        static int TierCount { get; set; }
        static string CurrentTiered { get; set; }
        static string CurrentTieredName { get; set; }
        static void Main(string[] args)
        {
            Config = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .Build();
            TierCount = Convert.ToInt32(Config["TierCount"]);

            for (int i = 1; i <= TierCount; i++)
            {
                string[] sa = Config["Tiered" + i + "Name"].Split(',');
                if (i == 1)
                {
                    Console.Write("即將");
                }
                else
                {
                    Console.Write("及");
                }
                Console.WriteLine("產生 " + sa.Length + " 個 " + Config["Tiered" + i]);

            }

            Console.WriteLine("按任意鍵繼續");
            Console.Read();

            if (!Directory.Exists("./output"))
            {
                Directory.CreateDirectory("./output");
            }

            for (int i = 1; i <= TierCount; i++)
            {
                string[] sa = Config["Tiered" + i + "Name"].Split(',');
                CurrentTiered = Config["Tiered" + i];

                IsPartition = Convert.ToBoolean(Config["IsPartition"]);
                foreach (var tieredName in sa)
                {
                    if (IsPartition)
                    {
                        string csFileMain = Config["Schema"];

                        csFileMain = csFileMain.Replace("{InheritSchema}", Config["InheritSchema"]);

                        csFileMain = csFileMain.Replace("{CustomMethod}", Config["CustomMethod"]);

                        if (Convert.ToBoolean(Config["IsUsingInterfaceNamespace"]))
                        {
                            string usingNamespace = Config["UsingNameSpace"] + "\r\nusing " + Config["ITieredNamespace"] + "." + tieredName + ";";

                            csFileMain = csFileMain.Replace("{usinNamespace}", usingNamespace);
                        }
                        else
                        {
                            csFileMain = csFileMain.Replace("{usinNamespace}", Config["UsingNameSpace"]);
                        }
                        //TODO:待補上 TieredProject參數判斷

                        csFileMain = csFileMain.Replace("{TieredNamespace}", Config["TieredNamespace"]);

                        csFileMain = csFileMain.Replace("{TieredName}", tieredName);

                        csFileMain = csFileMain.Replace("{Tiered}", CurrentTiered);

                        if (!Directory.Exists("./output/" + CurrentTiered + "/" + tieredName))
                        {
                            Directory.CreateDirectory("./output/" + CurrentTiered + "/" + tieredName);
                        }

                        File.WriteAllText("./output/" + CurrentTiered + "/" + tieredName + "/" + tieredName + CurrentTiered + ".cs", csFileMain);

                        //Interface生成
                        if (Convert.ToBoolean(Config["CreateInterface"]))
                        {
                            string interfaceFileMain = Config["ISchema"];

                            interfaceFileMain = interfaceFileMain.Replace("{ICustomMethod}", Config["ICustomMethod"]);

                            interfaceFileMain = interfaceFileMain.Replace("{IInheritSchema}", Config["IInheritSchema"]);

                            interfaceFileMain = interfaceFileMain.Replace("{InterfaceUsingNameSpace}", Config["InterfaceUsingNameSpace"]);

                            //TODO:待補上 TieredProject參數判斷
                            interfaceFileMain = interfaceFileMain.Replace("{ITieredNamespace}", Config["ITieredNamespace"]);

                            interfaceFileMain = interfaceFileMain.Replace("{TieredName}", tieredName);

                            interfaceFileMain = interfaceFileMain.Replace("{Tiered}", CurrentTiered);


                            if (!Directory.Exists("./output/" + CurrentTiered + "/Interface/" + tieredName))
                            {
                                Directory.CreateDirectory("./output/" + CurrentTiered + "/Interface/" + tieredName);
                            }

                            File.WriteAllText("./output/" + CurrentTiered + "/Interface/" + tieredName + "/I" + tieredName + CurrentTiered + ".cs", interfaceFileMain);
                        }
                    }
                    else
                    {

                    }
                }

            }

            Console.WriteLine("按任意鍵 結束程式");
            Console.Read();
        }
    }
}