using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WallpaperManager.Common
{
    public class WallpaperSwitchManager
    {
        DateTime? lastUpdateTime;
        string lastImage;
        int index = 0;
        private bool keepRunning;
        private Thread workThread;
        private const int ThreadInteraval = 30000;
        private ManualResetEvent threadSleepSigal = new ManualResetEvent(true);
        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Start()
        {
            WallpaperSettingManager.Root.RefreshConfig();
            lastUpdateTime = ParseDateTime(WallpaperSettingManager.Root.WallPaperSetting.LastUpdatTime);
            lastImage = WallpaperSettingManager.Root.WallPaperSetting.LastUpdateImage;
            if (!string.IsNullOrEmpty(lastImage) && WallpaperSettingManager.Root.WallPaperSetting.ImagePathList.Contains(lastImage))
            {
                WallpaperAPI.ChangeSystemWallPaper(lastImage, WallpaperSettingManager.Root.WallPaperSetting.IsTile);
                index = WallpaperSettingManager.Root.WallPaperSetting.ImagePathList.IndexOf(lastImage) + 1;
            }
            else if (WallpaperSettingManager.Root.WallPaperSetting.ImagePathList!=null && WallpaperSettingManager.Root.WallPaperSetting.ImagePathList.Any())
            {
                WallpaperAPI.ChangeSystemWallPaper(WallpaperSettingManager.Root.WallPaperSetting.ImagePathList[0], WallpaperSettingManager.Root.WallPaperSetting.IsTile);
            }
            keepRunning = true;
            threadSleepSigal.Reset();
            workThread = new Thread(Work);
            workThread.IsBackground = true;
            workThread.Start();
        }

        public void Stop()
        {
            keepRunning = false;
            threadSleepSigal.Set();
            workThread.Join();
            lastUpdateTime = null;
            WallpaperSettingManager.Root = null;
        }

        private DateTime ParseDateTime(string lastUpdateTime)
        {
            var dateTime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(lastUpdateTime))
           {
               DateTime.TryParse(lastUpdateTime, out dateTime);
           }
           return dateTime;
        }

        private void Work()
        {
            while (keepRunning)
            {
                try
                {
                    AutoChangeWallPaper();
                    threadSleepSigal.WaitOne(ThreadInteraval);
                }
                catch (Exception ex)
                {
                    log.Error("Work()", ex);
                }
            }
        }

        private void AutoChangeWallPaper()
        {
            if (lastUpdateTime != null)
            {
                ChangeWallPaper(lastUpdateTime);
            }
            else
            {
                ChangeWallPaper(null);
            }
        }

        private void ChangeWallPaper(DateTime? lastUpdateTime)
        {
            log.Info("ChangeWallPaper started");

            var wallPaperSetting = WallpaperSettingManager.Root.WallPaperSetting;
            if (wallPaperSetting == null || wallPaperSetting.ImagePathList == null || wallPaperSetting.ImagePathList.Count == 0)
                return;
            if (lastUpdateTime != null)
            {
                if ((DateTime.Now - lastUpdateTime.Value).TotalHours >= wallPaperSetting.UpdateInterval)
                {
                    UpdateWallPaper(wallPaperSetting);
                }
            }
            else
            {
                UpdateWallPaper(wallPaperSetting);
            }
            log.Info("ChangeWallPaper finished");
        }

        private void UpdateWallPaper(WallpaperSetting wallPaperSetting)
        {
            if (wallPaperSetting.IsRandom)
            {
                Random random = new Random((int)DateTime.Now.Ticks);
                var path = wallPaperSetting.ImagePathList[random.Next(wallPaperSetting.ImagePathList.Count)];
                WallpaperAPI.ChangeSystemWallPaper(path, wallPaperSetting.IsTile);
                WallpaperSettingManager.Root.WallPaperSetting.LastUpdateImage = path;
                log.Info("Random mode, UpdateWallPaper to " + path);
            }
            else
            {
                if (index >= wallPaperSetting.ImagePathList.Count)
                {
                    index = 0;
                }
                WallpaperAPI.ChangeSystemWallPaper(wallPaperSetting.ImagePathList[index], wallPaperSetting.IsTile);
                WallpaperSettingManager.Root.WallPaperSetting.LastUpdateImage = wallPaperSetting.ImagePathList[index];
                log.Info("Nomal mode, UpdateWallPaper to " + wallPaperSetting.ImagePathList[index]);
                index++;
            }
            this.lastUpdateTime = DateTime.Now;
            WallpaperSettingManager.Root.WallPaperSetting.LastUpdatTime = lastUpdateTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
            WallpaperSettingManager.Root.Save();
        }
    }
}
