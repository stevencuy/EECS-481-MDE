using System;
using System.Collections.Generic;
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

namespace SETKeyboard.GUI
{
    public class TabPhrase : Button
    {
        private string data;

        public TabPhrase(string data_, int width_, int height_, int margin_l, int margin_t, int margin_r, int margin_b, SolidColorBrush backColor)
            : base()
        {
            data = data_;
            Content = data;
            Margin = new Thickness(margin_l, margin_t, margin_r, margin_b);
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Width = width_;
            Height = height_;
            Background = backColor;
        }
    }
}
