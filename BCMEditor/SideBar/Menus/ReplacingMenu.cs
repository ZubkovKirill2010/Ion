namespace BCMEditor.SideBarMenu
{
    public sealed class ReplacingMenu : SideBarMenu
    {
        public override string _Header => "Замена";
        public override string _CancelButtonText => "Выйти";
        public override string _ApplyButtonText => "Заменить";

        public ReplacingMenu(MainWindow Window) : base(Window.ReplacingMenu)
        {

        }


        public override void Apply()
        {

        }
    }
}