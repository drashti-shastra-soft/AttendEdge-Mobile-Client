using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeMobileClient.Converter
{
    public class IntToBGColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && int.TryParse(value.ToString(), out int input))
            {
                if (input == 1)
                    return Color.FromArgb("#FF0000"); // Light Gray
                else if (input == 2)
                    return Color.FromArgb("#D3D3D3"); // Slightly darker Gray
                else if (input == 3)
                    return Color.FromArgb("#88E788"); // Approve
                else if (input == 4)
                    return Color.FromArgb("#F7CB73"); // Pending
                else if (input == 5)
                    return Color.FromArgb("#D14249");
                else
                    return Color.FromArgb("#B88383");
            }

            // Default color if value is null or invalid
            return Color.FromArgb("#E6E6E6");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
