using System;
using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class TeamBLL
    {
        private readonly TeamDAL _dal = new TeamDAL();
        private readonly TournamentDAL _tourDal = new TournamentDAL();

        public List<Team> GetAll() => _dal.GetAll();
        public List<Team> GetByTournament(int tournamentId) => _dal.GetByTournament(tournamentId);
        public List<Team> GetByManager(int managerAccountId) => _dal.GetByManager(managerAccountId);
        public Team GetByID(int id) => _dal.GetByID(id);

        public int Create(Team t)
        {
            Validate(t);
            // BR-TEAM-02: chỉ đăng ký vào giải đang ở trạng thái "Chưa bắt đầu"
            var tour = _tourDal.GetByID(t.TournamentID);
            if (tour == null) throw new InvalidOperationException("Giải đấu không tồn tại.");
            if (tour.Status != TournamentStatus.ChuaBatDau)
                throw new InvalidOperationException(
                    "Chỉ có thể đăng ký đội vào giải đấu chưa bắt đầu. (BR-TEAM-02)");

            if (_dal.IsTeamNameDuplicate(t.TournamentID, t.TeamName, null))
                throw new InvalidOperationException("Tên đội đã tồn tại trong giải này. (BR-TEAM-01)");

            return _dal.Insert(t);
        }

        public void Update(Team t)
        {
            Validate(t);
            if (_dal.IsTeamNameDuplicate(t.TournamentID, t.TeamName, t.TeamID))
                throw new InvalidOperationException("Tên đội đã tồn tại trong giải này.");
            _dal.Update(t);
        }

        public void Delete(int id)
        {
            var n = _dal.Delete(id);
            if (n == -1)
                throw new InvalidOperationException(
                    "Đội đã có lịch thi đấu - không thể xóa. (FR-TEAM-08)");
        }

        private static void Validate(Team t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (string.IsNullOrWhiteSpace(t.TeamName))
                throw new ArgumentException("Tên đội không được trống.");
            if (t.TournamentID <= 0)
                throw new ArgumentException("Phải chọn giải đấu.");
        }
    }
}
