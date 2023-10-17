namespace ChatBotConsole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BotEngine
    {
        private List<Message> messages;

        public BotEngine()
        {
            Messages messagesData = new Messages();
            messagesData.Load();
            this.messages = messagesData.List;
        }

        public string GetAnAnswer(string input)
        {
            string answer = "";

            string bestMatch = messages.Select(m => m.Keyword).OrderBy(keyword => ComputeLevenshteinDistance(input, keyword)).First();

            int distance = ComputeLevenshteinDistance(input, bestMatch);

            if (distance <= 3)
            {
                answer = messages.First(m => m.Keyword == bestMatch).Answer;
            }
            else
            {
                answer = "Ich habe Sie nicht verstanden.";
            }

            return answer;
        }

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
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[a.Length, b.Length];
        }
    }

}