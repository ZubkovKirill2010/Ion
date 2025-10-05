using System.IO;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ion
{
    public abstract class LanguageTab : Tab
    {
        public LanguageTab() : base() { }
        public LanguageTab(string Text) : base(Text) { }
        public LanguageTab(FlowDocument Document) : base(Document) { }


        protected override void Save()
        {
            StatusBar.Write($"{Translater._Current._StartOfSaving} \"{_CurrentFile}\"");
            Task.Run(async () =>
            {
                try
                {
                    await File.WriteAllTextAsync(_CurrentFile, GetText());
                    _IsSaved = true;
                    StatusBar.Write($"{Translater._Current._File} \"{_CurrentFile}\" {Translater._Current._FileSaved}");
                }
                catch
                {
                    StatusBar.Write($"{Translater._Current._SavingError} \"{_CurrentFile}\"");
                }
            });
        }

        public override void SaveAs()
        {
            Task.Run(async () =>
            {
                string? FilePath = GetFilePath();
                if (FilePath is not null)
                {
                    await File.WriteAllTextAsync(FilePath, GetText());
                }
            });
        }

        public override void ReadFile()
        {
            throw new NotImplementedException();
        }


        protected override string? GetFilePath()
            => GetFilePath("Text files (*.txt)|*.txt|Файлы C# (*.cs)|*.cs|Все файлы (*.*)|*.*", ".txt");
    }
}