using System.Windows;

namespace BCMEditor.SideBar
{
    public abstract class SideBarMenu
    {
        private static SideBar _SideBar;

        public readonly UIElement _Menu;

        public abstract string _Header { get; }
        public abstract string _CancelButtonText { get; }
        public abstract string _ApplyButtonText { get; }


        public SideBarMenu(UIElement Menu)
            => _Menu = Menu;

        public static void Initialize(SideBar SideBar)
        {
            if (_SideBar is null)
            {
                _SideBar = SideBar;
            }
        }

        public virtual void Start() { }

        public virtual void Cancel() { }
        public virtual void Apply() { }

        protected void CloseMenu()
        {
            _SideBar.CloseMenu();
        }

        public void SetVisibility(Visibility Visibility)
            => _Menu.Visibility = Visibility;
    }
}