using Ion.Extensions;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.Gaming.Input.ForceFeedback;
using Zion;

namespace Ion
{
    public sealed class EditMenu : Menu
    {
        private bool _LineMoving;

        private static readonly Brush _HighlightBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(160, 85, 55, 155));
        private static readonly Dictionary<char, (char Min, char Max)> _Digits = new Dictionary<char, (char Min, char Max)>()
        {
            { '-', ('\u208B', '\u207B') },
            { '+', ('\u208A', '\u207A') },
            { '0', ('\u2080', '\u2070') },
            { '1', ('\u2081', '\u00B9') },
            { '2', ('\u2082', '\u00B2') },
            { '3', ('\u2083', '\u00B3') },
            { '4', ('\u2084', '\u2074') },
            { '5', ('\u2085', '\u2075') },
            { '6', ('\u2086', '\u2076') },
            { '7', ('\u2087', '\u2077') },
            { '8', ('\u2088', '\u2078') },
            { '9', ('\u2089', '\u2079') },

            { '\u207B', ('\u208B', '\u207B') },
            { '\u207A', ('\u208A', '\u207A') },
            { '\u2070', ('\u2080', '\u2070') },
            { '\u00B9', ('\u2081', '\u00B9') },
            { '\u00B2', ('\u2082', '\u00B2') },
            { '\u00B3', ('\u2083', '\u00B3') },
            { '\u2074', ('\u2084', '\u2074') },
            { '\u2075', ('\u2085', '\u2075') },
            { '\u2076', ('\u2086', '\u2076') },
            { '\u2077', ('\u2087', '\u2077') },
            { '\u2078', ('\u2088', '\u2078') },
            { '\u2079', ('\u2089', '\u2079') },

            { '\u208A', ('\u208A', '\u207A') },
            { '\u208B', ('\u208B', '\u207B') },
            { '\u2080', ('\u2080', '\u2070') },
            { '\u2081', ('\u2081', '\u00B9') },
            { '\u2082', ('\u2082', '\u00B2') },
            { '\u2083', ('\u2083', '\u00B3') },
            { '\u2084', ('\u2084', '\u2074') },
            { '\u2085', ('\u2085', '\u2075') },
            { '\u2086', ('\u2086', '\u2076') },
            { '\u2087', ('\u2087', '\u2077') },
            { '\u2088', ('\u2088', '\u2078') },
            { '\u2089', ('\u2089', '\u2079') }
        };
        private static readonly Dictionary<char, char> _NormalizedDigits = new Dictionary<char, char>()
        {
            { '\u207B', '-' },
            { '\u207A', '+' },
            { '\u2070', '0' },
            { '\u00B9', '1' },
            { '\u00B2', '2' },
            { '\u00B3', '3' },
            { '\u2074', '4' },
            { '\u2075', '5' },
            { '\u2076', '6' },
            { '\u2077', '7' },
            { '\u2078', '8' },
            { '\u2079', '9' },

            { '\u208B', '-' },
            { '\u208A', '+' },
            { '\u2080', '0' },
            { '\u2081', '1' },
            { '\u2082', '2' },
            { '\u2083', '3' },
            { '\u2084', '4' },
            { '\u2085', '5' },
            { '\u2086', '6' },
            { '\u2087', '7' },
            { '\u2088', '8' },
            { '\u2089', '9' }
        };
        public static readonly (string, char)[] _KeyWords =
        [
            ("+-", '\u00B1'),
            ("~=", '\u2248'),
            ("!=", '\u2260'),
            ("<=", '\u2264'),
            (">=", '\u2265'),
            ("->", '\u2192'),
            ("<-", '\u2190'),

            ("up", '\u2191'),
            ("down", '\u2193'),

            ("sqrt4", '\u221C'),
            ("sqrt3", '\u221B'),
            ("sqrt", '\u221A'),

            ("null", '\u2205'),

            ("deg", '\u00B0'),
            ("angle", '\u2220'),

            ("infinity", '\u221E'),
            ("sum", '\u2211'),

            ("copyr", '\u00A9'),
            ("rego", '\u00AE'),
            ("tm", '\u2122'),

            ("pi", '\u03C0'),
            ("alpha", '\u03B1'),
            ("beta", '\u03B2'),
            ("lamda", '\u03BB'),
            ("gamma", '\u03B3'),
            ("delta", '\u0394'),
            ("omega", '\u03C9'),
            ("nu", '\u03BD'),
            ("eta", '\u03B7')
        ];


