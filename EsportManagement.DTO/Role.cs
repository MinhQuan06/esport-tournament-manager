namespace EsportManagement.DTO
{
    /// <summary>
    /// Lớp 3.2 - Role: Vai trò trong hệ thống (Admin/TeamManager/Viewer).
    /// </summary>
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
