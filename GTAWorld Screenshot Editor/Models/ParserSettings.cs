using ExtensionMethods;

namespace GTAWorld_Screenshot_Editor
{
    public class Criteria : OnPropertyChange
    {
        private string _name;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _filter;

        public string Filter
        {
            get => _filter;
            set { _filter = value; OnPropertyChanged(); }
        }

        private bool _selected;

        public bool Selected
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); }
        }
    }
}
