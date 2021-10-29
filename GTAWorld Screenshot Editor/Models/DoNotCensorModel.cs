using System.Collections.ObjectModel;
using ExtensionMethods;

namespace GTAWorld_Screenshot_Editor.Models
{
    public class ParserSettingsModel : OnPropertyChange
    {
        private DoNotCensor _doNotCensor = new DoNotCensor();

        public DoNotCensor DoNotCensor
        {
            get => _doNotCensor;
            set { _doNotCensor = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Criteria> _filters = new ObservableCollection<Criteria>();

        public ObservableCollection<Criteria> Filters
        {
            get => _filters;
            set { _filters = value; OnPropertyChanged(); }
        }
    }

    public class DoNotCensor : OnPropertyChange
    {
        private bool _money = true;

        public bool Money
        {
            get => _money;
            set { _money = value; OnPropertyChanged(); }
        }

        private bool _items = true;

        public bool Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(); }
        }
    }
}
