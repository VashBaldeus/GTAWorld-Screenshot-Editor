using ExtensionMethods;

namespace GTAWorld_Screenshot_Editor
{
    public class ResolutionPreset : OnPropertyChange
    {
        private bool _allowEdit;

        public bool AllowEdit
        {
            get => _allowEdit;
            set { _allowEdit = value; OnPropertyChanged(); }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private int _width;

        public int Width
        {
            get => _width;
            set { _width = value; OnPropertyChanged(); }
        }

        private int _height;

        public int Height
        {
            get => _height;
            set { _height = value; OnPropertyChanged(); }
        }
    }
}
