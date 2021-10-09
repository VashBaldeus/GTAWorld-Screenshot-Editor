using System;
using System.Linq;
using ExtensionMethods;

namespace GTAWorld_Screenshot_Editor.Models
{
    public class NamesToReplace : OnPropertyChange
    {
        public NamesToReplace()
        {
            Mask = $"Mask_{RandomString(5)}_{random.Next(10,99)}";
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(value) && value.Contains(' '))
                {
                    FirstName = value.Split(' ')[0];
                    LastName = value.Split(' ')[1];
                }
            }
        }
        
        private string _firstName;

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        private string _lastName;

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        private string _mask;

        public string Mask
        {
            get => _mask;
            set { _mask = value; OnPropertyChanged(); }
        }

        private Random random = new Random();

        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private string RandomString(int length)
        {
            return new string(Enumerable.Repeat(Chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
