using Ion.Extensions;
using System.Windows.Controls;

namespace Ion.SideBar
{
    public sealed class SendingMenu : SideBarMenu
    {
        private readonly MainWindow _Window;
        private readonly RichTextBox _TextField;

        public override string _Header => Translater._Current._Send;
        public override string _CancelButtonText => Translater._Current._Cancel;
        public override string _ApplyButtonText => Translater._Current._Send;

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
                _TextField.Selection : _TextField.GetAll()
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