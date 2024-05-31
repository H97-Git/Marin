using System.Collections.Generic;
using WaifuGallery.ViewModels;
using WaifuGallery.ViewModels.Tabs;

namespace WaifuGallery.Helpers;

public class TabsComparer : IComparer<TabViewModelBase>
{
    public int Compare(TabViewModelBase? x, TabViewModelBase? y)
    {
        if (x is ImageTabViewModel && y is ImageTabViewModel)
            // Compare ImageTabViewModel objects
            // Add your custom sorting logic here
            return 0;

        // Can't have two settings tabs
        // if (x is TabSettingsViewModel && y is TabSettingsViewModel)
        // {
        //     // Compare SettingsViewModel objects
        //     // Add your custom sorting logic here
        //     return 0;
        // }

        if (x is ImageTabViewModel)
            return -1; // ImageTabViewModel comes before SettingsViewModel

        if (y is ImageTabViewModel)
            return 1; // SettingsViewModel comes after ImageTabViewModel

        return 0; // Both are neither ImageTabViewModel nor SettingsViewModel
    }
}