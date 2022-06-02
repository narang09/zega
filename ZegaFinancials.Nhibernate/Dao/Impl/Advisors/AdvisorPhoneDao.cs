
using NHibernate;
using ZegaFinancials.Nhibernate.Dao.Interface.Advisors;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Advisors
{
    public class AdvisorPhoneDao : NHibernateDao<UserPhone>, IAdvisorPhoneDao
    {
        public AdvisorPhoneDao(ISession session) : base(session)
        {
        }
    }
}
