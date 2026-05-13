using System;
using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class TournamentBLL
    {
        private readonly TournamentDAL _dal = new TournamentDAL();

        public List<Tournament> GetAll()
        {
            try
            {
                return _dal.GetAll();
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.Tournaments();
                throw;
            }
        }

        public Tournament GetByID(int id)
        {
            try
            {
                return _dal.GetByID(id);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.TournamentById(id);
                throw;
            }
        }

        public List<Tournament> Search(string keyword, string status, string gameType)
        {
            try
            {
                return _dal.Search(keyword, status, gameType);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.SearchTournaments(keyword, status, gameType);
                throw;
            }
        }

        public int Create(Tournament t)
        {
            Validate(t);
            return _dal.Insert(t);
        }

        public void Update(Tournament t)
        {
            Validate(t);
            var existing = _dal.GetByID(t.TournamentID);
            if (existing == null) throw new InvalidOperationException("Giải đấu không tồn tại.");
            _dal.Update(t);
        }

        public void UpdateStatus(int id, TournamentStatus status) { _dal.UpdateStatus(id, status); }

        public void Delete(int id)
        {
            var n = _dal.Delete(id);
            if (n == -1)
                throw new InvalidOperationException("Giải đấu đã có đội/trận - không thể xóa. (FR-TOUR-08)");
        }

        private static void Validate(Tournament t)
        {
            if (t == null) throw new ArgumentNullException("t");
            if (string.IsNullOrWhiteSpace(t.TournamentName))
                throw new ArgumentException("Tên giải đấu không được trống.");
            if (t.EndDate <= t.StartDate)
                throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu. (FR-TOUR-07)");
        }
    }
}
