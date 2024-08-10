using System;
using System.Windows.Input;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.Tabs.Preferences;

public class FileManagerPreferencesViewModel : ViewModelBase
{
    private bool _isVisible;
    private bool _shouldAskExtractionFolderName;
    private bool _shouldCalculateFolderSize;
    private bool _shouldSaveLastPathOnExit;

    public FileManagerPreferencesViewModel()
    {
        ShouldAskExtractionFolderName = Settings.Instance.FileManagerPreference.ShouldAskExtractionFolderName;
        ShouldCalculateFolderSize = Settings.Instance.FileManagerPreference.ShouldCalculateFolderSize;
        ShouldSaveLastPathOnExit = Settings.Instance.FileManagerPreference.ShouldSaveLastPathOnExit;
        this.WhenAnyValue(x => x.ShouldAskExtractionFolderName)
            .Subscribe(value => Settings.Instance.FileManagerPreference.ShouldAskExtractionFolderName = value);
        this.WhenAnyValue(x => x.ShouldCalculateFolderSize)
            .Subscribe(value => Settings.Instance.FileManagerPreference.ShouldCalculateFolderSize = value);
        this.WhenAnyValue(x => x.ShouldSaveLastPathOnExit)
            .Subscribe(value => Settings.Instance.FileManagerPreference.ShouldSaveLastPathOnExit = value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public bool ShouldAskExtractionFolderName
    {
        get => _shouldAskExtractionFolderName;
        set => this.RaiseAndSetIfChanged(ref _shouldAskExtractionFolderName, value);
    }

    public bool ShouldSaveLastPathOnExit
    {
        get => _shouldSaveLastPathOnExit;
        set => this.RaiseAndSetIfChanged(ref _shouldSaveLastPathOnExit, value);
    }

    public bool ShouldCalculateFolderSize
    {
        get => _shouldCalculateFolderSize;
        set => this.RaiseAndSetIfChanged(ref _shouldCalculateFolderSize, value);
    }

    public ICommand SetFileManagerPosition => ReactiveCommand.Create<string>((args) =>
    {
        Settings.Instance.FileManagerPreference.Position = args;
        MessageBus.Current.SendMessage(new SetFileManagerPositionCommand(args));
    });
}