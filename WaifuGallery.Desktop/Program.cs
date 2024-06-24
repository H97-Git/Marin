using System;
using Avalonia;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Controls;

namespace WaifuGallery.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp() // => AppBuilder.Configure<Sandbox.App>()
    {
        // var symbols = Enum.GetValues(typeof(Symbol));
        // foreach (Symbol symbol in symbols)
        // {
        //     var val = $"<ui:SymbolIconSource x:Key=\"{symbol + "Icon"}\" Symbol=\"{symbol}\" />";
        //     var value = $"<StackPanel VerticalAlignment=\"Center\" HorizontalAlignment=\"Center\">" +
        //                 $"<ui:SymbolIcon Symbol=\"{symbol}\"  FontSize=\"24\" />" +
        //                 $"<TextBlock Text=\"{symbol}\" />" +
        //                 $"</StackPanel>";
        //     Console.WriteLine(value);
        // }
        //
        // Environment.Exit(0);
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }
}