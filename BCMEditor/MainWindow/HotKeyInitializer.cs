using HotKeyManagement;
using System.Windows;
using System.Windows.Input;

namespace BCMEditor
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
            AddKey("Find", Key.F, ModifierKeys.Control, Find);
            AddKey("Replace", Key.H, ModifierKeys.Control, Replace);
            //AddKey("GoTo", Key.G, ModifierKeys.Control, GoTo);

            AddKey("DuplicateLine", Key.D, ModifierKeys.Control, DuplicateLine);
        }
        private void InitalizeInsertsHotKeys()
        {
            AddKey("Time/Date", Key.F5, ModifierKeys.None, InsertTimeDate);
            AddKey("Email", Key.E, ModifierKeys.Control, InsertEmail);
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
        }


        private void AddKey(string Name, Key Key, ModifierKeys Modifiers, RoutedEventHandler Handler)
            => LocalHotKeys.AddKey(Name, Key, Modifiers, Handler);


        private void RichTextBoxKeyDown(object Sender, KeyEventArgs E)
        {
            if (E.Key == Key.Tab && Keyboard.Modifiers == ModifierKeys.None)
            {
                InsertText("\t");
                E.Handled = true;
            }

            if (E.Key == Key.B && Keyboard.Modifiers == ModifierKeys.Control)
            {
                CloseSideBar(Sender, E);
                E.Handled = true;
            }

            if (E.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Reload(Sender, E);
                E.Handled = true;
            }
            if (E.Key == Key.E && Keyboard.Modifiers == ModifierKeys.Control)
            {
                InsertEmail(Sender, E);
                E.Handled = true;
            }

            if (E.Key == Key.T && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                Trim(Sender, E);
                E.Handled = true;
            }

            if (E.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                E.Handled = true;
            }
        }
    }
}