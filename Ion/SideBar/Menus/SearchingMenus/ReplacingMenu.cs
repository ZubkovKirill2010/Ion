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

        public override string _Header => "Замена";
        public override string _CancelButtonText => "Выйти";
        public override string _ApplyButtonText => "Заменить";

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
                MainWindow.Log("Старый текст совпадает с новым");
                return;
            }
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
                    MainWindow.Log("Совпадений не найдено");
                    return;
                }

                for (int i = Matches.Count - 1; i >= 0; i--)
                {
                    Match Batch = Matches[i];
                    ReplaceAndHighlightMatch(Batch, ReplaceText);
                }

                MainWindow.Log($"Заменено: {_MatchesCount / 2}");
            }
            catch (ArgumentException Exception)
            {
                MainWindow.Log("Ошибка в регулярном выражении");
                MainWindow.LogError(Exception);
            }
            catch (Exception Exception)
            {
                MainWindow.Log("При замене произошла ошибка");
                MainWindow.LogError(Exception);
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