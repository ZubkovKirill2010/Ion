using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BCMEditor.Tabs
{
    public sealed class CSharpTab : LanguageTab
    {
        private static readonly List<string> Keywords = new List<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
            "checked", "class", "const", "continue", "decimal", "default", "delegate",
            "do", "double", "else", "enum", "event", "explicit", "extern", "false",
            "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit",
            "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
            "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct",
            "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
            "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
        };

        private static readonly List<string> Types = new List<string>
        {
            "bool", "byte", "char", "decimal", "double", "float", "int", "long",
            "object", "sbyte", "short", "string", "uint", "ulong", "ushort"
        };

        private bool _isHighlighting = false;

        public CSharpTab() : base() { }
        public CSharpTab(FlowDocument Document) : base(Document) { }
        public CSharpTab(string Text) : base(Text) { }

        public override void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isHighlighting || !(sender is RichTextBox richTextBox))
                return;

            try
            {
                _isHighlighting = true;
                HighlightCSharpCode(richTextBox);
            }
            finally
            {
                _isHighlighting = false;
            }
        }

        private void HighlightCSharpCode(RichTextBox richTextBox)
        {
            // Сохраняем текущую позицию курсора и выделение
            TextPointer caretPosition = richTextBox.CaretPosition;
            TextSelection selection = richTextBox.Selection;

            // Получаем весь текст
            TextRange fullRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            string text = fullRange.Text;

            // Очищаем предыдущее форматирование
            fullRange.ClearAllProperties();

            // Оптимизация: подсвечиваем только видимую часть текста
            TextRange visibleRange = GetVisibleTextRange(richTextBox);
            if (visibleRange == null)
                visibleRange = fullRange;

            // Применяем подсветку для разных элементов
            HighlightPattern(visibleRange, @"(//.*?$|/\*.*?\*/)", Colors.Green, false); // Комментарии
            HighlightPattern(visibleRange, @"\b(" + string.Join("|", Keywords) + @")\b", Colors.Blue, true); // Ключевые слова
            HighlightPattern(visibleRange, @"\b(" + string.Join("|", Types) + @")\b", Colors.DarkBlue, true); // Типы
            HighlightPattern(visibleRange, @"""(?:\\.|[^""])*""|@""(?:(?!"")""|""""|[^""])*""|\$@""(?:(?!"")""|""""|[^""])*""|@\$""(?:(?!"")""|""""|[^""])*""", Colors.DarkRed, false); // Строки
            HighlightPattern(visibleRange, @"\b\d+(\.\d+)?\b", Colors.DarkCyan, false); // Числа

            // Восстанавливаем позицию курсора и выделение
            richTextBox.CaretPosition = caretPosition;
            richTextBox.Selection.Select(selection.Start, selection.End);
        }

        private TextRange GetVisibleTextRange(RichTextBox richTextBox)
        {
            try
            {
                TextPointer start = richTextBox.GetPositionFromPoint(new Point(0, 0), true);
                TextPointer end = richTextBox.GetPositionFromPoint(new Point(richTextBox.ActualWidth, richTextBox.ActualHeight), true);

                if (start != null && end != null && start.CompareTo(end) < 0)
                    return new TextRange(start, end);
            }
            catch { }

            return null;
        }

        private void HighlightPattern(TextRange searchRange, string pattern, Color color, bool isBold)
        {
            string text = searchRange.Text;
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                TextPointer start = searchRange.Start.GetPositionAtOffset(match.Index);
                TextPointer end = searchRange.Start.GetPositionAtOffset(match.Index + match.Length);

                if (start != null && end != null)
                {
                    TextRange matchRange = new TextRange(start, end);
                    matchRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));

                    if (isBold)
                        matchRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                }
            }
        }

        protected override string? GetFilePath()
            => GetFilePath("Файлы C# (*.cs)|*.cs|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*", ".cs");
    }
}