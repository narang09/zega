using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccountModel : ZegaModel
    {
        public AccountModel()
        {
            CustomFields = new();
        }
        public string ShortName { get; set; }
        public CommonModel Model { get; set; }
        public Dictionary<string, object> CustomFields { get; set; }
        public CommonModel Manager { get; set; }
        public AccountBrokerAccountModel DefaultBrokerAccount { get; set; }
        public decimal TotalMarketValue { get; set; }
        public AccountType TaxStatus { get; set; }
    }
}
