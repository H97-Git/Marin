using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
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

    private static WindowState _bufferedWindowState = WindowState.Normal;

    private static Window? GetMainWindow()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }

    private static void SetThemeVariant(string themeVariant)
    {
        if (Current is null) return;
        if (themeVariant.Equals("Light", StringComparison.OrdinalIgnoreCase))
        {
            Current.RequestedThemeVariant = ThemeVariant.Light;
        }
        else if (themeVariant.Equals("Dark", StringComparison.OrdinalIgnoreCase))
        {
            Current.RequestedThemeVariant = ThemeVariant.Dark;
        }
        else
        {
            Current.RequestedThemeVariant = ThemeVariant.Default;
        }
    }

    private static ServiceCollection CreateServiceCollection()
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
        return collection;
    }

    #endregion

    #region Public Methods

    public static IClipboard? GetClipboard() =>
        GetMainWindow()?.Clipboard;

    public static IStorageProvider? GetStorageProvider() =>
        GetMainWindow()?.StorageProvider;

    public static WindowState? GetWindowState() =>
        GetMainWindow()?.WindowState;

    public static void ToggleFullScreen()
    {
        var mainWindow = GetMainWindow();
        if (mainWindow is null) return;
        if (mainWindow.WindowState is WindowState.FullScreen)
        {
            mainWindow.WindowState = _bufferedWindowState;
        }
        else
        {
            _bufferedWindowState = mainWindow.WindowState;
            mainWindow.WindowState = WindowState.FullScreen;
        }
    }

    public static MainViewViewModel? GetMainViewViewModel() =>
        ((GetMainWindow() as MainWindow)?.Content as MainView)?.DataContext as MainViewViewModel;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        SetThemeVariant(Settings.Instance.Theme);
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

            desktop.MainWindow = new MainWindow()
            {
                Content = new MainView()
                {
                    DataContext = CreateServiceCollection().BuildServiceProvider()
                        .GetRequiredService<MainViewViewModel>()
                }
            };
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            desktop.ShutdownRequested += (_, _) => { SaveSettings(); };
            desktop.MainWindow.Show();
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