using System;

namespace EsportManagement.DTO
{
    /// <summary>
    /// Lớp 3.9 - Match: Trận đấu giữa 2 đội. Team1ID != Team2ID, cùng tournament.
    /// </summary>
    public class Match
    {
        public int MatchID { get; set; }
        public int TournamentID { get; set; }
        public int Team1ID { get; set; }
        public int Team2ID { get; set; }
        public DateTime MatchTime { get; set; }
        public MatchStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Bổ sung
        public string RoundName { get; set; }    // "Vòng bảng", "Tứ kết", "Bán kết", "Chung kết"
        public string GroupName { get; set; }    // "A", "B"
        public string MatchFormat { get; set; }  // BO1/BO3/BO5

        // Trường mở rộng
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public string Team1Short { get; set; }
        public string Team2Short { get; set; }
        public string Team1Color { get; set; }
        public string Team2Color { get; set; }
        public string TournamentName { get; set; }
        public int? ScoreTeam1 { get; set; }
        public int? ScoreTeam2 { get; set; }
        public int? WinnerTeamID { get; set; }
        public string WinnerName { get; set; }
        public bool? IsConfirmed { get; set; }
    }
}
