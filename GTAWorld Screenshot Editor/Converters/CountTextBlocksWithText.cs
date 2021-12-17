using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using GTAWorld_Screenshot_Editor.Models;
using System.IO;
using System.Linq;

namespace GTAWorld_Screenshot_Editor.Converters
{
    public class CountTextBlocksWithText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var textBlocks = value as ObservableCollection<TextBlockModel>;

            if (value == null || textBlocks == null)
                return false;

            return textBlocks.Count(c => !string.IsNullOrEmpty(c.ParsedChat)) > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}
