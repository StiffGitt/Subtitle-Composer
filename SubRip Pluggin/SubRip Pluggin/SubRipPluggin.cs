using SubtitlesPlugger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubRip_Pluggin
{
    public class SubRipPluggin : IPluggin
    {
        public string Name { get; } = "SubRip";

        public string Extention { get; set; } = ".srt";

        public ICollection<TextInterval> Load(string Path)
        {
            List<TextInterval> subtitleObjects = new List<TextInterval>();

            string srtContent = File.ReadAllText(Path);
            string[] lines = File.ReadAllLines(Path);
            // Przetwarzanie linii pliku SRT
            for (int i = 0; i < lines.Length;)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    break;
                //int index = int.Parse(lines[i]);
                TimeSpan showTime = TimeSpan.ParseExact((lines[i + 1].Split(new[] { "-->" }, StringSplitOptions.RemoveEmptyEntries)[0]).Trim(), @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
                TimeSpan hideTime = TimeSpan.ParseExact((lines[i + 1].Split(new[] { "-->" }, StringSplitOptions.RemoveEmptyEntries)[1]).Trim(), @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
                i += 2;
                string text = "";
                while (i < lines.Length && !string.IsNullOrEmpty(lines[i]))
                {
                    text += lines[i++] + '\n';
                }
                text = text.Substring(0, text.Length - 1);
                i++;
                TextInterval subtitleObject = new TextInterval
                {
                    ShowTime = showTime,
                    HideTime = hideTime,
                    Text = text
                };
                subtitleObjects.Add(subtitleObject);
            }
            return subtitleObjects;
        }

        public void Save(string path, ICollection<TextInterval> intervals)
        {
            string srtContent = string.Empty;
            int index = 1;

            foreach (TextInterval subtitleObject in intervals)
            {
                // Formatowanie czasu w formacie HH:mm:ss,fff
                string showTime = subtitleObject.ShowTime.ToString(@"hh\:mm\:ss\,fff");
                string hideTime = subtitleObject.HideTime.ToString(@"hh\:mm\:ss\,fff");

                // Dodaj numer linii, czas wyświetlania i tekst do zawartości pliku SRT
                srtContent += $"{index}\n{showTime} --> {hideTime}\n{subtitleObject.Text}\n\n";

                index++;
            }
            File.WriteAllText(path, srtContent);
        }
    }
}
