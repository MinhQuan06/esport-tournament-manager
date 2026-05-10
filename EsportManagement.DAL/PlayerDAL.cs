using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EsportManagement.DTO;

namespace EsportManagement.DAL
{
    public class PlayerDAL
    {
        public List<Player> GetByTeam(int teamId)
        {
            const string sql = @"
                SELECT p.*, t.TeamName, t.ShortName AS TeamShortName, t.LogoColor AS TeamLogoColor
                FROM Player p
                JOIN Team t ON p.TeamID = t.TeamID
                WHERE p.TeamID = @id
                ORDER BY p.PlayerName;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@id", teamId)));
        }

        public List<Player> GetAll()
        {
            const string sql = @"
                SELECT p.*, t.TeamName, t.ShortName AS TeamShortName, t.LogoColor AS TeamLogoColor
                FROM Player p
                JOIN Team t ON p.TeamID = t.TeamID
                ORDER BY t.TeamName, p.PlayerName;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text));
        }

        public List<Player> Search(string keyword, int? teamId, string position)
        {
            const string sql = @"
                SELECT p.*, t.TeamName, t.ShortName AS TeamShortName, t.LogoColor AS TeamLogoColor
                FROM Player p
                JOIN Team t ON p.TeamID = t.TeamID
                WHERE (@kw IS NULL OR p.PlayerName LIKE '%'+@kw+'%' OR p.Nickname LIKE '%'+@kw+'%')
                  AND (@tid IS NULL OR p.TeamID = @tid)
                  AND (@pos IS NULL OR p.Position = @pos)
                ORDER BY t.TeamName, p.PlayerName;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@kw", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : keyword),
                new SqlParameter("@tid", (object)teamId ?? DBNull.Value),
                new SqlParameter("@pos", string.IsNullOrEmpty(position) ? (object)DBNull.Value : position)));
        }

        public Player GetByID(int id)
        {
            const string sql = @"
                SELECT p.*, t.TeamName, t.ShortName AS TeamShortName, t.LogoColor AS TeamLogoColor
                FROM Player p
                JOIN Team t ON p.TeamID = t.TeamID
                WHERE p.PlayerID = @id;";
            var dt = DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@id", id));
            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public int Insert(Player p)
        {
            const string sql = @"
                INSERT INTO Player (TeamID, PlayerName, ContactInfo, Nickname, Position, Country, BirthDate, IsActive)
                VALUES (@t, @n, @c, @nk, @pos, @co, @bd, @ia);
                SELECT ISNULL((SELECT MAX(PlayerID) FROM Player WHERE TeamID = @t), 0);";
            return (int)DBHelper.ExecuteScalar(sql, CommandType.Text,
                new SqlParameter("@t", p.TeamID),
                new SqlParameter("@n", p.PlayerName),
                new SqlParameter("@c", (object)p.ContactInfo ?? DBNull.Value),
                new SqlParameter("@nk", (object)p.Nickname ?? DBNull.Value),
                new SqlParameter("@pos", (object)p.Position ?? DBNull.Value),
                new SqlParameter("@co", (object)(p.Country ?? "Việt Nam")),
                new SqlParameter("@bd", (object)p.BirthDate ?? DBNull.Value),
                new SqlParameter("@ia", p.IsActive));
        }

        public int Update(Player p)
        {
            const string sql = @"
                UPDATE Player SET PlayerName=@n, ContactInfo=@c, Nickname=@nk, Position=@pos,
                       Country=@co, BirthDate=@bd, IsActive=@ia
                WHERE PlayerID=@id;";
            return DBHelper.ExecuteNonQuery(sql, CommandType.Text,
                new SqlParameter("@n",  p.PlayerName),
                new SqlParameter("@c",  (object)p.ContactInfo ?? DBNull.Value),
                new SqlParameter("@nk", (object)p.Nickname ?? DBNull.Value),
                new SqlParameter("@pos", (object)p.Position ?? DBNull.Value),
                new SqlParameter("@co", (object)(p.Country ?? "Việt Nam")),
                new SqlParameter("@bd", (object)p.BirthDate ?? DBNull.Value),
                new SqlParameter("@ia", p.IsActive),
                new SqlParameter("@id", p.PlayerID));
        }

        public int Delete(int id)
        {
            return DBHelper.ExecuteNonQuery(
                "DELETE FROM Player WHERE PlayerID = @id", CommandType.Text,
                new SqlParameter("@id", id));
        }

        public int CountByTeam(int teamId)
        {
            return (int)DBHelper.ExecuteScalar(
                "SELECT dbo.FN_CountPlayersInTeam(@id)", CommandType.Text,
                new SqlParameter("@id", teamId));
        }

        private static List<Player> MapList(DataTable dt)
        {
            var list = new List<Player>();
            foreach (DataRow r in dt.Rows) list.Add(MapRow(r));
            return list;
        }

        private static Player MapRow(DataRow r)
        {
            return new Player
            {
                PlayerID    = (int)r["PlayerID"],
                TeamID      = (int)r["TeamID"],
                PlayerName  = r["PlayerName"].ToString(),
                ContactInfo = r["ContactInfo"] == DBNull.Value ? null : r["ContactInfo"].ToString(),
                CreatedAt   = (DateTime)r["CreatedAt"],
                Nickname    = r.Table.Columns.Contains("Nickname")  && r["Nickname"]  != DBNull.Value ? r["Nickname"].ToString()  : "",
                Position    = r.Table.Columns.Contains("Position")  && r["Position"]  != DBNull.Value ? r["Position"].ToString()  : "",
                Country     = r.Table.Columns.Contains("Country")   && r["Country"]   != DBNull.Value ? r["Country"].ToString()   : "Việt Nam",
                BirthDate   = r.Table.Columns.Contains("BirthDate") && r["BirthDate"] != DBNull.Value ? (DateTime?)(DateTime)r["BirthDate"] : null,
                IsActive    = r.Table.Columns.Contains("IsActive")  ? (bool)r["IsActive"] : true,
                TeamName    = r.Table.Columns.Contains("TeamName")  ? r["TeamName"].ToString() : null,
                TeamShortName = r.Table.Columns.Contains("TeamShortName") && r["TeamShortName"] != DBNull.Value ? r["TeamShortName"].ToString() : null,
                TeamLogoColor = r.Table.Columns.Contains("TeamLogoColor") && r["TeamLogoColor"] != DBNull.Value ? r["TeamLogoColor"].ToString() : "#3b82f6"
            };
        }
    }
}
