
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        DataGridModel GetAdvisorsById(UserContextModel userContext);
    }
}
