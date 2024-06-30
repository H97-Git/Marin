using ReactiveUI;

namespace WaifuGallery.ViewModels.Dialogs;

public class NewFolderViewModel: ViewModelBase
{
    
    private string _newFolderName = string.Empty;
    public string NewFolderName
    {
        get => _newFolderName;
        set => this.RaiseAndSetIfChanged(ref _newFolderName, value);
    }
}