using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subtitle_Composer
{
    public static class TimeSpanFormatting
    {
        public static (bool, TimeSpan) StringToTimeSpan(string value)
        {
            TimeSpan res;
            bool isParsable = true;
            if (TimeSpan.TryParseExact(value, new[] { "%s", "%s\\.f", "%m\\:%s","%m\\:%s\\.ff", "%h\\:%m\\:%s\\.fff", "%h\\:%m\\:%s" }, CultureInfo.InvariantCulture, out res)) ;
            //else if (TimeSpan.TryParseExact(value, "ss\\.f", CultureInfo.InvariantCulture, out res)) ;
            //else if (TimeSpan.TryParseExact(value, "mm\\:ss", CultureInfo.InvariantCulture, out res)) ;
            //else if (TimeSpan.TryParseExact(value, "mm\\:ss\\.ss", CultureInfo.InvariantCu
            //lture, out res)) ;
            //else if (TimeSpan.TryParseExact(value, "hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture, out res)) ;
            //else if (TimeSpan.TryParseExact(value, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out res)) ;
            else
            {
                res = new TimeSpan();
                isParsable = false;
            }
            return (isParsable, res);

        }
        public static string GetString(TimeSpan ts)
        {
            //string pattern = "^[ $"
            string s = ts.ToString("h\\:mm\\:ss\\.fff");
            //if (s.IndexOf('.') < s.IndexOf(':'))
            //{
            //    s = '.' + s 
            //    s.Trim()
            //    return s;
            //}
            s = s.Trim('0', ':');
            if (s[0] == '.')
                s = '0' + s;
            s = s.Trim('.');
            

            return s;
        }
    }
}
