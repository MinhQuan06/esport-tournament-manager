using System;
using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class MatchBLL
    {
        private readonly MatchDAL _dal = new MatchDAL();

        public List<Match> GetByTournament(int tournamentId) => _dal.GetByTournament(tournamentId);
        public Match GetByID(int id) => _dal.GetByID(id);

        public int Create(Match m)
        {
            Validate(m);
            try
            {
                return _dal.Insert(m);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                // Trigger TR_PreventDuplicateMatch có thể RAISERROR
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public void Update(Match m)
        {
            Validate(m);
            _dal.Update(m);
        }

        public void UpdateStatus(int matchId, MatchStatus status) => _dal.UpdateStatus(matchId, status);

        public void Delete(int id)
        {
            var n = _dal.Delete(id);
            if (n == -1)
                throw new InvalidOperationException(
                    "Trận đấu đã có kết quả - không thể xóa.");
        }

        private static void Validate(Match m)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));
            if (m.Team1ID == m.Team2ID)
                throw new ArgumentException("Hai đội thi đấu phải khác nhau. (FR-MATCH-06)");
            if (m.Team1ID <= 0 || m.Team2ID <= 0)
                throw new ArgumentException("Phải chọn đủ 2 đội.");
            if (m.MatchTime < new DateTime(2000, 1, 1))
                throw new ArgumentException("Thời gian thi đấu không hợp lệ.");
        }
    }
}
