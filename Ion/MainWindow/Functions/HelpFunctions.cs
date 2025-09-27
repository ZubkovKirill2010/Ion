using Ion.Tabs;
using System.Windows;
using Zion;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private const string _Version = "0.1.0";

        private void AboutEditor(object Sender, RoutedEventArgs E)
        {
            string Text =
@$"~~Ion (Beta)~~
Version {_Version}
GitHub  https://github.com/ZubkovKirill2010";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Information",
            };
            AddTab(Tab);
        }

        private void AboutCharsConverter(object Sender, RoutedEventArgs E)
        {
            var Structure = new Structure<string>("Conversions")
            {
                new Structure<string>("\"n^1234\" → \"n¹²³⁴\""),
                new Structure<string>("Key-value", _KeyWords.ConvertAll(Pair => $"'{Pair.Item1}' - '{Pair.Item2}'"))
            };

            string Text =
@$"~~Char conversions~~
{{ Edit\Convert Chars (Ctrl+T) }}

{Structure}";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Char conversions",
            };
            AddTab(Tab);
        }
    }
}