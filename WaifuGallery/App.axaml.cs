using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

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
            var splash = new SplashScreenWindow();

            desktop.MainWindow = splash;
            splash.Show();

            await splash.InitApp();

            var main = new MainWindow();
            desktop.MainWindow = main;
            main.Show();

            splash.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }
}