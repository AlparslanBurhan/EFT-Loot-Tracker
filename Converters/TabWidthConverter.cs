using System;
using System.Globalization;
using System.Windows.Data;

namespace EFTLootTracker.Converters
{
    public class TabWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width)
            {
                return (width - 40) / 2;
            }
            return 430.0; // fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
