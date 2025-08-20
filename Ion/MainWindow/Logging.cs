using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private static TextBlock _MessageBox;

        public static ObservableCollection<Exception> _ErrorLogs
        {
            get; private set;
        } = new ObservableCollection<Exception>();

        public ObservableCollection<Exception> ErrorLogs
        {
            get => _ErrorLogs;
            set
            {
                _ErrorLogs = value;
                OnPropertyChanged();
            }
        }

        public static void Log(object? Message)
        {
            try
            {
                string Text = Message?.ToString() ?? "null";

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _MessageBox.Text = Text;
                });
            }
            catch (Exception Exception)
            {
                LogError(Exception);
            }
        }

        public static void LogError(Exception Exception)
        {
            _ErrorLogs.Add(Exception);
        }
        public static void LogError(string Message)
        {
            _ErrorLogs.Add(new Exception(Message));
        }
    }
}