﻿using SETKeyboard.GUI;
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

namespace SETKeyboard
{
    public partial class MainWindow : Window
    {
        private QwertyKeyboard qwerty;
        public TabReadWrite TRW;
        private T9Keyboard T9;
        private Output output;
        private String consoleText;
        public Grid ctab_controller_grid = new Grid();
        public MainWindow window;
        private CustomTabController ctab_controller;
        private TabItem ctab_controller_item;
        public Dictionary<String, CustomTab> ctabs = new Dictionary<String, CustomTab>();
        public Dictionary<String, HashSet<String>> tabPhrases = new Dictionary<String, HashSet<String>>();
        public Dictionary<String, Grid> ctab_grids = new Dictionary<String, Grid>();
        public Dictionary<String, TabItem> ctab_items = new Dictionary<String, TabItem>();
        public MainWindow()
        {
           
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
            loadCustomTabs();
            generateTabController();
 
        }
        ~MainWindow()
        {
            TRW.Write();
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
            tab.Height = 50;
            tab.Content = ctab_grids[name];
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
            tab.Height = 50;
            tab.Content = ctab_grids[name];
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
            ctab_controller_item.Header = "Custom Tab Controller";
            ctab_controller_item.Width = "Custom Tab Controller".Length * 15;
            ctab_controller_item.Height = 50;
            var converter = new System.Windows.Media.BrushConverter();
            ctab_controller_grid.Background = (Brush)converter.ConvertFromString("#FFEDF5FF");
            ctab_controller_item.Content = ctab_controller_grid;
            TabPanel.Items.Add(ctab_controller_item);
        }

        private void sizeChanged(object sender, RoutedEventArgs e)
        {
            qwerty_grid.Children.Clear();
            OUTPUTGrid.Children.Clear();
            qwerty = new QwertyKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
            output = new Output(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
        }

        public String getConsoleText()
        {
            return this.consoleText;
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
            this.consoleText = consoleText;
            window.SETConsole.Document.Blocks.Clear();
            window.SETConsole.Document.Blocks.Add(new Paragraph(new Run(consoleText)));
            window.FocusCaret();
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