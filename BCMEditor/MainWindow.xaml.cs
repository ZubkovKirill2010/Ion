using Ion.Tabs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Zion;

namespace Ion
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static readonly string _NewLine = Environment.NewLine;

        public event PropertyChangedEventHandler? PropertyChanged;

        public readonly Settings _Settings = Settings.Load();

        public SideBar.SideBar _SideBar { get; private set; }
        public TextEditor _Editor { get; private set; }

        public MainWindow()
        {
            Initialize();

            _Editor = new TextEditor(this);
            _SideBar = new SideBar.SideBar(this);
            _MessageBox = MessageBox;

            Tab.Initialize(this);

            string[] Arguments = App._Arguments;

            if (Arguments.IsNullOrEmpty())
            {
                AddTab(Tab.GetEditor(_Settings._DefaultTabExtension));
            }
            else
            {
                foreach (string File in Arguments.Where(String => String is not null))
                {
                    Open(File);
                }
            }
            Arguments = null;

            Dispatcher.BeginInvoke
            (
                () => TabList.SelectedIndex = 0,
                DispatcherPriority.Loaded
            );

            TextEditor.Focus();
        }


        public void ApplySettings()
        {
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


        protected void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}