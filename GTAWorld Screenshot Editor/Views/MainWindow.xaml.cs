using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoUpdaterDotNET;
using ExtensionMethods;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using FileMode = System.IO.FileMode;

namespace GTAWorld_Screenshot_Editor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        private string _version;

        public MainWindow()
        {
            InitializeComponent();

            DuplicateProgramInstance.Check(this.Title);

            CheckForUpdate();
        }

        /// <summary>
        /// Runs a check against GitHub releases
        /// checking current program version agianst release
        /// </summary>
        private void CheckForUpdate()
        {
            try
            {
                AutoUpdater.AppTitle = this.Title;

                AutoUpdater.Synchronous = true;

                AutoUpdater.Mandatory = true;

                AutoUpdater.UpdateMode = Mode.Forced;

                AutoUpdater.ShowSkipButton = false;

                AutoUpdater.ShowRemindLaterButton = false;

                AutoUpdater.DownloadPath = Environment.CurrentDirectory;

                AutoUpdater.ApplicationExitEvent += delegate
                {
                    Application.Current.Shutdown();
                };

                var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

                if (currentDirectory.Parent != null)
                {
                    AutoUpdater.InstallationPath = currentDirectory.FullName;

                    // ReSharper disable once LocalizableElement
                    Debug.WriteLine($"{currentDirectory.FullName}");
                }

                AutoUpdater.ReportErrors = true;

                AutoUpdater.CheckForUpdateEvent += delegate (UpdateInfoEventArgs args)
                {
                    if (!(args is { IsUpdateAvailable: true })) return;

                    try
                    {
                        if (File.Exists(@"parser.cfg"))
                            File.Delete(@"parser.cfg");

                        if (AutoUpdater.DownloadUpdate(args))
                        {
                            this.Close();
                        }
                    }
                    catch (Exception exception)
                    {
                        Message.Log(exception);
                    }
                };

                AutoUpdater.Start(@"http://screenshot.vashbaldeus.pw/updates.xml");
            }
            catch (Exception ex)
            {
                Message.Log(ex);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dc = (MainViewModel)DataContext;

            if (dc == null)
                return;

            var ver = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');

            _version = $"{ver[0]}.{ver[1]}.{ver[2]}";

            this.Title = $"{this.Title} - version {_version}";

            dc.Canvas = ScreenshotCanvas;

            dc.OnLoadCommand.Execute(null);

            dc.NamesList = NamesToRemoveList;
        }

        private void ScreenshotCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                && !string.IsNullOrEmpty(dc.SelectedImage.Path))
            {
                var point = Mouse.GetPosition(ScreenshotCanvas);

                if (dc.SelectedResolution.Width < point.X || dc.SelectedResolution.Height < point.Y)
                    return;

                dc.SelectedBlock.Margin = new Thickness(point.X, point.Y, 0, 0);
            }
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            var dc = (dynamic)DataContext;

            if (dc == null)
                return;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            // Note that you can have more than one file.
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files == null)
                return;

            if (files.Length > 1)
                return;

            // Assuming you have one file that you care about, pass it off to whatever
            // handling code you have defined.
            dc.DragDropCommand.Execute(files[0]);
        }

        private void ChatFilterExpander_OnExpanded(object sender, RoutedEventArgs e)
        {
        }

        private void SaveLocally_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //get source button name
                var control = (e.Source as Button)?.Name;

                //picture canvas
                var source = ScreenshotCanvas;
            
                //variables for saving size
                double renderHeight, renderWidth;

                //assign sizes
                var height = renderHeight = source.RenderSize.Height;
                var width = renderWidth = source.RenderSize.Width;

                //Specification for target bitmap like width/height pixel etc.
                var renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);

                //creates Visual Brush of UIElement
                var visualBrush = new VisualBrush(source);

                var drawingVisual = new DrawingVisual();

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    //draws image of element
                    drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), new Point(width, height)));
                }

                //renders image
                renderTarget.Render(drawingVisual);

                //PNG encoder for creating PNG file
                var encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                //copy image to clipboard
                if (control == "CopyClipboard")
                {
                    Clipboard.SetImage(renderTarget);
                }
                else
                {
                    //open dialog where to save image
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

                    //save file locally
                    using var stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write);

                    encoder.Save(stream);
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

            if (value > 1 || ScreenshotCanvas == null)
                return;

            ScreenshotCanvas.RenderTransform = new ScaleTransform(value, value); // transform Canvas size
        }

        private void ScreenCacheListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ScreenCacheListView.SelectedItem != null)
            {
                MainTabControl.SelectedIndex = 0;
            }
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            
            var dirPath = @"temp";

            if (!Directory.Exists(dirPath))
                return;

            //delete temp file folder
            var dir = new DirectoryInfo(dirPath);
            dir.Delete(true);
        }
    }
}
