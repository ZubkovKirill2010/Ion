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

        private void InsertSeparator(object Sender, RoutedEventArgs E)
        {
            InsertText("-------------->");
        }

        private void SpecialChars(object Sender, RoutedEventArgs E)
        {
            string Text =
@"──────
━━━━━━
╌╌╌╌╌╌
╍╍╍╍╍╍
┄┄┄┄┄┄
┅┅┅┅┅┅
┈┈┈┈┈┈
┉┉┉┉┉┉

│┃╎╏┆┇┊┋
│┃╎╏┆┇┊┋

┌┐┍┑┎┒┏┓╭╮
└┘┕┙┖┚┗┛╰╯

├ ┤┝ ┥┞ ┦┟ ┧

┼ ┽ ┾ ┿ ╂ ╋

╱ ╲ ╳

◈ ◆ ◇ ■ □ ▣ ▤ ▥ ▦ ● ○ ◎ ★ ☆";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Special chars"
            };
            AddTab(Tab);
        }
    }
}