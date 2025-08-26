using Ion.SideBar;
using System.Windows;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private const int _MinZoom = 10, _MaxZoom = 45;

        private void ZoomIn(object Sender, RoutedEventArgs E) => AddZoom(1);
        private void ZoomOut(object Sender, RoutedEventArgs E) => AddZoom(-1);

        private void ZoomInX5(object Sender, RoutedEventArgs E) => AddZoom(5);
        private void ZoomOutX5(object Sender, RoutedEventArgs E) => AddZoom(-5);

        private void SetDefaultZoom(object Sender, RoutedEventArgs E)
        {
            TextEditor.FontSize = _Settings._FontSize;
        }

        private void AddZoom(int Zoom)
        {
            int CurrentZoom = (int)TextEditor.FontSize;
            Zoom = Math.Clamp(CurrentZoom + Zoom, _MinZoom, _MaxZoom);

            if (CurrentZoom != Zoom)
            {
                TextEditor.FontSize = Zoom;

                ZoomInButton.IsEnabled = Zoom != _MinZoom;
                ZoomOutButton.IsEnabled = Zoom != _MaxZoom;
            }
        }

        private void OpenSettingsMenu(object Sender, RoutedEventArgs E)
            => _SideBar.OpenMenu(SideBarType.Settings);

        private void OpenLoggingMenu(object Sender, RoutedEventArgs E)
            => _SideBar.OpenMenu(SideBarType.Logging);


        private void MoveToStart()
        {
            TextEditor.CaretPosition = TextEditor.Document.ContentStart;
            TextEditor.ScrollToHome();
        }
        private void MoveToEnd()
        {
            TextEditor.CaretPosition = TextEditor.Document.ContentEnd;
            TextEditor.ScrollToEnd();
        }
    }
}