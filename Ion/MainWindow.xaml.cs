using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Zion;

namespace Ion
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public  static readonly string _AssetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
        private static readonly string _NewLine = Environment.NewLine;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Hub _Hub { get; }
        public Settings _Settings => _Hub._Settings;

        public SideBar _SideBar => _Hub._SideBar;
        public TextEditor _Editor => _Hub._Editor;

        public MainWindow()
        {
            _Hub = new Hub(this);
            Initialize();

            string[]? Arguments = App._Arguments;

            if (Arguments.IsNullOrEmpty())
            {
                _Hub._TabManager.AddTab(Tab.GetEditor(_Settings._DefaultTabExtension));
            }
            else
            {
                foreach (string File in Arguments.NotNullable())
                {
                    _Hub._Menus._FileMenu.Open(File);
                }
            }

            Dispatcher.BeginInvoke
            (
                () => TabList.SelectedIndex = 0,
                DispatcherPriority.Loaded
            );

            TextEditor.Focus();
        }


        public void ApplySettings()
        {
            Debug.WriteLine("Applying settings");
            Menu.FontSize = _Settings._MenuScale;
            TextEditor.FontSize = _Settings._FontSize;
        }


        public static void Invoke(Action Action)
        {
            Application.Current.Dispatcher.Invoke(Action);
        }
        public static async Task InvokeAsync(Action Action)
        {
            await Application.Current.Dispatcher.InvokeAsync(Action);
        }


        private void AddTab(object Sender, RoutedEventArgs E)
        {
            _Hub._TabManager.AddTab(Sender, E);
        }
        private void CloseTab(object Sender, RoutedEventArgs E)
        {
            _Hub._TabManager.CloseTab(Sender, E);
        }
        private void Exit(object Sender, RoutedEventArgs E)
        {
            _Hub.Exit(Sender, E);
        }

        protected void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}