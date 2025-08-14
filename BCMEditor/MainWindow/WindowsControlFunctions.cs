using HotKeyManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BCMEditor
{
    public partial class MainWindow : Window
    {
        private Thickness _MaximizedThinkes = new Thickness(0);
        private Thickness _NormalThinkes;

        private CornerRadius _MaximizedCornerRadius = new CornerRadius(0);
        private CornerRadius _NormalCornerRadius;

        private double _MaximizedFontSize = 13d;
        private double _NormalFontSize = 19d;

        private Rect _NormalWindowRect;
        private bool _IsMaximized;


        private void InitializeWindowParameters()
        {
            HotKeyInterceptor.Initialize(this);

            _NormalThinkes = WindowChrome.ResizeBorderThickness;
            _NormalCornerRadius = WindowChrome.CornerRadius;

            HotKeyInterceptor.OverwriteGlobalHotKey
            (
                GlobalHotKey.Win_Up,
                MaximizeWindow
            );
            HotKeyInterceptor.OverwriteGlobalHotKey
            (
                GlobalHotKey.Win_Down,
                NormalizeWindow
            );
            HotKeyInterceptor.OverwriteGlobalHotKey
            (
                GlobalHotKey.Alt_F4,
                Exit
            );
        }


        private void DragWindow(object Sender, MouseButtonEventArgs E)
        {
            HitTestResult Hit = VisualTreeHelper.HitTest(TabList, E.GetPosition(TabList));

            if (Hit is null || Hit.VisualHit.GetType() == typeof(TabItem))
            {
                if (E.ChangedButton == MouseButton.Left)
                {
                    if (E.ClickCount == 2)
                    {
                        MaximizeWindow(this, E);
                    }
                    else if (!_IsMaximized)
                    {
                        DragMove();
                    }
                }
            }
        }


        public void MinimizeWindow(object Sender, RoutedEventArgs E)
        {
            WindowState = WindowState.Minimized;
        }

        public void MaximizeWindow(object Sender, RoutedEventArgs E)
        {
            if (_IsMaximized)
            {
                NormalizeWindow();
            }
            else
            {
                MaximizeWindow();
            }
        }


        public void MaximizeWindow()
        {
            if (_IsMaximized)
            {
                return;
            }

            SaveWindowRect();

            WindowChrome.ResizeBorderThickness = _MaximizedThinkes;
            WindowChrome.CornerRadius = _MaximizedCornerRadius;
            MaximizeButton.FontSize = _MaximizedFontSize;

            Rect WorkArea = SystemParameters.WorkArea;

            _IsMaximized = true;

            Left = WorkArea.Left;
            Top = WorkArea.Top;
            Width = WorkArea.Width;
            Height = WorkArea.Height;

            MaximizeButton.Content = "❐";
        }
        public void NormalizeWindow()
        {
            if (!_IsMaximized)
            {
                return;
            }

            WindowChrome.ResizeBorderThickness = _NormalThinkes;
            WindowChrome.CornerRadius = _NormalCornerRadius;
            MaximizeButton.FontSize = _NormalFontSize;

            _IsMaximized = false;

            Left = _NormalWindowRect.Left;
            Top = _NormalWindowRect.Top;
            Width = _NormalWindowRect.Width;
            Height = _NormalWindowRect.Height;

            MaximizeButton.Content = "□";
        }


        private void SaveWindowRect()
            => _NormalWindowRect = new Rect(Left, Top, Width, Height);
    }
}