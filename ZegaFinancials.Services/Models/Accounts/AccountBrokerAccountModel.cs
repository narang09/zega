using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccountBrokerAccountModel
    {
        public string AccountName { get; set; }
        public CommonModel Broker { get; set; }
    }
}
