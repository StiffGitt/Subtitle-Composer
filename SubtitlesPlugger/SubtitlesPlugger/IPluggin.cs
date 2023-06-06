using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesPlugger
{
    public interface IPluggin
    {
        string Name { get; }
        string Extention { get; }
        ICollection<TextInterval> Load(string Path);
        void Save(string path, ICollection<TextInterval> intervals);
    }
}
