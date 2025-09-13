using Ion.Extensions;
using Ion.SideBar;
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
            string Line = GetLine(out TextPointer EndOfLine).Text;

            if (Line is null)
            {
                return;
            }

            TextEditor.CaretPosition = EndOfLine;

            AppendText(_NewLine);
            AppendText(Line);
        }
        private void RemoveEmptyLines(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();
            Range.Text = RemoveEmptyLines(Range.Text);
        }

        private void Do(object Sender, RoutedEventArgs E)
        {
            Log("Функция пока не реализована");
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

        //private void GoTo(object Sender, RoutedEventArgs E)
        //{
        //    _SideBar.OpenMenu(SideBarType.GoTo);
        //}
    }
}