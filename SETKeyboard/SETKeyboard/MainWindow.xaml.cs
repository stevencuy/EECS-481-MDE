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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QwertyKeyboard qwerty;

        public MainWindow()
        {
            InitializeComponent();
            qwerty = new QwertyKeyboard(this);
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