using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETKeyboard.GUI
{
    class DatabaseInteractor
    {
        private string FILE_PATH;
        private SQLiteConnection connection;
        private const int NUM_RESULTS = 5;

        public DatabaseInteractor()
        {
            this.FILE_PATH = @"database_en.db";
            this.connection = new SQLiteConnection("Data Source=" + FILE_PATH + ";Version=3;");
        }

        public string[] getNextWordPredictionsFor(string word1)
        {
            string sql = "select * from _2_gram " +
                         "where word_1 = '" + word1 + "' " +
                         "order by count desc;";
            string[] results = execute(sql);
            return results;
        }

        public string[] getNextWordPredictionsFor(string word1, string word2)
        {
            string sql = "select * from _3_gram " +
                         "where word_2 = '" + word1 + "' " +
                         "and word_1 = '" + word2 + "' " +
                         "order by count desc;";
            string[] results = execute(sql);
            return results;
        }

        public string[] getCompletionFor(string partialWord)
        {
            int partialWordLength = partialWord.Length;
            string sql = "select * from _1_gram " +
                         "where substr(word,1," + partialWordLength + ") = '" + partialWord + "' " +
                         "order by count desc;";
            string[] results = execute(sql);
            return results;
        }

        public string[] getCompletionFor(string word1, string partialWord)
        {
            int partialWordLength = partialWord.Length;
            string sql = "select * from _2_gram " +
                         "where word_1 = '" + word1 + "' and substr(word,1," + partialWordLength + ") = '" + partialWord + "' " +
                         "order by count desc;";
            string[] results = execute(sql);
            return results;
        }

        public string[] getCompletionFor(string word1, string word2, string partialWord)
        {
            int partialWordLength = partialWord.Length;
            string sql = "select * from _3_gram " +
                         "where word_2 = '" + word1 + "' and word_1 = '" + word2 + "' and substr(word,1," + partialWordLength + ") = '" + partialWord + "' " +
                         "order by count desc;";
            string[] results = execute(sql);
            return results;
        }

        private string[] execute(string sql)
        {
            string[] results = new string[NUM_RESULTS];

            try
            {
                connection.Open();
                DataSet ds = new DataSet();
                var da = new SQLiteDataAdapter(sql, connection);
                da.Fill(ds);

                DataTable dt = ds.Tables[0];

                int index = 0;
                while (index < dt.Rows.Count)
                {
                    if (index == NUM_RESULTS)
                    {
                        break;
                    }
                    else
                    {
                        DataRow row = dt.Rows[index];
                        results[index] = row["word"].ToString();
                        index++;
                    }
                }
            }
            catch
            {
            }

            connection.Close();
            return results;
        }
    }
}
