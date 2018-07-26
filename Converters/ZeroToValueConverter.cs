using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BlueEyes.Converters
{
    public class ZeroToVisibilityConverter : ZeroToValueConverter<Visibility> { }

    public class ZeroToValueConverter<T> : IValueConverter
    {
        public T ZeroValue { get; set; }
        public T NonZeroValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return ZeroValue;
            else
            {
                if (System.Convert.ToInt32(value) == 0)
                {
                    return ZeroValue;
                }
                else
                {
                    return NonZeroValue;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;
            else
            {
                if (value.Equals(ZeroValue))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
