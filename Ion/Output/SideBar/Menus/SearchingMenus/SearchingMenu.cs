using Ion.Extensions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Ion.SideBar
{
    public sealed class SearchingMenu : SearchReplaceBaseMenu
    {
        protected override SolidColorBrush _HighlightingBrush { get; init; }
            = new SolidColorBrush(Color.FromArgb(100, 16, 232, 30));

        public override string _Header => Translater._Current._Search;
        public override string _CancelButtonText => Translater._Current._Exit;
        public override string _ApplyButtonText => Translater._Current._Search;

        public SearchingMenu(MainWindow Window)
            : base(Window, Window.SearchingMenu) { }


        public override void Start()
        {
            _Window.SearchingMenu_Target.Focus();
        }

        public override void Apply()
        {
            string Target = _Window.SearchingMenu_Target.Text;

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

            TextRange FullTextRange = _Editor.GetAll();
            string Text = FullTextRange.Text;

            RegexOptions Options = GetOptions
            (
                Target, out string Pattern,
                _Window.SearchingMenu_CaseSensitive,
                _Window.SearchingMenu_WholeWord,
                _Window.SearchingMenu_UsingRegularExpressions
            );

            if (_Window.SearchingMenu_UsingUnicodeChars.IsChecked.GetValueOrDefault())
            {
                Pattern = Regex.Unescape(Pattern);
            }

            try
            {
                Regex Regex = new Regex(Pattern, Options);
                List<Match> Matches = Regex.Matches(Text).Cast<Match>().ToList();
                _MatchesCount = Matches.Count;

                if (_MatchesCount == 0)
                {
                    StatusBar.Write(Translater._Current._MatchesNotFound);
                    return;
                }

                foreach (Match Match in Matches)
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
                                int LengthInRun = Math.Min(Match.Length, RunLength - LocalIndex);

                                TextPointer Start = Navigator.GetPositionAtOffset(LocalIndex);
                                TextPointer End = Start.GetPositionAtOffset(LengthInRun);

                                if (Start is not null && End is not null)
                                {
                                    TextRange range = new TextRange(Start, End);
                                    Highlight(range);
                                    _HighlightedRanges.Add(range);

                                    if (LengthInRun < Match.Length)
                                    {
                                        int RemainingLength = Match.Length - LengthInRun;
                                        TextPointer NextRunStart = End.GetNextContextPosition(LogicalDirection.Forward);

                                        if (NextRunStart is not null)
                                        {
                                            TextPointer NextEnd = NextRunStart.GetPositionAtOffset(RemainingLength);
                                            if (NextEnd is not null)
                                            {
                                                TextRange nextRange = new TextRange(NextRunStart, NextEnd);
                                                Highlight(nextRange);
                                                _HighlightedRanges.Add(nextRange);
                                            }
                                        }
                                    }
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

                StatusBar.Write($"{Translater._Current._MatchesFound}: {_MatchesCount}");
            }
            catch (ArgumentException Exception)
            {
                StatusBar.Write(Translater._Current._RegexError);
                StatusBar.WriteError(Exception);
            }
            catch (Exception Exception)
            {
                StatusBar.Write(Translater._Current._ErrorInSearch);
                StatusBar.WriteError(Exception);
            }
        }
    }
}