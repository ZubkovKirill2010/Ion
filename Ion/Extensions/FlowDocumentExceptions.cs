using System.Windows.Documents;

namespace Ion.Extensions
{
    public static class FlowDocumentExceptions
    {
        public static bool IsEmpty(this FlowDocument Document)
        {
            return Document.ContentStart == Document.ContentEnd;
        }

        public static TextPointer GetLineEndPosition(this TextPointer Point)
        {
            return (Point.GetLineStartPosition(1) ?? Point.DocumentEnd).GetPositionAtOffset(-Environment.NewLine.Length);
        }
    }
}