using System;

namespace ChatBot.Classes
{
    [Serializable]
    public class AppSettings
    {
        public bool IsDarkMode { get; set; }
        public bool IsSystemMode { get; set; }
    }

    public enum SystemThemeMode
    {
        Light,
        Dark
    }
}
