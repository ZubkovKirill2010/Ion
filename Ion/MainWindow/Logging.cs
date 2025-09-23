using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Zion;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private const int _MessageDisplayTime = 700;
        private const int _TimeBeforeClearing = 5000;
        private const bool _ClearAfterLogging = true;

        private static readonly Queue<string> _Messages = new Queue<string>();
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
            _Messages.Enqueue(Message.ToNotNullString());

            if (_Messages.Count != 1)
            {
                return;
            }

            Task.Run
            (
                async () =>
                {
                    while (_Messages.Count != 0)
                    {
                        try
                        {
                            SetMessageText(_Messages.Dequeue());
                        }
                        catch (Exception Exception)
                        {
                            LogError(Exception);
                        }

                        await Task.Delay(_MessageDisplayTime);
                    }
                }
            );

            if (_ClearAfterLogging)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(_TimeBeforeClearing);
                    if (_Messages.Count == 0)
                    {
                        SetMessageText(string.Empty);
                    }
                });
            }
        }

        public static void LogError(Exception Exception)
        {
            _ErrorLogs.Add(Exception);
        }
        public static void LogError(object? Message)
        {
            _ErrorLogs.Add(new Exception(Message.ToNotNullString()));
        }


        private static void SetMessageText(string Text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _MessageBox.Text = Text;
            });
        }
    }
}