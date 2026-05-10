using System;

namespace EsportManagement.DTO
{
    public class Team
    {
        public int TeamID { get; set; }
        public int TournamentID { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }
        public int? ManagerAccountID { get; set; }
        public string ManagerName { get; set; }            // Mới: tên người quản lý dạng text
        public DateTime CreatedAt { get; set; }

        public string ShortName { get; set; }
        public string LogoColor { get; set; }
        public string GameType { get; set; }
        public bool IsActive { get; set; }

        public string TournamentName { get; set; }
        public int PlayerCount { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Rank { get; set; }
    }
}