        public override void Initialize()
        {
            AddKey(ToLower, Key.L, ModifierKeys.Control | ModifierKeys.Shift, true);
            AddKey(ToUpper, Key.U, ModifierKeys.Control | ModifierKeys.Shift, true);
            AddKey(Capitalize, Key.C, ModifierKeys.Control | ModifierKeys.Shift, true);

            AddKey(Find, Key.F, ModifierKeys.Control);
            AddKey(Replace, Key.H, ModifierKeys.Control);
            AddKey(Highlight, Key.M, ModifierKeys.Control);
            AddKey(Do, Key.D, ModifierKeys.Control);
            AddKey(DuplicateLine, Key.D, ModifierKeys.Control | ModifierKeys.Shift);

            AddKey(ConvertChars, Key.T, ModifierKeys.Control, true);
            AddKey(GetInformation, Key.I, ModifierKeys.Control, true);
            
            AddKey(ToUnicode, Key.U, ModifierKeys.Control);

            AddKey(Event: WriteTab, Key.Tab, ModifierKeys.None, true);
            AddKey(LevelDown, Key.Tab, ModifierKeys.Shift, true);
            AddKey(LevelUp);

            AddKey(UpDigit, Key.Up, ModifierKeys.Control, true);
            AddKey(DownDigit, Key.Down, ModifierKeys.Control, true);
            AddKey(NormalizeDigit, Key.D0, ModifierKeys.Control, true);

            AddHotKey("NormalizeDigit_NumPad", NormalizeDigit, Key.NumPad0, ModifierKeys.Control, true);

            AddKey(RaiseLine, Key.Up, ModifierKeys.Alt | ModifierKeys.Shift);
            AddKey(LowerLine, Key.Down, ModifierKeys.Alt | ModifierKeys.Shift);
        }


        private void ToLower(object Sender, RoutedEventArgs E)
        {
            ConvertText(String => String.ToUpper());
        }
        private void ToUpper(object Sender, RoutedEventArgs E)
        {
            ConvertText(String => String.ToLower());
        }
        private void Capitalize(object Sender, RoutedEventArgs E)
        {
            ConvertText(CapitalizeEachWord);
        }

        private void Do(object Sender, RoutedEventArgs E)
        {
            StatusBar.Write("Function not released");
        }

        private void DuplicateLine(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetCurrentLine();

            _Editor.CaretPosition = Range.End;
            AppendText(_NewLine + Range.Text.TrimEnd());
        }


        private void ConvertChars(object Sender, RoutedEventArgs E)
        {
            ConvertText(ConvertChars);
        }

        private void GetInformation(object Sender, RoutedEventArgs E)
        {
            TextRange Range = _Editor.Selection;

            if (Range.IsEmpty)
            {
                Tab Tab = _Hub._TabManager.SelectedTab;

                if (Tab._CurrentFile is not null)
                {
                    FileInfo Info = new FileInfo(Tab._CurrentFile);

                    if (Info.Exists)
                    {
                        StatusBar.Write
                        (
                            $"File: '{Tab._CurrentFile}'  |  Date of creation: '{Info.CreationTime}'  |  Date of writing: '{Info.LastWriteTime}'"
                        );
                    }
                    else
                    {
                        StatusBar.Write("File: 'Удалённый файл'");
                    }
                }
                else
                {
                    StatusBar.Write("File: 'Не сохранённый файл'");
                }
                return;
            }

            string Text = Range.Text;
            int Length = Text.Length;
            int Lines = Text.CountOf('\n') + 1;

            if (Range.End.CompareTo(_Editor.Document.ContentEnd) == 0)
            {
                Length -= _NewLine.Length;
                Lines--;
            }

            if (Length == 1)
            {
                char Char = Text[0];
                StatusBar.Write
                (
                    $"Character: '{Char}'  |  Decimal: '{(int)Char}'  |  Unicode: '{(int)Char:X4}'"
                );
            }
            else
            {
                StatusBar.Write
                (
                    $"Characters: '{Length}'  |  Lines: '{Lines}'"
                );
            }
        }

        private void Highlight(object Sender, RoutedEventArgs E)
        {
            TextRange Range = _Editor.Selection;

            if (Range.IsEmpty)
            {
                _Editor.GetAll().ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
            }
            else
            {
                Range.ApplyPropertyValue(TextElement.BackgroundProperty, _HighlightBrush);
                _Editor.DeSelect();
                _Editor.CaretPosition = Range.End;
            }
        }


