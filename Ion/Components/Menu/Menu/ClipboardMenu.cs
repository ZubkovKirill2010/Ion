using System.Windows;
using System.Windows.Input;

namespace Ion
{
    public sealed class ClipboardMenu : Menu
    {
        public override void Initialize()
        {
            AddKey(Paste, Key.V, ModifierKeys.Control, true);
        }

        private void Paste(object Sender, RoutedEventArgs E)
        {
            try
            {
                InsertText(Clipboard.GetText());
            }
            catch (Exception Exception)
            {
                StatusBar.Write(Translater._Current._InsertError);
                StatusBar.WriteError(Exception);
            }
        }
    }
}