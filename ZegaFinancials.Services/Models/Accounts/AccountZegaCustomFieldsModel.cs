using System;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccountZegaCustomFieldsModel : ZegaModel
    {
        public WithdrawlorDepositStatus Zega_Confirmed { get; set; }
        public DateTime? Zega_Alert_Date { get; set; }
        public string Zega_Notes { get; set; }
        public string Zega_ConfirmedValue  { get { return EnumFunctions.GetNameEnumByValue<WithdrawlorDepositStatus>((int)Zega_Confirmed); } }
    }
}
    
