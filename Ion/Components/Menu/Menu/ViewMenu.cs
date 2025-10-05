using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ion
{
    public sealed class ViewMenu : Menu
    {
        private Settings _Settings => _Window._Settings;
        private MenuItem _ZoomInButton => _Window.ZoomInButton;
        private MenuItem _ZoomOutButton => _Window.ZoomOutButton;

        public override void Initialize()
        {
            AddHotKey("ZoomIn", ZoomIn, Key.OemPlus, ModifierKeys.Control);
            AddHotKey("ZoomOut", ZoomOut, Key.OemMinus, ModifierKeys.Control);

            AddKey(ZoomInX5, Key.OemPlus, ModifierKeys.Control | ModifierKeys.Shift);
            AddKey(ZoomOutX5, Key.OemMinus, ModifierKeys.Control | ModifierKeys.Shift);

            AddKey(SetDefaultZoom, Key.D0, ModifierKeys.Control);


            AddHotKey("ZoomInX5_NamPad", ZoomInX5, Key.Add, ModifierKeys.Control | ModifierKeys.Shift);
            AddHotKey("ZoomOutX5_NamPad", ZoomOutX5, Key.Subtract, ModifierKeys.Control | ModifierKeys.Shift);

            AddHotKey("ZoomIn_NamPad", ZoomIn, Key.Add, ModifierKeys.Control);
            AddHotKey("ZoomOut_NamPad", ZoomOut, Key.Subtract, ModifierKeys.Control);

            AddHotKey("DefaultZoom_NumPad", SetDefaultZoom, Key.NumPad0, ModifierKeys.Control);


            AddKey(CloseSideBar, Key.B, ModifierKeys.Control);


            AddHotKey("MoveUp", TextEditor.LineUp, Key.Up, ModifierKeys.Alt);
            AddHotKey("MoveDown", TextEditor.LineDown, Key.Down, ModifierKeys.Alt);
            AddHotKey("MoveToStart", MoveToStart, Key.Up, ModifierKeys.Control | ModifierKeys.Alt);
            AddHotKey("MoveToEnd", MoveToEnd, Key.Down, ModifierKeys.Control | ModifierKeys.Alt);

            AddKey(OpenLoggingMenu);
            AddKey(OpenSettingsMenu);
        }


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

                _ZoomInButton.IsEnabled = Zoom != _MinZoom;
                _ZoomOutButton.IsEnabled = Zoom != _MaxZoom;
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



        private void CloseSideBar(object Sender, RoutedEventArgs E)
            => _SideBar.OpenMenu(SideBarType.None);

        private void CancelButtonClick(object Sender, RoutedEventArgs E)
            => _SideBar.Cancel();

        private void ApplyButtonClick(object Sender, RoutedEventArgs E)
            => _SideBar.Apply();
    }
}