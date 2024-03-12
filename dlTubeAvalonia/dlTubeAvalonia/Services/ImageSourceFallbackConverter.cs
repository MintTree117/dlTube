namespace dlTubeAvalonia.Services;

using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;

public class ImageSourceFallbackConverter : IValueConverter
{
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
        string? imageUrl = value as string;
        
        if ( !string.IsNullOrWhiteSpace( imageUrl ) )
            return new Bitmap( imageUrl );

        // Specify the path to your fallback image
        string fallbackImagePath = "Assets/fallback.jpg";
        return new Bitmap( fallbackImagePath );
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
        throw new NotImplementedException();
    }
}