        private void Find(object Sender, RoutedEventArgs E)
        {
            OpenMenu(SideBarType.Searching);
        }

        private void Replace(object Sender, RoutedEventArgs E)
        {
            OpenMenu(SideBarType.Replacing);
        }

        private void GoTo(object Sender, RoutedEventArgs E)
        {
            _SideBar.OpenMenu(SideBarType.GoTo);
        }


        private void UpDigit(object Sender, RoutedEventArgs E)
        {
            ConvertDigits(Char => _Digits.GetValue(Char, Char, Pair => Pair.Max));
        }

        private void DownDigit(object Sender, RoutedEventArgs E)
        {
            ConvertDigits(Char => _Digits.GetValue(Char, Char, Pair => Pair.Min));
        }

        private void NormalizeDigit(object Sender, RoutedEventArgs E)
        {
            ConvertDigits(Char => _NormalizedDigits.GetValue(Char, Char));
        }


        private void ToUnicode(object Sender, RoutedEventArgs E)
        {
            ConvertText(ToUnicode);
        }


        private void LevelUp(object sender, RoutedEventArgs e)
        {
            if (DocumentIsEmpty())
            {
                StatusBar.Write(Translater._Current._EmptyText);
                return;
            }

            TextRange Range = GetSelectedLinesOrAll();

            string[] Lines = Array.ConvertAll
            (
                Range.Text.Split(_NewLine, StringSplitOptions.None),
                Line => string.IsNullOrWhiteSpace(Line) ? string.Empty : '\t' + Line
            );

            Range.Text = string.Join(_NewLine, Lines);
        }

        private void LevelDown(object sender, RoutedEventArgs e)
        {
            if (DocumentIsEmpty())
            {
                StatusBar.Write(Translater._Current._EmptySelection);
                return;
            }

            TextRange Range = GetSelectedLines();

            string[] Lines = Array.ConvertAll
            (
                Range.Text.Split(_NewLine, StringSplitOptions.None),
                Line =>
                {
                    if (Line.StartsWith("\t"))
                    {
                        return Line[1..];
                    }
                    else if (Line.StartsWith("    "))
                    {
                        return Line[4..];
                    }
                    return Line;
                }
            );

            Range.Text = Lines.JoinTrimEnd(_NewLine);
        }


        private void WriteTab(object Sender, RoutedEventArgs E)
        {
            WriteTab();
        }

        private void WriteTab()
        {
            if (_Editor.Selection.IsEmpty)
            {
                AppendText("\t");
            }
            else
            {
                LevelUp(this, null);
            }
        }


        private void RaiseLine(object sender, RoutedEventArgs e)
        {
            if (_LineMoving)
            {
                return;
            }

            _LineMoving = true;

            TextRange CurrentLine = GetSelectedLines();
            TextPointer StartOfUpLine = CurrentLine.Start.GetLineStartPosition(-1);
            
            int CaretOffset = CurrentLine.Start.GetOffsetToPosition(_Editor.CaretPosition);

            if (StartOfUpLine is null)
            {
                _LineMoving = false;
                return;
            }

            string LineText = CurrentLine.Text;

            _Window.Dispatcher.BeginInvoke
            (
                () =>
                {
                    _Editor.BeginChange();

                    new TextRange
                    (
                        CurrentLine.Start.GetPositionAtOffset(-_NewLine.Length - 1),
                        CurrentLine.End
                    ).Text = string.Empty;

                    _Editor.CaretPosition = StartOfUpLine;

                    TextRange Range = new TextRange(_Editor.CaretPosition, _Editor.CaretPosition);
                    Range.Text = LineText + _NewLine;

                    _Editor.CaretPosition = StartOfUpLine.GetPositionAtOffset(CaretOffset);
                    _Editor.Focus();

                    _Editor.EndChange();
                    _LineMoving = false;
                },
                DispatcherPriority.Input
            );
        }

