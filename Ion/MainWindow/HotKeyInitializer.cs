using HotKeyManagement;
using System.Windows;
using System.Windows.Input;

namespace Ion
{
    public partial class MainWindow : Window
    {
        public void InitializeHotKeys()
        {
            LocalHotKeys.Initialize(this);

            InitalizeFileHotKeys();
            InitalizeViewHotKeys();
            InitalizeEditHotKeys();
            InitalizeInsertsHotKeys();
            InitalizePunctuationHotKeys();
            InitalizeStructuringHotKeys();
            InitalizeViewHotKeys();
            InitalizeTabsKeys();
            InitializeNumbersKeys();
        }

        private void InitalizeFileHotKeys()
        {
            AddKey("New", Key.N, ModifierKeys.Control, AddTab);
            AddKey("Open", Key.O, ModifierKeys.Control, Open);

            AddKey("Save", Key.S, ModifierKeys.Control, Save);
            AddKey("SaveAs", Key.S, ModifierKeys.Control | ModifierKeys.Shift, SaveAs);
            AddKey("SaveAll", Key.S, ModifierKeys.Alt, SaveAll);

            AddKey("Print", Key.P, ModifierKeys.Control, Print);
            AddKey("Send", Key.T, ModifierKeys.Control, Send);

            AddKey("CloseCurrentTab", Key.C, ModifierKeys.Alt, CloseCurrentTab);
        }
        private void InitalizeEditHotKeys()
        {
            AddKey("Find", Key.F, ModifierKeys.Control, Find);
            AddKey("Replace", Key.H, ModifierKeys.Control, Replace);
            //AddKey("GoTo", Key.G, ModifierKeys.Control, GoTo);

            AddKey("Highlight", Key.M, ModifierKeys.Control, Highlight);

            AddKey("Do", Key.D, ModifierKeys.Control, Do);
            AddKey("DuplicateLine", Key.D, ModifierKeys.Control | ModifierKeys.Shift, DuplicateLine);
        }
        private void InitalizeInsertsHotKeys()
        {
            AddKey("Time/Date", Key.F5, ModifierKeys.None, InsertTimeDate);
        }
        private void InitalizePunctuationHotKeys()
        {
            AddKey("RemoveEmptyLines", Key.Delete, ModifierKeys.Control | ModifierKeys.Shift, RemoveEmptyLines);

            AddKey("CheckBrackets", Key.B, ModifierKeys.Control | ModifierKeys.Shift, CheckBrackets);
            AddKey("CorrectSpaces", Key.K, ModifierKeys.Control | ModifierKeys.Shift, CorrectSpaces);
            AddKey("CorrectPunctuation", Key.P, ModifierKeys.Control | ModifierKeys.Shift, CorrectPunctuation);
        }
        private void InitalizeStructuringHotKeys()
        {
            AddKey("Enumerate", Key.L, ModifierKeys.Control, Enumerate);
            AddKey("Enumerate", Key.L, ModifierKeys.Control | ModifierKeys.Shift, DeEnumerate);

            AddKey("ToStructure", Key.W, ModifierKeys.Control, ToStructure);
            AddKey("ToStructure", Key.W, ModifierKeys.Control | ModifierKeys.Shift, FromStructure);

            AddKey("Group", Key.G, ModifierKeys.Control, Group);
            AddKey("Ungroup", Key.G, ModifierKeys.Control | ModifierKeys.Shift, Ungroup);
        }
        private void InitalizeViewHotKeys()
        {
            AddKey("ZoomIn", Key.OemPlus, ModifierKeys.Control, ZoomIn);
            AddKey("ZoomOut", Key.OemMinus, ModifierKeys.Control, ZoomOut);

            AddKey("ZoomInX5", Key.OemPlus, ModifierKeys.Control | ModifierKeys.Shift, ZoomInX5);
            AddKey("ZoomOutX5", Key.OemMinus, ModifierKeys.Control | ModifierKeys.Shift, ZoomOutX5);

            AddKey("DefaultZoom", Key.D0, ModifierKeys.Control, SetDefaultZoom);


            AddKey("ZoomInX5_NumPad", Key.Add, ModifierKeys.Control | ModifierKeys.Shift, ZoomInX5);
            AddKey("ZoomOutX5_NumPad", Key.Subtract, ModifierKeys.Control | ModifierKeys.Shift, ZoomOutX5);

            AddKey("ZoomIn_NumPad", Key.Add, ModifierKeys.Control, ZoomIn);
            AddKey("ZoomOut_NumPad", Key.Subtract, ModifierKeys.Control, ZoomOut);

            AddKey("DefaultZoom_NumPad", Key.NumPad0, ModifierKeys.Control, SetDefaultZoom);


            AddKey("CloseSideBar", Key.B, ModifierKeys.Control, CloseSideBar);


            AddKey("MoveUp", Key.Up, ModifierKeys.Alt, TextEditor.LineUp);
            AddKey("MoveDown", Key.Down, ModifierKeys.Alt, TextEditor.LineDown);
            AddKey("MoveToStart", Key.Up, ModifierKeys.Control | ModifierKeys.Alt, MoveToStart);
            AddKey("MoveToEnd", Key.Down, ModifierKeys.Control | ModifierKeys.Alt, MoveToEnd);
        }
        private void InitalizeTabsKeys()
        {
            AddKey("NextTab", Key.Right, ModifierKeys.Alt, NextTab);
            AddKey("PreviousTab", Key.Left, ModifierKeys.Alt, PreviousTab);
            AddKey("OpenLastTab", Key.D0, ModifierKeys.Alt, OpenLastTab);
        }
        private void InitializeNumbersKeys()
        {
            for (int i = 0; i < 9; i++)
            {
                int Index = i;
                Key Key = (Key)(Index + 35);

                AddKey($"OpenTab[{Index}]", Key, ModifierKeys.Alt, () => OpenTab(Index));
            }
        }

