using System;
using System.Windows.Data;
using System.Windows.Media;

namespace GTAWorld_Screenshot_Editor.Converters
{
    public class BooleanToSolidColorBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
                return Brushes.Transparent;

            var flag = (bool)value;

            if (!flag) return Brushes.Transparent;

            var brush = (SolidColorBrush)new BrushConverter().ConvertFrom("#000");

            if (brush == null) return Brushes.Transparent;

            brush.Opacity = 0.5;

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Brushes.Transparent;
        }
    }
}
