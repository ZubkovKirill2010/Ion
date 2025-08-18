using System.Windows;
using System.Windows.Documents;
using Zion.Text;

namespace BCMEditor
{
    public partial class MainWindow : Window
    {
        private void Trim(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                Log("������ �����");
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
                Log("������ �����");
            }
            else
            {
                Log
                (
                    $"������ ����������� {(Text.CheckBrackets() ? string.Empty : "��")} �����"
                );
            }

        }
        private void CorrectSpaces(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                Log("������ �����");
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
                Log("������ �����");
            }
            else
            {
                Range.Text = CorrectPunctuation(Range.Text);
            }
        }
    }
}