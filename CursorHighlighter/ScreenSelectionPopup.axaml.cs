using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CursorHighlighter
{
    public partial class ScreenSelectionPopup : Window
    {
        public event EventHandler<Screen> ScreenSelected;
        private ListBox screenListBox;
        private List<Screen> screenList;

        public ScreenSelectionPopup()
        {
            InitializeComponent();
            Opened += ScreenSelectionPopup_Opened;
            this.Topmost = true;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            screenListBox = this.FindControl<ListBox>("ScreenListBox");
            screenList = Screens.All.ToList();

            var screenItems = screenList.Select((s, index) =>
                $"Screen {index + 1}: {s.Bounds.Width}x{s.Bounds.Height}").ToList();

            foreach (var item in screenItems)
            {
                screenListBox.Items.Add(item);
            }

            screenListBox.SelectedIndex = 0;

            var selectButton = this.FindControl<Button>("SelectButton");
            selectButton.Click += SelectButton_Click;
        }

        private void ScreenSelectionPopup_Opened(object sender, EventArgs e)
        {
            var screen = Screens.Primary;
            this.Position = new PixelPoint(
                (int)(screen.Bounds.Width - this.Width) / 2 + screen.Bounds.X,
                (int)(screen.Bounds.Height - this.Height) / 2 + screen.Bounds.Y);
        }

        private void SelectButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (screenListBox.SelectedIndex >= 0 && screenListBox.SelectedIndex < screenList.Count)
            {
                var selectedScreen = screenList[screenListBox.SelectedIndex];
                ScreenSelected?.Invoke(this, selectedScreen);
            }
        }
    }
}