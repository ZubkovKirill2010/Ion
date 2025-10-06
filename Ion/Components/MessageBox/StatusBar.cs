using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Zion;

namespace Ion
{
    public static class StatusBar
    {
        private static TextBlock _MessageBox;
        private static ExecutableQueue<string> _Messages;

        public static readonly ErrorList _ErrorList = new ErrorList();

        static StatusBar()
        {
            _Messages = new ExecutableQueue<string>(SetText)
            {
                Delay = 3000,
                DelayToCompletion = 10_000,
                Completion = () => Task.Run(() => SetText(string.Empty))
            };
        }

        public static void Initialize(MainWindow Window)
        {
            Debug.WriteLine("Инитиализация строки состояния");
            _MessageBox = Window.MessageBox;
            Window.ErrorListView.ItemsSource = _ErrorList.ErrorLogs;
        }

        public static void Write(string? String)
        {
            _Messages.Add(String ?? "null");
        }
        public static void Write(object? Object)
        {
            _Messages.Add(Object.ToNotNullString());
        }

        public static void WriteError(Exception Exception)
        {
            _ErrorList.Add(Exception);
            Write($"{Translater._Current._UnknowException} : '{Exception.GetType()}'");
        }
        public static void WriteError(object? Message)
        {
            _ErrorList.Add(new Exception(Message.ToNotNullString()));
            Write($"{Translater._Current._UnknowException}");
        }


        private static void SetText(string Text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _MessageBox.Text = Text;
            });
        }
    }
}