using BCMEditor.Extensions;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Zion;

namespace BCMEditor
{
    public partial class MainWindow : Window
    {
        private void Enumerate(object Sender, RoutedEventArgs E)
        {

        }
        private void DeEnumerate(object Sender, RoutedEventArgs E)
        {

        }

        private void ToStructure(object Sender, RoutedEventArgs E)
        {
            if (TextEditor.Selection.IsEmpty)
            {
                TextEditor.SelectAll();
            }

            string Text = GetRange().Text;
            string Result = Structure.Parse(Text).ToString();

            int LastLinesCount = Text.CountOf('\n');
            int NewLinesCount = Result.CountOf('\n');

            if (LastLinesCount - 1 > NewLinesCount)
            {
                Log($"Ошибка преобразования структуры");
                LogError($"Ошибка преобразования структуры [To] : {LastLinesCount}/{NewLinesCount}");
                return;
            }

            TextEditor.Selection.Text = Result;
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

            Selection.Text = Result;
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


            Range.Text = Builder.ToString();
            TextEditor.Selection.Select(Range.Start, Range.End);
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

            Range.Text = UnGroup(Lines, Text.Length, Level);
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

        private static string UnGroup(string[] Lines, int TextLength, int Level)
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
    }
}