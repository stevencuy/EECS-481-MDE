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
    class CustomTabController
    {
        public MainWindow window;
        public String consoleText;
        public int width;
        public int height;
        private int type = 0;
        private DispatcherTimer timer;
        private DispatcherTimer confirmTimer;
        private int dwellTime;
        private SolidColorBrush backColor;
        private SolidColorBrush selectColor;
        private SolidColorBrush hoverColor;

        public CustomTabController(MainWindow window, double height, double width)
        {
            this.window = window;
            dwellTime = window.getDwellTime();
            backColor = window.getBackColor();
            selectColor = window.getSelectColor();
            hoverColor = window.getHoverColor();

            consoleText = "";
            this.width = (int)(width);
            this.height = (int)(height);

            renderAll(0);
        }

        public void renderAll(int type)
        {
            renderTabController(type);
            renderAdmin(type);
        }
        
        public void renderTabController(int type)
        {
            List<String> ctab_names = new List<String>();
            foreach (KeyValuePair<String, HashSet<String>> pair in window.tabPhrases)
            {
                ctab_names.Add(pair.Key);
            }

            ctab_names.Sort();
            window.ctab_controller_grid.Children.Clear();

            const int strlen_to_width_conversion = 12;
            int total_width = (int)(width);
            int button_height = (int)(height / 4) - 25;
            int button_height_margin = (int)(height / 4);
            int width_so_far = 0;
            int row = 0;

            for (int i = 0; i < ctab_names.Count(); ++i)
            {
                TabPhrase tb;
                int button_width = 50 + ctab_names[i].Length * strlen_to_width_conversion;
                int margin_left = (i > 0) ? width_so_far + 50 + ctab_names[i - 1].Length * strlen_to_width_conversion + 10 : 0;

                if (i > 0 && width_so_far + margin_left > total_width - 10)
                {
                    ++row;
                    margin_left = 0;
                }

                width_so_far = margin_left;
                tb = new TabPhrase(ctab_names[i], button_width, button_height, margin_left, button_height_margin + row * button_height_margin, 0, 0, backColor);
                assignEventHandler(type, tb);
                window.ctab_controller_grid.Children.Add(tb);
            }
        }

        public void updateEvents()
        {
            dwellTime = window.getDwellTime();
            backColor = window.getBackColor();
            selectColor = window.getSelectColor();
            hoverColor = window.getHoverColor();
            renderAll(type);
        }

        private void assignEventHandler(int type, TabPhrase tb)
        {
            if (type < 2)
                tb.MouseEnter += new MouseEventHandler(SwitchTab);
            else
                tb.MouseEnter += new MouseEventHandler(RemoveTab);
        }

        private void renderAdmin(int type)
        {
            TabPhrase create, remove, normal;
            int button_height = (int)(height / 4) - 10;
            int button_width = (width - 20) / 3;
            var converter = new System.Windows.Media.BrushConverter();

            create = new TabPhrase("Add Tab", button_width, button_height, 0, 0, 0, 0, backColor);
            remove = new TabPhrase("Erase Tab", button_width, button_height, button_width + 10, 0, 0, 0, backColor);
            normal = new TabPhrase("Use Tabs", button_width, button_height, 2 * (button_width + 10), 0, 0, 0, backColor);

            if (type == 1)
                create.Background = selectColor;
            else if (type == 0)
                normal.Background = selectColor;
            else
                remove.Background = selectColor;

            create.MouseEnter += new MouseEventHandler(UseInsertClick);
            normal.MouseEnter += new MouseEventHandler(UseNormalClick);
            remove.MouseEnter += new MouseEventHandler(UseRemoveClick);

            window.ctab_controller_grid.Children.Add(create);
            window.ctab_controller_grid.Children.Add(normal);
            window.ctab_controller_grid.Children.Add(remove);
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

        private void SwitchTab(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabPhrase chosenTab = (TabPhrase)sender;

            if (chosenTab.Background != selectColor)
                chosenTab.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(chosenTab);

                window.TabPanel.SelectedIndex = window.TabPanel.Items.IndexOf(window.ctab_items[chosenTab.Content.ToString()]);

                SwitchTab(sender, e);
            };

            chosenTab.MouseLeave += (s, eA) =>
            {
                chosenTab.Background = backColor;
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
                if (consoleText.Length != 0 && !window.tabPhrases.ContainsKey(consoleText))
                {
                    window.createCustomTab(consoleText);
                }

                window.setConsoleText("");
                type = 1;
                renderAll(1);

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
                renderAll(0);

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
                renderAll(2);

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

        private void RemoveTab(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabPhrase deletePhrase = (TabPhrase)sender;

            if (deletePhrase.Background != selectColor)
                deletePhrase.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                highlight(deletePhrase);

                String phrase = deletePhrase.Content.ToString();

                /*
                if (MessageBox.Show("Are you sure that you want to remove the custom tab <" + phrase + ">?",  "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Close the window
                    if (phrase.Length != 0 && window.tabPhrases.ContainsKey(phrase))
                    {
                        window.removeCustomTab(phrase);
                    }
                }
                */

                if (phrase.Length != 0 && window.tabPhrases.ContainsKey(phrase))
                {
                    window.removeCustomTab(phrase);
                }

                renderAll(2);

                RemoveTab(sender, e);
            };

            deletePhrase.MouseLeave += (s, eA) =>
            {
                timer.Stop();

                if (deletePhrase.Background != selectColor)
                    deletePhrase.Background = backColor;
            };

            timer.Start();  
        }
    }
}
