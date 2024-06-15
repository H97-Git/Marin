using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace WaifuGallery;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // var splash = new SplashScreenWindow();
            //
            // desktop.MainWindow = splash;
            // splash.Show();
            //
            // await splash.InitApp();

            var main = new MainWindow();
            desktop.MainWindow = main;
            main.Show();

            // splash.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void SetTheme(string theme)
    {
        if (Current is null) return;
        if (theme.Equals("Light", StringComparison.OrdinalIgnoreCase))
        {
            Current.RequestedThemeVariant = ThemeVariant.Light;
        }
        else if (theme.Equals("Dark", StringComparison.OrdinalIgnoreCase))
        {
            Current.RequestedThemeVariant = ThemeVariant.Dark;
        }
        else
        {
            Current.RequestedThemeVariant = ThemeVariant.Default;
        }
    }
}