using System;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Accounts
{
    [Logable(EntityType.Account)]
    public class AccountDetails : ZegaEntity
    {
        public virtual Account Account { get; set; }
        public virtual decimal? AccountValue { get; set; }
        public virtual AccountType AccountType { get; set; }
        public virtual decimal? SBuyingPower { get; set; }
        public virtual decimal? OBuyingPower { get; set; }
        public virtual decimal? CashNetBal { get; set; }
        public virtual DateTime? B2_Allocation_Start_Date { get; set; }
        public virtual decimal? CashEq { get; set; }
        public virtual DateTime? VeoImportDate { get; set; }
        public virtual string Notes { get; set; }
        public virtual WithdrawlorDepositStatus Future_Withdrawal { get; set; }
        public virtual decimal? C1_Withdrawal_Amount { get; set; }
        public virtual Frequency C4_Withdrawal_Frequency { get; set; }
        public virtual DateTime? C2_Withdrawal_Date { get; set; }
        public virtual WithdrawlorDepositStatus One_Time_Withdrawal { get; set; }
        public virtual decimal? One_Time_Withdrawal_Amount { get; set; }
        public virtual DateTime? One_Time_Withdrawal_Date { get; set; }
        public virtual WithdrawlorDepositStatus D3_Deposit_Status { get; set; }
        public virtual decimal? D1_Deposit { get; set; }
        public virtual Frequency D4_Deposit_Frequency { get; set; }
        public virtual DateTime? D2_Deposit_Date { get; set; }
        public virtual WithdrawlorDepositStatus Z2_ZEGA_Confirmed { get; set; }
        public virtual DateTime? Z3_ZEGA_Alert_Date { get; set; }
        public virtual string Z4_ZEGA_Notes { get; set; }
        

        [LogSignature]
        public virtual string AccountName
        {
            get
            {
                return string.Format("Account Number = {0}",
                      Account.Number
                    );
            }
        }
    }
}
