using JetBrains.Annotations;
using NHibernate;
using ZegaFinancials.Nhibernate.Dao.Interface.SpecificationsImport;
using ZegaFinancials.Nhibernate.Entities.Import;

namespace ZegaFinancials.Nhibernate.Dao.Impl.SpecificationsImport
{
    public class ImportProfileDao : NHibernateDao<ImportProfile>, IImportProfileDao
    {
        public ImportProfileDao(ISession session) : base(session) { }

        public ImportProfile GetBy([NotNull] int profileId)
        {
            var query = _session.CreateQuery(@"      
FROM ImportProfile WHERE Id =:profileId")
.SetParameter("profileId", profileId);

            return query.UniqueResult<ImportProfile>();
        }
        public int GetProfileId()
        {
            var query = _session.CreateQuery("Select Id FROM ImportProfile");

            return query.UniqueResult<int>();
        }       

    }
}
