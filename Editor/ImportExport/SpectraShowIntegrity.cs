using System.Security.Cryptography;
using System.Text;

namespace SpectraOverdrive.Editor
{
    public static class SpectraShowIntegrity
    {
        public static string ComputeSha256(string value)
        {
            if (value == null) value = string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            using (SHA256 sha = SHA256.Create())
            {
                byte[] digest = sha.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder(digest.Length * 2);
                for (int i = 0; i < digest.Length; i++)
                    builder.Append(digest[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
