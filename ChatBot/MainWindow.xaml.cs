using System;
using ChatBot;
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

namespace ChatBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BotEngine bot = new BotEngine();
        Storage storage = new Storage();
        public MainWindow()
        {
            InitializeComponent();
            storage.Load();
            DataContext = storage;
            Application.Current.Exit += Current_Exit;
        }
        private void Current_Exit(object sender, ExitEventArgs e)
        {
            storage.Save();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string userInput = Input.Text;

            string chatbotResponse = bot.GetAnAnswer(userInput);

            //For displaying as bubble
            storage.List.Add(new Storage.Conversation { User = userInput, Assistant = chatbotResponse });

            Input.Text = string.Empty;

            ConversationDisplay.ItemsSource = null;
            ConversationDisplay.ItemsSource = storage.List;
        }


        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            storage.Clear();

            ConversationDisplay.ItemsSource = null;
            Console.WriteLine("Conversation cleared.");
        }


    }


}

