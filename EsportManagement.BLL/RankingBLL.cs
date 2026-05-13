using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class RankingBLL
    {
        private readonly RankingDAL _dal = new RankingDAL();

        public List<Ranking> GetByTournament(int tournamentId)
        {
            try
            {
                return _dal.GetByTournament(tournamentId);
            }
            catch (System.Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.RankingsByTournament(tournamentId);
                throw;
            }
        }

        public void Recalculate(int tournamentId)
        {
            _dal.Recalculate(tournamentId);
        }
    }
}
