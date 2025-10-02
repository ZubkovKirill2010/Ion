using System.IO;
using System.Text.Json;

namespace Ion
{
    public sealed class Localization
    {
        private static string _LocalizationsPath = $"{MainWindow._AssetsPath}\\Localization";

        public string _WithoutName { get; set; } = "Without a name";//Без имени
        public string _NotSavedFile { get; set; } = "Unsaved file";//не сохранённый файл

        public string _File { get; set; } = "File";
        public string _StartOfSaving { get; set; } = "Start saving a file";//Начало сохранения файла
        public string _SaveFile { get; set; } = "Save file";//Сохранить файл

        public string _EmptySelection { get; set; } = "Empty selection";
        public string _EmptyText { get; set; } = "Empty text";
        public string _EmptyDocument { get; set; } = "Empty document";
        public string _StructureParsingError { get; set; } = "Structure conversion error";

        public string _PrintingDocument { get; set; } = "Printing document";
        public string _PrintingError { get; set; } = "Printing error";

        public string _FileSaved { get; set; } = "saved";//сохранён
        public string _OpenFile { get; set; } = "Open file";//открыть файл
        public string _Not { get; set; } = "not";//не
        public string _BracketsPlaces { get; set; } = "The brackets are placed";//скобки расставлены
        public string _Right { get; set; } = "right";//верно
        public string _NotExists { get; set; } = "not exists";//не существует

        public string _UnknowException { get; set; } = "Unknown exception";//Неизвестное исключение
        public string _SavingError { get; set; } = "Error when saving";
        public string _LoadingError { get; set; } = "Error when reading";
        public string _InsertError { get; set; } = "Error when inserting text";//Ошибка при вставке текста
        public string _ReloadError { get; set; } = "Cannot reload an unsaved file"; //Нельзя перезагрузить не сохранённый 
        public string _RegexError { get; set; } = "Error in the regular expression";
        public string _ErrorInSearch { get; set; } = "Error in the search";
        public string _ReplaceError { get; set; } = "An error occurred during the replacement";
        public string _OldTextEqualNew { get; set; } = "The old text matches the new one";

        public string _EnterSearchText { get; set; } = "Enter the search text";

        public string _Settings { get; set; } = "Settings";
        public string _Replacement { get; set; } = "Replacement";
        public string _Send { get; set; } = "Send";
        public string _Search { get; set; } = "Search";
        public string _Replace { get; set; } = "Replace";
        public string _Exit { get; set; } = "Exit";

        public string _Replaced { get; set; } = "Replaced";
        public string _MatchesFound { get; set; } = "Matches found";
        public string _MatchesNotFound { get; set; } = "No matches found";

        public string _Cancel { get; set; } = "Cancel";
        public string _Apply { get; set; } = "Apply";


        public static async Task<Localization> LoadFromAsync(string? Language)
        {
            if (string.IsNullOrEmpty(Language))
            {
                return new Localization();
            }

            string Path = $"{_LocalizationsPath}\\{Language}.json";

            if (!File.Exists(Path))
            {
                return new Localization();
            }

            try
            {
                using (FileStream Stream = new FileStream(Path, FileMode.Open))
                {
                    return await JsonSerializer.DeserializeAsync<Localization>(Stream) ?? new Localization();
                }
            }
            catch (Exception Exception)
            {
                StatusBar.WriteError(Exception);
                return new Localization();
            }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}