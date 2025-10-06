using Ion.Extensions;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Zion;

namespace Ion
{
    public sealed class PunctuationMenu : Menu
    {
        public override void Initialize()
        {
            AddKey(CheckBrackets, Key.B, ModifierKeys.Control | ModifierKeys.Shift);
            AddKey(CorrectSpaces, Key.K, ModifierKeys.Control | ModifierKeys.Shift);
            AddKey(CorrectPunctuation, Key.P, ModifierKeys.Control | ModifierKeys.Shift);

            AddKey(JoinLines, Key.P, ModifierKeys.Control, true);
            AddKey(Trim, Key.T, ModifierKeys.Control | ModifierKeys.Shift, true);

            AddKey(RemoveEmptyLines, Key.Delete, ModifierKeys.Control | ModifierKeys.Shift);
        }



        private void Trim(object Sender, RoutedEventArgs E)
        {
            ConvertText(Trim);
        }

        private void CheckBrackets(object Sender, RoutedEventArgs E)
        {
            string Text = GetSelectionOrAll().Text;

            if (string.IsNullOrWhiteSpace(Text))
            {
                StatusBar.Write(Translater._Current._EmptyText);
            }
            else
            {
                StatusBar.Write
                (
                    $"{Translater._Current._BracketsPlaces} {(Text.CheckBrackets() ? string.Empty : Translater._Current._Not)} {Translater._Current._Right}"
                );
            }

        }
        private void CorrectSpaces(object Sender, RoutedEventArgs E)
        {
            ConvertText(CorrectSpaces);
        }
        private void CorrectPunctuation(object Sender, RoutedEventArgs E)
        {
            ConvertText(CorrectPunctuation);
        }

        private void JoinLines(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetSelectionOrAll();

            if (Range.IsEmpty)
            {
                return;
            }

            Range.Text = Range.Text.RemoveChars('\r', '\n');
            _Editor.DeSelect();
        }

        private void RemoveEmptyLines(object Sender, RoutedEventArgs E)
        {
            ConvertText(GetSelectedLinesOrAll, Text.RemoveEmptyLines);
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
    }
}