namespace ChatBotConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            BotEngine bot = new BotEngine();
            Storage storage = new Storage();
            storage.Load();

            foreach (var conversation in storage.List)
            {
                Console.WriteLine("Benutzer: " + conversation.User);
                Console.WriteLine("ChatBot: " + conversation.ChatBot);
                Console.WriteLine();
            }

            while (true)
            {
                Console.Write("Bitte geben Sie eine Nachricht ein (oder 'exit' zum Beenden): ");
                string keyword = Console.ReadLine();

                if (keyword.ToLower() == "exit")
                {
                    storage.Save();
                    break;
                }

                if (keyword.ToLower() == "clear")
                {
                    storage.Clear();
                    Console.Clear();
                    Console.WriteLine("Konversationsverlauf gelöscht.");
                    continue;
                }

                string answer = bot.GetAnAnswer(keyword);

                storage.List.Add(new Storage.Conversation { User = keyword, ChatBot = answer });

                Console.WriteLine("Antwort: " + answer);
            }
        }
    }
}


