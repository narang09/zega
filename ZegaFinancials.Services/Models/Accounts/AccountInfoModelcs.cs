
using System;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccountInfoModelcs : ZegaModel
    {
        public AccountInfoModelcs()
        {
            accountBasicDetails = new();
            accountZegaCustomFieldsModel = new();
            accountDepositsInfoModelcs = new();
            accounWithdrawlInfoModelcs = new();
            Model = new();
        }
      
        public CommonModel Model { get; set; }
        public AccountBasisDetailsModel accountBasicDetails { get; set; }
        public AccountZegaCustomFieldsModel accountZegaCustomFieldsModel { get; set; }
        public AccountDepositsInfoModelcs accountDepositsInfoModelcs { get; set; }
        public AccounWithdrawlInfoModelcs accounWithdrawlInfoModelcs { get; set; }


    }
}
