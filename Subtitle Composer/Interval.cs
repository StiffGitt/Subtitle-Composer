using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subtitle_Composer
{
    public class Interval
    {
        public TimeSpan ShowTime { get; set; }
        public TimeSpan HideTime { get; set; }
        public string Text { get; set; }
        public string Translation { get; set; }

    }
}
