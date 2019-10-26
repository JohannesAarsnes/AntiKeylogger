using System;
using System.Globalization;
using System.Windows.Data;

namespace AntiKeyloggerUI.Auxiliary
{
    class IsStringEmptyConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type type = value.GetType();
            if (type == typeof(string))
            {
                if (string.IsNullOrEmpty(value as string))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}