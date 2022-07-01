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
    }
}
