using System;

namespace EsportManagement.DTO
{
    /// <summary>
    /// Lớp 3.8 - Player: Người chơi thuộc 1 đội. Tối đa 10 player/đội (BR-PLAYER-02).
    /// </summary>
    public class Player
    {
        public int PlayerID { get; set; }
        public int TeamID { get; set; }
        public string PlayerName { get; set; }
        public string ContactInfo { get; set; }
        public DateTime CreatedAt { get; set; }

        // Bổ sung từ UI mockup
        public string Nickname { get; set; }      // Faker, GAM_SofM...
        public string Position { get; set; }      // Mid, Jungle, Top, ADC, Support, Duelist, Sentinel...
        public string Country { get; set; }       // Việt Nam
        public DateTime? BirthDate { get; set; }
        public bool IsActive { get; set; }

        // Mở rộng
        public string TeamName { get; set; }
        public string TeamShortName { get; set; }
        public string TeamLogoColor { get; set; }
    }
}
