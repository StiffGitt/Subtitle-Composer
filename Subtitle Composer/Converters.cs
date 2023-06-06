using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Subtitle_Composer
{
    public class TimeSpanToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpanFormatting.GetString((TimeSpan)value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpanFormatting.StringToTimeSpan((string)value).Item2;
            //string s = (string)value;
            //string[] ss = s.Split(':');
            //int[] times = new int[ss.Length];
            //for (int i = 0; i < ss.Length; i++)
            //{
            //    times[i] = int.Parse(ss[i]);
            //}
            
            //return new TimeSpan(times[0], times[1], times[2]);
        }
    }
    public class StringLengthConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "0";
            return ((string )value).Length.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DurationTimeConverter : IMultiValueConverter
    {
        private TimeSpan showTime;
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string show = (string)values[0];
            string hide = (string)values[1];
            (_, var showts) = TimeSpanFormatting.StringToTimeSpan(show);
            (_, var hidets) = TimeSpanFormatting.StringToTimeSpan(hide);
            showTime = showts;
            return TimeSpanFormatting.GetString(hidets - showts);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (showTime == null)
                return new string[] { "","" };
            return new string[] { TimeSpanFormatting.GetString(showTime), TimeSpanFormatting.GetString(showTime + TimeSpanFormatting.StringToTimeSpan((string)value).Item2) };
        }
    }
}
