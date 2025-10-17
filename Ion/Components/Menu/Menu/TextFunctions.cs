using Ion.Extensions;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using Zion;

namespace Ion
{
    public sealed class TextFunctions : Menu
    {
        public override void Initialize()
        {
            AddHotKey("WriteNewLine", WriteNewLine, Key.Enter, ModifierKeys.None, true);
            AddHotKey("WriteNewLineBefore", WriteNewLineBefore, Key.Enter, ModifierKeys.Control, true);
            AddHotKey("WriteNewLineAfter", WriteNewLineAfter, Key.Enter, ModifierKeys.Shift, true);

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



        private void WriteNewLine()
        {
            TextRange Range = GetCurrentLine();
            string Insert = _NewLine + GetStart(Range.Text);

            InsertText(Insert);
        }

        private void WriteNewLineBefore()
        {
            TextRange Range = GetCurrentLine();
            string Insert = _NewLine + GetStart(Range.Text);

            TextPointer LineStart = _Editor.CaretPosition.GetLineStartPosition(0);

            _Editor.CaretPosition = LineStart;
            InsertText(Insert);
            _Editor.CaretPosition = LineStart.GetPositionAtOffset(-_NewLine.Length) ?? _Editor.Document.ContentStart;
        }

        private void WriteNewLineAfter()
        {
            TextRange Range = GetCurrentLine();
            string Insert = _NewLine + GetStart(Range.Text);

            _Editor.CaretPosition = _Editor.CaretPosition.GetLineEndPosition();
            InsertText(Insert);
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