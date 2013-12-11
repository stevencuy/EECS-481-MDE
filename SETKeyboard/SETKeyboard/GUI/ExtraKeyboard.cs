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
    class ExtraKeyboard
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

        public ExtraKeyboard(MainWindow window, double height, double width)
        {
            this.window = window;
            dwellTime = window.getDwellTime();
            backColor = window.getBackColor();
            selectColor = window.getSelectColor();
            hoverColor = window.getHoverColor();

            keys = new List<KeyButton>();

            string[] keyStrings = new string[24];
            //ROW 0
            keyStrings[0] = "0!";
            keyStrings[1] = "1@";
            keyStrings[2] = "2#";
            keyStrings[3] = "3$";
            keyStrings[4] = "4%";
            keyStrings[5] = "5^";
            keyStrings[6] = "6&";
            keyStrings[7] = "7*";
            keyStrings[8] = "8(";
            keyStrings[9] = "9)";
            //ROW 1
            keyStrings[10] = "-_";
            keyStrings[11] = "=+";
            keyStrings[12] = "[{";
            keyStrings[13] = "]}";
            keyStrings[14] = "|\\";
            keyStrings[15] = ";:";
            keyStrings[16] = "'\"";
            keyStrings[17] = ",<";
            keyStrings[18] = ".>";
            keyStrings[19] = "/?";

            //ROW 2
            keyStrings[20] = "more";
            keyStrings[21] = "space";
            keyStrings[22] = "back";
            keyStrings[23] = "clear";

            const int row_size = 2;
            int col_size = 10;
            int button_width = (int)(width / 10) - 1, button_height = (int)(height / 4) - 28;
            int margin_inc = button_width;
            int[] margins_l = new int[3];
            margins_l[0] = 0;
            margins_l[1] = 0;
            margins_l[2] = (button_width / 2) * 3;

            int index = 0;

            for (int row = 0; row < row_size; ++row)
            {
                for (int col = 0; col < col_size; ++col)
                {
                    string lower = keyStrings[index][0].ToString();
                    string upper = keyStrings[index][1].ToString();
                    KeyButton b;
                    b = new KeyButton(upper, lower, button_width, (3 * button_height)/2 , margins_l[row] + margin_inc * col, row * ( 3 * button_height) /2 , 0, 0, backColor);
                    b.MouseEnter += new MouseEventHandler(KeyHit_Click);
                    keys.Add(b);
                    window.extra_grid.Children.Add(b);
                    ++index;
                }
            }

            int bh = (3 * button_height) / 2;
            //add lock button
            KeyButton lockButton = new KeyButton("lock", "lock", (2 * button_width), button_height, 0, bh * 2, 0, 0, backColor);
            window.extra_grid.Children.Add(lockButton);
            lockButton.MouseEnter += new MouseEventHandler(lockButtons_Click);
            

            //add more options
            KeyButton options = new KeyButton(keyStrings[20], keyStrings[20], 2 * button_width, button_height, 2 * button_width, bh * 2, 0, 0, backColor);
            keys.Add(options);
            window.extra_grid.Children.Add(options);
            options.MouseEnter += new MouseEventHandler(Options_Click);


            //add spacebar
            KeyButton spacebar = new KeyButton(keyStrings[21], keyStrings[21], 2 * button_width, button_height, 4 * button_width, bh * 2, 0, 0, backColor);
            keys.Add(spacebar);
            window.extra_grid.Children.Add(spacebar);
            spacebar.MouseEnter += new MouseEventHandler(Space_Click);
            
            //add back
            KeyButton back = new KeyButton(keyStrings[22], keyStrings[22], 2 * button_width, button_height, 6 * button_width, bh * 2, 0, 0, backColor);
            keys.Add(back);
            window.extra_grid.Children.Add(back);
            back.MouseEnter += new MouseEventHandler(Backspace_Click);

            //add clear button
            KeyButton clear = new KeyButton(keyStrings[23], keyStrings[23], 2 * button_width, button_height, 8 * button_width , bh * 2, 0, 0, backColor);
            keys.Add(clear);
            window.extra_grid.Children.Add(clear);
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

        public void updateEvents()
        {
            dwellTime = window.getDwellTime();
            backColor = window.getBackColor();
            selectColor = window.getSelectColor();
            hoverColor = window.getHoverColor();

            for (int i = 0; i < keys.Count() - 1; ++i)
                keys[i].Background = backColor;
        }

        private void toggleKeyButtons()
        {
            for (int i = 0; i < keys.Count(); ++i)
                keys[i].toggle();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            KeyButton optionsButton = (KeyButton)sender;

            if (optionsButton.Background != selectColor)
                optionsButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(optionsButton);

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

                Options_Click(sender, e);
            };

            optionsButton.MouseLeave += (s, eA) =>
            {
                optionsButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();
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
                        keys[i].MouseEnter -= Options_Click;
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
                        if (keys[i].getLower() == "back")
                            keys[i].MouseEnter += Backspace_Click;
                        else if (keys[i].getLower() == "space")
                            keys[i].MouseEnter += Space_Click;
                        else if (keys[i].getLower() == "clear")
                            keys[i].MouseEnter += Clear_Click;
                        else if (keys[i].getLower() == "more")
                            keys[i].MouseEnter += Options_Click;
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

                /*if (letter == ",")
                    letter += " ";
                else if (letter == ".")
                {
                    letter += " ";
                    if (!shift)
                    {
                        shift = true;
                        toggleKeyButtons();
                    }
                }*/

                consoleText += letter;
                window.setConsoleText(consoleText);

                /*if (shift && letter != ", " && letter != ". ")
                {
                    shift = false;
                    toggleKeyButtons();
                }*/

                KeyHit_Click(sender, e);
            };

            keyButton.MouseLeave += (s, eA) =>
            {
                keyButton.Background = backColor;
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
