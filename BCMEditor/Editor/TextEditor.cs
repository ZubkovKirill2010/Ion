using BCMEditor.Tabs;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace BCMEditor
{
    public sealed class TextEditor
    {
        private const double _MinLineHeight = 0.0034d;
        private bool _TextChanged = true;

        private readonly MainWindow _Window;
        public readonly RichTextBox _TextField;
        public Tab _CurrentTab;

        public TextEditor(MainWindow Window)
        {
            _Window = Window;
            _TextField = Window.TextEditor;

            Window.TextEditor.TextChanged += TextChanged;
            Window.TextEditor.SelectionChanged += (Sender, E) => Window.DeleteButton.IsEnabled = !_TextField.Selection.IsEmpty;

            Window.TextEditor.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, PasteText));
        }

        public void PasteText(object Sender, ExecutedRoutedEventArgs E)
        {
            E.Handled = true;

            if (Clipboard.ContainsText())
            {
                _Window.TextEditor.Selection.Text = Clipboard.GetText();
            }
        }

        public void Save() => _CurrentTab.SaveFile();
        public void SaveAs() => _CurrentTab.SaveAs();

        public void SetDocument(FlowDocument Document)
        {
            Document.LineHeight = _MinLineHeight;

            _TextChanged = false;
            _TextField.Document = Document;
            _TextChanged = true;
        }
        public void SetDocument(string Text)
        {
            var Document = new FlowDocument();
            Document.Blocks.Add(new Paragraph(new Run(Text)));

            SetDocument(Document);
        }

        public void TextChanged(object Sender, TextChangedEventArgs E)
        {
            if (_TextChanged)
            {
                _CurrentTab._IsSaved = false;
                _CurrentTab.TextChanged(Sender, E);
            }
        }
    }
}