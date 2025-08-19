using BCMEditor.Extensions;
using System.Windows.Controls;

namespace BCMEditor.SideBar
{
    public sealed class SendingMenu : SideBarMenu
    {
        private readonly MainWindow _Window;
        private readonly RichTextBox _TextField;

        public override string _Header => "Отправить";
        public override string _CancelButtonText => "Отмена";
        public override string _ApplyButtonText => "Отправить";

        public SendingMenu(MainWindow Window) : base(Window.SendingMenu)
        {
            _Window = Window;
            _TextField = Window.TextEditor;
        }


        public override void Apply()
        {
            string Text =
            (
                (_Window.SendSelection.IsChecked ?? false && !_TextField.Selection.IsEmpty) ?
                _TextField.Selection : _TextField.GetAllText()
            ).Text;

            //Email.Send
            //(
            //    _Window._Settings,
            //    _Window.MessageHeader.Text,
            //    Text,
            //);
        }
    }
}