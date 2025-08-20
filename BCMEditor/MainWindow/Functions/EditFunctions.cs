using BCMEditor.Extensions;
using BCMEditor.SideBar;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Zion;

namespace BCMEditor
{
    public partial class MainWindow : Window
    {
        private void ToLower(object Sender, RoutedEventArgs E)
        {
            if (TextEditor.Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }

            Log("ToLower");

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

            Log("ToUpper");

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