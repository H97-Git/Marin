using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Marin.UI.Converters;

public class DepthToBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int depth && parameter is string buttonValueString && int.TryParse(buttonValueString, out int buttonValue))
        {
            return depth == buttonValue;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isChecked && isChecked && parameter is string buttonValueString && int.TryParse(buttonValueString, out int buttonValue))
        {
            return buttonValue;
        }
        return BindingOperations.DoNothing;
    }
}