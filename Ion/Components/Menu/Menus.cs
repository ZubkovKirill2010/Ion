using System.Diagnostics;

namespace Ion
{
    public sealed class Menus
    {
        public FileMenu _FileMenu { get; }               = new FileMenu();
        public EditMenu _EditMenu { get; }               = new EditMenu();
        public InsertsMenu _InsertsMenu { get; }         = new InsertsMenu();
        public PunctuationMenu _PunctuationMenu { get; } = new PunctuationMenu();
        public StructuringMenu _StructuringMenu { get; } = new StructuringMenu();
        public ClipboardMenu _ClipboardMenu { get; }     = new ClipboardMenu();
        public ViewMenu _ViewMenu { get; }               = new ViewMenu();
        public TabsMenu _TabsMenu { get; }               = new TabsMenu();
        public HelpMenu _HelpMenu { get; }               = new HelpMenu();

        public void Initialize()
        {
            Debug.WriteLine("Initialization of functions");

            _FileMenu.Initialize();
            _EditMenu.Initialize();
            _InsertsMenu.Initialize();
            _PunctuationMenu.Initialize();
            _StructuringMenu.Initialize();
            _ClipboardMenu.Initialize();
            _ViewMenu.Initialize();
            _TabsMenu.Initialize();
            _HelpMenu.Initialize();
        }
    }
}