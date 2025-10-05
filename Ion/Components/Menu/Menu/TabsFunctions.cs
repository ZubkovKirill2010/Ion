using System.Windows;
using System.Windows.Input;

namespace Ion
{
    public sealed class TabsMenu : Menu
    {
        public override void Initialize()
        {
            AddKey(_Hub._SideBar.CloseMenu, Key.B, ModifierKeys.Control, true);

            AddKey(_Hub._TabManager.CloseCurrentTab, Key.C, ModifierKeys.Alt);
            AddKey(_Hub._TabManager.CloseSavedTabs, Key.C, ModifierKeys.Control | ModifierKeys.Alt);

            AddKey(_Hub._TabManager.NextTab, Key.Right, ModifierKeys.Alt);
            AddKey(_Hub._TabManager.PreviousTab, Key.Left, ModifierKeys.Alt);
            AddKey(OpenLastTab, Key.D0, ModifierKeys.Alt);

            for (int i = 0; i < 9; i++)
            {
                int Index = i;
                Key Key = (Key)(Index + 35);

                AddHotKey
                (
                    $"OpenTab[{Index}]",
                    () => _Hub._TabManager.OpenTab(Index),
                    Key,
                    ModifierKeys.Alt
                );
            }
        }

        private void OpenLastTab(object Sender, RoutedEventArgs E)
        {
            _Hub._TabManager.OpenLastTab();
        }
    }
}