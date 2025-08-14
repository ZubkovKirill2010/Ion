using BCMEditor.SideBarMenu;
using BCMEditor.Tabs;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Zion;
using Zion.Text;

namespace BCMEditor
{
    public partial class MainWindow : Window
    {
        #region File
        private void New(object Sender, RoutedEventArgs E)
        {
            if (Sender is MenuItem Item)
            {
                Tab NewTab = Tab.GetEditor(Item.Header.ToString() ?? string.Empty);
                AddTab(NewTab);
                Log($"Новая вкладки ({NewTab.GetType().Name})");
            }
            else
            {
                Log("Неверный вызов функции New");
            }
        }
        private void Open(object Sender, RoutedEventArgs E)
        {
            OpenFileDialog Dialog = new OpenFileDialog
            {
                Title = "Открыть файл",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Все файлы (*.*)|*.*|Bcm файлы (*.bcm)|*.bcm|Текстовые файлы (*.txt)|*.txt|Файлы C# (*.cs)|*.cs",
                DefaultExt = ""
            };

            if (Dialog.ShowDialog() == true)
            {
                string FilePath = Dialog.FileName;

                if (!File.Exists(FilePath))
                {
                    Log($"Файл \"{FilePath}\" не существует");
                    return;
                }

                try
                {
                    string Extension = Path.GetExtension(FilePath);

                    try
                    {
                        Tab NewTab = Tab.GetEditor(Extension);

                        NewTab._CurrentFile = FilePath;
                        NewTab.ReadFile();

                        AddTab(NewTab);

                        NewTab._IsSaved = true;

                    }
                    catch (Exception Exception)
                    {
                        Log($"Ошибка при создании вкладки: {Exception}");
                    }
                }
                catch (Exception Exception)
                {
                    Log($"Ошибка при чтении файла \"{FilePath}\": {Exception}");
                }
            }
        }

        private void Save(object Sender, RoutedEventArgs E)
        {
            _Editor.Save();
            ReloadButton.IsEnabled = true;
        }
        private void SaveAs(object Sender, RoutedEventArgs E)
        {
            _Editor.SaveAs();
        }

        private void SaveAll(object Sender, RoutedEventArgs E)
        {
            foreach (Tab Tab in _Tabs)
            {
                Tab.SaveFile();
            }
        }
        private void CloseAll(object Sender, RoutedEventArgs E)
        {
            foreach (Tab Tab in _Tabs)
            {
                CloseTab(Tab);
            }
        }

        private void Reload(object Sender, RoutedEventArgs E)
        {
            Tab SelectedTab = _SelectedTab;

            if (SelectedTab._CurrentFile is not null || !File.Exists(SelectedTab._CurrentFile))
            {
                Log("Нельзя перезагрузить не сохранённый файл");
                return;
            }

            SelectedTab.ReadFile();
            TextEditor.Document = SelectedTab._Document;
        }
        private void Print(object Sender, RoutedEventArgs E)
        {
            PrintDialog Dialog = new PrintDialog();

            if (Dialog.ShowDialog() == true)
            {
                try
                {
                    IDocumentPaginatorSource Document = TextEditor.Document;
                    Dialog.PrintDocument(Document.DocumentPaginator, "Печать документа");
                }
                catch
                {
                    Log($"Ошибка печати");
                }
            }
        }
        private void Send(object Sender, RoutedEventArgs E)
        {
            _SideBar.OpenMenu(SideBarType.Sending);
        }

        private void Exit(object Sender, RoutedEventArgs E)
        {
            Exit();
        }
        private void Exit()
        {
            ObservableCollection<Tab> Tabs = _Tabs;

            if (Tabs.Count != 0)
            {
                int LastCount = 0;

                while (Tabs.Count >= 0)
                {
                    LastCount = _Tabs.Count;
                    CloseTab(Tabs[Tabs.Count - 1]);

                    if (Tabs.Count == LastCount)
                    {
                        return;
                    }
                }
            }

            Environment.Exit(0);
        }


