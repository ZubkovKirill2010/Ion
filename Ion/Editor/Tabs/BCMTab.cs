//using BinaryCodeMarkup;
//using System.IO;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using Zion;

//namespace Ion.Tabs
//{
//    public sealed class BCMTab : Tab
//    {
//        private static readonly FlowDocument _EmptyMarkup =
//            TextToDocument(Code.GetEmptyMarkup());

//        public BCMTab()
//        {
//            _Document = _EmptyMarkup;
//        }
//        public BCMTab(string Text) : base(Text) { }
//        public BCMTab(FlowDocument Document) : base(Document) { }

//        #region File
//        protected override void Save()
//        {
//            MainWindow.Log($"Сохранение файла \"{_CurrentFile}\"");
//            try
//            {
//                GetCode().Save(_CurrentFile);

//                _IsSaved = true;
//                MainWindow.Log($"Файл \"{_CurrentFile}\" сохранён");
//            }
//            catch (Exception ex)
//            {
//                MainWindow.Log($"Ошибка при сохранении \"{_CurrentFile}\" : {ex}");
//            }
//        }

//        public override void SaveAs()
//        {
//            Task.Run(async () =>
//            {
//                string? FilePath = GetFilePath();
//                if (FilePath is not null)
//                {
//                    await File.WriteAllTextAsync(FilePath, GetText());
//                }
//            });
//        }


//        public override void ReadFile()
//        {
//            MainWindow.Log($"Чтение файла \"{_CurrentFile}\"");

//            try
//            {
//                Code Code = Code.Read(_CurrentFile);

//                Application.Current.Dispatcher.Invoke(() =>
//                {
//                    SetDocument(Code.ToString());
//                    _IsSaved = true;
//                });
//            }
//            catch
//            {
//                MainWindow.Log($"Ошибка при чтении файла \"{_CurrentFile}\"");
//            }
//        }

//        #endregion

//        protected override string? GetFilePath() => GetFilePath("BCM файлы (*.bcm)|*.bcm", ".bcm");

//        private Code GetCode()
//        {
//            string Text = GetText();
//            try
//            {
//                return new CodeParser(Text).Parse();
//            }
//            catch (CodeSyntaxException Exception)
//            {
//                string[] Lines = Text.Split(Environment.NewLine);
//                int LineIndex = Exception._LineIndex;

//                MainWindow.Log($"{(LineIndex.IsClamp(0, Lines.Length - 1) ? "?" : Lines[LineIndex])}. {Exception.Message}");

//                return new Code(Text);
//            }
//        }

//        #region Syntax high lighting

//        public override void TextChanged(object Sender, TextChangedEventArgs E)
//        {
//            //Region CurrentRegion = GetCurrentRegion();
//            //MainWindow.Log(CurrentRegion);

//            //switch (CurrentRegion)
//            //{
//            //    case Region.None:
//            //        return;

//            //    case Region.InputData:

//            //        break;

//            //    case Region.Inserts:

//            //        break;

//            //    case Region.Code:

//            //        break;
//            //}
//        }


//        private Region GetCurrentRegion()
//        {
//            TextPointer CaretPosition = _TextEditor._TextField.CaretPosition;

//            TextPointer CurrentLineStart = CaretPosition.GetLineStartPosition(0);
//            TextPointer CurrentPosition = CurrentLineStart;

//            while (CurrentPosition is not null)
//            {
//                TextPointer PreviewLineStart = CurrentPosition.GetLineStartPosition(-1);
//                if (PreviewLineStart is null)
//                {
//                    break;
//                }

//                TextPointer PreviewLineEnd = PreviewLineStart.GetLineStartPosition(1) ?? PreviewLineStart.DocumentEnd;

//                string LineText = new TextRange(PreviewLineStart, PreviewLineEnd).Text.Trim();

//                if (LineText.StartsWith("~"))
//                {
//                    Region Region = LineText.GetRegion();
//                    if (Region != Region.Null)
//                    {
//                        return Region;
//                    }
//                }

//                CurrentPosition = PreviewLineStart;
//            }

//            return Region.None;
//        }

//        #endregion
//    }
//}