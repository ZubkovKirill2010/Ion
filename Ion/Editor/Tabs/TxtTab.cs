using System.IO;
using System.Windows.Documents;
using System.Text;

namespace Ion.Tabs
{
    public sealed class TxtTab : Tab
    {
        public TxtTab() { }
        public TxtTab(string Text) : base(Text) { }
        public TxtTab(FlowDocument Document) : base(Document) { }

        public override void ReadFile()
        {
            try
            {
                SetDocument
                (
                    File.ReadAllText(_CurrentFile)
                );
                _IsSaved = true;
            }
            catch (Exception Exception)
            {
                MainWindow.Log($"{Translater._Current._SavingError} \"{_CurrentFile}\"");
                MainWindow.LogError(Exception);
            }
        }


        protected override string? GetFilePath()
            => GetFilePath("Text files (*.txt)|*.txt|Файлы C# (*.cs)|*.cs|Все файлы (*.*)|*.*", ".txt");
    }
}