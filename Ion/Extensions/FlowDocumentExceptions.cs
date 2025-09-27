using System.Windows.Documents;

namespace Ion.Extensions
{
    public static class FlowDocumentExceptions
    {
        public static bool IsEmpty(this FlowDocument Document)
        {
            return Document.ContentStart.CompareTo(Document.ContentEnd) == 0;
        }

        public static TextPointer GetLineEndPosition(this TextPointer Point)
        {
            return (Point.GetLineStartPosition(1) ?? Point.DocumentEnd).GetPositionAtOffset(-Environment.NewLine.Length);
        }

        public static TextPointer GetLineStartPosition(this FlowDocument Document, int LineIndex)
        {
            return Document.ContentStart.GetPositionAtOffset(1).GetLineStartPosition(LineIndex) ?? Document.ContentEnd;
        }
    }
}