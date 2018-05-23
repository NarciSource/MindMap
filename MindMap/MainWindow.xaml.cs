using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MindStorm
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        ExIdea active_idea = null;


        public MainWindow()
        {
            InitializeComponent();
        }


        private ExIdea MakeIdea(Key direction)
        {
            const int each_distance = 100;
            const int peer_distance = 200;

            ExIdea idea = new ExIdea();
            var children = active_idea.Children(direction);

            int sign = (children.Count % 2 == 1) ? -1 : 1;
            int distance = sign * (children.Count + 1) / 2 * each_distance;

            switch (direction)
            {
                case Key.Left:
                    idea.Margin = new Thickness(active_idea.Margin.Left - peer_distance,
                        active_idea.Margin.Top + distance, 0, 0);
                    break;
                case Key.Right:
                    idea.Margin = new Thickness(active_idea.Margin.Left + peer_distance,
                        active_idea.Margin.Top + distance, 0, 0);
                    break;
                case Key.Up:
                    idea.Margin = new Thickness(active_idea.Margin.Left + distance * 2,
                        active_idea.Margin.Top - peer_distance, 0, 0);
                    break;
                case Key.Down:
                    idea.Margin = new Thickness(active_idea.Margin.Left + distance * 2,
                        active_idea.Margin.Top + peer_distance, 0, 0);
                    break;
            }
            idea.notify += Active;

            children.Add(idea);
            active_idea.AddRelation(idea);

            canvas.Children.Add(idea);
            canvas.Children.Add(idea.parent_rel);
            Canvas.SetZIndex(canvas.Children[canvas.Children.Count - 1], -1);

            return idea;
        }

        private void DeleteIdea(ExIdea idea)
        {
            foreach (ExIdea child in idea.Children(Key.Left))
                DeleteIdea(child);
            foreach (ExIdea child in idea.Children(Key.Right))
                DeleteIdea(child);
            foreach (ExIdea child in idea.Children(Key.Up))
                DeleteIdea(child);
            foreach (ExIdea child in idea.Children(Key.Down))
                DeleteIdea(child);

            canvas.Children.Remove(idea);
            if(idea.parent_rel!=null)
                canvas.Children.Remove(idea.parent_rel);
        }

        private void Active(object sender)
        {

            if (active_idea == (ExIdea)sender)
            {
                active_idea.BorderThickness = new Thickness(0);

                active_idea = null;
                SettingDock.Visibility = Visibility.Hidden;
            }
            else
            {
                if (active_idea != null)
                    active_idea.BorderThickness = new Thickness(0);

                active_idea = (ExIdea)sender;
                active_idea.BorderThickness = new Thickness(1);
                setting_name.Text = active_idea.Name;
                setting_fontsize.Text = active_idea.FontSize.ToString();

                SettingDock.Visibility = Visibility.Visible;
            }
        }
        



        private void New_Click(object sender, RoutedEventArgs e)
        {
            ExIdea idea = new ExIdea()
            {
                Margin = new Thickness((int)canvas.RenderSize.Width/2, (int)canvas.RenderSize.Height/2 -100, 0, 0)
            };
            idea.notify += Active;

            canvas.Children.Add(idea);

            door_img.Visibility = Visibility.Hidden;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png Image|*.png";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == "")
                saveFileDialog.FileName = "image.png";

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)canvas.RenderSize.Width,
                (int)canvas.RenderSize.Height,
                96d, 96d, System.Windows.Media.PixelFormats.Default);

            rtb.Render(canvas);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));


            using (var fs = System.IO.File.OpenWrite(saveFileDialog.FileName))
            {
                pngEncoder.Save(fs);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Autor : Jeong Won-Cheol\rData : 2018-05-23", "About");
        }





        private void setting_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            active_idea.Name = ((TextBox)sender).Text;
        }

        private void setting_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBox)sender).SelectedValue.ToString())
            {
                case "Alice Blue":
                    active_idea.Background = new SolidColorBrush(Colors.AliceBlue);
                    break;
                case "Light Pink":
                    active_idea.Background = new SolidColorBrush(Colors.LightPink);
                    break;
                case "Lavender":
                    active_idea.Background = new SolidColorBrush(Colors.Lavender);
                    break;
                case "MintCream":
                    active_idea.Background = new SolidColorBrush(Colors.MintCream);
                    break;
                case "MistyRose":
                    active_idea.Background = new SolidColorBrush(Colors.MistyRose);
                    break;
                case "Snow":
                    active_idea.Background = new SolidColorBrush(Colors.Snow);
                    break;

            }
        }

        private void setting_fontsize_TextChanged(object sender, TextChangedEventArgs e)
        {
            double num = 0;
            if (double.TryParse(setting_fontsize.Text, out num))
            {
                active_idea.FontSize = num;
            }
        }



        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            point_position.Content = e.GetPosition(null);

            if (active_idea != null)
                num_of_children.Content = active_idea.ChildrenCount;

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (active_idea == null) return;

            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    var idea = MakeIdea(e.Key);
                    break;

                case Key.Delete:
                    DeleteIdea(active_idea);
                    break;
            }
        }
    }
}
