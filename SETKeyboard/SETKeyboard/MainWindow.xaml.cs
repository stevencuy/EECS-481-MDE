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
    //ButtonKey
    public class KeyButton : Button
    {
        private string lower;
        private string upper;

        public KeyButton(string upper_, string lower_, int width_, int height_, int margin_l, int margin_t, int margin_r, int margin_b) : base()
        {
            lower = lower_;
            upper = upper_;
            Content = lower;
            Margin = new Thickness(margin_l, margin_t, margin_r, margin_b);
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Width = width_;
            Height = height_;
        }

        public void toggle()
        {
            Content = (Content.ToString() == lower) ? upper : lower;
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool shift = false;
        private List<KeyButton> keys = new List<KeyButton>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeQWERTY();
        }

        private void InitializeQWERTY()
        {
            string[] keyStrings = new string[29];
            //ROW 0
            keyStrings[0] = "q";
            keyStrings[1] = "w";
            keyStrings[2] = "e";
            keyStrings[3] = "r";
            keyStrings[4] = "t";
            keyStrings[5] = "y";
            keyStrings[6] = "u";
            keyStrings[7] = "i";
            keyStrings[8] = "o";
            keyStrings[9] = "p";
            //ROW 1
            keyStrings[10] = "a";
            keyStrings[11] = "s";
            keyStrings[12] = "d";
            keyStrings[13] = "f";
            keyStrings[14] = "g";
            keyStrings[15] = "h";
            keyStrings[16] = "j";
            keyStrings[17] = "k";
            keyStrings[18] = "l";
            //ROW 2
            keyStrings[19] = "shift";
            keyStrings[20] = "z";
            keyStrings[21] = "x";
            keyStrings[22] = "c";
            keyStrings[23] = "v";
            keyStrings[24] = "b";
            keyStrings[25] = "n";
            keyStrings[26] = "m";
            keyStrings[27] = "back";
            //SPACE ROW
            keyStrings[28] = "space";

            const int row_size = 3;
            int col_size = 10;
            const int button_width = 100, button_height = 50;
            const int margin_inc = 100;
            int[] margins_l = new int[3];
            margins_l[0] = 0;
            margins_l[1] = 50;
            margins_l[2] = 150;

            int index = 0;
            for (int row = 0; row < row_size; ++row)
            {
                for (int col = 0; col < col_size; ++col)
                {
                    string lower = keyStrings[index];
                    string upper = lower.ToUpper();
                    KeyButton b;
                    if (row != 2)
                    {
                        b = new KeyButton(upper, lower, button_width, button_height, margins_l[row] + margin_inc * col, row * 50, 0, 0);
                        b.Click += new RoutedEventHandler(KeyHit_Click);
                    }
                    else
                    {
                        if (lower == "shift")
                        {
                            b = new KeyButton(upper, lower, button_width + 50, button_height, 0, row * 50, 0, 0);
                            b.Click += new RoutedEventHandler(Shift_Click);
                        }
                        else if (lower == "back")
                        {
                            b = new KeyButton(upper, lower, button_width + 50, button_height, margins_l[row] + margin_inc * (col - 1), row * 50, 0, 0);
                            b.Click += new RoutedEventHandler(Backspace_Click);
                        }
                        else
                        {
                            b = new KeyButton(upper, lower, button_width, button_height, margins_l[row] + margin_inc * (col - 1), row * 50, 0, 0);
                            b.Click += new RoutedEventHandler(KeyHit_Click);
                        }
                    }
                    
                    keys.Add(b);
                    QWERTYGrid.Children.Add(b);
                    ++index;
                }
                //first row has 10 every other row has 9
                if (row == 0)
                    --col_size;
            }

            //add spacebar
            KeyButton spacebar = new KeyButton(keyStrings[28].ToUpper(), keyStrings[28], 500, 50, 250, 150, 0, 0);
            keys.Add(spacebar);
            QWERTYGrid.Children.Add(spacebar);
            spacebar.Click += new RoutedEventHandler(Space_Click);
        }

        private void toggleKeyButtons()
        {
            for (int i = 0; i < keys.Count(); ++i)
                keys[i].toggle();
        }

        private void KeyHit_Click(object sender, RoutedEventArgs e)
          {
              KeyButton letterKey = (KeyButton)sender;
              SETConsole.AppendText(letterKey.Content.ToString());
              FocusCaret();
              if (keys[19].Background == Brushes.MediumSpringGreen)
                  keys[19].Background = Brushes.LightGray;
              if(shift)
              {
                  shift = false;
                  toggleKeyButtons();
              }
              FocusCaret();
              
          }

          private void Shift_Click(object sender, RoutedEventArgs e)
          {
              if (!shift)
              {
                  //KeyButton shiftKey = (KeyButton)sender;
                  keys[19].Background = Brushes.MediumSpringGreen;        
                  shift = true;
                  toggleKeyButtons();
              }
              else
              {
                  keys[19].Background = Brushes.LightGray;
                  shift = false;
                  toggleKeyButtons();
              }
              FocusCaret();
          }

          private void Backspace_Click(object sender, RoutedEventArgs e)
          {
              //Button backKey = (Button)sender;
              SETConsole.Undo();
              FocusCaret();
          }

          private void Space_Click(object sender, RoutedEventArgs e)
          {
              //KeyButton spaceKey = (KeyButton)sender;
              SETConsole.AppendText(" ");
              FocusCaret();
              if (keys[19].Background == Brushes.MediumSpringGreen)
                  keys[19].Background = Brushes.LightGray;
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