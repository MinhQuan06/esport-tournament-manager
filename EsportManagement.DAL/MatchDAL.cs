using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EsportManagement.DTO;

namespace EsportManagement.DAL
{
    public class MatchDAL
    {
        public List<Match> GetByTournament(int tournamentId)
        {
            const string sql = @"
                SELECT m.MatchID, m.TournamentID, m.Team1ID, m.Team2ID,
                       m.MatchTime, m.Status, m.CreatedAt, m.RoundName, m.GroupName, m.MatchFormat,
                       t1.TeamName AS Team1Name, t1.ShortName AS Team1Short, t1.LogoColor AS Team1Color,
                       t2.TeamName AS Team2Name, t2.ShortName AS Team2Short, t2.LogoColor AS Team2Color,
                       mr.ScoreTeam1, mr.ScoreTeam2, mr.WinnerTeamID, mr.IsConfirmed,
                       tw.TeamName AS WinnerName
                FROM Match m
                JOIN Team t1 ON m.Team1ID = t1.TeamID
                JOIN Team t2 ON m.Team2ID = t2.TeamID
                LEFT JOIN MatchResult mr ON m.MatchID = mr.MatchID
                LEFT JOIN Team tw ON mr.WinnerTeamID = tw.TeamID
                WHERE m.TournamentID = @id
                ORDER BY m.MatchTime ASC;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@id", tournamentId)));
        }

        public List<Match> GetUpcoming(int days)
        {
            const string sql = @"
                SELECT TOP 20 m.*,
                       t1.TeamName AS Team1Name, t1.ShortName AS Team1Short, t1.LogoColor AS Team1Color,
                       t2.TeamName AS Team2Name, t2.ShortName AS Team2Short, t2.LogoColor AS Team2Color,
                       mr.ScoreTeam1, mr.ScoreTeam2, mr.WinnerTeamID, mr.IsConfirmed,
                       tw.TeamName AS WinnerName
                FROM Match m
                JOIN Team t1 ON m.Team1ID = t1.TeamID
                JOIN Team t2 ON m.Team2ID = t2.TeamID
                LEFT JOIN MatchResult mr ON m.MatchID = mr.MatchID
                LEFT JOIN Team tw ON mr.WinnerTeamID = tw.TeamID
                WHERE m.MatchTime >= DATEADD(DAY, -@d, GETDATE())
                ORDER BY m.MatchTime ASC;";
            return MapList(DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@d", days)));
        }

        public Match GetByID(int id)
        {
            const string sql = @"
                SELECT m.*,
                       t1.TeamName AS Team1Name, t1.ShortName AS Team1Short, t1.LogoColor AS Team1Color,
                       t2.TeamName AS Team2Name, t2.ShortName AS Team2Short, t2.LogoColor AS Team2Color,
                       mr.ScoreTeam1, mr.ScoreTeam2, mr.WinnerTeamID, mr.IsConfirmed,
                       tw.TeamName AS WinnerName
                FROM Match m
                JOIN Team t1 ON m.Team1ID = t1.TeamID
                JOIN Team t2 ON m.Team2ID = t2.TeamID
                LEFT JOIN MatchResult mr ON m.MatchID = mr.MatchID
                LEFT JOIN Team tw ON mr.WinnerTeamID = tw.TeamID
                WHERE m.MatchID = @id;";
            var dt = DBHelper.ExecuteDataTable(sql, CommandType.Text,
                new SqlParameter("@id", id));
            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public int Insert(Match m)
        {
            const string sql = @"
                INSERT INTO Match (TournamentID, Team1ID, Team2ID, MatchTime, Status, RoundName, GroupName, MatchFormat)
                VALUES (@t, @a, @b, @time, @s, @rn, @gn, @mf);
                SELECT ISNULL((SELECT MAX(MatchID) FROM Match), 0);";
            return (int)DBHelper.ExecuteScalar(sql, CommandType.Text,
                new SqlParameter("@t",    m.TournamentID),
                new SqlParameter("@a",    m.Team1ID),
                new SqlParameter("@b",    m.Team2ID),
                new SqlParameter("@time", m.MatchTime),
                new SqlParameter("@s",    StatusHelper.ToDb(m.Status)),
                new SqlParameter("@rn",   (object)(m.RoundName ?? "Vòng bảng")),
                new SqlParameter("@gn",   (object)m.GroupName ?? DBNull.Value),
                new SqlParameter("@mf",   (object)(m.MatchFormat ?? "BO3")));
        }

        public int Update(Match m)
        {
            const string sql = @"
                UPDATE Match SET Team1ID=@a, Team2ID=@b, MatchTime=@time, Status=@s,
                       RoundName=@rn, GroupName=@gn, MatchFormat=@mf
                 WHERE MatchID=@id;";
            return DBHelper.ExecuteNonQuery(sql, CommandType.Text,
                new SqlParameter("@a",    m.Team1ID),
                new SqlParameter("@b",    m.Team2ID),
                new SqlParameter("@time", m.MatchTime),
                new SqlParameter("@s",    StatusHelper.ToDb(m.Status)),
                new SqlParameter("@rn",   (object)(m.RoundName ?? "Vòng bảng")),
                new SqlParameter("@gn",   (object)m.GroupName ?? DBNull.Value),
                new SqlParameter("@mf",   (object)(m.MatchFormat ?? "BO3")),
                new SqlParameter("@id",   m.MatchID));
        }

        public int UpdateStatus(int matchId, MatchStatus status)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE Match SET Status = @s WHERE MatchID = @id", CommandType.Text,
                new SqlParameter("@s",  StatusHelper.ToDb(status)),
                new SqlParameter("@id", matchId));
        }