        private void AddKey(string Name, Key Key, ModifierKeys Modifiers, RoutedEventHandler Handler)
            => LocalHotKeys.AddKey(Name, Key, Modifiers, Handler);
        private void AddKey(string Name, Key Key, ModifierKeys Modifiers, Action Handler)
            => LocalHotKeys.AddKey(Name, Key, Modifiers, Handler);


        private void RichTextBoxKeyDown(object sender, KeyEventArgs E)
        {
            bool Handle(RoutedEventHandler Action) => HandleAction(Action, E);

            E.Handled = (E.Key, Keyboard.Modifiers) switch
            {
                (Key.Up, ModifierKeys.Control | ModifierKeys.Alt) => HandleAction(SetCursorInStart),
                (Key.Down, ModifierKeys.Control | ModifierKeys.Alt) => HandleAction(SetCursorInEnd),

                (Key.Tab, ModifierKeys.None) => HandleAction(WriteTab),
                (Key.Tab, ModifierKeys.Shift) => Handle(LevelDown),
                (Key.Enter, _) => HandleAction(() => WriteNewLine(Keyboard.Modifiers)),

                (Key.Up, ModifierKeys.Control) => Handle(UpDigit),
                (Key.Down, ModifierKeys.Control) => Handle(DownDigit),
                (Key.D0, ModifierKeys.Control) => Handle(NormalizeDigit),
                (Key.NumPad0, ModifierKeys.Control) => Handle(NormalizeDigit),

                (Key.L, ModifierKeys.Control | ModifierKeys.Shift) => Handle(ToLower),
                (Key.U, ModifierKeys.Control | ModifierKeys.Shift) => Handle(ToUpper),
                (Key.C, ModifierKeys.Control | ModifierKeys.Shift) => Handle(Capitalize),

                (Key.T, ModifierKeys.Control) => Handle(ConvertChars),
                (Key.I, ModifierKeys.Control) => Handle(GetInformation),

                (Key.V, ModifierKeys.Control) => Handle(Paste),

                (Key.B, ModifierKeys.Control) => Handle(CloseSideBar),
                (Key.R, ModifierKeys.Control) => Handle(Reload),

                (Key.E, ModifierKeys.Control) => Handle(InsertEmail),

                (Key.J, ModifierKeys.Control) => Handle(JoinLines),

                (Key.T, ModifierKeys.Control | ModifierKeys.Shift) => Handle(Trim),
                (Key.OemSemicolon, ModifierKeys.Control) => HandleAction(() => Enumerate(this, E)),
                (Key.OemSemicolon, ModifierKeys.Control | ModifierKeys.Shift) => Handle(DeEnumerate),

                (Key.U, ModifierKeys.Control) => true,
                _ => false
            };
        }

        private bool HandleAction(Action Action)
        {
            Action();
            return true;
        }
        private bool HandleAction(RoutedEventHandler Action, KeyEventArgs E)
        {
            Action(this, E);
            return true;
        }
    }
}