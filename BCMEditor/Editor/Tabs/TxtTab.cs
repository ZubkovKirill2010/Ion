using System.IO;
using System.Windows.Documents;

namespace BCMEditor.Tabs
{
    public sealed class TxtTab : Tab
    {
        public TxtTab() : base() { }
        public TxtTab(string Text) : base(Text) { }
        public TxtTab(FlowDocument Document) : base(Document) { }

        public override void ReadFile()
        {
            SetDocument
            (
                File.ReadAllText(_CurrentFile)
            );
        }


        protected override string? GetFilePath()
            => GetFilePath("Текстовые файлы (*.txt)|*.txt|Файлы C# (*.cs)|*.cs|Все файлы (*.*)|*.*", ".txt");
    }
}