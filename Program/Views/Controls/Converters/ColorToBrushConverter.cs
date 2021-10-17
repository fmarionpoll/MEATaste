using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

// adapted from Illya Reznykov https://github.com/IReznykov/Blog

namespace MEATaste.Views.Controls.Converters
{
    [ValueConversion(typeof(double[]), typeof(double))]
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color)
                return new SolidColorBrush((Color)value);

            return DependencyProperty.UnsetValue;
            //throw new InvalidOperationException("Unsupported type [" + value.GetType().Name + "], ColorToSolidColorBrushValueConverter.Convert()");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            var brush = value as SolidColorBrush;
            if (brush != null)
                return brush.Color;

            return DependencyProperty.UnsetValue;
            //throw new InvalidOperationException("Unsupported type [" + value.GetType().Name + "], ColorToSolidColorBrushValueConverter.ConvertBack()");
        }

    }
}
