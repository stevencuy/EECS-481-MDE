using SETKeyboard.GUI;
using System;
using System.Collections.Generic;
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
        private QwertyKeyboard qwerty;
        private T9Keyboard T9;
        private Output output;
        private CustomTabController ctab_controller;

        public TabReadWrite TRW;
        private String consoleText;
        public Grid ctab_controller_grid = new Grid();
        private TabItem ctab_controller_item;

        public Dictionary<String, CustomTab> ctabs = new Dictionary<String, CustomTab>();
        public Dictionary<String, HashSet<String>> tabPhrases = new Dictionary<String, HashSet<String>>();
        public Dictionary<String, Grid> ctab_grids = new Dictionary<String, Grid>();
        public Dictionary<String, TabItem> ctab_items = new Dictionary<String, TabItem>();

        private PredictionHandler predictionHandler;
        private Query query;

        private int dwellTime;
        private SolidColorBrush hoverColor;
        private SolidColorBrush selectColor;
        private SolidColorBrush backColor;
        private DispatcherTimer timer;

        public MainWindow()
        {
            dwellTime = 2;
            backColor = Brushes.LightGray;
            hoverColor = Brushes.Gray;
            selectColor = Brushes.MediumSpringGreen;

            this.predictionHandler = new PredictionHandler();
            this.query = new Query();

            InitializeComponent();
            this.consoleText = "";
            window = this;
            
            //T9 keyboard resizes from within MainWindow xaml file
            T9 = new T9Keyboard(window);
  
            //Qwerty keyboard and output interface resize through this delegate
            Loaded += delegate
            {
                qwerty = new QwertyKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
                output = new Output(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
            };

            //read ctab files from disk
            TRW = new TabReadWrite(window);
            TRW.Read();
            generateTabController();
            loadCustomTabs();          
        }

        ~MainWindow()
        {
            TRW.Write();
        }

        private void OptionsMenu(object sender, RoutedEventArgs e)
        {

        }

        private void sizeChanged(object sender, RoutedEventArgs e)
        {
            qwerty_grid.Children.Clear();
            OUTPUTGrid.Children.Clear();
            qwerty = new QwertyKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
            output = new Output(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
        }

        private void QWERTY_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem QwertyButton = (TabItem)sender;

            QwertyButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = 0;
            };

            QwertyButton.MouseLeave += (s, eA) =>
            {
                QwertyButton.Background = Brushes.LightGray;
                timer.Stop();
            };

            timer.Start();      
        }

        private void T9_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem T9Button = (TabItem)sender;

            T9Button.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = 1;
            };

            T9Button.MouseLeave += (s, eA) =>
            {
                T9Button.Background = Brushes.LightGray;
                timer.Stop();
            };

            timer.Start();
        }

        private void Output_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem OutputButton = (TabItem)sender;

            OutputButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = 2;
            };

            OutputButton.MouseLeave += (s, eA) =>
            {
                OutputButton.Background = Brushes.LightGray;
                timer.Stop();
            };

            timer.Start();
        }

        private void CTAB_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem CTABButton = (TabItem)sender;

            CTABButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = 3;
            };

            CTABButton.MouseLeave += (s, eA) =>
            {
                CTABButton.Background = Brushes.LightGray;
                timer.Stop();
            };

            timer.Start();
        }

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dwellTime);
            TabItem TabButton = (TabItem)sender;

            TabButton.Background = hoverColor;

            timer.Tick += (sender2, eventArgs) =>
            {
                timer.Stop();

                window.TabPanel.SelectedIndex = window.TabPanel.Items.IndexOf(TabButton);
            };

            TabButton.MouseLeave += (s, eA) =>
            {
                TabButton.Background = Brushes.LightGray;
                timer.Stop();
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

        public void loadCustomTab(String name)
        {
            ctabs.Add(name, new CustomTab(window, name, 500, 800));
            TabItem tab = new TabItem();
            tab.Header = name;
            tab.Width = 30 + name.Length * 15;
            tab.Height = 100;
            tab.Content = ctab_grids[name];
            tab.MouseEnter += Tab_Click;
            ctab_items.Add(name, tab);
            TabPanel.Items.Add(tab);
        }

        public void createCustomTab(String name)
        {
            ctabs.Add(name, new CustomTab(window, name, 500, 800));
            tabPhrases[name] = new HashSet<String>();
            TabItem tab = new TabItem();
            tab.Header = name;
            tab.Width = 30 + name.Length * 15;
            tab.Height = 100;
            tab.Content = ctab_grids[name];
            tab.MouseEnter += Tab_Click;
            ctab_items.Add(name, tab);
            TabPanel.Items.Add(tab);
        }

        public void removeCustomTab(String name)
        {
            TabPanel.Items.Remove(ctab_items[name]);
            ctab_grids.Remove(name);
            tabPhrases.Remove(name);
            ctab_items.Remove(name);
            ctabs.Remove(name);
            window.TabPanel.SelectedIndex = window.TabPanel.Items.IndexOf(ctab_controller_item);
        }

        private void generateTabController()
        {
            ctab_controller = new CustomTabController(window, 500, 800);
            ctab_controller_item = new TabItem();
            ctab_controller_item.MouseEnter += CTAB_Click;
            ctab_controller_item.Header = "Tab Controller";
            ctab_controller_item.Width = 15 + "Tab Controller".Length * 15;
            ctab_controller_item.Height = 100;
            var converter = new System.Windows.Media.BrushConverter();

            ctab_controller_grid.Background = (Brush)converter.ConvertFromString("#FFEDF5FF");
            ctab_controller_item.Content = ctab_controller_grid;
            TabPanel.Items.Add(ctab_controller_item);
        }

        public void assignGrid(String tab_name)
        {
            Grid g = new Grid();
            var converter = new System.Windows.Media.BrushConverter();
            g.Background = (Brush)converter.ConvertFromString("#FFEDF5FF");
            ctab_grids.Add(tab_name, g);
        }

        public void setConsoleText(String consoleText)
        {
            query.setQuery(consoleText);
            string[] bro = predictionHandler.getPredictions(query);

            System.Windows.Controls.Button[] buttons = new System.Windows.Controls.Button[bro.Length];

            for (int i = 0; i < bro.Length; i++)
            {
                System.Windows.Controls.Button suggestionButton = new System.Windows.Controls.Button();
                suggestionButton.FontSize = 65;

                if (bro[i] == "t")
                    suggestionButton.Content = " 't ";
                else if (bro[i] == "s")
                    suggestionButton.Content = " 's ";
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