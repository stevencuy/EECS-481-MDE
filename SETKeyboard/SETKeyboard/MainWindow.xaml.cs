using SETKeyboard.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SETKeyboard
{
    public partial class MainWindow : Window
    {
        public MainWindow window;
        public Options options;
        private QwertyKeyboard qwerty;
        private T9Keyboard T9;
        private ExtraKeyboard extra;
        private Output output;
        private CustomTabController ctab_controller;

        public ReadWrite RW;
        private String consoleText;
        public Grid ctab_controller_grid = new Grid();
        private TabItem ctab_controller_item;

        public Dictionary<String, CustomTab> ctabs = new Dictionary<String, CustomTab>();
        public Dictionary<String, HashSet<String>> tabPhrases = new Dictionary<String, HashSet<String>>();
        public Dictionary<String, Grid> ctab_grids = new Dictionary<String, Grid>();
        public Dictionary<String, TabItem> ctab_items = new Dictionary<String, TabItem>();

        System.Windows.Controls.Button[] buttons;
        private PredictionHandler predictionHandler;
        private Query query;

        public int dwellTime;
        public SolidColorBrush backColor;
        public SolidColorBrush hoverColor;
        public SolidColorBrush selectColor;
        public SolidColorBrush tabColor;
        public SolidColorBrush tabHoverColor;
        public SolidColorBrush tabSelectColor;
        public SolidColorBrush consoleColor;
        public SolidColorBrush wordColor;
        public SolidColorBrush fontColor;
        public DispatcherTimer timer;

        public MainWindow()
        {

            dwellTime = 2;
            backColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
            hoverColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EFFAFC"));
            selectColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FAAF49"));
            tabColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3C4246"));
            tabHoverColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#454C51"));
            tabSelectColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FAB049"));
            consoleColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#00AFF0");
            wordColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#C0DC6E");
            fontColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFF");

            this.predictionHandler = new PredictionHandler();
            this.query = new Query();

            InitializeComponent();
            this.consoleText = "";
            window = this;
            RW = new ReadWrite(window);
            //load existing settings if settings file exists
            RW.LoadSettings();

            //T9 keyboard resizes from within MainWindow xaml file
            T9 = new T9Keyboard(window);

            //Qwerty keyboard and output interface resize through this delegate
            Loaded += delegate
            {
                qwerty = new QwertyKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
                extra = new ExtraKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
                output = new Output(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
                buttons = new System.Windows.Controls.Button[0];
                window.SETConsole.Background = consoleColor;
                window.suggestionBar.Background = wordColor;

                refreshTabs();
            };

            //read ctab files from disk
            
            RW.ReadTabs();
            generateTabController();
            loadCustomTabs();
        }

        ~MainWindow()
        {
            RW.Write();
        }

        private void OptionsMenu(object sender, RoutedEventArgs e)
        {
            options = new Options(dwellTime.ToString(), backColor.ToString(), hoverColor.ToString(), selectColor.ToString(), tabColor.ToString(), tabHoverColor.ToString(), tabSelectColor.ToString(), consoleColor.ToString(), wordColor.ToString(), fontColor.ToString());
            options.Owner = this;
            options.ShowDialog();

            if (options.DialogResult == true)
            {
                int value;
                if (!int.TryParse(options.getOptionSettings().DwellTime, out value))
                {
                    MessageBox.Show("Dwell Time must be an integer.");
                }
                else if (Convert.ToInt32(options.getOptionSettings().DwellTime) <= 1)
                {
                    MessageBox.Show("Dwell Time must be greater than 1.");
                }
                else
                    dwellTime = Convert.ToInt32(options.getOptionSettings().DwellTime);

                backColor = (SolidColorBrush)new BrushConverter().ConvertFromString(options.getOptionSettings().BackColor);
                hoverColor = (SolidColorBrush)new BrushConverter().ConvertFromString(options.getOptionSettings().HoverColor);
                selectColor = (SolidColorBrush)new BrushConverter().ConvertFromString(options.getOptionSettings().SelectColor);
                tabColor = (SolidColorBrush)new BrushConverter().ConvertFromString(options.getOptionSettings().TabColor);
                tabHoverColor = (SolidColorBrush)new BrushConverter().ConvertFromString(options.getOptionSettings().TabHoverColor);
                tabSelectColor = (SolidColorBrush)new BrushConverter().ConvertFromString(options.getOptionSettings().TabSelectColor);
                consoleColor = (SolidColorBrush)new BrushConverter().ConvertFromString(options.getOptionSettings().ConsoleColor);
                wordColor = (SolidColorBrush)new BrushConverter().ConvertFromString(options.getOptionSettings().WordColor);
                fontColor = (SolidColorBrush)new BrushConverter().ConvertFromString(options.getOptionSettings().FontColor);

                refreshTabs();
                window.SETConsole.Background = consoleColor;
                window.SETConsole.Foreground = fontColor;
                window.suggestionBar.Background = wordColor;
                window.suggestionBar.Foreground = fontColor;
                updateSuggestionBar();
                ctab_controller.updateEvents();
                updateCustomTabs();
                T9 = new T9Keyboard(window);
                qwerty = new QwertyKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
                extra = new ExtraKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
                output = new Output(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
                RW.SyncSettings(window);
            }
        }

        private void sizeChanged(object sender, RoutedEventArgs e)
        {
            qwerty_grid.Children.Clear();
            extra_grid.Children.Clear();
            OUTPUTGrid.Children.Clear();
            qwerty = new QwertyKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
            extra = new ExtraKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
            output = new Output(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
        }

        private void refreshTabs()
        {
            TabItem tab;

            for (int i = 0; i < window.TabPanel.Items.Count; i++)
            {
                tab = (TabItem)window.TabPanel.Items.GetItemAt(i);
                tab.Foreground = fontColor;

                if (tab == (TabItem)window.TabPanel.SelectedItem)
                    tab.Background = tabSelectColor;
                else if (tab.IsMouseOver)
                    tab.Background = tabHoverColor;
                else
                    tab.Background = tabColor;
            }
        }

        private void Extra_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem ExtraButton = (TabItem)sender;

            if(ExtraButton.Background != tabSelectColor)
                ExtraButton.Background = tabHoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = 2;
                refreshTabs();
            };

            ExtraButton.MouseLeave += (s, eA) =>
            {
                timer.Stop();
                refreshTabs();
            };

            timer.Start();
        }

        private void QWERTY_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem QwertyButton = (TabItem)sender;

            if (QwertyButton.Background != tabSelectColor)
                QwertyButton.Background = tabHoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = 0;
                refreshTabs();
            };

            QwertyButton.MouseLeave += (s, eA) =>
            {
                timer.Stop();
                refreshTabs();
            };

            timer.Start();
        }

        private void T9_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem T9Button = (TabItem)sender;

            if(T9Button.Background != tabSelectColor)
                T9Button.Background = tabHoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = 1;
                refreshTabs();
            };

            T9Button.MouseLeave += (s, eA) =>
            {
                timer.Stop();
                refreshTabs();
            };

            timer.Start();
        }

        private void Output_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem OutputButton = (TabItem)sender;

            if (OutputButton.Background != tabSelectColor)
                OutputButton.Background = tabHoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = 3;
                refreshTabs();
            };

            OutputButton.MouseLeave += (s, eA) =>
            {
                timer.Stop();
                refreshTabs();
            };

            timer.Start();
        }

        private void CTAB_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem CTABButton = (TabItem)sender;

            if(CTABButton.Background != tabSelectColor)
                CTABButton.Background = tabHoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = 4;
                refreshTabs();
            };

            CTABButton.MouseLeave += (s, eA) =>
            {
                timer.Stop();
                refreshTabs();
            };

            timer.Start();
        }

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem TabButton = (TabItem)sender;
            
            if(TabButton.Background != tabSelectColor)
                TabButton.Background = tabHoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = window.TabPanel.Items.IndexOf(TabButton);
                refreshTabs();
            };

            TabButton.MouseLeave += (s, eA) =>
            {
                timer.Stop();
                refreshTabs();
            };

            timer.Start();
        }

        public void loadCustomTabs()
        {
            foreach (var pair in tabPhrases)
            {
                loadCustomTab(pair.Key);
            }
        }

        public void updateCustomTabs()
        {
            foreach (var pair in tabPhrases)
            {
                updateCustomTab(pair.Key);
            }
        }

        public void updateCustomTab(String name)
        {
            ctabs[name].updateEvents();
        }

        public void loadCustomTab(String name)
        {
            ctabs.Add(name, new CustomTab(window, name, 500, 800));
            TabItem tab = new TabItem();
            tab.Header = name;
            tab.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
            tab.Width = 30 + name.Length * 15;
            tab.Height = 100;
            tab.Content = ctab_grids[name];
            tab.MouseEnter += Tab_Click;
            ctab_items.Add(name, tab);
            TabPanel.Items.Add(tab);
            refreshTabs();
        }

        public void createCustomTab(String name)
        {
            ctabs.Add(name, new CustomTab(window, name, 500, 800));
            tabPhrases[name] = new HashSet<String>();
            TabItem tab = new TabItem();
            tab.Header = name;
            tab.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
            tab.Width = 30 + name.Length * 15;
            tab.Height = 100;
            tab.Content = ctab_grids[name];
            tab.MouseEnter += Tab_Click;
            ctab_items.Add(name, tab);
            TabPanel.Items.Add(tab);
            refreshTabs();
        }

        public void removeCustomTab(String name)
        {
            TabPanel.Items.Remove(ctab_items[name]);
            ctab_grids.Remove(name);
            tabPhrases.Remove(name);
            ctab_items.Remove(name);
            ctabs.Remove(name);
            window.TabPanel.SelectedIndex = window.TabPanel.Items.IndexOf(ctab_controller_item);
            refreshTabs();
        }

        private void generateTabController()
        {
            ctab_controller = new CustomTabController(window, 500, 800);
            ctab_controller_item = new TabItem();
            ctab_controller_item.MouseEnter += CTAB_Click;
            ctab_controller_item.Foreground = fontColor;
            ctab_controller_item.Header = "Edit Tabs";
            ctab_controller_item.Width = 15 + "Edit Tabs".Length * 15;
            ctab_controller_item.Height = 100;

            ctab_controller_grid.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EEEEEE"));
            ctab_controller_item.Content = ctab_controller_grid;
            TabPanel.Items.Add(ctab_controller_item);
        }

        public void assignGrid(String tab_name)
        {
            Grid g = new Grid();
            g.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EEEEEE"));
            ctab_grids.Add(tab_name, g);
        }

        public void setConsoleText(String consoleText)
        {
            query.setQuery(consoleText);
            string[] bro = predictionHandler.getPredictions(query);

            buttons = new System.Windows.Controls.Button[bro.Length];

            for (int i = 0; i < bro.Length; i++)
            {
                System.Windows.Controls.Button suggestionButton = new System.Windows.Controls.Button();
                suggestionButton.FontSize = 65;
                suggestionButton.Foreground = fontColor;

                if (bro[i] == "t")
                    suggestionButton.Content = " 't ";
                else if (bro[i] == "s")
                    suggestionButton.Content = " 's ";
                else if (bro[i] == "i")
                    suggestionButton.Content = " I ";
                else
                    suggestionButton.Content = " " + bro[i] + " ";

                if (bro[i] != null)
                {
                    suggestionButton.MouseEnter += new MouseEventHandler(suggestionBar_Click);
                    buttons[i] = suggestionButton;
                }
            }
            suggestionBar.ItemsSource = buttons;

            /*
             * Takes care of the event of an autocompletion followed by a period. A completion will add a space
             * after the selected suggested word in the console. Clicking the period will remove this extra space. 
             */
            consoleText = consoleText.Replace(" i ", " I ");
            consoleText = consoleText.Replace(" .", ". ");
            consoleText = consoleText.Replace("  ", " ");

            //Puts text onto the console
            this.consoleText = consoleText;
            window.SETConsole.Document.Blocks.Clear();
            window.SETConsole.Document.Blocks.Add(new Paragraph(new Run(consoleText)));
            window.FocusCaret();
        }

        public void updateSuggestionBar()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if(buttons[i] != null)
                    buttons[i].Foreground = fontColor;
            }
        }

        private void suggestionBar_Click(object sender, EventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            Button button = (Button)sender;
            string replacement = button.Content.ToString();
            replacement = replacement.Substring(1, replacement.Length - 2);

            button.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                int replaceIndex = 0;
                for (int i = this.consoleText.Length - 1; i >= 0; i--)
                {
                    if (consoleText[i].Equals(' ') || (i == 0))
                    {
                        replaceIndex = i;
                        break;
                    }
                }
                /*
                 * Preserves the case of a word being replaced by a selected suggestion
                 */
                int replacementOffset = 0;
                if (replaceIndex != 0)
                {
                    replacementOffset = 1;
                }

                if (!consoleText[consoleText.Length - 1].Equals(' '))
                {
                    replacement = consoleText[replaceIndex + replacementOffset] + replacement.Substring(1, replacement.Length - 1);
                }

                consoleText = consoleText.Substring(0, replaceIndex);

                if (replaceIndex != 0)
                {
                    consoleText += " ";
                }

                if (replacement == "'t" && consoleText.Length > 1)
                    consoleText = consoleText.Substring(0, consoleText.Length - 1) + "'t ";
                else if (replacement == "'s" && consoleText.Length > 1)
                    consoleText = consoleText.Substring(0, consoleText.Length - 1) + "'s ";
                else
                    consoleText += replacement + " ";

                setConsoleText(consoleText);

                suggestionBar_Click(sender, e);
            };

            button.MouseLeave += (s, eA) =>
            {
                button.Background = null;
                timer.Stop();
            };

            timer.Start();
        }

        public int getDwellTime()
        {
            return dwellTime;
        }

        public SolidColorBrush getBackColor()
        {
            return backColor;
        }

        public SolidColorBrush getHoverColor()
        {
            return hoverColor;
        }

        public SolidColorBrush getSelectColor()
        {
            return selectColor;
        }

        public String getConsoleText()
        {
            return this.consoleText;
        }

        public void FocusCaret()
        {
            TextPointer caretPos = SETConsole.CaretPosition;
            caretPos = caretPos.DocumentEnd;
            SETConsole.CaretPosition = caretPos;
            SETConsole.Focus();
        }
    }
}