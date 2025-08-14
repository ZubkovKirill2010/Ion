using BCMEditor.Tabs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BCMEditor
{
    public sealed class TextEditor
    {
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

        public void TextChanged(object Sender, TextChangedEventArgs E)
        {
            _CurrentTab._IsSaved = false;
            _CurrentTab.TextChanged(Sender, E);
        }
    }
}