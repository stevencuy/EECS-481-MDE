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

namespace SETKeyboard.GUI
{
    class CustomTabController
    {
        public MainWindow window;
        public String consoleText;
        public int width;
        public int height;
        public CustomTabController(MainWindow window, double height, double width)
        {
            this.window = window;
            consoleText = "";
            this.width = (int)(width);
            this.height = (int)(height);
            renderAll(0);
        }
        public void renderAll(int type)
        {
            renderTabController(type);
            renderAdmin();

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
            int button_height = (int)(height / 4) - 10;
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
                tb = new TabPhrase(ctab_names[i], button_width, button_height, margin_left, button_height_margin + row * button_height_margin, 0, 0);
                assignEventHandler(type, tb);
                window.ctab_controller_grid.Children.Add(tb);
            }
        }
        private void assignEventHandler(int type, TabPhrase tb)
        {
            if (type < 2)
                tb.Click += new RoutedEventHandler(SwitchTab);
            else
                tb.Click += new RoutedEventHandler(RemoveTab);
        }
        private void renderAdmin()
        {
            TabPhrase create, remove, normal;
            int button_height = (int)(height / 4) - 10;
            int button_width = (width - 20) / 3;
            create = new TabPhrase("(+) Tab", button_width, button_height, 0, 0, 0, 0);
            remove = new TabPhrase("(-) Tab", button_width, button_height, button_width + 10, 0, 0, 0);
            normal = new TabPhrase("Use Tabs", button_width, button_height, 2 * (button_width + 10), 0, 0, 0);
            create.Click += new RoutedEventHandler(UseInsertClick);
            normal.Click += new RoutedEventHandler(UseNormalClick);
            remove.Click += new RoutedEventHandler(UseRemoveClick);

            window.ctab_controller_grid.Children.Add(create);
            window.ctab_controller_grid.Children.Add(normal);
            window.ctab_controller_grid.Children.Add(remove);
        }   
        private void SwitchTab(object sender, RoutedEventArgs e)
        {
            TabPhrase chosenTab = (TabPhrase)sender;
            window.TabPanel.SelectedIndex = window.TabPanel.Items.IndexOf(window.ctab_items[chosenTab.Content.ToString()]);
        }

        private void UseInsertClick(object sender, RoutedEventArgs e)
        {

            consoleText = window.getConsoleText();
            if (consoleText.Length != 0 && !window.tabPhrases.ContainsKey(consoleText))
            {
                window.createCustomTab(consoleText);
            }
            renderAll(1);
        }
        private void UseNormalClick(object sender, RoutedEventArgs e)
        {
            renderAll(0);
        }
        private void UseRemoveClick(object sender, RoutedEventArgs e)
        {
            
            renderAll(2);
        }
        private void RemoveTab(object sender, RoutedEventArgs e)
        {
            TabPhrase deletePhrase = (TabPhrase)sender;
            String phrase = deletePhrase.Content.ToString();
            if (MessageBox.Show("Are you sure that you want to remove the custom tab <" + phrase + ">?",
  "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Close the window
                if (phrase.Length != 0 && window.tabPhrases.ContainsKey(phrase))
                {
                    window.removeCustomTab(phrase);
                }
            }
            renderAll(2);
        }
    }
}
