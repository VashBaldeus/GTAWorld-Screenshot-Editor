using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ControlzEx.Theming;
using ExtensionMethods;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Screenshot_Editor.Components;

#pragma warning disable 618

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
            //OpacityPercentage.Value = 0;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            dc.OnLoadCommand.Execute(null);

            FontWeightCombo.SelectedIndex = 6;
            TextStrokeWidth.Value = 0.75;
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
            var dc = (MainViewModel)DataContext;

            if (dc == null)
                return;

            if (string.IsNullOrEmpty(InputTxtBox.Text))
                return;

            //TxtBlock.Inlines.Clear();
            TxtBlock.Children.Clear();

            InputTxtBox.Text = InputTxtBox.Text.Replace("[!] ", "");

            var lines = InputTxtBox.Text.Split(new[]{ "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()
                .Where(w => !(w.Contains("((") && w.Contains("))")));

            var lineCount = 0;

            var formattedText = new FormattedText(
                "testString",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                32,
                Brushes.Black);

            //var textGeometry = formattedText.BuildGeometry(new Point(1, 1));
            //ScreenShotCanvas.DrawText(formattedText, new Point(0, 0));
            //drawingContext.DrawGeometry(null, new Pen((SolidColorBrush)new BrushConverter().ConvertFrom(GetColor("*")), 2), textGeometry);

            var shadowColor = new Color();

            shadowColor.B = Byte.MinValue;
            shadowColor.R = Byte.MinValue;
            shadowColor.G = Byte.MinValue;

            foreach (var line in lines)
            {
                var str = line;

                if (!line.EndsWith(".") && line.EndsWith("?") && line.EndsWith("!") && line.EndsWith("!?") && line.EndsWith("?!"))
                    str = $"{line}.";

                var newLine = lineCount + 1 == lines.Count() ? "" : "\r\n";

                var _outlinedTextBlock = new OutlinedTextBlock()
                {
                    Text = $"{(line.EndsWith(".") ? line : str)}{newLine}",
                    Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(GetColor(line)),
                    Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#000"),
                    StrokeThickness = Convert.ToDouble(TextStrokeWidth.Value),
                    FontSize = Convert.ToDouble(FontSizeUpDown.Value),
                    FontFamily = new FontFamily(dc.SelectedFont),
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    FontWeight = FontWeight.FromOpenTypeWeight(Convert.ToInt32(FontWeightCombo.SelectedValue)),
                    //Effect = new DropShadowEffect()
                    //{
                    //    //BlurRadius = 1,
                    //    Color = shadowColor,
                    //    //ShadowDepth = 1,
                    //    //Direction = 1,
                    //    Opacity = 1
                    //}
                };

                TxtBlock.Children.Add(_outlinedTextBlock);

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

                TxtBlock.SetValue(Canvas.LeftProperty, point.X);
                TxtBlock.SetValue(Canvas.TopProperty, point.Y);
            }
        }

        /// <summary>
        /// Copies a UI element to the clipboard as an image.
        /// </summary>
        /// <param name="element">The element to copy.</param>
        // ReSharper disable once InconsistentNaming
        public void CopyUIElementToClipboard(FrameworkElement element)
        {
            RenderTargetBitmap renderTargetBitmap =
                new RenderTargetBitmap((int)Math.Abs(element.ActualWidth), (int)Math.Abs(element.ActualHeight), 96, 96, PixelFormats.Pbgra32);

            renderTargetBitmap.Render(element);

            PngBitmapEncoder pngImage = new PngBitmapEncoder();

            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            var saveDialog = new SaveFileDialog
            {
                Title = "Select Where to save screenshot:",
                FileName = $"screenshot_{DateTime.Now:yyyyMMdd_hhmmss}",
                Filter = "png (*.png) | *.png;"
            };

            if (saveDialog.ShowDialog() == false)
                return;

            if (string.IsNullOrEmpty(saveDialog.FileName))
                return;

            //using (Stream fileStream = File.Create(saveDialog.FileName))
            //{
            //    pngImage.Save(fileStream);
            //}

            //var dc2 = (MainViewModel)DataContext;

            //if (dc2 == null)
            //    return;
            ////var width = element.ActualWidth;

            ////var height = element.ActualHeight;

            //var width = dc2.XY.X;

            //var height = dc2.XY.Y;

            //var bmpCopied = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Default);

            //var dv = new DrawingVisual();

            //using (var dc = dv.RenderOpen())
            //{
            //    var vb = new VisualBrush(element);

            //    dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            //}

            //bmpCopied.Render(dv);

            //BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bmpCopied));

            //var saveDialog = new SaveFileDialog()
            //{
            //    Title = "Select Where to save screenshot:",
            //    FileName = $"screenshot_{DateTime.Now:yyyyMMdd_hhmmss}",
            //    Filter = "png (*.png) | *.png;"
            //};

            ////saveDialog.ImageTypes();
            ////$"Image files ({imageFileTypes.Aggregate(string.Empty, (str, i) => str + $"*.{i},")}) | {imageFileTypes.Aggregate(string.Empty, (str, i) => str + $"*.{i};")}";

            //if (saveDialog.ShowDialog() == false)
            //    return;

            //if (string.IsNullOrEmpty(saveDialog.FileName))
            //    return;

            //var fs = File.Open(saveDialog.FileName, FileMode.Create);

            //encoder.Save(fs);

            //fs.Close();
        }

        private void SaveImageBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Select Where to save screenshot:",
                FileName = $"screenshot_{DateTime.Now:yyyyMMdd_hhmmss}",
                Filter = "png (*.png) | *.png;"
            };

            if (saveDialog.ShowDialog() == false)
                return;

            if (string.IsNullOrEmpty(saveDialog.FileName))
                return;

            //CopyUIElementToClipboard(ScreenShotCanvas);
            CaptureScreen(ScreenShotCanvas, saveDialog.FileName);
        }

        private void SelectImageBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var dc = (MainViewModel)DataContext;

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

            dc.Image = new BitmapImage(new Uri(openFile.FileName));

            ResizeImage();
        }

        private void CanvasSizeCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = (MainViewModel)DataContext;

            if (dc == null)
                return;

            TextBlockWidthNumber.Value = dc.XY.X;

            ResizeImage();
        }

        private void ResizeImage()
        {
            var dc = (MainViewModel)DataContext;

            if (dc == null || string.IsNullOrEmpty(dc.ImageFile))
                return;

            //dc.Image.BeginInit();
            dc.Image.UriSource = new Uri(dc.ImageFile);
            dc.Image.DecodePixelHeight = dc.XY.Y;
            dc.Image.DecodePixelWidth = dc.XY.X;
            //dc.Image.EndInit();
        }

        private void NumericUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            var dc = (MainViewModel)DataContext;

            if (dc == null || !WidthRes.IsEnabled && !HeightRes.IsEnabled)
                return;

            TextBlockWidthNumber.Value = dc.XY.X;

            ResizeImage();
        }

        public void CaptureScreen(UIElement source, string filePath)
        {
            try
            {
                double Height, renderHeight, Width, renderWidth;

                Height = renderHeight = source.RenderSize.Height;
                Width = renderWidth = source.RenderSize.Width;

                //Specification for target bitmap like width/height pixel etc.
                RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
                //creates Visual Brush of UIElement
                VisualBrush visualBrush = new VisualBrush(source);

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    //draws image of element
                    drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), new Point(Width, Height)));
                }
                //renders image
                renderTarget.Render(drawingVisual);

                //PNG encoder for creating PNG file
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
