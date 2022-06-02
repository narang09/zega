using JetBrains.Annotations;
using ZegaFinancials.Nhibernate.Entities.Import;

namespace ZegaFinancials.Nhibernate.Dao.Interface.SpecificationsImport
{
    public interface IImportProfileDao : INHibernateDao<ImportProfile>
    {
        /// <summary>
        /// Get the Profile by Id.
        /// </summary>
        /// <param name="profileId">Profile Id</param>
        /// <returns></returns>
        ImportProfile GetBy([NotNull] int profileId);

        /// <summary>
        /// Get the Profile Id.
        /// </summary>
        /// <returns></returns>
        int GetProfileId();       
    }
}