        private void LowerLine(object sender, RoutedEventArgs e)
        {
            if (_LineMoving)
            {
                return;
            }

            _LineMoving = true;

            TextRange CurrentLine = GetCurrentLine();
            TextPointer StartOfDownLine = CurrentLine.Start.GetLineStartPosition(2);

            int CaretOffset = CurrentLine.Start.GetOffsetToPosition(_Editor.CaretPosition);
            bool IsEnd = false;

            if (StartOfDownLine is null)
            {
                if (CurrentLine.Start.GetLineStartPosition(1) is null)
                {
                    _LineMoving = false;
                    return;
                }

                StartOfDownLine = _ContentEnd;
                IsEnd = true;
            }

            string LineText = CurrentLine.Text;

            _Window.Dispatcher.BeginInvoke
            (
                () =>
                {
                    _Editor.BeginChange();

                    new TextRange
                    (
                        CurrentLine.Start,
                        CurrentLine.End.GetPositionAtOffset(+_NewLine.Length + 1) ?? CurrentLine.End
                    ).Text = string.Empty;

                    _Editor.CaretPosition = StartOfDownLine;

                    TextRange Range = new TextRange(_Editor.CaretPosition, _Editor.CaretPosition);

                    if (IsEnd)
                    {
                        Range.Text = _NewLine + LineText;
                        _Editor.CaretPosition = StartOfDownLine.GetPositionAtOffset(CaretOffset - LineText.Length);
                    }
                    else
                    {
                        Range.Text = LineText + _NewLine;
                        _Editor.CaretPosition = StartOfDownLine.GetLineStartPosition(0).GetPositionAtOffset(CaretOffset);
                    }

                    _Editor.Focus();
                    _Editor.EndChange();
                    _LineMoving = false;
                },
                DispatcherPriority.Input
            );
        }


        private string ConvertChars(string String)
        {
            String = StringParser.Parse(String);

            int Start = 0,
                Index = 0;

            StringBuilder Builder = new StringBuilder(String.Length);

            while (Index < String.Length)
            {
                bool KeyNotFounded = true;

                if (String[Index] == '^')
                {
                    Start = Index + 1;
                    Index = String.Skip(Start, Char => Char == '-' || Char == '+' || char.IsDigit(Char));

                    Builder.Append(String[Start..Index].ConvertAll(Char => _Digits[Char].Max));
                    Index += Index - Start - 1;
                    continue;
                }

                foreach ((string, char) Key in _KeyWords)
                {
                    if (String.Begins(Index, Key.Item1, true))
                    {
                        Builder.Append(Key.Item2);
                        Index += Key.Item1.Length;

                        KeyNotFounded = false;
                        break;
                    }
                }

                if (KeyNotFounded)
                {
                    Builder.Append(String[Index]);
                    Index++;
                }
            }

            return Builder.ToString();
        }

        private void ConvertDigits(Func<char, char> Converter)
        {
            TextRange Range = GetSelection();
            if (Range.IsEmpty)
            {
                TextPointer Caret = _Editor.CaretPosition;

                Range = new TextRange
                (
                    Caret.GetPositionAtOffset(-1) ?? _Editor.Document.ContentStart,
                    Caret
                );

                if (Range.IsEmpty)
                {
                    StatusBar.Write(Translater._Current._EmptySelection);
                    return;
                }
            }

            Range.Text = Range.Text.ConvertAll(Converter);
            _Editor.CaretPosition = Range.End;
        }

        private string ToUnicode(string String)
        {
            StringBuilder Builder = new StringBuilder(String.Length * 5);

            foreach (char Char in String)
            {
                if (char.IsAscii(Char))
                {
                    Builder.Append(Char);
                }
                else
                {
                    Builder.Append("\\u");
                    Builder.Append(((int)Char).ToString("X4"));
                }
            }

            return Builder.ToString();
        }


        private string CapitalizeEachWord(string Text)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return Text;
            }

            string[] Lines = Text.SplitIntoLines();

            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = CapitalizeWordsInLine(Lines[i]);
            }

            return string.Join(_NewLine, Lines);
        }
        private string CapitalizeWordsInLine(string Line)
        {
            if (string.IsNullOrEmpty(Line))
            {
                return Line;
            }

            List<string> Words = new List<string>();
            StringBuilder CurrentWord = new StringBuilder();
            bool IsWord = false;

            foreach (char Char in Line)
            {
                if (char.IsWhiteSpace(Char))
                {
                    if (IsWord)
                    {
                        Words.Add(CurrentWord.ToString().Capitalize());
                        CurrentWord.Clear();
                        IsWord = false;
                    }
                    Words.Add(Char.ToString());
                }
                else
                {
                    CurrentWord.Append(Char);
                    IsWord = true;
                }
            }

            if (IsWord)
            {
                Words.Add(CurrentWord.ToString().Capitalize());
            }

            return string.Concat(Words);
        }
    }
}