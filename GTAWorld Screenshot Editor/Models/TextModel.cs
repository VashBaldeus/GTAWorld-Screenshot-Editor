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
