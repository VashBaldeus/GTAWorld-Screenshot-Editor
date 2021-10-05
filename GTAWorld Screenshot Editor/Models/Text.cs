using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using ExtensionMethods;

namespace GTAWorld_Screenshot_Editor.Models
{
    public class TextBlockModel : OnPropertyChange
    {
        private double _top;

        public double Top
        {
            get => _top;
            set { _top = value; OnPropertyChanged(); }
        }

        private double _left;

        public double Left
        {
            get => _left;
            set { _left = value; OnPropertyChanged(); }
        }

        private string _blockName = string.Empty;

        public string BlockName
        {
            get => _blockName;
            set { _blockName = value; OnPropertyChanged(); }
        }

        private string _parsedChat = string.Empty;

        public string ParsedChat
        {
            get => _parsedChat;
            set { _parsedChat = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ImageText> _texts = new ObservableCollection<ImageText>();

        public ObservableCollection<ImageText> Texts
        {
            get => _texts;
            set { _texts = value; OnPropertyChanged(); }
        }
    }

    public class ImageText : OnPropertyChange
    {
        private string _text = string.Empty;

        public string String
        {
            get => _text;
            set { _text = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF");

        public SolidColorBrush Foreground
        {
            get => _foreground;
            set { _foreground = value; OnPropertyChanged(); }
        }

        private double _fontSize;

        public double FontSize
        {
            get => _fontSize;
            set { _fontSize = value; OnPropertyChanged(); }
        }

        private FontFamily _fontFamily = new FontFamily("Arial, Helvetica, sans-serif;");

        public FontFamily FontFamily
        {
            get => _fontFamily;
            set { _fontFamily = value; OnPropertyChanged(); }
        }

        private TextWrapping _textWrapping = TextWrapping.WrapWithOverflow;

        public TextWrapping TextWrapping
        {
            get => _textWrapping;
            set { _textWrapping = value; OnPropertyChanged(); }
        }

        private string _fontWeight = "Bold";

        public string FontWeight
        {
            get => _fontWeight;
            set { _fontWeight = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#000");

        public SolidColorBrush Stroke
        {
            get => _stroke;
            set { _stroke = value; OnPropertyChanged(); }
        }

        private double _strokeThickness = 0.25;

        public double StrokeThickness
        {
            get => _strokeThickness;
            set { _strokeThickness = value; OnPropertyChanged(); }
        }

        private DropShadowEffect _effect = new DropShadowEffect
        {
            Color = new Color
            {
                R = byte.MinValue,
                G = byte.MinValue,
                B = byte.MinValue
            },
        };

        public DropShadowEffect Effect
        {
            get => _effect;
            set { _effect = value; OnPropertyChanged(); }
        }
    }
}
