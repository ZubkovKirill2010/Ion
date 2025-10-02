using System.Windows;
using System.Windows.Threading;

namespace Ion
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
            StatusBar.WriteError(Exception ?? new Exception("Null exception"));
            StatusBar.Write($"{Translater._Current._UnknowException} '{Exception?.GetType().FullName}'");
        }
    }
}
