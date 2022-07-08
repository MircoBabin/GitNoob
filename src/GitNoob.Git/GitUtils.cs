using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace GitNoob.Git
{
    public class GitUtils
    {
        public static string GenerateRandomSha1()
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] randombytes = new byte[48];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(randombytes);
                }

                //Hexadecimal output
                return BitConverter.ToString(sha1.ComputeHash(randombytes)).Replace("-", string.Empty);
            }
        }

        public static string EncodeUtf8Base64(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static string DecodeUtf8Base64(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            var bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string FormatDateTimeForGit(DateTime time)
        {
            return time.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK", CultureInfo.InvariantCulture); //iso 8601 without fraction of seconds
        }

        public static GitBranch CreateTemporaryBranchAndCheckout(GitWorkingDirectory GitWorkingDirectory, string branchFromBranchNameOrCommitId)
        {
            string tempbranchname = "gitnoob-tempbranch-" + GenerateRandomSha1();

            var create = new Command.Branch.CreateBranch(GitWorkingDirectory, tempbranchname, branchFromBranchNameOrCommitId, true);
            create.WaitFor();

            var tempbranch = new Command.Branch.GetCurrentBranch(GitWorkingDirectory);
            tempbranch.WaitFor();
            if (tempbranch.shortname != tempbranchname)
            {
                return null;
            }

            return tempbranch.branch;
        }

        public static string DateTimeToHumanString(DateTime? input)
        {
            if (input == null || !input.HasValue)
            {
                return string.Empty;
            }

            return DateTimeToHumanDateString(input) + " at " + DateTimeToHumanTimeString(input) + "h";
        }

        public static string DateTimeToHumanDateString(DateTime? input)
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

        public static string DateTimeToHumanTimeString(DateTime? input)
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
