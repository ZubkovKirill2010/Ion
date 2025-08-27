using Ion.Extensions;
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
            return GetLine(out _);
        }

        private TextRange GetLine(out TextPointer EndOfLine)
        {
            RichTextBox Editor = TextEditor;

            TextPointer CaretPosition = Editor.CaretPosition;
            TextPointer StartOfLine = CaretPosition.GetLineStartPosition(0);
            EndOfLine = CaretPosition.GetLineStartPosition(1) ?? Editor.Document.ContentEnd;

            EndOfLine = EndOfLine.GetPositionAtOffset(-_NewLine.Length);

            return new TextRange(StartOfLine, EndOfLine);
        }


        private TextRange GetLines()
        {
            RichTextBox Editor = TextEditor;

            if (Editor.Selection.IsEmpty)
            {
                return GetAllText(Editor);
            }

            TextPointer selectionStart = Editor.Selection.Start;
            TextPointer startOfFirstLine = selectionStart.GetLineStartPosition(0);

            if (startOfFirstLine is null)
            {
                startOfFirstLine = Editor.Document.ContentStart;
            }
            else
            {
                TextPointer lineStart = selectionStart.GetLineStartPosition(0);
                if (lineStart is not null && lineStart.CompareTo(selectionStart) == 0)
                {
                    startOfFirstLine = selectionStart;
                }
            }
   
            TextPointer selectionEnd = Editor.Selection.End;
            TextPointer endOfLastLine = selectionEnd.GetLineStartPosition(1);

            if (endOfLastLine is null)
            {
                endOfLastLine = Editor.Document.ContentEnd;
            }
            else
            {
                TextPointer nextLineStart = selectionEnd.GetLineStartPosition(1);
                if (nextLineStart is not null && nextLineStart.CompareTo(selectionEnd) == 0)
                {
                    endOfLastLine = nextLineStart;
                }
            }

            return new TextRange(startOfFirstLine, endOfLastLine);
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