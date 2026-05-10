using System;

namespace EsportManagement.DTO
{
    /// <summary>
    /// Lớp 3.11 - Ranking: Bảng xếp hạng tự động cập nhật theo MatchResult.
    /// Quy tắc: Win=3đ, Draw=1đ, Loss=0đ. Tie-break: Points -> GoalDiff -> TeamID.
    /// </summary>
    public class Ranking
    {
        public int RankingID { get; set; }
        public int TournamentID { get; set; }
        public int TeamID { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int Points { get; set; }
        public int Rank { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Trường mở rộng (load qua SP_GetTournamentRanking)
        public string TeamName { get; set; }
        public int Played => Wins + Losses + Draws;
        public int GoalDiff { get; set; }
    }
}
