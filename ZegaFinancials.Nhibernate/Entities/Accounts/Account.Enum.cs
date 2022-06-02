using System.ComponentModel;

namespace ZegaFinancials.Nhibernate.Entities.Accounts
{
    public enum AccountType
    {
        [Description("Not Set")]
        NotSet = 0,
        ROTH = 1,
        IRA = 2,
        Taxable = 3,
        Qualified = 4,
        Other = 5
    }
    public enum AccountStatus
    {
        Ready = 0,
        NotReady = 1
    }
    public enum WithdrawlorDepositStatus
    {
        No = 0,
        Yes = 1
    }

    public enum WithdrawalStatus
    {
        InActive = 0,
        Active = 1
    }

    public enum Frequency
    { 
        [Description("Not Set")]
        NotSet = 0,
        OneTime = 1,
        Monthly = 2,
        Weekly = 3,
        Quarterly = 4
    }

    public enum Broker
    {
        Other = 0,
        TDA = 1
    }
}
