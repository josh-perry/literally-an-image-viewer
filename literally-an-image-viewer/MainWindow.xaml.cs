using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace literally_an_image_viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double aspectRatio = 0.0;
        private bool animatedGif = false;

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
            if (source.ToString().EndsWith(".gif"))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = source;
                image.EndInit();

                ImageBehavior.SetAnimatedSource(ImageControl, image);
                animatedGif = true;

                return;
            }

            ImageControl.Source = new BitmapImage(source);
            animatedGif = false;
        }

        private void ImageControlOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!animatedGif)
            {
                Width = ImageControl.Source.Width;
                Height = ImageControl.Source.Height;
                MinWidth = ImageControl.Source.Width;
                MinHeight = ImageControl.Source.Height;
            }
            else
            {
                var gif = ImageBehavior.GetAnimatedSource(ImageControl);

                Width = gif.Width;
                Height = gif.Height;
                MinWidth = gif.Width;
                MinHeight = gif.Height;
            }
        }

        /// <summary>
        /// Displays the image specified in the source parameter.
        /// </summary>
        /// <param name="source"></param>
        private void LoadImage(string source)
        {
            LoadImage(new Uri(source));
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

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
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

            // Figure out what aspect ratio we're supposed to be locking to based on the loaded image
            aspectRatio = ImageControl.Source.Width / ImageControl.Source.Height;

            // Lock window size
            if (sizeInfo.WidthChanged)
            {
                Width = sizeInfo.NewSize.Height * aspectRatio;
            }
            else
            {
                Height = sizeInfo.NewSize.Width * aspectRatio;
            }
        }
    }
}
