using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class RankingBLL
    {
        private readonly RankingDAL _dal = new RankingDAL();

        public List<Ranking> GetByTournament(int tournamentId) => _dal.GetByTournament(tournamentId);
        public void Recalculate(int tournamentId) => _dal.Recalculate(tournamentId);
    }
}
