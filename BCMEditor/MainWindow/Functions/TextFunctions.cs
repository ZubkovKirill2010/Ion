using BCMEditor.Extensions;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using Zion;
using Zion.Text;

namespace BCMEditor
{
    public partial class MainWindow : Window
    {
        private static string RemoveEmptyLines(string String)
        {
            using (StringReader Reader = new StringReader(String))
            {
                StringBuilder Builder = new StringBuilder();
                string Line;

                while ((Line = Reader.ReadLine()) is not null)
                {
                    if (!string.IsNullOrWhiteSpace(Line))
                    {
                        Builder.AppendLine(Line);
                    }
                }

                if (Builder.Length > 0 && Builder[Builder.Length - 1] == '\n')
                {
                    Builder.Length--;
                    if (Builder.Length > 0 && Builder[Builder.Length - 1] == '\r')
                    {
                        Builder.Length--;
                    }
                }

                return Builder.ToString();
            }
        }

        private static string Trim(string String)
        {
            StringBuilder Result = new StringBuilder(String.Length);
            int LineStart = 0;
            int LineEnd = 0;

            for (int i = 0; i < String.Length; i++)
            {
                if (String[i] == '\n')
                {
                    LineEnd = i - 1;

                    while (LineEnd >= LineStart && char.IsWhiteSpace(String[LineEnd]))
                    {
                        LineEnd--;
                    }

                    if (LineEnd >= LineStart)
                    {
                        Result.Append(String, LineStart, LineEnd - LineStart + 1);
                    }

                    Result.Append(String[i]);
                    LineStart = i + 1;
                }
            }

            int LastLineEnd = String.Length - 1;
            while (LastLineEnd >= LineStart && char.IsWhiteSpace(String[LastLineEnd]))
            {
                LastLineEnd--;
            }

            if (LastLineEnd >= LineStart)
            {
                Result.Append(String, LineStart, LastLineEnd - LineStart + 1);
            }

            return Result.ToString();
        }

        private string CorrectSpaces(string String)
        {
            List<char> Result = new List<char>(String.Length);
            bool IsSpace = false;

            foreach (char Char in String)
            {
                if (Char == ' ')
                {
                    if (!IsSpace)
                    {
                        Result.Add(' ');
                        IsSpace = true;
                    }
                }
                else
                {
                    Result.Add(Char);
                    IsSpace = false;
                }
            }

            return new string(Result.ToArray());
        }

        private string CorrectPunctuation(string String)
        {
            List<char> Result = new List<char>(String.Length);
            bool SpaceNeeded = false;
            bool ToUpperNext = false;
            bool InsideQuotes = false;

            for (int i = 0; i < String.Length; i++)
            {
                char Current = String[i];

                if (Current == '"')
                {
                    InsideQuotes = !InsideQuotes;
                    Result.Add(Current);
                    continue;
                }

                if (Current == ' ')
                {
                    if (Result.Count == 0 || Result[^1] == ' ')
                    {
                        continue;
                    }

                    SpaceNeeded = false;
                    Result.Add(Current);
                    continue;
                }

                if (IsPunctuationMark(Current))
                {
                    while (Result.Count > 0 && Result[^1] == ' ')
                    {
                        Result.RemoveAt(Result.Count - 1);
                    }

                    Result.Add(Current);
                    ToUpperNext = !InsideQuotes;
                    SpaceNeeded = !InsideQuotes;
                    continue;
                }
                else if (IsPunctuation(Current))
                {
                    while (Result.Count > 0 && Result[^1] == ' ')
                    {
                        Result.RemoveAt(Result.Count - 1);
                    }

                    Result.Add(Current);
                    SpaceNeeded = !InsideQuotes;  // Пробел только вне кавычек
                    continue;
                }

                if (SpaceNeeded && Current != ' ' && !InsideQuotes)
                {
                    Result.Add(' ');
                    SpaceNeeded = false;
                }

                if (ToUpperNext && char.IsLetter(Current))
                {
                    Current = char.ToUpper(Current);
                    ToUpperNext = false;
                }

                Result.Add(Current);
            }

            while (Result.Count > 0 && Result[^1] == ' ')
            {
                Result.RemoveAt(Result.Count - 1);
            }

            return new string(Result.ToArray());
        }

        private bool IsPunctuationMark(char Char) => Char == '.' || Char == '!' || Char == '?';
        private bool IsPunctuation(char Char) => Char == ',' || Char == ';' || Char == ':';


        private void WriteTab(object Sender, RoutedEventArgs E)
        {
            WriteTab();
        }

        private void LevelUp(object sender, RoutedEventArgs e)
        {
            if (TextEditor.Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }

            TextRange Range = GetLines();

            if (Range.IsEmpty)
            {
                Log("Пустой текст");
                return;
            }

            string[] Lines = Range.Text.Split(new[] { _NewLine }, StringSplitOptions.None).ConvertAll
            (
                Line => string.IsNullOrEmpty(Line) ? Line : '\t' + Line
            );

            Range.Text = string.Join(_NewLine, Lines);
            TextEditor.DeSelect();
        }

        private void LevelDown(object sender, RoutedEventArgs e)
        {
            if (TextEditor.Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }

            TextRange Range = GetLines();

            if (Range.IsEmpty)
            {
                Log("Пустой текст");
                return;
            }

            string[] Lines = Range.Text.Split(_NewLine, StringSplitOptions.None).ConvertAll
            (
                Line =>
                {
                    if (Line.StartsWith("\t"))
                    {
                        return Line[1..];
                    }
                    else if (Line.StartsWith("    "))
                    {
                        return Line[4..];
                    }
                    return Line;
                }
            );

            Range.Text = string.Join(_NewLine, Lines);
            TextEditor.DeSelect();
        }


        private void WriteTab()
        {
            if (TextEditor.Selection.IsEmpty)
            {
                AppendText("\t");
            }
            else
            {
                LevelUp(null, null);
            }
        }

        private void WriteNewLine(ModifierKeys Modifier)
        {
            if (TextEditor.Document.Blocks.Count == 0)
            {
                Dispatcher.Invoke(() =>
                {
                    TextEditor.Document.Blocks.Add(new Paragraph(new Run(_NewLine)));
                    TextEditor.CaretPosition = TextEditor.Document.ContentEnd;
                }, DispatcherPriority.Input);
                return;
            }

            TextRange Range = GetLine();
            string String = Range.Text;

            if (String.Length == 0)
            {
                AppendText(_NewLine);
                return;
            }

            string Prefix = GetStart(String);


            InsertText(_NewLine + Prefix);
            //#Update
            //switch (Modifier)
            //{
            //    case ModifierKeys.Control:
            //        break;

            //    case ModifierKeys.Shift:
            //        break;

            //    default:
            //        InsertText(_NewLine + Prefix);
            //        break;
            //}
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