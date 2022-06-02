using NHibernate;
using ZegaFinancials.Nhibernate.Dao.Interface.SpecificationsImport;
using ZegaFinancials.Nhibernate.Entities.Import.ImportSettings;

namespace ZegaFinancials.Nhibernate.Dao.Impl.SpecificationsImport
{
    public class TDVeoSettingsDao : NHibernateDao<TDAmertideSettings>, ITDVeoSettingsDao
    {
        public TDVeoSettingsDao(ISession session) : base(session) { }

        public TDAmertideSettings GetByProfileId(int profileId)
        {
            return _session.CreateQuery(@"From TDAmertideSettings Where Profile.Id =: profileId")
                .SetParameter("profileId", profileId)
                .UniqueResult<TDAmertideSettings>();
        }
    }
}
