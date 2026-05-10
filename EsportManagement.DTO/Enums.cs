namespace EsportManagement.DTO
{
    /// <summary>
    /// Trạng thái giải đấu (Section 4.1 - TournamentStatus).
    /// </summary>
    public enum TournamentStatus
    {
        ChuaBatDau,
        DangDienRa,
        DaKetThuc
    }

    /// <summary>
    /// Trạng thái trận đấu (Section 4.2 - MatchStatus).
    /// </summary>
    public enum MatchStatus
    {
        ChuaDienRa,
        DangDienRa,
        DaKetThuc
    }

    /// <summary>
    /// Helper chuyển đổi enum &lt;-&gt; string lưu trong DB (CK_Tour_Status, CK_Match_Status).
    /// </summary>
    public static class StatusHelper
    {
        public static string ToDb(TournamentStatus s)
        {
            switch (s)
            {
                case TournamentStatus.ChuaBatDau: return "Chưa bắt đầu";
                case TournamentStatus.DangDienRa: return "Đang diễn ra";
                case TournamentStatus.DaKetThuc:  return "Đã kết thúc";
                default: return "Chưa bắt đầu";
            }
        }

        public static TournamentStatus TournamentFromDb(string s)
        {
            if (s == "Đang diễn ra") return TournamentStatus.DangDienRa;
            if (s == "Đã kết thúc")  return TournamentStatus.DaKetThuc;
            return TournamentStatus.ChuaBatDau;
        }

        public static string ToDb(MatchStatus s)
        {
            switch (s)
            {
                case MatchStatus.ChuaDienRa: return "Chưa diễn ra";
                case MatchStatus.DangDienRa: return "Đang diễn ra";
                case MatchStatus.DaKetThuc:  return "Đã kết thúc";
                default: return "Chưa diễn ra";
            }
        }

        public static MatchStatus MatchFromDb(string s)
        {
            if (s == "Đang diễn ra") return MatchStatus.DangDienRa;
            if (s == "Đã kết thúc")  return MatchStatus.DaKetThuc;
            return MatchStatus.ChuaDienRa;
        }
    }
}
