using ExtensionMethods;
using GTAWorld_Screenshot_Editor.Controllers;
using GTAWorld_Screenshot_Editor.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Windows.System.UserProfile;
using Clipboard = System.Windows.Clipboard;
using Message = ExtensionMethods.Message;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using CroppingAdorner = CroppingImageLibrary.CroppingAdorner;

namespace GTAWorld_Screenshot_Editor
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

            RageMPFolderCommand = new RelayCommand(RageMPFolderExecute);

            SelectImageCommand = new RelayCommand(SelectImageExecute);

            ResizeImageCommandCommand = new RelayCommand(ResizeImageCommandExecute);

            DragDropCommand = new RelayCommand(DragDropExecute);

            ParseChatCommand = new RelayCommand(ParseChatExecute);

            AddTextToImageCommand = new RelayCommand(AddTextToImageExecute);

            ResetCommand = new RelayCommand(ResetExecute);

            LoadCacheCommand = new RelayCommand(LoadCacheExecute);

            DeleteCachedImageCommand = new RelayCommand(DeleteCachedImageExecute);

            ReadOcrCommand = new RelayCommand(ReadOcrExecute);

            CopyColorCodeCommand = new RelayCommand(CopyColorCodeExecute);

            CropImageCommand = new RelayCommand(CropImageExecute);
        }

        #endregion

        #region ICommands

        public ICommand OnLoadCommand { get; set; }

        public void OnLoadExecute(object obj)
        {
            try
            {
                LookForMainDirectory();

                SelectedResolution = Resolutions.FirstOrDefault(fod => fod.Name == "720p");

                //load parser settings
                if (File.Exists(@"./parser.cfg"))
                {
                    ParserSettings = Xml.Deserialize<ObservableCollection<Criteria>>(@"./parser.cfg");
                }
                else Xml.Serialize<ObservableCollection<Criteria>>(@"./parser.cfg", ParserSettings);

                Fonts = new ObservableCollection<string>(new InstalledFontCollection().Families.Select(s => s.Name));

                if (File.Exists(@"./cached_screens/screenshots.cache"))
                {
                    ScreenCache =
                        Xml.Deserialize<ObservableCollection<CacheScreenshot>>(@"./cached_screens/screenshots.cache");

                    foreach (var cacheScreenshot in ScreenCache)
                    {
                        cacheScreenshot.Command = DeleteCachedImageCommand;

                        cacheScreenshot.InitImage();
                    }
                }
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand RageMPFolderCommand { get; set; }

        public void RageMPFolderExecute(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(RageFolder)
                    && MessageBox.Show("Are you sure you want to select a new folder?", "Folder Select",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    return;

                var dialog = new FolderBrowserDialog
                {
                    Description = "Select RageMP Installation folder",
                    Tag = "RageMP",
                    ShowNewFolderButton = false
                };

                if (dialog.ShowDialog() == DialogResult.Abort || string.IsNullOrEmpty(dialog.SelectedPath))
                    return;

                RageFolder = $@"{dialog.SelectedPath}\";
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand SelectImageCommand { get; set; }

        public void SelectImageExecute(object obj)
        {
            try
            {
                var openFileDiag = new OpenFileDialog
                {
                    Title = "Select screenshot",
                    Multiselect = false
                };

                openFileDiag.ImageTypes();

                if (openFileDiag.ShowDialog() == false)
                    return;

                SelectedImage.Path = openFileDiag.FileName;

                SelectedImage.ResizeImage(SelectedResolution.Width, SelectedResolution.Height);
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand ResizeImageCommandCommand { get; set; }

        public void ResizeImageCommandExecute(object obj)
        {
            try
            {
                SelectedImage.ResizeImage(SelectedResolution.Width, SelectedResolution.Height);
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand DragDropCommand { get; set; }

        public void DragDropExecute(object obj)
        {
            try
            {
                if (obj == null)
                    return;

                if(!CheckIfFileIsImage(obj.ToString()))
                    throw new Exception($"'{obj}' is not an image.");

                SelectedImage.Path = obj.ToString();

                SelectedImage.ResizeImage(SelectedResolution.Width, SelectedResolution.Height);
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand ParseChatCommand { get; set; }

        public void ParseChatExecute(object obj)
        {
            try
            {
                //save parser filter changes
                Xml.Serialize<ObservableCollection<Criteria>>(@"./parser.cfg", ParserSettings);

                ChatParser.InitializeServerIp();

                if (string.IsNullOrWhiteSpace(RageFolder) || !Directory.Exists(RageFolder + "client_resources\\"))
                {
                    MessageBox.Show("Invalid RAGEMP directory path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!File.Exists(RageFolder + ChatParser.LogLocation))
                {
                    MessageBox.Show("Can't find the GTA World chat log.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //parse chat log
                ParsedChat = ChatParser.ParseChatLog(RageFolder, true, true);

                //filter chatlog based on selections
                ParsedChat = ChatParser.TryToFilter(ParsedChat, ParserSettings.ToList(), ParserSettings.FirstOrDefault(fod => fod.Name == "Other (non listed)").Selected);
                
                //reset rtf textbox line height
                LineHeight = 1;
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand AddTextToImageCommand { get; set; }

        public void AddTextToImageExecute(object obj)
        {
            try
            {
                if(obj == null || obj.ToString() != "no_save")
                    CacheCurrentImageAndText();

                ScreenshotText.Clear();

                //remove highlight if left
                ParsedChat = ParsedChat.Replace("[!] ", "");

                var lines = ParsedChat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(w => !string.IsNullOrEmpty(w));

                if (lines.Count() > 100)
                    throw new Exception(
                        "Your parse chat contains more than 100 lines, please edit the some out and try again.");

                var lineCount = 0;

                var shadowColor = new Color
                {
                    R = Byte.MinValue,
                    G = Byte.MinValue,
                    B = Byte.MinValue
                };

                foreach (var line in lines)
                {
                    var str = line.TrimEnd(' ');

                    Console.WriteLine(line);

                    if (!line.EndsWith(".") && !line.EndsWith("?") && !line.EndsWith("!") && !line.EndsWith("!?") && !line.EndsWith("?!"))
                        str = $"{line.TrimEnd(' ')}.";

                    var newLine = lineCount + 1 == lines.Count() ? "" : "\n";

                    var _outlinedTextBlock = new OutlinedTextBlock();

                    var color = GetColor(str);

                    _outlinedTextBlock.Text = $"{str.Replace($"({color})", string.Empty).TrimEnd(' ')}{newLine}";
                    _outlinedTextBlock.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
                    _outlinedTextBlock.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#000");
                    _outlinedTextBlock.StrokeThickness = TextSettings.StrokeThickness / 100;
                    _outlinedTextBlock.FontSize = TextSettings.FontSize;
                    _outlinedTextBlock.FontFamily = new FontFamily(TextSettings.FontFamily);
                    _outlinedTextBlock.TextWrapping = TextWrapping.WrapWithOverflow;
                    _outlinedTextBlock.FontWeight = FontWeight.FromOpenTypeWeight(TextSettings.FontWeight);
                    _outlinedTextBlock.Effect = new DropShadowEffect
                    {
                        Color = shadowColor,
                        Opacity = TextSettings.ShadowOpacity / 100
                    };

                    ScreenshotText.Add(_outlinedTextBlock);

                    lineCount++;
                }
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand ResetCommand { get; set; }

        public void ResetExecute(object obj)
        {
            try
            {
                TextSettings = new TextModel();

                ParsedChat = string.Empty;

                SelectedResolution = Resolutions.FirstOrDefault(fod => fod.Name == "720p");

                SelectedImage = new ImageModel();

                ScreenshotText.Clear();
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand LoadCacheCommand { get; set; }

        public void LoadCacheExecute(object obj)
        {
            try
            {
                if (obj == null)
                    return;

                var cache = (CacheScreenshot)obj;

                ParsedChat = cache.ScreenshotText;

                SelectedImage.Path = cache.ImageFullPath;

                SelectedImage.Guid = cache.Guid;

                SelectedImage.ResizeImage(SelectedResolution.Width, SelectedResolution.Height);
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand DeleteCachedImageCommand { get; set; }

        public void DeleteCachedImageExecute(object obj)
        {
            try
            {
                if (obj == null)
                    return;

                if (MessageBox.Show("Are you sure you wanted to delete selected screenshot", "Confirm Delition",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    return;

                var cacheDir = @"cached_screens";

                var cache = (CacheScreenshot)obj;

                File.Delete(cache.ImageFullPath);

                ScreenCache.Remove(cache);

                if (ScreenCache.Count > 0)
                {
                    ScreenCache =
                        new ObservableCollection<CacheScreenshot>(ScreenCache.OrderByDescending(obd => obd.ScreenshotDate));

                    Xml.Serialize<ObservableCollection<CacheScreenshot>>($@"{cacheDir}\screenshots.cache", ScreenCache);
                }
                else File.Delete($@"{cacheDir}\screenshots.cache");
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand ReadOcrCommand { get; set; }

        public async void ReadOcrExecute(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedImage.Path))
                    return;

                var ocr = new OcrService();

                ParsedChat = await ocr.ExtractText(SelectedImage.Path, "en-US");

                ParsedChat = Regex.Replace(ParsedChat, @"\[\d{1,2}:\d{1,2}:\d{1,2}\] ", string.Empty);
                ParsedChat = Regex.Replace(ParsedChat, @"\[\d{1,2}:\d{1,2} :\d{1,2}\] ", string.Empty);
                ParsedChat = Regex.Replace(ParsedChat, @"\[\d{1,2}:\d{1,2} ", string.Empty);

                AddTextToImageCommand.Execute("no_save");
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand CopyColorCodeCommand { get; set; }

        public void CopyColorCodeExecute(object obj)
        {
            try
            {
                if (obj == null)
                    return;

                var hex = obj.ToString();

                Clipboard.SetText($"({hex})");
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand CropImageCommand { get; set; }

        public void CropImageExecute(object obj)
        {
            try
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(Canvas);

                //initiate crop
                if ((bool)obj)
                {
                    //add cropping to ui on canvas
                    CroppingAdorner = new CroppingAdorner(Canvas);

                    adornerLayer?.Add(CroppingAdorner);
                }
                //finish crop and store image
                else
                {
                    //create temp image folder
                    if (!Directory.Exists(@"temp"))
                        Directory.CreateDirectory(@"temp");

                    //file datetime
                    var date = $"{DateTime.Now:yyyyMMdd_HHmmss}";

                    //save crop in temp folder
                    using (var fs = new FileStream($@"temp/temp_{date}.jpg", FileMode.Create))
                    {
                        var encoder = new JpegBitmapEncoder();

                        encoder.Frames.Add(CroppingAdorner.GetCroppedBitmapFrame());

                        //remove cropping from ui
                        adornerLayer?.Remove(CroppingAdorner);

                        encoder.Save(fs);
                    }

                    //load new image from temp
                    SelectedImage.Path = $@"{AppDomain.CurrentDomain.BaseDirectory}temp\temp_{date}.jpg";

                    //init image onto canvas
                    SelectedImage.InitImage();

                    //select custom resolutiuon
                    SelectedResolution = Resolutions.FirstOrDefault(fod => fod.Name == "Custom");

                    if (SelectedResolution == null)
                        return;

                    //apply image resolution to canvas
                    SelectedResolution.Height = (int)SelectedImage.Bitmap.Height;
                    SelectedResolution.Width = (int)SelectedImage.Bitmap.Width;
                }
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        #endregion

        #region Public Properties

        private string _rageFolder = Properties.Settings.Default.DirectoryPath;

        public string RageFolder
        {
            get => _rageFolder;
            set
            {
                _rageFolder = value;
                OnPropertyChanged();

                Properties.Settings.Default.DirectoryPath = value;
                Properties.Settings.Default.Save();
            }
        }

        private string _parsedChat = string.Empty;

        public string ParsedChat
        {
            get => _parsedChat;
            set { _parsedChat = value; OnPropertyChanged(); }
        }

        private ObservableCollection<OutlinedTextBlock> _screenshotText = new ObservableCollection<OutlinedTextBlock>();

        public ObservableCollection<OutlinedTextBlock> ScreenshotText
        {
            get => _screenshotText;
            set { _screenshotText = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ResolutionPreset> _resolutions = new ObservableCollection<ResolutionPreset>()
        {
            new ResolutionPreset
            {
                AllowEdit = true,
                Name = "Custom",
                Width = 1280,
                Height = 720
            },

            new ResolutionPreset
            {
                Name = "720p",
                Width = 1280,
                Height = 720
            },

            new ResolutionPreset
            {
                Name = "1080p",
                Width = 1920,
                Height = 1080
            },

            new ResolutionPreset
            {
                Name = "1440p",
                Width = 2560,
                Height = 1440
            },

            new ResolutionPreset
            {
                Name = "4k",
                Width = 3840,
                Height = 2160
            },
        };

        private ObservableCollection<Criteria> _parserSettings = new ObservableCollection<Criteria>
        {
            new Criteria
            {
                Name = "IC",
                Filter = @"^(\(Car\) ){0,1}[\p{L}]+ {0,1}([\p{L}]+){0,1} (says|shouts|whispers)( \[low\]){0,1}:.*$",
                Selected = true
            },

            new Criteria
            {
                Selected = true,
                Name = "Cellphone",
                Filter = @"(\(cellphone\))"
            },

            new Criteria
            {
                Selected = true,
                Name = "Whispers",
                Filter = @"\whispers:"
            },

            new Criteria
            {
                Selected = true,
                Name = "Payments",
                Filter = @".(\$xxxx)."
            },

            new Criteria
            {
                Selected = true,
                Name = "Items",
                Filter = @".(\(x\) to|received \(x\) of)."
            },

            new Criteria
            {
                Name = "Radio",
                Filter = @"(\*\*\[S\:)"
            },

            new Criteria
            {
                Name = "Dept. Radio",
                Filter = @"[(\:\\*\*\\[)]"
            },

            new Criteria
            {
                Name = "Megaphone",
                Filter = @"(\[Megaphone\])"
            },

            new Criteria
            {
                Name = "OOC",
                Filter = @"[\(\(\(]"
            },

            new Criteria
            {
                Name = "PM",
                Filter = @"^\(\( PM (to|from)"
            },

            new Criteria
            {
                Name = "AD",
                Filter = @"(\[Advertisement])"
            },

            new Criteria
            {
                Name = "BAD",
                Filter = @"(\[BusinessAdvertisement])"
            },

            new Criteria
            {
                Name = "CAD",
                Filter = @"(\[CompanyAdvertisement])"
            },

            new Criteria
            {
                Name = "Other (non listed)",
                Filter = string.Empty
            },
        };

        public ObservableCollection<Criteria> ParserSettings
        {
            get => _parserSettings;
            set { _parserSettings = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ResolutionPreset> Resolutions
        {
            get => _resolutions;
            set { _resolutions = value; OnPropertyChanged(); }
        }

        private ResolutionPreset _selectedResolution;

        public ResolutionPreset SelectedResolution
        {
            get => _selectedResolution;
            set { _selectedResolution = value; OnPropertyChanged(); }
        }

        private ImageModel _selectedImage = new ImageModel();

        public ImageModel SelectedImage
        {
            get => _selectedImage;
            set { _selectedImage = value; OnPropertyChanged(); }
        }

        private TextModel _textSettings = new TextModel();

        public TextModel TextSettings
        {
            get => _textSettings;
            set { _textSettings = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _fonts = new ObservableCollection<string>();

        public ObservableCollection<string> Fonts
        {
            get => _fonts;
            set { _fonts = value; OnPropertyChanged(); }
        }

        private int _lineHeight = 1;

        public int LineHeight
        {
            get => _lineHeight;
            set { _lineHeight = value; OnPropertyChanged(); }
        }

        private ObservableCollection<CacheScreenshot> _screenCache = new ObservableCollection<CacheScreenshot>();

        public ObservableCollection<CacheScreenshot> ScreenCache
        {
            get => _screenCache;
            set { _screenCache = value; OnPropertyChanged(); }
        }

        private CroppingAdorner _cropping;

        public CroppingAdorner CroppingAdorner
        {
            get => _cropping;
            set { _cropping = value; OnPropertyChanged(); }
        }

        private System.Windows.Controls.Canvas _canvas;

        public System.Windows.Controls.Canvas Canvas
        {
            get => _canvas;
            set { _canvas = value; OnPropertyChanged(); }
        }

        #endregion

        #region Private Properties

        private List<string> InstalledLanguages => GlobalizationPreferences.Languages.ToList();

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        /// <summary>
        /// Get HEX color based on chat line type
        /// </summary>
        private string GetColor(string line)
        {
            // Detect if line starts with hex color code
            // If found, pull it out and remove from line,
            // return hex-color code to paint this line.
            var eightHex = Regex.IsMatch(line, @"^\(#(?:[0-9a-fA-F]{4}){1,2}\)", RegexOptions.IgnoreCase);

            if (eightHex || Regex.IsMatch(line, @"^\(#(?:[0-9a-fA-F]{3}){1,2}\)", RegexOptions.IgnoreCase))
            {
                return eightHex
                    ? Regex.Matches(line, @"^\(#(?:[0-9a-fA-F]{4}){1,2}\)")[0].Value
                        .Replace("(", string.Empty)
                        .Replace(")", string.Empty)
                    : Regex.Matches(line, @"^\(#(?:[0-9a-fA-F]{3}){1,2}\)")[0].Value
                    .Replace("(", string.Empty)
                        .Replace(")", string.Empty);
            }

            if (line.Contains("*") || line.Contains(">"))
            {
                return "#bea3d6";//purple
            }

            if (line.Contains("[low]"))
            {
                return "#a6a4a6";//grey
            }

            if (line.Contains("paid") || line.Contains("gave") || line.Contains("received (x)"))
            {
                return "#29943e";//green
            }

            if (line.Contains("whispers") && line.Contains("(Car)"))
            {
                return "#f9fb12";//yellow
            }

            if (line.Contains("whispers"))
            {
                return "#eda841";//orange
            }

            return "#fff";//white
        }

        /// <summary>
        /// Looks for the main RAGEMP directory
        /// path on the first start
        /// </summary>
        private void LookForMainDirectory()
        {
            try
            {
                if (!string.IsNullOrEmpty(RageFolder))
                    return;

                var keyValue = Registry.GetValue(@"HKEY_CURRENT_USER\Software\RAGE-MP", "rage_path", null);
                if (keyValue != null)
                {
                    RageFolder = keyValue + @"\";
                    MessageBox.Show(
                        $"Automatically found your RAGEMP directory at {RageFolder}\n\nPlease browse for the correct path manually if this is incorrect or you have multiple RAGEMP installations.",
                        "Information (First Start)", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new IOException();
                }
            }
            catch(Exception ex)
            {
                Message.Log(ex);
            }
        }

        private void CacheCurrentImageAndText()
        {
            if (ScreenCache.Any(a => a.Guid == SelectedImage.Guid) && ScreenCache.Count > 0 || string.IsNullOrEmpty(SelectedImage.Path))
                return;

            var cached = new CacheScreenshot
            {
                Guid = SelectedImage.Guid,
                ScreenshotText = ParsedChat,
                Command = DeleteCachedImageCommand
            };

            ScreenCache.Add(cached);

            var cacheDir = @"cached_screens";

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            var suffix = SelectedImage.Path.Split('.').OrderByDescending(obd => obd).ToList();

            var fileName = $@"{cacheDir}\screenshot_{cached.ScreenshotDate:yyyyMMdd_hhmmss}.{suffix[0]}";

            File.Copy(SelectedImage.Path, fileName);

            cached.ImageFilePath = fileName;

            cached.InitImage();

            ScreenCache =
                new ObservableCollection<CacheScreenshot>(ScreenCache.OrderByDescending(obd => obd.ScreenshotDate));

            Xml.Serialize<ObservableCollection<CacheScreenshot>>($@"{cacheDir}\screenshots.cache", ScreenCache);
        }

        private bool CheckIfFileIsImage(string file)
        {
            return Regex.IsMatch(Path.GetExtension(file).ToLower(),
                "(jpg|jpeg|jfif|png|bmp|webp|gif)$", RegexOptions.Compiled);
        }

        #endregion
    }
}
