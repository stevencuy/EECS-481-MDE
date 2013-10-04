using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SETKeyboard.GUI
{
    public class QwertyKeyboard
    {
        private MainWindow window;
        private List<KeyButton> keys;
        private bool shift = false;

        public QwertyKeyboard(MainWindow window)
        {
            this.window = window;
            keys = new List<KeyButton>();

            keys.Add(new KeyButton("q", 100, 50, 0, 0, 0, 0));
            keys.Add(new KeyButton("w", 100, 50, 100, 0, 0, 0));
            keys.Add(new KeyButton("e", 100, 50, 200, 0, 0, 0));
            keys.Add(new KeyButton("r", 100, 50, 300, 0, 0, 0));
            keys.Add(new KeyButton("t", 100, 50, 400, 0, 0, 0));
            keys.Add(new KeyButton("y", 100, 50, 500, 0, 0, 0));
            keys.Add(new KeyButton("u", 100, 50, 600, 0, 0, 0));
            keys.Add(new KeyButton("i", 100, 50, 700, 0, 0, 0));
            keys.Add(new KeyButton("o", 100, 50, 800, 0, 0, 0));
            keys.Add(new KeyButton("p", 100, 50, 900, 0, 0, 0));

            keys.Add(new KeyButton("a", 100, 50, 50, 50, 0, 0));
            keys.Add(new KeyButton("s", 100, 50, 150, 50, 0, 0));
            keys.Add(new KeyButton("d", 100, 50, 250, 50, 0, 0));
            keys.Add(new KeyButton("f", 100, 50, 350, 50, 0, 0));
            keys.Add(new KeyButton("g", 100, 50, 450, 50, 0, 0));
            keys.Add(new KeyButton("h", 100, 50, 550, 50, 0, 0));
            keys.Add(new KeyButton("j", 100, 50, 650, 50, 0, 0));
            keys.Add(new KeyButton("k", 100, 50, 750, 50, 0, 0));
            keys.Add(new KeyButton("l", 100, 50, 850, 50, 0, 0));

            keys.Add(new KeyButton("z", 100, 50, 150, 100, 0, 0));
            keys.Add(new KeyButton("x", 100, 50, 250, 100, 0, 0));
            keys.Add(new KeyButton("c", 100, 50, 350, 100, 0, 0));
            keys.Add(new KeyButton("v", 100, 50, 450, 100, 0, 0));
            keys.Add(new KeyButton("b", 100, 50, 550, 100, 0, 0));
            keys.Add(new KeyButton("n", 100, 50, 650, 100, 0, 0));
            keys.Add(new KeyButton("m", 100, 50, 750, 100, 0, 0));

            foreach (KeyButton bt in keys)
            {
                bt.Click += new RoutedEventHandler(LetterClicked);
                window.QWERTYGrid.Children.Add(bt);
            }

            keys.Add(new KeyButton("Shift", 150, 50, 0, 100, 0, 0));
            keys.Add(new KeyButton("Back", 150, 50, 850, 100, 0, 0));
            keys.Add(new KeyButton("Space", 500, 50, 250, 150, 0, 0));

            keys[26].Click += new RoutedEventHandler(ShiftClicked);
            keys[27].Click += new RoutedEventHandler(BackspaceClicked);
            keys[28].Click += new RoutedEventHandler(SpaceClicked);

            window.QWERTYGrid.Children.Add(keys[26]);
            window.QWERTYGrid.Children.Add(keys[27]);
            window.QWERTYGrid.Children.Add(keys[28]);
        }

        private void LetterClicked(object sender, RoutedEventArgs e)
        {
            KeyButton letterKey = (KeyButton)sender;
            window.SETConsole.AppendText(letterKey.Content.ToString());
            if (shift) { ToggleShift(); }
            window.FocusCaret();
        }

        private void ShiftClicked(object sender, RoutedEventArgs e)
        {
            ToggleShift();
        }

        private void ToggleShift()
        {
            shift = !shift;
            keys[26].Background = (shift) ? Brushes.MediumSpringGreen : Brushes.LightGray;
            ToggleKeyButtons();
            window.FocusCaret();
        }

        private void BackspaceClicked(object sender, RoutedEventArgs e)
        {
            window.SETConsole.Undo();
            window.FocusCaret();
        }

        private void SpaceClicked(object sender, RoutedEventArgs e)
        {
            window.SETConsole.AppendText(" ");
            window.FocusCaret();
            if (shift) { ToggleShift(); }
        }

        private void ToggleKeyButtons()
        {
            for (int i = 0; i < 26; ++i)
            {
                keys[i].Toggle();
            }
        }
    }
}
