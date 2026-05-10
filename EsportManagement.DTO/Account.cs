using System;
using System.Collections.Generic;

namespace EsportManagement.DTO
{
    /// <summary>
    /// Lớp 3.1 - Account: Trung tâm xác thực, mọi vai trò đều kế thừa từ đây.
    /// </summary>
    public class Account
    {
        public int AccountID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Roles của tài khoản (load từ AccountRole)
        public List<string> Roles { get; set; }

        public Account() { Roles = new List<string>(); }

        public bool HasRole(string roleName)
        {
            if (Roles == null) return false;
            foreach (var r in Roles)
                if (string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        public string PrimaryRole
        {
            get
            {
                if (HasRole("Admin")) return "Admin";
                if (HasRole("TeamManager")) return "Team Manager";
                return "Viewer";
            }
        }

        public string Initials
        {
            get
            {
                if (string.IsNullOrEmpty(FullName)) return "?";
                var parts = FullName.Trim().Split(' ');
                if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
                var last = parts[parts.Length - 1];
                return (parts[0].Substring(0, 1) + last.Substring(0, 1)).ToUpper();
            }
        }
    }
}
