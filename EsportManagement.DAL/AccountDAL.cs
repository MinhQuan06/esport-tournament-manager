using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EsportManagement.DTO;

namespace EsportManagement.DAL
{
    public class AccountDAL
    {
        public Account Login(string username, string passwordHash)
        {
            const string sql = @"
                SELECT a.AccountID, a.Username, a.PasswordHash, a.Email, a.FullName,
                       a.IsLocked, a.CreatedAt, a.LastLoginAt
                FROM Account a
                WHERE a.Username = @u AND a.PasswordHash = @p AND a.IsLocked = 0;";
            var dt = DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@u", username),
                new SqlParameter("@p", passwordHash));
            if (dt.Rows.Count == 0) return null;
            var acc = MapRow(dt.Rows[0]);
            acc.Roles = GetRoles(acc.AccountID);
            UpdateLastLogin(acc.AccountID);
            return acc;
        }

        public void UpdateLastLogin(int accountId)
        {
            DBHelper.ExecuteNonQuery(
                "UPDATE Account SET LastLoginAt = GETDATE() WHERE AccountID = @id",
                CommandType.Text, new SqlParameter("@id", accountId));
        }

        public Account GetByID(int id)
        {
            const string sql = "SELECT * FROM Account WHERE AccountID = @id";
            var dt = DBHelper.ExecuteDataTable(sql, CommandType.Text, new SqlParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            var acc = MapRow(dt.Rows[0]);
            acc.Roles = GetRoles(id);
            return acc;
        }

        public Account GetByUsername(string username)
        {
            const string sql = "SELECT * FROM Account WHERE Username = @u";
            var dt = DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@u", username));
            if (dt.Rows.Count == 0) return null;
            var acc = MapRow(dt.Rows[0]);
            acc.Roles = GetRoles(acc.AccountID);
            return acc;
        }

        public List<Account> GetAll()
        {
            const string sql = "SELECT * FROM Account ORDER BY AccountID";
            var dt = DBHelper.ExecuteDataTable(sql, CommandType.Text);
            var list = new List<Account>();
            foreach (DataRow r in dt.Rows)
            {
                var a = MapRow(r);
                a.Roles = GetRoles(a.AccountID);
                list.Add(a);
            }
            return list;
        }

        public int Insert(Account acc)
        {
            const string sql = @"
                INSERT INTO Account (Username, PasswordHash, Email, FullName, IsLocked)
                VALUES (@u, @p, @e, @f, @lock);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return (int)DBHelper.ExecuteScalar(sql, CommandType.Text,
                new SqlParameter("@u", acc.Username),
                new SqlParameter("@p", acc.PasswordHash),
                new SqlParameter("@e", acc.Email),
                new SqlParameter("@f", acc.FullName),
                new SqlParameter("@lock", acc.IsLocked));
        }

        public int Update(Account acc)
        {
            const string sql = @"
                UPDATE Account SET Email = @e, FullName = @f, IsLocked = @lock
                WHERE AccountID = @id;";
            return DBHelper.ExecuteNonQuery(sql, CommandType.Text,
                new SqlParameter("@e", acc.Email),
                new SqlParameter("@f", acc.FullName),
                new SqlParameter("@lock", acc.IsLocked),
                new SqlParameter("@id", acc.AccountID));
        }

        public int UpdatePassword(int accountId, string newHash)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Account SET PasswordHash = @p WHERE AccountID = @id",
                CommandType.Text,
                new SqlParameter("@p", newHash),
                new SqlParameter("@id", accountId));
        }

        public int Delete(int id)
        {
            DBHelper.ExecuteNonQuery("DELETE FROM AccountRole WHERE AccountID = @id",
                CommandType.Text, new SqlParameter("@id", id));
            return DBHelper.ExecuteNonQuery("DELETE FROM Account WHERE AccountID = @id",
                CommandType.Text, new SqlParameter("@id", id));
        }

        public List<string> GetRoles(int accountId)
        {
            const string sql = @"
                SELECT r.RoleName FROM Role r
                JOIN AccountRole ar ON r.RoleID = ar.RoleID
                WHERE ar.AccountID = @id";
            var dt = DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@id", accountId));
            var list = new List<string>();
            foreach (DataRow r in dt.Rows) list.Add(r["RoleName"].ToString());
            return list;
        }

        public void AssignRole(int accountId, int roleId)
        {
            DBHelper.ExecuteNonQuery("dbo.SP_AssignRole", CommandType.StoredProcedure,
                new SqlParameter("@AccountID", accountId),
                new SqlParameter("@RoleID", roleId));
        }

        public int RemoveRole(int accountId, int roleId)
        {
            return DBHelper.ExecuteNonQuery(
                "DELETE FROM AccountRole WHERE AccountID = @a AND RoleID = @r",
                CommandType.Text,
                new SqlParameter("@a", accountId),
                new SqlParameter("@r", roleId));
        }

        public bool IsUsernameTaken(string username)
        {
            return (int)DBHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Account WHERE Username = @u",
                CommandType.Text, new SqlParameter("@u", username)) > 0;
        }

        public bool IsEmailTaken(string email)
        {
            return (int)DBHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Account WHERE Email = @e",
                CommandType.Text, new SqlParameter("@e", email)) > 0;
        }

        private static Account MapRow(DataRow r)
        {
            return new Account
            {
                AccountID    = (int)r["AccountID"],
                Username     = r["Username"].ToString(),
                PasswordHash = r["PasswordHash"].ToString(),
                Email        = r["Email"].ToString(),
                FullName     = r["FullName"].ToString(),
                IsLocked     = (bool)r["IsLocked"],
                CreatedAt    = (DateTime)r["CreatedAt"],
                LastLoginAt  = r.Table.Columns.Contains("LastLoginAt") && r["LastLoginAt"] != DBNull.Value
                                ? (DateTime?)(DateTime)r["LastLoginAt"] : null
            };
        }
    }
}
