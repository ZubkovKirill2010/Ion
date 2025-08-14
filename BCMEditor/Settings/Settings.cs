using System.IO;
using Zion;

namespace BCMEditor
{
    public sealed class Settings
    {
        private static readonly string _ProgramDirectory = Directory.CreateDirectory
        (
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BCNEditor")
        ).FullName;
        private static readonly string _SettingsPath = Path.Combine(_ProgramDirectory, "Settings.settings");

        #region Properties
        public int _DefaultFontSize = 20;
        public int _FontSize = 20;

        public int _MenuScale = 14;

        public bool _SpellCheck = false;
        public string _DefaultTabExtension = ".txt";

        public string _Email = string.Empty;
        public List<string> _Recipients = new List<string>();

        public string _EmailPassword = string.Empty;

        #endregion

        public void Save()
        {
            using (FileStream Stream = new FileStream(_SettingsPath, FileMode.Create))
            using (BinaryWriter Writer = new BinaryWriter(Stream))
            {
                Writer.Write((byte)_DefaultFontSize);
                Writer.Write((byte)_FontSize);
                Writer.Write((byte)_MenuScale);

                Writer.Write(_SpellCheck);
                Writer.Write(_DefaultTabExtension);

                Writer.Write(_Email);
                Writer.Write(_EmailPassword);

                Writer.Write(_Recipients.Count);
                foreach (string Recipient in _Recipients)
                {
                    Writer.Write(Recipient);
                }
            }
        }
        public static Settings Load()
        {
            if (!File.Exists(_SettingsPath))
            {
                Settings Default = new Settings();
                Default.Save();
                return Default;
            }
            try
            {
                using (FileStream Stream = new FileStream(_SettingsPath, FileMode.Open))
                using (BinaryReader Reader = new BinaryReader(Stream))
                {
                    return new Settings()
                    {
                        _DefaultFontSize = Reader.ReadByte(),
                        _FontSize = Reader.ReadByte(),
                        _MenuScale = Reader.ReadByte(),

                        _SpellCheck = Reader.ReadBoolean(),
                        _DefaultTabExtension = Reader.ReadString(),

                        _Email = Reader.ReadString(),
                        _EmailPassword = Reader.ReadString(),

                        _Recipients = Reader.ReadList(Reader => Reader.ReadString(), Reader.ReadInt32(), 1)
                    };
                }
            }
            catch
            {
                //#temp
                Settings Default = new Settings();
                Default.Save();
                return Default;
            }
        }
    }
}