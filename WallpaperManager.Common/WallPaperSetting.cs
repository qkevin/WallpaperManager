using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperManager.Common
{
   public class WallpaperSetting
    {
        public string FolderPath { get; set; }
        public int UpdateInterval { get; set; }
        public List<string> ImagePathList { get; set; }
        public bool IsRandom { get; set; }
        public bool IsTile { get; set; }
        public string LastUpdatTime { get; set; }
        public string LastUpdateImage { get; set; }
        public WallpaperSetting()
        {
            UpdateInterval = 24;
            ImagePathList = new List<string>();
        }
    }
}
