using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperManager.Common
{
   public class WallpaperAPI
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);//系统外观的API  

        //[DllImport("shell32.dll", PreserveSig = true, CharSet = CharSet.Auto)]
        //public static extern void SHGetSpecialFolderLocation(int hwnd, int csidl, ref IntPtr ppidl);//寻找指定系统文件夹的API  

        //[DllImport("shell32.dll", PreserveSig = true, CharSet = CharSet.Auto)]
        //public static extern void SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);//给出指定系统文件夹地址的API，配合上面那个使用 

        public static void ChangeSystemWallPaper(string bmpPath, bool tile=false)
        {
            using (RegistryKey rkWallPaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true))
            {
                if (tile)
                {
                    rkWallPaper.SetValue("WallpaperStyle", "0");
                    rkWallPaper.SetValue("TileWallpaper", "1");
                }
                else
                {
                    rkWallPaper.SetValue("WallpaperStyle", "2");
                    rkWallPaper.SetValue("TileWallpaper", "0");
                }
            }
            SystemParametersInfo(0x0014, 0, bmpPath, 0x2 | 0x1);
        }
    }
}
