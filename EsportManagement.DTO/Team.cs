using System;

namespace EsportManagement.DTO
{
    /// <summary>
    /// Lớp 3.7 - Team: Đội tham gia giải đấu.
    /// </summary>
    public class Team
    {
        public int TeamID { get; set; }
        public int TournamentID { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }
        public int? ManagerAccountID { get; set; }
        public DateTime CreatedAt { get; set; }

        // Bổ sung từ UI mockup
        public string ShortName { get; set; }     // T1, GAM, EVS...
        public string LogoColor { get; set; }     // Hex color cho avatar tròn
        public string GameType { get; set; }
        public bool IsActive { get; set; }

        // Trường mở rộng
        public string TournamentName { get; set; }
        public string ManagerName { get; set; }
        public int PlayerCount { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Rank { get; set; }
    }
}
