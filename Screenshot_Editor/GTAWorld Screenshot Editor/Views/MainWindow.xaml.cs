using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExtensionMethods;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace GTAWorld_Screenshot_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            dc.OnLoadCommand.Execute(null);
        }

        private void ScreenshotCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                var point = Mouse.GetPosition(ScreenshotCanvas);

                ScreenshotTextControl.SetValue(Canvas.LeftProperty, point.X);
                ScreenshotTextControl.SetValue(Canvas.TopProperty, point.Y);
            }
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 1)
                    return;

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                dc.DragDropCommand.Execute(files[0]);
            }
        }

        private void ChatFilterExpander_OnExpanded(object sender, RoutedEventArgs e)
        {
            ControlsScrollViewer.VerticalScrollBarVisibility = ChatFilterExpander.IsExpanded
                ? ScrollBarVisibility.Auto
                : ScrollBarVisibility.Disabled;
        }

        private void SaveLocally_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var control = (e.Source as Button).Name;

                var source = ScreenshotCanvas;
            
                double Height, renderHeight, Width, renderWidth;

                Height = renderHeight = source.RenderSize.Height;
                Width = renderWidth = source.RenderSize.Width;

                //Specification for target bitmap like width/height pixel etc.
                var renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);

                //creates Visual Brush of UIElement
                var visualBrush = new VisualBrush(source);

                var drawingVisual = new DrawingVisual();

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    //draws image of element
                    drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), new Point(Width, Height)));
                }

                //renders image
                renderTarget.Render(drawingVisual);

                //PNG encoder for creating PNG file
                var encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                if (control == "CopyClipboard")
                {
                    Clipboard.SetImage(renderTarget);
                }
                else
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

                    using (var stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write))
                    {
                        encoder.Save(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        /// <summary>
        /// Capture screen based on UIElement
        /// </summary>
        /// <param name="source">UIElement</param>
        /// <param name="filePath">Filepath</param>
        public void CaptureScreen(UIElement source, object sender)
        {
            
        }
    }
}
