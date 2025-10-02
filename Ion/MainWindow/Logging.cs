//using System.Collections.ObjectModel;
//using System.Windows;
//using System.Windows.Controls;
//using Zion;

//namespace Ion
//{
//    public partial class MainWindow : Window
//    {
//        public static ObservableCollection<Exception> _ErrorLogs
//        {
//            get; private set;
//        } = new ObservableCollection<Exception>();

//        public ObservableCollection<Exception> ErrorLogs
//        {
//            get => _ErrorLogs;
//            set
//            {
//                _ErrorLogs = value;
//                OnPropertyChanged();
//            }
//        }

//        public static void Log(object? Message)
//        {
            
//        }

//        public static void LogError(Exception Exception)
//        {
//            _ErrorLogs.Add(Exception);
//        }
//        public static void LogError(object? Message)
//        {
//            _ErrorLogs.Add(new Exception(Message.ToNotNullString()));
//        }
//    }
//}