using Ion.Extensions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Zion;

namespace Ion.SideBar
{
    public abstract class SearchReplaceBaseMenu : SideBarMenu
    {
        protected abstract SolidColorBrush _HighlightingBrush { get; init; }
        protected int _MatchesCount = 0;

        protected RichTextBox _Editor;
        protected MainWindow _Window;
        protected List<TextRange> _HighlightedRanges = new List<TextRange>();

        public SearchReplaceBaseMenu(MainWindow Window, UIElement Menu)
            : base(Menu)
        {
            _Window = Window;
            _Editor = Window.TextEditor;
        }

        public override void Cancel()
        {
            ClearHighlights();
            _MatchesCount = 0;
        }

        protected static RegexOptions GetOptions(string Target, out string Pattern, CheckBox CaseSensitive, CheckBox WholeWord, CheckBox UsingRegularExpressions)
        {
            RegexOptions Options = RegexOptions.None;

            if (!CaseSensitive.IsChecked.GetValueOrDefault())
            {
                Options |= RegexOptions.IgnoreCase;
            }

            Pattern = Target;

            if (WholeWord.IsChecked.GetValueOrDefault())
            {
                Pattern = $"\\b{Regex.Escape(Target)}\\b";
            }
            else if (!UsingRegularExpressions.IsChecked.GetValueOrDefault())
            {
                Pattern = StringParser.Parse(Pattern);
            }

            return Options;
        }

        protected void ClearHighlights()
        {
            if (_HighlightedRanges.Count == 0)
            {
                return;
            }

            _Editor.GetAll().ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
        }

        protected void Highlight(TextRange Range)
        {
            Range.ApplyPropertyValue(TextElement.BackgroundProperty, _HighlightingBrush);
        }
    }
}