using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using WallpaperManager.Common;
using WallpaperService;

namespace WallPaperService
{
   public partial class WallpaperService : ServiceBase
   {
       private string WallpaperManagerPath = AppDomain.CurrentDomain.BaseDirectory + "WallpaperManager.exe" + " True";
       log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       System.Timers.Timer aTimer;
       AppLoader.PROCESS_INFORMATION procInfo;
        public WallpaperService()
        {
            InitializeComponent(); 
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(ListenDeskTop);
            aTimer.Interval = 5000;
        }


        private void ListenDeskTop(object source, ElapsedEventArgs e)
        {
            bool isWallpaperManagerStarted = false;//标识记进程是否启动
            Process[] processes = Process.GetProcesses();//获取所有进程信息
            for (int i = 0; i < processes.Length; i++)
            {
                if (processes[i].ProcessName.ToLower() == "wallpapermanager")
                {
                    isWallpaperManagerStarted = true;
                    return;
                }
            }
            if (!isWallpaperManagerStarted)
            {
                AppLoader.StartProcessAndBypassUAC(WallpaperManagerPath, System.IO.Path.GetDirectoryName(WallpaperManagerPath), out procInfo);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                aTimer.Enabled = true;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        protected override void OnStop()
        {
            aTimer.Enabled = false;
            var process= Process.GetProcessById((int)procInfo.dwProcessId);
            if (process != null)
            {
                process.Kill();
            }
        }
    }
}
