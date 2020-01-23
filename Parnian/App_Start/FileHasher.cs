using System;
using System.IO;
using System.Security.Cryptography;

namespace Parnian
{
    public static class FileHasher
    {
        public static string CalculateMD5(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream))
                        .Replace("-", "")
                        .ToLowerInvariant();
                }
            }
        }
    }
}
