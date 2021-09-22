using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using ExtensionMethods;

namespace GTAWorld_Screenshot_Editor.Models
{
    public class Text : OnPropertyChange
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

        private int _fontWeight = 900;

        public int FontWeight
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

        private double _strokeThickness = 0.5;

        public double StrokeThickness
        {
            get => _strokeThickness;
            set { _strokeThickness = value; OnPropertyChanged(); }
        }

        private DropShadowEffect _effect = new DropShadowEffect()
        {
            Color = new Color
            {
                R = Byte.MinValue,
                G = Byte.MinValue,
                B = Byte.MinValue
            },
        };

        public DropShadowEffect Effect
        {
            get => _effect;
            set { _effect = value; OnPropertyChanged(); }
        }
    }
}
