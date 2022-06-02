using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Dao.Interface.Logging
{
    public interface IAuditLogDao : INHibernateDao<AuditLog>
    {
        IEnumerable<AuditLog> GetAuditLogByFilter(DataGridFilterModel model, string userLogin, bool IsAdmin, out int count);
    }
}
