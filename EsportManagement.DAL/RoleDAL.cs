using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EsportManagement.DTO;

namespace EsportManagement.DAL
{
    public class RoleDAL
    {
        public List<Role> GetAll()
        {
            var dt = DBHelper.ExecuteDataTable(
                "SELECT * FROM Role ORDER BY RoleID", CommandType.Text);
            var list = new List<Role>();
            foreach (DataRow r in dt.Rows) list.Add(MapRow(r));
            return list;
        }

        public Role GetByID(int id)
        {
            var dt = DBHelper.ExecuteDataTable(
                "SELECT * FROM Role WHERE RoleID = @id", CommandType.Text,
                new SqlParameter("@id", id));
            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public Role GetByName(string name)
        {
            var dt = DBHelper.ExecuteDataTable(
                "SELECT * FROM Role WHERE RoleName = @n", CommandType.Text,
                new SqlParameter("@n", name));
            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public int Insert(Role role)
        {
            const string sql = @"
                INSERT INTO Role (RoleName, Description) VALUES (@n, @d);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return (int)DBHelper.ExecuteScalar(sql, CommandType.Text,
                new SqlParameter("@n", role.RoleName),
                new SqlParameter("@d", (object)role.Description ?? System.DBNull.Value));
        }

        public int Update(Role role)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Role SET RoleName = @n, Description = @d WHERE RoleID = @id",
                CommandType.Text,
                new SqlParameter("@n", role.RoleName),
                new SqlParameter("@d", (object)role.Description ?? System.DBNull.Value),
                new SqlParameter("@id", role.RoleID));
        }

        public int Delete(int id)
        {
            // Kiểm tra ràng buộc (BR-ROLE-04: không xóa nếu đang được sử dụng)
            var inUse = (int)DBHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM AccountRole WHERE RoleID = @id",
                CommandType.Text, new SqlParameter("@id", id));
            if (inUse > 0) return -1;
            return DBHelper.ExecuteNonQuery(
                "DELETE FROM Role WHERE RoleID = @id", CommandType.Text,
                new SqlParameter("@id", id));
        }

        private static Role MapRow(DataRow r)
        {
            return new Role
            {
                RoleID      = (int)r["RoleID"],
                RoleName    = r["RoleName"].ToString(),
                Description = r["Description"] == System.DBNull.Value ? null : r["Description"].ToString()
            };
        }
    }
}
