using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using ExtensionMethods;
using GTAWorld_Screenshot_Editor.Models;
using Microsoft.Win32;
using Message = ExtensionMethods.Message;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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

                //TextSettings.FontFamily = Fonts.FirstOrDefault(fod => fod == "Arial");
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

                ParsedChat = ChatParser.ParseChatLog(RageFolder, true, true);

                var regexStrings = ParserSettings.Where(w => w.Selected).Select(s => s.Filter);

                ParsedChat = ChatParser.TryToFilter(ParsedChat, ParserSettings.Where(w => w.Selected).ToList());

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
                ScreenshotText.Clear();

                //remove highlight if left
                ParsedChat = ParsedChat.Replace("[!] ", "");

                var lines = ParsedChat.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(w => !string.IsNullOrEmpty(w));

                if (lines.Count() > 100)
                    throw new Exception(
                        "Your parse chat contains more than 100 lines, please edit the some out and try again.");

                var lineCount = 0;

                var shadowColor = new Color();

                shadowColor.R = Byte.MinValue;
                shadowColor.G = Byte.MinValue;
                shadowColor.B = Byte.MinValue;

                foreach (var line in lines)
                {
                    var str = line;

                    if (!line.EndsWith(".") && !line.EndsWith("?") && !line.EndsWith("!") && !line.EndsWith("!?") && !line.EndsWith("?!"))
                        str = $"{line}.";

                    var newLine = lineCount + 1 == lines.Count() ? "" : "\n";

                    var _outlinedTextBlock = new OutlinedTextBlock()
                    {
                        Text = $"{(line.EndsWith(".") ? line : str)}{newLine}",
                        Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(GetColor(line)),
                        Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#000"),
                        StrokeThickness = TextSettings.StrokeThickness / 100,
                        FontSize = TextSettings.FontSize,
                        FontFamily = new FontFamily(TextSettings.FontFamily),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        FontWeight = FontWeight.FromOpenTypeWeight(TextSettings.FontWeight),
                        Effect = new DropShadowEffect()
                        {
                            //BlurRadius = 1,
                            Color = shadowColor,
                            //ShadowDepth = 1,
                            //Direction = 1,
                            Opacity = TextSettings.ShadowOpacity / 100
                        }
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

        private ObservableCollection<Criteria> _parserSettings = new ObservableCollection<Criteria>
        {
            new Criteria
            {
                Name = "OOC",
                Filter = @"^\(\( \(\d*\) [\p{L}]+ {0,1}([\p{L}]+){0,1}:.*?\)\)$"
            },

            new Criteria
            {
                Name = "IC",
                Filter = @"^(\(Car\) ){0,1}[\p{L}]+ {0,1}([\p{L}]+){0,1} (says|shouts|whispers)( \[low\]){0,1}:.*$",
                Selected = true
            },

            new Criteria
            {
                Name = "Emote",
                Filter = @"^\* [\p{L}]+ {0,1}([\p{L}]+){0,1} .*$",
                Selected = true
            },

            new Criteria
            {
                Name = "Action",
                Filter = @"^\* .* \(\([\p{L}]+ {0,1}([\p{L}]+){0,1}\)\)\*$",
                Selected = true
            },

            new Criteria
            {
                Name = "PM",
                Filter = @"^\(\( PM (to|from) \(\d*\) [\p{L}]+ {0,1}([\p{L}]+){0,1}:.*?\)\)$"
            },

            new Criteria
            {
                Name = "Radio",
                Filter = @"^\*\*\[S: .* CH: .*\] [\p{L}]+ {0,1}([\p{L}]+){0,1}.*$",
                Selected = true
            },

            new Criteria
            {
                Name = "Advertisements",
                Filter = @"^\[.*Advertisement.*\] .*$"
            },

            //new Criteria
            //{
            //    Name = "Timestamps",
            //    Filter = @"^\[\d{1,2}:\d{1,2}:\d{1,2}\] $"
            //},

            new Criteria
            {
                Name = "Other (non listed)",
                Filter = @"Other"
            },
        };

        public ObservableCollection<Criteria> ParserSettings
        {
            get => _parserSettings;
            set { _parserSettings = value; OnPropertyChanged(); }
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

        #endregion

        #region Private Properties



        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        /// <summary>
        /// Get HEX color based on chat line type
        /// </summary>
        private string GetColor(string line)
        {
            if (line.Contains("*") || line.Contains(">"))
            {
                return "#bea3d6";//purple
            }

            if (line.Contains("[low]"))
            {
                return "#a6a4a6";//grey
            }

            if (line.Contains("paid") || line.Contains("gave"))
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

        #endregion
    }
}
