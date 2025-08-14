using System.IO;
using System.Windows.Documents;

namespace BCMEditor.Tabs
{
    public abstract class LanguageTab : Tab
    {
        public LanguageTab() : base() { }
        public LanguageTab(string Text) : base(Text) { }
        public LanguageTab(FlowDocument Document) : base(Document) { }


        protected override void Save()
        {
            MainWindow.Log($"Начало сохранения файла \"{_CurrentFile}\"");
            Task.Run(async () =>
            {
                try
                {
                    await File.WriteAllTextAsync(_CurrentFile, GetText());
                    _IsSaved = true;
                    MainWindow.Log($"Файл \"{_CurrentFile}\" успешно сохранён");
                }
                catch
                {
                    MainWindow.Log($"Ошибка при сохранении \"{_CurrentFile}\"");
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
            => GetFilePath("Текстовые файлы (*.txt)|*.txt|Файлы C# (*.cs)|*.cs|Все файлы (*.*)|*.*", ".txt");
    }
}