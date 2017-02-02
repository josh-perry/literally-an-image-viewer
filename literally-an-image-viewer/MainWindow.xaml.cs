using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace literally_an_image_viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Window constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            SetEventHandlers();

            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                LoadImage(args[1]);
            }
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
        }

        /// <summary>
        /// Displays the image specified in the source parameter.
        /// </summary>
        /// <param name="source"></param>
        private void LoadImage(Uri source)
        {
            ImageControl.Source = new BitmapImage(source);
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
    }
}
