using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using WaifuGallery.ViewModels;

namespace WaifuGallery;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        SetTheme(Preferences.Instance.Theme);
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
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            desktop.ShutdownRequested += (_, _) => { Preferences.Instance.Save(); };
            main.Show();

            // splash.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void SetTheme(string theme)
    {
        if (theme is null) return;
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

    public static TopLevel? GetTopLevel()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }

    public static void Close()
    {
        Preferences.Instance.Save();
        Environment.Exit(0);
    }
}