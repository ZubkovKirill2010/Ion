using System.IO;

namespace Ion
{
    public sealed class LoggingMenu : SideBarMenu
    {
        public override string _Header => "Логи";
        public override string _CancelButtonText => "Закрыть";
        public override string _ApplyButtonText => "Сохранить";


        public LoggingMenu(MainWindow Window) : base(Window.LoggingMenu) { }

        public override void Apply()
        {
            string FilePath = Zion.FilePath.GetUniqueFilePath
            (
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "ErrorLogs",
                ".log"
            );

            if (StatusBar._ErrorList.Count == 0)
            {
                File.Create(FilePath);
                return;
            }

            const string Seporator = "--------------->";
            using FileStream Stream = new FileStream(FilePath, FileMode.Create);
            using StreamWriter Writer = new StreamWriter(Stream);
            Writer.WriteLine(ErrorToString(0));

            for (int i = 1; i < StatusBar._ErrorList.Count; i++)
            {
                Writer.WriteLine(Seporator);
                Writer.WriteLine(ErrorToString(i));
            }
        }

        private string ErrorToString(int Index)
        {
            return ErrorToString(StatusBar._ErrorList[Index]);
        }
        private static string ErrorToString(Exception Exception)
        {
            return
$@"[Type] {Exception.GetType().FullName}
[Source] {Exception.Source}
[Message] {Exception.Message}
[Stack] {Exception.StackTrace}";
        }
    }
}