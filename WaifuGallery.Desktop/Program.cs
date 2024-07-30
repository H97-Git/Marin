using System;
using System.IO;
using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Svg.Skia;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using WaifuGallery.Models;

namespace WaifuGallery.Desktop;

internal static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
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
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Something went wrong during lifetime!");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp() //
    {
#if DEBUG
        GC.KeepAlive(typeof(SvgImageExtension).Assembly);
        GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);
#endif
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            // .LogToTrace()
            .UseReactiveUI();
    }
}