        public void Open(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                return;
            }

            Tab NewTab = Tab.GetEditor(Path.GetExtension(FilePath));
            NewTab._CurrentFile = FilePath;
            NewTab.ReadFile();

            AddTab(NewTab);
        }

        #endregion

        #region Edit
        private void ToLower(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();
            Range.Text = Range.Text.ToLower();
        }
        private void ToUpper(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();
            Range.Text = Range.Text.ToUpper();
        }
        private void Capitalize(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();
            Range.Text = Range.Text.Capitalize();
        }

        private void DuplicateLine(object Sender, RoutedEventArgs E)
        {
            string Line = GetLine(out TextPointer EndOfLine).Text;

            if (Line is null)
            {
                return;
            }

            TextEditor.CaretPosition = EndOfLine;

            AppendText(_NewLine);
            AppendText(Line);
        }
        private void RemoveEmptyLines(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();
            Range.Text = RemoveEmptyLines(Range.Text);
        }

        private void Find(object Sender, RoutedEventArgs E)
        {
            _SideBar.OpenMenu(SideBarType.Searching);
        }
        private void Replace(object Sender, RoutedEventArgs E)
        {
            _SideBar.OpenMenu(SideBarType.Replacing);
        }
        //private void GoTo(object Sender, RoutedEventArgs E)
        //{
        //    _SideBar.OpenMenu(SideBarType.GoTo);
        //}

        #endregion

        #region Inserts
        private void InsertTimeDate(object Sender, RoutedEventArgs E)
        {
            InsertText(DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy"));
        }
        private void InsertDate(object Sender, RoutedEventArgs E)
        {
            InsertText(DateTime.Now.ToString("dd.MM.yyyy"));
        }
        private void InsertTime(object Sender, RoutedEventArgs E)
        {
            InsertText(DateTime.Now.ToString("HH:mm:ss"));
        }

        private void InsertCalendar(object Sender, RoutedEventArgs E)
        {
            DateTime Now = DateTime.Now;
            MonthInfo Month = DateTimeExtension.GetMonthInfo(Now.Month, true);

            int Day = 1;
            int DayOfWeek = (int)Now.DayOfWeek - 1;
            int CurrentDay = Now.Day;

            int FirstWeekDayOfMonth = (DayOfWeek - (CurrentDay - 1) % 7 + 7) % 7;

            const string Space = "   ";

            StringBuilder Builder = new StringBuilder();

            Builder.AppendLine($"{Month.Name} {Now.Year}");
            Builder.AppendLine("Mo Tu We Th Fr Sa Su");

            while (DayOfWeek < FirstWeekDayOfMonth)
            {
                Builder.Append(Space);
                DayOfWeek++;
            }

            while (Day <= Month.DaysCount)
            {
                string DayString = Day.ToString();
                if (DayString.Length == 1)
                {
                    DayString = ' ' + DayString;
                }

                Day++;
                DayOfWeek++;

                if (DayOfWeek == 0 || DayOfWeek == 7)
                {
                    DayOfWeek = 0;
                    Builder.AppendLine(DayString.PadRight(2));
                }
                else
                {
                    Builder.Append(DayString.PadRight(3));
                }
            }

            InsertText(Builder.ToString());
        }

        private void InsertEmail(object Sender, RoutedEventArgs E)
        {
            string Email = _Settings._Email;
            InsertText(Email.Length == 0 ? "{Укажите почту в настройках}" : Email);
        }

        #endregion

        #region Punctuation
        private void Trim(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                Log("Пустой текст");
            }
            else
            {
                Range.Text = Trim(Range.Text);
            }
        }

