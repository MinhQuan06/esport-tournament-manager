using System;
using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class TournamentBLL
    {
        private readonly TournamentDAL _dal = new TournamentDAL();

        public List<Tournament> GetAll() { return _dal.GetAll(); }
        public Tournament GetByID(int id) { return _dal.GetByID(id); }
        public List<Tournament> Search(string keyword, string status, string gameType)
        {
            return _dal.Search(keyword, status, gameType);
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
