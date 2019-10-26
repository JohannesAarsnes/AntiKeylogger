using AntiKeyloggerUI.Auxiliary;
using System;
using System.Globalization;
using System.Windows.Data;

namespace AntiKeyloggerUI.Auxiliary
{
    public sealed class ConvertEnumToDescription : IValueConverter
    {
        public object Convert( object values, Type targetType, object parameter, CultureInfo culture ) {
             return EnumDescription.Description(values);
        }
        public object ConvertBack( object value, Type targetTypes, object parameter, CultureInfo culture ){
            return (byte)value;
        }
    }
}

