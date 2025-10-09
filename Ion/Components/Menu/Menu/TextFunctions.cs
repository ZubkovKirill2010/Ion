using Ion.Extensions;
using System.Windows.Documents;
using System.Windows.Input;
using Zion;

namespace Ion
{
    public sealed class TextFunctions : Menu
    {
        public override void Initialize()
        {
            AddKey((S, E) => WriteNewLine(Keyboard.Modifiers), Key.Enter, ModifierKeys.None);
            
            AddKey(SetCursorInStart, Key.Up, ModifierKeys.Control | ModifierKeys.Alt);
            AddKey(SetCursorInEnd, Key.Down, ModifierKeys.Control | ModifierKeys.Alt);
        }

        private void SetCursorInStart()
        {
            _Editor.CaretPosition = _Editor.Document.ContentStart;
        }

        private void SetCursorInEnd()
        {
            _Editor.CaretPosition = _Editor.Document.ContentEnd;
        }



        private void WriteNewLine(ModifierKeys Modifier)
        {
            TextRange Range = GetCurrentLine();
            string Insert = _NewLine + GetStart(Range.Text);

            switch (Modifier)
            {
                case ModifierKeys.Control:

                    TextPointer LineStart = _Editor.CaretPosition.GetLineStartPosition(0);

                    _Editor.CaretPosition = LineStart;
                    InsertText(Insert);
                    _Editor.CaretPosition = LineStart.GetPositionAtOffset(-_NewLine.Length) ?? _Editor.Document.ContentStart;
                    break;

                case ModifierKeys.Shift:

                    _Editor.CaretPosition = _Editor.CaretPosition.GetLineEndPosition();
                    InsertText(Insert);
                    break;

                default:

                    InsertText(Insert);
                    break;
            }
        }


        private static string GetStart(string Line)
        {
            int Index = 0;

            while (Index < Line.Length)
            {
                if (Line[Index] == '\t')
                {
                    Index++;
                }
                else if (Line.Begins(Index, "│ "))
                {
                    Index += 2;
                }
                else
                {
                    break;
                }
            }

            return Line[..Index];
        }
    }
}