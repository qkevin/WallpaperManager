using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WallpaperManager
{
   public class ImageModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ConcurrencyTaskScheduler _lcts;

        private Dispatcher _disPatcher;
        public ImageModel(Dispatcher dispatcher, ConcurrencyTaskScheduler lcts)
        {
            _disPatcher = dispatcher;
            _lcts = lcts;
        }
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ImageSource image;

        public ImageSource Image
        {
            get { return image; }
            set
            {
                image = value;
                RaisePropertyChanged();
            }
        }

        private string imagePath;

        public string ImagePath
        {
            get
            {
                return imagePath; }
            set
            {
                imagePath = value;
                LoadImage(imagePath);
                RaisePropertyChanged();
            }
        }

        private void LoadImage(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                var task = new Task(() =>
                {
                    using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = fs;
                        image.EndInit();
                        image.Freeze();
                        _disPatcher.BeginInvoke(new Action(() =>
                        Image = image
                        ), DispatcherPriority.ContextIdle);
                    }
                });
                task.Start(_lcts);
            }
        }

    }
}
