using System;

namespace EsportManagement.DTO
{
    /// <summary>
    /// Lớp 3.10 - MatchResult: Kết quả trận đấu. WinnerTeamID NULL = Hòa.
    /// IsConfirmed = true => khóa, không sửa được (BR-RESULT-04).
    /// </summary>
    public class MatchResult
    {
        public int ResultID { get; set; }
        public int MatchID { get; set; }
        public int? WinnerTeamID { get; set; }
        public int ScoreTeam1 { get; set; }
        public int ScoreTeam2 { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
