using SETKeyboard.GUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
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
    public partial class Options : Window
    {
        private OptionSettings optionSettings;

        public Options(string dwellTime_, string backColor_, string hoverColor_, string selectColor_, string tabColor_, string tabHoverColor_, string tabSelectColor_, string consoleColor_, string wordColor_, string fontColor_)
        {
            InitializeComponent();
            optionSettings = new OptionSettings(this, dwellTime_, backColor_, hoverColor_, selectColor_, tabColor_, tabHoverColor_, tabSelectColor_, consoleColor_, wordColor_, fontColor_);

            Binding DwellTimeBinding = new Binding("DwellTime");
            DwellTimeBinding.Source = optionSettings;
            DwellTimeBox.SetBinding(TextBox.TextProperty, DwellTimeBinding);
            
            Binding BackColorBinding = new Binding("BackColor");
            BackColorBinding.Source = optionSettings;
            backBox.SetBinding(ComboBox.SelectedValueProperty, BackColorBinding);

            Binding HoverColorBinding = new Binding("HoverColor");
            HoverColorBinding.Source = optionSettings;
            hoverBox.SetBinding(ComboBox.SelectedValueProperty, HoverColorBinding);

            Binding SelectColorBinding = new Binding("SelectColor");
            SelectColorBinding.Source = optionSettings;
            selectBox.SetBinding(ComboBox.SelectedValueProperty, SelectColorBinding);
            
            Binding TabColorBinding = new Binding("TabColor");
            TabColorBinding.Source = optionSettings;
            tabBox.SetBinding(ComboBox.SelectedValueProperty, TabColorBinding);
            
            Binding TabHoverColorBinding = new Binding("TabHoverColor");
            TabHoverColorBinding.Source = optionSettings;
            tabHoverBox.SetBinding(ComboBox.SelectedValueProperty, TabHoverColorBinding);

            Binding TabSelectColorBinding = new Binding("TabSelectColor");
            TabSelectColorBinding.Source = optionSettings;
            tabSelectBox.SetBinding(ComboBox.SelectedValueProperty, TabSelectColorBinding);

            Binding ConsoleColorBinding = new Binding("ConsoleColor");
            ConsoleColorBinding.Source = optionSettings;
            consoleBox.SetBinding(ComboBox.SelectedValueProperty, ConsoleColorBinding);

            Binding WordColorBinding = new Binding("WordColor");
            WordColorBinding.Source = optionSettings;
            wordBox.SetBinding(ComboBox.SelectedValueProperty, WordColorBinding);

            Binding FontColorBinding = new Binding("FontColor");
            FontColorBinding.Source = optionSettings;
            fontBox.SetBinding(ComboBox.SelectedValueProperty, FontColorBinding);

            optionSettings.setIndex();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            return;
        }

        public OptionSettings getOptionSettings()
        {
            return optionSettings;
        }
    }

    public class OptionSettings : INotifyPropertyChanged
    {
        private Options optionWindow;
        private string dwellTime;
        private string backColor;
        private string hoverColor;
        private string selectColor;
        private string tabColor;
        private string tabHoverColor;
        private string tabSelectColor;
        private string consoleColor;
        private string wordColor;
        private string fontColor;

        List<string> colors;

        public event PropertyChangedEventHandler PropertyChanged;

        public OptionSettings()
        {
        }

        public OptionSettings(Options window_, string dwellTime_, string backColor_, string hoverColor_, string selectColor_, string tabColor_, string tabHoverColor_, string tabSelectColor_, string consoleColor_, string wordColor_, string fontColor_)
        {
            optionWindow = window_;
            dwellTime = dwellTime_;
            backColor = backColor_;
            hoverColor = hoverColor_;
            selectColor = selectColor_;
            tabColor = tabColor_;
            tabHoverColor = tabHoverColor_;
            tabSelectColor = tabSelectColor_;
            consoleColor = consoleColor_;
            wordColor = wordColor_;
            fontColor = fontColor_;

            colors = new List<string>();
            generateColorList();

            fillComboBox();
        }

        private void generateColorList()
        {
            colors.Add("Default");
            colors.Add("Aqua");
            colors.Add("Beige");
            colors.Add("Blue");
            colors.Add("BlueViolet");
            colors.Add("Brown");
            colors.Add("Cyan");
            colors.Add("Fuchsia");
            colors.Add("Gold");
            colors.Add("Gray");
            colors.Add("Green");
            colors.Add("LightGray");
            colors.Add("Lime");
            colors.Add("MediumSpringGreen");
            colors.Add("Pink");
            colors.Add("Purple");
            colors.Add("Red");
            colors.Add("Silver");
            colors.Add("Yellow");
        }

        private void fillComboBox()
        {
            ComboBoxItem a;
            ComboBoxItem b;
            ComboBoxItem c;
            ComboBoxItem d;
            ComboBoxItem e;
            ComboBoxItem f;
            ComboBoxItem g;
            ComboBoxItem h;

            foreach (string color in colors)
            {
                a = new ComboBoxItem();
                b = new ComboBoxItem();
                c = new ComboBoxItem();
                d = new ComboBoxItem();
                e = new ComboBoxItem();
                f = new ComboBoxItem();
                g = new ComboBoxItem();
                h = new ComboBoxItem();

                a.Content = color;
                b.Content = color;
                c.Content = color;
                d.Content = color;
                e.Content = color;
                f.Content = color;
                g.Content = color;
                h.Content = color;

                optionWindow.backBox.Items.Add(a);
                optionWindow.hoverBox.Items.Add(b);
                optionWindow.selectBox.Items.Add(c);
                optionWindow.tabBox.Items.Add(d);
                optionWindow.tabHoverBox.Items.Add(e);
                optionWindow.tabSelectBox.Items.Add(f);
                optionWindow.consoleBox.Items.Add(g);
                optionWindow.wordBox.Items.Add(h);
            }

            a = new ComboBoxItem();
            b = new ComboBoxItem();

            a.Content = "White";
            b.Content = "Black";

            optionWindow.fontBox.Items.Add(a);
            optionWindow.fontBox.Items.Add(b);
        }

        public void setIndex()
        {
            int counter = 0;

            foreach(string color in colors)
            {
                if (color != "Default")
                {
                    if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == backColor)
                        optionWindow.backBox.SelectedIndex = counter;
                    if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == hoverColor)
                        optionWindow.hoverBox.SelectedIndex = counter;
                    if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == selectColor)
                        optionWindow.selectBox.SelectedIndex = counter;
                    if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == tabColor)
                        optionWindow.tabBox.SelectedIndex = counter;
                    if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == tabHoverColor)
                        optionWindow.tabHoverBox.SelectedIndex = counter;
                    if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == tabSelectColor)
                        optionWindow.tabSelectBox.SelectedIndex = counter;
                    if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == consoleColor)
                        optionWindow.consoleBox.SelectedIndex = counter;
                    if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == wordColor)
                        optionWindow.wordBox.SelectedIndex = counter;
                }

                counter++;
            }

            if (optionWindow.backBox.SelectedIndex == -1)
                optionWindow.backBox.SelectedIndex = 0;
            if (optionWindow.hoverBox.SelectedIndex == -1)
                optionWindow.hoverBox.SelectedIndex = 0;
            if (optionWindow.selectBox.SelectedIndex == -1)
                optionWindow.selectBox.SelectedIndex = 0;
            if (optionWindow.tabBox.SelectedIndex == -1)
                optionWindow.tabBox.SelectedIndex = 0;
            if (optionWindow.tabHoverBox.SelectedIndex == -1)
                optionWindow.tabHoverBox.SelectedIndex = 0;
            if (optionWindow.tabSelectBox.SelectedIndex == -1)
                optionWindow.tabSelectBox.SelectedIndex = 0;
            if (optionWindow.consoleBox.SelectedIndex == -1)
                optionWindow.consoleBox.SelectedIndex = 0;
            if (optionWindow.wordBox.SelectedIndex == -1)
                optionWindow.wordBox.SelectedIndex = 0;

            if (Brushes.White.ToString() == fontColor)
                optionWindow.fontBox.SelectedIndex = 0;
            if (Brushes.Black.ToString() == fontColor)
                optionWindow.fontBox.SelectedIndex = 1;
        }

        public string DwellTime
        {
            get { return dwellTime; }
            set { dwellTime = value; }
        }

        public string BackColor
        {
            get 
            {
                Console.WriteLine(backColor);
                if (backColor == "Default")
                    return "#FFFFFF";

                return backColor; 
            }
            set 
            {
                if (value.IndexOf(':') > 0)
                    backColor = value.Substring(value.IndexOf(':') + 2);
                else
                    backColor = value;
            }
        }

        public string HoverColor
        {
            get 
            {
                if (hoverColor == "Default")
                    return "#EFFAFC";

                return hoverColor; 
            }
            set
            {
                if (value.IndexOf(':') > 0)
                    hoverColor = value.Substring(value.IndexOf(':') + 2);
                else
                    hoverColor = value;
            }
        }

        public string SelectColor
        {
            get 
            {
                if (selectColor == "Default")
                    return "#FAAF49";

                return selectColor; 
            }
            set
            {
                if (value.IndexOf(':') > 0)
                    selectColor = value.Substring(value.IndexOf(':') + 2);
                else
                    selectColor = value;
            }
        }
        
        public string TabColor
        {
            get 
            {
                if (tabColor == "Default")
                    return "#3C4246";

                return tabColor; 
            }
            set
            {
                if (value.IndexOf(':') > 0)
                    tabColor = value.Substring(value.IndexOf(':') + 2);
                else
                    tabColor = value;
            }
        }
        
        public string TabHoverColor
        {
            get 
            {
                if (tabHoverColor == "Default")
                    return "#454C51";

                return tabHoverColor; 
            }
            set
            {
                if (value.IndexOf(':') > 0)
                    tabHoverColor = value.Substring(value.IndexOf(':') + 2);
                else
                    tabHoverColor = value;
            }
        }

        public string TabSelectColor
        {
            get 
            {
                if (tabSelectColor == "Default")
                    return "#FAB049";

                return tabSelectColor; 
            }
            set
            {
                if (value.IndexOf(':') > 0)
                    tabSelectColor = value.Substring(value.IndexOf(':') + 2);
                else
                    tabSelectColor = value;
            }
        }

        public string ConsoleColor
        {
            get 
            {
                if (consoleColor == "Default")
                    return "#00AFF0";

                return consoleColor; 
            }
            set
            {
                if (value.IndexOf(':') > 0)
                    consoleColor = value.Substring(value.IndexOf(':') + 2);
                else
                    consoleColor = value;
            }
        }

        public string WordColor
        {
            get 
            {
                if (wordColor == "Default")
                    return "#C0DC6E";

                return wordColor; 
            }
            set
            {
                if (value.IndexOf(':') > 0)
                    wordColor = value.Substring(value.IndexOf(':') + 2);
                else
                    wordColor = value;
            }
        }

        public string FontColor
        {
            get { return fontColor; }
            set
            {
                if (value.IndexOf(':') > 0)
                    fontColor = value.Substring(value.IndexOf(':') + 2);
                else
                    fontColor = value;
            }
        }

        protected void OnPropertyChanged(String info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
