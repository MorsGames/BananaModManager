using System;
using Windows.UI.Xaml.Data;
using Microsoft.UI.Xaml.Data;

namespace BananaModManager.NewUI;

// This is so dumb
public class QuoteConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (value is null or "") ? "(None)" : $"\"{value}\"";

    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string stringValue)
        {
            return stringValue.Trim('"');
        }

        return value;
    }
}
