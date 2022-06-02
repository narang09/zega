using NHibernate;
using ZegaFinancials.Nhibernate.Dao.Interface.Admin;
using ZegaFinancials.Nhibernate.Entities.Admin;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Admin
{
    public class GlobalSettingsDao : NHibernateDao<GlobalSettings>, IGlobalSettingsDao
    {
        public GlobalSettingsDao(ISession session) : base(session) { }


    }
}
