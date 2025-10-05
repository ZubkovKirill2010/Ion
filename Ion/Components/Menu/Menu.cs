using HotKeyManagement;
using Ion.Extensions;
using System.CodeDom;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using Zion;

namespace Ion
{
    public abstract class Menu
    {
        protected static readonly string _NewLine = Environment.NewLine;

        protected static Hub _Hub               { get; private set; }
        protected static MainWindow _Window     { get; private set; }
        protected static RichTextBox TextEditor { get; private set; }
        protected static SideBar _SideBar       { get; private set; }
        protected static TextEditor _Editor     { get; private set; }

        private static Dictionary<string, RoutedEventHandler>? _Functions;

        public static void Initialize(Hub Hub)
        {
            Debug.WriteLine("Gettings references of components in Menu");

            MainWindow Window = Hub._Window;

            _Functions = new Dictionary<string, RoutedEventHandler>(50);

            _Hub = Hub;
            _Window = Window;
            TextEditor = Window.TextEditor;
            _SideBar = Window._SideBar;

            _Editor = Hub._Editor;
        }


        public static void BindFunctions()
        {
            Debug.WriteLine("Binding functions");

            Debug.WriteLine("Registered functions:");
            foreach (string Key in _Functions.Keys)
            {
                Debug.WriteLine('\t' + Key ?? "null");
            }
            Debug.WriteLine("--->");

            RoutedEventHandler GetEvent(string String)
            {
                if (_Functions.TryGetValue(String, out var Function))
                {
                    return Function;
                }
                throw new Exception($"Function \"{String}\" not registered");
            }

            foreach (var Item in _Window.Menu.Items)
            {
                if (Item is MenuItem Menu)
                {
                    BindFunctions(Menu, GetEvent, null);
                }
            }
        }

        private static void BindFunctions(MenuItem Menu, Func<string, RoutedEventHandler> GetEvent, string ParentTag)
        {
            foreach (object Item in Menu.Items)
            {
                if (Item is MenuItem MenuItem)
                {
                    Debug.WriteLine($"Menu: \"{MenuItem.Header}\"");

                    if (MenuItem.Command is not null || MenuItem.Tag is null)
                    {
                        Debug.WriteLine($"Skipping function binding in menu \"{MenuItem.Header.ToNotNullString()}\"");
                        continue;
                    }

                    string Tag = MenuItem.Tag?.ToString() ?? ParentTag;

                    if (MenuItem.Items.Count == 0)
                    {
                        MenuItem.Click += GetEvent(Tag);
                    }
                    else
                    {
                        BindFunctions(MenuItem, GetEvent, Tag);
                    }
                }
            }
        }


        public abstract void Initialize();


        protected void AddKey(RoutedEventHandler Event)
        {
            Debug.WriteLine($"AddKey : {Event.Method.Name}");
            _Functions.Add(Event.Method.Name, Event);
        }
        protected void AddKey(Action Action, Key Key, ModifierKeys Modifiers, bool OverrideKeyDown = false)
        {
            AddKey((S, E) => Action());
            AddHotKey(Action.Method.Name, Action, Key, Modifiers, OverrideKeyDown);
        }
        protected void AddKey(RoutedEventHandler Event, Key Key, ModifierKeys Modifiers, bool OverrideKeyDown = false)
        {
            AddKey(Event);
            AddHotKey(Event.Method.Name, Event, Key, Modifiers, OverrideKeyDown);
        }
        protected void AddHotKey(string Name, Action Event, Key Key, ModifierKeys Modifiers, bool OverrideKeyDown = false)
        {
            AddHotKey(Name, (S, E) => Event(), Key, Modifiers, OverrideKeyDown);
        }
        protected void AddHotKey(string Name,RoutedEventHandler Event, Key Key, ModifierKeys Modifiers, bool OverrideKeyDown = false)
        {
            Debug.WriteLine($"AddHotKey : {Name}");

            if (OverrideKeyDown)
            {
                _Editor.AddHotKey(Event, Key, Modifiers);
            }
            else
            {
                LocalHotKeys.AddKey(Name, Key, Modifiers, Event);
            }
        }


        protected void InsertText(string Text)
        {
            _Window.Dispatcher.BeginInvoke
            (
                () =>
                {
                    if (TextEditor.Selection.IsEmpty)
                    {
                        AppendText(Text);
                    }
                    else
                    {
                        TextEditor.Selection.Text = Text;
                    }
                    TextEditor.Focus();
                },
                DispatcherPriority.Input
            );
        }

        protected void AppendText(string Text)
        {
            _Window.Dispatcher.BeginInvoke
            (
                () =>
                {
                    TextRange Range = new TextRange(TextEditor.CaretPosition, TextEditor.CaretPosition);
                    Range.Text = Text;
                    TextEditor.CaretPosition = Range.End;
                    TextEditor.Focus();
                },
                DispatcherPriority.Input
            );
        }


        protected TextRange GetRange()
        {
            RichTextBox Editor = TextEditor;

            return Editor.Selection.IsEmpty ?
                GetAllText() :
                Editor.Selection;
        }


        protected TextRange GetLine()
        {
            var Caret = TextEditor.CaretPosition;

            return new TextRange
            (
                Caret.GetLineStartPosition(0),
                Caret.GetLineEndPosition()
            );
        }

        protected TextRange GetLines()
        {
            var Editor = TextEditor;
            var Selection = Editor.Selection;

            return Selection.IsEmpty ?
                GetLine() :
                new TextRange
                (
                    Selection.Start.GetLineStartPosition(0) ?? Editor.Document.ContentStart,
                    Selection.End.GetLineEndPosition()
                );
        }

        protected TextRange GetAllText()
        {
            var Editor = _Editor._TextField;

            FlowDocument Document = Editor.Document;
            if (Document.IsEmpty())
            {
                return new TextRange(Editor.CaretPosition, Editor.CaretPosition);
            }

            return new TextRange(Document.ContentStart, Document.ContentEnd);
        }
    }
}