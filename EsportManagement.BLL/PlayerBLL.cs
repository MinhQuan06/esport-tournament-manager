using System;
using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class PlayerBLL
    {
        private readonly PlayerDAL _dal = new PlayerDAL();

        public List<Player> GetByTeam(int teamId)
        {
            try
            {
                return _dal.GetByTeam(teamId);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.PlayersByTeam(teamId);
                throw;
            }
        }

        public List<Player> GetAll()
        {
            try
            {
                return _dal.GetAll();
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.Players();
                throw;
            }
        }

        public List<Player> Search(string keyword, int? teamId, string position)
        {
            try
            {
                return _dal.Search(keyword, teamId, position);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.SearchPlayers(keyword, teamId, position);
                throw;
            }
        }

        public Player GetByID(int id)
        {
            try
            {
                return _dal.GetByID(id);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.PlayerById(id);
                throw;
            }
        }

        public int Add(Player p)
        {
            Validate(p);
            var count = _dal.CountByTeam(p.TeamID);
            if (count >= 10)
                throw new InvalidOperationException(
                    "Đội đã đủ 10 người chơi - không thể thêm. (BR-PLAYER-02)");

            try { return _dal.Insert(p); }
            catch (System.Data.SqlClient.SqlException ex)
            { throw new InvalidOperationException(ex.Message, ex); }
        }

        public void Update(Player p) { Validate(p); _dal.Update(p); }
        public void Remove(int id) { _dal.Delete(id); }
        public int CountByTeam(int teamId)
        {
            try
            {
                return _dal.CountByTeam(teamId);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.PlayersByTeam(teamId).Count;
                throw;
            }
        }

        private static void Validate(Player p)
        {
            if (p == null) throw new ArgumentNullException("p");
            if (string.IsNullOrWhiteSpace(p.PlayerName))
                throw new ArgumentException("Tên người chơi không được trống.");
            if (p.TeamID <= 0) throw new ArgumentException("Phải chọn đội.");
        }
    }
}
