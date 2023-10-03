using System;
using NAudio.Wave;
using System.Media;
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
using System.IO;
using ChatBot.Classes;

namespace ChatBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BotEngine bot = new BotEngine();
        Storage storage = new Storage();
        SoundPlayer soundPlayer = new SoundPlayer("../../../Resources/Receive.wav");
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            storage.Load();
            DataContext = storage;
            Application.Current.Exit += Current_Exit;
            Loaded += MainWindow_Loaded;

            InitializeConversation();

            Uri iconUri = new Uri("pack://application:,,,/ChatBot;component/Resources/ChatBotLogo.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
        }

        

        /// <summary>
        /// Function to scroll to bottom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);
            ScrollSmoothlyToBottom();
        }

        private void HideWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }


        private void Current_Exit(object sender, ExitEventArgs e)
        {
            storage.Save();
        }

        /// <summary>
        /// checks if its normal o maximized and changes the 
        /// value to the oposite mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                Input.FontSize = 16;
            }
            else
            {
                WindowState = WindowState.Normal;
                Input.FontSize = 12;
            }
        }

        /// <summary>
        /// CLoses this.Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Checks the best matching word and responds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void 
            Send_Click(object sender, RoutedEventArgs e)
        {
            string userInput = Input.Text;

            storage.List.Add(new Storage.Conversation { User = userInput, Assistant = string.Empty });

            Input.Text = string.Empty;

            ConversationDisplay.ItemsSource = null;
            ConversationDisplay.ItemsSource = storage.List;

            Random random = new Random();
            int delayMilliseconds = random.Next(1000, 3001);

            int numSteps = delayMilliseconds / 500;
            string chatbotResponse = string.Empty;

            for (int i = 0; i < numSteps; i++)
            {
                chatbotResponse += ".";
                var lastConversation = storage.List.LastOrDefault();
                if (lastConversation != null)
                {
                    lastConversation.Assistant = chatbotResponse;
                    ConversationDisplay.Items.Refresh();
                    ScrollSmoothlyToBottom();
                }
                await Task.Delay(500);
            }

            
            chatbotResponse = bot.GetAnAnswer(userInput);
            soundPlayer.PlaySync();


            var lastConversationFinal = storage.List.LastOrDefault();
            if (lastConversationFinal != null)
            {
                lastConversationFinal.Assistant = chatbotResponse;
                ConversationDisplay.Items.Refresh();

                ScrollSmoothlyToBottom();
            }
        }

        /// <summary>
        /// Clearing conversation after click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            storage.Clear();

            ConversationDisplay.ItemsSource = null;
        }

        /// <summary>
        /// Function to make a smoth movement
        /// </summary>
        private async void ScrollSmoothlyToBottom()
        {
            double currentVerticalOffset = ConversationScrollViewer.VerticalOffset;
            double targetVerticalOffset = ConversationScrollViewer.ScrollableHeight;

            while (currentVerticalOffset < targetVerticalOffset)
            {
                double step = 10.0;
                currentVerticalOffset += step;

                if (currentVerticalOffset > targetVerticalOffset)
                    currentVerticalOffset = targetVerticalOffset;

                ConversationScrollViewer.ScrollToVerticalOffset(currentVerticalOffset);

                await Task.Delay(10);
            }
        }

        /// <summary>
        /// ChatBot answers if nothing has been typed
        /// </summary>
        private void InitializeConversation()
        {
            if (storage.List.Count == 0)
            {
                storage.List.Add(new Storage.Conversation { User = "Hey ChatBot", Assistant = "Hey Meister, wie kann ich Ihnen helfen?", IsUserMessage = false });
                ConversationDisplay.ItemsSource = storage.List;
            }
        }
    }
}