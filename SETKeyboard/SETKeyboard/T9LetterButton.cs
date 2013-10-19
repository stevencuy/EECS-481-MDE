using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SETKeyboard
{
    class T9LetterButton : Button
    {
        private char[] lowerCase;
        private char[] upperCase;

        private int numLetters;
        private int index;

        /*Generates set of upper and lower case characters based on input. Assumes letter input.
         */
        public T9LetterButton()
        {
            this.index = 0;
        }

        public void setLetters(char[] letterSet)
        {
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


        /*Get current character in selection cycle.
         */
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

        /*Ends the current selection for a letter. Sets index back to zero.
         */
        public void endSelection()
        {
            index = 0;
        }

    }

}
