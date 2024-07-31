using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace CursorHighlighter
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow{
                    TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent },
                    Background = null,
                    SystemDecorations = SystemDecorations.None,
                    ShowInTaskbar = false,
                    // Topmost = true
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}