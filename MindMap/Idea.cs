using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MindStorm
{
    class ExIdea : Idea
    {
        private Collection<Idea> children_left = new Collection<Idea>();
        private Collection<Idea> children_right = new Collection<Idea>();
        private Collection<Idea> children_up = new Collection<Idea>();
        private Collection<Idea> children_down = new Collection<Idea>();
        public Collection<Line> relations = new Collection<Line>();
        public Line parent_rel = null;

        public ExIdea() : base()
        {
            this.MouseMove += new MouseEventHandler(Mouse_Move);
        }

        public Collection<Idea> Children(Key direction)
        {
            switch (direction)
            {
                case Key.Left:
                    return children_left;
                case Key.Right:
                    return children_right;
                case Key.Up:
                    return children_up;
                case Key.Down:
                    return children_down;
            }
            return null;
        }

        public void AddRelation(ExIdea child)
        {
            var relation = new Line()
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                X1 = this.Margin.Left + this.Width / 2,
                Y1 = this.Margin.Top + this.Height / 2,
                X2 = child.Margin.Left + child.Width / 2,
                Y2 = child.Margin.Top + child.Height / 2
            };

            relations.Add(relation);
            child.parent_rel = relation;
        }

        public int ChildrenCount
        {
            get { return children_left.Count + children_right.Count + children_down.Count + children_up.Count; }
        }

        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                Point point = e.GetPosition(null);

                foreach (var relation in relations)
                {
                    relation.X1 = point.X;
                    relation.Y1 = point.Y;
                }

                if (parent_rel != null)
                {
                    parent_rel.X2 = point.X;
                    parent_rel.Y2 = point.Y;
                }
            }

            base.Mouse_Move(sender, e);
        }
    }

    public delegate void del(object sender);

    class Idea : Border
    {
        public del notify;

        const int default_width = 120;
        const int default_height = 70;
        const int default_fontsize = 50;

        TextBlock name;

        public Idea()
        {
            this.Width = default_width;
            this.Height = default_height;
            this.Background = new SolidColorBrush(Colors.AliceBlue);
            this.BorderBrush = new SolidColorBrush(Colors.DodgerBlue);

            this.MouseDown += new MouseButtonEventHandler(Mouse_Down);
            this.MouseMove += new MouseEventHandler(Mouse_Move);
            this.MouseUp += new MouseButtonEventHandler(Mouse_Left_Up);

            TextBox textbox = new TextBox() { FontSize = default_fontsize-20 };
            textbox.KeyDown += (object sender, KeyEventArgs e) =>
            {
                if (e.Key == Key.Enter)
                {
                    name = new TextBlock()
                    {
                        Text = textbox.Text,
                        FontSize = default_fontsize,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    this.Child = name;
                }
            };
            this.Child = textbox;
        }

        public string Name
        {
            get { return name.Text; }
            set { name.Text = value; Width = Name.Length * name.FontSize / 2 + 20; }
        }

        public double FontSize
        {
            get { return name.FontSize; }
            set { name.FontSize = value; Height = value + 20; Width = Name.Length * name.FontSize / 2 + 20; }
        }

        private void Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            notify(this);
            this.CaptureMouse();
        }

        protected void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                Point point = e.GetPosition(null);

                this.Margin = new Thickness(point.X - this.Width / 2,
                    point.Y - this.Height / 2, 0, 0);
            }
        }

        private void Mouse_Left_Up(object sender, MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
        }
    }
}
