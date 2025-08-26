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
            InitalizeClipboardHotKeys();
            InitalizeViewHotKeys();
            InitalizeTabsKeys();
        }

        private void InitalizeFileHotKeys()
        {
            AddKey("New", Key.N, ModifierKeys.Control, AddTab);
            AddKey("Open", Key.O, ModifierKeys.Control, Open);

            AddKey("Save", Key.S, ModifierKeys.Control, Save);
            AddKey("SaveAs", Key.S, ModifierKeys.Control | ModifierKeys.Shift, SaveAs);

            AddKey("Reload", Key.R, ModifierKeys.Control, Reload);
            AddKey("Print", Key.P, ModifierKeys.Control, Print);
            AddKey("Send", Key.M, ModifierKeys.Control, Send);

            AddKey("SaveAll", Key.S, ModifierKeys.Alt, SaveAll);
            AddKey("CloseAll", Key.C, ModifierKeys.Alt, CloseAll);
        }
        private void InitalizeEditHotKeys()
        {
            AddKey("ToLower", Key.L, ModifierKeys.Control | ModifierKeys.Shift, ToLower);
            AddKey("ToUpper", Key.U, ModifierKeys.Control | ModifierKeys.Shift, ToUpper);
            AddKey("Capitalize", Key.C, ModifierKeys.Control | ModifierKeys.Shift, Capitalize);

            AddKey("Find", Key.F, ModifierKeys.Control, Find);
            AddKey("Replace", Key.H, ModifierKeys.Control, Replace);
            //AddKey("GoTo", Key.G, ModifierKeys.Control, GoTo);

            AddKey("Do", Key.D, ModifierKeys.Control, Do);
            AddKey("DuplicateLine", Key.D, ModifierKeys.Control | ModifierKeys.Shift, DuplicateLine);
        }
        private void InitalizeInsertsHotKeys()
        {
            AddKey("Time/Date", Key.F5, ModifierKeys.None, InsertTimeDate);
            AddKey("Email", Key.E, ModifierKeys.Control, InsertEmail);
            AddKey("Separator", Key.I, ModifierKeys.Control, InsertSeparator);
        }
        private void InitalizePunctuationHotKeys()
        {
            AddKey("Trim", Key.T, ModifierKeys.Control | ModifierKeys.Shift, Trim);
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
        private void InitalizeClipboardHotKeys()
        {
            AddKey("Paste", Key.V, ModifierKeys.Control, Paste);
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

            for (int i = 0; i < 9; i++)
            {
                int TabIndex = i;
                AddKey($"OpenTab[{TabIndex}]", (Key)(TabIndex + 35), ModifierKeys.Alt, () => OpenTab(TabIndex));
            }
        }


        private void AddKey(string Name, Key Key, ModifierKeys Modifiers, RoutedEventHandler Handler)
            => LocalHotKeys.AddKey(Name, Key, Modifiers, Handler);
        private void AddKey(string Name, Key Key, ModifierKeys Modifiers, Action Handler)
            => LocalHotKeys.AddKey(Name, Key, Modifiers, Handler);


        private void RichTextBoxKeyDown(object sender, KeyEventArgs E)
        {
            E.Handled = (E.Key, Keyboard.Modifiers) switch
            {
                (Key.Tab, ModifierKeys.None) => HandleAction(WriteTab),
                (Key.Enter, _) => HandleAction(() => WriteNewLine(Keyboard.Modifiers)),
                (Key.I, ModifierKeys.Control) => HandleAction(InsertSeparator, E),
                (Key.Tab, ModifierKeys.Shift) => HandleAction(LevelDown, E),
                (Key.V, ModifierKeys.Control) => HandleAction(Paste, E),
                (Key.B, ModifierKeys.Control) => HandleAction(CloseSideBar, E),
                (Key.R, ModifierKeys.Control) => HandleAction(Reload, E),
                (Key.E, ModifierKeys.Control) => HandleAction(InsertEmail, E),
                (Key.L, ModifierKeys.Control | ModifierKeys.Shift) => HandleAction(ToLower, E),
                (Key.C, ModifierKeys.Control | ModifierKeys.Shift) => HandleAction(Capitalize, E),
                (Key.T, ModifierKeys.Control | ModifierKeys.Shift) => HandleAction(Trim, E),
                (Key.OemSemicolon, ModifierKeys.Control) => HandleAction(() => Enumerate(this, E)),
                (Key.OemSemicolon, ModifierKeys.Control | ModifierKeys.Shift) => HandleAction(DeEnumerate, E),
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