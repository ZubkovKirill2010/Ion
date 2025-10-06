using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Ion
{
    public sealed class FileMenu : Menu
    {
        public override void Initialize()
        {
            AddKey(New, Key.N, ModifierKeys.Control);
            AddKey(Open, Key.O, ModifierKeys.Control);
            AddKey(Save, Key.S, ModifierKeys.Control);
            AddKey(SaveAs, Key.S, ModifierKeys.Control | ModifierKeys.Shift);
            AddKey(SaveAll, Key.S, ModifierKeys.Alt);
            AddKey(Print, Key.P, ModifierKeys.Control);
            AddKey(Send, Key.T, ModifierKeys.Control);

            AddKey(Reload, Key.R, ModifierKeys.Control, true);

            AddKey(_Hub.Exit);
        }


        private void New(object Sender, RoutedEventArgs E)
        {
            if (Sender is MenuItem Item)
            {
                Tab NewTab = Tab.GetEditor(Item.Header.ToString() ?? string.Empty);
                _Hub._TabManager.AddTab(NewTab);
            }
        }
        private void Open(object Sender, RoutedEventArgs E)
        {
            OpenFileDialog Dialog = new OpenFileDialog
            {
                Title = Translater._Current._OpenFile,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "All files (*.*)|*.*|Bcm פאיכû (*.bcm)|*.bcm|Text files (*.txt)|*.txt|Files C# (*.cs)|*.cs",
                DefaultExt = ""
            };

            if (Dialog.ShowDialog() == true)
            {
                string FilePath = Dialog.FileName;

                if (!File.Exists(FilePath))
                {
                    StatusBar.Write($"{Translater._Current._File} \"{FilePath}\" {Translater._Current._NotExists}");
                    return;
                }

                try
                {
                    string Extension = Path.GetExtension(FilePath);

                    Tab NewTab = Tab.GetEditor(Extension);

                    NewTab._CurrentFile = FilePath;
                    NewTab.ReadFile();
                    NewTab._IsSaved = true;

                    _Hub._TabManager.AddTab(NewTab);

                    _Editor.CaretPosition = _Editor.Document.ContentEnd;
                }
                catch (Exception Exception)
                {
                    StatusBar.Write($"{Translater._Current._LoadingError} \"{FilePath}\": {Exception}");
                }
            }
        }

        private void Save(object Sender, RoutedEventArgs E)
        {
            _TextEditor.Save();
            _Window.ReloadButton.IsEnabled = true;
        }
        private void SaveAs(object Sender, RoutedEventArgs E)
        {
            _TextEditor.SaveAs();
        }

        private void SaveAll(object Sender, RoutedEventArgs E)
        {
            foreach (Tab Tab in _Hub._TabManager.Tabs)
            {
                Tab.SaveFile();
            }
        }

        private void Reload(object Sender, RoutedEventArgs E)
        {
            Tab SelectedTab = _Hub._TabManager.SelectedTab;

            if (SelectedTab._CurrentFile is not null || !File.Exists(SelectedTab._CurrentFile))
            {
                StatusBar.Write(Translater._Current._ReloadError);
                return;
            }

            SelectedTab.ReadFile();
            _TextEditor.SetDocument(SelectedTab._Document);
        }
        private void Print(object Sender, RoutedEventArgs E)
        {
            PrintDialog Dialog = new PrintDialog();

            if (Dialog.ShowDialog() == true)
            {
                try
                {
                    IDocumentPaginatorSource Document = _Editor.Document;
                    Dialog.PrintDocument(Document.DocumentPaginator, Translater._Current._PrintingDocument);
                }
                catch
                {
                    StatusBar.Write(Translater._Current._PrintingError);
                }
            }
        }
        private void Send(object Sender, RoutedEventArgs E)
        {
            OpenMenu(SideBarType.Sending);
        }


        public void Open(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                return;
            }

            Tab NewTab = Tab.GetEditor(Path.GetExtension(FilePath));
            NewTab._CurrentFile = FilePath;
            NewTab._IsSaved = true;
            NewTab.ReadFile();

            _Hub._TabManager.AddTab(NewTab);
        }
    }
}