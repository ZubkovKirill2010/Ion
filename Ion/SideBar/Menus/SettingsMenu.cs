namespace Ion.SideBar
{
    public sealed class SettingsMenu : SideBarMenu
    {
        public readonly MainWindow _Window;
        public readonly Settings _Settings;

        public override string _Header => Translater._Current._Settings;
        public override string _CancelButtonText => Translater._Current._Cancel;
        public override string _ApplyButtonText => Translater._Current._Apply;

        public SettingsMenu(MainWindow Window) : base(Window.SettingsMenu)
        {
            _Window = Window;
            _Settings = Window._Settings;

            Window.SettingsMenu.DataContext = _Settings;
        }


        public override void Apply()
        {
            _Settings.Save();
            _Window.ApplySettings();
            CloseMenu();
        }
    }
}