using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EsportManagement.DTO;

namespace EsportManagement.DAL
{
    public class TeamDAL
    {
        public List<Team> GetByTournament(int tournamentId)
        {
            const string sql = @"
                SELECT t.TeamID, t.TournamentID, t.TeamName, t.Description,
                       t.ManagerAccountID, t.CreatedAt, t.ShortName, t.LogoColor, t.GameType, t.IsActive,
                       tour.TournamentName,
                       COALESCE(a.FullName, '') AS ManagerName,
                       (SELECT COUNT(*) FROM Player WHERE TeamID = t.TeamID) AS PlayerCount,
                       ISNULL(r.Wins, 0)   AS Wins,
                       ISNULL(r.Losses, 0) AS Losses,
                       ISNULL(r.Rank, 0)   AS Rank
                FROM Team t
                JOIN Tournament tour ON t.TournamentID = tour.TournamentID
                LEFT JOIN Account a ON t.ManagerAccountID = a.AccountID
                LEFT JOIN Ranking r ON r.TeamID = t.TeamID AND r.TournamentID = t.TournamentID
                WHERE t.TournamentID = @id
                ORDER BY t.TeamName;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@id", tournamentId)));
        }

        public List<Team> GetAll()
        {
            const string sql = @"
                SELECT t.*, tour.TournamentName,
                       COALESCE(a.FullName, '') AS ManagerName,
                       (SELECT COUNT(*) FROM Player WHERE TeamID = t.TeamID) AS PlayerCount,
                       ISNULL(r.Wins, 0)   AS Wins,
                       ISNULL(r.Losses, 0) AS Losses,
                       ISNULL(r.Rank, 0)   AS Rank
                FROM Team t
                JOIN Tournament tour ON t.TournamentID = tour.TournamentID
                LEFT JOIN Account a ON t.ManagerAccountID = a.AccountID
                LEFT JOIN Ranking r ON r.TeamID = t.TeamID AND r.TournamentID = t.TournamentID
                ORDER BY tour.TournamentName, t.TeamName;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text));
        }

        public List<Team> GetByManager(int managerAccountId)
        {
            const string sql = @"
                SELECT t.*, tour.TournamentName,
                       COALESCE(a.FullName, '') AS ManagerName,
                       (SELECT COUNT(*) FROM Player WHERE TeamID = t.TeamID) AS PlayerCount,
                       ISNULL(r.Wins, 0)   AS Wins,
                       ISNULL(r.Losses, 0) AS Losses,
                       ISNULL(r.Rank, 0)   AS Rank
                FROM Team t
                JOIN Tournament tour ON t.TournamentID = tour.TournamentID
                LEFT JOIN Account a ON t.ManagerAccountID = a.AccountID
                LEFT JOIN Ranking r ON r.TeamID = t.TeamID AND r.TournamentID = t.TournamentID
                WHERE t.ManagerAccountID = @id
                ORDER BY tour.TournamentName, t.TeamName;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@id", managerAccountId)));
        }

        public Team GetByID(int id)
        {
            const string sql = @"
                SELECT t.*, tour.TournamentName,
                       COALESCE(a.FullName, '') AS ManagerName,
                       (SELECT COUNT(*) FROM Player WHERE TeamID = t.TeamID) AS PlayerCount,
                       ISNULL(r.Wins, 0)   AS Wins,
                       ISNULL(r.Losses, 0) AS Losses,
                       ISNULL(r.Rank, 0)   AS Rank
                FROM Team t
                JOIN Tournament tour ON t.TournamentID = tour.TournamentID
                LEFT JOIN Account a ON t.ManagerAccountID = a.AccountID
                LEFT JOIN Ranking r ON r.TeamID = t.TeamID AND r.TournamentID = t.TournamentID
                WHERE t.TeamID = @id;";
            var dt = DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@id", id));
            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public int Insert(Team t)
        {
            const string sql = @"
                INSERT INTO Team (TournamentID, TeamName, Description, ManagerAccountID,
                                  ShortName, LogoColor, GameType, IsActive)
                VALUES (@tid, @n, @d, @m, @sn, @lc, @gt, @ia);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return (int)DBHelper.ExecuteScalar(sql, CommandType.Text,
                new SqlParameter("@tid", t.TournamentID),
                new SqlParameter("@n",   t.TeamName),
                new SqlParameter("@d",   (object)t.Description ?? DBNull.Value),
                new SqlParameter("@m",   (object)t.ManagerAccountID ?? DBNull.Value),
                new SqlParameter("@sn",  (object)t.ShortName ?? DBNull.Value),
                new SqlParameter("@lc",  (object)(t.LogoColor ?? "#3b82f6")),
                new SqlParameter("@gt",  (object)t.GameType ?? DBNull.Value),
                new SqlParameter("@ia",  t.IsActive));
        }

        public int Update(Team t)
        {
            const string sql = @"
                UPDATE Team SET TeamName=@n, Description=@d, ManagerAccountID=@m,
                       ShortName=@sn, LogoColor=@lc, GameType=@gt, IsActive=@ia
                WHERE TeamID=@id;";
            return DBHelper.ExecuteNonQuery(sql, CommandType.Text,
                new SqlParameter("@n",   t.TeamName),
                new SqlParameter("@d",   (object)t.Description ?? DBNull.Value),
                new SqlParameter("@m",   (object)t.ManagerAccountID ?? DBNull.Value),
                new SqlParameter("@sn",  (object)t.ShortName ?? DBNull.Value),
                new SqlParameter("@lc",  (object)(t.LogoColor ?? "#3b82f6")),
                new SqlParameter("@gt",  (object)t.GameType ?? DBNull.Value),
                new SqlParameter("@ia",  t.IsActive),
                new SqlParameter("@id",  t.TeamID));
        }

        public int Delete(int id)
        {
            var matchCount = (int)DBHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM Match WHERE Team1ID = @id OR Team2ID = @id",
                CommandType.Text, new SqlParameter("@id", id));
            if (matchCount > 0) return -1;
            DBHelper.ExecuteNonQuery("DELETE FROM Player WHERE TeamID = @id",
                CommandType.Text, new SqlParameter("@id", id));
            return DBHelper.ExecuteNonQuery("DELETE FROM Team WHERE TeamID = @id",
                CommandType.Text, new SqlParameter("@id", id));
        }

        public bool IsTeamNameDuplicate(int tournamentId, string teamName, int? excludeTeamId)
        {
            const string sql = @"
                SELECT COUNT(*) FROM Team
                WHERE TournamentID = @tid AND TeamName = @n
                  AND (@excl IS NULL OR TeamID <> @excl);";
            return (int)DBHelper.ExecuteScalar(sql, CommandType.Text,
                new SqlParameter("@tid", tournamentId),
                new SqlParameter("@n", teamName),
                new SqlParameter("@excl", (object)excludeTeamId ?? DBNull.Value)) > 0;
        }

        private static List<Team> MapList(DataTable dt)
        {
            var list = new List<Team>();
            foreach (DataRow r in dt.Rows) list.Add(MapRow(r));
            return list;
        }

        private static Team MapRow(DataRow r)
        {
            return new Team
            {
                TeamID           = (int)r["TeamID"],
                TournamentID     = (int)r["TournamentID"],
                TeamName         = r["TeamName"].ToString(),
                Description      = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
                ManagerAccountID = r["ManagerAccountID"] == DBNull.Value ? (int?)null : (int)r["ManagerAccountID"],
                CreatedAt        = (DateTime)r["CreatedAt"],
                ShortName        = r.Table.Columns.Contains("ShortName") && r["ShortName"] != DBNull.Value ? r["ShortName"].ToString() : "",
                LogoColor        = r.Table.Columns.Contains("LogoColor") && r["LogoColor"] != DBNull.Value ? r["LogoColor"].ToString() : "#3b82f6",
                GameType         = r.Table.Columns.Contains("GameType")  && r["GameType"]  != DBNull.Value ? r["GameType"].ToString()  : "",
                IsActive         = r.Table.Columns.Contains("IsActive")  ? (bool)r["IsActive"] : true,
                TournamentName   = r.Table.Columns.Contains("TournamentName") ? r["TournamentName"].ToString() : null,
                ManagerName      = r.Table.Columns.Contains("ManagerName")    ? r["ManagerName"].ToString()    : null,
                PlayerCount      = r.Table.Columns.Contains("PlayerCount")    ? (int)r["PlayerCount"]          : 0,
                Wins             = r.Table.Columns.Contains("Wins")           ? (int)r["Wins"]                 : 0,
                Losses           = r.Table.Columns.Contains("Losses")         ? (int)r["Losses"]               : 0,
                Rank             = r.Table.Columns.Contains("Rank")           ? (int)r["Rank"]                 : 0
            };
        }
    }
}
