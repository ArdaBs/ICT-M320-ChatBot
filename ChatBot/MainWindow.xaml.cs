using ChatBot.Classes;
using Microsoft.Win32;
using System;
using System.Windows.Media;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace ChatBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isSending = false;
        BotEngine bot = new BotEngine();
        Storage storage = new Storage();
        SoundPlayer receiveSound = new SoundPlayer("../../../Resources/Receive.wav");
        SoundPlayer emptyConversationSound = new SoundPlayer("../../../Resources/Empty_ConversationEdit.wav");
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            storage.Load();
            DataContext = storage;
            Application.Current.Exit += Current_Exit;
            Loaded += MainWindow_Loaded;
            InitializeConversation();
            AppSettings settings = LoadSettings();
            receiveSound.Load();
            emptyConversationSound.Load();

            // Checks settings and selects the right color with index
            if (settings.IsDarkMode)
            {
                SetDarkMode();
                ThemeColor.SelectedIndex = 0;
            }
            else
            {
                SetLightMode();
                ThemeColor.SelectedIndex = 1;
            }

            if (settings.IsSystemMode)
            {
                var systemMode = GetSystemThemeMode();
                if (systemMode == SystemThemeMode.Dark)
                {
                    SetDarkMode();
                    ThemeColor.SelectedIndex = 2;
                }
                else
                {
                    SetLightMode();
                    ThemeColor.SelectedIndex = 2;
                }
            }

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

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            storage.Save();
        }

        /// <summary>
        /// Checks the best matching word and responds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            if (isSending)
            {
                SendingMessageIndicator.Visibility = Visibility.Visible;
                return;
            }

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

            isSending = true;
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
                receiveSound.Play();

                ScrollSmoothlyToBottom();

                SendingMessageIndicator.Visibility = Visibility.Collapsed;
                isSending = false;
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
        private async void Clear_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                emptyConversationSound.PlaySync();
            }).Start();
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
                
            }
        }

        /// <summary>
        /// If combox value gets changed it checks what color app should have
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            if (selectedItem.Content.ToString() == "Hell")
            {
                SetLightMode();
                SaveSettings(false, false);
            }
            else if (selectedItem.Content.ToString() == "Dunkel")
            {
                SetDarkMode();
                SaveSettings(true, false);
            }
            else if (selectedItem.Content.ToString() == "System")
            {
                var systemMode = GetSystemThemeMode();
                if (systemMode == SystemThemeMode.Dark)
                {
                    SetDarkMode();
                    SaveSettings(true, true);
                }
                else
                {
                    SetLightMode();
                    SaveSettings(false, true);
                }
            }
        }

        /// <summary>
        /// Saves mode. If both false then LightMode
        /// </summary>
        /// <param name="isDarkMode"></param>
        /// <param name="isSystemMode"></param>
        private void SaveSettings(bool isDarkMode, bool isSystemMode)
        {
            AppSettings settings = new AppSettings
            {
                IsDarkMode = isDarkMode,
                IsSystemMode = isSystemMode,
            };

            //path for settings
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string chatBotFolder = Path.Combine(appDataPath, "ChatBot01");
            string settingsFilePath = Path.Combine(chatBotFolder, "Theme.xml");

            try
            {
                if (!Directory.Exists(chatBotFolder))
                {
                    Directory.CreateDirectory(chatBotFolder);
                }

                using (FileStream stream = new FileStream(settingsFilePath, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                    serializer.Serialize(stream, settings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern der Einstellungen: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Reads the setting file
        /// </summary>
        /// <returns></returns>
        private AppSettings LoadSettings()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string chatBotFolder = Path.Combine(appDataPath, "ChatBot01");
            string settingsFilePath = Path.Combine(chatBotFolder, "Theme.xml");

            if (File.Exists(settingsFilePath))
            {
                try
                {
                    using (FileStream stream = new FileStream(settingsFilePath, FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                        return (AppSettings)serializer.Deserialize(stream);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Lesen der Einstellungen: " + ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return new AppSettings();
        }

        /// <summary>
        /// Setting colors for important items
        /// </summary>
        private void SetLightMode()
        {
            Background = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            ChatBotTitle.Foreground = Brushes.Black;
            Grid1.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));
            Grid2.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));
            Grid3.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));
            ThemeColorBorder.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));
            ThemeColorBorder.BorderBrush = Brushes.Transparent;
            ThemeColor.Foreground = Brushes.Black;
            InputBorder.Background = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            InputBorder.BorderBrush = Brushes.Transparent;
            Input.Foreground = Brushes.Black;
            InputClearButton.Foreground = Brushes.Black;
        }

        /// <summary>
        /// Setting colors for important items
        /// </summary>
        private void SetDarkMode()
        {
            Background = new SolidColorBrush(Color.FromRgb(24, 23, 53));
            Grid1.Background = new SolidColorBrush(Color.FromRgb(15, 15, 45));
            Grid2.Background = new SolidColorBrush(Color.FromRgb(15, 15, 45));
            Grid3.Background = new SolidColorBrush(Color.FromRgb(15, 15, 45));
            ThemeColorBorder.Background = new SolidColorBrush(Color.FromRgb(44, 47, 51));
            InputBorder.Background = new SolidColorBrush(Color.FromRgb(44, 47, 51));

            ChatBotTitle.Foreground = Brushes.White;
            ThemeColor.Foreground = Brushes.White;
            Input.Foreground = Brushes.White;
            InputClearButton.Foreground = Brushes.White;
        }

        /// <summary>
        /// Gets information from system if its dark or light mode
        /// </summary>
        /// <returns></returns>
        private SystemThemeMode GetSystemThemeMode()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
            {
                if (key != null)
                {
                    object registryValue = key.GetValue("AppsUseLightTheme");
                    if (registryValue != null && (int)registryValue == 0)
                    {
                        return SystemThemeMode.Dark;
                    }
                }
            }
            return SystemThemeMode.Light;
        }

        public enum SystemThemeMode
        {
            Light,
            Dark
        }

        private void InputClearButton_Click(object sender, RoutedEventArgs e)
        {
            Input.Text = string.Empty;
        }
    }
}