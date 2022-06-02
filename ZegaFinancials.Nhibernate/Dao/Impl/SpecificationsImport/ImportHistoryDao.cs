using NHibernate;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.SpecificationsImport;
using ZegaFinancials.Nhibernate.Entities.Import;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support.QueryGenerator;

namespace ZegaFinancials.Nhibernate.Dao.Impl.SpecificationsImport
{
    public class ImportHistoryDao : NHibernateDao<ImportHistory>, IImportHistoryDao
    {
        public ImportHistoryDao(ISession session) : base(session) { }

        public IEnumerable<ImportHistory> GetByFilter(DataGridFilterModel model, out int count)
        {
            var sql = @"
FROM ImportHistory AS ih WHERE 1=1";

            var qg = new QueryGenerator("ih", model, new[]
                {
                new QueryReplace("TypeValue", "ih.ImportType"),
                new QueryReplace("ImportStatusValue", "ih.Status")
                },
                new Dictionary<string, string> { { "TypeValue", "ImportType" }, { "ImportStatusValue", "ImportStatus" } }
                );
            var qb = qg.ApplyBasicMultiFilters<ImportHistory>(_session, sql);
            var history = qb.GetResult<ImportHistory>(0);
            count = (int)qb.GetResult<long>(1).Single();
            return history;
        }

    }
}
