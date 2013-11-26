using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETKeyboard.GUI
{
    class PredictionHandler
    {
        private DatabaseInteractor databaseInteractor;

        public PredictionHandler()
        {
            this.databaseInteractor = new DatabaseInteractor();
        }

        public string[] getPredictions(Query query)
        {
            if (query.getStatus() == ContextStatus.EMPTY)
            {
                string[] b = { };
                return b;
            }
            else
            {
                if (query.getThird() == "")
                {
                    return wordPrediction(query);
                }
                else
                {
                    return wordCompletion(query);
                }
            }
        }

        private string[] wordCompletion(Query query)
        {
            Console.WriteLine("Making Completion");
            if (query.getStatus() == ContextStatus.ONEGRAM)
            {
                string searchTerm = query.getThird();
                return databaseInteractor.getCompletionFor(searchTerm);
            }
            else if (query.getStatus() == ContextStatus.TWOGRAM)
            {
                string context = query.getSecond();
                string searchTerm = query.getThird();

                string[] results;

                results = databaseInteractor.getCompletionFor(context, searchTerm);
                if (results[0] == null)
                {
                    results = databaseInteractor.getCompletionFor(searchTerm);
                }
                return results;
            }
            else if (query.getStatus() == ContextStatus.THREEGRAM)
            {
                string context1 = query.getFirst();
                string context2 = query.getSecond();
                string searchTerm = query.getThird();

                string[] results;
                results = databaseInteractor.getCompletionFor(context1, context2, searchTerm);
                if (results[0] == null)
                {
                    results = databaseInteractor.getCompletionFor(context2, searchTerm);
                }
                if (results[0] == null)
                {
                    results = databaseInteractor.getCompletionFor(searchTerm);
                }
                return results;
            }
            else
            {
                string[] empty = { };
                return empty;
            }
        }

        private string[] wordPrediction(Query query)
        {
            Console.WriteLine("Making Next Word Prediction");

            if (query.getStatus() == ContextStatus.TWOGRAM)
            {
                string searchTerm = query.getSecond();
                string[] results = databaseInteractor.getNextWordPredictionsFor(searchTerm);
                return results;
            }
            else if (query.getStatus() == ContextStatus.THREEGRAM)
            {
                string firstTerm = query.getFirst();
                string secondTerm = query.getSecond();
                string[] results = databaseInteractor.getNextWordPredictionsFor(firstTerm, secondTerm);
                if (results[0] == null)
                {
                    results = databaseInteractor.getNextWordPredictionsFor(secondTerm);
                }
                return results;
            }
            else
            {
                string[] empt = { };
                return empt;
            }
        }
    }
}
