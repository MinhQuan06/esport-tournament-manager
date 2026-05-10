using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EsportManagement.DTO;

namespace EsportManagement.DAL
{
    public class RankingDAL
    {
        /// <summary>
        /// Gọi SP_GetTournamentRanking để lấy BXH chi tiết.
        /// </summary>
        public List<Ranking> GetByTournament(int tournamentId)
        {
            var dt = DBHelper.ExecuteDataTable(
                "dbo.SP_GetTournamentRanking", CommandType.StoredProcedure,
                new SqlParameter("@TournamentID", tournamentId));
            var list = new List<Ranking>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Ranking
                {
                    Rank       = (int)r["Rank"],
                    TeamID     = (int)r["TeamID"],
                    TeamName   = r["TeamName"].ToString(),
                    Wins       = (int)r["Wins"],
                    Draws      = (int)r["Draws"],
                    Losses     = (int)r["Losses"],
                    Points     = (int)r["Points"],
                    GoalDiff   = (int)r["GoalDiff"]
                });
            }
            return list;
        }

        public void Recalculate(int tournamentId)
        {
            DBHelper.ExecuteNonQuery("dbo.SP_RecalcRanking", CommandType.StoredProcedure,
                new SqlParameter("@TournamentID", tournamentId));
        }
    }
}
