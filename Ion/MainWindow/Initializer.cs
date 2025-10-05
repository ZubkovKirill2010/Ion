using HotKeyManagement;

namespace Ion
{
    public partial class MainWindow
    {
        private void Initialize()
        {
            DataContext = this;

            InitializeComponent();
            InitializeWindowParameters();
            MaximizeWindow();
            ApplySettings();

            Task.Run(() => Translater.Initialize(_Settings._Language)).GetAwaiter().GetResult();
            Translater.Initialize(this);
            StatusBar.Initialize(this);

            LocalHotKeys.Initialize(this);
            Tab.Initialize(this);

            _Hub.Initialize();
        }
    }
}