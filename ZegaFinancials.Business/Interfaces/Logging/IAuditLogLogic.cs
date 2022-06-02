using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Business.Interfaces.Logging
{
    public interface IAuditLogLogic
    {
        IEnumerable<AuditLog> GetAuditLogByFilter(DataGridFilterModel model, string userLogin, bool isAdmin, out int count);
        void Log(EntityType type, string message);
        void Log(EntityType type, string message, int userId);
    }
}
