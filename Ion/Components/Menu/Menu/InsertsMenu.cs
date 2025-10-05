using System.Text;
using System.Windows;
using System.Windows.Input;
using Zion;

namespace Ion
{
    public sealed class InsertsMenu : Menu
    {
        public override void Initialize()
        {
            AddKey(InsertTimeDate, Key.F5, ModifierKeys.None);
            AddKey(InsertEmail, Key.E, ModifierKeys.Control, true);

            AddKey(InsertDate);
            AddKey(InsertTime);
            AddKey(InsertCalendar);
            AddKey(SpecialChars);
        }


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
            string Email = _Hub._Settings._Email;
            InsertText(Email.Length == 0 ? "{Укажите почту в настройках}" : Email);
        }

        private void SpecialChars(object Sender, RoutedEventArgs E)
        {
            string Text =
@"";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Special chars"
            };
            _Hub._TabManager.AddTab(Tab);
        }
    }
}