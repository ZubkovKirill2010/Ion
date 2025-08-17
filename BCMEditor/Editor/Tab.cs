using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BCMEditor.Tabs
{
    public abstract class Tab : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly string _ExtensionFilter;
        public static readonly ReadOnlyDictionary<string, Func<Tab>> _Editors = new
        (
            new Dictionary<string, Func<Tab>>()
            {
                { ".txt", () => new TxtTab() },
                { ".bcm", () => new BCMTab() },
                //{ ".cs", () => new CSharpTab() }
            }
        );

        public static TextEditor _TextEditor;
        public FlowDocument _Document { get; protected set; }

        private string? _CurrentFileField;
        public string? _CurrentFile
        {
            get => _CurrentFileField;
            set
            {
                _CurrentFileField = value;
                UpdateHeader();
            }
        }

        private bool _IsSavedField;
        public virtual bool _IsSaved
        {
            get => _IsSavedField;
            set
            {
                _IsSavedField = value;
                _SavedIndicator = value ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private Visibility _SavedIndicatorField;
        public Visibility _SavedIndicator
        {
            get => _SavedIndicatorField;
            set
            {
                _SavedIndicatorField = value;
                OnPropertyChanged();
            }
        }

        private string _HeaderField = "Без имени";
        public string _Header
        {
            get => _HeaderField;
            set
            {
                _HeaderField = value;
                OnPropertyChanged(nameof(_Header));
            }
        }

        public Tab() => _Document = new FlowDocument();
        public Tab(string Text) => _Document = TextToDocument(Text);
        public Tab(FlowDocument Document) => _Document = Document;

        ~Tab()
        {
            //#reserve saving
        }

        public static void Initialize(MainWindow Window)
        {
            if (_TextEditor is null)
            {
                _TextEditor = Window._Editor;
            }   
        }


        public void SaveFile()
        {
            if (_CurrentFile is null)
            {
                _CurrentFile = GetFilePath();

                if (_CurrentFile is not null)
                {
                    Save();
                }               
            }
        }

        protected virtual void Save()
        {
            MainWindow.Log($"Сохранение файла \"{_CurrentFile}\"");

            Task.Run(async () =>
            {
                try
                {
                    await File.WriteAllTextAsync(_CurrentFile, GetText());
                    _IsSaved = true;
                    MainWindow.Log($"Файл \"{_CurrentFile}\" сохранён");
                }
                catch (Exception Exception)
                {
                    MainWindow.Log($"Ошибка при сохранении \"{_CurrentFile}\"");
                    MainWindow.LogError(Exception);
                }
            });
        }
        public virtual void SaveAs()
        {
            Task.Run(async () =>
            {
                string? FilePath = GetFilePath();
                if (FilePath is not null)
                {
                    await File.WriteAllTextAsync(FilePath, GetText());
                }
            });
        }


        public abstract void ReadFile();


        public virtual void TextChanged(object Sender, TextChangedEventArgs E) { }


        protected static string? GetFilePath(string Filter, string DefaultExtension)
        {
            SaveFileDialog Dialog = new SaveFileDialog
            {
                Title = "Сохранить файл",
                Filter = Filter,
                DefaultExt = DefaultExtension,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (Dialog.ShowDialog() == true)
            {
                return Dialog.FileName;
            }
            return null;
        }
        protected virtual string? GetFilePath()
        {
            SaveFileDialog Dialog = new SaveFileDialog
            {
                Title = "Сохранить файл",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (Dialog.ShowDialog() == true)
            {
                return Dialog.FileName;
            }
            return null;
        }


        public bool IsEmpty()
            => string.IsNullOrWhiteSpace(new TextRange(_Document.ContentStart, _Document.ContentEnd).Text);

        public bool IsSaved()
            => _CurrentFile is not null && File.Exists(_CurrentFile);


        protected string GetText()
        {
            return new TextRange
            (
                _TextEditor._TextField.Document.ContentStart,
                _TextEditor._TextField.Document.ContentEnd
            ).Text;
        }

        protected void SetDocument(string Text)
        {
            _Document = TextToDocument(Text);
        }

        protected static void Log(object? Message) => MainWindow.Log(Message);


        public static Tab GetEditor(string Extension)
        {
            return _Editors.TryGetValue(Extension, out Func<Tab>? Value) ? Value() : new TxtTab();
        }

        public static FlowDocument TextToDocument(string Text)
        {
            FlowDocument Document = new FlowDocument();
            Paragraph Paragraph = new Paragraph(new Run(Text));

            Document.Blocks.Add(Paragraph);

            return Document;
        }


        private void UpdateHeader()
        {
            _Header = Path.GetFileName(_CurrentFile) ?? "Без имени";
        }

        protected void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}