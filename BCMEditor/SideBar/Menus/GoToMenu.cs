//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Input;

//namespace BCMEditor.SideBarMenu
//{
//    public sealed class GoToMenu : SideBarMenu
//    {
//        private readonly RichTextBox _TextBox;
//        private readonly TextBox _IndexBox;

//        public override string _Header => "Перейти";
//        public override string _CancelButtonText => "Отмена";
//        public override string _ApplyButtonText => "Перейти";

//        public GoToMenu(MainWindow Window) : base(Window.GoToMenu)
//        {
//            _TextBox = Window.TextEditor;
//            _IndexBox = Window.GoTo_LineIndex;

//            _IndexBox.PreviewTextInput += IndexTextChanged;
//        }

//        private static void IndexTextChanged(object Sender, TextCompositionEventArgs E)
//        {
//            E.Handled = !char.IsDigit(E.Text, 0);
//        }

//        private int GetIndex()
//        {
//            string Text = _IndexBox.Text;

//            return string.IsNullOrEmpty(Text) ? 0 : int.Parse(Text);
//        }

//        private TextPointer GetLinePosition(int LineIndex)
//        {
//            TextPointer lineStart = _TextBox.Document.ContentStart;
//            int currentLine = 0;

//            // Последовательно перебираем строки
//            while (currentLine < LineIndex && lineStart is not null)
//            {
//                TextPointer nextLine = lineStart.GetLineStartPosition(1);
//                if (nextLine == null) break;

//                lineStart = nextLine;
//                currentLine++;
//            }

//            return lineStart ?? _TextBox.Document.ContentEnd;
//        }


//        public override void Apply()
//        {
//            int LineIndex = GetIndex();

//            _TextBox.CaretPosition = GetLinePosition(LineIndex);

//            //_TextBox.CaretPosition = _TextBox.Document.ContentStart;

//            CloseMenu();
//            _TextBox.Focus();
//        }
//    }
//}