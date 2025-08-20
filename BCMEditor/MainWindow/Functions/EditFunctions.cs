using BCMEditor.Extensions;
using BCMEditor.SideBar;
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

            Log("Capitalize");

            TextRange Range = TextEditor.Selection;
            Range.Text = Range.Text.Capitalize();
            TextEditor.DeSelect();
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