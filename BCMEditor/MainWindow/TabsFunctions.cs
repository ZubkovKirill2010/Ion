using BCMEditor.Tabs;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BCMEditor
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Tab> _Tabs = new ObservableCollection<Tab>();
        private Tab _SelectedTab;

        public ObservableCollection<Tab> Tabs
        {
            get => _Tabs;
            private set
            {
                _Tabs = value;
                OnPropertyChanged(nameof(Tabs));
            }
        }
        public Tab SelectedTab
        {
            get => _SelectedTab;
            set
            {
                _SelectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public void AddTab(Tab Tab)
        {
            Tabs.Add(Tab);
            SelectTab(_Tabs.Count - 1);
        }


        public void AddTab(object Sender, RoutedEventArgs E)
        {
            TextEditor.Focus();
            Tabs.Add(new TxtTab());
            SelectTab(_Tabs.Count - 1);
            TextEditor.Document = Tabs[0]._Document;
        }

        public void CloseTab(object Sender, RoutedEventArgs E)
        {
            if (Sender is Button Button && Button.Tag is Tab ClosingTab)
            {
                CloseTab(ClosingTab);
            }
        }

        public void SelectTab(object Sender, SelectionChangedEventArgs E)
        {
            if (TabList.SelectedItem is Tab SelectedTab)
            {
                SelectTab(SelectedTab);

                Log(SelectedTab._CurrentFile ?? "Не сохранённый файл");
            }
            else
            {
                TextEditor.Document = new FlowDocument();
            }
        }


        private void SelectTab(Tab Tab)
        {
            _Editor._CurrentTab = Tab;

            TextEditor.Document = Tab._Document;
            TabList.SelectedItem = Tab;

            ReloadButton.IsEnabled = Tab.IsSaved();

            TextEditor.Focus();
        }

        private void SelectTab(int Index)
        {
            SelectTab(Tabs[Index]);
        }


        private void CloseTab(Tab Tab)
        {
            if (Tab.IsEmpty() || Tab._IsSaved)
            {
                Tabs.Remove(Tab);
                if (Tabs.Count == 0)
                {
                    Exit();
                }
                return;
            }

            //#Temp
            MessageBoxResult Result = System.Windows.MessageBox.Show
            (
                $"Сохранить изменения в \"{Tab.Header}\" перед закрытием?",
                "Подтверждение",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question
            );


            if (Result == MessageBoxResult.Yes)
            {
                _Editor.Save();
                Tabs.Remove(Tab);
            }
            else if (Result == MessageBoxResult.No)
            {
                Tabs.Remove(Tab);
            }

            if (Tabs.Count == 0)
            {
                Exit();
            }
        }
    }
}