using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using FluentAvalonia.UI.Controls;

namespace WaifuGallery.Controls.Tabs.Preferences;

public class Category : TemplatedControl
{
    public static readonly StyledProperty<IconSource> IconSourceProperty =
        AvaloniaProperty.Register<Category, IconSource>(nameof(IconSource));

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<Category, string>(nameof(Description));

    public static readonly StyledProperty<string> ContentProperty =
        AvaloniaProperty.Register<Category, string>(nameof(Content));

    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<Category, ICommand>(nameof(Command));

    public static readonly StyledProperty<string> CommandParameterProperty =
        AvaloniaProperty.Register<Category, string>(nameof(CommandParameter));

    public IconSource IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public string CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
}