using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SETKeyboard.GUI
{
    class QwertyKeyboard
    {
        private MainWindow window;
        private List<KeyButton> keys;
        private bool shift = false;
        public QwertyKeyboard(MainWindow window, double height, double width)
        {
            this.window = window;
            keys = new List<KeyButton>();

            string[] keyStrings = new string[29];
            //ROW 0
            keyStrings[0] = "q";
            keyStrings[1] = "w";
            keyStrings[2] = "e";
            keyStrings[3] = "r";
            keyStrings[4] = "t";
            keyStrings[5] = "y";
            keyStrings[6] = "u";
            keyStrings[7] = "i";
            keyStrings[8] = "o";
            keyStrings[9] = "p";
            //ROW 1
            keyStrings[10] = "a";
            keyStrings[11] = "s";
            keyStrings[12] = "d";
            keyStrings[13] = "f";
            keyStrings[14] = "g";
            keyStrings[15] = "h";
            keyStrings[16] = "j";
            keyStrings[17] = "k";
            keyStrings[18] = "l";
            //ROW 2
            keyStrings[19] = "shift";
            keyStrings[20] = "z";
            keyStrings[21] = "x";
            keyStrings[22] = "c";
            keyStrings[23] = "v";
            keyStrings[24] = "b";
            keyStrings[25] = "n";
            keyStrings[26] = "m";
            keyStrings[27] = "back";
            //SPACE ROW
            keyStrings[28] = "space";

            const int row_size = 3;
            int col_size = 10;
            int button_width = (int)(width / 10) - 1, button_height = (int)(height / 4) - 15;
            int margin_inc = button_width;
            int[] margins_l = new int[3];
            margins_l[0] = 0;
            margins_l[1] = button_width / 2;
            margins_l[2] = (button_width / 2) * 3;

            int index = 0;
            for (int row = 0; row < row_size; ++row)
            {
                for (int col = 0; col < col_size; ++col)
                {
                    string lower = keyStrings[index];
                    string upper = lower.ToUpper();
                    KeyButton b;
                    if (row != 2)
                    {
                        b = new KeyButton(upper, lower, button_width, button_height, margins_l[row] + margin_inc * col, row * button_height, 0, 0);
                        b.Click += new RoutedEventHandler(KeyHit_Click);
                    }
                    else
                    {
                        if (lower == "shift")
                        {
                            b = new KeyButton(upper, lower, (button_width / 2) * 3, button_height, 0, row * button_height, 0, 0);
                            b.Click += new RoutedEventHandler(Shift_Click);
                        }
                        else if (lower == "back")
                        {
                            b = new KeyButton(upper, lower, (button_width / 2) * 3, button_height, margins_l[row] + margin_inc * (col - 1), row * button_height, 0, 0);
                            b.Click += new RoutedEventHandler(Backspace_Click);
                        }
                        else
                        {
                            b = new KeyButton(upper, lower, button_width, button_height, margins_l[row] + margin_inc * (col - 1), row * button_height, 0, 0);
                            b.Click += new RoutedEventHandler(KeyHit_Click);
                        }
                    }

                    keys.Add(b);
                    window.QWERTYGrid.Children.Add(b);
                    ++index;
                }
                //first row has 10 every other row has 9
                if (row == 0)
                    --col_size;
            }

            //add spacebar
            KeyButton spacebar = new KeyButton(keyStrings[28].ToUpper(), keyStrings[28], 5 * button_width, button_height, (button_width / 2) * 5, button_height * 3, 0, 0);
            keys.Add(spacebar);
            window.QWERTYGrid.Children.Add(spacebar);
            spacebar.Click += new RoutedEventHandler(Space_Click);
        }

        private void toggleKeyButtons()
        {
            for (int i = 0; i < keys.Count(); ++i)
                keys[i].toggle();
        }

        private void KeyHit_Click(object sender, RoutedEventArgs e)
        {
            KeyButton letterKey = (KeyButton)sender;
            window.SETConsole.AppendText(letterKey.Content.ToString());
            window.FocusCaret();
            if (keys[19].Background == Brushes.MediumSpringGreen)
                keys[19].Background = Brushes.LightGray;
            if (shift)
            {
                shift = false;
                toggleKeyButtons();
            }
            window.FocusCaret();

        }

        private void Shift_Click(object sender, RoutedEventArgs e)
        {
            if (!shift)
            {
                keys[19].Background = Brushes.MediumSpringGreen;
                shift = true;
                toggleKeyButtons();
            }
            else
            {
                keys[19].Background = Brushes.LightGray;
                shift = false;
                toggleKeyButtons();
            }
            window.FocusCaret();
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            window.SETConsole.Undo();
            window.FocusCaret();
        }

        private void Space_Click(object sender, RoutedEventArgs e)
        {
            window.SETConsole.AppendText(" ");
            window.FocusCaret();
            if (keys[19].Background == Brushes.MediumSpringGreen)
                keys[19].Background = Brushes.LightGray;
        }
    }
}
