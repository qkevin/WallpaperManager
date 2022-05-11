using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WallpaperManager.Common
{
    public class WallpaperSettingManager:IDisposable
    {
        public bool KeepRunning { get;private set; }
        private static WallpaperSettingManager _instance;
        public WallpaperSetting WallPaperSetting { get; private set; }
        FileSystemWatcher watcher;
        public string ConfigPath { get; private set; }  
        public static WallpaperSettingManager Root
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WallpaperSettingManager();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public WallpaperSettingManager()
        {
            ConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Config.Json";
            RefreshConfig();
            watcher = new FileSystemWatcher();
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            RefreshConfig();
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            RefreshConfig();
        }

        public void RefreshConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var result = JsonConvert.DeserializeObject<WallpaperSetting>(File.ReadAllText(ConfigPath));
                    if (result != null)
                    {
                        this.WallPaperSetting = result;
                    }
                    else
                    {
                        this.WallPaperSetting = new WallpaperSetting();
                    }
                }
                else
                {
                    this.WallPaperSetting = new WallpaperSetting();
                }
            }
            catch (Exception ex)
            {
                this.WallPaperSetting = new WallpaperSetting();
            }
        }

        public void Save()
        {
            var newSetting = JsonConvert.SerializeObject(WallPaperSetting);
            System.IO.File.WriteAllText(ConfigPath, newSetting);
        }

        public void Dispose()
        {
            KeepRunning = false;
        }
    }
}
