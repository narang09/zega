
using NHibernate;
using ZegaFinancials.Nhibernate.Dao.Interface.Advisors;
using ZegaFinancials.Nhibernate.Entities.Advisors;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Advisors
{
    public class AdvisorModelDao : NHibernateDao<AdvisorModel>, IAdvisorModelDao
    {
        public AdvisorModelDao(ISession session) : base(session)
    {
    }
    }
}
