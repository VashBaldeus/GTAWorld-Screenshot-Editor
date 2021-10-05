using ExtensionMethods;
using GTAWorld_Screenshot_Editor.Controllers;
using GTAWorld_Screenshot_Editor.Models;
using Microsoft.Win32;
using System;
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
using Clipboard = System.Windows.Clipboard;
using Message = ExtensionMethods.Message;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using CroppingAdorner = CroppingImageLibrary.CroppingAdorner;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
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

            ClearImageTextCommand = new RelayCommand(ClearImageTextExecute);

            AddManuallyTypedToImageCommand = new RelayCommand(AddManuallyTypedToImageExecute);

            CopyChatToClipboardCommand = new RelayCommand(CopyChatToClipboardExecute);
        }

        #endregion

        #region ICommands

        public ICommand OnLoadCommand { get; set; }

        public void OnLoadExecute(object obj)
        {
            try
            {
                LookForMainDirectory();

                InitFilters();

                InitResolutions();

                ResetCommand.Execute(null);

                DebugExecute();
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
                    // ReSharper disable once LocalizableElement
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

                //file dialog returned false or file was not selected
                if (openFileDiag.ShowDialog() == false || string.IsNullOrEmpty(openFileDiag.FileName))
                    return;

                InitImage(openFileDiag.FileName);
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

                TextSettings.Width = (int)(SelectedResolution.Width * 0.85);

                if (string.IsNullOrEmpty(ParsedChat))
                    return;

                AddTextToImageCommand.Execute(null);
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

                InitImage(obj.ToString());
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
                ParsedChat = ChatParser.ParseChatLog(RageFolder, TextSettings.ParseLines, true);

                //filter chatlog based on selections
                ParsedChat =
                    ChatParser.TryToFilter(ParsedChat, ParserSettings.ToList(),
                        ParserSettings.FirstOrDefault(fod => fod.Name == "Other (non listed)")?.Selected ?? false);
                
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
                if (obj == null || obj.ToString() != "no_save")
                    CacheCurrentImageAndText();

                GenerateText();
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

                SelectedResolution = cache.Resolution;

                TextSettings = cache.Text;
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
                    SelectedImage.Path = $@"{StartupDirectory}temp\temp_{date}.jpg";

                    //init image onto canvas
                    SelectedImage.InitImage();

                    //select custom resolutiuon
                    SelectedResolution = Resolutions.FirstOrDefault(fod => fod.Name == "Custom");

                    if (SelectedResolution == null)
                        return;

                    //apply image resolution to canvas
                    SelectedResolution.Height = (int)SelectedImage.Bitmap.Height;

                    SelectedResolution.Width = (int)SelectedImage.Bitmap.Width;

                    TextSettings.Width = (int)(SelectedResolution.Width * 0.85);
                }
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand ClearImageTextCommand { get; set; }

        public void ClearImageTextExecute(object obj)
        {
            try
            {
                ScreenshotText.Clear();
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand AddManuallyTypedToImageCommand { get; set; }

        public void AddManuallyTypedToImageExecute(object obj)
        {
            try
            {
                //filter chatlog based on selections
                ParsedChat =
                    ChatParser.TryToFilter(ParsedChat, ParserSettings.ToList(),
                        ParserSettings.FirstOrDefault(fod => fod.Name == "Other (non listed)")?.Selected ?? false);

                //reset rtf textbox line height
                LineHeight = 1;

                if (obj == null || obj.ToString() != "no_save")
                    CacheCurrentImageAndText();

                GenerateText();
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        public ICommand CopyChatToClipboardCommand { get; set; }

        public void CopyChatToClipboardExecute(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(ParsedChat))
                    return;

                Clipboard.SetText(ParsedChat);
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        #endregion

        #region Public Properties

        private string _rageFolder = Properties.Settings.Default.DirectoryPath;

        /// <summary>
        /// RageMP Install directory
        /// </summary>
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

        /// <summary>
        /// Chat that was pulled or written manually
        /// </summary>
        public string ParsedChat
        {
            get => _parsedChat;
            set { _parsedChat = value; OnPropertyChanged(); }
        }

        private int _lineHeight = 1;

        /// <summary>
        /// Assigns Rich Text Box line height
        /// </summary>
        public int LineHeight
        {
            get => _lineHeight;
            set { _lineHeight = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ImageText> _screenshotTextCollection = new ObservableCollection<ImageText>();

        /// <summary>
        /// Text that shows on top of canvas for screenshot
        /// </summary>
        public ObservableCollection<ImageText> ScreenshotText
        {
            get => _screenshotTextCollection;
            set { _screenshotTextCollection = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Criteria> _parserSettings;

        /// <summary>
        /// Parser filter settings and patterns
        /// </summary>
        public ObservableCollection<Criteria> ParserSettings
        {
            get => _parserSettings;
            set { _parserSettings = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ResolutionPreset> _resolutions;

        /// <summary>
        /// Resolutions user can choose from,
        /// includes a Custom resolution allowing manual Height & Width input
        /// </summary>
        public ObservableCollection<ResolutionPreset> Resolutions
        {
            get => _resolutions;
            set { _resolutions = value; OnPropertyChanged(); }
        }

        private ResolutionPreset _selectedResolution;

        /// <summary>
        /// Selected current resolution
        /// </summary>
        public ResolutionPreset SelectedResolution
        {
            get => _selectedResolution;
            set { _selectedResolution = value; OnPropertyChanged(); }
        }

        private ImageModel _selectedImage = new ImageModel();

        /// <summary>
        /// Current loaded image on canvas
        /// </summary>
        public ImageModel SelectedImage
        {
            get => _selectedImage;
            set { _selectedImage = value; OnPropertyChanged(); }
        }

        private TextModel _textSettings = new TextModel();

        /// <summary>
        /// Text Settings, mostly deprecated. TODO remove unused properties
        /// </summary>
        public TextModel TextSettings
        {
            get => _textSettings;
            set { _textSettings = value; OnPropertyChanged(); }
        }

        private ObservableCollection<CacheScreenshot> _screenCache = new ObservableCollection<CacheScreenshot>();

        /// <summary>
        /// Cached screenshots that were created in the past
        /// saves the image and text in another folder locally
        /// </summary>
        public ObservableCollection<CacheScreenshot> ScreenCache
        {
            get => _screenCache;
            set { _screenCache = value; OnPropertyChanged(); }
        }

        private CroppingAdorner _cropping;

        /// <summary>
        /// Cropping Adorner used for manual cropping of loaded image
        /// </summary>
        public CroppingAdorner CroppingAdorner
        {
            get => _cropping;
            set { _cropping = value; OnPropertyChanged(); }
        }

        private System.Windows.Controls.Canvas _canvas;

        /// <summary>
        /// Reference to image Canvas for saving image
        /// </summary>
        public System.Windows.Controls.Canvas Canvas
        {
            get => _canvas;
            set { _canvas = value; OnPropertyChanged(); }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Startup folder of *.exe file
        /// </summary>
        public string StartupDirectory => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Regex pattern, detects FirstName LastName or Mask format of player
        /// </summary>
        public const string PlayerNamePattern = @"([\p{L}]+ {0,1}([\p{L}]+){0,1}|Mask+[a-zA-Z0-9_]+)";

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize Filters, load saved filter file if exits, otherwise create new file
        /// </summary>
        private void InitFilters()
        {
            ParserSettings = new ObservableCollection<Criteria>
            {
                new Criteria
                {
                    Name = "IC",
                    Filter =
                        @"^(\(Car\) ){0,1}((([\p{L}]+ {0,1} [\p{L}]+){0,1})|(Mask+[a-zA-Z0-9_]+ {0,1})) (says|shouts)( \[low\]){0,1}:.*$",
                    Selected = true
                },

                new Criteria
                {
                    Name = "Emote",
                    Filter =
                        @"^(\*|\* .* \(\() ((([\p{L}]+ {0,1} [\p{L}]+){0,1})|(Mask+[a-zA-Z0-9_]+ {0,1}))( .*$|\)\)\*$)",
                    Selected = true
                },

                new Criteria
                {
                    Name = "Above Emote",
                    Filter = @"^(\> .*)((([\p{L}]+ {0,1} [\p{L}]+){0,1})|(Mask+[a-zA-Z0-9_]+ {0,1}))( .*$|\)\)\*$)",
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
                    Filter = @"^[\p{L}]+ [\p{L}]+ paid you (?<SYMBOL>[$]){1}(?<AMOUNT>[\d.,]+).$|^You paid (?<SYMBOL>[$]){1}(?<AMOUNT>[\d.,]+) to ([\p{L}]+ {0,1} [\p{L}]+ {0,1}).$"
                },

                new Criteria
                {
                    Selected = true,
                    Name = "Items",
                    Filter = @"^You (gave|received) [\p{L}]+ \((\d{1,2})\) (to|from) [\p{L}]+ [\p{L}]+.$"
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

            //load parser settings
            if (File.Exists(@"./parser.cfg"))
            {
                ParserSettings = Xml.Deserialize<ObservableCollection<Criteria>>(@"./parser.cfg");
            }
            else Xml.Serialize<ObservableCollection<Criteria>>(@"./parser.cfg", ParserSettings);
        }

        /// <summary>
        /// Initialize Resolutons list
        /// </summary>
        private void InitResolutions()
        {
            Resolutions = new ObservableCollection<ResolutionPreset>
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

            SelectedResolution = Resolutions.FirstOrDefault(fod => fod.Name == "720p");
        }

        /// <summary>
        /// Used for debugging during development
        /// </summary>
        private void DebugExecute()
        {
#if DEBUG
            if(File.Exists(@"parser.cfg"))
                File.Delete(@"parser.cfg");
#endif
        }

        /// <summary>
        /// Generates text on image
        /// </summary>
        private void GenerateText()
        {
            ScreenshotText.Clear();

            //split parsed chat into lines, remove empty strings
            var lines =
                ParsedChat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => !string.IsNullOrEmpty(w)).ToList();

            //line count, used to ignore last line receiving '\n'
            var lineCount = 0;

            foreach (var line in lines)
            {
                var str = line.TrimEnd(' ');

                //if string ends with '\' replace with '.'
                if (line.EndsWith(@"\"))
                {
                    str = $"{line.TrimEnd('\\')}.";
                }

                str = Regex.Replace(str,
                    @"( \(\d{2}/[A-z]{3}/\d{4} - \d{2}:\d{2}:\d{2}\))", string.Empty);

                //if string missing '.' at end, add it.
                if (line.EndsWith("$xxxx") || (!line.EndsWith(".") && !line.EndsWith("?") && !line.EndsWith("!") && !line.EndsWith("!?") && !line.EndsWith("?!")))
                    str = $"{line.TrimEnd(' ')}.";

                //new line marker, last line does not receive new line command
                var newLine = lineCount + 1 == lines.Count ? "" : "\n";

                //get hex color
                var color = GetColor(str);

                //if manually color exists, remove it
                str = $"{str.Replace($"({color})", string.Empty).TrimEnd(' ')}{newLine}";

                //replace payment amount with '$xxxx'
                if (Regex.IsMatch(str,
                    @"^[\p{L}]+ [\p{L}]+ paid you (?<SYMBOL>[$]){1}(?<AMOUNT>[\d.,]+)\.$|^You paid (?<SYMBOL>[$]){1}(?<AMOUNT>[\d.,]+) to ([\p{L}]+ {0,1} [\p{L}]+ {0,1})\.$"))
                {
                    str = Regex.Replace(str, @"(?<SYMBOL>[$]){1}(?<AMOUNT>[\d.,]+)", "$xxxx");
                }

                //replace given/received item amount with '(x)'
                if (Regex.IsMatch(str,
                    @"^You (gave|received) [\p{L}]+ \((\d{1,2})\) (to|from) [\p{L}]+ [\p{L}]+.$"))
                {
                    str = Regex.Replace(str, @"\((?<AMOUNT>[\d.]+)\)", "(x)");
                }

                var txt = new ImageText();

                //set line text
                txt.String = str;

                //set foreground of line
                txt.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(color);

                // select font size based on selected resolution
                // if resolution smaller than 960, will use 960 for font base
                txt.FontSize = SelectedResolution.Height < 960
                    ? 960 * (1.6 / 100)
                    : SelectedResolution.Height * (1.6 / 100);

                //line shadow opacity
                txt.Effect.Opacity = SelectedResolution.Height * (0.18 / 100);

                //line shadow blur radius
                txt.Effect.BlurRadius = SelectedResolution.Height * (0.18 / 100);

                //line shadow direction
                txt.Effect.Direction = SelectedResolution.Height * (0.18 / 100);

                //line shadow depth
                txt.Effect.ShadowDepth = SelectedResolution.Height * (0.18 / 100);

                //add line to collection to dispaly on image
                ScreenshotText.Add(txt);

                //count line
                lineCount++;
            }
        }

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

            //emote
            if (Regex.IsMatch(line, @"^(\*|\>|\* .* \(\() ((([\p{L}]+ {0,1} [\p{L}]+){0,1})|(Mask+[a-zA-Z0-9_]+ {0,1}))( .*$|\)\)\*$)"))
            {
                return "#bea3d6";//purple
            }

            //low speech
            if (line.Contains("[low]"))
            {
                return "#a6a4a6";//grey
            }
            
            //money & item transfers
            if (Regex.IsMatch(line, @"^[\p{L}]+ [\p{L}]+ paid you (?<SYMBOL>[$]){1}(?<AMOUNT>[\d.,]+)\.$|^You paid (?<SYMBOL>[$]){1}(?<AMOUNT>[\d.,]+) to ([\p{L}]+ {0,1} [\p{L}]+ {0,1})\.$")
                || Regex.IsMatch(line, @"^You (gave|received) [\p{L}]+ \((\d{1,2})\) (to|from) [\p{L}]+ [\p{L}]+.$"))
            {
                return "#29943e";//green
            }

            //in-car whispers
            if (Regex.IsMatch(line, @"^(\(Car\) )((([\p{L}]+ {0,1} [\p{L}]+){0,1})|(Mask+[a-zA-Z0-9_]+ {0,1}))( whispers:).*$"))
            {
                return "#f9fb12";//yellow
            }

            //regular whispers
            if (Regex.IsMatch(line, @"^((([\p{L}]+ {0,1} [\p{L}]+){0,1})|(Mask+[a-zA-Z0-9_]+ {0,1}))( whispers:).*$"))
            {
                return "#eda841";//orange
            }

            //default
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

        /// <summary>
        /// Adds created screenshot into cache file
        /// </summary>
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

            cached.Text = TextSettings;

            cached.Resolution = SelectedResolution;

            ScreenCache =
                new ObservableCollection<CacheScreenshot>(ScreenCache.OrderByDescending(obd => obd.ScreenshotDate));

            Xml.Serialize<ObservableCollection<CacheScreenshot>>($@"{cacheDir}\screenshots.cache", ScreenCache);
        }

        /// <summary>
        /// Check if dropped file is an image file
        /// </summary>
        /// <param name="file">dropped file path</param>
        /// <returns>boolean value indicating file type</returns>
        private bool CheckIfFileIsImage(string file)
        {
            return Regex.IsMatch(Path.GetExtension(file).ToLower(),
                "(jpg|jpeg|jfif|png|bmp|webp|gif)$", RegexOptions.Compiled);
        }

        /// <summary>
        /// Initialize dropped or selected iamge
        /// </summary>
        /// <param name="path">image file path</param>
        private void InitImage(string path)
        {
            SelectedImage.Path = path;

            SelectedImage.ResizeImage(SelectedResolution.Width, SelectedResolution.Height);

            //if both height & width of selected image are smaller than selected
            //resolution, set image resolution as custom.
            if (SelectedImage.Bitmap.Height < SelectedResolution.Height ||
                SelectedImage.Bitmap.Width < SelectedResolution.Width)
            {
                SelectedResolution = Resolutions.FirstOrDefault(fod => fod.Name == "Custom");

                if (SelectedResolution == null)
                    return;

                SelectedResolution.Height = (int)SelectedImage.Bitmap.Height;
                SelectedResolution.Width = (int)SelectedImage.Bitmap.Width;
            }
        }

        #endregion
    }
}
