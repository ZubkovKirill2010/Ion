using System.Diagnostics;
using System.IO;

namespace Ion
{
    public static class Translater
    {
        private static readonly string _LocalizationPath = $"{MainWindow._AssetsPath}\\Localization";

        public static Localization _Current;
        public static string? _Language { get; private set; }

        public static async Task Initialize(string? Language)
        {
            Debug.WriteLine("Загрузка переводов");
            _Current = await Localization.LoadFromAsync(Language);
            _Language = Language;
        }
        public static void Initialize(MainWindow Window)
        {
            //Инитиализация объектов (xaml)
        }

        public static string[] GetAllLanguage()
        {
            return Directory.GetDirectories(_LocalizationPath);
        }

        public static bool ExistsLanguage(string Language)
        {
            return Directory.Exists($"{_LocalizationPath}\\{Language}.json");
        }
    }
}