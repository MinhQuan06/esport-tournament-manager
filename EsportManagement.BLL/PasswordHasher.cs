using System.Security.Cryptography;
using System.Text;

namespace EsportManagement.BLL
{
    /// <summary>
    /// Hash mật khẩu một chiều bằng SHA256 (BR-ACC-03).
    /// Lưu ý: production nên dùng PBKDF2/BCrypt + salt.
    /// </summary>
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public static bool Verify(string password, string hash)
        {
            return Hash(password) == hash;
        }
    }
}
