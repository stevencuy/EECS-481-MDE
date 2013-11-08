using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SETKeyboard.GUI
{
    class QwertyKeyboard
    {
        private MainWindow window;
        private String consoleText;
        private List<KeyButton> keys;
        private bool shift = false;
        public QwertyKeyboard(MainWindow window, double height, double width)
        {
            this.window = window;
            keys = new List<KeyButton>();

            string[] keyStrings = new string[31];
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
            keyStrings[28] = ",";
            keyStrings[29] = "space";
            keyStrings[30] = ".";

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

            //add lock button
            ToggleButton lockButton = new ToggleButton();
            lockButton.Height = button_height;
            lockButton.Width = (3 * button_width) / 2;
            lockButton.Margin = new Thickness(0, button_height * 3, 0, 0);
            lockButton.Content = "Lock";
            lockButton.VerticalAlignment = VerticalAlignment.Top;
            lockButton.HorizontalAlignment = HorizontalAlignment.Left;
            window.QWERTYGrid.Children.Add(lockButton);
            lockButton.Name = "lockButton";

            //add comma
            KeyButton comma = new KeyButton(keyStrings[28], keyStrings[28], button_width, button_height, (button_width / 2) * 3, button_height * 3, 0, 0);
            keys.Add(comma);
            window.QWERTYGrid.Children.Add(comma);
            comma.Click += new RoutedEventHandler(KeyHit_Click);

            //add spacebar
            KeyButton spacebar = new KeyButton(keyStrings[29].ToUpper(), keyStrings[29], 5 * button_width, button_height, (button_width / 2) * 5, button_height * 3, 0, 0);
            keys.Add(spacebar);
            window.QWERTYGrid.Children.Add(spacebar);
            spacebar.Click += new RoutedEventHandler(Space_Click);

            //add period
            KeyButton period = new KeyButton(keyStrings[30], keyStrings[30], button_width, button_height, (button_width / 2) * 15, button_height * 3, 0, 0);
            keys.Add(period);
            window.QWERTYGrid.Children.Add(period);
            period.Click += new RoutedEventHandler(KeyHit_Click);
        }

        private void toggleKeyButtons()
        {
            for (int i = 0; i < keys.Count(); ++i)
                keys[i].toggle();
        }

        private void KeyHit_Click(object sender, RoutedEventArgs e)
        {
            consoleText = window.getConsoleText();
            KeyButton letterKey = (KeyButton)sender;

            String letter = letterKey.Content.ToString();
            consoleText += letter;
            window.setConsoleText(consoleText);

            if (shift)
            {
                shift = false;
                toggleKeyButtons();
            }
        }

        private void Shift_Click(object sender, RoutedEventArgs e)
        {
            if (!shift)
            {
                shift = true;
                toggleKeyButtons();
            }
            else
            {
                shift = false;
                toggleKeyButtons();
            }
            window.FocusCaret();
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            consoleText = window.getConsoleText();
            if ((consoleText.Length == 1) || (consoleText.Length == 0))
            {
                consoleText = "";
            }
            else
            {
                //Removes last character of console text string
                consoleText = consoleText.Substring(0, consoleText.Length - 1);
            }
            window.setConsoleText(consoleText);
        }

        private void Space_Click(object sender, RoutedEventArgs e)
        {
            consoleText = window.getConsoleText();
            consoleText += " ";
            window.setConsoleText(consoleText);
        }
    }
}
