using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SETKeyboard.GUI
{
    public class CustomTab
    {
        private MainWindow window;
        private String consoleText;
        private String name;
        public int width;
        public int height;
        private int type = 0;

        private DispatcherTimer timer;
        private DispatcherTimer confirmTimer;
        private int dwellTime;
        private SolidColorBrush backColor;
        private SolidColorBrush selectColor;
        private SolidColorBrush hoverColor;
       
        private Dictionary<String, TabPhrase> phrases;
        public CustomTab(MainWindow window, String name_, double height, double width)
        {
            this.window = window;
            dwellTime = window.getDwellTime();
            backColor = window.getBackColor();
            selectColor = window.getSelectColor();
            hoverColor = window.getHoverColor();

            consoleText = window.getConsoleText();
            name = name_;
            this.width = (int)width;
            this.height = (int)height;
            phrases = new Dictionary<String, TabPhrase>();
            type = 0;
            renderTab(0);
        }

        private void renderTab(int type)
        {
            renderPhrases(type);
            renderControl(type);
        }

        private void renderPhrases(int type)
        {
            phrases.Clear();
            List<String> phraseStrings;
            if (window.tabPhrases.ContainsKey(name))
            {
                phraseStrings = window.tabPhrases[name].ToList();
                phraseStrings.Sort();
            }
            else
            {
                phraseStrings = new List<String>();
                window.tabPhrases.Add(name, new HashSet<String>());
            }

            if (!window.ctab_grids.ContainsKey(name))
            {
                window.assignGrid(name);
            }

            window.ctab_grids[name].Children.Clear();

            const int strlen_to_width_conversion = 12;
            int total_width = (int)(width);
            int button_height = (int)(height / 4) - 10;
            int button_height_margin = (int)(height / 4);
            int width_so_far = 0;
            int row = 0;

            for (int i = 0; i < phraseStrings.Count(); ++i)
            {
                TabPhrase tb;
                int button_width = 50 + phraseStrings[i].Length * strlen_to_width_conversion;
                int margin_left = (i > 0) ? width_so_far + 50 + phraseStrings[i - 1].Length * strlen_to_width_conversion + 10 : 0;

                if (i > 0 && width_so_far + margin_left > total_width - 10)
                {
                    ++row;
                    margin_left = 0;
                }

                width_so_far = margin_left;
                tb = new TabPhrase(phraseStrings[i], button_width, button_height, margin_left, button_height_margin + row * button_height_margin, 0, 0, backColor);
                assignEventHandler(type, tb);
                phrases.Add(phraseStrings[i], tb);
                window.ctab_grids[name].Children.Add(tb);
            }
        }

        public void updateEvents()
        {
            dwellTime = window.getDwellTime();
            backColor = window.getBackColor();
            selectColor = window.getSelectColor();
            hoverColor = window.getHoverColor();
            renderTab(type);
        }

        private void assignEventHandler(int type, TabPhrase tb)
        {
            if (type < 2)
                tb.MouseEnter += new MouseEventHandler(UseClick);
            else
                tb.MouseEnter += new MouseEventHandler(RemoveClick);
        }

        private void renderControl(int type)
        {
            TabPhrase create, remove, normal, clear, backspace;
            int button_height = (int)(height/4)-10;
            int button_width = (width - 30) / 4;
            var converter = new System.Windows.Media.BrushConverter();

            create = new TabPhrase("Add Phrase", button_width, button_height, 0, 0, 0, 0, backColor);
            remove = new TabPhrase("Erase Phrase", button_width, button_height, button_width + 10, 0, 0, 0, backColor);
            normal = new TabPhrase("Use Phrases", button_width, button_height, 2 * (button_width + 10), 0, 0, 0, backColor);
            clear = new TabPhrase("Clear", button_width, button_height, 3 * (button_width + 10), 0, 0, 0, backColor);
            backspace = new TabPhrase("Backspace", button_width, button_height, 4 * (button_width + 10), 0, 0, 0, backColor);

            if (type == 1)
                create.Background = selectColor;
            else if (type == 0)
                normal.Background = selectColor;
            else
                remove.Background = selectColor;

            create.MouseEnter += new MouseEventHandler(UseInsertClick);
            normal.MouseEnter += new MouseEventHandler(UseNormalClick);
            remove.MouseEnter += new MouseEventHandler(UseRemoveClick);
            clear.MouseEnter += new MouseEventHandler(ClearConsoleClick);
            backspace.MouseEnter += new MouseEventHandler(Backspace_Click);

            window.ctab_grids[name].Children.Add(create);
            window.ctab_grids[name].Children.Add(normal);
            window.ctab_grids[name].Children.Add(remove);
            window.ctab_grids[name].Children.Add(clear);
            window.ctab_grids[name].Children.Add(backspace);
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

        private void UseNormalClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabPhrase button = (TabPhrase)sender;

            if (button.Background != selectColor)
                button.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(button);
                type = 0;
                renderTab(0);

                UseNormalClick(sender, e);
            };

            button.MouseLeave += (s, eA) =>
            {
                timer.Stop();

                if (button.Background != selectColor)
                    button.Background = backColor;
            };

            timer.Start();  
        }

        private void ClearConsoleClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabPhrase button = (TabPhrase)sender;

            if (button.Background != selectColor)
                button.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(button);

                window.setConsoleText("");

                ClearConsoleClick(sender, e);
            };

            button.MouseLeave += (s, eA) =>
            {
                button.Background = backColor;
                timer.Stop();
            };

            timer.Start();
        }
        
        private void UseInsertClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabPhrase button = (TabPhrase)sender;

            if (button.Background != selectColor)
                button.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(button);

                consoleText = window.getConsoleText();
                if (consoleText.Length != 0 && !window.tabPhrases[name].Contains(consoleText))
                    window.tabPhrases[name].Add(consoleText);
                type = 1;
                renderTab(1);

                UseInsertClick(sender, e);
            };

            button.MouseLeave += (s, eA) =>
            {
                timer.Stop();

                if (button.Background != selectColor)
                    button.Background = backColor;
            };

            timer.Start();  
        }

        private void UseRemoveClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabPhrase button = (TabPhrase)sender;

            if (button.Background != selectColor)
                button.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(button);
                type = 2;
                renderTab(2);

                UseRemoveClick(sender, e);
            };

            button.MouseLeave += (s, eA) =>
            {
                timer.Stop();

                if (button.Background != selectColor)
                    button.Background = backColor;
            };

            timer.Start();  
        }

        private void UseClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabPhrase button = (TabPhrase)sender;

            if (button.Background != selectColor)
                button.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(button);

                consoleText = window.getConsoleText();
                TabPhrase chosenPhrase = (TabPhrase)sender;
                String phrase = chosenPhrase.Content.ToString();
                if (consoleText.Length > 0)
                    consoleText += " ";
                consoleText += phrase;
                window.setConsoleText(consoleText);

                UseClick(sender, e);
            };

            button.MouseLeave += (s, eA) =>
            {
                timer.Stop();
                button.Background = backColor;
            };

            timer.Start();  
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabPhrase button = (TabPhrase)sender;

            if (button.Background != selectColor)
                button.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(button);

                TabPhrase deletePhrase = (TabPhrase)sender;
                String phrase = deletePhrase.Content.ToString();

                /*
                if (MessageBox.Show("Are you sure that you want to remove the tab phrase \"" + phrase + "\"?",  "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    window.tabPhrases[name].Remove(phrase);
                }
                */

                window.tabPhrases[name].Remove(phrase);
                type = 2;
                renderTab(2);

                RemoveClick(sender, e);
            };

            button.MouseLeave += (s, eA) =>
            {
                timer.Stop();

                if (button.Background != selectColor)
                    button.Background = backColor;
            };

            timer.Start();  
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabPhrase button = (TabPhrase)sender;

            if (button.Background != selectColor)
                button.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(button);

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

            button.MouseLeave += (s, eA) =>
            {
                button.Background = backColor;
                timer.Stop();
            };

            timer.Start();  
        }
    }
}
