using System.Windows;
using Zion;

namespace Ion
{
    public sealed class HelpMenu : Menu
    {
        private const string _Version = "0.1.0";


        public override void Initialize()
        {
            AddKey(AboutEditor);
            AddKey(AboutCharsConverter);
        }


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
            _Hub._TabManager.AddTab(Tab);
        }

        private void AboutCharsConverter(object Sender, RoutedEventArgs E)
        {
            Structure<string> Structure = new Structure<string>("Conversions")
            {
                new Structure<string>("\"n^1234\" \u2192 \"n\u00B9\u00B2\u00B3\u0074\""),
                new Structure<string>("Key-value", EditMenu._KeyWords.ConvertAll(Pair => $"'{Pair.Item1}' - '{Pair.Item2}'"))
            };

            string Text =
@$"~~Char conversions~~
{{ Edit\Convert Chars (Ctrl+T) }}

{Structure}";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Char conversions",
            };
            _Hub._TabManager.AddTab(Tab);
        }
    }
}