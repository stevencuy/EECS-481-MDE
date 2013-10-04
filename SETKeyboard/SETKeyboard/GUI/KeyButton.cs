using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SETKeyboard.GUI
{
    public class KeyButton : Button
    {
        private bool lowerCase = true;

        public KeyButton(string str, int width, int height, int marginL, int marginT, int marginR, int marginB)
            : base()
        {
            Content = str;
            Margin = new Thickness(marginL, marginT, marginR, marginB);
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Width = width;
            Height = height;
        }

        public void Toggle()
        {
            Content = (lowerCase) ? Content.ToString().ToUpper() : Content.ToString().ToLower();
            lowerCase = !lowerCase;
        }
    }
}
