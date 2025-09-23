using Ion.Tabs;
using System.Text;
using System.Windows;
using Zion;

namespace Ion
{
    public partial class MainWindow : Window
    {
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
            DateTime FirstDay = new DateTime(Now.Year, Now.Month, 1);
            MonthInfo Month = DateTimeExtension.GetMonthInfo(Now.Month, true);

            int DayOfWeek = (int)FirstDay.DayOfWeek;
            DayOfWeek = DayOfWeek == 0 ? 6 : DayOfWeek - 1;

            StringBuilder Builder = new StringBuilder();

            Builder.AppendLine($"{Month.Name} {Now.Year}");
            Builder.AppendLine("Mo Tu We Th Fr Sa Su");

            for (int i = 0; i < DayOfWeek; i++)
            {
                Builder.Append("   ");
            }

            for (int Day = 1; Day <= Month.DaysCount; Day++)
            {
                Builder.Append(Day.ToString().PadLeft(2));

                if ((DayOfWeek + Day) % 7 == 0)
                {
                    Builder.AppendLine();
                }
                else
                {
                    Builder.Append(' ');
                }
            }

            Builder.Remove(Builder.Length - 1, 1);

            InsertText(Builder.ToString());
        }

        private void InsertEmail(object Sender, RoutedEventArgs E)
        {
            string Email = _Settings._Email;
            InsertText(Email.Length == 0 ? "{Укажите почту в настройках}" : Email);
        }

        private void SpecialChars(object Sender, RoutedEventArgs E)
        {
            string Text =
@"─
━
╌
╍
┄
┅
┈
┉
│┃╎╏┆┇┊┋
┌┐┍┑┎┒┏┓╭╮
└┘┕┙┖┚┗┛╰╯
├ ┤┝ ┥┞ ┦┟ ┧
┼ ┽ ┾ ┿ ╂ ╋
╱ ╲ ╳
◈ ◆ ◇ ■ □ ▣ ▤ ▥ ▦ ● ○ ◎ ★ ☆
✔ ✕ ✖ ✗ ✘
← → ↑ ↓
± √ ∛ ∜";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Special chars"
            };
            AddTab(Tab);
        }
    }
}