using System;

namespace EsportManagement.DTO
{
    /// <summary>
    /// Bảng trung gian N:M Account &lt;-&gt; Role.
    /// </summary>
    public class AccountRole
    {
        public int AccountID { get; set; }
        public int RoleID { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
