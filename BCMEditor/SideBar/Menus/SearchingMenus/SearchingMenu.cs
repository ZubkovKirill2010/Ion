using BCMEditor.Extensions;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BCMEditor.SideBar
{
    public sealed class SearchingMenu : SearchReplaceBaseMenu
    {
        protected override SolidColorBrush _HighlightingBrush { get; init; }
            = new SolidColorBrush(Color.FromArgb(100, 16, 232, 30));

        public override string _Header => "Поиск";
        public override string _CancelButtonText => "Выйти";
        public override string _ApplyButtonText => "Искать";

        public SearchingMenu(MainWindow Window)
            : base(Window, Window.SearchingMenu) { }


        //public override void Start()
        //{
        //    _Window.SearchingMenu_OnlySelection.IsChecked = !_Editor.Selection.IsEmpty;
        //}

        public override void Apply()
        {
            ClearHighlights();

            string Target = _Window.SearchingMenu_Target.Text;
            if (string.IsNullOrEmpty(Target))
            {
                MainWindow.Log("Введите текст для поиска");
                return;
            }

            if (_Editor.Document.IsEmpty())
            {
                MainWindow.Log("Пустой текст");
                return;
            }

            TextRange FullTextRange = _Editor.GetAllText();
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
                Target = Regex.Unescape(Target);
            }

            try
            {
                Regex Regex = new Regex(Target, Options);
                List<Match> Matches = Regex.Matches(Text).Cast<Match>().ToList();
                _MatchesCount = Matches.Count;

                if (_MatchesCount == 0)
                {
                    MainWindow.Log("Совпадений не найдено");
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

                MainWindow.Log($"Найдено совпадений: {_MatchesCount}");
            }
            catch (ArgumentException Exception)
            {
                MainWindow.Log("Ошибка в регулярном выражении");
                MainWindow.LogError(Exception);
            }
            catch (Exception Exception)
            {
                MainWindow.Log("При поиске произошла ошибка");
                MainWindow.LogError(Exception);
            }
        }
    }
}