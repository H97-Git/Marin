using System;
using Marin.UI.ViewModels.Tabs;

namespace Marin.UI.Factories;

public class TabFactory(Func<TabType, TabViewModelBase> factory)
{
    private readonly Func<TabType, TabViewModelBase> _factory = factory;
    
    public TabViewModelBase Create(TabType type) => _factory.Invoke(type);
}