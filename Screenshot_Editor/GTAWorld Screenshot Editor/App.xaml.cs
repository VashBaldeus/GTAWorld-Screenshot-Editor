using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GTAWorld_Screenshot_Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Setup Quick Converter.
            QuickConverter.EquationTokenizer.AddNamespace(typeof(object));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Visibility));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(SolidColorBrush));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(DateTime));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(BrushConverter));
            QuickConverter.EquationTokenizer.AddExtensionMethods(typeof(Enumerable));
        }
    }
}
