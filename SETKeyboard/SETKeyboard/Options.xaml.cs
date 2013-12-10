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

        public Options(string dwellTime_, string backColor_, string hoverColor_, string selectColor_)
        {
            InitializeComponent();
            optionSettings = new OptionSettings(this, dwellTime_, backColor_, hoverColor_, selectColor_);

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

            optionSettings.setIndex(backBox, hoverBox, selectBox);
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

        List<string> colors;

        public event PropertyChangedEventHandler PropertyChanged;

        public OptionSettings()
        {
        }

        public OptionSettings(Options window_, string dwellTime_, string backColor_, string hoverColor_, string selectColor_)
        {
            optionWindow = window_;
            dwellTime = dwellTime_;
            backColor = backColor_;
            hoverColor = hoverColor_;
            selectColor = selectColor_;

            colors = new List<string>();
            generateColorList();

            fillComboBox(optionWindow.backBox, optionWindow.hoverBox, optionWindow.selectBox);
        }

        private void generateColorList()
        {
            colors.Add("Aqua");
            colors.Add("Beige");
            colors.Add("Black");
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
            colors.Add("White");
            colors.Add("Yellow");
        }

        private void fillComboBox(ComboBox backComboBox, ComboBox hoverComboBox, ComboBox selectComboBox)
        {
            ComboBoxItem a;
            ComboBoxItem b;
            ComboBoxItem c;

            foreach (string color in colors)
            {
                a = new ComboBoxItem();
                b = new ComboBoxItem();
                c = new ComboBoxItem();

                a.Content = color;
                b.Content = color;
                c.Content = color;

                backComboBox.Items.Add(a);
                hoverComboBox.Items.Add(b);
                selectComboBox.Items.Add(c);
            }
        }

        public void setIndex(ComboBox backComboBox, ComboBox hoverComboBox, ComboBox selectComboBox)
        {
            int counter = 0;
            Console.WriteLine(backColor);

            foreach(string color in colors)
            {
                if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == backColor)
                    backComboBox.SelectedIndex = counter;
                if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == hoverColor)
                    hoverComboBox.SelectedIndex = counter;
                if (((SolidColorBrush)new BrushConverter().ConvertFromString(color)).ToString() == selectColor)
                    selectComboBox.SelectedIndex = counter;

                counter++;
            }
        }

        public string DwellTime
        {
            get { return dwellTime; }
            set { dwellTime = value; }
        }

        public string BackColor
        {
            get { return backColor; }
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
            get { return hoverColor; }
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
            get { return selectColor; }
            set
            {
                if (value.IndexOf(':') > 0)
                    selectColor = value.Substring(value.IndexOf(':') + 2);
                else
                    selectColor = value;
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
