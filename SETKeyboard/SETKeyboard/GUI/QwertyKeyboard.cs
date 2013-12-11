using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SETKeyboard.GUI
{
    class QwertyKeyboard
    {
        private MainWindow window;
        private String consoleText;
        private List<KeyButton> keys;
        private bool shift = false;
        private bool locked = false;

        private DispatcherTimer timer;
        private DispatcherTimer confirmTimer;
        private int dwellTime;
        private SolidColorBrush selectColor;
        private SolidColorBrush hoverColor;
        private SolidColorBrush backColor;

        public QwertyKeyboard(MainWindow window, double height, double width)
        {
            this.window = window;
            dwellTime = window.getDwellTime();
            backColor = window.getBackColor();
            selectColor = window.getSelectColor();
            hoverColor = window.getHoverColor();

            keys = new List<KeyButton>();

            string[] keyStrings = new string[32];
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
            keyStrings[31] = "clear";

            const int row_size = 3;
            int col_size = 10;
            int button_width = (int)(width / 10) - 1, button_height = (int)(height / 4) - 28;
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
                        b = new KeyButton(upper, lower, button_width, button_height, margins_l[row] + margin_inc * col, row * button_height, 0, 0, backColor);
                        b.MouseEnter += new MouseEventHandler(KeyHit_Click);
                    }
                    else
                    {
                        if (lower == "shift")
                        {
                            b = new KeyButton(upper, lower, (button_width / 2) * 3, button_height, 0, row * button_height, 0, 0, backColor);
                            b.MouseEnter += new MouseEventHandler(Shift_Click);
                        }
                        else if (lower == "back")
                        {
                            b = new KeyButton(upper, lower, (button_width / 2) * 3, button_height, margins_l[row] + margin_inc * (col - 1), row * button_height, 0, 0, backColor);
                            b.MouseEnter += new MouseEventHandler(Backspace_Click);
                        }
                        else
                        {
                            b = new KeyButton(upper, lower, button_width, button_height, margins_l[row] + margin_inc * (col - 1), row * button_height, 0, 0, backColor);
                            b.MouseEnter += new MouseEventHandler(KeyHit_Click);
                        }
                    }

                    keys.Add(b);
                    window.qwerty_grid.Children.Add(b);
                    ++index;
                }
                //first row has 10 every other row has 9
                if (row == 0)
                    --col_size;
            }

            //add lock button
            KeyButton lockButton = new KeyButton("LOCK", "lock", (3 * button_width) / 2, button_height, 0, button_height * 3, 0, 0, backColor);
            window.qwerty_grid.Children.Add(lockButton);
            lockButton.MouseEnter += new MouseEventHandler(lockButtons_Click);

            //add comma
            KeyButton comma = new KeyButton(keyStrings[28], keyStrings[28], button_width, button_height, (button_width / 2) * 3, button_height * 3, 0, 0, backColor);
            keys.Add(comma);
            window.qwerty_grid.Children.Add(comma);
            comma.MouseEnter += new MouseEventHandler(KeyHit_Click);

            //add spacebar
            KeyButton spacebar = new KeyButton(keyStrings[29].ToUpper(), keyStrings[29], 5 * button_width, button_height, (button_width / 2) * 5, button_height * 3, 0, 0, backColor);
            keys.Add(spacebar);
            window.qwerty_grid.Children.Add(spacebar);
            spacebar.MouseEnter += new MouseEventHandler(Space_Click);

            //add period
            KeyButton period = new KeyButton(keyStrings[30], keyStrings[30], button_width, button_height, (button_width / 2) * 15, button_height * 3, 0, 0, backColor);
            keys.Add(period);
            window.qwerty_grid.Children.Add(period);
            period.MouseEnter += new MouseEventHandler(KeyHit_Click);

            //add clear button
            KeyButton clear = new KeyButton(keyStrings[31].ToUpper(), keyStrings[31], (button_width / 2) * 3, button_height, (button_width / 2) * 17, button_height * 3, 0, 0, backColor);
            keys.Add(clear);
            window.qwerty_grid.Children.Add(clear);
            clear.MouseEnter += new MouseEventHandler(Clear_Click);

            keys.Add(lockButton);
        }

        private void highlight(ButtonBase button)
        {
            confirmTimer = new DispatcherTimer();
            button.Background = selectColor;
            confirmTimer.Interval = TimeSpan.FromMilliseconds(350);
            confirmTimer.Start();
            confirmTimer.Tick += (se, eAr) =>
            {
                confirmTimer.Stop();
                
                if (button.Background == selectColor)
                    button.Background = hoverColor;
            };
        }

        private void toggleKeyButtons()
        {
            for (int i = 0; i < keys.Count(); ++i)
                keys[i].toggle();
        }

        private void lockButtons_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            KeyButton lockButton = (KeyButton)sender;

            if (lockButton.Background != selectColor)
                lockButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                if (!locked)
                {
                    locked = true;
                    lockButton.Background = selectColor;

                    for (int i = 0; i < keys.Count() - 1; ++i)
                    {
                        keys[i].MouseEnter -= KeyHit_Click;
                        keys[i].MouseEnter -= Shift_Click;
                        keys[i].MouseEnter -= Backspace_Click;
                        keys[i].MouseEnter -= Space_Click;
                        keys[i].MouseEnter -= Clear_Click;
                    }
                }
                else
                {
                    locked = false;
                    lockButton.Background = hoverColor;

                    for (int i = 0; i < keys.Count() - 1; ++i)
                    {
                        if (keys[i].getLower() == "shift")
                            keys[i].MouseEnter += Shift_Click;
                        else if (keys[i].getLower() == "back")
                            keys[i].MouseEnter += Backspace_Click;
                        else if (keys[i].getLower() == "space")
                            keys[i].MouseEnter += Space_Click;
                        else if (keys[i].getLower() == "clear")
                            keys[i].MouseEnter += Clear_Click;
                        else
                            keys[i].MouseEnter += KeyHit_Click;
                    }
                }

                lockButtons_Click(sender, e);
            };

            lockButton.MouseLeave += (s, eA) =>
            {
                timer.Stop();

                if (lockButton.Background == hoverColor)
                    lockButton.Background = backColor;
            };

            timer.Start();
        }

        private void KeyHit_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            KeyButton keyButton = (KeyButton)sender;

            if (keyButton.Background != selectColor)
                keyButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(keyButton);

                consoleText = window.getConsoleText();
                String letter = keyButton.Content.ToString();

                if (letter == ",")
                    letter += " ";
                else if (letter == ".")
                {
                    letter += " ";
                    if (!shift)
                    {
                        shift = true;
                        toggleKeyButtons();
                    }
                }

                consoleText += letter;
                window.setConsoleText(consoleText);

                if (shift && letter != ", " && letter != ". ")
                {
                    shift = false;
                    toggleKeyButtons();
                }

                KeyHit_Click(sender, e);
            };

            keyButton.MouseLeave += (s, eA) =>
            {
                keyButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();            
        }

        private void Shift_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            KeyButton shiftButton = (KeyButton)sender;

            if (shiftButton.Background != selectColor)
                shiftButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(shiftButton);

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

                Shift_Click(sender, e);
            };

            shiftButton.MouseLeave += (s, eA) =>
            {
                shiftButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();        
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            KeyButton backButton = (KeyButton)sender;

            if (backButton.Background != selectColor)
                backButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(backButton);

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
                //window.TabPanel.Items.Remove()
                //window.TabPanel.Items.Remove(window.dictionary["new tab 2"]);

                Backspace_Click(sender, e);
            };

            backButton.MouseLeave += (s, eA) =>
            {
                backButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();        
            
        }

        private void Space_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            KeyButton spaceButton = (KeyButton)sender;

            if (spaceButton.Background != selectColor)
                spaceButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(spaceButton);

                consoleText = window.getConsoleText();
                consoleText += " ";
                window.setConsoleText(consoleText);

                Space_Click(sender, e);
            };

            spaceButton.MouseLeave += (s, eA) =>
            {
                spaceButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();        
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            KeyButton clearButton = (KeyButton)sender;

            if (clearButton.Background != selectColor)
                clearButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(clearButton);

                window.setConsoleText("");

                Clear_Click(sender, e);
            };

            clearButton.MouseLeave += (s, eA) =>
            {
                clearButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();
        }
    }
}
