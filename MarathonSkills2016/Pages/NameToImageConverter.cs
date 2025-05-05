using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MarathonSkills2016.Pages
{
    public class NameToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            string name = value.ToString().ToLower().Replace(" ", "-");
            try
            {
                return new BitmapImage(new Uri($"pack://application:,,,/Resources/HowLong/{name}.jpg"));
            }
            catch
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/HowLong/default.jpg"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

