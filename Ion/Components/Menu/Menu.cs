using HotKeyManagement;
using Ion.Extensions;
using System.Diagnostics;
using System.Reflection;
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
        private static Dictionary<string, RoutedEventHandler>? _Functions;

        protected static Hub _Hub { get; private set; }
        protected static MainWindow _Window { get; private set; }
        protected static RichTextBox _Editor { get; private set; }
        protected static SideBar _SideBar { get; private set; }
        protected static TextEditor _TextEditor { get; private set; }

        protected static TextRange _Selection => _Editor.GetSelection();
        protected static TextPointer _ContentStart => _Editor.Document.ContentStart;
        protected static TextPointer _ContentEnd => _Editor.Document.ContentEnd;
        protected static bool _TextSelected => !_Editor.Selection.IsEmpty;


        public static void Initialize(Hub Hub)
        {
            Debug.WriteLine("Gettings references of components in Menu");

            MainWindow Window = Hub._Window;

            _Functions = new Dictionary<string, RoutedEventHandler>(50);

            _Hub = Hub;
            _Window = Window;
            _Editor = Window.TextEditor;
            _SideBar = Window._SideBar;

            _TextEditor = Hub._Editor;
        }


        public static void BindFunctions()
        {
            RoutedEventHandler GetEvent(string String)
            {
                if (_Functions.TryGetValue(String, out RoutedEventHandler? Function))
                {
                    return Function;
                }
                throw new Exception($"Function \"{String}\" not registered");
            }

            foreach (object? Item in _Window.Menu.Items)
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
            string MethodName = Event.Method.Name;

            Debug.WriteLine($"AddKey : {MethodName}");
            
            if (_Functions.ContainsKey(MethodName))
            {
                Debug.WriteLine($"!!!ERROR!!! Key \"{MethodName}\" alreay exists");
                return;
            }

            _Functions.Add(MethodName, Event);
        }
        protected void AddKey(Action Action, Key Key, ModifierKeys Modifiers, bool OverrideKeyDown = false)
        {
            string MethodName = Action.Method.Name;

            Debug.WriteLine($"AddKey : {MethodName} (_Action)");

            if (_Functions.ContainsKey(MethodName))
            {
                Debug.WriteLine($"!!!ERROR!!! Key (Action) \"{MethodName}\" alreay exists");
                return;
            }

            _Functions.Add(MethodName, (S, E) => Action());
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
        protected void AddHotKey(string Name, RoutedEventHandler Event, Key Key, ModifierKeys Modifiers, bool OverrideKeyDown = false)
        {
            Debug.WriteLine($"AddHotKey : {Name}");

            if (OverrideKeyDown)
            {
                _TextEditor.AddHotKey(Event, Key, Modifiers);
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
                    if (_Editor.Selection.IsEmpty)
                    {
                        AppendText(Text);
                    }
                    else
                    {
                        _Editor.Selection.Text = Text;
                    }
                    _Editor.Focus();
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
                    TextRange Range = new TextRange(_Editor.CaretPosition, _Editor.CaretPosition);
                    Range.Text = Text;
                    _Editor.CaretPosition = Range.End;
                    _Editor.Focus();
                },
                DispatcherPriority.Input
            );
        }


        protected TextRange GetAll()
        {
            return _Editor.GetAll();
        }

        protected TextRange GetSelection()
        {
            return _Editor.GetSelection();
        }

        protected TextRange GetSelectionOrAll()
        {
            TextRange Selection = _Editor.GetSelection();
            return Selection.IsEmpty ? _Editor.GetAll() : Selection;
        }

        protected TextRange GetSelectedLinesOrAll()
        {
            RichTextBox Editor = _Editor;
            TextRange Selection = GetSelectedLines();

            return Selection.IsEmpty ?
            GetAll() :
            new TextRange
            (
                Selection.Start.GetLineStartPosition(0) ?? Editor.Document.ContentStart,
                Selection.End.GetLineEndPosition()
            );
        }

        protected TextRange GetCurrentLine()
        {
            TextPointer Caret = _Editor.CaretPosition;

            return new TextRange
            (
                Caret.GetLineStartPosition(0),
                Caret.GetLineEndPosition()
            );
        }

        protected TextRange GetSelectedLines()
        {
            RichTextBox Editor = _Editor;
            TextSelection Selection = Editor.Selection;

            return Selection.IsEmpty ?
            GetCurrentLine() :
            new TextRange
            (
                Selection.Start.GetLineStartPosition(0) ?? Editor.Document.ContentStart,
                Selection.End.GetLineEndPosition()
            );
        }


        protected bool TryGetSelection(out TextRange Selection)
        {
            Selection = _Editor.GetSelection();
            return !Selection.IsEmpty;
        }


        protected void ConvertText(Func<string, string> Converter)
        {
            TextRange Range = GetSelectionOrAll();
            Range.Text = Converter(Range.Text);
            _Editor.DeSelect();
        }
        protected void ConvertText(Func<TextRange> GetRange, Func<string, string> Converter)
        {
            TextRange Range = GetRange();
            Range.Text = Converter(Range.Text);
            _Editor.DeSelect();
        }


        protected void OpenMenu(SideBarType Menu)
        {
            _SideBar.OpenMenu(Menu);
        }


        protected bool DocumentIsEmpty()
        {
            return _Editor.Document.IsEmpty();
        }
    }
}