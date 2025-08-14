namespace BCMEditor.SideBarMenu
{
    public sealed class SearchingMenu : SideBarMenu
    {
        public override string _Header => "Поиск";
        public override string _CancelButtonText => "Выйти";
        public override string _ApplyButtonText => "Искать";

        public SearchingMenu(MainWindow Window) : base(Window.SearchingMenu)
        {

        }


        public override void Apply()
        {

        }
    }
}