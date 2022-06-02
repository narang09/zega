using System;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Models.Accounts
{
    public class AccounWithdrawlInfoModelcs : ZegaModel
    {
        public decimal? Withdrawl_Amount { get; set; }
        public DateTime? Withdrawl_Date { get; set; }
        public Frequency Withdrawl_Frequency { get; set; }
        public WithdrawlorDepositStatus Future_Withdrawal { get; set; }
        public WithdrawlorDepositStatus One_Time_Withdrawal { get; set; }
        public decimal? One_Time_Withdrawal_Amount { get; set; }
        public DateTime? One_Time_Withdrawal_Date { get; set; }
        public string One_Time_WithdrawalValue { get { return EnumFunctions.GetNameEnumByValue<WithdrawlorDepositStatus>((int)One_Time_Withdrawal); } }
        public string Future_Withdrawal_StatusValue { get { return EnumFunctions.GetNameEnumByValue<WithdrawlorDepositStatus>((int)Future_Withdrawal); } }
        public string Withdrawal_FrequencyValue { get { return EnumFunctions.GetNameEnumByValue<Frequency>((int)Withdrawl_Frequency); } }
       
    }
}
