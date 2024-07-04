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
    #region Private Methods

    private static void SetTheme(string theme)
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

    #endregion

    #region Public Methods

    public static TopLevel? GetTopLevel()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }

    public static void CloseOnExitCommand()
    {
        Settings.Instance.Save();
        Environment.Exit(0);
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        SetTheme(Settings.Instance.Theme);
    }

    public override void OnFrameworkInitializationCompleted()
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
            desktop.ShutdownRequested += (_, _) => { Settings.Instance.Save(); };
            main.Show();

            // splash.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }

    #endregion
}