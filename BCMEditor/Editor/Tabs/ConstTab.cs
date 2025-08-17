using System.Windows;
using System.Windows.Documents;

namespace BCMEditor.Tabs
{
    public sealed class ConstTab : Tab
    {
        public override bool _IsSaved => true;

        public ConstTab(string Text) : base(Text)
        {
            _SavedIndicator = Visibility.Hidden;
        }
        public ConstTab(FlowDocument Document) : base(Document)
        {
            _SavedIndicator = Visibility.Hidden;
        }

        public override void ReadFile() { }
    }
}