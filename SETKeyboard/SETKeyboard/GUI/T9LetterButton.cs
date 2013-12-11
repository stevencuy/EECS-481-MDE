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

namespace SETKeyboard.GUI
{
    class T9LetterButton : Button
    {
        private string originalContent;
        private char[] lowerCase;
        private char[] upperCase;

        private int numLetters;
        public int index;

        //Generates set of upper and lower case characters based on input. Assumes letter input.
        public T9LetterButton()
        {
            this.index = 0;
        }

        public void setLetters(char[] letterSet)
        {

            this.originalContent = this.Content.ToString();
            numLetters = letterSet.Length;
            lowerCase = new char[letterSet.Length];
            upperCase = new char[letterSet.Length];
            for (int i = 0; i < numLetters; i++)
            {
                char c = letterSet[i];
                lowerCase[i] = char.ToLower(c);
                upperCase[i] = char.ToUpper(c);

            }
        }

        //Get current character in selection cycle.
        public char getCurrent(bool isLowerCase)
        {
            int tmp = index;
            if (tmp == (numLetters-1))
            {
                index = 0;
            }
            else
            {
                index++;
            }

            if (isLowerCase)
            {
                return lowerCase[tmp];
            }
            else
            {
                return upperCase[tmp];
            }
        }

        //Ends the current selection for a letter. Sets index back to zero.
        public void endSelection()
        {
            index = 0;
        }

        public string getOriginalContent(bool isLowerCase)
        {
            if (isLowerCase)
                return this.originalContent.ToUpper();

            return this.originalContent;
        }
    }
}