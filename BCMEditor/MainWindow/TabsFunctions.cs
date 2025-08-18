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


        public void NextTab(object Sender, RoutedEventArgs E)
        {
            if (Tabs.Count == 1)
            {
                return;
            }

            int TargetTab = TabList.SelectedIndex + 1;
            if (TargetTab >= Tabs.Count)
            {
                TargetTab = 0;
            }

            SelectTab(TargetTab);
        }
        public void PreviousTab(object Sender, RoutedEventArgs E)
        {
            if (Tabs.Count == 1)
            {
                return;
            }

            if (Tabs.Count == 1)
            {
                return;
            }

            int TargetTab = TabList.SelectedIndex - 1;
            if (TargetTab < 0)
            {
                TargetTab = Tabs.Count - 1;
            }

            SelectTab(TargetTab);
        }


        public void OpenLastTabClick(object Sender, RoutedEventArgs E)
        {
            OpenLastTab();
        }

        public void CloseSavedTabs(object Sender, RoutedEventArgs E)
        {
            int Index = 0;

            while (Index < Tabs.Count)
            {
                if (Tabs[Index]._IsSaved)
                {
                    Tabs.RemoveAt(Index);

                }
                else
                {
                    Index++;
                }
            }
        }


        public void AddTab(object Sender, RoutedEventArgs E)
        {
            TextEditor.Focus();

            AddTab(new TxtTab());
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
                _Editor.SetDocument(new FlowDocument());
            }
        }


        public void OpenTab(int Index)
        {
            SelectTab(int.Clamp(Index, 0, Tabs.Count - 1));
        }
        public void OpenLastTab()
        {
            SelectTab(Tabs.Count - 1);
        }

        public void AddTab(Tab Tab)
        {
            Tabs.Add(Tab);
            SelectTab(Tab);
        }


        private void SelectTab(int Index)
        {
            SelectTab(Tabs[Index]);
        }
        private void SelectTab(Tab Tab)
        {
            _Editor._CurrentTab = Tab;

            _Editor.SetDocument(Tab._Document);
            TabList.SelectedItem = Tab;

            ReloadButton.IsEnabled = Tab.IsSaved();

            TextEditor.Focus();
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
                $"Сохранить изменения в \"{Tab._Header}\" перед закрытием?",
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