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

        public List<Team> GetAll()
        {
            try
            {
                return _dal.GetAll();
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.Teams();
                throw;
            }
        }

        public List<Team> GetByTournament(int tournamentId)
        {
            try
            {
                return _dal.GetByTournament(tournamentId);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.TeamsByTournament(tournamentId);
                throw;
            }
        }

        public List<Team> GetByManager(int managerAccountId)
        {
            try
            {
                return _dal.GetByManager(managerAccountId);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.TeamsByManager(managerAccountId);
                throw;
            }
        }

        public List<Team> GetMyTeamsByGame(int managerAccountId, string gameType)
        {
            try
            {
                return _dal.GetMyTeamsByGame(managerAccountId, gameType);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.MyTeamsByGame(managerAccountId, gameType);
                throw;
            }
        }

        public Team GetByID(int id)
        {
            try
            {
                return _dal.GetByID(id);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.TeamById(id);
                throw;
            }
        }

        public int Create(Team t)
        {
            Validate(t);
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
                throw new InvalidOperationException("Đội đã có lịch thi đấu - không thể xóa.");
        }

        private static void Validate(Team t)
        {
            if (t == null) throw new ArgumentNullException("t");
            if (string.IsNullOrWhiteSpace(t.TeamName))
                throw new ArgumentException("Tên đội không được trống.");
            if (t.TournamentID <= 0)
                throw new ArgumentException("Phải chọn giải đấu.");
        }
    }
}