        public int Delete(int id)
        {
            var hasResult = (int)DBHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM MatchResult WHERE MatchID = @id",
                CommandType.Text, new SqlParameter("@id", id));
            if (hasResult > 0) return -1;
            return DBHelper.ExecuteNonQuery(
                "DELETE FROM Match WHERE MatchID = @id", CommandType.Text,
                new SqlParameter("@id", id));
        }

        private static List<Match> MapList(DataTable dt)
        {
            var list = new List<Match>();
            foreach (DataRow r in dt.Rows) list.Add(MapRow(r));
            return list;
        }

        private static Match MapRow(DataRow r)
        {
            return new Match
            {
                MatchID      = (int)r["MatchID"],
                TournamentID = (int)r["TournamentID"],
                Team1ID      = (int)r["Team1ID"],
                Team2ID      = (int)r["Team2ID"],
                MatchTime    = (DateTime)r["MatchTime"],
                Status       = StatusHelper.MatchFromDb(r["Status"].ToString()),
                CreatedAt    = (DateTime)r["CreatedAt"],
                RoundName    = r.Table.Columns.Contains("RoundName")   && r["RoundName"]   != DBNull.Value ? r["RoundName"].ToString()   : "Vòng bảng",
                GroupName    = r.Table.Columns.Contains("GroupName")   && r["GroupName"]   != DBNull.Value ? r["GroupName"].ToString()   : "",
                MatchFormat  = r.Table.Columns.Contains("MatchFormat") && r["MatchFormat"] != DBNull.Value ? r["MatchFormat"].ToString() : "BO3",
                Team1Name    = r.Table.Columns.Contains("Team1Name")  ? r["Team1Name"].ToString() : null,
                Team2Name    = r.Table.Columns.Contains("Team2Name")  ? r["Team2Name"].ToString() : null,
                Team1Short   = r.Table.Columns.Contains("Team1Short") && r["Team1Short"] != DBNull.Value ? r["Team1Short"].ToString() : "",
                Team2Short   = r.Table.Columns.Contains("Team2Short") && r["Team2Short"] != DBNull.Value ? r["Team2Short"].ToString() : "",
                Team1Color   = r.Table.Columns.Contains("Team1Color") && r["Team1Color"] != DBNull.Value ? r["Team1Color"].ToString() : "#3b82f6",
                Team2Color   = r.Table.Columns.Contains("Team2Color") && r["Team2Color"] != DBNull.Value ? r["Team2Color"].ToString() : "#ef4444",
                ScoreTeam1   = r.Table.Columns.Contains("ScoreTeam1") && r["ScoreTeam1"] != DBNull.Value ? (int?)(int)r["ScoreTeam1"] : null,
                ScoreTeam2   = r.Table.Columns.Contains("ScoreTeam2") && r["ScoreTeam2"] != DBNull.Value ? (int?)(int)r["ScoreTeam2"] : null,
                WinnerTeamID = r.Table.Columns.Contains("WinnerTeamID") && r["WinnerTeamID"] != DBNull.Value ? (int?)(int)r["WinnerTeamID"] : null,
                WinnerName   = r.Table.Columns.Contains("WinnerName") && r["WinnerName"] != DBNull.Value ? r["WinnerName"].ToString() : null,
                IsConfirmed  = r.Table.Columns.Contains("IsConfirmed") && r["IsConfirmed"] != DBNull.Value ? (bool?)(bool)r["IsConfirmed"] : null
            };
        }
    }
}
