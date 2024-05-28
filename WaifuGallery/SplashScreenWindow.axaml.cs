using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace WaifuGallery;

public partial class SplashScreenWindow : Window
{
    public SplashScreenWindow()
    {
        InitializeComponent();
    }
    
    public async Task InitApp()
    {
        var start = DateTime.Now.Ticks;
        var time = start;
        var progressValue = 0;

        while ((time - start) < TimeSpan.TicksPerSecond)
        {
            progressValue++;
            Dispatcher.UIThread.Post(() => ProgressBar1.Value = progressValue);
            await Task.Delay(25);
            time = DateTime.Now.Ticks;
        }

        start = time;
        Dispatcher.UIThread.Post(() => LoadingText.Text = "Initializing Application Settings...");
        const long limit = TimeSpan.TicksPerSecond * 2;
        while ((time - start) < limit)
        {
            progressValue += 1;
            Dispatcher.UIThread.Post(() => ProgressBar1.Value = progressValue);
            await Task.Delay(50);
            time = DateTime.Now.Ticks;
        }

        Dispatcher.UIThread.Post(() => LoadingText.Text = "Preparing App...");

        while (progressValue < 100)
        {
            progressValue += 1;
            Dispatcher.UIThread.Post(() => ProgressBar1.Value = progressValue);
            await Task.Delay(10);
        }
    }
}