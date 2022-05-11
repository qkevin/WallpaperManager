using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Common;

namespace WallPaperService
{
    class Program
    {

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            if (args != null && args.Any())
            {
                string WallpaperManagerPath = AppDomain.CurrentDomain.BaseDirectory + "WallpaperManager.exe";
                Process managerPrcess = null;
                if (File.Exists(WallpaperManagerPath))
                {
                    managerPrcess = new Process();
                    managerPrcess.StartInfo.FileName = WallpaperManagerPath;
                    managerPrcess.StartInfo.Arguments = "True";
                    managerPrcess.Start();
                }
                Console.WriteLine("Press Escape to exit");
                while (Console.ReadKey().Key != ConsoleKey.Escape)
                {

                }
                if (managerPrcess != null)
                {
                    managerPrcess.Kill();
                }
            }
            else
            {
                try
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                {
                    new WallpaperService()
                };
                    ServiceBase.Run(ServicesToRun);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
