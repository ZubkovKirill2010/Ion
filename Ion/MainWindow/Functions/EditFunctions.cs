using Ion.Extensions;
using Ion.SideBar;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Zion;

namespace Ion
{
    public partial class MainWindow : Window
    {
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
        private static readonly (string, char)[] _KeyWords =
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

            ("pi", '\u03C0'),
            ("deg", '\u00B0'),
            ("angle", '\u2220'),

            ("infinity", '\u221E'),
            ("sum", '\u2211'),

            ("copyr", '\u00A9'),
            ("rego", '\u00AE'),
            ("tm", '\u2122'),

            ("alpha", '\u03B1'),
            ("beta", '\u03B2'),
            ("gamma", '\u03B3')
        ];

        private void ToLower(object Sender, RoutedEventArgs E)
        {
            if (TextEditor.Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }

            TextRange Range = TextEditor.Selection;
            Range.Text = Range.Text.ToLower();
            TextEditor.DeSelect();
        }
        private void ToUpper(object Sender, RoutedEventArgs E)
        {
            if (TextEditor.Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }

            TextRange Range = TextEditor.Selection;
            Range.Text = Range.Text.ToUpper();
            TextEditor.DeSelect();
        }
        private void Capitalize(object Sender, RoutedEventArgs E)
        {
            if (TextEditor.Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }

            TextRange Range = TextEditor.Selection;

            Range.Text = CapitalizeEachWord(Range.Text);
            TextEditor.DeSelect();
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

        private void DuplicateLine(object Sender, RoutedEventArgs E)
        {
            var Range = GetLine();

            if (Range.IsEmpty)
            {
                return;
            }

            TextEditor.CaretPosition = Range.End;
            AppendText(_NewLine + Range.Text.TrimEnd());
        }
        private void RemoveEmptyLines(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();
            Range.Text = RemoveEmptyLines(Range.Text);
        }

        private void Do(object Sender, RoutedEventArgs E)
        {
            Log("Function not released");
        }

        private void ConvertChars(object Sender, RoutedEventArgs E)
        {
            if (TextEditor.Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }

            TextRange Range = TextEditor.Selection;

            Range.Text = ConvertChars(Range.Text);
            TextEditor.DeSelect();
        }

        private void GetInformation(object Sender, RoutedEventArgs E)
        {
            TextRange Range = TextEditor.Selection;

            if (Range.IsEmpty)
            {
                var Tab = _SelectedTab;

                if (Tab._CurrentFile is not null)
                {
                    FileInfo Info = new FileInfo(Tab._CurrentFile);

                    if (Info.Exists)
                    {
                        Log
                        (
                            $"File: '{Tab._CurrentFile}'  |  Date of creation: '{Info.CreationTime}'  |  Date of writing: '{Info.LastWriteTime}'"
                        );
                    }
                    else
                    {
                        Log("File: 'Удалённый файл'");
                    }
                }
                else
                {
                    Log("File: 'Не сохранённый файл'");
                }
                return;
            }

            string Text = Range.Text;
            int Length = Text.Length;
            int Lines = Text.CountOf('\n') + 1;

            if (Range.End.CompareTo(TextEditor.Document.ContentEnd) == 0)
            {
                Length -= _NewLine.Length;
                Lines--;
            }

            if (Length == 1)
            {
                char Char = Text[0];
                Log
                (
                    $"Character: '{Char}'  |  Decimal: '{(int)Char}'  |  Unicode: '{(int)Char:X4}'"
                );
            }
            else
            {
                Log
                (
                    $"Characters: '{Length}'  |  Lines: '{Lines}'"
                );
            }
        }

        private void Highlight(object Sender, RoutedEventArgs E)
        {
            TextRange Range = TextEditor.Selection;

            if (Range.IsEmpty)
            {
                //Clear formating
                TextEditor.GetAll().ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
            }
            else
            {
                //Highlight
                Range.ApplyPropertyValue(TextElement.BackgroundProperty, _HighlightBrush);
                TextEditor.DeSelect();
                TextEditor.CaretPosition = Range.End;
            }
        }

        private void Find(object Sender, RoutedEventArgs E)
        {
            _SideBar.OpenMenu(SideBarType.Searching);
        }
        private void Replace(object Sender, RoutedEventArgs E)
        {
            _SideBar.OpenMenu(SideBarType.Replacing);
        }

        private void UpDigit(object Sender, RoutedEventArgs E)
        {
            var Range = TextEditor.Selection;
            if (Range.IsEmpty)
            {
                Log("Translater._Current._EmptySelection");
                return;
            }
            Range.Text = Range.Text.ConvertAll(Char => _Digits.TryGetValue(Char, out var Pair) ? Pair.Max : Char);
        }
        private void DownDigit(object Sender, RoutedEventArgs E)
        {
            var Range = TextEditor.Selection;
            if (Range.IsEmpty)
            {
                Log("Translater._Current._EmptySelection");
                return;
            }
            Range.Text = Range.Text.ConvertAll(Char => _Digits.TryGetValue(Char, out var Pair) ? Pair.Min : Char);
        }
        private void NormalizeDigit(object Sender, RoutedEventArgs E)
        {
            var Range = TextEditor.Selection;
            if (Range.IsEmpty)
            {
                Log("Translater._Current._EmptySelection");
                return;
            }
            Range.Text = Range.Text.ConvertAll(Char => _NormalizedDigits.TryGetValue(Char, out var Normal) ? Normal : Char);
        }


        //private void GoTo(object Sender, RoutedEventArgs E)
        //{
        //    _SideBar.OpenMenu(SideBarType.GoTo);
        //}

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


        private void SetCursorInStart()
        {
            TextEditor.CaretPosition = TextEditor.Document.ContentStart;
        }
        private void SetCursorInEnd()
        {
            TextEditor.CaretPosition = TextEditor.Document.ContentEnd;
        }
    }
}