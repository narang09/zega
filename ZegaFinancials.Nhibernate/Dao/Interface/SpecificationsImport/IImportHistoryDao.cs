using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Import;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Dao.Interface.SpecificationsImport
{
    public interface IImportHistoryDao : INHibernateDao<ImportHistory>
    {
        IEnumerable<ImportHistory> GetByFilter(DataGridFilterModel model, out int count);
    }
}
