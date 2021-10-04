using ExtensionMethods;

namespace GTAWorld_Screenshot_Editor.Models
{
    public class TextModel : OnPropertyChange
    {
        private int _width = 1000;

        public int Width
        {
            get => _width;
            set { _width = value; OnPropertyChanged(); }
        }

        private string _fontFamily = "Arial Black";

        public string FontFamily
        {
            get => _fontFamily;
            set { _fontFamily = value; OnPropertyChanged(); }
        }

        private int _fontSize = 20;

        public int FontSize
        {
            get => _fontSize;
            set { _fontSize = value; OnPropertyChanged(); }
        }

        private int _fontWeight = 700;

        public int FontWeight
        {
            get => _fontWeight;
            set { _fontWeight = value; OnPropertyChanged(); }
        }

        private double _strokeThickness = 75;

        public double StrokeThickness
        {
            get => _strokeThickness;
            set { _strokeThickness = value; OnPropertyChanged(); }
        }

        private double _shadowOpacity = 50;

        public double ShadowOpacity
        {
            get => _shadowOpacity;
            set { _shadowOpacity = value; OnPropertyChanged(); }
        }

        private int _canvasScale = 100;

        public int CanvasScale
        {
            get => _canvasScale;
            set { _canvasScale = value; OnPropertyChanged(); }
        }

        private int _parseLines = 100;

        public int ParseLines
        {
            get => _parseLines;
            set { _parseLines = value; OnPropertyChanged(); }
        }
    }
}
