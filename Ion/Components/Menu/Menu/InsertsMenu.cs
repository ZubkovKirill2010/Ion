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
@$"{'\u2500'}
{'\u2501'}
{'\u254C'}
{'\u254D'}
{'\u2504'}
{'\u2505'}
{'\u2508'}
{'\u2509'}
{'\u2502'}{'\u2503'}{'\u254E'}{'\u254F'}{'\u2506'}{'\u2507'}{'\u250A'}{'\u250B'}
{'\u250C'}{'\u2510'}{'\u250D'}{'\u2511'}{'\u250E'}{'\u2512'}{'\u250F'}{'\u2513'}{'\u256D'}{'\u256E'}
{'\u2514'}{'\u2518'}{'\u2515'}{'\u2519'}{'\u2516'}{'\u251A'}{'\u2517'}{'\u251B'}{'\u2570'}{'\u256F'}
{'\u251C'} {'\u2524'}{'\u251D'} {'\u2525'}{'\u251E'} {'\u2526'}{'\u251F'} {'\u2527'}
{'\u253C'} {'\u253D'} {'\u253E'} {'\u253F'} {'\u2542'} {'\u254B'}
{'\u2571'} {'\u2572'} {'\u2573'}
{'\u25C8'} {'\u25C6'} {'\u25C7'} {'\u25A0'} {'\u25A1'} {'\u25A3'} {'\u25A4'} {'\u25A5'} {'\u25A6'} {'\u25CF'} {'\u25CB'} {'\u25CE'} {'\u2605'} {'\u2606'}
{'\u2714'} {'\u2715'} {'\u2716'} {'\u2717'} {'\u2718'}
{'\u2190'} {'\u2192'} {'\u2191'} {'\u2193'}
{'\u00B1'} {'\u221A'} {'\u221B'} {'\u221C'}";

            ConstTab Tab = new ConstTab(Text)
            {
                _Header = "Special chars"
            };
            _Hub._TabManager.AddTab(Tab);
        }
    }
}