namespace Ion
{
    public partial class MainWindow
    {
        private void Initialize()
        {
            DataContext = this;

            InitializeComponent();
            InitializeWindowParameters();
            InitializeHotKeys();
            ApplySettings();
            MaximizeWindow();

            Task.Run(() => Translater.Initialize(_Settings._Language)).GetAwaiter().GetResult();
            Translater.Initialize(this);

            StatusBar.Initialize(this);
        }
    }
}