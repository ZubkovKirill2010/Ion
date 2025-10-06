using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Ion
{
    public sealed class TabManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Hub _Hub;
        private readonly TextEditor _Editor;
        private readonly TabControl _TabList;

        private ObservableCollection<Tab> _Tabs = new ObservableCollection<Tab>();
        public ObservableCollection<Tab> Tabs
        {
            get => _Tabs;
            private set
            {
                _Tabs = value;
                OnPropertyChanged(nameof(Tabs));
            }
        }

        private Tab _SelectedTab;
        public Tab SelectedTab
        {
            get => _SelectedTab;
            set
            {
                _SelectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public int Count => _Tabs.Count;


        public TabManager(Hub Hub, TextEditor Editor)
        {
            Debug.WriteLine("New TabManager");

            MainWindow Window = Hub._Window;

            _Hub = Hub;
            _Tabs = new ObservableCollection<Tab>();
            _Editor = Editor;
            _TabList = Window.TabList;

            Window.TabList.ItemsSource = _Tabs;
            Window.TabList.SelectionChanged += SelectTab;

            Window.MoveTabLeft.Click += MoveTabsLeft;
            Window.MoveTabRight.Click += MoveTabsRight;
        }


        public Tab this[int Index] => _Tabs[Index];


        public void NextTab(object Sender, RoutedEventArgs E)
        {
            if (Tabs.Count == 1)
            {
                return;
            }

            int TargetTab = _TabList.SelectedIndex + 1;
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

            int TargetTab = _TabList.SelectedIndex - 1;
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
            _Editor.Focus();
            AddTab(new TxtTab());
        }

        public void CloseTab(object Sender, RoutedEventArgs E)
        {
            if (Sender is Button Button && Button.Tag is Tab ClosingTab)
            {
                CloseTab(ClosingTab);
            }
        }
        public void CloseCurrentTab(object Sender, RoutedEventArgs E)
        {
            CloseTab(SelectedTab);
        }

        public void SelectTab(object Sender, SelectionChangedEventArgs E)
        {
            if (_TabList.SelectedItem is Tab SelectedTab)
            {
                SelectTab(SelectedTab);
                StatusBar.Write(SelectedTab._CurrentFile ?? Translater._Current._NotSavedFile);
            }
            else
            {
                _Editor.SetDocument(new FlowDocument());
            }
        }


        public void OpenTab(int Index)
        {
            SelectTab(Math.Clamp(Index, 0, Tabs.Count - 1));
        }
        public void OpenLastTab()
        {
            SelectTab(Tabs.Count - 1);
        }

        public void AddTab(Tab Tab)
        {
            Debug.WriteLine("Adding new tab");
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
            _TabList.SelectedItem = Tab;

            _Hub._Window.ReloadButton.IsEnabled = Tab.IsSaved();
            //# Reload button
            _Editor.Focus();
        }


        public void CloseTab(int Index)
        {
            CloseTab(_Tabs[Index]);
        }
        public void CloseTab(Tab Tab)
        {
            if (Tab.IsEmpty() || Tab._IsSaved)
            {
                Tabs.Remove(Tab);
                if (Tabs.Count == 0)
                {
                    _Hub.Exit();
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
                _Hub.Exit();
            }
        }


        private void MoveTabsLeft(object Sender, RoutedEventArgs E)
        {
            StatusBar.Write("Move tabs left");
        }
        private void MoveTabsRight(object Sender, RoutedEventArgs E)
        {
            StatusBar.Write("Move tabs right");
        }


        private void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}