        private void CheckBrackets(object Sender, RoutedEventArgs E)
        {
            string Text = GetRange().Text;

            if (string.IsNullOrWhiteSpace(Text))
            {
                Log("Пустой текст");
            }
            else
            {
                Log
                (
                    $"Скобки расставлены {(Text.CheckBrackets() ? string.Empty : "не")} верно"
                );
            }

        }
        private void CorrectSpaces(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                Log("Пустой текст");
            }
            else
            {
                Range.Text = CorrectSpaces(Range.Text);
            }
        }
        private void CorrectPunctuation(object Sender, RoutedEventArgs E)
        {
            TextRange Range = GetRange();

            if (Range.IsEmpty)
            {
                Log("Пустой текст");
            }
            else
            {
                Range.Text = CorrectPunctuation(Range.Text);
            }
        }

        #endregion

        #region Structuring
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

            if (Text.CountOf('\n') > Result.CountOf('\n'))
            {
                LogError("Ошибка преобразования структуры");
                LogError($"Ошибка преобразования структуры : {Text.CountOf('\n')}/{Result.CountOf('\n')}");
                return;
            }

            TextEditor.Selection.Text = Result;
        }
        private void FromStructure(object Sender, RoutedEventArgs E)
        {
            if (TextEditor.Selection.IsEmpty)
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

            if (Text.CountOf('\n') != Result.CountOf('\n'))
            {
                Log("Ошибка преобразования структуры");
                return;
            }

            TextEditor.Selection.Text = Result;
        }

        #endregion

        #region Clipboard
        private void Delete(object Sender, RoutedEventArgs E)
        {
            Dispatcher.BeginInvoke
            (
                () =>
                {
                    if (TextEditor.Selection.IsEmpty)
                    {
                        TextRange Range = new TextRange(TextEditor.CaretPosition, TextEditor.CaretPosition);
                        Range.Text = string.Empty;
                        TextEditor.CaretPosition = Range.End;
                    }
                    TextEditor.Focus();
                },
                DispatcherPriority.Input
            );
        }

        #endregion

        #region View
        private void ZoomIn(object Sender, RoutedEventArgs E) => AddZoom(1);
        private void ZoomOut(object Sender, RoutedEventArgs E) => AddZoom(-1);

        private void ZoomInX5(object Sender, RoutedEventArgs E) => AddZoom(5);
        private void ZoomOutX5(object Sender, RoutedEventArgs E) => AddZoom(-5);

        private void SetDefaultZoom(object Sender, RoutedEventArgs E)
        {
            TextEditor.FontSize = _Settings._FontSize;
        }

        private void AddZoom(int Zoom)
        {
            int CurrentZoom = (int)TextEditor.FontSize;
            Zoom = Math.Clamp(CurrentZoom + Zoom, 10, 45);

            if (CurrentZoom != Zoom)
            {
                TextEditor.FontSize = Zoom;
            }
        }

        private void OpenSettingsMenu(object Sender, RoutedEventArgs E)
            => _SideBar.OpenMenu(SideBarType.Settings);
        private void OpenLoggingMenu(object Sender, RoutedEventArgs E)
            => _SideBar.OpenMenu(SideBarType.Logging);

        #endregion

        #region SideBar
        private void CloseSideBar(object Sender, RoutedEventArgs E)
            => _SideBar.OpenMenu(SideBarType.None);

        private void CancelButtonClick(object Sender, RoutedEventArgs E)
            => _SideBar.Cancel();

        private void ApplyButtonClick(object Sender, RoutedEventArgs E)
            => _SideBar.Apply();

        #endregion


        #region TextFunctions
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

        #endregion


        #region Interaction
        private void InsertText(string Text)
        {
            Dispatcher.BeginInvoke
            (
                () =>
                {
                    if (TextEditor.Selection.IsEmpty)
                    {
                        TextRange Range = new TextRange(TextEditor.CaretPosition, TextEditor.CaretPosition);
                        Range.Text = Text;
                        TextEditor.CaretPosition = Range.End;
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
                new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd) :
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

        #endregion
    }
}