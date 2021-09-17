using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExtensionMethods;

namespace Screenshot_Editor
{
    public class MainViewModel : OnPropertyChange
    {
        #region Constructor

        public MainViewModel()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            OnLoadCommand = new RelayCommand(OnLoadExecute);
        }

        #endregion

        #region ICommands

        public ICommand OnLoadCommand { get; set; }

        public void OnLoadExecute(object obj)
        {
            try
            {
                FontsList = new ObservableCollection<string>(new InstalledFontCollection().Families.Select(s => s.Name));

                SelectedFont = FontsList.FirstOrDefault(fod => fod == "Arial");

                XY = Sizes.FirstOrDefault(fod => fod.Size == "720p");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Public Properties

        private ObservableCollection<string> _accents = new ObservableCollection<string>
        {
            "Amber",
            "Blue",
            "Brown",
            "Cobalt",
            "Crimson",
            "Cyan",
            "Emerald",
            "Green",
            "Indigo",
            "Lime",
            "Magenta",
            "Mauve",
            "Olive",
            "Orange",
            "Pink",
            "Purple",
            "Red",
            "Sienna",
            "Steel",
            "Taupe",
            "Teal",
            "Violet",
            "Yellow",
        };

        public ObservableCollection<string> Accents
        {
            get => _accents;
            set { _accents = value; OnPropertyChanged(); }
        }

        private string _selectedAccent = Properties.Settings.Default.AccentTheme;

        public string SelectedAccent
        {
            get => _selectedAccent ?? "Blue";
            set
            {
                _selectedAccent = value;

                Properties.Settings.Default.AccentTheme = value;
                Properties.Settings.Default.Save();

                OnPropertyChanged();
            }
        }


        private ObservableCollection<string> _fonts = new ObservableCollection<string>();

        public ObservableCollection<string> FontsList
        {
            get => _fonts;
            set { _fonts = value; OnPropertyChanged(); }
        }

        private string _selectedFont;

        public string SelectedFont
        {
            get => _selectedFont;
            set { _selectedFont = value; OnPropertyChanged(); }
        }

        //private int _canvasX = 800;

        //public int CanvasX
        //{
        //    get => _canvasX;
        //    set { _canvasX = value; OnPropertyChanged(); }
        //}

        //private int _canvasY = 600;

        //public int CanvasY
        //{
        //    get => _canvasY;
        //    set { _canvasY = value; OnPropertyChanged(); }
        //}

        private ObservableCollection<CanvasXY> _sizes = new ObservableCollection<CanvasXY>()
        {
            new CanvasXY()
            {
                Size = "Custom",
                X = 800,
                Y = 600
            },
            new CanvasXY()
            {
                Size = "800x600",
                X = 800,
                Y = 600
            },
            new CanvasXY()
            {
                Size = "720p",
                X = 1280,
                Y = 720
            },
            new CanvasXY()
            {
                Size = "1080p",
                X = 1920,
                Y = 1080
            },
            new CanvasXY()
            {
                Size = "1440p",
                X = 2560,
                Y = 1440
            },
            new CanvasXY()
            {
                Size = "4K",
                X = 3840,
                Y = 2160
            }
        };

        public ObservableCollection<CanvasXY> Sizes
        {
            get => _sizes;
            set { _sizes = value; OnPropertyChanged(); }
        }

        private CanvasXY _xy;

        public CanvasXY XY
        {
            get => _xy;
            set { _xy = value; OnPropertyChanged(); }
        }

        private string _imageFile = string.Empty;

        public string ImageFile
        {
            get => _imageFile;
            set { _imageFile = value; OnPropertyChanged(); }
        }

        private decimal? _textBackgroudOpacity = 0;

        public decimal? TextBackgroundOpacity
        {
            get => _textBackgroudOpacity ?? 0;
            set { _textBackgroudOpacity = value; OnPropertyChanged(); }
        }

        private BitmapImage _image = new BitmapImage();

        public BitmapImage Image
        {
            get => _image;
            set { _image = value; OnPropertyChanged(); }
        }

        #endregion

        #region Private Properties



        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion
    }

    public class CanvasXY : OnPropertyChange
    {
        private string _size;

        public string Size
        {
            get => _size;
            set { _size = value; OnPropertyChanged(); }
        }

        private int _x;

        public int X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(); }
        }

        private int _y;

        public int Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(); }
        }
    }
}
