using Ion.Tabs;
using System.Windows;
using Zion;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private void AboutEditor(object Sender, RoutedEventArgs E)
        {
            string Text =
@"Информация о редакторе";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Общая информация",
            };
            AddTab(Tab);
        }

        private void AboutCharsConverter(object Sender, RoutedEventArgs E)
        {
            var Structure = new Structure<string>("Преобразования")
            {
                new Structure<string>("\"n^1234\" → \"n¹²³⁴\""),
                new Structure<string>("Ключ-значение", _KeyWords.ConvertAll(Pair => $"'{Pair.Item1}' - '{Pair.Item2}'"))
            };

            string Text =
@$"~~Преобразование символов~~
{{ Edit\Convert Chars (Ctrl+T) }}

{Structure}";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Преобразование символов",
            };
            AddTab(Tab);
        }
    }
}