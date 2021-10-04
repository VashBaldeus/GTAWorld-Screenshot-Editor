using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using ExtensionMethods;

namespace GTAWorld_Screenshot_Editor.Models
{
    public class CacheScreenshot : OnPropertyChange
    {
        private string _guid;

        public string Guid
        {
            get => _guid;
            set { _guid = value; OnPropertyChanged(); }
        }

        private DateTime _screenshotDate = DateTime.Now;

        public DateTime ScreenshotDate
        {
            get => _screenshotDate;
            set { _screenshotDate = value; OnPropertyChanged(); }
        }

        private string _imageFile = string.Empty;

        public string ImageFilePath
        {
            get => _imageFile;
            set { _imageFile = value; OnPropertyChanged(); }
        }

        private string _screenshotText = string.Empty;

        public string ScreenshotText
        {
            get => _screenshotText;
            set { _screenshotText = value; OnPropertyChanged(); }
        }

        public string ImageFullPath =>
            $@"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName)}\{ImageFilePath}";

        private TextModel _text = new TextModel();

        public TextModel Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(); }
        }

        private ResolutionPreset _res = new ResolutionPreset
        {
            Name = "720p",
            Width = 1280,
            Height = 720
        };

        public ResolutionPreset Resolution
        {
            get => _res;
            set { _res = value; OnPropertyChanged(); }
        }
        
        [XmlIgnore]
        public ICommand Command { get; set; }

        [XmlIgnore]
        private BitmapImage _bitmap = new BitmapImage();

        [XmlIgnore]
        public BitmapImage Bitmap
        {
            get => _bitmap;
            set { _bitmap = value; OnPropertyChanged(); }
        }

        public void InitImage()
        {
            if (string.IsNullOrEmpty(ImageFilePath))
                return;

            using (var fs = new FileStream(ImageFullPath, FileMode.Open))
            {
                Bitmap.BeginInit();
                Bitmap.StreamSource = fs;
                Bitmap.CacheOption = BitmapCacheOption.OnLoad;
                Bitmap.EndInit();
            }
        }
    }
}
