using System.Collections.Generic;
using WaifuGallery.ViewModels;

namespace WaifuGallery.Helpers;

public class TabsComparer : IComparer<TabViewModel>
{
    public int Compare(TabViewModel? x, TabViewModel? y)
    {
        if (x is ImageTabViewModel && y is ImageTabViewModel)
        {
            // Compare ImageTabViewModel objects
            // Add your custom sorting logic here
            return 0;
        }

        // Can't have two settings tabs
        // if (x is TabSettingsViewModel && y is TabSettingsViewModel)
        // {
        //     // Compare SettingsViewModel objects
        //     // Add your custom sorting logic here
        //     return 0;
        // }

        if (x is ImageTabViewModel)
        {
            return -1; // ImageTabViewModel comes before SettingsViewModel
        }

        if (y is ImageTabViewModel)
        {
            return 1; // SettingsViewModel comes after ImageTabViewModel
        }

        return 0; // Both are neither ImageTabViewModel nor SettingsViewModel
    }
}