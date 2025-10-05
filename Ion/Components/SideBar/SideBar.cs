using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Ion
{
    public enum SideBarType
    {
        Settings, Searching, Replacing, GoTo, Logging, Sending,
        None
    }

    public sealed class SideBar
    {
        private bool _MenuIsOpen;

        private readonly TextBlock _Header;
        private readonly Button _CancelButton;
        private readonly Button _ApplyButton;

        private readonly UIElement _SideBar;
        private readonly SideBarMenu[] _Menus;

        private SideBarType _CurrentMenuType = SideBarType.None;
        private SideBarMenu? _CurrentMenu;

        public SideBar(MainWindow Window)
        {
            Debug.WriteLine("New SideBar");

            SideBarMenu.Initialize(this);

            _Header = Window.SideBarHeader;
            _CancelButton = Window.CancelButton;
            _ApplyButton = Window.ApplyButton;

            _CancelButton.Click += (S, E) => Cancel();
            _ApplyButton.Click += (S, E) => Apply();

            _SideBar = Window.SideBar;
            _Menus =
            [
                new SettingsMenu(Window),
                new SearchingMenu(Window),
                new ReplacingMenu(Window),
                new SettingsMenu(Window),//Go to
                new LoggingMenu(Window),
                new SendingMenu(Window)
            ];
        }


        public void OpenMenu(SideBarType Menu)
        {
            if (_CurrentMenuType == Menu)
            {
                Menu = SideBarType.None;
            }

            _CurrentMenuType = Menu;
            if (_CurrentMenu is not null)
            {
                _CurrentMenu.SetVisibility(Visibility.Collapsed);
            }

            if (Menu == SideBarType.None)
            {
                Close();

                _CurrentMenu = null;
            }
            else
            {
                Open();

                _CurrentMenu = _Menus[(int)Menu];
                _CurrentMenu.SetVisibility(Visibility.Visible);

                _Header.Text = _CurrentMenu._Header;
                _CancelButton.Content = _CurrentMenu._CancelButtonText;
                _ApplyButton.Content = _CurrentMenu._ApplyButtonText;

                _CurrentMenu.Start();
            }
        }

        public void Cancel()
        {
            _CurrentMenu?.Cancel();
            OpenMenu(SideBarType.None);
        }

        public void Apply()
        {
            _CurrentMenu?.Apply();
        }


        public void CloseMenu()
        {
            OpenMenu(SideBarType.None);
        }


        private void Open()
        {
            if (_MenuIsOpen)
            {
                return;
            }

            _MenuIsOpen = true;

            _SideBar.Visibility = Visibility.Visible;
            DoubleAnimation OpenAnimation = new DoubleAnimation
            {
                From = 300,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            _SideBar.RenderTransform.BeginAnimation(TranslateTransform.XProperty, OpenAnimation);
        }

        private void Close()
        {
            if (!_MenuIsOpen)
            {
                return;
            }


            _MenuIsOpen = false;

            DoubleAnimation closeAnimation = new DoubleAnimation
            {
                To = 300,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            closeAnimation.Completed += (s, _) =>
            {
                _SideBar.Visibility = Visibility.Collapsed;
            };

            _SideBar.RenderTransform.BeginAnimation(TranslateTransform.XProperty, closeAnimation);
        }
    }
}