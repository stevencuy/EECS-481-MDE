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

namespace SETKeyboard
{
    public partial class MainWindow : Window
    {
        private QwertyKeyboard qwerty;
        private T9Keyboard T9;
        private String consoleText;
        MainWindow window;

        public MainWindow()
        {
            InitializeComponent();
            window = this;

            this.consoleText = "";

            //T9 keyboard resizes from within MainWindow xaml file
            T9 = new T9Keyboard(window);

            //Qwerty keyboard resizes through this delegate
            Loaded += delegate
            {
                qwerty = new QwertyKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
            };
        }

        private void sizeChanged(object sender, RoutedEventArgs e)
        {
            QWERTYGrid.Children.Clear();
            qwerty = new QwertyKeyboard(window, TabPanel.ActualHeight, TabPanel.ActualWidth);
        }

        public String getConsoleText()
        {
            return this.consoleText;
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