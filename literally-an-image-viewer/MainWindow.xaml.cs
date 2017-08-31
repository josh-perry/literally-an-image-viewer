using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace literally_an_image_viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public bool LockedToAspectRatio { get; set; }
        
        private double AspectRatio => ImageControl.Source.Width / ImageControl.Source.Height;
        private bool _animatedGif;

        /// <summary>
        /// Window constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            SetEventHandlers();

            var args = Environment.GetCommandLineArgs();
            
            // If there's a file in the argument list, load that
            if (args.Length > 1)
            {
                LoadImage(args[1]);
            }
            // Otherwise, open a file open dialog
            else
            {
                LoadImageFromDialog();
            }
        }

        /// <summary>
        /// Opens a file open dialog and then loads the selected file.
        /// </summary>
        private void LoadImageFromDialog()
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadImage(dialog.FileName);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Displays the image specified in the source parameter.
        /// </summary>
        /// <param name="source"></param>
        private void LoadImage(Uri source)
        {
            ImageBehavior.SetAnimatedSource(ImageControl, null);

            if (source.ToString().EndsWith(".gif"))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = source;
                image.EndInit();

                ImageBehavior.SetAnimatedSource(ImageControl, image);
                _animatedGif = true;

                return;
            }

            ImageControl.Source = new BitmapImage(source);
            _animatedGif = false;
        }

        /// <summary>
        /// Event handler for when an image is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void ImageControlOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ReinitializeWindow();
            CenterWindow();
            LockWindowSizes();
        }

        /// <summary>
        /// Set up width/height of window.
        /// </summary>
        private void ReinitializeWindow()
        {
            Width = SystemParameters.PrimaryScreenWidth / 3;

            LockHeightToAspectRatio(Width);
        }

        /// <summary>
        /// Set up max widths and heights.
        /// </summary>
        private void LockWindowSizes()
        {
            ImageControl.MaxHeight = SystemParameters.PrimaryScreenHeight;
            ImageControl.MaxWidth = SystemParameters.PrimaryScreenWidth;
            MaxHeight = ImageControl.MaxHeight;
            MaxWidth = ImageControl.MaxWidth;
            MinWidth = 32;
            MinHeight = 32;
        }

        /// <summary>
        /// Center this window on the primary display.
        /// </summary>
        private void CenterWindow()
        {
            // TODO: Change this to work better with multiple screens
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var windowWidth = Width;
            var windowHeight = Height;

            Left = (screenWidth / 2) - (windowWidth / 2);
            Top = (screenHeight / 2) - (windowHeight / 2);
        }

        /// <summary>
        /// Displays the image specified in the source parameter.
        /// </summary>
        /// <param name="source"></param>
        private void LoadImage(string source)
        {
            LoadImage(new Uri(source, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Sets up event handling for the window.
        /// </summary>
        private void SetEventHandlers()
        {
            KeyDown += OnKeyDown;
            MouseDown += OnMouseDown;
            MouseDoubleClick += OnMouseDoubleClick;
            ImageControl.Loaded += ImageControlOnLoaded;
        }

        /// <summary>
        /// Double click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseButtonEventArgs"></param>
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // Toggle maximised state
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

            if (WindowState == WindowState.Maximized)
            {
                ResizeMode = ResizeMode.NoResize;

                WindowState = WindowState.Normal;
                WindowState = WindowState.Maximized;
            }
            else
            {
                ResizeMode = ResizeMode.CanResizeWithGrip;
                WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Event handler for mouse button down events.
        /// Allows the window to be dragged/moved around.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // If any part of the window is clicked with the left mouse button
            if (e.ChangedButton == MouseButton.Left)
            {
                // Let the OS drag and move it around
                DragMove();
            }
        }

        /// <summary>
        /// Event handler for key down events.
        /// Quits the application if escape is hit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // If escape is pressed: quit
            if (e.Key == Key.Escape)
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Event handler for window size change.
        /// Locks the window to the same aspect ratio as the loaded image.
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            // If an image has not been loaded yet, just return
            if (ImageControl.Source == null)
            {
                return;
            }

            // TODO: Fix aspect ratio locking
            if (!LockedToAspectRatio)
            {
                return;
            }

            // Lock window size
            if (sizeInfo.WidthChanged)
            {
                LockWidthToAspectRatio(sizeInfo.NewSize.Height);
            }
            else
            {
                LockHeightToAspectRatio(sizeInfo.NewSize.Width);
            }
        }
        
        /// <summary>
        /// Sets width in accordance with the aspect ratio: based on height.
        /// </summary>
        /// <param name="height">Height to base calculation on</param>
        private void LockWidthToAspectRatio(double height)
        {
            Width = height * AspectRatio;
        }

        /// <summary>
        /// Sets height in accordance with the aspect ratio: based on width.
        /// </summary>
        /// <param name="width">Width to base calculation on</param>
        private void LockHeightToAspectRatio(double width)
        {
            Height = width / AspectRatio;
        }

        #region Commands
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadImageFromDialog();
        }
        #endregion
    }
}
