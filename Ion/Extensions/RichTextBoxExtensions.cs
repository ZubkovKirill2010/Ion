using System.Windows.Controls;
using System.Windows.Documents;
using Zion;

namespace Ion.Extensions
{
    public static class RichTextBoxExtensions
    {
        public static TextRange GetAllText(this RichTextBox TextBox)
        {
            FlowDocument Document = TextBox.Document;
            return new TextRange
            (
                Document.ContentStart,
                Document.ContentEnd
            );
        }

        public static int GetLinesCount(this RichTextBox TextBox)
        {
            return TextBox.GetAllText().Text.CountOf('\n') + 1;
        }

        public static void DeSelect(this RichTextBox TextBox)
        {
            TextBox.Selection.Select(TextBox.CaretPosition, TextBox.CaretPosition);
        }
    }
}