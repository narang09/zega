
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Models.Logging;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Interfaces.Logging
{
    public interface IAuditLogService
    {
        DataGridModel LoadAuditLogByFilter(DataGridFilterModel dataGridFilterModel,UserContextModel userContext);
        LogModel[] FilterNSortClassesTime(DataGridFilterModel filter);
    }
}
