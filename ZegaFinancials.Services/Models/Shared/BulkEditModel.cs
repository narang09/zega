using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Services.Models.Shared
{
    public class BulkEditModel : ZegaModel
    {
        public BulkEditModel()
        {
            DataStoreIds = new();
        }

        public List<int> DataStoreIds { get; set; }
        public string GridName { get; set; }
        // For Users Bulk Change
        public Status Status { get; set; }
        // For Accounts Bulk Change
        public AccountType? AccountType { get; set; }
        public AccountStatus? AccountStatus { get; set; }
        public int ModelId { get; set; }
        public List<int> StrategyIds { get; set; }
 
    }
}
