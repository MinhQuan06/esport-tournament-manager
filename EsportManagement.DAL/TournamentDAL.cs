using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EsportManagement.DTO;

namespace EsportManagement.DAL
{
    public class TournamentDAL
    {
        public List<Tournament> GetAll()
        {
            const string sql = @"
                SELECT t.TournamentID, t.TournamentName, t.StartDate, t.EndDate,
                       t.Description, t.Status, t.CreatedAt, t.GameType, t.Format,
                       (SELECT COUNT(*) FROM Team  WHERE TournamentID = t.TournamentID) AS TeamCount,
                       (SELECT COUNT(*) FROM Match WHERE TournamentID = t.TournamentID) AS MatchCount
                FROM Tournament t
                ORDER BY t.StartDate DESC;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text));
        }

        public Tournament GetByID(int id)
        {
            var dt = DBHelper.ExecuteDataTable(
                "SELECT * FROM Tournament WHERE TournamentID = @id",
                CommandType.Text, new SqlParameter("@id", id));
            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public int Insert(Tournament t)
        {
            const string sql = @"
                INSERT INTO Tournament (TournamentName, StartDate, EndDate, Description, Status, GameType, Format)
                VALUES (@n, @s, @e, @d, @st, @g, @f);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return (int)DBHelper.ExecuteScalar(sql, CommandType.Text,
                new SqlParameter("@n", t.TournamentName),
                new SqlParameter("@s", t.StartDate),
                new SqlParameter("@e", t.EndDate),
                new SqlParameter("@d", (object)t.Description ?? DBNull.Value),
                new SqlParameter("@st", StatusHelper.ToDb(t.Status)),
                new SqlParameter("@g", (object)t.GameType ?? DBNull.Value),
                new SqlParameter("@f", (object)t.Format ?? DBNull.Value));
        }

        public int Update(Tournament t)
        {
            const string sql = @"
                UPDATE Tournament SET TournamentName=@n, StartDate=@s, EndDate=@e,
                       Description=@d, Status=@st, GameType=@g, Format=@f
                 WHERE TournamentID=@id;";
            return DBHelper.ExecuteNonQuery(sql, CommandType.Text,
                new SqlParameter("@n", t.TournamentName),
                new SqlParameter("@s", t.StartDate),
                new SqlParameter("@e", t.EndDate),
                new SqlParameter("@d", (object)t.Description ?? DBNull.Value),
                new SqlParameter("@st", StatusHelper.ToDb(t.Status)),
                new SqlParameter("@g", (object)t.GameType ?? DBNull.Value),
                new SqlParameter("@f", (object)t.Format ?? DBNull.Value),
                new SqlParameter("@id", t.TournamentID));
        }

        public int UpdateStatus(int id, TournamentStatus status)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Tournament SET Status = @st WHERE TournamentID = @id",
                CommandType.Text,
                new SqlParameter("@st", StatusHelper.ToDb(status)),
                new SqlParameter("@id", id));
        }

        public int Delete(int id)
        {
            var teams = (int)DBHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Team WHERE TournamentID = @id",
                CommandType.Text, new SqlParameter("@id", id));
            if (teams > 0) return -1;
            return DBHelper.ExecuteNonQuery(
                "DELETE FROM Tournament WHERE TournamentID = @id", CommandType.Text,
                new SqlParameter("@id", id));
        }

        public List<Tournament> Search(string keyword, string status, string gameType)
        {
            const string sql = @"
                SELECT t.*,
                       (SELECT COUNT(*) FROM Team  WHERE TournamentID = t.TournamentID) AS TeamCount,
                       (SELECT COUNT(*) FROM Match WHERE TournamentID = t.TournamentID) AS MatchCount
                FROM Tournament t
                WHERE (@kw IS NULL OR t.TournamentName LIKE '%' + @kw + '%')
                  AND (@st IS NULL OR t.Status = @st)
                  AND (@gt IS NULL OR t.GameType = @gt)
                ORDER BY t.StartDate DESC;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@kw", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : keyword),
                new SqlParameter("@st", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status),
                new SqlParameter("@gt", string.IsNullOrEmpty(gameType) ? (object)DBNull.Value : gameType)));
        }

        private static List<Tournament> MapList(DataTable dt)
        {
            var list = new List<Tournament>();
            foreach (DataRow r in dt.Rows) list.Add(MapRow(r));
            return list;
        }

        private static Tournament MapRow(DataRow r)
        {
            return new Tournament
            {
                TournamentID   = (int)r["TournamentID"],
                TournamentName = r["TournamentName"].ToString(),
                StartDate      = (DateTime)r["StartDate"],
                EndDate        = (DateTime)r["EndDate"],
                Description    = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
                Status         = StatusHelper.TournamentFromDb(r["Status"].ToString()),
                CreatedAt      = (DateTime)r["CreatedAt"],
                GameType       = r.Table.Columns.Contains("GameType") && r["GameType"] != DBNull.Value ? r["GameType"].ToString() : "",
                Format         = r.Table.Columns.Contains("Format")   && r["Format"]   != DBNull.Value ? r["Format"].ToString()   : "",
                TeamCount      = r.Table.Columns.Contains("TeamCount")  ? (int)r["TeamCount"]  : 0,
                MatchCount     = r.Table.Columns.Contains("MatchCount") ? (int)r["MatchCount"] : 0
            };
        }
    }
}
