using System.Windows.Documents;

namespace BCMEditor.Extensions
{
    public static class FlowDocumentExceptions
    {
        public static bool IsEmpty(this FlowDocument Document)
        {
            return Document.ContentStart == Document.ContentEnd;
        }
    }
}