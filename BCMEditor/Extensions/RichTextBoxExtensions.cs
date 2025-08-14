using System.Windows.Controls;
using System.Windows.Documents;
using Zion.Text;

namespace BCMEditor.Extensions
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
    }
}