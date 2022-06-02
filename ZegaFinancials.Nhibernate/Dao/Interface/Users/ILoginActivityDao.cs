
using ZegaFinancials.Nhibernate.Entities.Logging;

namespace ZegaFinancials.Nhibernate.Dao.Interface.Users
{
    public interface ILoginActivityDao : INHibernateDao<LoginActivity>
    {
        LoginActivity GetLoginActivity(int userId , string sessionId);
        void DeleteBySessionId(string SessionId);
        void DeleteAllLoginActivity();
    }
}
