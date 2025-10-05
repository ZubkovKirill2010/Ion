using Ion.Extensions;
using System.IO;
using System.Text;
using System.Windows;
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

        public static string RemoveEmptyLines(string String)
        {
            using StringReader Reader = new StringReader(String);
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

        public static string Trim(string String)
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

        public static string CorrectSpaces(string String)
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

        public static string CorrectPunctuation(string String)
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

        private static bool IsPunctuationMark(char Char) => Char == '.' || Char == '!' || Char == '?';
        private static bool IsPunctuation(char Char) => Char == ',' || Char == ';' || Char == ':';



        private void SetCursorInStart()
        {
            TextEditor.CaretPosition = TextEditor.Document.ContentStart;
        }
        private void SetCursorInEnd()
        {
            TextEditor.CaretPosition = TextEditor.Document.ContentEnd;
        }


        private void WriteNewLine(ModifierKeys Modifier)
        {
            TextRange Range = GetLine();
            string Insert = _NewLine + GetStart(Range.Text);

            switch (Modifier)
            {
                case ModifierKeys.Control:

                    TextPointer LineStart = TextEditor.CaretPosition.GetLineStartPosition(0);

                    TextEditor.CaretPosition = LineStart;
                    InsertText(Insert);
                    TextEditor.CaretPosition = LineStart.GetPositionAtOffset(-_NewLine.Length) ?? TextEditor.Document.ContentStart;
                    break;

                case ModifierKeys.Shift:

                    TextEditor.CaretPosition = TextEditor.CaretPosition.GetLineEndPosition();
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