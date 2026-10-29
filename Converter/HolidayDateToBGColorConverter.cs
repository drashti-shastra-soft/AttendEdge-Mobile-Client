using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeMobileClient.Converter
{
    public class HolidayDateToBGColorConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is DateTime holidayDate)
            {
                // Holiday has already passed
                if (holidayDate.Date < DateTime.Today)
                {
                    return Color.FromArgb("#D3D3D3"); // Gray
                }

                // Today or upcoming holiday
                return Color.FromArgb("#2F80ED"); // Primary color
            }

            // Default
            return Color.FromArgb("#D3D3D3");
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
