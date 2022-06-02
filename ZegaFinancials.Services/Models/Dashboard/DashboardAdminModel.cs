
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Services.Models.Import;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Models.Dashboard
{
   public class DashboardAdminModel : ZegaModel
    {
        public virtual long AccountsCount { get; set; }
        public virtual long ModelsCount { get; set; }
        public virtual long ClientsCount { get; set; }
        public Status Status { get; set; }
        public string StatusValue { get { return EnumFunctions.GetNameEnumByValue<Status>((int)Status); } }
        public virtual ImportHistoryModel ImportHistory { get; set; }
       
    }
}
