using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Subtitle_Composer
{
    public class TimeSpanValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            //string s = (string)value;
            if ((string) value == "" || !TimeSpanFormatting.StringToTimeSpan((string)value).Item1)
                return new ValidationResult(false, "not a timespan");
            else
                return ValidationResult.ValidResult;
        }
    }
}
