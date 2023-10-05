using ChatBot.Classes;
using System;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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

        /// <summary>
        /// If button pressed window gets darker until its minimized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);

            if (window.WindowState == WindowState.Normal)
            {
                // Darken the window before minimizing
                DoubleAnimation darkenAnimation = new DoubleAnimation();
                darkenAnimation.From = 1.0;
                darkenAnimation.To = 0.5;
                darkenAnimation.Duration = TimeSpan.FromSeconds(0.5);

                // Add a completed event handler to minimize the window when the animation finishes
                darkenAnimation.Completed += (s, args) =>
                {
                    window.WindowState = WindowState.Minimized;

                    DoubleAnimation resetAnimation = new DoubleAnimation();
                    resetAnimation.From = 0.5;
                    resetAnimation.To = 1.0;
                    resetAnimation.Duration = TimeSpan.FromSeconds(0.5);

                    window.BeginAnimation(Window.OpacityProperty, resetAnimation);
                };

                window.BeginAnimation(Window.OpacityProperty, darkenAnimation);
            }
            else if (window.WindowState == WindowState.Minimized)
            {
                DoubleAnimation restoreAnimation = new DoubleAnimation();
                restoreAnimation.From = 0.5;
                restoreAnimation.To = 1.0;
                restoreAnimation.Duration = TimeSpan.FromSeconds(0.5);

                // Add a completed event handler to set the window state to Normal when the animation finishes
                restoreAnimation.Completed += (s, args) =>
                {
                    window.WindowState = WindowState.Normal;
                };

                // Apply the restore animation to the windows opacity property
                window.BeginAnimation(Window.OpacityProperty, restoreAnimation);
            }
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
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string userInput = Input.Text.Trim();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                return;
            }

            DateTime currentTime = DateTime.Now;

            storage.List.Add(new Storage.Conversation { User = userInput, Assistant = string.Empty, Timestamp = currentTime });

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
            


            var lastConversationFinal = storage.List.LastOrDefault();
            if (lastConversationFinal != null)
            {
                lastConversationFinal.Assistant = chatbotResponse;
                ConversationDisplay.Items.Refresh();
                soundPlayer.Play();

                ScrollSmoothlyToBottom();
            }
        }

        /// <summary>
        /// Allows to send also with "Enter"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Send_Click(sender, e);
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
            InitializeConversation();
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
                DateTime currentTime = DateTime.Now;

                storage.List.Add(new Storage.Conversation { User = "Hey ChatBot", Assistant = "Hallo Meister, wie kann ich Ihnen helfen?", Timestamp = currentTime, IsUserMessage = false });
                ConversationDisplay.ItemsSource = storage.List;
                soundPlayer.Play();
            }
        }
    }
}