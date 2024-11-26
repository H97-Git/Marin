using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using WaifuGallery.Factories;
using WaifuGallery.Models;
using WaifuGallery.ViewModels;
using WaifuGallery.ViewModels.Tabs;
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
        var collection = new ServiceCollection();
        collection.AddSingleton<MainViewViewModel>();
        collection.AddTransient<ImageTabViewModel>();
        collection.AddTransient<PreferencesTabViewModel>();
        collection.AddSingleton<Func<TabType, TabViewModelBase>>(x => type => type switch
        {
            TabType.Image => x.GetRequiredService<ImageTabViewModel>(),
            TabType.Preferences => x.GetRequiredService<PreferencesTabViewModel>(),
            TabType.Unknown => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        });
        collection.AddSingleton<TabFactory>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // var splash = new SplashScreenWindow();
            //
            // desktop.MainWindow = splash;
            // splash.Show();
            //
            // await splash.InitApp();

            var main = new MainWindow()
            {
                Content = new MainView()
                {
                    DataContext = collection.BuildServiceProvider().GetRequiredService<MainViewViewModel>()
                }
            };
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