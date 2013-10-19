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
using System.Collections.Generic;

namespace SETKeyboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool shift = false;
        private List<KeyButton.KeyButton> keys = new List<KeyButton.KeyButton>();

        public MainWindow()
        {
            InitializeComponent();
            initializeT9();
            Loaded += delegate
            {
                InitializeQWERTY(bPanel.ActualHeight, bPanel.ActualWidth);
            };
        }

        private void sizeChanged(object sender, RoutedEventArgs e)
        {
            QWERTYGrid.Children.Clear();
            InitializeQWERTY(bPanel.ActualHeight, bPanel.ActualWidth);
        }

        private void InitializeQWERTY(double height, double width)
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
            int button_width = (int)(width / 10) - 1, button_height = (int)(height / 4) - 15;
            int margin_inc = button_width;
            int[] margins_l = new int[3];
            margins_l[0] = 0;
            margins_l[1] = button_width / 2;
            margins_l[2] = (button_width / 2) * 3;

            int index = 0;
            for (int row = 0; row < row_size; ++row)
            {
                for (int col = 0; col < col_size; ++col)
                {
                    string lower = keyStrings[index];
                    string upper = lower.ToUpper();
                    KeyButton.KeyButton b;
                    if (row != 2)
                    {
                        b = new KeyButton.KeyButton(upper, lower, button_width, button_height, margins_l[row] + margin_inc * col, row * button_height, 0, 0);
                        b.Click += new RoutedEventHandler(KeyHit_Click);
                    }
                    else
                    {
                        if (lower == "shift")
                        {
                            b = new KeyButton.KeyButton(upper, lower, (button_width / 2) * 3, button_height, 0, row * button_height, 0, 0);
                            b.Click += new RoutedEventHandler(Shift_Click);
                        }
                        else if (lower == "back")
                        {
                            b = new KeyButton.KeyButton(upper, lower, (button_width / 2) * 3, button_height, margins_l[row] + margin_inc * (col - 1), row * button_height, 0, 0);
                            b.Click += new RoutedEventHandler(Backspace_Click);
                        }
                        else
                        {
                            b = new KeyButton.KeyButton(upper, lower, button_width, button_height, margins_l[row] + margin_inc * (col - 1), row * button_height, 0, 0);
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
            KeyButton.KeyButton spacebar = new KeyButton.KeyButton(keyStrings[28].ToUpper(), keyStrings[28], 5 * button_width, button_height, (button_width / 2) * 5, button_height * 3, 0, 0);
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
              KeyButton.KeyButton letterKey = (KeyButton.KeyButton)sender;
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


        //T9 KeyBoard Actions
        T9LetterButton lastButtonPressed;
        String consoleText;
        bool isLowerCase;
        List<T9LetterButton> letterButtons;

        private void initializeT9()
        {
            isLowerCase = true;
            consoleText = "";

            letterButtons = new List<T9LetterButton>();
            abcButton.setLetters(new char[] { 'a', 'b', 'c' });
            letterButtons.Add(abcButton);
            defButton.setLetters(new char[] { 'd', 'e', 'f' });
            letterButtons.Add(defButton);
            ghiButton.setLetters(new char[] { 'g', 'h', 'i' });
            letterButtons.Add(ghiButton);
            jklButton.setLetters(new char[] { 'j', 'k', 'l' });
            letterButtons.Add(jklButton);
            mnoButton.setLetters(new char[] { 'm', 'n', 'o' });
            letterButtons.Add(mnoButton);
            pqrsButton.setLetters(new char[] { 'p', 'q', 'r', 's' });
            letterButtons.Add(pqrsButton);
            tuvButton.setLetters(new char[] { 't', 'u', 'v' });
            letterButtons.Add(tuvButton);
            wxyzButton.setLetters(new char[] { 'w', 'x', 'y', 'z' });
            letterButtons.Add(wxyzButton);

            lastButtonPressed = abcButton;
        }

        private void toUpperClick(object sender, RoutedEventArgs e)
        {
            toggleCase();
        }

        private void toggleCase()
        {
            for (int i = 0; i < 8; i++)
            {
                String contentText = letterButtons.ElementAt(i).Content.ToString();
                if (isLowerCase)
                {
                    contentText = contentText.ToUpper();
                }
                else
                {
                    contentText = contentText.ToLower();
                }
                letterButtons.ElementAt(i).Content = contentText;
            }
            isLowerCase = !isLowerCase;
        }

        private void T9LetterClickEvent(object sender, RoutedEventArgs e)
        {
            T9LetterButton buttonPressed = (SETKeyboard.T9LetterButton)sender;
            
            char letter;
            if (!sender.Equals(lastButtonPressed))
            {
                lastButtonPressed.endSelection();
                lastButtonPressed = buttonPressed;
                
                letter = buttonPressed.getCurrent(isLowerCase);
                consoleText += letter;
            }
            else
            {
                lastButtonPressed = buttonPressed;
                letter = buttonPressed.getCurrent(isLowerCase);
                if (consoleText == "")
                {
                    consoleText = letter.ToString();
                }
                else
                {
                    this.backSpaceAction();
                    consoleText += letter;
                }
            }


            this.updateConsole();
        }

        private void updateConsole()
        {
            //Consider using something besides a rich text field
            SETConsole.Document.Blocks.Clear();
            SETConsole.Document.Blocks.Add(new Paragraph(new Run(consoleText)));
            FocusCaret();
        }

        private void backSpaceAction()
        {
            if ((consoleText.Length == 1) || (consoleText.Length == 0))
            {
                consoleText = "";
            }
            else
            {
                consoleText = consoleText.Substring(0, consoleText.Length - 1);
            }
        }

        private void nextLetterEvent(object sender, RoutedEventArgs e)
        {
            lastButtonPressed.endSelection();
            lastButtonPressed = new T9LetterButton();
        }

        private void backSpaceClick(object sender, RoutedEventArgs e)
        {
            this.backSpaceAction();
            lastButtonPressed.endSelection();
            this.updateConsole();
        }

        private void spaceClick(object sender, RoutedEventArgs e)
        {
            consoleText = consoleText + " ";
            this.updateConsole();
        }
    }
}