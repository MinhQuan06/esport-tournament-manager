using System;
using System.Data;
using System.Data.SqlClient;
using EsportManagement.DTO;

namespace EsportManagement.DAL
{
    public class MatchResultDAL
    {
        public MatchResult GetByMatch(int matchId)
        {
            var dt = DBHelper.ExecuteDataTable(
                "SELECT * FROM MatchResult WHERE MatchID = @id", CommandType.Text,
                new SqlParameter("@id", matchId));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new MatchResult
            {
                ResultID     = (int)r["ResultID"],
                MatchID      = (int)r["MatchID"],
                WinnerTeamID = r["WinnerTeamID"] == DBNull.Value ? (int?)null : (int)r["WinnerTeamID"],
                ScoreTeam1   = (int)r["ScoreTeam1"],
                ScoreTeam2   = (int)r["ScoreTeam2"],
                IsConfirmed  = (bool)r["IsConfirmed"],
                RecordedAt   = (DateTime)r["RecordedAt"]
            };
        }

        /// <summary>
        /// Gọi SP_EnterMatchResult - SP đã tự cập nhật Match.Status và recalc Ranking trong transaction.
        /// </summary>
        public void EnterResult(int matchId, int score1, int score2)
        {
            DBHelper.ExecuteNonQuery("dbo.SP_EnterMatchResult", CommandType.StoredProcedure,
                new SqlParameter("@MatchID",    matchId),
                new SqlParameter("@ScoreTeam1", score1),
                new SqlParameter("@ScoreTeam2", score2));
        }

        /// <summary>
        /// Gọi SP_ConfirmMatchResult - khóa kết quả + recalc Ranking lần cuối.
        /// </summary>
        public void ConfirmResult(int matchId)
        {
            DBHelper.ExecuteNonQuery("dbo.SP_ConfirmMatchResult", CommandType.StoredProcedure,
                new SqlParameter("@MatchID", matchId));
        }
    }
}
