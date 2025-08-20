using BCMEditor.Tabs;
using System.Windows;

namespace BCMEditor
{
    public partial class MainWindow : Window
    {
        private void AboutEditor(object Sender, RoutedEventArgs E)
        {
            string Text =
@"Информация о редакторе";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Общая информация"
            };
            AddTab(Tab);
        }
    }
}