using Ion.Extensions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private void InsertText(string Text)
        {
            Dispatcher.BeginInvoke
            (
                () =>
                {
                    if (TextEditor.Selection.IsEmpty)
                    {
                        AppendText(Text);
                    }
                    else
                    {
                        TextEditor.Selection.Text = Text;
                    }
                    TextEditor.Focus();
                },
                DispatcherPriority.Input
            );
        }

        private void AppendText(string Text)
        {
            Dispatcher.BeginInvoke
            (
                () =>
                {
                    TextRange Range = new TextRange(TextEditor.CaretPosition, TextEditor.CaretPosition);
                    Range.Text = Text;
                    TextEditor.CaretPosition = Range.End;
                    TextEditor.Focus();
                },
                DispatcherPriority.Input
            );
        }


        private TextRange GetRange()
        {
            RichTextBox Editor = TextEditor;

            return Editor.Selection.IsEmpty ?
                GetAllText(Editor) :
                Editor.Selection;
        }


        private TextRange GetLine()
        {
            var Caret = TextEditor.CaretPosition;

            return new TextRange
            (
                Caret.GetLineStartPosition(0),
                Caret.GetLineStartPosition(1)?.GetPositionAtOffset(-_NewLine.Length) ?? TextEditor.Document.ContentEnd
            );

            //return GetLine(out _);
        }

        //private TextRange GetLine(out TextPointer EndOfLine)
        //{
        //    RichTextBox Editor = TextEditor;

        //    TextPointer CaretPosition = Editor.CaretPosition;
        //    TextPointer StartOfLine = CaretPosition.GetLineStartPosition(0);
        //    EndOfLine = CaretPosition.GetLineStartPosition(1) ?? Editor.Document.ContentEnd;

        //    EndOfLine = EndOfLine.GetPositionAtOffset(-_NewLine.Length);

        //    return new TextRange(StartOfLine, EndOfLine);
        //}


        private TextRange GetLines()
        {
            var Editor = TextEditor;
            var Selection = Editor.Selection;

            return Selection.IsEmpty
            ? GetLine()
            : new TextRange
            (
                Selection.Start.GetLineStartPosition(0) ?? Editor.Document.ContentStart,
                Selection.End.GetLineStartPosition(1)?.GetPositionAtOffset(-_NewLine.Length) ?? Editor.Document.ContentEnd
            );
        }


        private TextRange GetAllText(RichTextBox Editor)
        {
            FlowDocument Document = Editor.Document;
            if (Document.IsEmpty())
            {
                return new TextRange(Editor.CaretPosition, Editor.CaretPosition);
            }

            return new TextRange(Document.ContentStart, Document.ContentEnd);
        }
    }
}