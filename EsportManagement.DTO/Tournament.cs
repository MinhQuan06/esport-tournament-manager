using System;

namespace EsportManagement.DTO
{
    /// <summary>
    /// Lớp 3.6 - Tournament: Lớp trung tâm, lưu trữ thông tin giải đấu.
    /// </summary>
    public class Tournament
    {
        public int TournamentID { get; set; }
        public string TournamentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public TournamentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Bổ sung từ UI mockup
        public string GameType { get; set; }      // LMHT/CSGO/Valorant/Dota2/...
        public string Format { get; set; }        // Round Robin / Single Elim / Double Elim / Group Stage / Battle Royale

        // Trường thống kê (không có trong DB - load qua JOIN)
        public int TeamCount { get; set; }
        public int MatchCount { get; set; }

        public string StatusText { get { return StatusHelper.ToDb(Status); } }
    }
}
