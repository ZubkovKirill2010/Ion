using System.Windows;
using System.Windows.Threading;

namespace BCMEditor
{
    public partial class App : Application
    {
        public static string[]? _Arguments;
        private MainWindow _Window;

        public App()
        {
            _Window = (MainWindow)MainWindow;
        }


        protected override void OnStartup(StartupEventArgs E)
        {
            base.OnStartup(E);

            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _Arguments = E.Args;
        }


        private void App_DispatcherUnhandledException(object Sender, DispatcherUnhandledExceptionEventArgs E)
        {
            HandleException(E.Exception);
            E.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object Sender, UnhandledExceptionEventArgs E)
        {
            HandleException(E.ExceptionObject as Exception);
        }

        private void HandleException(Exception? Exception)
        {
            BCMEditor.MainWindow.LogError(Exception ?? new Exception("Null exception"));
            BCMEditor.MainWindow.Log($"Произошла неизвестная ошибка типа {Exception?.GetType().FullName}");
        }
    }
}
