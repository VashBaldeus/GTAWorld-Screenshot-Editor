using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ControlzEx.Theming;
using ExtensionMethods;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace Screenshot_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            InputTxtBox.Text = string.Empty;
            OpacityPercentage.Value = 0;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            dc.OnLoadCommand.Execute(null);

            
        }

        private void CmbAccent_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            ThemeManager.Current.ChangeTheme(this, $"Dark.{dc.SelectedAccent}");
        }

        private void ConvertTxtBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InputTxtBox.Text))
                return;

            TxtBlock.Inlines.Clear();

            InputTxtBox.Text = InputTxtBox.Text.Replace("[!] ", "");

            var lines = InputTxtBox.Text.Split(new[]{ "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()
                .Where(w => !(w.Contains("((") && w.Contains("))")));

            var lineCount = 0;

            foreach (var line in lines)
            {
                var str = line;

                if (!line.EndsWith("."))
                    str = $"{line}.";

                var newLine = lineCount + 1 == lines.Count() ? "" : "\r\n";

                var run = new Run
                {
                    //FontFamily = new FontFamily($"{FontCombo.SelectedValue}"),
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(GetColor(line)),
                    Text = $"{(line.EndsWith(".") ? line : str)}{newLine}"
                };

                TxtBlock.Inlines.Add(run);

                lineCount++;
            }
        }

        private string GetColor(string line)
        {
            if (line.Contains("*") || line.Contains(">"))
            {
                return "#bea3d6";
            }

            if (line.Contains("[low]"))
            {
                return "#a6a4a6";
            }

            if (line.Contains("paid") || line.Contains("gave"))
            {
                return "#29943e";
            }

            if (line.Contains("whispers"))
            {
                return "#eda841";
            }

            return "#fff";
        }

        private void ScreenShotCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                var point = Mouse.GetPosition(ScreenShotCanvas);

                //TxtBlock.SetValue(Canvas.LeftProperty, point.X);
                //TxtBlock.SetValue(Canvas.TopProperty, point.Y);

                TxtBlockBorder.SetValue(Canvas.LeftProperty, point.X);
                TxtBlockBorder.SetValue(Canvas.TopProperty, point.Y);
            }
        }

        /// <summary>
        /// Copies a UI element to the clipboard as an image.
        /// </summary>
        /// <param name="element">The element to copy.</param>
        // ReSharper disable once InconsistentNaming
        public static void CopyUIElementToClipboard(FrameworkElement element)
        {
            var width = element.ActualWidth;

            var height = element.ActualHeight;

            var bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);

            var dv = new DrawingVisual();

            using (var dc = dv.RenderOpen())
            {
                var vb = new VisualBrush(element);

                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }

            bmpCopied.Render(dv);

            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpCopied));

            var saveDialog = new SaveFileDialog()
            {
                Title = "Select Where to save screenshot:",
                FileName = $"screenshot_{DateTime.Now:yyyyMMdd_hhmm}"
            };

            saveDialog.ImageTypes();

            if (saveDialog.ShowDialog() == false)
                return;

            if (string.IsNullOrEmpty(saveDialog.FileName))
                return;

            var fs = File.Open(saveDialog.FileName, FileMode.Create);

            encoder.Save(fs);

            fs.Close();
        }

        private void SaveImageBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CopyUIElementToClipboard(ScreenShotCanvas);
        }

        private void SelectImageBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            var openFile = new OpenFileDialog
            {
                Title = "Select image file",
                Multiselect = false
            };

            openFile.ImageTypes();

            if (openFile.ShowDialog() == false)
                return;

            dc.ImageFile = openFile.FileName;
        }
    }
}
