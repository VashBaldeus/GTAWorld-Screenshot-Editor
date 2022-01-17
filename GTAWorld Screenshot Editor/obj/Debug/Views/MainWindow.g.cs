﻿#pragma checksum "..\..\..\Views\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F2849A0D4F8A8F529403A336B77BA3676A739E3E6FFAD43107A1BD4C3D5A5494"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GTAWorld_Screenshot_Editor;
using GTAWorld_Screenshot_Editor.Converters;
using GTAWorld_Screenshot_Editor.Views;
using MahApps.Metro;
using MahApps.Metro.Accessibility;
using MahApps.Metro.Actions;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.Behaviors;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Converters;
using MahApps.Metro.IconPacks;
using MahApps.Metro.IconPacks.Converter;
using MahApps.Metro.Markup;
using MahApps.Metro.Theming;
using MahApps.Metro.ValueBoxes;
using QuickConverter;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Converters;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Mag.Converters;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Themes;
using Xceed.Wpf.Toolkit.Zoombox;


namespace GTAWorld_Screenshot_Editor.Views {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 38 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl MainTabControl;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabItem EditorHeader;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander InstructionsExpander;
        
        #line default
        #line hidden
        
        
        #line 195 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MahApps.Metro.Controls.NumericUpDown ImageWidth;
        
        #line default
        #line hidden
        
        
        #line 206 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MahApps.Metro.Controls.NumericUpDown ImageHeight;
        
        #line default
        #line hidden
        
        
        #line 261 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.ColorPicker HexColor;
        
        #line default
        #line hidden
        
        
        #line 343 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddNameToRemove;
        
        #line default
        #line hidden
        
        
        #line 352 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView NamesToRemoveList;
        
        #line default
        #line hidden
        
        
        #line 363 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.RichTextBox ParsedChatSmall;
        
        #line default
        #line hidden
        
        
        #line 396 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider CanvasZoom;
        
        #line default
        #line hidden
        
        
        #line 399 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton CropImageToggle;
        
        #line default
        #line hidden
        
        
        #line 442 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SaveLocally;
        
        #line default
        #line hidden
        
        
        #line 446 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CopyClipboard;
        
        #line default
        #line hidden
        
        
        #line 456 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ScreenshotCanvas;
        
        #line default
        #line hidden
        
        
        #line 462 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image ScreenshotImage;
        
        #line default
        #line hidden
        
        
        #line 466 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl ScreenshotTextControl;
        
        #line default
        #line hidden
        
        
        #line 556 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.ColorPicker HexColor2;
        
        #line default
        #line hidden
        
        
        #line 597 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.RichTextBox ParsedChatBig;
        
        #line default
        #line hidden
        
        
        #line 628 "..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView ScreenCacheListView;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GTAWorld Screenshot Editor;component/views/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 15 "..\..\..\Views\MainWindow.xaml"
            ((GTAWorld_Screenshot_Editor.Views.MainWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.MainWindow_OnLoaded);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\Views\MainWindow.xaml"
            ((GTAWorld_Screenshot_Editor.Views.MainWindow)(target)).Drop += new System.Windows.DragEventHandler(this.MainWindow_OnDrop);
            
            #line default
            #line hidden
            
            #line 20 "..\..\..\Views\MainWindow.xaml"
            ((GTAWorld_Screenshot_Editor.Views.MainWindow)(target)).Closed += new System.EventHandler(this.MainWindow_OnClosed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MainTabControl = ((System.Windows.Controls.TabControl)(target));
            return;
            case 3:
            this.EditorHeader = ((System.Windows.Controls.TabItem)(target));
            return;
            case 4:
            this.InstructionsExpander = ((System.Windows.Controls.Expander)(target));
            
            #line 80 "..\..\..\Views\MainWindow.xaml"
            this.InstructionsExpander.Expanded += new System.Windows.RoutedEventHandler(this.ChatFilterExpander_OnExpanded);
            
            #line default
            #line hidden
            
            #line 80 "..\..\..\Views\MainWindow.xaml"
            this.InstructionsExpander.Collapsed += new System.Windows.RoutedEventHandler(this.ChatFilterExpander_OnExpanded);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ImageWidth = ((MahApps.Metro.Controls.NumericUpDown)(target));
            return;
            case 6:
            this.ImageHeight = ((MahApps.Metro.Controls.NumericUpDown)(target));
            return;
            case 7:
            this.HexColor = ((Xceed.Wpf.Toolkit.ColorPicker)(target));
            return;
            case 8:
            this.AddNameToRemove = ((System.Windows.Controls.Button)(target));
            return;
            case 9:
            this.NamesToRemoveList = ((System.Windows.Controls.ListView)(target));
            return;
            case 10:
            this.ParsedChatSmall = ((Xceed.Wpf.Toolkit.RichTextBox)(target));
            return;
            case 11:
            this.CanvasZoom = ((System.Windows.Controls.Slider)(target));
            
            #line 397 "..\..\..\Views\MainWindow.xaml"
            this.CanvasZoom.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.CanvasZoom_OnValueChanged);
            
            #line default
            #line hidden
            return;
            case 12:
            this.CropImageToggle = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 13:
            this.SaveLocally = ((System.Windows.Controls.Button)(target));
            
            #line 442 "..\..\..\Views\MainWindow.xaml"
            this.SaveLocally.Click += new System.Windows.RoutedEventHandler(this.SaveLocally_OnClick);
            
            #line default
            #line hidden
            return;
            case 14:
            this.CopyClipboard = ((System.Windows.Controls.Button)(target));
            
            #line 446 "..\..\..\Views\MainWindow.xaml"
            this.CopyClipboard.Click += new System.Windows.RoutedEventHandler(this.SaveLocally_OnClick);
            
            #line default
            #line hidden
            return;
            case 15:
            this.ScreenshotCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 457 "..\..\..\Views\MainWindow.xaml"
            this.ScreenshotCanvas.MouseMove += new System.Windows.Input.MouseEventHandler(this.ScreenshotCanvas_OnMouseMove);
            
            #line default
            #line hidden
            return;
            case 16:
            this.ScreenshotImage = ((System.Windows.Controls.Image)(target));
            return;
            case 17:
            this.ScreenshotTextControl = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 18:
            this.HexColor2 = ((Xceed.Wpf.Toolkit.ColorPicker)(target));
            return;
            case 19:
            this.ParsedChatBig = ((Xceed.Wpf.Toolkit.RichTextBox)(target));
            return;
            case 20:
            this.ScreenCacheListView = ((System.Windows.Controls.ListView)(target));
            
            #line 629 "..\..\..\Views\MainWindow.xaml"
            this.ScreenCacheListView.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ScreenCacheListView_OnMouseDoubleClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
