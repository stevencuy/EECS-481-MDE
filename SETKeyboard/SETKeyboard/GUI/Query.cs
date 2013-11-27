using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETKeyboard.GUI
{
    class Query
    {
        private string first;
        private string second;
        private string third;

        public Query()
        {
            this.first = null;
            this.second = null;
            this.third = null;
        }

        public void setQuery(string context)
        {
            char[] blah = { ' ', '.' };

            string[] wordList = context.Split(blah);
            int numWords = wordList.Length - 1;

            int c = 2;
            string[] b = new string[3];
            for (int i = numWords; i >= 0; i--)
            {
                string word = wordList[i];
                if (word.Equals(" ") || (c == -1))
                {
                    break;
                }
                else
                {
                    string lower = wordList[i].ToLower();
                    b[c] = lower;
                    c--;
                }
            }

            setThird(b[2]);
            setSecond(b[1]);
            setFirst(b[0]);

            Console.WriteLine("Query On:" + getFirst() + " " + getSecond() + " " + getThird());
        }

        public ContextStatus getStatus()
        {
            if ((first == null) && (second == null) && (third == null))
            {
                return ContextStatus.EMPTY;
            }
            else if ((first == null) && (second == null) && (third != null))
            {
                return ContextStatus.ONEGRAM;
            }
            else if ((first == null) && (second != null) && (third != null))
            {
                return ContextStatus.TWOGRAM;
            }
            else
            {
                return ContextStatus.THREEGRAM;
            }
        }

        public void setFirst(string word)
        {
            this.first = word;
        }
        public string getFirst()
        {
            return this.first;
        }

        public void setSecond(string word)
        {
            this.second = word;
        }
        public string getSecond()
        {
            return this.second;
        }

        public void setThird(string word)
        {
            this.third = word;
        }
        public string getThird()
        {
            return this.third;
        }

    }

    enum ContextStatus
    {
        EMPTY,
        ONEGRAM,
        TWOGRAM,
        THREEGRAM
    }
}
