using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

namespace BCMEditor
{
    public partial class MainWindow : Window
    {
        private void Delete(object Sender, RoutedEventArgs E)
        {
            Dispatcher.BeginInvoke
            (
                () =>
                {
                    if (TextEditor.Selection.IsEmpty)
                    {
                        TextRange Range = new TextRange(TextEditor.CaretPosition, TextEditor.CaretPosition);
                        Range.Text = string.Empty;
                        TextEditor.CaretPosition = Range.End;
                    }
                    TextEditor.Focus();
                },
                DispatcherPriority.Input
            );
        }

        private void Paste(object Sender, RoutedEventArgs E)
        {
            InsertText(Clipboard.GetText());
        }
    }
}