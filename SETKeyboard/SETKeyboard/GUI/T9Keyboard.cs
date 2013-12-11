using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace SETKeyboard.GUI
{
    class T9Keyboard
    {
        private MainWindow window;
        List<T9LetterButton> letterButtons;
        T9LetterButton lastButtonPressed;
        String consoleText;
        bool isLowerCase;
        bool justShifted = false;
        bool locked = false;

        private DispatcherTimer timer;
        private DispatcherTimer confirmTimer;
        private int dwellTime;
        private SolidColorBrush backColor;
        private SolidColorBrush selectColor;
        private SolidColorBrush hoverColor;

        public T9Keyboard(MainWindow window)
        {
            this.window = window;
            dwellTime = window.getDwellTime();
            backColor = window.getBackColor();
            selectColor = window.getSelectColor();
            hoverColor = window.getHoverColor();

            isLowerCase = true;
            consoleText = window.getConsoleText();

            letterButtons = new List<T9LetterButton>();
            window.abcButton.setLetters(new char[] { 'a', 'b', 'c' });
            window.defButton.setLetters(new char[] { 'd', 'e', 'f' });
            window.ghiButton.setLetters(new char[] { 'g', 'h', 'i' });
            window.jklButton.setLetters(new char[] { 'j', 'k', 'l' });
            window.mnoButton.setLetters(new char[] { 'm', 'n', 'o' });
            window.pqrsButton.setLetters(new char[] { 'p', 'q', 'r', 's' });
            window.tuvButton.setLetters(new char[] { 't', 'u', 'v' });
            window.wxyzButton.setLetters(new char[] { 'w', 'x', 'y', 'z' });

            letterButtons.Add(window.abcButton);
            letterButtons.Add(window.defButton);
            letterButtons.Add(window.ghiButton);
            letterButtons.Add(window.jklButton);
            letterButtons.Add(window.mnoButton);
            letterButtons.Add(window.pqrsButton);
            letterButtons.Add(window.tuvButton);
            letterButtons.Add(window.wxyzButton);

            window.abcButton.MouseEnter += new MouseEventHandler(T9LetterClickEvent);
            window.defButton.MouseEnter += new MouseEventHandler(T9LetterClickEvent);
            window.ghiButton.MouseEnter += new MouseEventHandler(T9LetterClickEvent);
            window.jklButton.MouseEnter += new MouseEventHandler(T9LetterClickEvent);
            window.mnoButton.MouseEnter += new MouseEventHandler(T9LetterClickEvent);
            window.pqrsButton.MouseEnter += new MouseEventHandler(T9LetterClickEvent);
            window.tuvButton.MouseEnter += new MouseEventHandler(T9LetterClickEvent);
            window.wxyzButton.MouseEnter += new MouseEventHandler(T9LetterClickEvent);

            window.NextLetterButton.MouseEnter += new MouseEventHandler(nextLetterEvent);
            window.ShiftButton.MouseEnter += new MouseEventHandler(toUpperClick);
            window.BackButton.MouseEnter += new MouseEventHandler(backSpaceClick);
            window.SpaceButton.MouseEnter += new MouseEventHandler(spaceClick);
            window.LockButtonLeft.MouseEnter += new MouseEventHandler(lockClick);
            window.PeriodButton.MouseEnter += new MouseEventHandler(periodClick);
            window.CommaButton.MouseEnter += new MouseEventHandler(commaClick);

            lastButtonPressed = new T9LetterButton();

            window.abcButton.Background = backColor;
            window.defButton.Background = backColor;
            window.ghiButton.Background = backColor;
            window.jklButton.Background = backColor;
            window.mnoButton.Background = backColor;
            window.pqrsButton.Background = backColor;
            window.tuvButton.Background = backColor;
            window.wxyzButton.Background = backColor;
            window.NextLetterButton.Background = backColor;
            window.ShiftButton.Background = backColor;
            window.BackButton.Background = backColor;
            window.SpaceButton.Background = backColor;
            window.LockButtonLeft.Background = backColor;
            window.PeriodButton.Background = backColor;
            window.CommaButton.Background = backColor;
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

        private void toUpperClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            Button shiftButton = (Button)sender;

            if (shiftButton.Background != selectColor)
                shiftButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(shiftButton);

                if (isLowerCase)
                {
                    toUpperCase();
                    justShifted = true;
                }
                else
                {
                    toLowerCase();
                }

                toUpperClick(sender, e);
            };

            shiftButton.MouseLeave += (s, eA) =>
            {
                shiftButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();      
        }

        private void toUpperCase()
        {
            for (int i = 0; i < 8; i++)
            {
                String contentText = letterButtons.ElementAt(i).Content.ToString();
                contentText = contentText.ToUpper();
                letterButtons.ElementAt(i).Content = contentText;
                isLowerCase = false;
            }

            window.BackButton.Content = "BACK";
            window.SpaceButton.Content = "SPACE";
            window.ShiftButton.Content = "SHIFT";
            window.LockButtonLeft.Content = "LOCK";
        }

        private void toLowerCase()
        {
            for (int i = 0; i < 8; i++)
            {
                String contentText = letterButtons.ElementAt(i).Content.ToString();
                contentText = contentText.ToLower();
                letterButtons.ElementAt(i).Content = contentText;
                isLowerCase = true;
            }

            window.BackButton.Content = "back";
            window.SpaceButton.Content = "space";
            window.ShiftButton.Content = "shift";
            window.LockButtonLeft.Content = "lock";
        }

        private void T9LetterClickEvent(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            T9LetterButton buttonPressed = (T9LetterButton)sender;

            if (buttonPressed.Background != selectColor)
                buttonPressed.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(buttonPressed);

                consoleText = window.getConsoleText();
                int consoleTextSize = consoleText.Length;

                if (!lastButtonPressed.Equals(buttonPressed))
                {
                    lastButtonPressed.endSelection();
                    if (justShifted == true)
                    {
                        justShifted = false;
                    }
                    else if (!isLowerCase)
                    {
                        toLowerCase();
                    }
                }

                char letter = buttonPressed.getCurrent(isLowerCase);
                buttonPressed.Content = letter;

                if (lastButtonPressed.Equals(buttonPressed))
                {
                    if (consoleText == "")
                    {
                        consoleText = letter.ToString();
                    }
                    else
                    {
                        this.backSpaceAction();
                        consoleText += letter;
                    }
                }
                else
                {
                    consoleText += letter;
                }

                lastButtonPressed = buttonPressed;
                window.setConsoleText(consoleText);

                T9LetterClickEvent(sender, e);
            };

            buttonPressed.MouseLeave += (s, eA) =>
            {
                buttonPressed.Background = backColor;
                buttonPressed.Content = buttonPressed.getOriginalContent(!isLowerCase);
                timer.Stop();
            };
 
            timer.Start();            
        }

        private void backSpaceAction()
        {
            if ((consoleText.Length == 1) || (consoleText.Length == 0))
            {
                consoleText = "";
            }
            else
            {
                consoleText = consoleText.Substring(0, consoleText.Length - 1);
            }
        }

        private void nextLetterEvent(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            T9LetterButton nextButton = (T9LetterButton)sender;

            if (nextButton.Background != selectColor)
                nextButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(nextButton);

                lastButtonPressed.endSelection();
                lastButtonPressed = new T9LetterButton();
                window.FocusCaret();

                nextLetterEvent(sender, e);
            };

            nextButton.MouseLeave += (s, eA) =>
            {
                nextButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();            
        }

        private void backSpaceClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            Button backButton = (Button)sender;

            if (backButton.Background != selectColor)
                backButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(backButton);

                lastButtonPressed.endSelection();
                lastButtonPressed = new T9LetterButton();
                consoleText = window.getConsoleText();
                this.backSpaceAction();
                window.setConsoleText(consoleText);

                backSpaceClick(sender, e);
            };

            backButton.MouseLeave += (s, eA) =>
            {
                backButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();            
        }

        private void spaceClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            Button spaceButton = (Button)sender;

            if (spaceButton.Background != selectColor)
                spaceButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                consoleText = window.getConsoleText();
                consoleText = consoleText + " ";
                window.setConsoleText(consoleText);

                highlight(spaceButton);

                spaceClick(sender, e);
            };

            spaceButton.MouseLeave += (s, eA) =>
            {
                spaceButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();            
        }

        private void lockClick(object sender, RoutedEventArgs e)
        {

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            ToggleButton lockButton = (ToggleButton)sender;

            if (lockButton.Background != selectColor)
                lockButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                if (!locked)
                {
                    window.LockButtonLeft.Background = selectColor;

                    locked = true;
                    for (int i = 0; i < 8; i++)
                    {
                        letterButtons[i].MouseEnter -= T9LetterClickEvent;
                    }

                    window.NextLetterButton.MouseEnter -= nextLetterEvent;
                    window.ShiftButton.MouseEnter -= toUpperClick;
                    window.BackButton.MouseEnter -= backSpaceClick;
                    window.SpaceButton.MouseEnter -= spaceClick;
                    window.PeriodButton.MouseEnter -= periodClick;
                    window.CommaButton.MouseEnter -= commaClick;
                }
                else
                {
                    window.LockButtonLeft.Background = hoverColor;

                    locked = false;
                    for (int i = 0; i < 8; i++)
                    {
                        letterButtons[i].MouseEnter += T9LetterClickEvent;
                    }

                    window.NextLetterButton.MouseEnter += nextLetterEvent;
                    window.ShiftButton.MouseEnter += toUpperClick;
                    window.BackButton.MouseEnter += backSpaceClick;
                    window.SpaceButton.MouseEnter += spaceClick;
                    window.PeriodButton.MouseEnter += periodClick;
                    window.CommaButton.MouseEnter += commaClick;
                }

                lockClick(sender, e);
            };

            lockButton.MouseLeave += (s, eA) =>
            {
                timer.Stop();

                if (lockButton.Background == hoverColor)
                    window.LockButtonLeft.Background = backColor;
            };

            timer.Start();            

        }

        private void periodClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            Button periodButton = (Button)sender;

            if (periodButton.Background != selectColor)
                periodButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(periodButton);

                lastButtonPressed.endSelection();
                lastButtonPressed = new T9LetterButton();
                consoleText = window.getConsoleText();
                consoleText = consoleText + ". ";
                window.setConsoleText(consoleText);
                justShifted = true;
                toUpperCase();

                periodClick(sender, e);
            };

            periodButton.MouseLeave += (s, eA) =>
            {
                periodButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();            
        }

        private void commaClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            Button commaButton = (Button)sender;

            if (commaButton.Background != selectColor)
                commaButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                consoleText = window.getConsoleText();
                consoleText = consoleText + ", ";
                window.setConsoleText(consoleText);

                highlight(commaButton);

                commaClick(sender, e);
            };

            commaButton.MouseLeave += (s, eA) =>
            {
                commaButton.Background = backColor;
                timer.Stop();
            };

            timer.Start();            
        }
    }
}
