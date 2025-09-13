using Ion.SideBar;
using Ion.Tabs;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private void New(object Sender, RoutedEventArgs E)
        {
            if (Sender is MenuItem Item)
            {
                Tab NewTab = Tab.GetEditor(Item.Header.ToString() ?? string.Empty);
                AddTab(NewTab);
                Log($"Новая вкладки ({NewTab.GetType().Name})");
            }
            else
            {
                Log("Неверный вызов функции New");
            }
        }
        private void Open(object Sender, RoutedEventArgs E)
        {
            OpenFileDialog Dialog = new OpenFileDialog
            {
                Title = "Открыть файл",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Все файлы (*.*)|*.*|Bcm файлы (*.bcm)|*.bcm|Текстовые файлы (*.txt)|*.txt|Файлы C# (*.cs)|*.cs",
                DefaultExt = ""
            };

            if (Dialog.ShowDialog() == true)
            {
                string FilePath = Dialog.FileName;

                if (!File.Exists(FilePath))
                {
                    Log($"Файл \"{FilePath}\" не существует");
                    return;
                }

                try
                {
                    string Extension = Path.GetExtension(FilePath);

                    try
                    {
                        Tab NewTab = Tab.GetEditor(Extension);

                        NewTab._CurrentFile = FilePath;
                        NewTab.ReadFile();
                        NewTab._IsSaved = true;

                        AddTab(NewTab);

                        TextEditor.CaretPosition = TextEditor.Document.ContentEnd;
                    }
                    catch (Exception Exception)
                    {
                        Log($"Ошибка при создании вкладки: {Exception}");
                    }
                }
                catch (Exception Exception)
                {
                    Log($"Ошибка при чтении файла \"{FilePath}\": {Exception}");
                }
            }
        }

        private void Save(object Sender, RoutedEventArgs E)
        {
            _Editor.Save();
            ReloadButton.IsEnabled = true;
        }
        private void SaveAs(object Sender, RoutedEventArgs E)
        {
            _Editor.SaveAs();
        }

        private void SaveAll(object Sender, RoutedEventArgs E)
        {
            foreach (Tab Tab in _Tabs)
            {
                Tab.SaveFile();
            }
        }

        private void Reload(object Sender, RoutedEventArgs E)
        {
            Tab SelectedTab = _SelectedTab;

            if (SelectedTab._CurrentFile is not null || !File.Exists(SelectedTab._CurrentFile))
            {
                Log("Нельзя перезагрузить не сохранённый файл");
                return;
            }

            SelectedTab.ReadFile();
            _Editor.SetDocument(SelectedTab._Document);
        }
        private void Print(object Sender, RoutedEventArgs E)
        {
            PrintDialog Dialog = new PrintDialog();

            if (Dialog.ShowDialog() == true)
            {
                try
                {
                    IDocumentPaginatorSource Document = TextEditor.Document;
                    Dialog.PrintDocument(Document.DocumentPaginator, "Печать документа");
                }
                catch
                {
                    Log($"Ошибка печати");
                }
            }
        }
        private void Send(object Sender, RoutedEventArgs E)
        {
            _SideBar.OpenMenu(SideBarType.Sending);
        }

        private void Exit(object Sender, RoutedEventArgs E)
        {
            Exit();
        }
        private void Exit()
        {
            ObservableCollection<Tab> Tabs = _Tabs;

            if (Tabs.Count != 0)
            {
                int LastCount = 0;

                while (Tabs.Count >= 0)
                {
                    LastCount = _Tabs.Count;
                    CloseTab(Tabs[Tabs.Count - 1]);

                    if (Tabs.Count == LastCount)
                    {
                        return;
                    }
                }
            }

            Environment.Exit(0);
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

            AddTab(NewTab);
        }
    }
}