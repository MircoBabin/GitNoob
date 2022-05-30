using System;
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

    }
}
