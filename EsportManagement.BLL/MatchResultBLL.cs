using System;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class MatchResultBLL
    {
        private readonly MatchResultDAL _dal = new MatchResultDAL();

        public MatchResult GetByMatch(int matchId) => _dal.GetByMatch(matchId);

        public void EnterResult(int matchId, int score1, int score2)
        {
            if (score1 < 0 || score2 < 0)
                throw new ArgumentException("Điểm số phải lớn hơn hoặc bằng 0. (FR-RESULT-04)");

            // SP đã xử lý transaction + recalc ranking
            try
            {
                _dal.EnterResult(matchId, score1, score2);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        public void ConfirmResult(int matchId)
        {
            try
            {
                _dal.ConfirmResult(matchId);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
    }
}
