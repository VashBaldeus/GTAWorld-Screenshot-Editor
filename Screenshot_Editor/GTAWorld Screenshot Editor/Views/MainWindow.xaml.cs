using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExtensionMethods;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Octokit;
using FileMode = System.IO.FileMode;

namespace GTAWorld_Screenshot_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private GitHubClient _client;
        private string ProductHeader = "GTAWorld-Screenshot-Editor";
        private string _Version;

        // Zoom
        private double _zoomMax = 1;
        private double _zoomMin = 0.1;
        private double _zoomSpeed = 0.001;
        private double _zoom = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            var ver = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            _Version = $"{ver[0]}.{ver[1]}.{ver[2]}";
            this.Title = $"{this.Title} - version {_Version}";

            dc.OnLoadCommand.Execute(null);

            _client = new GitHubClient(new ProductHeaderValue(ProductHeader));
            _client.SetRequestTimeout(new TimeSpan(0, 0, 0, 4));

            //TryCheckingForUpdates();

            CheckForUpdates();
        }

        private void ScreenshotCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && !string.IsNullOrEmpty(dc.SelectedImage.Path))
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

        private void CanvasZoom_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = ((Slider)e.Source).Value / 100;

            if (value > 1 || ScreenshotCanvas == null || value == null)
                return;

            ScreenshotCanvas.RenderTransform = new ScaleTransform(value, value); // transform Canvas size
        }

        /// <summary>
        /// Checks for updates
        /// </summary>
        /// <param name="manual"></param>
#pragma warning disable 162
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        [SuppressMessage("ReSharper", "UnreachableCode")]
        private void CheckForUpdates(bool manual = false)
        {
            
            string installedVersion = _Version;

            try
            {
                IReadOnlyList<Release> releases = _client.Repository.Release.GetAll("VashBaldeus", ProductHeader).Result;

                string newVersion = string.Empty;
                bool isNewVersionBeta = false;

                // Prereleases are a go
                if (false)
                {
                    newVersion = releases[0].TagName;
                    isNewVersionBeta = releases[0].Prerelease;
                }
                else
                {
                    // If the user does not want to
                    // look for prereleases during
                    // the update check, ignore them
                    foreach (Release release in releases)
                    {
                        if (release.Prerelease)
                            continue;

                        newVersion = release.TagName;
                        isNewVersionBeta = release.Prerelease;
                        break;
                    }
                }

                if (!isNewVersionBeta && installedVersion != newVersion || installedVersion != newVersion)
                { // Update available
                    //if (Visibility != Visibility.Visible)
                    //    ResumeTrayStripMenuItem_Click(this, EventArgs.Empty);

                    var update =
                        $"A new version of the chat log parser is now available on GitHub.\n\nInstalled Version: {installedVersion}\nAvailable Version: {newVersion}\n\nWould you like to visit the releases page now?";

                    DisplayUpdateMessage(update, "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                }
                //else if (manual) // Latest version
                //    DisplayUpdateMessage($"You are running the latest version of the chat log parser.\n\nInstalled Version: {installedVersion}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch // No internet
            {
                if (manual)
                    DisplayUpdateMessage($"No updates could be found.\nTry checking your internet connection or increasing the update check timeout in the settings window.\n\nInstalled Version: {installedVersion}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
#pragma warning restore 162

        /// <summary>
        /// Displays a message box
        /// on the main UI thread
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <param name="image"></param>
        private void DisplayUpdateMessage(string text, string title, MessageBoxButton buttons, MessageBoxImage image)
        {
            Dispatcher?.Invoke(() =>
            {
                if (MessageBox.Show(text, title, buttons, image) == MessageBoxResult.Yes)
                    Process.Start(@"https://github.com/VashBaldeus/GTAWorld-Screenshot-Editor/releases");
            });
        }

        ///// <summary>
        ///// Disables the controls on the main window
        ///// and checks for updates
        ///// </summary>
        //private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        //private void TryCheckingForUpdates(bool manual = false)
        //{
        //    if (!manual)
        //    {
        //        _resetEvent.Reset();

        //        ThreadPool.QueueUserWorkItem(_ => CheckForUpdates(ref manual));
        //    }
        //}
        private void ScreenCacheListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ScreenCacheListView.SelectedItem != null)
            {
                MainTabControl.SelectedIndex = 0;
            }
        }
    }
}
