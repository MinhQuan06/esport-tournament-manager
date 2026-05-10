using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EsportManagement.DAL
{
    /// <summary>
    /// Cung cấp kết nối SQL Server và helper truy vấn dùng chung cho toàn DAL.
    /// Connection string đọc từ Web.config / App.config với key "EsportDB".
    /// </summary>
    public static class DBHelper
    {
        public static string ConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["EsportDB"];
                if (cs == null || string.IsNullOrEmpty(cs.ConnectionString))
                    throw new InvalidOperationException(
                        "Chưa cấu hình connection string 'EsportDB' trong Web.config / App.config.");
                return cs.ConnectionString;
            }
        }

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static int ExecuteNonQuery(string sql, CommandType type, params SqlParameter[] parameters)
        {
            using (var conn = CreateConnection())
            using (var cmd = new SqlCommand(sql, conn) { CommandType = type })
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, CommandType type, params SqlParameter[] parameters)
        {
            using (var conn = CreateConnection())
            using (var cmd = new SqlCommand(sql, conn) { CommandType = type })
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static DataTable ExecuteDataTable(string sql, CommandType type, params SqlParameter[] parameters)
        {
            using (var conn = CreateConnection())
            using (var cmd = new SqlCommand(sql, conn) { CommandType = type })
            using (var adapter = new SqlDataAdapter(cmd))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        // Helper an toàn cho null khi đọc từ DataRow / SqlDataReader
        public static T GetValue<T>(object dbValue, T defaultValue = default(T))
        {
            if (dbValue == null || dbValue == DBNull.Value) return defaultValue;
            return (T)Convert.ChangeType(dbValue, typeof(T));
        }
    }
}
