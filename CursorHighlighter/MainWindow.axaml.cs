using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.Linq;
using Avalonia.Platform;
using Avalonia.Media;

namespace CursorHighlighter
{
    public partial class MainWindow : Window
    {
        private Panel mainPanel;
        private Image cursorImage;
        private DispatcherTimer timer;
        private double angle = 0;
        private bool isShowing = false;
        private double opacity = 0;
        private Screen currentScreen;
        private ScreenSelectionPopup screenSelectionPopup;

        public MainWindow()
        {
            InitializeComponent();
            InitializeImage();
            InitializeKeyHandler();
            ShowScreenSelectionPopup();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            mainPanel = this.FindControl<Panel>("MainPanel");

            this.PointerMoved += MainWindow_PointerMoved;
        }

        private void InitializeImage()
        {
            // Replace "path/to/your/image.png" with the actual path to your PNG file
            var bitmap = new Bitmap("cursor.png");
            cursorImage = new Image
            {
                Source = bitmap,
                Width = 100,  // Set your custom width
                Height = 100, // Set your custom height
                IsVisible = false,
                Opacity = 0
            };

            mainPanel.Children.Add(cursorImage);

            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            timer.Tick += Timer_Tick;
        }

        private void InitializeKeyHandler()
        {
            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
        }

        private void SetupWindowForScreen(Screen screen)
        {
            currentScreen = screen;
            this.WindowState = WindowState.Normal; // Reset window state
            this.Position = screen.Bounds.TopLeft;
            this.Width = screen.Bounds.Width;
            this.Height = screen.Bounds.Height;

            // Make the window click-through
            this.ExtendClientAreaToDecorationsHint = true;
            this.ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
            this.ExtendClientAreaTitleBarHeightHint = -1;

            // Set up the main panel to cover the entire window
            mainPanel.Width = screen.Bounds.Width;
            mainPanel.Height = screen.Bounds.Height;

            // Make the window topmost after setting up
            // this.Topmost = true;
            // this.IsHitTestVisible = true;
        }

        private void ShowScreenSelectionPopup()
        {
            // Ensure the main window is not interfering
            this.Topmost = false;
            this.IsHitTestVisible = false;

            screenSelectionPopup = new ScreenSelectionPopup();
            screenSelectionPopup.ScreenSelected += (sender, screen) =>
            {
                SetupWindowForScreen(screen);
                screenSelectionPopup.Close();
            };
            screenSelectionPopup.Closed += (sender, args) =>
            {
                // Re-enable the main window
                this.Topmost = true;
                this.IsHitTestVisible = true;
                screenSelectionPopup = null;
            };
            screenSelectionPopup.Show();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                ShowImage();
            }
            else if (e.Key == Key.N && e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                if (screenSelectionPopup == null)
                {
                    ShowScreenSelectionPopup();
                }
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A || e.KeyModifiers == KeyModifiers.None)
            {
                HideImage();
            }
        }

        private void ShowImage()
        {
            cursorImage.IsVisible = true;
            isShowing = true;
            timer.Start();
        }

        private void HideImage()
        {
            isShowing = false;
        }

        private void MainWindow_PointerMoved(object sender, PointerEventArgs e)
        {
            var position = e.GetPosition(mainPanel);
            Canvas.SetLeft(cursorImage, position.X - cursorImage.Width / 2);
            Canvas.SetTop(cursorImage, position.Y - cursorImage.Height / 2);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            angle += 5;
            if (angle >= 360) angle = 0;

            var rotateTransform = new RotateTransform(angle);
            cursorImage.RenderTransform = rotateTransform;

            if (isShowing && opacity < 1)
            {
                opacity += 0.05;
                cursorImage.Opacity = opacity;
            }
            else if (!isShowing && opacity > 0)
            {
                opacity -= 0.05;
                cursorImage.Opacity = opacity;
                if (opacity <= 0)
                {
                    cursorImage.IsVisible = false;
                    timer.Stop();
                }
            }
        }
    }
}