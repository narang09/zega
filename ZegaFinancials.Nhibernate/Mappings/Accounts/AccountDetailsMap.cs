using ZegaFinancials.Nhibernate.Entities.Accounts;

namespace ZegaFinancials.Nhibernate.Mappings.Accounts
{
    public class AccountDetailsMap : ZegaMap<AccountDetails>
    {
        public AccountDetailsMap()
        {
            References(x => x.Account).Column("Account").ForeignKey("FK_AccountDetails_Account").UniqueKey("UK_AccountDetails_Account").Index("IDX_AccountDetails_Account").Cascade.All();
            Map(x => x.AccountValue);
            Map(x => x.AccountType).CustomType<AccountType>();
            Map(x => x.SBuyingPower);
            Map(x => x.OBuyingPower);
            Map(x => x.CashNetBal);
            Map(x => x.B2_Allocation_Start_Date);
            Map(x => x.CashEq);
            Map(x => x.VeoImportDate);
            Map(x => x.Notes).Length(4001);
            Map(x => x.Future_Withdrawal).CustomType<WithdrawlorDepositStatus>();
            Map(x => x.C1_Withdrawal_Amount);
            Map(x => x.C4_Withdrawal_Frequency).CustomType<Frequency>();
            Map(x => x.C2_Withdrawal_Date);
            Map(x => x.One_Time_Withdrawal).CustomType<WithdrawlorDepositStatus>();
            Map(x => x.One_Time_Withdrawal_Amount);
            Map(x => x.One_Time_Withdrawal_Date);
            Map(x => x.D3_Deposit_Status).CustomType<WithdrawlorDepositStatus>();
            Map(x => x.D1_Deposit);
            Map(x => x.D4_Deposit_Frequency).CustomType<Frequency>();
            Map(x => x.D2_Deposit_Date);
            Map(x => x.Z2_ZEGA_Confirmed).CustomType<WithdrawlorDepositStatus>();
            Map(x => x.Z3_ZEGA_Alert_Date);
            Map(x => x.Z4_ZEGA_Notes).Length(4001);
        }
    }
}
