using NHibernate;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Logging;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Users
{
    public class LoginActivityDao : NHibernateDao<LoginActivity>,ILoginActivityDao
    {
        public LoginActivityDao(ISession session) : base(session) { }

        public void DeleteAllLoginActivity()
        {
            var sql = @"Delete From LoginActivity ";
              _session.CreateQuery(sql).ExecuteUpdate();
        }
        public void DeleteBySessionId(string sessionId)
        {
            var sql = @"Delete From LoginActivity Where SessionId =:sessionId";
              _session.CreateQuery(sql).SetParameter("sessionId", sessionId).ExecuteUpdate();
        }

        public LoginActivity GetLoginActivity( int userId , string sessionId)
        {
            var sql = @"From LoginActivity Where User =:userId and SessionId =:sessionId";
            var LoginActivity = _session.CreateQuery(sql).SetParameter("userId", userId).SetParameter("sessionId", sessionId).List<LoginActivity>().FirstOrDefault();
            return LoginActivity;
        }
    }
}
