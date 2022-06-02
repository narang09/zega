using ZegaFinancials.Nhibernate.Entities.Import.ImportSettings;

namespace ZegaFinancials.Nhibernate.Dao.Interface.SpecificationsImport
{
    public interface ITDVeoSettingsDao : INHibernateDao<TDAmertideSettings>
    {
        TDAmertideSettings GetByProfileId(int profileId);
    }
}
