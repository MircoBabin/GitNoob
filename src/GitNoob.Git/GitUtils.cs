using System;
using System.Security.Cryptography;

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
    }
}
