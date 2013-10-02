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

        private bool shift = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void KeyHit_Click(object sender, RoutedEventArgs e)
        {
            Button letterKey = (Button)sender;
            char c = letterKey.Content.ToString()[0];
            SETConsole.AppendText((shift) ? c.ToString() : ((char)(c + 32)).ToString());
            FocusCaret();
            shift = false;
        }

        private void Shift_Click(object sender, RoutedEventArgs e)
        {
            Button shiftKey = (Button)sender;
            shift = true;
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            Button backKey = (Button)sender;
            SETConsole.Undo();
            FocusCaret();
        }

        private void Space_Click(object sender, RoutedEventArgs e)
        {
            Button spaceKey = (Button)sender;
            SETConsole.AppendText(" ");
            FocusCaret();
            shift = false;
        }

        private void FocusCaret()
        {
            TextPointer caretPos = SETConsole.CaretPosition;
            caretPos = caretPos.DocumentEnd;
            SETConsole.CaretPosition = caretPos;
            SETConsole.Focus();
        }

    }
}
