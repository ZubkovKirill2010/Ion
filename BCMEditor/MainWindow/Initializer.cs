namespace BCMEditor
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
        }
    }
}