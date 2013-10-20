using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace SETKeyboard.GUI
{
    class T9Keyboard
    {
        private MainWindow window;
        T9LetterButton lastButtonPressed;
        String consoleText;
        bool isLowerCase;
        List<T9LetterButton> letterButtons;

        public T9Keyboard(MainWindow window)
        {
            this.window = window;
            isLowerCase = true;
            consoleText = window.getConsoleText();

            letterButtons = new List<T9LetterButton>();
            window.abcButton.setLetters(new char[] { 'a', 'b', 'c' });
            window.abcButton.Click += new RoutedEventHandler(T9LetterClickEvent);
            letterButtons.Add(window.abcButton);
            window.defButton.setLetters(new char[] { 'd', 'e', 'f' });
            window.defButton.Click += new RoutedEventHandler(T9LetterClickEvent);
            letterButtons.Add(window.defButton);
            window.ghiButton.setLetters(new char[] { 'g', 'h', 'i' });
            window.ghiButton.Click += new RoutedEventHandler(T9LetterClickEvent);
            letterButtons.Add(window.ghiButton);
            window.jklButton.setLetters(new char[] { 'j', 'k', 'l' });
            window.jklButton.Click += new RoutedEventHandler(T9LetterClickEvent);
            letterButtons.Add(window.jklButton);
            window.mnoButton.setLetters(new char[] { 'm', 'n', 'o' });
            window.mnoButton.Click += new RoutedEventHandler(T9LetterClickEvent);
            letterButtons.Add(window.mnoButton);
            window.pqrsButton.setLetters(new char[] { 'p', 'q', 'r', 's' });
            window.pqrsButton.Click += new RoutedEventHandler(T9LetterClickEvent);
            letterButtons.Add(window.pqrsButton);
            window.tuvButton.setLetters(new char[] { 't', 'u', 'v' });
            window.tuvButton.Click += new RoutedEventHandler(T9LetterClickEvent);
            letterButtons.Add(window.tuvButton);
            window.wxyzButton.setLetters(new char[] { 'w', 'x', 'y', 'z' });
            window.wxyzButton.Click += new RoutedEventHandler(T9LetterClickEvent);
            letterButtons.Add(window.wxyzButton);

            window.NextLetterButton.Click += new RoutedEventHandler(nextLetterEvent);
            window.ShiftButton.Click += new RoutedEventHandler(toUpperClick);
            window.BackButton.Click += new RoutedEventHandler(backSpaceClick);
            window.SpaceButton.Click += new RoutedEventHandler(spaceClick);
            window.PeriodButton.Click += new RoutedEventHandler(periodClick);

            lastButtonPressed = window.abcButton;
        }

        private void toUpperClick(object sender, RoutedEventArgs e)
        {
            if (isLowerCase)
            {
                toUpperCase();
            }
            else
            {
                toLowerCase();
            }
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
        }

        private void T9LetterClickEvent(object sender, RoutedEventArgs e)
        {
            consoleText = window.getConsoleText();
            T9LetterButton buttonPressed = (SETKeyboard.GUI.T9LetterButton)sender;

            char letter;
            if (!sender.Equals(lastButtonPressed))
            {
                lastButtonPressed.endSelection();
                lastButtonPressed = buttonPressed;
                letter = buttonPressed.getCurrent(isLowerCase);
                consoleText += letter;

                if (isLowerCase == false)
                {
                    toLowerCase();
                }
            }
            else
            {
                lastButtonPressed = buttonPressed;
                letter = buttonPressed.getCurrent(isLowerCase);
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

            window.setConsoleText(consoleText);
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
            lastButtonPressed.endSelection();
            lastButtonPressed = new T9LetterButton();
            window.FocusCaret();
        }

        private void backSpaceClick(object sender, RoutedEventArgs e)
        {
            lastButtonPressed.endSelection();
            lastButtonPressed = new T9LetterButton();
            consoleText = window.getConsoleText();
            this.backSpaceAction();
            window.setConsoleText(consoleText);
        }

        private void spaceClick(object sender, RoutedEventArgs e)
        {
            consoleText = window.getConsoleText();
            consoleText = consoleText + " ";
            window.setConsoleText(consoleText);
        }

        private void periodClick(object sender, RoutedEventArgs e)
        {
            lastButtonPressed.endSelection();
            lastButtonPressed = new T9LetterButton();
            consoleText = window.getConsoleText();
            consoleText = consoleText + ". ";
            window.setConsoleText(consoleText);
            toUpperCase();
        }


    }
}
