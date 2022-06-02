using ZegaFinancials.Services.Models.Accounts;
using ZegaFinancials.Services.Models.Logging;
using ZegaFinancials.Services.Models.Models;
using ZegaFinancials.Services.Models.Strategies;
using ZegaFinancials.Services.Models.RepCodes;
using ZegaFinancials.Services.Models.Users;
using ZegaFinancials.Services.Models.Import;
using ZegaFinancials.Services.Models.Dashboard;

namespace ZegaFinancials.Services.Models.Shared
{
    public class DataGridModel
    {
        public AccountInfoModelcs[] Accounts { get; set; }
        public ModelModel[] Models { get; set; }
        public SleeveModel[] Sleeves { get; set; }
        public UserModel[] Advisors { get; set; }
        public RepCodeModel[] RepCodes { get; set; }
        public AuditLogModel[] AuditLogs { get; set; }
        public StrategyListingModel[] Strategies { get; set; }
        public ImportHistoryModel[] ImportHistory { get; set; }
        public DashboardAdminModel[] AdvisorsList { get; set; }
        public int TotalRecords { get; set; }
    }
}
