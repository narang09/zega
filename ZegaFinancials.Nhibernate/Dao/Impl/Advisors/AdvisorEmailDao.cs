
using NHibernate;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Advisors;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Advisors
{
    public class AdvisorEmailDao : NHibernateDao<UserEmail>, IAdvisorEmailDao
    {
        public AdvisorEmailDao(ISession session) : base(session)
        {
        }

    }
}