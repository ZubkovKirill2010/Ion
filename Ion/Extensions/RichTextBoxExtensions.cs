using System.Windows.Controls;
using System.Windows.Documents;
using Zion;

namespace Ion.Extensions
{
    public static class RichTextBoxExtensions
    {
        public static TextRange GetAll(this RichTextBox TextBox)
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
            return TextBox.GetAll().Text.CountOf('\n') + 1;
        }

        public static void DeSelect(this RichTextBox TextBox)
        {
            TextBox.Selection.Select(TextBox.CaretPosition, TextBox.CaretPosition);
        }

        public static TextRange? GetSelection(this RichTextBox TextBox)
        {
            var Selection = TextBox.Selection;
            return Selection.IsEmpty ? null : new TextRange(Selection.Start, Selection.End);
        }
    }
}