using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using WaifuGallery.Models;
using WaifuGallery.ViewModels;
using WaifuGallery.Views;

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

    public static TopLevel? GetTopLevel() =>
        Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

    public static MainViewViewModel? GetMainViewViewModel() =>
        ((GetTopLevel() as MainWindow)?.Content as MainView)?.DataContext as MainViewViewModel;

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
            desktop.ShutdownRequested += (_, _) => { SaveSettings(); };
            main.Show();

            // splash.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void SaveSettings(bool shouldExit = false)
    {
        Settings.Instance.Save();
        if (shouldExit)
            Environment.Exit(0);
    }

   

    #endregion
}