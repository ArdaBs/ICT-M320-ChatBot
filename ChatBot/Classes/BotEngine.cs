using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Classes
{
    public class BotEngine
    {
        private List<Message> messages;

        /// <summary>
        /// Construktor
        /// </summary>
        public BotEngine()
        {
            Messages messagesData = new Messages();
            messagesData.Load();
            messages = messagesData.List;
        }

        /// <summary>
        /// Getting the matching keywoard with max 3 mistakes,
        /// also possible without limit of 3
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetAnAnswer(string input)
        {
            string answer = "";

            string bestMatch = messages.Select(m => m.Keyword).OrderBy(keyword => ComputeLevenshteinDistance(input, keyword)).First();

            int distance = ComputeLevenshteinDistance(input, bestMatch);

            //Can be changed
            if (distance <= 3)
            {
                answer = messages.First(m => m.Keyword == bestMatch).Answer;
            }
            else
            {
                answer = "Es tut mir leid, ich habe Sie nicht verstanden.";
            }

            return answer;
        }

        /// <summary>
        /// Calculating if the word matches the keywoard
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int ComputeLevenshteinDistance(string a, string b)
        {
            int[,] matrix = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++)
            {
                matrix[i, 0] = i;
            }

            for (int j = 0; j <= b.Length; j++)
            {
                matrix[0, j] = j;
            }

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[a.Length, b.Length];
        }
    }
}
