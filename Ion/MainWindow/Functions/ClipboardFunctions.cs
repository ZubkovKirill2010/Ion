using System.Windows;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private void Paste(object Sender, RoutedEventArgs E)
        {
            try
            {
                InsertText(Clipboard.GetText());
            }
            catch (Exception Exception)
            {
                Log("Ошибка при вставке текста");
                LogError(Exception);
            }
        }
    }
}