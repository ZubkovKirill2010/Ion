using Ion.SideBar;
using System.Windows;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private void CloseSideBar(object Sender, RoutedEventArgs E)
            => _SideBar.OpenMenu(SideBarType.None);

        private void CancelButtonClick(object Sender, RoutedEventArgs E)
            => _SideBar.Cancel();

        private void ApplyButtonClick(object Sender, RoutedEventArgs E)
            => _SideBar.Apply();
    }
}