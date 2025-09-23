using Ion.Extensions;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Zion;

namespace Ion
{
    public partial class MainWindow : Window
    {
        private void Enumerate(object Sender, RoutedEventArgs E)
        {
            if (TextEditor.Document.IsEmpty())
            {
                Log("Пустой документ");
                return;
            }

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

            string Text = Range.Text;
            string[] Lines = Text.SplitIntoLines();

            StringBuilder Builder = new StringBuilder(Text.Length + Lines.Length * 4);

            string CommonStart = GetCommonStart(Enumerable.Where(Lines, String => !string.IsNullOrWhiteSpace(String)));

            for (int i = 0; i < Lines.Length; i++)
            {
                string Line = Lines[i];

                if (Line.Length > CommonStart.Length)
                {
                    Builder.AppendLine($"{CommonStart}{i + 1}. {Line[CommonStart.Length..]}");
                }
                else
                {
                    Builder.AppendLine();
                }
            }

            Range.Text = Builder.ToString().TrimEnd();
            TextEditor.DeSelect();
        }
        private void DeEnumerate(object Sender, RoutedEventArgs E)
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

            string Text = Range.Text;
            string[] Lines = Text.SplitIntoLines();

            StringBuilder Builder = new StringBuilder(Text.Length + Lines.Length * 4);

            string CommonStart = GetCommonStart(Enumerable.Where(Lines, String => !string.IsNullOrWhiteSpace(String)));
            int Start = CommonStart.Length + 1;

            foreach (string Line in Lines)
            {
                if (Line.Length > Start &&
                    Accessor.Out(Line.IndexOf(". ", CommonStart.Length), out int DotIndex) != -1
                    && Line.TrueFor(Start, DotIndex, char.IsDigit))
                {
                    Builder.Append(CommonStart);
                    Builder.AppendLine(Line[(DotIndex + 2)..]);
                }
                else
                {
                    Builder.AppendLine(Line);
                }
            }

            Range.Text = Builder.ToString().TrimEnd();
            TextEditor.DeSelect();
        }

        private void ToStructure(object Sender, RoutedEventArgs E)
        {
            if (TextEditor.Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }

            TextRange Range = GetLines();
            string Text = Range.Text;
            string Result = Structure.Parse(Text).ToString();

            int LastLinesCount = Text.CountOf('\n');
            int NewLinesCount = Result.CountOf('\n');

            if (LastLinesCount - 1 > NewLinesCount)
            {
                Log($"Ошибка преобразования структуры");
                LogError($"Ошибка преобразования структуры [To] : {LastLinesCount}/{NewLinesCount}");
                return;
            }

            TextEditor.Selection.Text = Result.TrimEnd();
            TextEditor.DeSelect();
        }
        private void FromStructure(object Sender, RoutedEventArgs E)
        {
            TextSelection Selection = TextEditor.Selection;
            if (Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }
            string Text = GetRange().Text;

            StringBuilder Builder = new StringBuilder(Text.Length);

            Structure.UnParse(Text).InvokeForAll
            (
                (int Level, string String) =>
                {
                    Builder.Append('\t', Level);
                    Builder.Append(String);
                    Builder.AppendLine();
                }
            );

            string Result = Builder.ToString();

            int LastLinesCount = Text.CountOf('\n');
            int NewLinesCount = Result.CountOf('\n');

            if (LastLinesCount - 1 > NewLinesCount)
            {
                Log($"Ошибка преобразования структуры");
                LogError($"Ошибка преобразования структуры [From] : {LastLinesCount}/{NewLinesCount}");
                return;
            }

            Selection.Text = Result.TrimEnd();
            TextEditor.DeSelect();
        }

        private void Group(object Sender, RoutedEventArgs E)
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

            string Text = Range.Text;

            string[] Lines = Text.SplitIntoLines().TrimEmptyLines();
            StringBuilder Builder = new StringBuilder(Text.Length + Lines.Length * 2 + 27);
            

            Builder.AppendLine("╭──────┤ Header");
            if (Lines.Length > 0)
            {
                foreach (string Line in Lines)
                {
                    Builder.Append("│ ");
                    Builder.AppendLine(Line);
                }
            }
            else
            {
                Builder.AppendLine("│ ");
            }
            Builder.Append("╰──────────┘");


            Range.Text = Builder.ToString().TrimEnd();
            TextEditor.DeSelect();
        }
        private void Ungroup(object Sender, RoutedEventArgs E)
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

            string Text = Range.Text;
            string[] Lines = Text.SplitIntoLines().TrimEmptyLines();

            if (!IsGroup(Lines, out int Level))
            {
                //Log("Нельзя разгруппировать не группу");
                return;
            }

            Range.Text = UnGroup(Range, Lines, Text.Length, Level).TrimEnd();
            TextEditor.DeSelect();
        }


        private static bool IsGroup(string[] Lines, out int Level)
        {
            Level = -1;
            if (Lines.Length < 3)
            {
                return false;
            }
            Level = Lines[0].GetLevel();
            int End = Lines.Length - 1;

            if (!Lines[0].Begins(Level, "╭──────┤") || !Lines[End].Begins(Level, "╰──────────┘"))
            {
                return false;
            }

            for (int i = 1; i < End; i++)
            {
                if (!Lines[i].Begins(Level, "│ "))
                {
                    return false;
                }
            }

            return true;
        }

        private string UnGroup(TextRange Range, string[] Lines, int TextLength, int Level)
        {
            StringBuilder Builder = new StringBuilder(TextLength - Lines.Length * 2 - 27);
            

            Level += 2;
            int End = Lines.Length - 1;

            for (int i = 1; i < End; i++)
            {
                Builder.AppendLine(Lines[i][Level..]);
            }

            return Builder.ToString();
        }


        private static string GetCommonStart(IEnumerable<string> Lines)
        {
            if (Lines.IsEmpty())
            {
                return string.Empty;
            }

            string First = Lines.First();
            int End = Lines.Where(String => !string.IsNullOrWhiteSpace(String)).Select(String => String.Length).Min();
            int Index = 0;

            while (Index < End)
            {
                char Char = First[Index];

                if (char.IsLetterOrDigit(Char) || Lines.Any(String => String[Index] != Char))
                {
                    break;
                }

                Index++;
            }
            return First[..Index];
        }
    }
}