using System.Diagnostics;
using System.Windows;

namespace Ion
{
    public sealed class Hub
    {
        public MainWindow _Window     { get; }
        public Settings   _Settings   { get; }
        public Menus      _Menus      { get; private set; }
        public TextEditor _Editor     { get; private set; }
        public SideBar    _SideBar    { get; private set; }
        public TabManager _TabManager { get; private set; }

        public Hub(MainWindow Window)
        {
            Debug.WriteLine("New Hub");
            _Settings = Settings.Load();
            _Window = Window;
        }


        public void Initialize()
        {
            Debug.WriteLine("Initialization of components");

            _Editor = new TextEditor(_Window);
            _SideBar = new SideBar(_Window);
            _Menus = new Menus();
            _TabManager = new TabManager(this, _Editor);

            Menu.Initialize(this);
            _Menus.Initialize();
            Menu.BindFunctions();
        }


        public void Exit(object Sender, RoutedEventArgs E)
        {
            Exit();
        }
        public void Exit()
        {
            var Tabs = _TabManager;

            if (Tabs.Count != 0)
            {
                int LastCount = 0;

                while (Tabs.Count >= 0)
                {
                    LastCount = Tabs.Count;
                    Tabs.CloseTab(Tabs.Count - 1);

                    if (Tabs.Count == LastCount)
                    {
                        return;
                    }
                }
            }

            Environment.Exit(0);
        }
    }
}