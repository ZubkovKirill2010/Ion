namespace BCMEditor.SideBarMenu
{
    public sealed class SettingsMenu : SideBarMenu
    {
        public readonly MainWindow _Window;
        public readonly Settings _Settings;

        public override string _Header => "Настройки";
        public override string _CancelButtonText => "Отмена";
        public override string _ApplyButtonText => "Применить";

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