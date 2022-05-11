using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WallpaperManager.Common;

namespace WallpaperManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WallpaperSwitchManager manager;
        private ConcurrencyTaskScheduler lclts;
        public MainWindow()
        {
            if ((App.Current as App).StartWithoutGUI)
            {
                this.Visibility = System.Windows.Visibility.Hidden;
                this.ShowInTaskbar = false;
            }
            InitializeComponent();
            lclts = new ConcurrencyTaskScheduler(Environment.ProcessorCount);
            manager = new WallpaperSwitchManager();
            manager.Start();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            manager.Stop();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (WallpaperSettingManager.Root.WallPaperSetting != null)
            {
                this.txtFolderPath.Text = WallpaperSettingManager.Root.WallPaperSetting.FolderPath;
                this.chkIsRandom.IsChecked = WallpaperSettingManager.Root.WallPaperSetting.IsRandom;
                this.txtUpdateInterval.Text = WallpaperSettingManager.Root.WallPaperSetting.UpdateInterval.ToString();
                UpdatePictureList(this.txtFolderPath.Text);
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtFolderPath.Text = folderBrowserDialog.SelectedPath;
                UpdatePictureList(folderBrowserDialog.SelectedPath);
            }
        }

        private void UpdatePictureList(string folderPath)
        {
            if (System.IO.Directory.Exists(folderPath))
            {
                var files = System.IO.Directory.GetFiles(folderPath, "*.bmp");
                var lst = new List<ImageModel>();
                foreach (var item in files)
                {
                    lst.Add(new ImageModel(this.Dispatcher, lclts) { ImagePath = item });
                }
                this.lstBox.ItemsSource = lst;
            }
        }

        private void lstBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var path = e.AddedItems[0].ToString();
            WallpaperAPI.ChangeSystemWallPaper(path, chkIsTile.IsChecked.Value);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUpdateInterval.Text))
            {
                System.Windows.MessageBox.Show("Update interval not set");
                return;
            }
            try
            {
                var processes = System.Diagnostics.Process.GetProcessesByName(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

                if (processes != null && processes.Any())
                {
                    foreach (var item in processes)
                    {
                        if (item.Id != System.Diagnostics.Process.GetCurrentProcess().Id)
                        {
                            item.Kill();
                        }
                    }
                }
                var setting = WallpaperSettingManager.Root.WallPaperSetting;
                setting.FolderPath = this.txtFolderPath.Text;
                setting.IsRandom = chkIsRandom.IsChecked.Value;
                setting.IsTile = chkIsTile.IsChecked.Value;
                setting.ImagePathList = System.IO.Directory.GetFiles(this.txtFolderPath.Text, "*.bmp").ToList();
                setting.UpdateInterval = int.Parse(txtUpdateInterval.Text);
                WallpaperSettingManager.Root.Save();
                System.Windows.MessageBox.Show("Save config completed");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Save failed, exception: " + ex.ToString());
            }
        }
    }
}
