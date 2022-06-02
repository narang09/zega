using System;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccountDepositsInfoModelcs : ZegaModel
    {
        public decimal? Deposit_Amount { get; set; }
        public DateTime? Deposit_Date { get; set; }
        public WithdrawlorDepositStatus Deposit_Status { get; set; }
        public Frequency Deposit_Frequency { get; set; }
        public string Deposit_StatusValue { get { return EnumFunctions.GetNameEnumByValue<WithdrawlorDepositStatus>((int)Deposit_Status); } }
        public string Deposit_FrequencyValue { get { return EnumFunctions.GetNameEnumByValue<Frequency>((int)Deposit_Frequency); } }
    }
}
