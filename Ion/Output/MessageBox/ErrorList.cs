using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ion
{
    public sealed class ErrorList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Exception> _ErrorLogs
        {
            get; private set;
        } = new ObservableCollection<Exception>();

        public ObservableCollection<Exception> ErrorLogs
        {
            get => _ErrorLogs;
            set
            {
                _ErrorLogs = value;
                OnPropertyChanged(nameof(ErrorLogs));
            }
        }

        public int Count => _ErrorLogs.Count;

        public Exception this[int Index] => _ErrorLogs[Index];


        public void Add(Exception Item)
        {
            ErrorLogs.Add(Item);
        }

        protected void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}