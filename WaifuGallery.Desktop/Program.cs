using System;
using System.IO;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using WaifuGallery.Models;

namespace WaifuGallery.Desktop;

internal static class Program
{
    #region Private Members

    private const int TimeoutSeconds = 3;

    private static void SubscribeToDomainUnhandledException() =>
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            Log.Fatal(e.ExceptionObject as Exception, "Unhandled exception in domain!");
        };

    private static void CreateLogger()
    {
        Directory.CreateDirectory(Settings.LogsPath);
        var today = DateTime.Today.ToString("dd-MM-yyyy");
        const string outputTemplate = "[{Timestamp:HH:mm:ss.fff} {Level}]: {Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: outputTemplate,
                theme: AnsiConsoleTheme.Literate)
            .WriteTo.File(
                outputTemplate: outputTemplate,
                path: Path.Combine(Settings.LogsPath, $"{today}.log"))
            .CreateLogger();
    }

    #endregion

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp() //
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var mutex = new Mutex(false, typeof(Program).FullName);
        try
        {
            if (!mutex.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), true))
            {
                return;
            }

            CreateLogger();
            SubscribeToDomainUnhandledException();
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Something went wrong during lifetime!");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
            mutex.ReleaseMutex();
        }
    }
}