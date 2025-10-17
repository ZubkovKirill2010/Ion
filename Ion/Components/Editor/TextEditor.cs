using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Ion
{
    public sealed class TextEditor
    {
        private const double _MinLineHeight = 0.0034d;
        private bool _TextChanged = false;

        private readonly MainWindow _Window;
        public readonly RichTextBox _TextField;
        public Tab _CurrentTab;

        private readonly Dictionary<int, RoutedEventHandler?> _Functions;


        public TextEditor(MainWindow Window)
        {
            RichTextBox Editor = Window.TextEditor;

            _Window = Window;
            _TextField = Editor;

            _Functions = new Dictionary<int, RoutedEventHandler?>(20)
            {
                { ToHotKey(Key.U, ModifierKeys.Control), null }
            };
            Window.TextEditor.TextChanged += TextChanged;
            Window.PreviewKeyDown += KeyDown;
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
            Debug.WriteLine("Set Document");
            Document.LineHeight = _MinLineHeight;

            _TextChanged = false;
            _TextField.Document = Document;
            _TextChanged = true;
        }
        public void SetDocument(string Text)
        {
            FlowDocument Document = new FlowDocument();
            Document.Blocks.Add(new Paragraph(new Run(Text)));

            SetDocument(Document);
        }


        public void AddHotKey(Action Event, Key Key, ModifierKeys Modifiers)
        {
            AddHotKey((S, E) => Event(), Key, Modifiers);
        }
        public void AddHotKey(RoutedEventHandler Event, Key Key, ModifierKeys Modifiers)
        {
            int HotKey = ToHotKey(Key, Modifiers);
            _Functions.Add(HotKey, Event);
        }


        private void KeyDown(object sender, KeyEventArgs E)
        {
            int HotKey = ToHotKey(E.Key, Keyboard.Modifiers);

            if (_Functions.TryGetValue(HotKey, out RoutedEventHandler? Event))
            {
                if (Event is not null)
                {
                    Event(this, E);
                }
                E.Handled = true;
            }
        }

        private void TextChanged(object Sender, TextChangedEventArgs E)
        {
            if (_TextChanged)
            {
                _CurrentTab._IsSaved = false;
                _CurrentTab.TextChanged(Sender, E);
            }
        }

        public void Focus()
        {
            _TextField.Focus();
        }


        private static int ToHotKey(Key Key, ModifierKeys Modifiers)
        {
            return (int)Modifiers << 16 | (int)Key;
        }
    }
}