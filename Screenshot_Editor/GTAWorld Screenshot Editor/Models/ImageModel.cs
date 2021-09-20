using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExtensionMethods;

namespace GTAWorld_Screenshot_Editor.Models
{
    public class ImageModel : OnPropertyChange
    {
        private string _guid = $"{System.Guid.NewGuid().ToString()}-{System.Guid.NewGuid().ToString()}";

        public string Guid
        {
            get => _guid;
            set { _guid = value; OnPropertyChanged(); }
        }

        private string _imagePath = string.Empty;

        public string Path
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        private BitmapImage _bitmap = new BitmapImage();

        public BitmapImage Bitmap
        {
            get => _bitmap;
            set { _bitmap = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Resize image based on selected resolution
        /// </summary>
        public void ResizeImage(int x, int y)
        {
            if (string.IsNullOrEmpty(Path))
                return;

            Bitmap = new BitmapImage(new Uri(Path));
            Bitmap.DecodePixelHeight = y;
            Bitmap.DecodePixelWidth = x;
            Bitmap.CacheOption = BitmapCacheOption.OnLoad;
        }

        public void InitImage()
        {
            if (string.IsNullOrEmpty(Path))
                return;

            Bitmap = new BitmapImage(new Uri(Path));
            Bitmap.CacheOption = BitmapCacheOption.OnLoad;
        }
    }
}
