using Ion.Extensions;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Zion;

namespace Ion
{
    public sealed class PunctuationMenu : Menu
    {
        public override void Initialize()
        {
            AddKey(CheckBrackets, Key.B, ModifierKeys.Control | ModifierKeys.Shift);
            AddKey(CorrectSpaces, Key.K, ModifierKeys.Control | ModifierKeys.Shift);
            AddKey(CorrectPunctuation, Key.P, ModifierKeys.Control | ModifierKeys.Shift);

            AddKey(JoinLines, Key.P, ModifierKeys.Control, true);
            AddKey(Trim, Key.T, ModifierKeys.Control | ModifierKeys.Shift, true);

            AddKey(RemoveEmptyLines);
        }

        private void RemoveEmptyLines(object Sender, RoutedEventArgs E)
        {
            var Range = GetLines();

            if (Range.IsEmpty)
            {
                Range = GetAllText();
            }

            Range.Text = TextFunctions.RemoveEmptyLines(Range.Text);
        }

        private void Trim(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                StatusBar.Write(Translater._Current._EmptyText);
            }
            else
            {
                Range.Text = TextFunctions.Trim(Range.Text);
            }
        }

        private void CheckBrackets(object Sender, RoutedEventArgs E)
        {
            string Text = GetRange().Text;

            if (string.IsNullOrWhiteSpace(Text))
            {
                StatusBar.Write(Translater._Current._EmptyText);
            }
            else
            {
                StatusBar.Write
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
                StatusBar.Write(Translater._Current._EmptyText);
            }
            else
            {
                Range.Text = TextFunctions.CorrectSpaces(Range.Text);
            }
        }
        private void CorrectPunctuation(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                StatusBar.Write(Translater._Current._EmptyText);
            }
            else
            {
                Range.Text = TextFunctions.CorrectPunctuation(Range.Text);
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