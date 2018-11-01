using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AP.Cryptography
{
    public class SHA
    {
        public static string GenerateSHA256String(string input)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        public static string ComputePasswordAndSalt(string password, string salt, HashAlgorithm hashAlgorithm)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);

            List<byte> passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordAsBytes);
            passwordWithSaltBytes.AddRange(saltBytes);

            byte[] digestBytes = hashAlgorithm.ComputeHash(passwordWithSaltBytes.ToArray());

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < digestBytes.Length; i++)
            {
                result.Append(digestBytes[i].ToString("X2"));
            }
            return result.ToString();
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}