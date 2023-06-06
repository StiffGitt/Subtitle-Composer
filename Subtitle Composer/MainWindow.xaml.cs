using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SubtitlesPlugger;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

namespace Subtitle_Composer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Interval> Intervals { get; set; }
        private bool playerIsPlaying = false;
        private bool isSliderDragged = false;
        //private IPluggin usedPluggin;
        private List<IPluggin> plugginsList;
        public MainWindow()
        {
            InitializeComponent();

            Intervals = new ObservableCollection<Interval>();

            DataContext = Intervals;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += timer_Tick;
            timer.Start();
            timeTable.Items.SortDescriptions.Add(new SortDescription(timeTable.Columns[0].SortMemberPath, ListSortDirection.Ascending));
            LoadPluggins();
        }
        private void LoadPluggins()
        {
            try
            {

                string plugPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\pluggins";
                var pluggins = new List<IPluggin>();
                string connect = plugPath;
                var files = Directory.GetFiles(connect, "*.dll");
                foreach (var file in files)
                {
                    Assembly _Assembly = Assembly.LoadFile(file);
                    var types = _Assembly.GetTypes()?.ToList();
                    var type = types?.Find(a => typeof(IPluggin).IsAssignableFrom(a));
                    var win = (IPluggin)Activator.CreateInstance(type);
                    pluggins.Add(win);
                }
                LoadPlugginsMenu(pluggins);
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadPlugginsMenu(List<IPluggin> pluggins)
        {
            foreach (IPluggin pluggin in pluggins)
            {
                MenuItem plugginOpenItem = new MenuItem();
                plugginOpenItem.Header = pluggin.Name;
                plugginOpenItem.DataContext = pluggin;
                plugginOpenItem.Click += PlugginItemOpen_Click;
                openPlugginMenu.Items.Add(plugginOpenItem);
                var plugginSaveItem = new MenuItem();
                plugginSaveItem.Header = pluggin.Name;
                plugginSaveItem.DataContext = pluggin;
                plugginSaveItem.Click += PlugginItemSave_Click;
                savePlugginMenu.Items.Add(plugginSaveItem);
                var plugginSaveTransItem = new MenuItem();
                plugginSaveTransItem.Header = pluggin.Name;
                plugginSaveTransItem.DataContext = pluggin;
                plugginSaveTransItem.Click += PlugginItemSaveTrans_Click;
                saveTransPlugginMenu.Items.Add(plugginSaveTransItem);
            }
        }

        private void PlugginItemOpen_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            IPluggin pluggin = (IPluggin) item.DataContext;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = $"{pluggin.Extention} Files (*{pluggin.Extention})|*{pluggin.Extention}";
            if (!(bool)dialog.ShowDialog())
                return;
            string path = dialog.FileName;
            string field = (isTranslation.IsChecked)? "Translation" : "Text";
            //string field = "Translation";
            List<TextInterval> textIntervals = (List<TextInterval>)pluggin.Load(path);
            GetIntervals(textIntervals, field);
            //GetIntervals(subtitleObjects, "Text");

            //MessageBox.Show(usedPluggin.Name);
        }
        private void PlugginItemSave_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            IPluggin pluggin = (IPluggin)item.DataContext;
            Save(pluggin, "Text"); 
            
            //MessageBox.Show(usedPluggin.Name);
        }

        private void PlugginItemSaveTrans_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            IPluggin pluggin = (IPluggin)item.DataContext;
            Save(pluggin, "Translation");
            //MessageBox.Show(usedPluggin.Name);
        }
        private List<TextInterval> GetTextIntervals(string field)
        {
            var list = new List<TextInterval>();
            Func<Interval, string> getText;
            if (field == "Text")
                getText = new Func<Interval, string>(x => x.Text);
            else
                getText = new Func<Interval, string>(x => x.Translation);
            foreach (var it in Intervals)
            {
                TextInterval tIt = new TextInterval();
                tIt.ShowTime = it.ShowTime;
                tIt.HideTime = it.HideTime;
                tIt.Text = getText(it);
                list.Add(tIt);
            }
            return list;
        }
        private void Save(IPluggin pluggin, string field)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = $"{pluggin.Extention} Files (*{pluggin.Extention})|*{pluggin.Extention}";
            dialog.DefaultExt = pluggin.Extention;
            if (!(bool)dialog.ShowDialog())
            {
                return;
            }
            string filePath = dialog.FileName;
            var collection = GetTextIntervals(field);
            pluggin.Save(filePath, collection);

            //MessageBox.Show(filePath);
        }
        private void GetIntervals(List<TextInterval> textIntervals, string field)
        {
            //var list = new List<Interval>();
            Action<Interval, string> setText;
            if (field == "Text")
                setText = new Action<Interval, string>((x, y) => x.Text = y);
            else
                setText = new Action<Interval, string>((x, y) => x.Translation = y);
            Intervals.Clear();
            foreach (var tIt in textIntervals)
            {
                Interval it = new Interval();
                it.ShowTime = tIt.ShowTime;
                it.HideTime = tIt.HideTime;
                setText(it, tIt.Text);
                Intervals.Add(it);
            }
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((player.Source != null) && (player.NaturalDuration.HasTimeSpan) && (!isSliderDragged))
            {
                progressSlider.Minimum = 0;
                progressSlider.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
                progressSlider.Value = player.Position.TotalSeconds;
                SetSubtitles();
            }
        }
        private void isTranslation_Checked(object sender, RoutedEventArgs e)
        {
            //if(!((MenuItem)sender).IsChecked)
            //    dataTranslationColumn.Visibility = Visibility.Collapsed;
            //else 
            //MessageBox.Show(Intervals.First().Text);
            dataTranslationColumn.Visibility = Visibility.Visible;
        }
        private void SetSubtitles()
        {
            string s = "";
            foreach (var it in Intervals)
            {
                if (it.ShowTime == null || it.HideTime == null)
                    continue;
                if (it.ShowTime <= player.Position && it.HideTime >= player.Position)
                    s += ((isTranslation.IsChecked) ? it.Translation : it.Text) + '\n';
            }
            if(s.Length > 0)
                s = s.Substring(0, s.Length - 1);
            subtitleTextBlock.Text = s;
        }
        private void isTranslation_Unchecked(object sender, RoutedEventArgs e)
        {
            dataTranslationColumn.Visibility = Visibility.Collapsed;
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg; *.mp4)|*.mp3;*.mpg;*.mpeg;*.mp4|All files (*.*)|*.*";
            if ((bool)dialog.ShowDialog())
                player.Source = new Uri(dialog.FileName);
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = player != null && player.Source != null;
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            player.Play();
            playerIsPlaying = true;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = playerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            player.Pause();
            playerIsPlaying = false;
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = playerIsPlaying;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            player.Stop();
            playerIsPlaying = false;
        }


        private void progressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isSliderDragged = true;
        }

        private void progressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isSliderDragged = false;
            player.Position = TimeSpan.FromSeconds(progressSlider.Value);
        }

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            progressTextBlock.Text = TimeSpan.FromSeconds(progressSlider.Value).ToString("hh\\:mm\\:ss\\.fff");
        }

        private void player_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            player.Volume += (e.Delta > 0) ? 0.1 : -0.1;
            if(player.Volume < 0)
                player.Volume = 0;
            if(player.Volume > 1)
                player.Volume = 1;
        }

        private void player_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (playerIsPlaying)
            {
                player.Pause();
                playerIsPlaying = false;
            }
            else
            {
                player.Play();
                playerIsPlaying = true;
            }
        }

        private void progressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double clickPosition = e.GetPosition(progressBar).X;
            double width = progressBar.ActualWidth;
            player.Volume = clickPosition / width * progressBar.Maximum;
        }

        private void timeTable_Selected(object sender, SelectedCellsChangedEventArgs e)
        {
            if (timeTable.SelectedItem != null)
            {
                try
                {
                    var cur = ((Interval) timeTable.SelectedItem).ShowTime;
                    if(player != null && player.Source != null && player.NaturalDuration >= cur)
                        player.Position = cur;
                }
                catch { }
            }
            
        }


        private void timeTable_Add(object sender, RoutedEventArgs e)
        {
            Interval it = new Interval();
            TimeSpan max = Intervals.Max(x => x.HideTime);
            it.ShowTime = max;
            it.HideTime = max;
            Intervals.Add(it);
        }

        private void timeTable_AddAfter(object sender, RoutedEventArgs e)
        {
            var its = new List<Interval>();
            foreach (var r in timeTable.SelectedItems)
            {
                its.Add((Interval)r);
            }
            Interval it = new Interval();
            TimeSpan max = its.Max(x => x.HideTime);
            it.ShowTime = max;
            it.HideTime = max;
            Intervals.Add(it);
            
        }

        private void time_Delete(object sender, RoutedEventArgs e)
        {
            var its = new List<Interval>();
            foreach (var r in timeTable.SelectedItems)
            {
                its.Add((Interval)r);
            }
            while (its.Count() > 0)
            {
                Intervals.Remove(its.First());
                its.Remove(its.First());
            }
        }

        
        
    }
}
