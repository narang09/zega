using System;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccountBasisDetailsModel : ZegaModel
    {
        public string Number { get; set; }
        public virtual DateTime? VeoImportDate { get; set; }
        public decimal? AccountValue { get; set; }
        public string ClientName { get; set; }
        public string AdvisorsName { get; set; }
        public AccountType AccountType { get; set; }
        public decimal? SBuyingPower { get; set; }
        public decimal? OBuyingPower { get; set; }
        public CommonModel RepCode { get; set; }
        // public BuyingPower BuyingPower { get; set; }
        public decimal? CashNetBal { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public DateTime? AllocationDate { get; set; }
        public string Notes { get; set; }
        public virtual decimal? CashEq { get; set; }
        public Broker Broker { get; set; }
        public string BrokerValue { get { return EnumFunctions.GetNameEnumByValue<Broker>((int)Broker); } }
        public string AccountStatusValue { get { return EnumFunctions.GetNameEnumByValue<AccountStatus>((int)AccountStatus); } }
        public string AccountTypeValue { get { return EnumFunctions.GetNameEnumByValue<AccountType>((int)AccountType); } }
    }
}
