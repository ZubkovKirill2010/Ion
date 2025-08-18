using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BCMEditor.SideBarMenu
{
    public sealed class SearchingMenu : SideBarMenu
    {
        private SolidColorBrush _HighlightingBrush = new SolidColorBrush(Color.FromArgb(100, 16, 232, 30));

        private int _MatchesCount = 0;

        private RichTextBox _Editor;
        private TextRange _PreviousHighlight;
        private MainWindow _Window;

        public override string _Header => "Поиск";
        public override string _CancelButtonText => "Выйти";
        public override string _ApplyButtonText => "Искать";

        public SearchingMenu(MainWindow Window) : base(Window.SearchingMenu)
        {
            _Editor = Window.TextEditor;
            _Window = Window;
        }

        public override void Apply()
        {
            ClearPreviousHighlight();

            string Target = _Window.SearchingMenu_Target.Text;
            if (string.IsNullOrEmpty(Target))
            {
                MainWindow.Log("Введите текст для поиска");
                return;
            }

            TextRange FullTextRange = new TextRange(_Editor.Document.ContentStart, _Editor.Document.ContentEnd);
            string FullText = FullTextRange.Text;

            RegexOptions Options = RegexOptions.None;

            if (!_Window.SearchingMenu_CaseSensitive.IsChecked.GetValueOrDefault())
            {
                Options |= RegexOptions.IgnoreCase;
            }

            if (_Window.SearchingMenu_WholeWord.IsChecked.GetValueOrDefault())
            {
                Target = $"\\b{Regex.Escape(Target)}\\b";
            }
            else if (!_Window.SearchingMenu_UsingRegulatExpressions.IsChecked.GetValueOrDefault())
            {
                Target = Regex.Escape(Target);
            }

            if (_Window.SearchingMenu_UsingUnicodeChars.IsChecked.GetValueOrDefault())
            {
                Target = Regex.Unescape(Target);
            }

            try
            {
                Regex Regex = new Regex(Target, Options);
                MatchCollection Matches = Regex.Matches(FullText);
                _MatchesCount = Matches.Count;

                // Если нет совпадений, выходим
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
                                    Highlight(new TextRange(Start, End));

                                    if (LengthInRun < Match.Length)
                                    {
                                        int RemainingLength = Match.Length - LengthInRun;
                                        TextPointer NextRunStart = End.GetNextContextPosition(LogicalDirection.Forward);

                                        if (NextRunStart is not null)
                                        {
                                            TextPointer NextEnd = NextRunStart.GetPositionAtOffset(RemainingLength);
                                            if (NextEnd is not null)
                                            {
                                                Highlight(new TextRange(NextRunStart, NextEnd));
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
                MainWindow.LogError(Exception.Message);
            }
        }

        public override void Cancel()
        {
            if (_MatchesCount > 0)
            {
                TextRange FullRange = new TextRange(_Editor.Document.ContentStart, _Editor.Document.ContentEnd);

                FullRange.ApplyPropertyValue(TextElement.BackgroundProperty, null);
            }
            _MatchesCount = 0;
        }

        private void ClearPreviousHighlight()
        {
            if (_PreviousHighlight is not null)
            {
                _PreviousHighlight.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
            }

            TextRange FullRange = new TextRange(_Editor.Document.ContentStart, _Editor.Document.ContentEnd);
            FullRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
        }

        private void Highlight(TextRange Range)
        {
            Range.ApplyPropertyValue(TextElement.BackgroundProperty, _HighlightingBrush);
        }
    }
}