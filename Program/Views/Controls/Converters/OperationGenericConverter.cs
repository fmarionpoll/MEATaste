using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// adapted from Illya Reznykov https://github.com/IReznykov/Blog

namespace MEATaste.Views.Controls.Converters
{
	public abstract class OperationGenericConverter<T> : BaseGenericConverter<T>, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType.IsAssignableFrom(typeof(T)), $"targetType should be {typeof(T).FullName}");

            if (values == null || values.Length <= 0)
                return DependencyProperty.UnsetValue;

            var tValues = Convert(values, culture);

            if (tValues.Count < 2)
                return DependencyProperty.UnsetValue;

            var tResult = BinaryMethod(tValues[0], tValues[1]);

            ApplyParameter(parameter, culture, ref tResult);
            return System.Convert.ChangeType(tResult, targetType);

        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, CultureInfo culture)
        {
            return null;
        }

        protected abstract Func<T, T, T> BinaryMethod { get; }

    }
}
