using Ion.Extensions;
using System.Windows;
using System.Windows.Documents;
using Zion;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private void Trim(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                Log(Translater._Current._EmptyText);
            }
            else
            {
                Range.Text = Trim(Range.Text);
            }
        }

        private void CheckBrackets(object Sender, RoutedEventArgs E)
        {
            string Text = GetRange().Text;

            if (string.IsNullOrWhiteSpace(Text))
            {
                Log(Translater._Current._EmptyText);
            }
            else
            {
                Log
                (
                    $"{Translater._Current._BracketsPlaces} {(Text.CheckBrackets() ? string.Empty : Translater._Current._Not)} {Translater._Current._Right}"
                );
            }

        }
        private void CorrectSpaces(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                Log(Translater._Current._EmptyText);
            }
            else
            {
                Range.Text = CorrectSpaces(Range.Text);
            }
        }
        private void CorrectPunctuation(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                Log(Translater._Current._EmptyText);
            }
            else
            {
                Range.Text = CorrectPunctuation(Range.Text);
            }
        }

        private void JoinLines(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                return;
            }

            Range.Text = Range.Text.RemoveChars('\r', '\n');
            TextEditor.DeSelect();
        }
    }
}