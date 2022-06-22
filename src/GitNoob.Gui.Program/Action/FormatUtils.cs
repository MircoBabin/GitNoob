using System;

namespace GitNoob.Gui.Program.Action
{
    public static class FormatUtils
    {
        public static string DateTimeToString(DateTime? input)
        {
            if (input == null || !input.HasValue)
            {
                return string.Empty;
            }

            return DateTimeToDateString(input) + " at " + DateTimeToTimeString(input) + "h";
        }

        public static string DateTimeToDateString(DateTime? input)
        {
            if (input == null || !input.HasValue)
            {
                return string.Empty;
            }
            DateTime time = input.Value;

            string dayname;
            switch (time.DayOfWeek)
            {
                case DayOfWeek.Monday: dayname = "monday"; break;
                case DayOfWeek.Tuesday: dayname = "tuesday"; break;
                case DayOfWeek.Wednesday: dayname = "wednesday"; break;
                case DayOfWeek.Thursday: dayname = "thursday"; break;
                case DayOfWeek.Friday: dayname = "friday"; break;
                case DayOfWeek.Saturday: dayname = "saturday"; break;
                case DayOfWeek.Sunday: dayname = "sunday"; break;
                default: dayname = String.Empty; break;
            }

            string monthname;
            switch (time.Month)
            {
                case 1: monthname = "january"; break;
                case 2: monthname = "february"; break;
                case 3: monthname = "march"; break;
                case 4: monthname = "april"; break;
                case 5: monthname = "may"; break;
                case 6: monthname = "june"; break;
                case 7: monthname = "july"; break;
                case 8: monthname = "august"; break;
                case 9: monthname = "september"; break;
                case 10: monthname = "october"; break;
                case 11: monthname = "november"; break;
                case 12: monthname = "december"; break;
                default: monthname = String.Empty; break;
            }

            return dayname + " " + time.Day + " " + monthname + " " + time.Year.ToString("0000");
        }

        public static string DateTimeToTimeString(DateTime? input)
        {
            if (input == null || !input.HasValue)
            {
                return string.Empty;
            }
            DateTime time = input.Value;

            return time.Hour.ToString("00") + ":" + time.Minute.ToString("00") + ":" + time.Second.ToString("00");
        }
    }
}
