using HotKeyManagement;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private Thickness _MaximizedThinkes = new Thickness(0);
        private Thickness _NormalThinkes;

        private CornerRadius _MaximizedCornerRadius = new CornerRadius(0);
        private CornerRadius _NormalCornerRadius;

        private Rect _NormalWindowRect;
        private Rect _WorkArea => SystemParameters.WorkArea;

        private double _MaximizedFontSize = 13d;
        private double _NormalFontSize = 19d;

        private bool _IsMaximized;


        private void InitializeWindowParameters()
        {
            Debug.WriteLine("Инитиализация параметров окна");
            _NormalThinkes = WindowChrome.ResizeBorderThickness;
            _NormalCornerRadius = WindowChrome.CornerRadius;

            HotKeyInterceptor.Initialize(this);

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
                _Hub.Exit
            );

            StateChanged += OnStateChanged;
            Closing += OnClosing;
        }


        private void DragWindow(object Sender, MouseButtonEventArgs E)
        {
            HitTestResult Hit = VisualTreeHelper.HitTest(TabList, E.GetPosition(TabList));

            if ((Hit is null || Hit.VisualHit.GetType() == typeof(TabItem)) && E.ChangedButton == MouseButton.Left)
            {
                if (E.ClickCount == 2)
                {
                    MaximizeWindow(this, E);
                }
                else if (E.ButtonState == MouseButtonState.Pressed)
                {
                    if (_IsMaximized)
                    {
                        Point Cursor = E.GetPosition(this);
                        double MaximizedWidth = ActualWidth;

                        NormalizeWindow();

                        double RelativeX = Cursor.X / MaximizedWidth;

                        if (RelativeX < 0.5)
                        {
                            double OffsetFromLeft = Cursor.X * (ActualWidth / MaximizedWidth);
                            Left = Cursor.X - Math.Max(280, 2 * OffsetFromLeft * (ActualWidth / MaximizedWidth));
                        }
                        else
                        {
                            double OffsetFromRight = MaximizedWidth - Cursor.X;
                            Left = Cursor.X - ActualWidth + Math.Max(140, OffsetFromRight * (ActualWidth / MaximizedWidth));
                        }

                        Top = Cursor.Y - 28;

                        UpdateLayout();
                    }

                    DragMove();
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

        public void CenteringWindow(object Sender, RoutedEventArgs E)
        {
            if (_IsMaximized)
            {
                NormalizeWindow();
            }

            Left = _WorkArea.Left + (_WorkArea.Width - ActualWidth) / 2;
            Top = _WorkArea.Top + (_WorkArea.Height - ActualHeight) / 2;
        }

        public void MarginMaximizeWindow(object Sender, RoutedEventArgs E)
        {
            if (_IsMaximized)
            {
                NormalizeWindow();
            }

            Left = _WorkArea.Left + 10;
            Top = _WorkArea.Top + 10;
            Width = _WorkArea.Width - 20;
            Height = _WorkArea.Height - 20;
        }


        public void MaximizeWindow()
        {
            Debug.WriteLine("Максимизация окна");
            if (_IsMaximized)
            {
                return;
            }
            _IsMaximized = true;

            SaveWindowRect();

            WindowChrome.ResizeBorderThickness = _MaximizedThinkes;
            WindowChrome.CornerRadius = _MaximizedCornerRadius;
            MaximizeButton.FontSize = _MaximizedFontSize;

            Left = _WorkArea.Left;
            Top = _WorkArea.Top;
            Width = _WorkArea.Width;
            Height = _WorkArea.Height;

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
        {
            _NormalWindowRect = new Rect(Left, Top, Width, Height);
        }


        public void OnStateChanged(object Sender, EventArgs E)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MaximizeWindow();
            }
        }

        public void OnClosing(object Sender, EventArgs E)
        {
            _Hub.Exit();
        }
    }
}