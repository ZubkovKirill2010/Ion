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

            TextPointer SelectionStart = Editor.Selection.Start;
            TextPointer StartOfFirstLine = SelectionStart.GetLineStartPosition(0);

            if (StartOfFirstLine is null)
            {
                StartOfFirstLine = Editor.Document.ContentStart;
            }
            else
            {
                TextPointer SineStart = SelectionStart.GetLineStartPosition(0);
                if (SineStart is not null && SineStart.CompareTo(SelectionStart) == 0)
                {
                    StartOfFirstLine = SelectionStart;
                }
            }

            TextPointer SelectionEnd = Editor.Selection.End;
            TextPointer EndOfLastLine = SelectionEnd.GetLineStartPosition(1);

            if (EndOfLastLine is null)
            {
                EndOfLastLine = Editor.Document.ContentEnd;
            }
            else
            {
                TextPointer NextLineStart = SelectionEnd.GetLineStartPosition(1);
                if (NextLineStart is not null && NextLineStart.CompareTo(SelectionEnd) == 0)
                {
                    EndOfLastLine = NextLineStart;
                }
            }

            return new TextRange(StartOfFirstLine, EndOfLastLine);
        }

        private static void FixRangeText(TextRange Range, StringBuilder Builder)
        {
            if (Range.Start.GetLineStartPosition(-1) is not null)
            {
                Builder.AppendLine();
            }
        }
        private static string FixRangeText(TextRange Range, string String)
        {
            return Range.Start.GetLineStartPosition(-1) is not null ? _NewLine + String : String;
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