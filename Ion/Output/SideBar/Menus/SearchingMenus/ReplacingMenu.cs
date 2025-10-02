using Ion.Extensions;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;

namespace Ion.SideBar
{
    public sealed class ReplacingMenu : SearchReplaceBaseMenu
    {
        protected override SolidColorBrush _HighlightingBrush { get; init; }
            = new SolidColorBrush(Color.FromArgb(100, 236, 35, 239));

        public override string _Header => Translater._Current._Replacement;
        public override string _CancelButtonText => Translater._Current._Exit;
        public override string _ApplyButtonText => Translater._Current._Replace;

        public ReplacingMenu(MainWindow Window)
            : base(Window, Window.ReplacingMenu) { }

        public override void Start()
        {
            _Window.ReplacingMenu_From.Focus();
        }

        public override void Apply()
        {
            ClearHighlights();
            _MatchesCount = 0;

            string Target = _Window.ReplacingMenu_From.Text;
            string ReplaceText = _Window.ReplacingMenu_To.Text;

            if (Target == ReplaceText)
            {
                StatusBar.Write(Translater._Current._OldTextEqualNew);
                return;
            }
            if (string.IsNullOrEmpty(Target))
            {
                StatusBar.Write(Translater._Current._EnterSearchText);
                return;
            }
            if (_Editor.Document.IsEmpty())
            {
                StatusBar.Write(Translater._Current._EmptyText);
                return;
            }

            TextRange FullRange = _Editor.GetAll();
            string Text = FullRange.Text;

            RegexOptions Options = GetOptions
            (
                Target, out string Pattern,
                _Window.ReplacingMenu_CaseSensitive,
                _Window.ReplacingMenu_WholeWord,
                _Window.ReplacingMenu_UsingRegulatExpressions
            );

            if (_Window.ReplacingMenu_UsingUnicodeChars.IsChecked.GetValueOrDefault())
            {
                Pattern = Regex.Unescape(Pattern);
                ReplaceText = Regex.Unescape(ReplaceText);
            }

            try
            {
                Regex Regex = new Regex(Pattern, Options);

                List<Match> Matches = Regex.Matches(Text).Cast<Match>().ToList();
                _MatchesCount = Matches.Count;

                if (Matches.Count == 0)
                {
                    StatusBar.Write(Translater._Current._MatchesNotFound);
                    return;
                }

                for (int i = Matches.Count - 1; i >= 0; i--)
                {
                    Match Batch = Matches[i];
                    ReplaceAndHighlightMatch(Batch, ReplaceText);
                }

                StatusBar.Write($"{Translater._Current._Replaced}: {_MatchesCount / 2}");
            }
            catch (ArgumentException Exception)
            {
                StatusBar.Write(Translater._Current._RegexError);
                StatusBar.WriteError(Exception);
            }
            catch (Exception Exception)
            {
                StatusBar.Write(Translater._Current._ReplaceError);
                StatusBar.WriteError(Exception);
            }
        }

        private void ReplaceAndHighlightMatch(Match Match, string ReplaceText)
        {
            TextPointer Navigator = _Editor.Document.ContentStart;
            int CurrentOffset = 0;

            while (Navigator is not null && Navigator.CompareTo(_Editor.Document.ContentEnd) < 0)
            {
                TextPointerContext Context = Navigator.GetPointerContext(LogicalDirection.Forward);

                if (Context == TextPointerContext.Text)
                {
                    string TextRun = Navigator.GetTextInRun(LogicalDirection.Forward);
                    int RunLength = TextRun.Length;

                    if (CurrentOffset <= Match.Index && Match.Index < CurrentOffset + RunLength)
                    {
                        int LocalIndex = Match.Index - CurrentOffset;
                        TextPointer Start = Navigator.GetPositionAtOffset(LocalIndex);
                        TextPointer End = Start.GetPositionAtOffset(Match.Length);

                        if (Start is not null && End is not null)
                        {
                            new TextRange(Start, End).Text = ReplaceText;
                            _MatchesCount++;

                            TextRange NewRange = new TextRange(Start, Start.GetPositionAtOffset(ReplaceText.Length));
                            Highlight(NewRange);
                            _HighlightedRanges.Add(NewRange);
                        }
                        break;
                    }

                    CurrentOffset += RunLength;
                    Navigator = Navigator.GetPositionAtOffset(RunLength);
                }
                else
                {
                    int ElementLength = 0;

                    if (Context == TextPointerContext.ElementEnd &&
                        Navigator.GetAdjacentElement(LogicalDirection.Forward) is Paragraph)
                    {
                        ElementLength = 2;
                    }
                    CurrentOffset += ElementLength;
                    Navigator = Navigator.GetNextContextPosition(LogicalDirection.Forward);
                }
            }
        }
    }
}